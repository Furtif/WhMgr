namespace WhMgr
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using DSharpPlus;
    using DSharpPlus.Entities;
    using DSharpPlus.EventArgs;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.OpenApi.Models;

    using WhMgr.Configuration;
    using WhMgr.Data;
    using WhMgr.Data.Contexts;
    using WhMgr.HostedServices;
    using WhMgr.Queues;
    using WhMgr.Services.Alarms;
    using WhMgr.Services.Alarms.Models;
    using WhMgr.Services.Cache;
    using WhMgr.Services.Discord;
    using WhMgr.Services.Geofence;
    using WhMgr.Services.Subscriptions;
    using WhMgr.Services.Webhook;
    using WhMgr.Utilities;

    public class Startup
    {
        private readonly Dictionary<ulong, DiscordClient> _discordClients;
        private readonly Dictionary<ulong, ChannelAlarmsManifest> _alarms;
        private readonly ConfigHolder _config;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var configPath = Path.Combine(Environment.CurrentDirectory, Strings.BasePath + Strings.ConfigFileName);
            var config = Config.Load(configPath);
            if (config == null)
            {
                Console.WriteLine($"Failed to load config {configPath}.");
                return;
            }
            config.FileName = configPath;
            config.LoadDiscordServers();
            _config = new ConfigHolder(config);
            _alarms = ChannelAlarmsManifest.LoadAlarms(config.Servers);
            _discordClients = new Dictionary<ulong, DiscordClient>();

            IconFetcher.Instance.SetIconStyles(_config.Instance.IconStyles);

            // Build the dependency collection which will contain our objects that can be globally used within each command module
            var servicesCol = new ServiceCollection()
                //.AddSingleton(typeof(InteractivityExtension), interactivity)
                .AddSingleton(typeof(ConfigHolder), _config);
            //.AddSingleton(typeof(StripeService), new StripeService(_whConfig.Instance.StripeApiKey))
            //.AddSingleton(typeof(Osm.OsmManager), new Osm.OsmManager())
            //if (_subProcessor != null)
            {
                //servicesCol.AddSingleton(typeof(SubscriptionProcessor), _subProcessor ?? new SubscriptionProcessor(_servers, _whConfig, _whm));
            }
            var services = servicesCol.BuildServiceProvider();
            InitializeDiscord(services).ConfigureAwait(false)
                                       .GetAwaiter()
                                       .GetResult();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IGeofenceService>(new GeofenceService());
            // let DI create and manage the singleton instance
            services.AddSingleton<IAlarmControllerService, AlarmControllerService>();
            services.AddScoped<NotificationQueue, NotificationQueue>();
            services.AddSingleton<ISubscriptionProcessorQueueService, SubscriptionProcessorQueueService>();
            services.AddSingleton<ISubscriptionProcessorService, SubscriptionProcessorService>();
            services.AddSingleton<ISubscriptionManagerService, SubscriptionManagerService>();
            services.AddSingleton<IWebhookProcessorService, WebhookProcessorService>();
            services.AddSingleton<ChannelAlarmsManifest, ChannelAlarmsManifest>();
            services.AddSingleton(_config);
            services.AddSingleton(_alarms);
            services.AddSingleton(_discordClients);
            services.AddSingleton<IMapDataCache, MapDataCache>();

            services.AddHostedService<QuestPurgeHostedService>();

            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseMySql(
                    _config.Instance.Database.Main.ToString(),
                    ServerVersion.AutoDetect(_config.Instance.Database.Main.ToString())
                ), ServiceLifetime.Singleton
            );

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    _config.Instance.Database.Main.ToString(),
                    ServerVersion.AutoDetect(_config.Instance.Database.Main.ToString())
                ), ServiceLifetime.Scoped
            );

            services.AddDbContextFactory<MapDbContext>(options =>
                options.UseMySql(
                    _config.Instance.Database.Scanner.ToString(),
                    ServerVersion.AutoDetect(_config.Instance.Database.Scanner.ToString())
                ), ServiceLifetime.Singleton
            );

            services.AddDbContext<MapDbContext>(options =>
                options.UseMySql(
                    _config.Instance.Database.Scanner.ToString(),
                    ServerVersion.AutoDetect(_config.Instance.Database.Scanner.ToString())
                ), ServiceLifetime.Scoped
            );

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WhMgr", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WhMgr v1"));
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private async Task InitializeDiscord(ServiceProvider services)
        {
            foreach (var (guildId, guildConfig) in _config.Instance.Servers)
            {
                Console.WriteLine($"Configured Discord server {guildId}");
                var client = DiscordClientFactory.CreateDiscordClient(guildConfig, services);
                client.Ready += Client_Ready;
                client.GuildAvailable += Client_GuildAvailable;
                if ((guildConfig.GeofenceRoles?.Enabled ?? false) &&
                    (guildConfig.GeofenceRoles?.AutoRemove ?? false))
                {
                    client.GuildMemberUpdated += Client_GuildMemberUpdated;
                }
                client.MessageCreated += Client_MessageCreated;
                client.ClientErrored += Client_ClientErrored;
                if (!_discordClients.ContainsKey(guildId))
                {
                    _discordClients.Add(guildId, client);
                    await client.ConnectAsync();
                }

                // Wait 3 seconds between initializing each Discord client
                await Task.Delay(3000);
            }
        }

        #region Discord Events

        private Task Client_Ready(DiscordClient client, ReadyEventArgs e)
        {
            Console.WriteLine($"------------------------------------------");
            Console.WriteLine($"[DISCORD] Connected.");
            Console.WriteLine($"[DISCORD] ----- Current Application");
            Console.WriteLine($"[DISCORD] Name: {client.CurrentApplication.Name}");
            Console.WriteLine($"[DISCORD] Description: {client.CurrentApplication.Description}");
            var owners = string.Join(", ", client.CurrentApplication.Owners.Select(x => $"{x.Username}#{x.Discriminator}"));
            Console.WriteLine($"[DISCORD] Owner: {owners}");
            Console.WriteLine($"[DISCORD] ----- Current User");
            Console.WriteLine($"[DISCORD] Id: {client.CurrentUser.Id}");
            Console.WriteLine($"[DISCORD] Name: {client.CurrentUser.Username}#{client.CurrentUser.Discriminator}");
            Console.WriteLine($"[DISCORD] Email: {client.CurrentUser.Email}");
            Console.WriteLine($"------------------------------------------");

            return Task.CompletedTask;
        }

        private async Task Client_GuildAvailable(DiscordClient client, GuildCreateEventArgs e)
        {
            // If guild is in configured servers list then attempt to create emojis needed
            if (_config.Instance.Servers.ContainsKey(e.Guild.Id))
            {
                // Create default emojis
                await CreateEmojis(e.Guild.Id);

                // Set custom bot status if guild is in config server list
                var status = _config.Instance.Servers[e.Guild.Id].Bot?.Status ?? $"v{Strings.BotVersion}";
                await client.UpdateStatusAsync(new DiscordActivity(status, ActivityType.Playing), UserStatus.Online);
            }
        }

        private async Task Client_GuildMemberUpdated(DiscordClient client, GuildMemberUpdateEventArgs e)
        {
            if (!_config.Instance.Servers.ContainsKey(e.Guild.Id))
                return;

            var server = _config.Instance.Servers[e.Guild.Id];
            var hasBefore = e.RolesBefore.FirstOrDefault(x => server.DonorRoleIds.Contains(x.Id)) != null;
            var hasAfter = e.RolesAfter.FirstOrDefault(x => server.DonorRoleIds.Contains(x.Id)) != null;

            // Check if donor role was removed
            if (hasBefore && !hasAfter)
            {
                Console.WriteLine($"Member {e.Member.Username} ({e.Member.Id}) donor role removed, removing any city roles...");
                // If so, remove all city/geofence/area roles
                var areaRoles = server.Geofences.Select(x => x.Name.ToLower());
                foreach (var roleName in areaRoles)
                {
                    var role = e.Guild.Roles.FirstOrDefault(x => x.Value.Name == roleName).Value; // TODO: GetRoleFromName(roleName);
                    if (role == null)
                    {
                        Console.WriteLine($"Failed to get role by name {roleName}");
                        continue;
                    }
                    await e.Member.RevokeRoleAsync(role, "No longer a supporter/donor");
                }
                Console.WriteLine($"All city roles removed from member {e.Member.Username} ({e.Member.Id})");
            }
        }

        private async Task Client_MessageCreated(DiscordClient client, MessageCreateEventArgs e)
        {
            // Ignore bot messages
            if (e.Author.Id == client.CurrentUser.Id || e.Author.IsBot)
                return;

            if (!_config.Instance.Servers.ContainsKey(e.Guild?.Id ?? 0))
                return;

            var guildConfig = _config.Instance.Servers[e.Guild.Id];
            if (guildConfig.Bot.ChannelIds.Count > 0 && !guildConfig.Bot.ChannelIds.Contains(e.Channel.Id))
                return;

            // TODO: Handle command manually
            //await _commands.HandleCommandsAsync(e);
            Console.WriteLine($"handle command");
            await Task.CompletedTask;
        }

        private async Task Client_ClientErrored(DiscordClient client, ClientErrorEventArgs e)
        {
            Console.WriteLine(e.Exception);

            await Task.CompletedTask;
        }

        #endregion

        #region Discord Emojis

        private async Task CreateEmojis(ulong guildId)
        {
            if (!_discordClients.ContainsKey(guildId))
            {
                Console.WriteLine($"Discord client not ready yet to create emojis for guild {guildId}");
                return;
            }

            var server = _config.Instance.Servers[guildId];
            var client = _discordClients[guildId];
            if (!(client.Guilds?.ContainsKey(server.Bot.EmojiGuildId) ?? false))
            {
                Console.WriteLine($"Bot not in emoji server {server.Bot.EmojiGuildId}");
                return;
            }

            var guild = client.Guilds[server.Bot.EmojiGuildId];
            foreach (var emoji in Strings.EmojiList)
            {
                try
                {
                    var emojis = await guild.GetEmojisAsync();
                    var emojiExists = emojis.FirstOrDefault(x => string.Compare(x.Name, emoji, true) == 0);
                    if (emojiExists == null)
                    {
                        Console.WriteLine($"Emoji {emoji} doesn't exist, creating...");

                        var emojiPath = Path.Combine(Strings.EmojisFolder, emoji + ".png");
                        if (!File.Exists(emojiPath))
                        {
                            Console.WriteLine($"Unable to find emoji file at {emojiPath}, skipping...");
                            continue;
                        }

                        var fs = new FileStream(emojiPath, FileMode.Open, FileAccess.Read);
                        await guild.CreateEmojiAsync(emoji, fs, null, $"Missing `{emoji}` emoji.");

                        Console.WriteLine($"Emoji {emoji} created successfully.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            await CacheGuildEmojisList();
        }

        private async Task CacheGuildEmojisList()
        {
            foreach (var (guildId, serverConfig) in _config.Instance.Servers)
            {
                var emojiGuildId = serverConfig.Bot.EmojiGuildId;
                if (!_discordClients.ContainsKey(guildId))
                    continue;

                var guild = _discordClients[guildId];
                if (!guild.Guilds.ContainsKey(emojiGuildId))
                    continue;

                var emojiGuild = guild.Guilds[emojiGuildId];
                var emojis = await emojiGuild.GetEmojisAsync();
                foreach (var name in Strings.EmojiList)
                {
                    var emoji = emojis.FirstOrDefault(x => string.Compare(x.Name, name, true) == 0);
                    if (emoji == null)
                        continue;

                    if (!MasterFile.Instance.Emojis.ContainsKey(emoji.Name))
                    {
                        MasterFile.Instance.Emojis.Add(emoji.Name, emoji.Id);
                    }
                }
            }
        }

        #endregion
    }
}