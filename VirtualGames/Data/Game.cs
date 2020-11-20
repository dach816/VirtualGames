using Newtonsoft.Json;

namespace VirtualGames.Data
{
    public class Game : BaseDataItem
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("gameContent")]
        public object GameContent { get; set; }

        public T GetContentAs<T>()
        where T : IGameContent
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(GameContent));
        }
    }
}