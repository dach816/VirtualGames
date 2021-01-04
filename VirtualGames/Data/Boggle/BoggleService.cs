using System;
using System.Linq;
using System.Threading.Tasks;
using VirtualGames.Common.Enums;
using VirtualGames.Common.Interface;

namespace VirtualGames.Data.Boggle
{
    public class BoggleService
    {
        private readonly IRepository<BoggleDie> _diceRepo;
        private readonly IRepository<Game> _gameRepo;
        private static readonly Random Random = new Random();

        private const string GetInProgressGameQuery = @"SELECT TOP 1 * FROM items g WHERE g.gameState <> 2 ORDER BY g.startTimestamp DESC ";

        public BoggleService(IRepository<BoggleDie> diceRepo, IRepository<Game> gameRepo)
        {
            _diceRepo = diceRepo;
            _gameRepo = gameRepo;
            _gameRepo.SetPartitionKey("Boggle");
        }

        public async Task<Game> GetOrCreateGameAsync()
        {
            var game = (await _gameRepo.ReadAsync(GetInProgressGameQuery)).FirstOrDefault();
            if (game != null)
            {
                return game;
            }

            var dice = await _diceRepo.ReadAsync(null);
            var letters = dice.Select(d =>
            {
                var index = Random.Next(d.PossibleLetters.Count);
                return d.PossibleLetters[index];
            }).ToList();

            game = new Game
            {
                Id = Guid.NewGuid().ToString(),
                Category = GameType.Boggle.ToString("G"),
                GameContent = new BoggleGame
                {
                    GameState = GameState.NotStarted,
                    StartTimestamp = DateTime.UtcNow,
                    Letters = letters
                }
            };
            await _gameRepo.CreateAsync(game);
            return game;
        }

        public async Task<Game> GetGameAsync(string gameId)
        {
            return await _gameRepo.ReadByIdAsync(gameId);
        }

        public async Task UpdateGameAsync(string gameId, BoggleGame boggleGame)
        {
            var game = await _gameRepo.ReadByIdAsync(gameId);
            game.GameContent = boggleGame;
            await _gameRepo.UpdateAsync(game);
        }
    }
}