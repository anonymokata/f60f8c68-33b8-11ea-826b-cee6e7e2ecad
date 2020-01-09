using System;
using System.Text;
using System.Text.RegularExpressions;

namespace PencilDurability
{
    public class Pencil : IPencil
    {
        private readonly int _originalDurability;

        private const int _DefaultDegradeValue = 1;
        private const int _UppercaseDegradeValue = 2;

        private const string _WriteFailedCharacter = " ";
        private const char _FillerCharacter = ' ';
        private const char _EditConflictCharacter = '@';

        private const string _matchNonWhitespace = @"\S";
        const string _matchAllStartWhitespace = @"^\s+";

        public Pencil(int durability, int length, int eraserDurability)
        {
            _originalDurability = Math.Abs(durability);
            CurrentPointDurability = Math.Abs(durability);
            CurrentLength = Math.Abs(length);
            CurrentEraserDurability = Math.Abs(eraserDurability);
        }

        public int CurrentPointDurability { get; private set; }

        public int CurrentEraserDurability { get; private set; }

        public int CurrentLength { get; private set; }

        public void Write(IPaper paper, string text)
        {
            var stringBuilder = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                string currentCharacter = text[i].ToString();
                bool canWrite = AdjustPointDurability(currentCharacter);

                if (!canWrite && HasNonWhitespace(currentCharacter))
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

        public void Erase(IPaper paper, string matchText)
        {
            if (CurrentEraserDurability < 1 || !paper.Text.Contains(matchText))
            {
                return;
            }

            string adjustedMatchText = matchText;
            if (matchText.Length > CurrentEraserDurability)
            {
                adjustedMatchText = GetAdjustedEraseMatchText(matchText, CurrentEraserDurability);
            }

            CurrentEraserDurability -= GetNonWhitespaceCount(adjustedMatchText);

            int matchOffset = matchText.Length - adjustedMatchText.Length;
            int matchLocation = paper.Text.LastIndexOf(matchText) + matchOffset;
            var replacementString = new string(_FillerCharacter, adjustedMatchText.Length);
            var paperText = new StringBuilder(paper.Text);

            paperText.Replace(adjustedMatchText, replacementString, matchLocation, adjustedMatchText.Length);

            paper.Text = paperText.ToString();
        }

        public void Edit(IPaper paper, string editText, int startIndex)
        {
            const int minIndex = 0;
            startIndex = Math.Max(minIndex, startIndex);

            if (startIndex >= paper.Text.Length)
            {
                Write(paper, editText);
                return;
            }

            StringBuilder adjustedText = GetAdjustedPaperText(paper.Text, editText.Length, startIndex);
            string originalTextSection = adjustedText.ToString().Substring(startIndex, editText.Length);
            var replacementText = GetEditReplacementText(originalTextSection, editText);

            adjustedText.Remove(startIndex, replacementText.Length);
            adjustedText.Insert(startIndex, replacementText);

            paper.Text = adjustedText.ToString();
        }

        public void Sharpen()
        {
            if (CurrentLength <= 0)
            {
                return;
            }

            CurrentLength--;
            CurrentPointDurability = _originalDurability;
        }

        private string GetAdjustedEraseMatchText(string matchText, int matchesNeeded)
        {
            string adjustedText = matchText.Substring(matchText.Length - matchesNeeded);
            int adjustedTextMatches = GetNonWhitespaceCount(adjustedText);

            if (adjustedTextMatches == matchesNeeded)
            {
                return Regex.Replace(adjustedText, _matchAllStartWhitespace, "");
            }
            else
            {
                string firstHalf = matchText.Substring(0, matchText.Length - matchesNeeded);
                int newCount = matchesNeeded - adjustedTextMatches;

                return GetAdjustedEraseMatchText(firstHalf, newCount) + adjustedText;
            }
        }

        private StringBuilder GetAdjustedPaperText(string paperText, int editLength, int startIndex)
        {
            var adjustedText = new StringBuilder(paperText);
            int endIndex = editLength + startIndex;
            if (endIndex >= adjustedText.Length)
            {
                int overlapLength = adjustedText.Length - startIndex;
                int excessLength = editLength - overlapLength;
                adjustedText.Append(new string(_FillerCharacter, excessLength));
            }

            return adjustedText;
        }

        private string GetEditReplacementText(string originalTextSection, string editText)
        {
            var replacementText = new StringBuilder();
            for (int i = 0; i < originalTextSection.Length; i++)
            {
                string originalCharecter = originalTextSection[i].ToString();
                string editCharecter = editText[i].ToString();

                bool canWrite = AdjustPointDurability(editCharecter);

                if (!canWrite)
                {
                    return replacementText.ToString();
                }

                if (HasNonWhitespace(originalCharecter))
                {
                    if (HasNonWhitespace(editCharecter))
                    {
                        replacementText.Append(_EditConflictCharacter);
                    }
                    else
                    {
                        replacementText.Append(originalCharecter);
                    }
                }
                else
                {
                    replacementText.Append(editCharecter);
                }
            }

            return replacementText.ToString();
        }

        private int GetNonWhitespaceCount(string text)
        {
            return Regex.Matches(text, _matchNonWhitespace).Count;
        }

        private bool HasNonWhitespace(string text)
        {
            return Regex.IsMatch(text, _matchNonWhitespace);
        }

        private bool AdjustPointDurability(string currentLetter)
        {
            bool isUppercase = Regex.IsMatch(currentLetter, "[A-Z]");
            if (isUppercase && CurrentPointDurability >= _UppercaseDegradeValue)
            {
                CurrentPointDurability -= _UppercaseDegradeValue;
                return true;
            }

            if (!isUppercase && HasNonWhitespace(currentLetter) && CurrentPointDurability >= _DefaultDegradeValue)
            {
                CurrentPointDurability -= _DefaultDegradeValue;
                return true;
            }

            return false;
        }
    }
}
