using System;
using Newtonsoft.Json;

namespace VirtualGames.Data.Password
{
    public class Password : BaseDataItem
    {
        [JsonProperty("Password")]
        public string PasswordString { get; set; }

        [JsonProperty(PropertyName = "category")]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "lastUsedTimestamp")]
        public DateTime LastUsedTimestamp { get; set; }
    }
}