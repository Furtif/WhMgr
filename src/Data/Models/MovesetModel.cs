﻿namespace WhMgr.Data.Models
{
    using Newtonsoft.Json;

    using WhMgr.Net.Models;

    public class MovesetModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public PokemonType Type { get; set; }

        [JsonProperty("damage")]
        public int Damage { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("energy")]
        public int Energy { get; set; }

        [JsonProperty("dps")]
        public double Dps { get; set; }

        public MovesetModel()
        {
            Type = PokemonType.None;
        }
    }
}