using System.ComponentModel;

namespace VirtualGames.Data.GuessWho
{
    public enum GuessWhoCategory
    {
        [Description("Food")] Food = 0,
        [Description("Harry Potter")] HarryPotter = 1,
        [Description("Looney Toons")] LooneyToons = 2,
        [Description("Fullmetal Alchemist Brotherhood")] FullmetalAlchemist = 3,
        [Description("Avatar the Last Airbender")] Avatar = 4
    }
}