using System;
using System.Text.RegularExpressions;

namespace PencilDurability
{
    public class Pencil
    {
        private int _durability;
        private readonly bool _isDullable;

        public Pencil()
        {
            _isDullable = false;
        }

        public Pencil(int durability)
        {
            _isDullable = true;
            _durability = durability;
        }

        public void Write(Paper paper, string text)
        {
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
