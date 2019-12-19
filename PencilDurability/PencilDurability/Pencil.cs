using System;

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
            if (string.IsNullOrWhiteSpace(text))
            {
                paper.Text = text;
            }
            else if (!_isDullable || _durability > 0)
            {
                paper.Text += text;
            }
            else
            {
                paper.Text += new string(' ', text.Length);
            }
        }
    }
}
