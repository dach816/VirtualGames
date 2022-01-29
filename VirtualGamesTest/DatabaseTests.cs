using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using VirtualGames.Common;
using VirtualGames.Data;
using VirtualGames.Data.GuessWho;
using VirtualGames.Data.Password;
using VirtualGames.Data.Wordle;
using Xunit;

namespace VirtualGamesTest
{
    public class DatabaseTests
    {
        private readonly Repository<Password> _passwordRepo;
        private readonly Repository<Game> _GameRepo;
        private readonly Repository<GuessWhoItem> _guessWhoItemRepo;
        private readonly Repository<WordleWord> _wordRepo;

        public DatabaseTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetAssembly(typeof(DatabaseTests))).Build();
            var cosmosClient = new CosmosClient(configuration["CosmosDb:Account"], configuration["CosmosDb:Key"]);
            _passwordRepo = new Repository<Password>(cosmosClient, configuration["CosmosDb:DatabaseName"]);
            _GameRepo = new Repository<Game>(cosmosClient, configuration["CosmosDb:DatabaseName"]);
            _guessWhoItemRepo = new Repository<GuessWhoItem>(cosmosClient, configuration["CosmosDb:DatabaseName"]);
            _wordRepo = new Repository<WordleWord>(cosmosClient, configuration["CosmosDb:DatabaseName"]);
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
            var games = await _GameRepo.ReadAsync(query);

            foreach (var game in games)
            {
                await _GameRepo.DeleteAsync(game);
            }
        }

        [Theory]
        [InlineData("1", "HarryPotter", "")]
        public async Task ReplaceGuessWhoPictureAsync(string id, string category, string picturePath)
        {
            var query = $"SELECT * FROM c WHERE c.id = \"{id}\"";
            _guessWhoItemRepo.SetPartitionKey(category);
            var guessWhoItem = (await _guessWhoItemRepo.ReadAsync(query)).FirstOrDefault();

            if (guessWhoItem != null)
            {
                var imageArray = File.ReadAllBytes(picturePath);
                var imageString = Convert.ToBase64String(imageArray);
                guessWhoItem.Picture = $"data:image/jpeg;base64,{imageString}";
                await _guessWhoItemRepo.UpdateAsync(guessWhoItem);
            }
        }

        [Theory]
        [InlineData("", @"")]
        public async Task UploadPicturesFromFolderAsync(string category, string pictureFolderPath)
        {
            _guessWhoItemRepo.SetPartitionKey(category);
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

                var imageArray = File.ReadAllBytes(fileName);
                var imageString = Convert.ToBase64String(imageArray);
                var guessWhoItem = new GuessWhoItem
                {
                    Id = id.ToString(),
                    Category = category,
                    Name = GetFileNameNoExtension(fileName),
                    Picture = $"data:image/jpeg;base64,{imageString}"
                };
                await _guessWhoItemRepo.CreateAsync(guessWhoItem);
                id++;
            }
        }

        [Theory]
        [InlineData("")]
        public async Task AddWordleWords(string word)
        {
            word = word.ToUpper();

            _wordRepo.SetPartitionKey(word.Length.ToString());
            var wordleWord = new WordleWord {
                Id = Guid.NewGuid().ToString(),
                Word = word,
                Letters = word.Select((l, i) => new WordleLetter {
                    Letter = l,
                    Index = i
                }).ToList(),
                Category = word.Length.ToString()
            };
            await _wordRepo.CreateAsync(wordleWord);
        }

        private string GetFileNameNoExtension(string fileName)
        {
            var dotIndex = fileName.IndexOf('.');
            var slashIndex = fileName.LastIndexOf('\\') + 1;
            return fileName.Substring(slashIndex, dotIndex - slashIndex);
        }
    }
}
