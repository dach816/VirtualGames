using System.Collections.Generic;

namespace VirtualGames.Data.Wordle
{
    public class WordleGuessWord
    {
        public List<WordleLetter> Letters { get; set; }

        public string Word { get; set; }
    }
}