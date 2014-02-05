using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using Org.Reddragonit.PDFReports.PDF.PageComponents;
using System.Drawing;

namespace Org.Reddragonit.PDFReports.PDF
{
    internal class PDFDocument : IComponent
    {
        internal static readonly Version _VERSION = new Version("1.4");

        private List<IComponent> _parts;
        private PDFHeader _header;
        private PDFInfo _info;
        private PDFPageTree _tree;
        private int _curObjectID = 5;
        private int _curFontID = 0;
        private string _title;
        private string _author;

        public PDFDocument(string title,string author)
        {
            _parts = new List<IComponent>();
            _title = title;
            _author = author;
        }

        #region IComponent Members

        public long Offset
        {
            get { return 0; }
        }

        public void Append(StreamWriter bw)
        {
            _tree = new PDFPageTree(_parts, 4);
            _header = new PDFHeader(_tree, 1);
            _info = new PDFInfo(_title, _author, 2);
            _curObjectID++;
            _parts.Insert(0, _tree);
            _parts.Insert(0, new PDFOutlines(3));
            _parts.Insert(0, _info);
            _parts.Insert(0, _header);
            bw.Write("%PDF-" + _VERSION.ToString() + "\n");
            int xOffset = (int)bw.BaseStream.Position;
            foreach (IComponent ic in _parts)
                ic.Append(bw);
            new PDFTrailer(_parts, _curObjectID).Append(bw);
        }

        #endregion

        internal PDFFont DefineFont(Fonts font, FontStyle style, double size,TextDecoration decoration)
        {
            PDFFont ret = null;
            foreach (IComponent ic in _parts)
            {
                if (ic is PDFFont)
                {
                    PDFFont pf = (PDFFont)ic;
                    if (pf.Font == font && pf.Style == style && pf.Decoration==decoration)
                    {
                        ret = pf;
                        break;
                    }
                }
            }
            if (ret == null)
            {
                ret = new PDFFont(font, style, size, _curFontID,decoration, _curObjectID);
                _curFontID++;
                _curObjectID++;
                _parts.Add(ret);
            }
            else
                ret = new PDFFont(font, style, size, ret.FontNumber,decoration, ret.ObjectID);
            return ret;
        }

        internal PDFPage NewPage(PageSizes size, GraphicsUnit unit,bool landscape)
        {
            PDFPage ret = new PDFPage(size, unit,landscape, _curObjectID);
            _curObjectID++;
            _parts.Add(ret);
            return ret;
        }

        internal void DrawLine(PDFPage curPage, PDFPen pen, double x, double y, double x2, double y2)
        {
            PDFLine line = new PDFLine(pen, x, y, x2, y2, _curObjectID);
            _curObjectID++;
            curPage.AddPiece(line);
            _parts.Add(line);
        }

        internal void DrawRectangle(PDFPage curPage, PDFPen pen, double x, double y, double width, double height)
        {
            PDFRectangle rect = new PDFRectangle(pen, x, y, width, height, _curObjectID);
            _curObjectID++;
            curPage.AddPiece(rect);
            _parts.Add(rect);
        }

        internal void DrawString(PDFPage curPage, string text, PDFFont font, PDFBrush brush, double x,double y)
        {
            PDFText txt = new PDFText(font, brush, x, y, text, _curObjectID);
            _curObjectID++;
            curPage.AddPiece(txt);
            _parts.Add(txt);
        }

        internal void DrawImage(PDFPage curPage, Image img, double x, double y, double width, double height)
        {
            PDFImage pimg = new PDFImage(img, x, y, width, height, _curObjectID,_curObjectID+1);
            _curObjectID+=2;
            curPage.AddPiece(pimg);
            _parts.Add(pimg);
        }

        internal void DrawImage(PDFPage curPage, Image img, double x, double y)
        {
            PDFImage pimg = new PDFImage(img, x, y, _curObjectID,_curObjectID+1);
            _curObjectID+=2;
            curPage.AddPiece(pimg);
            _parts.Add(pimg);
        }

        internal void FillRectangle(PDFPage curPage, PDFColor color, double x, double y, double width, double height)
        {
            PDFFillRectangle rect = new PDFFillRectangle(color, x, y, width, height, _curObjectID);
            _curObjectID++;
            curPage.AddPiece(rect);
            _parts.Add(rect);
        }
    }
}
