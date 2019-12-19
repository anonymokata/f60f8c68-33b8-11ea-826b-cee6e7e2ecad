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
                if (Regex.IsMatch(currentLetter, "[A-Z]"))
                {
                    _durability -= _UppercaseDegradeValue;
                }
                else if (Regex.IsMatch(currentLetter, "[a-z]"))
                {
                    _durability -= _LowercaseDegradeValue;
                }
            }

            if (!_isDullable || _durability > 0)
            {
                paper.Text += text;
            }
            else
            {
                const string matchNonWhitespace = @"\S";
                paper.Text += Regex.Replace(text, matchNonWhitespace, replacement: " ");
            }
        }
    }
}
