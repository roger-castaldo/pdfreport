using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Org.Reddragonit.PDFReports.PDF.PageComponents
{
    internal class PDFBrush : IComponent
    {
        public static readonly PDFBrush Black = new PDFBrush(PDFColor.Black);

        private PDFColor _color;
        public PDFColor Color
        {
            get { return _color; }
        }

        public PDFBrush(PDFColor color)
        {
            _color = color;
        }

        #region IComponent Members

        public long Offset
        {
            get { return 0; }
        }

        public void Append(StreamWriter bw)
        {
            _color.Append(bw);
        }

        #endregion

        public override string ToString()
        {
            return _color.ToString();
        }
    }
}
