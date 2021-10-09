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

        [JsonProperty("chosenItems")]
        public List<GuessWhoBoardItem> ChosenItems { get; set; }

        [JsonProperty("boards")]
        public List<GuessWhoBoardItem> Boards { get; set; }

        [JsonProperty("category")]
        public GuessWhoCategory Category { get; set; }

        [JsonProperty("numToGuess")]
        public int NumToGuess { get; set; }

        [JsonProperty("numPlayers")]
        public int NumPlayers { get; set; }

        [JsonProperty("startTimestamp")]
        public DateTime StartTimestamp { get; set; }
    }
}