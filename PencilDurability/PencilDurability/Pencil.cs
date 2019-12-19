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
            _durability -= text.Length;

            if (Regex.IsMatch(text, "[A-Z]"))
            {
                _durability--;
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
