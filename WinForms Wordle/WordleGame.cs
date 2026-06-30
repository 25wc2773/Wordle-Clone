using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForms_Wordle
{
    public partial class Wordle : Form
    {
        private List<string> possibleGuesses;
        private List<string> possibleAnswers;
        private Label[,] labels;
        private Dictionary<char, Button> onScreenButtons;
        private int currentRow = 0;
        private int currentCol = 0;
        private string answer;
        private StringBuilder currentGuess = new StringBuilder();
        private bool stopTakingInput = false;

        public Wordle()
        {
            InitializeComponent();

            possibleGuesses = LoadWords(@"Text Files\validGuesses.txt");
            possibleAnswers = LoadWords(@"Text Files\answers.txt");

            labels = new Label[6, 5]
            {
                {Word1Character1, Word1Character2, Word1Character3, 
                    Word1Character4, Word1Character5},
                {Word2Character1, Word2Character2, Word2Character3,
                    Word2Character4, Word2Character5},
                {Word3Character1, Word3Character2, Word3Character3,
                    Word3Character4, Word3Character5},
                {Word4Character1, Word4Character2, Word4Character3,
                    Word4Character4, Word4Character5},
                {Word5Character1, Word5Character2, Word5Character3,
                    Word5Character4, Word5Character5},
                {Word6Character1, Word6Character2, Word6Character3,
                    Word6Character4, Word6Character5},
            };

            onScreenButtons = new Dictionary<char, Button>
            {
                { 'q', QKeyButton }, { 'w', WKeyButton },
                { 'e', EKeyButton }, { 'r', RKeyButton },
                { 't', TKeyButton }, { 'y', YKeyButton },
                { 'u', UKeyButton }, { 'i', IKeyButton },
                { 'o', OKeyButton }, { 'p', PKeyButton },
                { 'a', AKeyButton }, { 's', SKeyButton },
                { 'd', DKeyButton }, { 'f', FKeyButton },
                { 'g', GKeyButton }, { 'h', HKeyButton },
                { 'j', JKeyButton }, { 'k', KKeyButton },
                { 'l', LKeyButton }, { 'z', ZKeyButton },
                { 'x', XKeyButton }, { 'c', CKeyButton },
                { 'v', VKeyButton }, { 'b', BKeyButton },
                { 'n', NKeyButton }, { 'm', MKeyButton }
};

            answer = GetAnswer();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private void Wordle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (stopTakingInput) return;

            char key = e.KeyChar;

            if (key == '\b' && currentCol > 0)
            {
                BackspaceButton();
            }
            else if (key == '\r' && currentCol > 4)
            {
                EnterButton();
                e.Handled = true;
            }
            else if (currentCol < 5 && char.IsLetter(key))
            {
                LetterKeyButton(key);
            }
        }
        public void GuessResult()
        {
            char[] yellowList = answer.ToCharArray();
            Color[] backColours = new Color[currentGuess.Length];

            for (int i = 0; i < currentGuess.Length; i++)
            {
                backColours[i] = Color.DimGray;
            }

            for (int i = 0; i < currentGuess.Length; i++)
            {
                if (currentGuess[i] == answer[i])
                {
                    backColours[i] = Color.LimeGreen;
                    yellowList[i] = ' ';
                }
            }

            for (int i = 0; i < currentGuess.Length; i++)
            {
                if (yellowList.Contains(currentGuess[i]) && backColours[i] == Color.DimGray)
                {
                    backColours[i] = Color.Goldenrod;
                    int index = Array.IndexOf(yellowList, currentGuess[i]);
                    yellowList[index] = ' ';
                }
            }

            for (int i = 0; i < currentGuess.Length; i++)
            {
                Color setColour = backColours[i];

                labels[currentRow, i].BackColor = setColour;
                labels[currentRow, i].ForeColor = Color.White;

                Color currentColour = onScreenButtons[currentGuess[i]].BackColor;

                if (currentColour != Color.LimeGreen)
                {
                    if (currentColour != Color.Goldenrod || setColour == Color.LimeGreen)
                    {
                        onScreenButtons[currentGuess[i]].BackColor = setColour;
                    }
                }

                onScreenButtons[currentGuess[i]].ForeColor = Color.White;

            }
        }
        public bool IsValidWord()
        {
            return possibleGuesses.Contains(currentGuess.ToString());
        }
        public string GetAnswer()
        {
            Random random = new Random();
            return possibleAnswers[random.Next(possibleAnswers.Count)];
        }
        public static List<string> LoadWords(string filename)
        {
            return File.ReadAllLines(filename).ToList();
        }

        private void DarkTheme_CheckedChanged(object sender, EventArgs e)
        {
            CheckDarkTheme();
        }

        private void BackspaceButton()
        {
            if (stopTakingInput) return;

            currentCol--;
            labels[currentRow, currentCol].Text = "";
            currentGuess.Remove(currentCol, 1);
        }
        private void EnterButton()
        {
            if (stopTakingInput) return;

            if (currentGuess.ToString() == answer)
            {
                GuessResult();
                MessageBox.Show($"Correct! You got it in {currentRow + 1} guesses!");
                stopTakingInput = true;
            }
            else if (!IsValidWord())
            {
                MessageBox.Show("Not in word list");
            }
            else
            {
                GuessResult();
                if (currentRow > 4)
                {
                    MessageBox.Show($"You didnt get it! The word was '{answer}'");
                }
                currentRow++;
                currentCol = 0;
                currentGuess = new StringBuilder();
            }
        }
        private void LetterKeyButton(char key)
        {
            if (stopTakingInput) return;

            labels[currentRow, currentCol].Text = key.ToString().ToUpper();
            currentGuess.Append(key);
            currentCol++;
        }
        private void EnterKeyButton_Click(object sender, EventArgs e)
        {
            if (currentCol > 4) EnterButton();
            ActiveControl = null;
        }
        private void BackKeyButton_Click(object sender, EventArgs e)
        {
            if (currentCol > 0) BackspaceButton();
            ActiveControl = null;
        }

        private void LetterButton_Click(object sender, EventArgs e)
        {
            if (currentCol > 4) return;

            Button button = (Button)sender;
            string keyString = button.Text;
            char key = keyString.ToLower()[0];

            LetterKeyButton(key);
            ActiveControl = null;
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            currentRow = 0;
            currentCol = 0;
            stopTakingInput = false;
            answer = GetAnswer();
            currentGuess = new StringBuilder();

            foreach (Label label in labels)
            {
                label.Text = "";
                label.BackColor = Color.Transparent;
                label.ForeColor = Color.Black;
            }
            foreach (Button button in onScreenButtons.Values)
            {
                button.BackColor = Color.LightGray;
                button.ForeColor = Color.Black;
            }

            CheckDarkTheme();
            ActiveControl = null;
        }

        private void CheckDarkTheme()
        {
            if (DarkTheme.Checked)
            {
                BackColor = Color.Black;
                DarkTheme.ForeColor = Color.White;
                HeaderLine.BackColor = Color.White;
                WordleTitle.ForeColor = Color.White;
                ResetButton.ForeColor = Color.White;

                foreach (Label label in labels)
                {
                    if (label.BackColor == Color.Transparent)
                        label.ForeColor = Color.White;
                }
                foreach (Button button in onScreenButtons.Values)
                {
                    if (button.BackColor == Color.LightGray)
                        button.BackColor = Color.DarkGray;
                }
                EnterKeyButton.BackColor = Color.DarkGray;
                BackKeyButton.BackColor = Color.DarkGray;
            }
            else
            {
                BackColor = Color.White;
                DarkTheme.ForeColor = Color.Black;
                HeaderLine.BackColor = Color.Silver;
                WordleTitle.ForeColor = Color.Black;
                ResetButton.ForeColor = Color.Black;

                foreach (Label label in labels)
                {
                    if (label.BackColor == Color.Transparent)
                        label.ForeColor = Color.Black;
                }
                foreach (Button button in onScreenButtons.Values)
                {
                    if (button.BackColor == Color.DarkGray)
                        button.BackColor = Color.LightGray;
                }
                EnterKeyButton.BackColor = Color.LightGray;
                BackKeyButton.BackColor = Color.LightGray;
            }
        }
    }
}
