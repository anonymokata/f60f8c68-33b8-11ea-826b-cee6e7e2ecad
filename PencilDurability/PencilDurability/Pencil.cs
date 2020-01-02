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
        private const string _matchNonWhitespace = @"\S";

        public Pencil(int durability, int length, int eraserDurability)
        {
            _originalDurability = durability;
            CurrentDurability = durability;
            CurrentLength = length;
            CurrentEraserDurability = eraserDurability;
        }

        public int CurrentDurability { get; private set; }

        public int CurrentLength { get; private set; }

        public int CurrentEraserDurability { get; private set; }

        public void Write(Paper paper, string text)
        {
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                string currentCharacter = text[i].ToString();
                bool canWrite = AdjustDurability(currentCharacter);

                if (!canWrite && Regex.IsMatch(currentCharacter, _matchNonWhitespace))
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
            int lastMatchIndex = paper.Text.LastIndexOf(text);

            if (lastMatchIndex < 0 || CurrentEraserDurability < 1)
            {
                return;
            }

            CurrentEraserDurability -= Regex.Matches(text, _matchNonWhitespace).Count;

            var paperText = new StringBuilder(paper.Text);
            var replacementString = new string(_EraseReplacementCharacter, text.Length);

            paperText.Replace(text, replacementString, lastMatchIndex, text.Length);

            paper.Text = paperText.ToString();
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
