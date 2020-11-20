using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VirtualGames.Common.Enums;

namespace VirtualGames.Data.GuessWho
{
    public class GuessWhoGame : IGameContent
    {
        [JsonProperty("gameState")]
        public GameState GameState { get; set; }

        [JsonProperty("isRedTurn")]
        public bool IsRedTurn { get; set; }

        [JsonProperty("redChosenItems")]
        public List<GuessWhoBoardItem> RedChosenItems { get; set; }

        [JsonProperty("blueChosenItems")]
        public List<GuessWhoBoardItem> BlueChosenItems { get; set; }

        [JsonProperty("redBoard")]
        public List<GuessWhoBoardItem> RedBoard { get; set; }

        [JsonProperty("blueBoard")]
        public List<GuessWhoBoardItem> BlueBoard { get; set; }

        [JsonProperty("category")]
        public GuessWhoCategory Category { get; set; }

        [JsonProperty("numToGuess")]
        public int NumToGuess { get; set; }

        [JsonProperty("startTimestamp")]
        public DateTime StartTimestamp { get; set; }
    }
}