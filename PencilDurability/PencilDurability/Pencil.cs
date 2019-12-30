using System;
using System.Text;
using System.Text.RegularExpressions;

namespace PencilDurability
{
    public class Pencil
    {
        private readonly int _originalDurability;
        private const int _LowercaseDegradeValue = 1;
        private const int _UppercaseDegradeValue = 2;
        private const string _WriteFailedCharacter = " ";
        private const char _EraseReplacementCharacter = ' ';

        public Pencil(int durability, int length)
        {
            _originalDurability = durability;
            CurrentDurability = durability;
            CurrentLength = length;
        }

        public int CurrentDurability { get; private set; }

        public int CurrentLength { get; private set; }

        public void Write(Paper paper, string text)
        {
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                string currentCharacter = text[i].ToString();
                bool canWrite = AdjustDurability(currentCharacter);

                const string matchNonWhitespace = @"\S";
                if (!canWrite && Regex.IsMatch(currentCharacter, matchNonWhitespace))
                {
                    stringBuilder.Append(_WriteFailedCharacter);
                }
                else
                {
                    stringBuilder.Append(currentCharacter);
                }
            }

            paper.Text += stringBuilder.ToString();
        }

        public void Sharpen()
        {
            if (CurrentLength <= 0)
            {
                return;
            }

            CurrentLength--;
            CurrentDurability = _originalDurability;
        }

        public void Erase(Paper paper, string text)
        {
            paper.Text = new string(_EraseReplacementCharacter, text.Length);
        }

        private bool AdjustDurability(string currentLetter)
        {
            bool isUppercase = Regex.IsMatch(currentLetter, "[A-Z]");
            if (isUppercase && CurrentDurability >= _UppercaseDegradeValue)
            {
                CurrentDurability -= _UppercaseDegradeValue;
                return true;
            }

            bool isLowercase = Regex.IsMatch(currentLetter, "[a-z]");
            if (isLowercase && CurrentDurability >= _LowercaseDegradeValue)
            {
                CurrentDurability -= _LowercaseDegradeValue;
                return true;
            }

            return false;
        }
    }
}
