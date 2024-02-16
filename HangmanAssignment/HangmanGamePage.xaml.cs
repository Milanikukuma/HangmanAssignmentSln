using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace HangmanAssignment
{
    public partial class HangmanGamePage : ContentPage
    {
        private List<string> wordList = new List<string>
        {
            "tape",
            "train",
            "purple",
            "animal",
            "milani",
            "oxford"
        };

        private string chosenWord = "";
        private string guessedWord = "";
        private int maxIncorrectGuesses = 8; // Maximum incorrect guesses allowed
        private int remainingIncorrectGuesses;
        private string guessedLetters = "";
        private int currentHangmanImageIndex = 1;
        private bool gameOver = false;

        public HangmanGamePage()
        {
            InitializeComponent();
            StartNewGame();
        }

        private void StartNewGame()
        {
            chosenWord = SelectRandomWord();
            guessedWord = HideLetters(chosenWord);
            remainingIncorrectGuesses = maxIncorrectGuesses;
            currentHangmanImageIndex = 1;
            gameOver = false;

            WordLabel.Text = guessedWord;
            HangmanImage.Source = $"hang{currentHangmanImageIndex}.png";
        }

        private string SelectRandomWord()
        {
            return wordList[new Random().Next(wordList.Count)];
        }

        private string HideLetters(string word)
        {
            // Hide 30% of the letters
            int lettersToHide = (int)(word.Length * 0.3);
            List<int> indicesToHide = new List<int>();

            Random random = new Random();
            while (indicesToHide.Count < lettersToHide)
            {
                int index = random.Next(word.Length);
                if (!indicesToHide.Contains(index))
                {
                    indicesToHide.Add(index);
                }
            }

            char[] hiddenWordArray = word.ToCharArray();
            foreach (int index in indicesToHide)
            {
                hiddenWordArray[index] = '_';
            }

            return new string(hiddenWordArray);
        }

        private void OnGuessClicked(object sender, EventArgs e)
        {
            if (gameOver)
            {
                // Game is over, don't allow further guesses
                return;
            }

            if (GuessEntry.Text.Length == 0)
            {
                return;
            }

            char guess = GuessEntry.Text.ToLower()[0];

            if (guessedLetters.Contains(guess))
            {
                return;
            }

            guessedLetters += guess;

            if (chosenWord.Contains(guess))
            {
                char[] guessedWordArray = guessedWord.ToCharArray();
                for (int i = 0; i < chosenWord.Length; i++)
                {
                    if (chosenWord[i] == guess)
                    {
                        guessedWordArray[i] = guess;
                    }
                }
                guessedWord = new string(guessedWordArray);
                WordLabel.Text = guessedWord;
            }
            else
            {
                remainingIncorrectGuesses--;

                currentHangmanImageIndex++;
                if (remainingIncorrectGuesses == 0)
                {
                    // All hangman images displayed, end the game
                    DisplayAlert("Game Over", $"The word was: {chosenWord}", "OK");
                    StartNewGame();
                    gameOver = true; // Set game over flag
                }
                else
                {
                    HangmanImage.Source = $"hang{currentHangmanImageIndex}.png";
                }
            }

            if (guessedWord == chosenWord)
            {
                DisplayAlert("Congratulations!", $"You guessed the word: {chosenWord}", "OK");
                StartNewGame();
                gameOver = true; // Set game over flag
            }

            GuessEntry.Text = "";
        }
    }
}
