using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Org.Reddragonit.PDFReports.PDF.PageComponents
{
    internal class PDFPen : IComponent
    {
        private double _width;
        public double Width
        {
            get { return _width; }
        }
        private PDFColor _color;
        public PDFColor Color
        {
            get { return _color; }
        }

        public PDFPen(PDFColor color, double width)
        {
            _color = color;
            _width = width;
        }

        #region IComponent Members

        public long Offset
        {
            get { return 0; }
        }

        public void Append(StreamWriter bw)
        {
            bw.Write(ToString()+"\n");
            _color.Append(bw);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0:0.###} w", _width);
        }
    }
}
