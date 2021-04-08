using VirtualGames.Data;
using VirtualGames.Data.Boggle;
using VirtualGames.Data.GuessWho;
using VirtualGames.Data.Password;

namespace VirtualGames.Common.Extensions
{
    public static class GameExtensions
    {
        public static GuessWhoGame GetGuessWhoContent(this Game game)
        {
            if (game.GameContent is GuessWhoGame guessWhoGameContent)
            {
                return guessWhoGameContent;
            }

            return game.GetContentAs<GuessWhoGame>();
        }

        public static PasswordGame GetPasswordContent(this Game game)
        {
            if (game.GameContent is PasswordGame passwordGameContent)
            {
                return passwordGameContent;
            }

            return game.GetContentAs<PasswordGame>();
        }

        public static BoggleGame GetBoggleContent(this Game game)
        {
            if (game.GameContent is BoggleGame boggleGameContent)
            {
                return boggleGameContent;
            }

            return game.GetContentAs<BoggleGame>();
        }
    }
}