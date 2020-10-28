using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using VirtualGames.Common;
using VirtualGames.Data.Password;
using Xunit;

namespace VirtualGamesTest
{
    public class DatabaseTests
    {
        private readonly Repository<Password> _passwordRepo;
        private Repository<PasswordGame> _gameRepo;

        public DatabaseTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetAssembly(typeof(DatabaseTests))).Build();
            var cosmosClient = new CosmosClient(configuration["CosmosDb:Account"], configuration["CosmosDb:Key"]);
            _passwordRepo = new Repository<Password>(cosmosClient, configuration["CosmosDb:DatabaseName"]);
            _gameRepo = new Repository<PasswordGame>(cosmosClient, configuration["CosmosDb:DatabaseName"]);
        }

        [Fact]
        public async Task ReplacePasswordAsync()
        {
            var query = "SELECT * FROM c WHERE c.Password = \"Yoghurt\"";
            var password = (await _passwordRepo.ReadAsync(query)).FirstOrDefault();

            if (password != null)
            {
                password.PasswordString = "Yogurt";
                await _passwordRepo.UpdateAsync(password);
            }
        }

        [Fact]
        public async Task DeleteAllGamesAsync()
        {
            var query = "SELECT * FROM c";
            var games = await _gameRepo.ReadAsync(query);

            foreach (var game in games)
            {
                await _gameRepo.DeleteAsync(game);
            }
        }
    }
}
