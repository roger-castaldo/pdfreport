using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.PDFReports.PDF.PageComponents
{
    internal class PDFFillRectangle : ADocumentPiece
    {
        private PDFColor _color;
        private double _x;
        private double _y;
        private double _width;
        private double _height;
        private string _stream;

        public PDFFillRectangle(PDFColor color, double x, double y, double width, double height, int objectID)
            : base(objectID)
        {
            _color = color;
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public override void Append(System.IO.StreamWriter bw)
        {
            base.Append(bw);
            bw.Flush();
            bw.BaseStream.Position -= "endobj\n".Length;
            bw.Write("stream\n");
            bw.Write(_stream);
            bw.Write("\nendstream\nendobj\n");
        }

        protected override void _AppendContent(System.IO.StreamWriter bw)
        {
            _stream = "q\n"+_color.ToString()+"\n" + _color.ToString().ToLower() + "\n"
                + "0 w\n\n" + string.Format("{0:0.###} {1:0.###} {2:0.###} {3:0.###} re", new object[] { _x, _y-0.5, _width, 0-_height }) + "\nB\nQ\n";
            bw.Write("/Length "+_stream.Length.ToString()+"\n");
        }
    }
}
