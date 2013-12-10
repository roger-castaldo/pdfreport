using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Org.Reddragonit.PDFReports.PDF.PageComponents
{
    internal class PDFLine : ADocumentPiece
    {
        private PDFPen _pen;
        private double _x;
        private double _y;
        private double _x2;
        private double _y2;
        private string _stream;

        public PDFLine(PDFPen pen, double x, double y, double x2, double y2, int objectID)
            :base(objectID)
        {
            _pen = pen;
            _x = x;
            _y = y;
            _x2 = x2;
            _y2 = y2;
        }

        public override void Append(StreamWriter bw)
        {
            base.Append(bw);
            bw.Flush();
            bw.BaseStream.Position -= "endobj\n".Length;
            bw.Write("stream\r\n");
            bw.Write(_stream+"\r\n");
            bw.Write("endstream\r\n");
            bw.Write("endobj\r\n");
        }

        protected override void _AppendContent(StreamWriter bw)
        {
            _stream = _pen.Color.ToString()+"\r\n"; 
            _stream += "q\r\n"+_pen.ToString()+
            string.Format("\r\n\r\n{0:0.###} {1:0.###} m\r\n", _x, _y)+
            string.Format("{0:0.###} {1:0.###} l\r\n", _x2, _y2) +
            "S\r\nQ\r\n";
            bw.Write("/Length " + _stream.Length.ToString()+"\r\n");
        }
    }
}
