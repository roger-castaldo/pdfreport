using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Org.Reddragonit.PDFReports.PDF.PageComponents
{
    internal class PDFPage : ADocumentPiece
    {
        private int _pageTreeID;
        public int PageTreeID
        {
            set { _pageTreeID = value; }
        }

        private int _width;
        public int Width
        {
            get { return _width; }
        }
        private int _height;
        public int Height
        {
            get { return _height; }
        }
        private List<ADocumentPiece> _pieces;
        private PageSizes _size;
        public PageSizes Size{
            get{return _size;}
        }
        private GraphicsUnit _unit;
        public GraphicsUnit Unit{
            get { return _unit; }
        }

        public PDFPage(PageSizes size,GraphicsUnit unit,bool landscape, int objectID)
            : base(objectID)
        {
            _size = size;
            _unit = unit;
            switch (size)
            {
                case PageSizes.A0:
                    _width = (int)Utility.ConvertToPoint(33.1, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(46.8, GraphicsUnit.Inch);
                    break;
                case PageSizes.A1:
                    _width = (int)Utility.ConvertToPoint(23.4, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(33.1, GraphicsUnit.Inch);
                    break;
                case PageSizes.A2:
                    _width = (int)Utility.ConvertToPoint(16.5, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(23.4, GraphicsUnit.Inch);
                    break;
                case PageSizes.A3:
                    _width = (int)Utility.ConvertToPoint(11.7, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(16.5, GraphicsUnit.Inch);
                    break;
                case PageSizes.A4:
                case PageSizes.Letter:
                case PageSizes.Quarto:
                    _width = (int)Utility.ConvertToPoint(8.5, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(11, GraphicsUnit.Inch);
                    break;
                case PageSizes.A5:
                    _width = (int)Utility.ConvertToPoint(5.8, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(8.3, GraphicsUnit.Inch);
                    break;
                case PageSizes.RA0:
                    _width = (int)Utility.ConvertToPoint(33.0125, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(46.75, GraphicsUnit.Inch);
                    break;
                case PageSizes.RA1:
                    _width = (int)Utility.ConvertToPoint(24, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(33.9, GraphicsUnit.Inch);
                    break;
                case PageSizes.RA2:
                    _width = (int)Utility.ConvertToPoint(16.9, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(24, GraphicsUnit.Inch);
                    break;
                case PageSizes.RA3:
                    _width = (int)Utility.ConvertToPoint(12, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(16.9, GraphicsUnit.Inch);
                    break;
                case PageSizes.RA4:
                    _width = (int)Utility.ConvertToPoint(8.5, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(12, GraphicsUnit.Inch);
                    break;
                case PageSizes.RA5:
                    _width = (int)Utility.ConvertToPoint(6.02362, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(8.46457, GraphicsUnit.Inch);
                    break;
                case PageSizes.B0:
                    _width = (int)Utility.ConvertToPoint(55.7, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(39.4, GraphicsUnit.Inch);
                    break;
                case PageSizes.B1:
                    _width = (int)Utility.ConvertToPoint(39.4, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(27.8, GraphicsUnit.Inch);
                    break;
                case PageSizes.B2:
                    _width = (int)Utility.ConvertToPoint(27.8, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(19.7, GraphicsUnit.Inch);
                    break;
                case PageSizes.B3:
                    _width = (int)Utility.ConvertToPoint(19.7, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(13.9, GraphicsUnit.Inch);
                    break;
                case PageSizes.B4:
                    _width = (int)Utility.ConvertToPoint(13.9, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(9.8, GraphicsUnit.Inch);
                    break;
                case PageSizes.B5:
                    _width = (int)Utility.ConvertToPoint(9.8, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(6.9, GraphicsUnit.Inch);
                    break;
                case PageSizes.Foolscap:
                case PageSizes.Folio:
                    _width = (int)Utility.ConvertToPoint(8, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(13, GraphicsUnit.Inch);
                    break;
                case PageSizes.Executive:
                    _width = (int)Utility.ConvertToPoint(7.25, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(10.5, GraphicsUnit.Inch);
                    break;
                case PageSizes.GovernmentLetter:
                    _width = (int)Utility.ConvertToPoint(8, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(10.5, GraphicsUnit.Inch);
                    break;
                case PageSizes.Legal:
                    _width = (int)Utility.ConvertToPoint(8.5, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(14, GraphicsUnit.Inch);
                    break;
                case PageSizes.Ledger:
                case PageSizes.Tabloid:
                    _width = (int)Utility.ConvertToPoint(11, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(17, GraphicsUnit.Inch);
                    break;
                case PageSizes.Post:
                    _width = (int)Utility.ConvertToPoint(15.5, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(19.5, GraphicsUnit.Inch);
                    break;
                case PageSizes.Crown:
                    _width = (int)Utility.ConvertToPoint(15, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(20, GraphicsUnit.Inch);
                    break;
                case PageSizes.LargePost:
                    _width = (int)Utility.ConvertToPoint(16.5, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(21, GraphicsUnit.Inch);
                    break;
                case PageSizes.Demy:
                    _width = (int)Utility.ConvertToPoint(17.5, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(22.5, GraphicsUnit.Inch);
                    break;
                case PageSizes.Medium:
                    _width = (int)Utility.ConvertToPoint(18, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(23, GraphicsUnit.Inch);
                    break;
                case PageSizes.Royal:
                    _width = (int)Utility.ConvertToPoint(20, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(25, GraphicsUnit.Inch);
                    break;
                case PageSizes.Elephant:
                    _width = (int)Utility.ConvertToPoint(23, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(28, GraphicsUnit.Inch);
                    break;
                case PageSizes.DoubleDemy:
                    _width = (int)Utility.ConvertToPoint(22.5, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(35, GraphicsUnit.Inch);
                    break;
                case PageSizes.QuadDemy:
                    _width = (int)Utility.ConvertToPoint(35, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(45, GraphicsUnit.Inch);
                    break;
                case PageSizes.Statement:
                    _width = (int)Utility.ConvertToPoint(5.5, GraphicsUnit.Inch);
                    _height = (int)Utility.ConvertToPoint(8.5, GraphicsUnit.Inch);
                    break;
            }
            if (landscape)
            {
                int tmp = _width;
                _width = _height;
                _height = tmp;
            }
            _pieces = new List<ADocumentPiece>();
        }

        public void AddPiece(ADocumentPiece piece)
        {
            _pieces.Add(piece);
        }

        private static readonly char[] _IMGCHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        protected override void _AppendContent(StreamWriter bw)
        {
            bw.Write("/Type /Page"+(char)13);
            bw.Write("/Parent " + _pageTreeID.ToString() + " 0 R"+(char)13);
            bw.Write("/Resources <<");
            foreach (ADocumentPiece adp in _pieces)
            {
                if (adp is PDFText)
                {
                    bw.Write("/Font <<");
                    List<string> fonts = new List<string>();
                    foreach (ADocumentPiece a in _pieces)
                    {
                        if (a is PDFText)
                        {
                            PDFFont fnt = ((PDFText)a).Font;
                            if (!fonts.Contains("/F" + fnt.FontNumber.ToString() + " " + fnt.ObjectID.ToString() + " 0 R "))
                                fonts.Add("/F" + fnt.FontNumber.ToString() + " " + fnt.ObjectID.ToString() + " 0 R ");
                        }
                    }
                    foreach (string str in fonts)
                        bw.Write(str);
                    bw.Write(">>"+(char)13);
                    break;
                }
            }
            foreach (ADocumentPiece adp in _pieces)
            {
                if (adp is PDFImage)
                {
                    bw.Write("/XObject <<");
                    foreach (ADocumentPiece a in _pieces)
                    {
                        if (a is PDFImage)
                        {
                            PDFImage pi = (PDFImage)a;
                            bw.Write("/I" + pi.XObjectID.ToString() + " " + pi.XObjectID.ToString() + " 0 R ");
                        }
                    }
                    bw.Write(">>"+(char)13);
                    break;
                }
            }
            bw.Write(">>"+(char)13);
            bw.Write("/MediaBox [0 0 " + _width + " " + _height + "]"+(char)13);
            bw.Write("/CropBox [0 0 " + _width + " " + _height + "]"+(char)13);
            bw.Write("/Rotate 0"+(char)13);
            bw.Write("/ProcSet [/PDF /ImageC /Text]" + (char)13);
            if (_pieces.Count > 0)
            {
                bw.Write("/Contents [");
                foreach (ADocumentPiece adp in _pieces)
                    bw.Write(adp.ObjectID.ToString() + " 0 R ");
                bw.Write("]"+(char)13);
            }
        }
    }
}
