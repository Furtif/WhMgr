﻿namespace WhMgr.Services.Webhook
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.Logging;

    using WhMgr.Extensions;
    using WhMgr.Services.Alarms;
    using WhMgr.Services.Cache;
    using WhMgr.Services.Subscriptions;
    using WhMgr.Services.Webhook.Cache;
    using WhMgr.Services.Webhook.Models;

    public class WebhookProcessorService : IWebhookProcessorService
    {
        private readonly ILogger<WebhookProcessorService> _logger;
        private readonly IAlarmControllerService _alarmsService;
        private readonly ISubscriptionProcessorService _subscriptionService;
        private readonly IMapDataCache _mapDataCache;

        private readonly Dictionary<string, ScannedPokemon> _processedPokemon;
        private readonly Dictionary<string, ScannedRaid> _processedRaids;
        private readonly Dictionary<string, ScannedQuest> _processedQuests;

        #region Properties

        public bool Enabled { get; set; }

        public bool CheckForDuplicates { get; set; }

        public ushort DespawnTimerMinimumMinutes { get; set; }

        #endregion

        public WebhookProcessorService(
            ILogger<WebhookProcessorService> logger,
            IAlarmControllerService alarmsService,
            ISubscriptionProcessorService subscriptionService,
            IMapDataCache mapDataCache)
        {
            _logger = logger;
            _alarmsService = alarmsService;
            _subscriptionService = subscriptionService;
            _mapDataCache = mapDataCache;

            _processedPokemon = new Dictionary<string, ScannedPokemon>();
            _processedRaids = new Dictionary<string, ScannedRaid>();
            _processedQuests = new Dictionary<string, ScannedQuest>();

            Start();
        }

        #region Public Methods

        public void Start()
        {
            Enabled = true;
        }

        public void Stop()
        {
            Enabled = false;
        }

        public void ParseData(List<WebhookPayload> payloads)
        {
            if (!Enabled) return;

            _logger.LogInformation($"Received {payloads.Count:N0} webhook payloads");
            for (var i = 0; i < payloads.Count; i++)
            {
                var payload = payloads[i];
                switch (payload.Type)
                {
                    case WebhookHeaders.Pokemon:
                        ProcessPokemon(payload.Message);
                        break;
                    case WebhookHeaders.Raid:
                        ProcessRaid(payload.Message);
                        break;
                    case WebhookHeaders.Quest:
                        ProcessQuest(payload.Message);
                        break;
                    case WebhookHeaders.Invasion:
                    case WebhookHeaders.Pokestop:
                        ProcessPokestop(payload.Message);
                        break;
                    case WebhookHeaders.Gym:
                    case WebhookHeaders.GymDetails:
                        ProcessGym(payload.Message);
                        break;
                    case WebhookHeaders.Weather:
                        // TODO: Weather
                        break;
                }
            }
        }

        #endregion

        #region Processing Methods

        private void ProcessPokemon(dynamic message)
        {
            string json = Convert.ToString(message);
            var pokemon = json.FromJson<PokemonData>();
            if (pokemon == null)
            {
                _logger.LogWarning($"Failed to deserialize pokemon {message}, skipping...");
                return;
            }
            pokemon.SetDespawnTime();

            if (pokemon.SecondsLeft.TotalMinutes < DespawnTimerMinimumMinutes)
                return;

            if (CheckForDuplicates)
            {
                // Lock processed pokemon, check for duplicates of incoming pokemon
                lock (_processedPokemon)
                {
                    // If we have already processed pokemon, previously did not have stats, and currently does
                    // not have stats, skip.
                    if (_processedPokemon.ContainsKey(pokemon.EncounterId)
                        && (pokemon.IsMissingStats
                        || (!pokemon.IsMissingStats && !_processedPokemon[pokemon.EncounterId].IsMissingStats)))
                        return;

                    // Check if we have not processed this encounter before, is so then add
                    if (!_processedPokemon.ContainsKey(pokemon.EncounterId))
                        _processedPokemon.Add(pokemon.EncounterId, new ScannedPokemon(pokemon));

                    // Check if incoming pokemon has stats but previously processed pokemon did not and update it
                    if (!pokemon.IsMissingStats && _processedPokemon[pokemon.EncounterId].IsMissingStats)
                        _processedPokemon[pokemon.EncounterId] = new ScannedPokemon(pokemon);
                }
            }

            // Process pokemon alarms
            _alarmsService.ProcessPokemonAlarms(pokemon);
            // TODO: _subscriptionService.ProcessPokemonSubscription(pokemon);
        }

        private void ProcessRaid(dynamic message)
        {
            string json = Convert.ToString(message);
            var raid = json.FromJson<RaidData>();
            if (raid == null)
            {
                _logger.LogWarning($"Failed to deserialize raid {message}, skipping...");
                return;
            }
            raid.SetTimes();

            if (CheckForDuplicates)
            {
                // Lock processed raids, check for duplicates of incoming raid
                lock (_processedRaids)
                {
                    if (_processedRaids.ContainsKey(raid.GymId))
                    {
                        // Check if raid data matches existing scanned raids with
                        // pokemon_id, form_id, costume_id, and not expired
                        if (_processedRaids[raid.GymId].PokemonId == raid.PokemonId
                            && _processedRaids[raid.GymId].FormId == raid.Form
                            && _processedRaids[raid.GymId].CostumeId == raid.Costume
                            && _processedRaids[raid.GymId].Level == raid.Level
                            && !_processedRaids[raid.GymId].IsExpired)
                        {
                            // Processed raid already
                            return;
                        }

                        _processedRaids[raid.GymId] = new ScannedRaid(raid);
                    }
                    else
                    {
                        _processedRaids.Add(raid.GymId, new ScannedRaid(raid));
                    }
                }
            }

            // Process raid alarms
            _alarmsService.ProcessRaidAlarms(raid);
            // TODO: _subscriptionService.ProcessRaidSubscription(raid);
        }

        private void ProcessQuest(dynamic message)
        {
            string json = Convert.ToString(message);
            var quest = json.FromJson<QuestData>();
            if (quest == null)
            {
                _logger.LogWarning($"Failed to deserialize quest {message}, skipping...");
                return;
            }

            if (CheckForDuplicates)
            {
                // Lock processed quests, check for duplicates of incoming quest
                lock (_processedQuests)
                {
                    if (_processedQuests.ContainsKey(quest.PokestopId))
                    {
                        if (_processedQuests[quest.PokestopId].Type == quest.Type &&
                            _processedQuests[quest.PokestopId].Rewards == quest.Rewards &&
                            _processedQuests[quest.PokestopId].Conditions == quest.Conditions)
                        {
                            // Processed quest already
                            return;
                        }

                        _processedQuests[quest.PokestopId] = new ScannedQuest(quest);
                    }
                    else
                    {
                        _processedQuests.Add(quest.PokestopId, new ScannedQuest(quest));
                    }
                }
            }

            // Process quest alarms
            _alarmsService.ProcessQuestAlarms(quest);
            // TODO: _subscriptionService.ProcessQuestSubscription(quest);
        }

        private void ProcessPokestop(dynamic message)
        {
            string json = Convert.ToString(message);
            var pokestop = json.FromJson<PokestopData>();
            if (pokestop == null)
            {
                _logger.LogWarning($"Failed to deserialize pokestop {message}, skipping...");
                return;
            }
            pokestop.SetTimes();

            if (CheckForDuplicates)
            {
                // TODO: lock processed pokestops, check for dups
            }

            // Process pokestop alarms
            _alarmsService.ProcessPokestopAlarms(pokestop);
            // TODO: New threads
            // TODO: _subscriptionService.ProcessInvasionSubscription(pokestop);
            // TODO: _subscriptionService.ProcessLureSubscription(pokestop);
        }

        private void ProcessGym(dynamic message)
        {
            string json = Convert.ToString(message);
            var gym = json.FromJson<GymDetailsData>();
            if (gym == null)
            {
                _logger.LogWarning($"Failed to deserialize gym {message}, skipping...");
            }

            if (CheckForDuplicates)
            {
                // TODO: lock process gyms, check for dups
            }

            // Process gym alarms
            _alarmsService.ProcessGymAlarms(gym);
        }

        #endregion
    }
}