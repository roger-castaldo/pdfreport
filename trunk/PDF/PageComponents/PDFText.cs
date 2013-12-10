using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Org.Reddragonit.PDFReports.PDF.PageComponents
{/* /S /URL {0}  to add url, I think, must try*/
    internal class PDFText : ADocumentPiece
    {
        private PDFFont _font;
        public PDFFont Font
        {
            get { return _font; }
        }

        private PDFBrush _brush;
        private double _x;
        private double _y;
        private string _content;
        private StringBuilder _hexContent;

        public PDFText(PDFFont font, PDFBrush brush, double x, double y,string text,int objectID)
            : base(objectID)
        {
            _font = font;
            _brush = brush;
            _x = x;
            _y = y;
            _content = text;
        }

        protected override void _AppendContent(StreamWriter bw)
        {
            bw.Write("/Filter [/ASCIIHexDecode]"+(char)13);
            _hexContent = new StringBuilder();
            _hexContent.Append("q"+(char)13);
            _hexContent.Append("BT"+(char)13);
            _hexContent.Append(string.Format("/F{0} {1:0.###} Tf"+(char)13, _font.FontNumber, _font.Size));
            _hexContent.Append(_brush.ToString().ToLower() + ""+(char)13);
            _hexContent.Append(string.Format("{0:0.###} {1:0.###} Td"+(char)13,_x,_y));
            _hexContent.Append("("+_content.Replace("\\","\\\\").Replace(")","\\)").Replace("(","\\(")+") Tj"+(char)13);
            _hexContent.Append("ET"+(char)13);
            _hexContent.Append("Q");
            bw.Write("/Length " + ((_hexContent.Length * 2) + 1).ToString() + ""+(char)13);
        }

        public override void Append(StreamWriter bw)
        {
            base.Append(bw);
            bw.Flush();
            bw.BaseStream.Position -= ("endobj"+(char)13).Length;
            bw.Write("stream"+(char)13);
            bw.Write(_encodeHEX(_hexContent.ToString()) + ">"+(char)13);
            bw.Write("endstream"+(char)13);
            bw.Write("endobj"+(char)13);
        }

        private static string _encodeHEX(string strText)
        {
            char[] arrChar = strText.ToCharArray();
            string hexFormattedString = "";
            string appoStr;
            for (int i = 0; i < arrChar.Length; i++)
            {
                if ((arrChar[i] >= 0) && (arrChar[i] <= 255))
                {
                    appoStr = (Convert.ToByte(arrChar[i])).ToString("X");
                    if (appoStr.Length < 2)
                    {
                        appoStr = "0" + appoStr;
                    }
                    hexFormattedString += appoStr;
                }
            }
            return hexFormattedString;
        }
    }
}
