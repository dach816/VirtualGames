using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualGames.Common.Interface;

namespace VirtualGames.Data.Password
{
    public class PasswordService
    {
        private readonly IRepository<Password> _repo;

        public PasswordService(IRepository<Password> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<string>> GetPasswordsForGame()
        {
            var allPasswords = await _repo.ReadAsync();
            return allPasswords.Take(10).Select(p => p.PasswordString);
        }
    }
}