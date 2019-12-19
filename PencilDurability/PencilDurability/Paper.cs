using System;
using System.Collections.Generic;
using System.Text;

namespace PencilDurability
{
    public class Paper
    {
        private string _text = "";

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
    }
}
