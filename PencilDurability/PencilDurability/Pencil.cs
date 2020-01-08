﻿using System;
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
        private const char _EraseReplacementCharacter = ' ';
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
            var replacementString = new string(_EraseReplacementCharacter, adjustedMatchText.Length);
            var paperText = new StringBuilder(paper.Text);

            paperText.Replace(adjustedMatchText, replacementString, matchLocation, adjustedMatchText.Length);

            paper.Text = paperText.ToString();
        }

        public void Edit(IPaper paper, string editText, int startIndex)
        {
            if (!Regex.IsMatch(editText, _matchNonWhitespace))
            {
                return;
            }

            var paperText = new StringBuilder(paper.Text);
            string textToReplace = paper.Text.Substring(startIndex, editText.Length);

            string replacementText = editText;
            if (Regex.IsMatch(textToReplace, _matchNonWhitespace))
            {
                replacementText = new string(_EditConflictCharacter, textToReplace.Length);
            }

            paperText.Remove(startIndex, replacementText.Length);
            paperText.Insert(startIndex, replacementText);

            paper.Text = paperText.ToString();
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

        private int GetNonWhitespaceCount(string text)
        {
            return Regex.Matches(text, _matchNonWhitespace).Count;
        }

        private bool AdjustPointDurability(string currentLetter)
        {
            bool isUppercase = Regex.IsMatch(currentLetter, "[A-Z]");
            if (isUppercase && CurrentPointDurability >= _UppercaseDegradeValue)
            {
                CurrentPointDurability -= _UppercaseDegradeValue;
                return true;
            }

            bool isNonWhitespace = Regex.IsMatch(currentLetter, _matchNonWhitespace);
            if (!isUppercase && isNonWhitespace && CurrentPointDurability >= _DefaultDegradeValue)
            {
                CurrentPointDurability -= _DefaultDegradeValue;
                return true;
            }

            return false;
        }
    }
}
