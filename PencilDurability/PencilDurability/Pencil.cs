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

        public void Erase(Paper paper, string matchText)
        {
            if (CurrentEraserDurability < 1 || !paper.Text.Contains(matchText))
            {
                return;
            }

            string adjustedMatchText = matchText;
            if (GetNonWhitespaceCount(matchText) > CurrentEraserDurability)
            {
                adjustedMatchText = GetAdjustedEraseMatchText(matchText, CurrentEraserDurability);
            }

            CurrentEraserDurability -= GetNonWhitespaceCount(adjustedMatchText);

            int matchOffset = matchText.Length - adjustedMatchText.Length;
            int matchLocation = paper.Text.LastIndexOf(matchText) + matchOffset;
            var replacementString = new string(_EraseReplacementCharacter, adjustedMatchText.Length);
            var paperText = new StringBuilder(paper.Text);

            paperText.Replace(adjustedMatchText, replacementString, matchLocation, adjustedMatchText.Length);

            paper.Text = paperText.ToString();
        }

        private string GetAdjustedEraseMatchText(string matchText, int matchesNeeded)
        {
            string adjustedText = matchText.Substring(matchText.Length - matchesNeeded);
            int adjustedTextMatches = GetNonWhitespaceCount(adjustedText);

            if (adjustedTextMatches == matchesNeeded)
            {
                return adjustedText;
            }
            else
            {
                string firstHalf = matchText.Substring(0, matchText.Length - matchesNeeded);
                int newCount = matchesNeeded - adjustedTextMatches;

                return GetAdjustedEraseMatchText(firstHalf, newCount) + adjustedText;
            }
        }

        private int GetNonWhitespaceCount(string text)
        {
            return Regex.Matches(text, _matchNonWhitespace).Count;
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
