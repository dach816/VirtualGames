﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualGames.Common;
using VirtualGames.Common.Interface;

namespace VirtualGames.Data.GuessWho
{
    public class GuessWhoService
    {
        private readonly IRepository<GuessWhoItem> _itemRepo;
        private readonly IRepository<GuessWhoGame> _gameRepo;
        private static readonly Random _random = new Random();

        private const string GetInProgressGameQuery = @"SELECT TOP 1 * FROM items g WHERE g.gameState <> 2 ORDER BY g.startTimestamp DESC ";
        private const string GetLatestGameQuery = @"SELECT TOP 1 * FROM items g ORDER BY g.startTimestamp DESC ";

        public GuessWhoService(IRepository<GuessWhoItem> itemRepo, IRepository<GuessWhoGame> gameRepo)
        {
            _itemRepo = itemRepo;
            _gameRepo = gameRepo;
        }

        public async Task<GuessWhoGame> GetOrCreateGameAsync(GuessWhoCategory category)
        {
            var game = (await _gameRepo.ReadAsync(GetInProgressGameQuery, category.ToString("G"))).FirstOrDefault();
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
                    Item = i,
                    IsVisible = true
                }).ToList();
            var redChosenItem = GetRandomItem(boardItems);
            var blueChosenItem = GetRandomItem(boardItems.Where(i => i.Item.Id != redChosenItem.Item.Id).ToList());
            game = new GuessWhoGame
            {
                Id = Guid.NewGuid().ToString(),
                GameState = GameState.NotStarted,
                Category = category.ToString("G"),
                BlueBoard = boardItems,
                RedBoard = boardItems,
                RedChosenItem = redChosenItem,
                BlueChosenItem = blueChosenItem,
                IsRedTurn = false,
                StartTimestamp = DateTime.UtcNow
            };
            await _gameRepo.CreateAsync(game, category.ToString("G"));
            return game;
        }

        public async Task<GuessWhoGame> GetCurrentGame(GuessWhoCategory category)
        {
            return (await _gameRepo.ReadAsync(GetLatestGameQuery, category.ToString("G"))).FirstOrDefault();
        }

        public async Task UpdateGameAsync(GuessWhoGame game)
        {
            await _gameRepo.UpdateAsync(game, game.Category);
        }

        public async Task UpdateGameAsync(GuessWhoCategory category, bool isRed, GuessWhoBoardItem boardItem)
        {
            var game = await GetCurrentGame(category);
            UpdateBoardItem(isRed ? game.RedBoard.ToList() : game.BlueBoard.ToList(), boardItem);
            await _gameRepo.UpdateAsync(game, category.ToString("G"));
        }

        private async Task<List<GuessWhoItem>> GetAllCategoryItemsAsync(GuessWhoCategory category)
        {
            var items = (await _itemRepo.ReadAsync(null, category.ToString("G"))).ToList();
            if (!items.Any())
            {
                throw new Exception($"No items for category {category:G}.");
            }

            return items;
        }

        private GuessWhoBoardItem GetRandomItem(IList<GuessWhoBoardItem> items)
        {
            var index = _random.Next(items.Count);
            return items[index];
        }

        private List<GuessWhoBoardItem> UpdateBoardItem(
            List<GuessWhoBoardItem> boardItems,
            GuessWhoBoardItem updatedBoardItem)
        {
            var toUpdate = boardItems.FirstOrDefault(i => i.Id == updatedBoardItem.Id);
            if (toUpdate == null)
            {
                return boardItems;
            }

            toUpdate.IsVisible = updatedBoardItem.IsVisible;
            return boardItems;
        }
    }
}