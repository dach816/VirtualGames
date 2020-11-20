using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VirtualGames.Common.Enums;

namespace VirtualGames.Data.Password
{
    public class PasswordGame : IGameContent
    {
        [JsonProperty("passwords")]
        public List<string> Passwords { get; set; }

        [JsonProperty("redPoints")]
        public int RedPoints { get; set; }

        [JsonProperty("bluePoints")]
        public int BluePoints { get; set; }

        [JsonProperty("currentWord")]
        public string CurrentWord { get; set; }

        [JsonProperty("gameState")]
        public GameState GameState { get; set; }

        [JsonProperty("startTimestamp")]
        public DateTime StartTimestamp { get; set; }
    }
}