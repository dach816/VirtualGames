using Newtonsoft.Json;

namespace VirtualGames.Data.GuessWho
{
    public class GuessWhoItem : BaseDataItem
    {
        [JsonProperty("picture")]
        public string Picture { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }
    }
}