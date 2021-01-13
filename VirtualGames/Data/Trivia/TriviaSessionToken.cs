using System.Text.Json.Serialization;

namespace VirtualGames.Data.Trivia
{
    public class TriviaSessionToken
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("response_message")]
        public string ResponseMessage { get; set; }

        public string Token { get; set; }
    }
}