using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualGames.Common;
using VirtualGames.Common.Extensions;
using VirtualGames.Common.Interface;

namespace VirtualGames.Data.GuessWho
{
    public class GuessWhoService
    {
        private readonly IRepository<GuessWhoItem> _itemRepo;
        private readonly IRepository<GuessWhoGame> _gameRepo;
        private static readonly Random Random = new Random();

        private const string GetInProgressGameQuery = @"SELECT TOP 1 * FROM items g WHERE g.gameState <> 2 ORDER BY g.startTimestamp DESC ";
        private const string GetInProgressGamesQuery = @"SELECT * FROM items g WHERE g.gameState = 1 ORDER BY g.startTimestamp DESC ";

        public GuessWhoService(IRepository<GuessWhoItem> itemRepo, IRepository<GuessWhoGame> gameRepo)
        {
            _itemRepo = itemRepo;
            _gameRepo = gameRepo;
        }

        public async Task<GuessWhoGame> GetOrCreateGameAsync(GuessWhoCategory category, int numToGuess)
        {
            var game = (await _gameRepo.ReadAsync(GetInProgressGameQuery, category.ToString("G"))).FirstOrDefault();
            if (game != null)
            {
                return game;
            }

            var categoryItems = await GetAllCategoryItemsAsync(category.ToString("G"));
            if (categoryItems.Count != 24)
            {
                throw new Exception($"Need 24 items for category {category:G}.");
            }

            var boardItems = categoryItems.Select(i =>
                new GuessWhoBoardItem
                {
                    ItemId = i.Id,
                    IsVisible = true
                }).ToList();
            var redChosenItems = GetRandomItems(boardItems, null, numToGuess);
            var blueChosenItems = GetRandomItems(boardItems, redChosenItems, numToGuess);
            game = new GuessWhoGame
            {
                Id = Guid.NewGuid().ToString(),
                GameState = GameState.NotStarted,
                Category = category.ToString("G"),
                BlueBoard = boardItems,
                RedBoard = boardItems,
                RedChosenItems = redChosenItems,
                BlueChosenItems = blueChosenItems,
                IsRedTurn = false,
                NumToGuess = numToGuess,
                StartTimestamp = DateTime.UtcNow
            };
            await _gameRepo.CreateAsync(game, category.ToString("G"));
            return game;
        }

        public async Task<GuessWhoGame> GetGameAsync(string category, string gameId)
        {
            return await _gameRepo.ReadByIdAsync(gameId, category);
        }

        public async Task<IEnumerable<GuessWhoGame>> GetAllGamesAsync()
        {
            var categories = GuessWhoCategory.Avatar.ToStringList();
            var allGames = new List<GuessWhoGame>();
            foreach (var category in categories)
            {
                var games = await _gameRepo.ReadAsync(GetInProgressGamesQuery, category);
                allGames.AddRange(games);
            }

            return allGames;
        }

        public async Task UpdateGameAsync(GuessWhoGame game)
        {
            await _gameRepo.UpdateAsync(game, game.Category);
        }
        
        public async Task<List<GuessWhoItem>> GetAllCategoryItemsAsync(string category)
        {
            var items = (await _itemRepo.ReadAsync(null, category)).ToList();
            if (!items.Any())
            {
                throw new Exception($"No items for category {category:G}.");
            }

            return items;
        }

        private List<GuessWhoBoardItem> GetRandomItems(IList<GuessWhoBoardItem> items, IList<GuessWhoBoardItem> ignoreItems, int numItems)
        {
            if (ignoreItems == null)
            {
                ignoreItems = new List<GuessWhoBoardItem>();
            }

            var chosenItems = new List<GuessWhoBoardItem>();
            for (var n = 0; n < numItems; n++)
            {
                var index = Random.Next(items.Count - ignoreItems.Count - chosenItems.Count);
                var item = items
                    .Where(i => ignoreItems.All(r => r.ItemId != i.ItemId)
                                && chosenItems.All(r => r.ItemId != i.ItemId))
                    .ToList()[index];
                chosenItems.Add(item);
            }

            return chosenItems;
        }
    }
}