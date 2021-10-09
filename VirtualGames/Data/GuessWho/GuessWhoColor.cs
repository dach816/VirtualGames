using System.ComponentModel;

namespace VirtualGames.Data.GuessWho
{
    public enum GuessWhoColor
    {
        [Description("danger")] Red = 0,
        [Description("info")] Blue = 1,
        [Description("success")] Green = 2
    }
}