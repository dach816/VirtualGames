using System.Collections.Generic;
using Newtonsoft.Json;

namespace VirtualGames.Data.Boggle
{
    public class BoggleDie : BaseDataItem
    {
        [JsonProperty("possibleLetters")]
        public List<string> PossibleLetters { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }
    }
}