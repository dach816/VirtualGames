using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VirtualGames.Data.Trivia
{
    public class TriviaCategories
    {
        [JsonPropertyName("trivia_categories")]
        public IEnumerable<TriviaCategory> Categories { get; set; }
    }
}