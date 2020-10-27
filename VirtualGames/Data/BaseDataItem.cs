using Newtonsoft.Json;

namespace VirtualGames.Data
{
    public class BaseDataItem
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}