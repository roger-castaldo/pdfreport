using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Org.Reddragonit.PDFReports.PDF;
using Org.Reddragonit.PDFReports.PDF.PageComponents;

namespace Org.Reddragonit.PDFReports
{
    public class TestPageGenerator
    {
        private const string _TEST_STRING= "The quick brown fox jumps over the lazy dog";
        private static readonly PDFPen _PEN = new PDFPen(PDFColor.Black, Utility.ExtractPointSize("1pt", GraphicsUnit.Millimeter));
//        private static double _PERCENTAGE = 0.81;

        public static byte[] GenerateTestDocument()
        {
            MemoryStream ms = new MemoryStream();
            PDFGraphics gfx = new PDFGraphics("testing", "testing");
            gfx.NewPage(GraphicsUnit.Millimeter);
            double x = 15;
            double y = 15;
            double fsize = 8;
            double pt = Utility.ExtractPointSize("2pt", GraphicsUnit.Millimeter);
            for (double cnt = 0; cnt < 5; cnt++)
            {
                PDFFont fnt = gfx.Doc.DefineFont(Fonts.Helvetica, FontStyle.Regular, fsize + cnt);
                sSize sz = gfx.MeasureString(_TEST_STRING, fnt);
                gfx.DrawString(_TEST_STRING, fnt, PDFBrush.Black, 15, y);
                gfx.DrawRectangle(_PEN, x, y, sz.Width, sz.Height);
                gfx.DrawString(sz.Width.ToString(), fnt, PDFBrush.Black, 150, y);
                y += sz.Height + pt;
                sz = gfx.MeasureString(_TEST_STRING.Replace(" ", ""), fnt);
                gfx.DrawString(_TEST_STRING.Replace(" ", ""), fnt, PDFBrush.Black, 15, y);
                gfx.DrawRectangle(_PEN, x, y, sz.Width, sz.Height);
                gfx.DrawString(sz.Width.ToString(), fnt, PDFBrush.Black, 150, y);
                y += sz.Height + pt;
                sz = gfx.MeasureString(_TEST_STRING.ToUpper(), fnt);
                gfx.DrawString(_TEST_STRING.ToUpper(), fnt, PDFBrush.Black, x, y);
                gfx.DrawRectangle(_PEN, x, y, sz.Width, sz.Height);
                gfx.DrawString(sz.Width.ToString(), fnt, PDFBrush.Black, 150, y);
                y += sz.Height + pt;
                sz = gfx.MeasureString(_TEST_STRING.ToUpper().Replace(" ", ""), fnt);
                gfx.DrawString(_TEST_STRING.ToUpper().Replace(" ", ""), fnt, PDFBrush.Black, x, y);
                gfx.DrawRectangle(_PEN, x, y, sz.Width, sz.Height);
                gfx.DrawString(sz.Width.ToString(), fnt, PDFBrush.Black, 150, y);
                y += sz.Height + pt;
                fnt = gfx.Doc.DefineFont(Fonts.Helvetica, FontStyle.Bold, fsize + cnt);
                sz = gfx.MeasureString(_TEST_STRING, fnt);
                gfx.DrawString(_TEST_STRING, fnt, PDFBrush.Black, 15, y);
                gfx.DrawRectangle(_PEN, x, y, sz.Width, sz.Height);
                gfx.DrawString(sz.Width.ToString(), fnt, PDFBrush.Black, 150, y);
                y += sz.Height + pt;
                sz = gfx.MeasureString(_TEST_STRING.Replace(" ", ""), fnt);
                gfx.DrawString(_TEST_STRING.Replace(" ", ""), fnt, PDFBrush.Black, 15, y);
                gfx.DrawRectangle(_PEN, x, y, sz.Width, sz.Height);
                gfx.DrawString(sz.Width.ToString(), fnt, PDFBrush.Black, 150, y);
                y += sz.Height + pt;
                sz = gfx.MeasureString(_TEST_STRING.ToUpper(), fnt);
                gfx.DrawString(_TEST_STRING.ToUpper(), fnt, PDFBrush.Black, x, y);
                gfx.DrawRectangle(_PEN, x, y, sz.Width, sz.Height);
                gfx.DrawString(sz.Width.ToString(), fnt, PDFBrush.Black, 150, y);
                y += sz.Height + pt;
                sz = gfx.MeasureString(_TEST_STRING.ToUpper().Replace(" ", ""), fnt);
                gfx.DrawString(_TEST_STRING.ToUpper().Replace(" ", ""), fnt, PDFBrush.Black, x, y);
                gfx.DrawRectangle(_PEN, x, y, sz.Width, sz.Height);
                gfx.DrawString(sz.Width.ToString(), fnt, PDFBrush.Black, 150, y);
                y += sz.Height + pt;
            }
            gfx.NewPage();
            PDFColor color = new PDFColor(255, 255, 0, 0);
            for (int col = 10; col < gfx.PageSize.Width; col += 10)
                gfx.DrawLine(new PDFPen(color, pt / 2), col, 10, col, gfx.PageSize.Height);
            for (int row = 10; row < gfx.PageSize.Height; row += 10)
                gfx.DrawLine(new PDFPen(color, pt / 2), 10, row, gfx.PageSize.Width, row);
            PDFFont f = gfx.Doc.DefineFont(Fonts.Helvetica, FontStyle.Regular, 8);
            gfx.DrawString(_TEST_STRING, f, PDFBrush.Black, 10, 10);
            gfx.DrawString(gfx.MeasureString(_TEST_STRING,f).Height.ToString(), f, PDFBrush.Black, 10, 20);
            gfx.Save(ms);
            return ms.ToArray();
        }

        private static double _CharacterWidth(string text, PDFFont font, PDFGraphics gfx)
        {
            double ret = 0;
            foreach (char c in text.ToCharArray())
                ret += gfx.MeasureString(c.ToString(), font).Width;
            return ret;
        }
    }
}
