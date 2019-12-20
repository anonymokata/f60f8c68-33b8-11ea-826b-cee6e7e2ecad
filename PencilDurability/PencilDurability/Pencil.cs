using System;
using System.Text.RegularExpressions;

namespace PencilDurability
{
    public class Pencil
    {

        private int _durability;
        private readonly bool _isDullable;
        private const int _LowercaseDegradeValue = 1;
        private const int _UppercaseDegradeValue = 2;

        public Pencil()
        {
            _isDullable = false;
        }

        public Pencil(int durability)
        {
            _isDullable = true;
            _durability = durability;
        }

        public int CurrentDurability => _durability;

        public void Write(Paper paper, string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                string currentLetter = text[i].ToString();
                bool canWrite = AdjustDurability(currentLetter);

                if (!_isDullable || canWrite)
                {
                    paper.Text += currentLetter;
                }
                else
                {
                    const string matchNonWhitespace = @"\S";
                    paper.Text += Regex.Replace(currentLetter, matchNonWhitespace, replacement: " ");
                }
            }
        }

        private bool AdjustDurability(string currentLetter)
        {
            bool isUppercase = Regex.IsMatch(currentLetter, "[A-Z]");
            if (isUppercase && _durability >= _UppercaseDegradeValue)
            {
                _durability -= _UppercaseDegradeValue;
                return true;
            }

            bool isLowercase = Regex.IsMatch(currentLetter, "[a-z]");
            if (isLowercase && _durability >= _LowercaseDegradeValue)
            {
                _durability -= _LowercaseDegradeValue;
                return true;
            }

            return false;
        }
    }
}
