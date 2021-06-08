﻿namespace WhMgr.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json.Serialization;

    using WhMgr.Services.Alarms.Embeds;
    using WhMgr.Services.Geofence;

    /// <summary>
    /// Discord server configuration class
    /// </summary>
    public class DiscordServerConfig
    {
        /// <summary>
        /// Gets or sets the command prefix for all Discord commands
        /// </summary>
        [JsonPropertyName("commandPrefix")]
        public string CommandPrefix { get; set; }

        /// <summary>
        /// Gets or sets the emoji guild id
        /// </summary>
        [JsonPropertyName("emojiGuildId")]
        public ulong EmojiGuildId { get; set; }

        /// <summary>
        /// Gets or sets the owner id
        /// </summary>
        [JsonPropertyName("ownerId")]
        public ulong OwnerId { get; set; }

        //[JsonProperty("locale")]
        //public string Locale { get; set; }

        /// <summary>
        /// Gets or sets the donor role ID(s)
        /// </summary>
        [JsonPropertyName("donorRoleIds")]
        public List<ulong> DonorRoleIds { get; set; } = new();

        /// <summary>
        /// Gets or sets the free donor role name to assign by non-donors to get
        /// free donor access
        /// </summary>
        [JsonPropertyName("freeRoleName")]
        public string FreeRoleName { get; set; }

        /// <summary>
        /// Gets or sets the moderators of the Discord server
        /// </summary>
        [JsonPropertyName("moderatorRoleIds")]
        public List<ulong> ModeratorRoleIds { get; set; } = new();

        /// <summary>
        /// Gets or sets the Discord bot token
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the associated alarms file for the Discord server
        /// </summary>
        [JsonPropertyName("alarms")]
        public string AlarmsFile { get; set; }

        /// <summary>
        /// Gets or sets the list of Geofence files to use for the Discord server
        /// (in addition to the common ones)
        /// </summary>
        [JsonPropertyName("geofences")]
        public string[] GeofenceFiles { get; set; }

        [JsonIgnore]
        public List<Geofence> Geofences { get; } = new();

        /// <summary>
        /// Gets or sets whether to enable custom direct message subscriptions
        /// </summary>
        //[JsonPropertyName("subscriptions")]
        // TODO: public SubscriptionsConfig Subscriptions { get; set; }

        /// <summary>
        /// Gets or sets the GeofenceRoles config to use with assigning geofence 
        /// roles to see different sections
        /// </summary>
        [JsonPropertyName("geofenceRoles")]
        public GeofenceRolesConfig GeofenceRoles { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("questsPurge")]
        public QuestsPurgeConfig QuestsPurge { get; set; } = new();

        /// <summary>
        /// Gets or sets the nests config to use with reporting current nests
        /// </summary>
        [JsonPropertyName("nests")]
        public NestsConfig Nests { get; set; } = new();

        /// <summary>
        /// Gets or sets the shiny stats configuration class
        /// </summary>
        //[JsonPropertyName("shinyStats")]
        // TODO: public ShinyStatsConfig ShinyStats { get; set; }

        /// <summary>
        /// Gets or sets the icon style for messages on the Discord server
        /// </summary>
        [JsonPropertyName("iconStyle")]
        public string IconStyle { get; set; }

        /// <summary>
        /// Gets or sets the bot channel ID(s)
        /// </summary>
        [
            Obsolete("Not used"),
            JsonPropertyName("botChannelIds"),
        ]
        public List<ulong> BotChannelIds { get; set; } = new();

        /// <summary>
        /// Gets or sets the Discord bot's custom status
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the alerts file to use with direct message subscriptions
        /// </summary>
        [JsonPropertyName("dmAlertsFile")]
        public string DmAlertsFile { get; set; }

        /// <summary>
        /// Gets or sets the Discord embed colors to use for each message type
        /// </summary>
        //[JsonPropertyName("embedColors")]
        //public DiscordEmbedColorConfig DiscordEmbedColors { get; set; }

        /// <summary>
        /// Gets or sets the direct message alerts class to use for subscriptions
        /// </summary>
        [JsonIgnore]
        public EmbedMessage DmEmbeds { get; set; }

        /// <summary>
        /// Instantiate a new <see cref="DiscordServerConfig"/> class
        /// </summary>
        public DiscordServerConfig()
        {
            //Locale = "en";
            IconStyle = "Default";
            //ShinyStats = new ShinyStatsConfig();
            //Subscriptions = new SubscriptionsConfig();
            DmAlertsFile = "default.json";
            //DiscordEmbedColors = new DiscordEmbedColorConfig();

            LoadDmEmbed();
        }

        public void LoadDmEmbed()
        {
            var path = Path.Combine(Strings.EmbedsFolder, DmAlertsFile);
            DmEmbeds = Config.LoadInit<EmbedMessage>(path);
        }
    }
}