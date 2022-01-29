namespace VirtualGames.Data.Wordle
{
    public class WordleLetter
    {
        public char Letter { get; set; }

        public int Index { get; set; }

        public WordleLetterStatus? Status { get; set; }
    }
}