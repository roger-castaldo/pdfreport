using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace Org.Reddragonit.PDFReports.PDF.PageComponents
{
    internal class PDFRectangle : ADocumentPiece
    {
        private PDFPen _pen;
        private double _x;
        private double _y;
        private double _width;
        private double _height;
        private string _stream;

        public PDFRectangle(PDFPen pen, double x, double y, double width, double height, int objectID)
            : base(objectID)
        {
            _pen = pen;
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
            _stream = _pen.Color.ToString() + "\r\n";
            _stream += "q\r\n" + _pen.ToString() +
            string.Format("\r\n\r\n{0:0.###} {1:0.###} m\r\n", _x, _y) +
            string.Format("{0:0.###} {1:0.###} l\r\n", _x+_width, _y) +
            string.Format("{0:0.###} {1:0.###} l\r\n", _x + _width, _y-_height) +
            string.Format("{0:0.###} {1:0.###} l\r\n", _x, _y-_height) +
            string.Format("{0:0.###} {1:0.###} l\r\n", _x, _y) +
            "S\r\nQ\r\n";
            bw.Write("/Length " + _stream.Length.ToString() + "\n");
        }
    }
}
