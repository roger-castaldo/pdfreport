using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.Interfaces;
using Org.Reddragonit.PDFReports.Exceptions;
using System.Xml;
using Org.Reddragonit.PDFReports.PDF;
using System.Drawing;
using System.Drawing.Imaging;
using Org.Reddragonit.PDFReports.ReportElements;
using Org.Reddragonit.PDFReports.PDF.PageComponents;
using System.Text.RegularExpressions;

namespace Org.Reddragonit.PDFReports.PageElements
{
    internal class ReportBarCode : PageElement
    {
        private string _code;
        private const int _narrowWidth = 3;
        private const int _wideWidth = 6;
        private const int _blankGap = _narrowWidth;
        private const int _height = 30;
        private const int _margin = _narrowWidth;
        private const int _fontHeight = 10;

        private static readonly Dictionary<char, byte[]> _chars = new Dictionary<char, byte[]>()
        {
            {'0',new byte[]{0,0,0,1,1,0,1,0,0}},
            {'1',new byte[]{1,0,0,1,0,0,0,0,1}},
            {'2',new byte[]{0,0,1,1,0,0,0,0,1}},
            {'3',new byte[]{1,0,1,1,0,0,0,0,0}},
            {'4',new byte[]{0,0,0,1,1,0,0,0,1}},
            {'5',new byte[]{1,0,0,1,1,0,0,0,0}},
            {'6',new byte[]{0,0,1,1,1,0,0,0,0}},
            {'7',new byte[]{0,0,0,1,0,0,1,0,1}},
            {'8',new byte[]{1,0,0,1,0,0,1,0,0}},
            {'9',new byte[]{0,0,1,1,0,0,1,0,0}},
            {'A',new byte[]{1,0,0,0,0,1,0,0,1}},
            {'B',new byte[]{0,0,1,0,0,1,0,0,1}},
            {'C',new byte[]{1,0,1,0,0,1,0,0,0}},
            {'D',new byte[]{0,0,0,0,1,1,0,0,1}},
            {'E',new byte[]{1,0,0,0,1,1,0,0,0}},
            {'F',new byte[]{0,0,1,0,1,1,0,0,0}},
            {'G',new byte[]{0,0,0,0,0,1,1,0,1}},
            {'H',new byte[]{1,0,0,0,0,1,1,0,0}},
            {'I',new byte[]{0,0,1,0,0,1,1,0,0}},
            {'J',new byte[]{0,0,0,0,1,1,1,0,0}},
            {'K',new byte[]{1,0,0,0,0,0,0,1,1}},
            {'L',new byte[]{0,0,1,0,0,0,0,1,1}},
            {'M',new byte[]{1,0,1,0,0,0,0,1,0}},
            {'N',new byte[]{0,0,0,0,1,0,0,1,1}},
            {'O',new byte[]{1,0,0,0,1,0,0,1,0}},
            {'P',new byte[]{0,0,1,0,1,0,0,1,0}},
            {'Q',new byte[]{0,0,0,0,0,0,1,1,1}},
            {'R',new byte[]{1,0,0,0,0,0,1,1,0}},
            {'S',new byte[]{0,0,1,0,0,0,1,1,0}},
            {'T',new byte[]{0,0,0,0,1,0,1,1,0}},
            {'U',new byte[]{1,1,0,0,0,0,0,0,1}},
            {'V',new byte[]{0,1,1,0,0,0,0,0,1}},
            {'W',new byte[]{1,1,1,0,0,0,0,0,0}},
            {'X',new byte[]{0,1,0,0,1,0,0,0,1}},
            {'Y',new byte[]{1,1,0,0,1,0,0,0,0}},
            {'Z',new byte[]{0,1,1,0,1,0,0,0,0}},
            {'-',new byte[]{0,1,0,0,0,0,1,0,1}},
            {'.',new byte[]{1,1,0,0,0,0,1,0,0}},
            {' ',new byte[]{0,1,1,0,0,0,1,0,0}},
            {'$',new byte[]{0,1,0,1,0,1,0,0,0}},
            {'/',new byte[]{0,1,0,1,0,0,0,1,0}},
            {'+',new byte[]{0,1,0,0,0,1,0,1,0}},
            {'%',new byte[]{0,0,0,1,0,1,0,1,0}},
	        {'*',new byte[]{0,1,0,0,1,0,1,0,0}}
        };

        private static readonly Regex _regInvalidCode = new Regex("[^0-9A-Z \\-\\.$/+%]", RegexOptions.ECMAScript | RegexOptions.Compiled);

        public ReportBarCode(XmlNode node, ReportPage page)
            : base(node, page)
        {
            if (node.Name != "Barcode")
                throw new UnexpectedElementException("Barcode", node.Name, node);
        }

        protected override void _LoadData(XmlNode node)
        {
            _code = node.InnerText.Trim(new char[] { '\r', '\n', '\t', ' ', '*' });
            if (_regInvalidCode.IsMatch(_code))
                throw new ArgumentException("Invalid character encountered in specified code.");

            _code = "*" + _code + "*";
        }

        internal override void AppendToPage(ref PDFGraphics gfx, out double width, out double height)
        {
            bool drawText = (this["DrawText"] != null ? bool.Parse(this["DrawText"]) : false);
            int w = _margin*2;
            for (int i = 0; i < _code.Length; i++)
            {
                byte[] insByteArray = _chars[_code[i]];
                foreach (byte insByte in insByteArray)
                    w += (insByte == 1 ? _wideWidth : _narrowWidth);
                if (i + 1 != _code.Length)
                    w += _blankGap;
            }

            int hgt = (this["BarHeight"] != null ? int.Parse(this["BarHeight"]) : _height);
            int h = 2 * _margin + hgt;

            Bitmap bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.PageUnit = System.Drawing.GraphicsUnit.Pixel;
            g.FillRectangle(Brushes.White, 0, 0, w, h);

            float left = _margin;
            for (int i = 0; i < _code.Length; i++)
            {
                byte[] insByteArray = _chars[_code[i]];
                int index = 0;
                foreach (byte insByte in insByteArray)
                {
                    if (index % 2 == 0)
                        g.FillRectangle(Brushes.Black, left,_margin,(insByte == 1 ? _wideWidth : _narrowWidth), hgt);
                    left += (insByte == 1 ? _wideWidth : _narrowWidth);
                    index++;
                }
                left += _blankGap;
            }
            g.PageUnit = System.Drawing.GraphicsUnit.Pixel;

            width = Utility.ExtractPointSize((w*g.PageScale).ToString() + "pt", gfx.PageUnit);
            height = Utility.ExtractPointSize((h*g.PageScale).ToString() + "pt", gfx.PageUnit);
            if (this["Width"] != null)
                width = float.Parse(this["Width"]);
            if (this["Height"] != null)
                height = float.Parse(this["Height"]);

            gfx.DrawImage(bmp, X, Y, width, height);

            if (drawText)
            {
                PDFFont fnt = gfx.Doc.DefineFont(Fonts.TimesRoman, FontStyle.Regular, _fontHeight, TextDecoration.None);
                sSize size= gfx.MeasureString(_code,fnt);
                gfx.DrawString(_code, fnt, PDFBrush.Black, X + ((width-size.Width) / 2), Y + height);
                height += size.Height;
            }
        }
    }
}
