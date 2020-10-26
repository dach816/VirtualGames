using Newtonsoft.Json;

namespace VirtualGames.Data.Password
{
    public class Password
    {
        public string Id { get; set; }

        [JsonProperty("Password")]
        public string PasswordString { get; set; }

        public string Category { get; set; }
    }
}