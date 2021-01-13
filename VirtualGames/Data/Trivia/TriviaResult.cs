using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VirtualGames.Data.Trivia
{
    public class TriviaResult
    {
        public string Category { get; set; }

        public string Type { get; set; }

        public string Difficulty { get; set; }

        public string Question { get; set; }

        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get; set; }

        [JsonPropertyName("incorrect_answers")]
        public IEnumerable<string> IncorrectAnswers { get; set; }
    }
}