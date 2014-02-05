using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.PDF.PageComponents;
using System.IO;
using System.Drawing;
using Org.Reddragonit.PDFReports.ReportElements;

namespace Org.Reddragonit.PDFReports.PDF
{
    internal class PDFGraphics
    {

        private PDFPage _curPage;
        private PDFDocument _doc;
        public PDFDocument Doc
        {
            get { return _doc; }
        }

        private Graphics gfx;
        private sSize _pageSize;
        public sSize PageSize
        {
            get { return _pageSize; }
        }

        public PDFGraphics(string title, string author)
        {
            _doc = new PDFDocument(title, author);
        }

        public PDFGraphics(PDFDocument doc)
        {
            _doc = doc;
        }

        public GraphicsUnit PageUnit
        {
            get { return _curPage.Unit; }
        }

        #region NewPage
        public void NewPage()
        {
            NewPage(PageSizes.Letter);
        }

        public void NewPage(PageSizes size)
        {
            NewPage(size, false);
        }

        public void NewPage(PageSizes size,bool landscape)
        {
            NewPage(size, GraphicsUnit.Point,landscape);
        }

        public void NewPage(GraphicsUnit unit)
        {
            NewPage(unit,false);
        }

        public void NewPage(GraphicsUnit unit, bool landscape)
        {
            NewPage(PageSizes.Letter, unit, landscape);
        }

        public void NewPage(PageSizes size, GraphicsUnit unit)
        {
            NewPage(size, unit, false);
        }

        public void NewPage(PageSizes size, GraphicsUnit unit,bool landscape)
        {
            _curPage = _doc.NewPage(size,unit,landscape);
            _pageSize = new sSize(Utility.ExtractPointSize(_curPage.Width.ToString() + "pt", unit),
                Utility.ExtractPointSize(_curPage.Height.ToString() + "pt", unit));
            gfx = Graphics.FromImage(new Bitmap(10, 10));
            switch (unit)
            {
                case GraphicsUnit.Centimeter:
                case GraphicsUnit.Millimeter:
                    gfx.PageUnit = System.Drawing.GraphicsUnit.Millimeter;
                    break;
                case GraphicsUnit.Inch:
                    gfx.PageUnit = System.Drawing.GraphicsUnit.Inch;
                    break;
                case GraphicsUnit.Point:
                    gfx.PageUnit = System.Drawing.GraphicsUnit.Point;
                    break;
            }
        }
        #endregion

        public void DrawLine(PDFPen pen, double x, double y, double x2, double y2)
        {
            _doc.DrawLine(_curPage, new PDFPen(pen.Color,Utility.ConvertToPoint(pen.Width,PageUnit)),
                Utility.ConvertToPoint(x, PageUnit),
                _curPage.Height - Utility.ConvertToPoint(y, PageUnit),
                Utility.ConvertToPoint(x2, PageUnit),
                _curPage.Height - Utility.ConvertToPoint(y2, PageUnit));
        }

        public void DrawRectangle(PDFPen pen, double x,double y,double width,double height)
        {
            _doc.DrawRectangle(_curPage, new PDFPen(pen.Color, Utility.ConvertToPoint(pen.Width, PageUnit)),
                Utility.ConvertToPoint(x, PageUnit),
                _curPage.Height - Utility.ConvertToPoint(y, PageUnit),
                Utility.ConvertToPoint(width, PageUnit),
                Utility.ConvertToPoint(height, PageUnit));
        }

        public void FillRectangle(PDFColor color, double x, double y, double width, double height)
        {
            _doc.FillRectangle(_curPage,color,
                Utility.ConvertToPoint(x, PageUnit),
                _curPage.Height - Utility.ConvertToPoint(y, PageUnit),
                Utility.ConvertToPoint(width, PageUnit),
                Utility.ConvertToPoint(height, PageUnit));
        }

        public void DrawString(string text, PDFFont font, PDFBrush brush, double x, double y)
        {
            sSize sz = MeasureString(text, font);
            x += Utility.ExtractPointSize("1pt", PageUnit);
            y += sz.Height;
            _doc.DrawString(_curPage, text, font, brush,
                Utility.ConvertToPoint(x, PageUnit),
                _curPage.Height - Utility.ConvertToPoint(y-(sz.Height/4), PageUnit));
            switch (font.Decoration)
            {
                case TextDecoration.Strikeout:
                    DrawLine(new PDFPen(brush.Color, Utility.ExtractPointSize((font.Style==FontStyle.Bold || font.Style==FontStyle.BoldItalic ? "2pt" : "1pt"), _curPage.Unit)), x - Utility.ExtractPointSize("1pt", _curPage.Unit),
                        y + (sz.Height/2),
                        x + Utility.ExtractPointSize("1pt", _curPage.Unit) + sz.Width,
                        y + (sz.Height/2));
                    break;
                case TextDecoration.Underline:
                    DrawLine(new PDFPen(brush.Color, Utility.ExtractPointSize((font.Style == FontStyle.Bold || font.Style == FontStyle.BoldItalic ? "2pt" : "1pt"), _curPage.Unit)), x - Utility.ExtractPointSize("1pt", _curPage.Unit),
                        y - Utility.ExtractPointSize((font.Style == FontStyle.Bold || font.Style == FontStyle.BoldItalic ? "3pt" : "1.5pt"), _curPage.Unit), 
                        x+Utility.ExtractPointSize("1pt",_curPage.Unit)+MeasureString(text,font).Width,
                        y - Utility.ExtractPointSize((font.Style == FontStyle.Bold || font.Style == FontStyle.BoldItalic ? "3pt" : "1.5pt"), _curPage.Unit));
                    break;
            }
        }

        public void DrawImage(Image img, double x, double y, double width, double height)
        {
            _doc.DrawImage(_curPage,img,
                Utility.ConvertToPoint(x, PageUnit),
                _curPage.Height - Utility.ConvertToPoint(y, PageUnit),
                Utility.ConvertToPoint(width, PageUnit),
                Utility.ConvertToPoint(0 - height, PageUnit));
        }

        public void DrawImage(Image img, double x, double y)
        {
            _doc.DrawImage(_curPage, img,
                Utility.ConvertToPoint(x, PageUnit),
                _curPage.Height - Utility.ConvertToPoint(y, PageUnit));
        }

        private static float _COURIER_WIDTH_PERCENTAGE = 0.72f;
        private static float _TIMES_WIDTH_PERCENTAGE = 0.555f;
        private static float _HELVETICA_WIDTH_PERCENTAGE = 0.595f;
        private static readonly List<char> _UPPERCHARS = new List<char>("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray());

        public sSize MeasureString(string text, PDFFont font)
        {
            float width = 0;
            float height = 0;
            string mtext = text;
            float factor = 1;
            float upperFactor = 1;
            Font f = new Font(font.Font.ToString(), (float)font.Size, System.Drawing.FontStyle.Regular,System.Drawing.GraphicsUnit.Point);
            switch (font.Font)
            {
                case Fonts.Courier:
                    width = gfx.MeasureString("_", f).Width * 0.91f;
                    width = width * (mtext.Length - mtext.Replace(" ", "").Length);
                    factor = _COURIER_WIDTH_PERCENTAGE;
                    if (font.Style != FontStyle.Regular)
                    {
                        switch (font.Style)
                        {
                            case FontStyle.Bold:
                            case FontStyle.BoldItalic:
                                factor += 0.045f;
                                width *= 1.2f;
                                break;
                        }
                    }
                    mtext = mtext.ToLower().Replace(" ", "");
                    break;
                case Fonts.TimesRoman:
                    factor = _TIMES_WIDTH_PERCENTAGE;
                    if (font.Style != FontStyle.Regular)
                    {
                        switch (font.Style)
                        {
                            case FontStyle.Bold:
                            case FontStyle.BoldItalic:
                                factor += 0.025f;
                                break;
                        }
                        f = new Font(font.Font.ToString(), (float)font.Size, (System.Drawing.FontStyle)Enum.Parse(typeof(System.Drawing.FontStyle), font.Style.ToString()), System.Drawing.GraphicsUnit.Point);
                    }
                    width = gfx.MeasureString(" ", f).Width * 1.33f;
                    width = width * (mtext.Length - mtext.Replace(" ", "").Length);
                    mtext = mtext.Replace(" ", "");
                    upperFactor = 1.17f;
                    break;
                case Fonts.Helvetica:
                    factor = _HELVETICA_WIDTH_PERCENTAGE;
                    upperFactor = 1.12f;
                    if (font.Style != FontStyle.Regular)
                    {
                        switch (font.Style)
                        {
                            case FontStyle.Bold:
                            case FontStyle.BoldItalic:
                                factor += 0.025f;
                                upperFactor -= 0.045f;
                                break;
                        }
                        f = new Font(font.Font.ToString(), (float)font.Size, (System.Drawing.FontStyle)Enum.Parse(typeof(System.Drawing.FontStyle), font.Style.ToString()), System.Drawing.GraphicsUnit.Point);
                    }
                    width = gfx.MeasureString(" ", f).Width * 1.44f;
                    width = width * (mtext.Length - mtext.Replace(" ", "").Length);
                    mtext = mtext.Replace(" ", "");
                    break;
            }
            foreach (char c in mtext.ToCharArray())
            {
                SizeF sf = gfx.MeasureString(c.ToString(),f);
                width += sf.Width*(_UPPERCHARS.Contains(c) ? upperFactor : 1);
                height = Math.Max(height,sf.Height+(float)(font.Decoration==TextDecoration.Underline ? Utility.ExtractPointSize((font.Style == FontStyle.Bold || font.Style == FontStyle.BoldItalic ? "4pt" : "2pt"),PageUnit) : 0));
            }
            width = width * factor;
            return (PageUnit == GraphicsUnit.Centimeter ? new sSize(width / 10, height / 10) : new sSize(width, height));
        }

        public void Save(Stream strm)
        {
            StreamWriter bw = new StreamWriter(strm);
            _doc.Append(bw);
            bw.Flush();
            bw.Close();
        }

    }
}
