using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VirtualGames.Common;

namespace VirtualGames.Data.GuessWho
{
    public class GuessWhoGame : BaseDataItem
    {
        [JsonProperty("gameState")]
        public GameState GameState { get; set; }

        [JsonProperty("isRedTurn")]
        public bool IsRedTurn { get; set; }

        [JsonProperty("redChosenItem")]
        public GuessWhoBoardItem RedChosenItem { get; set; }

        [JsonProperty("blueChosenItem")]
        public GuessWhoBoardItem BlueChosenItem { get; set; }

        [JsonProperty("redBoard")]
        public IEnumerable<GuessWhoBoardItem> RedBoard { get; set; }

        [JsonProperty("blueBoard")]
        public IEnumerable<GuessWhoBoardItem> BlueBoard { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("startTimestamp")]
        public DateTime StartTimestamp { get; set; }
    }
}