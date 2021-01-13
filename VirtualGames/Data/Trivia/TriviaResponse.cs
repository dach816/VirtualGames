using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VirtualGames.Data.Trivia
{
    public class TriviaResponse
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }

        public IEnumerable<TriviaResult> Results { get; set; }
    }
}