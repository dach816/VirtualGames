using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualGames.Common.Enums;
using VirtualGames.Common.Interface;

namespace VirtualGames.Data.GuessWho
{
    public class GuessWhoService
    {
        private readonly IRepository<GuessWhoItem> _itemRepo;
        private readonly IRepository<Game> _gameRepo;
        private static readonly Random Random = new Random();

        private const string GetInProgressGamesQuery = @"SELECT * FROM items g WHERE g.gameContent.gameState = 1 ORDER BY g.gameContent.startTimestamp DESC ";

        public GuessWhoService(IRepository<GuessWhoItem> itemRepo, IRepository<Game> gameRepo)
        {
            _itemRepo = itemRepo;
            _gameRepo = gameRepo;
            _gameRepo.SetPartitionKey(GameType.GuessWho.ToString());
        }

        public async Task<Game> GetOrCreateGameAsync(GuessWhoCategory category, int numToGuess)
        {
            var getInProgressGameQuery = @$"SELECT TOP 1 * FROM items g WHERE g.gameContent.gameState <> 2 AND g.gameContent.category = {category:D} ORDER BY g.gameContent.startTimestamp DESC ";
            var game = (await _gameRepo.ReadAsync(getInProgressGameQuery)).FirstOrDefault();
            if (game != null)
            {
                return game;
            }

            var categoryItems = await GetAllCategoryItemsAsync(category);
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
            game = new Game
            {
                Id = Guid.NewGuid().ToString(),
                Category = GameType.GuessWho.ToString(),
                GameContent = new GuessWhoGame
                {
                    GameState = GameState.NotStarted,
                    Category = category,
                    BlueBoard = boardItems,
                    RedBoard = boardItems,
                    RedChosenItems = redChosenItems,
                    BlueChosenItems = blueChosenItems,
                    IsRedTurn = false,
                    NumToGuess = numToGuess,
                    StartTimestamp = DateTime.UtcNow
                }
            };
            await _gameRepo.CreateAsync(game);
            return game;
        }

        public async Task<Game> GetGameAsync(string gameId)
        {
            return await _gameRepo.ReadByIdAsync(gameId);
        }

        public async Task<IEnumerable<Game>> GetAllGamesAsync()
        {
            return await _gameRepo.ReadAsync(GetInProgressGamesQuery);
        }

        public async Task UpdateGameAsync(string gameId, GuessWhoGame guessWhoGame)
        {
            var game = await _gameRepo.ReadByIdAsync(gameId);
            game.GameContent = guessWhoGame;
            await _gameRepo.UpdateAsync(game);
        }
        
        public async Task<List<GuessWhoItem>> GetAllCategoryItemsAsync(GuessWhoCategory category)
        {
            _itemRepo.SetPartitionKey(category.ToString("G"));
            var items = (await _itemRepo.ReadAsync(null)).ToList();
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