using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VirtualGames.Common;

namespace VirtualGames.Data.Boggle
{
    public class BoggleGame : BaseDataItem
    {
        [JsonProperty("letters")]
        public List<string> Letters { get; set; }

        [JsonProperty("gameState")]
        public GameState GameState { get; set; }

        [JsonProperty("startTimestamp")]
        public DateTime StartTimestamp { get; set; }
    }
}