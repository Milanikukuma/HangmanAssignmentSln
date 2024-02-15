using System;
using System.Linq;
using System.Threading.Tasks;

namespace HangmanAssignment
{
    public partial class HangmanGamePage : ContentPage
    {
        private int clickCount = 0;
        private const int MaxInputLength = 11;
        private int incorrectAttempts = 0;
        private string[] wordsArray;
        private string currentWord;
        private int currentHangmanImageIndex = 1;

        public HangmanGamePage()
        {
            InitializeComponent();
            wordsArray = new string[] { "apple", "banana", "orange", "grape", "kiwi" };

            SetNextWord();
            _ = AnimateWord();
        }

        private void SetNextWord()
        {
            clickCount = 0;
            incorrectAttempts = 0;

            currentWord = GetNextWord();
            wordLabel.Text = string.Empty;
        }

        private string GetNextWord()
        {
            Random random = new Random();
            int index = random.Next(wordsArray.Length);
            return wordsArray[index];
        }

        private async Task AnimateWord()
        {
            string displayWord = GetDisplayWord(currentWord);

            for (int i = 0; i < currentWord.Length; i++)
            {
                displayWord = displayWord.Substring(0, i) + currentWord[i] + displayWord.Substring(i + 1);
                wordLabel.Text = displayWord;

                await Task.Delay(5);
            }

            displayWord = GetDisplayWordWithTwoMissing(currentWord);
            wordLabel.Text = displayWord;
        }

        private string GetDisplayWord(string word)
        {
            char[] displayChars = new char[word.Length];
            for (int i = 0; i < word.Length; i++)
            {
                if (char.IsLetter(word[i]))
                {
                    displayChars[i] = (i % 3 == 0) ? word[i] : '_';
                }
                else
                {
                    displayChars[i] = word[i];
                }
            }

            return new string(displayChars);
        }

        private string GetDisplayWordWithTwoMissing(string word)
        {
            char[] displayChars = new char[word.Length];
            int missingCount = 0;

            for (int i = 0; i < word.Length; i++)
            {
                if (char.IsLetter(word[i]))
                {
                    if (missingCount < 2)
                    {
                        displayChars[i] = '_';
                        missingCount++;
                    }
                    else
                    {
                        displayChars[i] = word[i];
                    }
                }
                else
                {
                    displayChars[i] = word[i];
                }
            }

            return new string(displayChars);
        }

        private void OnGuessButtonClicked(object sender, EventArgs e)
        {
            clickCount++;

            if (clickCount == 11)
            {
                ((Button)sender).IsEnabled = false;
                DisplayGameResultAlert();
            }
            else
            {
                _ = CheckWordCorrectnessAsync();
            }
        }

        private async Task CheckWordCorrectnessAsync()
        {
            if (guessEntry.Text.ToUpper() == currentWord.ToUpper())
            {
                await DisplayAlert("Congratulations", $"You guessed it right! The word is {currentWord}. Let's move on to the next word.", "OK").ConfigureAwait(false);
                SetNextWord();
                await AnimateWord();

                currentHangmanImageIndex = 1;

                // Clear the entry for the next word
                guessEntry.Text = string.Empty;
            }
            else
            {
                HandleIncorrectGuess();
                UpdateHangmanImage();
            }
        }

        private void UpdateHangmanImage()
        {
            int maxHangmanImages = 6;
            currentHangmanImageIndex = (currentHangmanImageIndex % maxHangmanImages) + 1;
            hangmanImage.Source = $"hang{currentHangmanImageIndex}.png";
        }

        private void HandleIncorrectGuess()
        {
            incorrectAttempts++;

            if (incorrectAttempts == 11)
            {
                DisplayGameResultAlert();
            }
            else
            {
                DisplayAlert("Incorrect Guess", "Try again! You can do it!", "OK");
            }
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length > MaxInputLength)
            {
                guessEntry.IsEnabled = false;

                if (guessEntry.Text.ToUpper() == currentWord.ToUpper())
                {
                    DisplayAlert("Congratulations", "You guessed it right! Let's move on to the next word.", "OK");
                    SetNextWord();
                    _ = AnimateWord();
                    currentHangmanImageIndex = 1;
                    guessEntry.Text = string.Empty; // Clear the entry for the next word
                }
                else
                {
                    HandleIncorrectGuess();
                }
            }
            else
            {
                guessEntry.IsEnabled = true;
            }
        }

        private void DisplayGameResultAlert()
        {
            if (incorrectAttempts == 11)
            {
                DisplayAlert("Game Over", "You have died in the game", "OK");
            }
            else
            {
                DisplayAlert("Game Over", "Congratulations! You have survived the game", "OK");
            }
        }
    }
}