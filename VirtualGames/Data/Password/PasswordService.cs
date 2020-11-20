using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualGames.Common.Enums;
using VirtualGames.Common.Interface;

namespace VirtualGames.Data.Password
{
    public class PasswordService
    {
        private readonly IRepository<Password> _passwordRepo;
        private readonly IRepository<Game> _gameRepo;

        private const string GetPasswordsForGameQuery = @"SELECT TOP 5 * FROM items i ORDER BY i.lastUsedTimestamp ";
        private const string GetInProgressGameQuery = @"SELECT TOP 1 * FROM items g WHERE g.gameContent.gameState <> 2 ORDER BY g.gameContent.startTimestamp DESC ";
        private const string GetLatestGameQuery = @"SELECT TOP 1 * FROM items g ORDER BY g.gameContent.startTimestamp DESC ";

        public PasswordService(IRepository<Password> passwordRepo, IRepository<Game> gameRepo)
        {
            _passwordRepo = passwordRepo;
            _gameRepo = gameRepo;
            _gameRepo.SetPartitionKey(GameType.Password.ToString());
        }

        public async Task<IEnumerable<string>> GetPasswordsForGameAsync()
        {
            var allPasswords = (await _passwordRepo.ReadAsync(GetPasswordsForGameQuery)).ToList();
            if (!allPasswords.Any())
            {
                throw new Exception("No passwords!");
            }

            foreach (var password in allPasswords)
            {
                // Update timestamp so we don't reuse these passwords next game
                password.LastUsedTimestamp = DateTime.UtcNow;
                await _passwordRepo.UpdateAsync(password);
            }

            return allPasswords.Select(p => p.PasswordString);
        }

        public async Task<Game> GetCurrentGame()
        {
            return (await _gameRepo.ReadAsync(GetLatestGameQuery)).FirstOrDefault();
        }

        public async Task<Game> GetOrCreateGameAsync()
        {
            var game = (await _gameRepo.ReadAsync(GetInProgressGameQuery)).FirstOrDefault();
            if (game != null)
            {
                return game;
            }

            var passwords = (await GetPasswordsForGameAsync()).ToList();
            game = new Game
            {
                Id = Guid.NewGuid().ToString(),
                Category = GameType.Password.ToString(),
                GameContent = new PasswordGame
                {
                    Passwords = passwords,
                    GameState = GameState.NotStarted,
                    StartTimestamp = DateTime.UtcNow
                }
            };
            return await _gameRepo.CreateAsync(game);
        }

        public async Task UpdateGameAsync(string gameId, PasswordGame passwordGame)
        {
            var game = await _gameRepo.ReadByIdAsync(gameId);
            game.GameContent = passwordGame;
            await _gameRepo.UpdateAsync(game);
        }
    }
}