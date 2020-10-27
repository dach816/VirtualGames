using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtualGames.Common.Interface;

namespace VirtualGames.Data.Password
{
    public class PasswordService
    {
        private readonly IRepository<Password> _repo;

        private const string GetPasswordsForGameQuery = @"SELECT TOP 5 * FROM items i ORDER BY i.lastUsedTimestamp ";

        public PasswordService(IRepository<Password> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<string>> GetPasswordsForGame()
        {
            var allPasswords = (await _repo.ReadAsync(GetPasswordsForGameQuery)).ToList();
            if (!allPasswords.Any())
            {
                throw new Exception("No passwords!");
            }

            foreach (var password in allPasswords)
            {
                // Update timestamp so we don't reuse these passwords next game
                password.LastUsedTimestamp = DateTime.UtcNow;
                await _repo.UpdateAsync(password);
            }

            return allPasswords.Select(p => p.PasswordString);
        }
    }
}