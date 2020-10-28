using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtualGames.Common;
using VirtualGames.Common.Interface;

namespace VirtualGames.Data.Password
{
    public class PasswordService
    {
        private readonly IRepository<Password> _passwordRepo;
        private readonly IRepository<PasswordGame> _gameRepo;

        private const string GetPasswordsForGameQuery = @"SELECT TOP 5 * FROM items i ORDER BY i.lastUsedTimestamp ";
        private const string GetInProgressGameQuery = @"SELECT TOP 1 * FROM items g WHERE g.gameState <> 2 ORDER BY g.startTimestamp DESC ";
        private const string GetLatestGameQuery = @"SELECT TOP 1 * FROM items g ORDER BY g.startTimestamp DESC ";

        public PasswordService(IRepository<Password> passwordRepo, IRepository<PasswordGame> gameRepo)
        {
            _passwordRepo = passwordRepo;
            _gameRepo = gameRepo;
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

        public async Task<PasswordGame> GetCurrentGame()
        {
            return (await _gameRepo.ReadAsync(GetLatestGameQuery)).FirstOrDefault();
        }

        public async Task<PasswordGame> GetOrCreateGameAsync()
        {
            var game = (await _gameRepo.ReadAsync(GetInProgressGameQuery)).FirstOrDefault();
            if (game != null)
            {
                return game;
            }

            var passwords = (await GetPasswordsForGameAsync()).ToList();
            game = new PasswordGame
            {
                Id = Guid.NewGuid().ToString(),
                Passwords = passwords,
                Category = "Default",
                GameState = GameState.NotStarted,
                StartTimestamp = DateTime.UtcNow
            };
            return await _gameRepo.CreateAsync(game);
        }

        public async Task UpdateGameAsync(PasswordGame game)
        {
            await _gameRepo.UpdateAsync(game);
        }
    }
}