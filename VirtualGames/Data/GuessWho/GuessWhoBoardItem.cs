using System;

namespace VirtualGames.Data.GuessWho
{
    public class GuessWhoBoardItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public GuessWhoItem Item { get; set; }

        public bool IsVisible { get; set; }
    }
}