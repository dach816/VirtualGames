using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using VirtualGames.Common;
using VirtualGames.Data.GuessWho;
using VirtualGames.Data.Password;
using Xunit;

namespace VirtualGamesTest
{
    public class DatabaseTests
    {
        private readonly Repository<Password> _passwordRepo;
        private readonly Repository<PasswordGame> _passwordGameRepo;
        private readonly Repository<GuessWhoItem> _guessWhoItemRepo;

        public DatabaseTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetAssembly(typeof(DatabaseTests))).Build();
            var cosmosClient = new CosmosClient(configuration["CosmosDb:Account"], configuration["CosmosDb:Key"]);
            _passwordRepo = new Repository<Password>(cosmosClient, configuration["CosmosDb:DatabaseName"]);
            _passwordGameRepo = new Repository<PasswordGame>(cosmosClient, configuration["CosmosDb:DatabaseName"]);
            _guessWhoItemRepo = new Repository<GuessWhoItem>(cosmosClient, configuration["CosmosDb:DatabaseName"]);
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
        public async Task DeleteAllPasswordGamesAsync()
        {
            var query = "SELECT * FROM c";
            var games = await _passwordGameRepo.ReadAsync(query);

            foreach (var game in games)
            {
                await _passwordGameRepo.DeleteAsync(game);
            }
        }

        [Theory]
        [InlineData("1", "HarryPotter", "")]
        public async Task ReplaceGuessWhoPictureAsync(string id, string category, string picturePath)
        {
            var query = $"SELECT * FROM c WHERE c.id = \"{id}\"";
            var guessWhoItem = (await _guessWhoItemRepo.ReadAsync(query, category)).FirstOrDefault();

            if (guessWhoItem != null)
            {
                var imageArray = File.ReadAllBytes(picturePath);
                var imageString = Convert.ToBase64String(imageArray);
                guessWhoItem.Picture = $"data:image/jpeg;base64,{imageString}";
                await _guessWhoItemRepo.UpdateAsync(guessWhoItem, category);
            }
        }

        [Theory]
        [InlineData("", @"")]
        public async Task UploadPictureAsync(string category, string pictureFolderPath)
        {
            if (!Directory.Exists(pictureFolderPath))
            {
                throw new Exception($"No directory at path {pictureFolderPath}.");
            }

            var files = Directory.GetFiles(pictureFolderPath);
            if (files.Length < 24)
            {
                throw new Exception($"Need at least 24 files in folder {pictureFolderPath}.");
            }

            var id = 1;
            foreach (var fileName in files)
            {
                if (id >= 25)
                {
                    break;
                }

                if (id < 7)
                {
                    id++;
                    continue;
                }

                var imageArray = File.ReadAllBytes(fileName);
                var imageString = Convert.ToBase64String(imageArray);
                var guessWhoItem = new GuessWhoItem
                {
                    Id = id.ToString(),
                    Category = category,
                    Name = GetFileNameNoExtension(fileName),
                    Picture = $"data:image/jpeg;base64,{imageString}"
                };
                await _guessWhoItemRepo.CreateAsync(guessWhoItem, category);
                id++;
            }
        }

        private string GetFileNameNoExtension(string fileName)
        {
            var dotIndex = fileName.IndexOf('.');
            var slashIndex = fileName.LastIndexOf('\\') + 1;
            return fileName.Substring(slashIndex, dotIndex - slashIndex);
        }
    }
}
