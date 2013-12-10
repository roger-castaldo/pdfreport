using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Org.Reddragonit.PDFReports.PDF
{
    internal class PDFFont : ADocumentPiece
    {
        private FontStyle _style;
        public FontStyle Style
        {
            get { return _style; }
        }

        private Fonts _font;
        public Fonts Font
        {
            get { return _font; }
        }

        private double _size;
        public double Size
        {
            get { return _size; }
        }

        private int _fontNumber;
        public int FontNumber
        {
            get { return _fontNumber; }
        }

        public PDFFont(Fonts font, FontStyle style, double size, int fontNumber, int objectID)
            : base(objectID)
        {
            _font = font;
            _style = style;
            _size = size;
            _fontNumber = fontNumber;
        }

        protected override void _AppendContent(StreamWriter bw)
        {
            bw.Write("/Type /Font"+(char)13);
            bw.Write("/Subtype /Type1"+(char)13);
            bw.Write("/Name /F" + _fontNumber.ToString() + ""+(char)13);
            bw.Write("/BaseFont /");
            switch (_font)
            {
                case Fonts.Courier:
                case Fonts.Symbol:
                case Fonts.Helvetica:
                case Fonts.Zapfdingbats:
                    bw.Write(_font.ToString());
                    break;
                case Fonts.TimesRoman:
                    bw.Write("Times");
                    break;
            }
            switch(_font){
                case Fonts.Courier:
                case Fonts.Helvetica:
                    switch (_style)
                    {
                        case FontStyle.Bold:
                            bw.Write("-Bold");
                            break;
                        case FontStyle.BoldItalic:
                            bw.Write("-BoldOblique");
                            break;
                        case FontStyle.Italic:
                            bw.Write("-Oblique");
                            break;
                    }
                    break;
                case Fonts.TimesRoman:
                    switch (_style)
                    {
                        case FontStyle.Bold:
                            bw.Write("-Bold");
                            break;
                        case FontStyle.BoldItalic:
                            bw.Write("-BoldItalic");
                            break;
                            case FontStyle.Italic:
                                bw.Write("-Italic");
                                break;
                        case FontStyle.Regular:
                                bw.Write("-Roman");
                                break;
                    }
                break;
            }
            bw.Write((char)13);
            bw.Write("/Encoding /WinAnsiEncoding"+(char)13);
        }
    }
}
