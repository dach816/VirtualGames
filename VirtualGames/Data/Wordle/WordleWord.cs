using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace VirtualGames.Data.Wordle
{
    public class WordleWord : BaseDataItem
    {
        private string _word;

        [JsonProperty("word")]
        public string Word { 
            get { return _word; } 
            set
            {
                _word = value;
                Letters = _word.Select((l, i) => new WordleLetter {
                    Letter = l,
                    Index = i
                }).ToList();
            } 
        }

        [JsonIgnore]
        public List<WordleLetter> Letters { get; set; }

        [JsonProperty("lastPlayedTimestampUtc")]
        public DateTime? LastPlayedTimestampUtc { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }
    }
}