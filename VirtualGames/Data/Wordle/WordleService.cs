using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualGames.Common.Enums;
using VirtualGames.Common.Interface;

namespace VirtualGames.Data.Wordle
{
    public class WordleService
    {
        private readonly IRepository<Game> _gameRepo;
        private readonly IRepository<WordleWord> _wordRepo;

        public WordleService(IRepository<Game> gameRepo, IRepository<WordleWord> wordRepo)
        {
            _wordRepo = wordRepo;
            _gameRepo = gameRepo;
            _gameRepo.SetPartitionKey(GameType.Wordle.ToString());
        }

        public async Task<Game> CreateGameAsync(int numberOfLetters)
        {
            _wordRepo.SetPartitionKey(numberOfLetters.ToString());
            var getNextWordQuery = "SELECT TOP 1 * FROM items g ORDER BY g.lastPlayedTimestampUtc";
            var nextWord = (await _wordRepo.ReadAsync(getNextWordQuery)).FirstOrDefault();
            if (nextWord == null){
                throw new NullReferenceException("Next word is null.");
            }

            nextWord.LastPlayedTimestampUtc = DateTime.UtcNow;
            var game = new Game {
                Id = Guid.NewGuid().ToString(),
                Category = GameType.Wordle.ToString(),
                GameContent = new WordleGame {
                    WordToGuess = nextWord,
                    Category = numberOfLetters.ToString()
                }
            };
            await _gameRepo.CreateAsync(game);
            await _wordRepo.UpdateAsync(nextWord);
            return game;
        }

        public WordleGuessWord GuessWord(WordleWord wordToGuess, string guessWord)
        {
            if (string.IsNullOrWhiteSpace(guessWord))
            {
                throw new ArgumentException("Guess word must not be empty");
            }

            if (wordToGuess.Letters.Count != guessWord.Length)
            {
                throw new ArgumentException($"Guess word must have {wordToGuess.Letters.Count} letters");
            }

            var guessLetters = new List<WordleLetter>();
            for (var i = 0; i < guessWord.Length; i++)
            {
                var guessLetter = new WordleLetter {
                    Letter = guessWord[i],
                    Index = i
                };
                var actualLetter = wordToGuess.Letters.FirstOrDefault(l => l.Index == i);
                if (actualLetter == null)
                {
                    throw new NullReferenceException($"No letter at index {i} for word to guess.");
                }

                guessLetter.Status = WordleLetterStatus.NotInWord;
                if (actualLetter.Letter == guessLetter.Letter)
                {
                    guessLetter.Status = WordleLetterStatus.InWordRightIndex;
                }
                else if (wordToGuess.Letters.Any(l => l.Letter == guessLetter.Letter))
                {
                    guessLetter.Status = WordleLetterStatus.InWordWrongIndex;
                }

                guessLetters.Add(guessLetter);
            }

            return new WordleGuessWord {
                Letters = guessLetters,
                Word = guessWord
            };
        }
    }
}