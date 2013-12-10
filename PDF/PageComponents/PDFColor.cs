using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.PDFReports.PDF.PageComponents
{
    internal class PDFColor : IComponent
    {
        public static readonly PDFColor Black = new PDFColor(255, 0, 0, 0);

        private int _r;
        private int _g;
        private int _b;
        private int _a;

        public PDFColor(int a, int r, int g, int b)
        {
            _a = a;
            _r = r;
            _g = g;
            _b = b;
        }

        #region IComponent Members

        public long Offset
        {
            get { return 0; }
        }

        public void Append(System.IO.StreamWriter bw)
        {
            bw.Write(ToString() + ""+(char)13);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0:0.###} {1:0.###} {2:0.###} RG",(double)_r / (double)_a, (double)_g / (double)_a, (double)_b / (double)_a);
        }

        public override bool Equals(object obj)
        {
            PDFColor col = (PDFColor)obj;
            return col._r == _r
                && col._a == _a
                && col._b == _b
                && col._g == _g;
        }
    }
}
