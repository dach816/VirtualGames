using Newtonsoft.Json;

namespace VirtualGames.Data.Wordle
{
    public class WordleGame : IGameContent
    {
        [JsonProperty("wordToGuess")]
        public WordleWord WordToGuess { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }
    }
}