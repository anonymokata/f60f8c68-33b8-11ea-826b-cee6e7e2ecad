using System;

namespace PencilDurability
{
    public class Pencil
    {
        public void Write(Paper paper, string text)
        {
            paper.Text += text;
        }
    }
}
