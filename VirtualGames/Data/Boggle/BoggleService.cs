using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualGames.Common;
using VirtualGames.Common.Interface;

namespace VirtualGames.Data.Boggle
{
    public class BoggleService
    {
        private readonly IRepository<BoggleDie> _diceRepo;
        private readonly IRepository<BoggleGame> _gameRepo;
        private static readonly Random Random = new Random();

        private const string GetInProgressGameQuery = @"SELECT TOP 1 * FROM items g WHERE g.gameState <> 2 ORDER BY g.startTimestamp DESC ";

        public BoggleService(IRepository<BoggleDie> diceRepo, IRepository<BoggleGame> gameRepo)
        {
            _diceRepo = diceRepo;
            _gameRepo = gameRepo;
        }

        public async Task<BoggleGame> GetOrCreateGameAsync()
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

            game = new BoggleGame
            {
                Id = Guid.NewGuid().ToString(),
                GameState = GameState.NotStarted,
                StartTimestamp = DateTime.UtcNow,
                Letters = letters
            };
            await _gameRepo.CreateAsync(game);
            return game;
        }

        public async Task UpdateGameAsync(BoggleGame game)
        {
            await _gameRepo.UpdateAsync(game);
        }
    }
}