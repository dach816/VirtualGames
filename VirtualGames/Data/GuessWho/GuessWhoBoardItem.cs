using System;

namespace VirtualGames.Data.GuessWho
{
    public class GuessWhoBoardItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string ItemId { get; set; }

        public bool IsVisible { get; set; }
    }
}