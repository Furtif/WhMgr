﻿namespace WhMgr
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using WhMgr.Data;
    using WhMgr.Extensions;
    using WhMgr.Net.Models;

    public class Statistics
    {
        #region Singleton

        private static Statistics _instance;
        public static Statistics Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Statistics();
                }

                return _instance;
            }
        }

        #endregion

        #region Properties

        public long PokemonSent { get; set; }

        public long RaidsSent { get; set; }

        public long QuestsSent { get; set; }

        public long SubscriptionPokemonSent { get; set; }

        public long SubscriptionRaidsSent { get; set; }

        public long SubscriptionQuestsSent { get; set; }

        public long SubscriptionInvasionsSent { get; set; }

        public Dictionary<int, int> PokemonStats { get; }

        public Dictionary<int, int> RaidStats { get; }

        public IEnumerable<KeyValuePair<int, int>> Top25Pokemon => PokemonStats.GroupWithCount(25);

        public IEnumerable<KeyValuePair<int, int>> Top25Raids => RaidStats.GroupWithCount(25);

        public Dictionary<DateTime, PokemonData> Hundos { get; }

        #endregion

        #region Constructor

        public Statistics()
        {
            PokemonStats = new Dictionary<int, int>();
            RaidStats = new Dictionary<int, int>();
            Hundos = new Dictionary<DateTime, PokemonData>();
        }

        #endregion

        #region Public Methods

        public void Add100Percent(PokemonData pokemon)
        {
            Hundos.Add(DateTime.Now, pokemon);
        }

        public void IncrementPokemonStats(int pokemonId)
        {
            if (PokemonStats.ContainsKey(pokemonId))
            {
                PokemonStats[pokemonId]++;
            }
            else
            {
                PokemonStats.Add(pokemonId, 1);
            }
        }

        public void IncrementRaidStats(int pokemonId)
        {
            if (RaidStats.ContainsKey(pokemonId))
            {
                RaidStats[pokemonId]++;
            }
            else
            {
                RaidStats.Add(pokemonId, 1);
            }
        }

        public void WriteOut()
        {
            if (!Directory.Exists(Strings.StatsFolder))
            {
                Directory.CreateDirectory(Strings.StatsFolder);
            }

            var stats = Statistics.Instance;
            var sb = new System.Text.StringBuilder();
            var header = "Pokemon Alarms,Raid Alarms,Quest Alarms,Pokemon Subscriptions,Raid Subscriptions,Quest Subscriptions,Top 25 Pokemon,Top 25 Raids";
            sb.AppendLine(header);

            sb.Append(stats.PokemonSent);
            sb.Append(",");
            sb.Append(stats.RaidsSent);
            sb.Append(",");
            sb.Append(stats.QuestsSent);
            sb.Append(",");
            sb.Append(stats.SubscriptionPokemonSent);
            sb.Append(",");
            sb.Append(stats.SubscriptionRaidsSent);
            sb.Append(",");
            sb.Append(stats.SubscriptionQuestsSent);
            sb.Append(",");
            sb.Append(string.Join(Environment.NewLine, stats.Top25Pokemon.Select(x => $"{MasterFile.GetPokemon(x.Key, 0)?.Name}: {x.Value.ToString("N0")}")));
            sb.Append(",");
            sb.Append(string.Join(Environment.NewLine, stats.Top25Raids.Select(x => $"{MasterFile.GetPokemon(x.Key, 0)?.Name}: {x.Value.ToString("N0")}")));

            try
            {
                File.WriteAllText(Path.Combine(Strings.StatsFolder, string.Format(Strings.StatsFileName, DateTime.Now.ToString("yyyy-MM-dd_hhmmss"))), sb.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Reset()
        {
            PokemonStats.Clear();
            RaidStats.Clear();
            PokemonSent = 0;
            RaidsSent = 0;
            QuestsSent = 0;
            SubscriptionPokemonSent = 0;
            SubscriptionRaidsSent = 0;
            SubscriptionQuestsSent = 0;
            SubscriptionInvasionsSent = 0;
        }

        #endregion
    }
}