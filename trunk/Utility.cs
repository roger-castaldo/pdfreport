using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using Org.Reddragonit.PDFReports.ReportElements;
using Org.Reddragonit.PDFReports.PageElements.TableComponents;
using System.Drawing;
using Org.Reddragonit.PDFReports.PDF.PageComponents;
using Org.Reddragonit.PDFReports.PDF;

namespace Org.Reddragonit.PDFReports
{
    internal static class Utility
    {
        private static readonly Regex _regRadix = new Regex("^([A-Za-z0-9+/]{4})+(([A-Za-z0-9+/]{3}=)|([A-Za-z0-9+/]{2}==))$", RegexOptions.Compiled | RegexOptions.ECMAScript);

        public static bool IsValidRadix64(string content)
        {
            return _regRadix.IsMatch(content);
        }

        public static string FindXPath(XmlNode node)
        {
            StringBuilder builder = new StringBuilder();
            while (node != null)
            {
                switch (node.NodeType)
                {
                    case XmlNodeType.Attribute:
                        builder.Insert(0, "/@" + node.Name);
                        node = ((XmlAttribute)node).OwnerElement;
                        break;
                    case XmlNodeType.Element:
                        int index = _FindElementIndex((XmlElement)node);
                        builder.Insert(0, "/" + node.Name + "[" + index + "]");
                        node = node.ParentNode;
                        break;
                    case XmlNodeType.Document:
                        return builder.ToString();
                    default:
                        throw new ArgumentException("Only elements and attributes are supported");
                }
            }
            throw new ArgumentException("Node was not in a document");
        }

        private static int _FindElementIndex(XmlElement element)
        {
            XmlNode parentNode = element.ParentNode;
            if (parentNode is XmlDocument)
            {
                return 1;
            }
            XmlElement parent = (XmlElement)parentNode;
            int index = 1;
            foreach (XmlNode candidate in parent.ChildNodes)
            {
                if (candidate is XmlElement && candidate.Name == element.Name)
                {
                    if (candidate == element)
                    {
                        return index;
                    }
                    index++;
                }
            }
            throw new ArgumentException("Couldn't find element within parent");
        }

        public static PDFColor ExtractColor(string value)
        {
            switch (value.ToUpper())
            {
                case "WHITE":
                    return new PDFColor(255, 255, 255, 255);
                case "RED":
                    return new PDFColor(255, 255, 0, 0);
                case "GREEN":
                    return new PDFColor(255, 0, 255, 0);
                case "BLUE":
                    return new PDFColor(255,0,0,255);
                case "BLACK":
                    return new PDFColor(255, 0, 0, 0);
                case "PURPLE":
                    return new PDFColor(255, 125, 0, 255);
                case "GREY":
                    return new PDFColor(255, 128, 128, 128);
                case "YELLOW":
                    return new PDFColor(255, 255, 255, 0);
                case "ORANGE":
                    return new PDFColor(255, 255, 125, 0);
                default:
                    int a, r, g, b;
                    a = int.Parse(value.Substring(0, 2), NumberStyles.HexNumber, null);
                    r = int.Parse(value.Substring(2, 2), NumberStyles.HexNumber, null);
                    g = int.Parse(value.Substring(4, 2), NumberStyles.HexNumber, null);
                    b = int.Parse(value.Substring(6, 2), NumberStyles.HexNumber, null);
                    return new PDFColor(a,r, g, b);
            }
        }

        public static Stream LocateEmbededResource(string name)
        {
            Stream ret = typeof(Utility).Assembly.GetManifestResourceStream(name);
            if (ret == null)
            {
                foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (!ass.GetName().Name.Contains("mscorlib") && !ass.GetName().Name.StartsWith("System") && !ass.GetName().Name.StartsWith("Microsoft"))
                        {
                            ret = ass.GetManifestResourceStream(name);
                            if (ret != null)
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        if (e.Message != "The invoked member is not supported in a dynamic assembly.")
                        {
                            throw e;
                        }
                    }
                }
            }
            return ret;
        }

        public static double ConvertToPoint(double size, GraphicsUnit units)
        {
            double ret = size;
            switch (units)
            {
                case GraphicsUnit.Millimeter:
                    ret = (size / 0.352777778);
                    break;
                case GraphicsUnit.Centimeter:
                    ret = (size / 0.035277777);
                    break;
                case GraphicsUnit.Inch:
                    ret = (size / 0.01388888889);
                    break;
            }
            return Math.Ceiling(ret);
        }

        public static double ExtractPointSize(string point, GraphicsUnit units)
        {
            if (point.EndsWith("pt"))
            {
                double ret = double.Parse(point.Replace("pt",""));
                switch (units)
                {
                    case GraphicsUnit.Millimeter:
                        ret = ret * 0.352777778;
                        break;
                    case GraphicsUnit.Centimeter:
                        ret = ret * 0.035277777;
                        break;
                    case GraphicsUnit.Inch:
                        ret = ret * 0.01388888889;
                        break;
                }
                return ret;
            }
            else
                return double.Parse(point);
        }

        internal static PDFColor ExtractGrayFill(double p)
        {
            p = (p > 1 ? p / 100 : p);
            return new PDFColor(255, (int)Math.Floor(p * (double)255), (int)Math.Floor(p * (double)255), (int)Math.Floor(p * (double)255));
        }

        internal static List<object> AlignTableCell(List<object> chunks, double width, Alignments alignment, PDFGraphics gfx, ReportPage page)
        {
            List<sParagraphChunk> pcs = new List<sParagraphChunk>();
            List<object> ret = new List<object>();
            double curY = 0;
            foreach (object obj in chunks)
            {
                if (obj is sParagraphChunk)
                    pcs.Add((sParagraphChunk)obj);
                else
                {
                    double maxHeight = 0;
                    double Y = 0;
                    double height = 0;
                    foreach (sPositionedParagraphChunk ppc in AlignText(pcs, 0, width, alignment, gfx, page))
                    {
                        if (Y == ppc.YOffset)
                            maxHeight = Math.Max(maxHeight, ppc.Height);
                        else
                        {
                            height += maxHeight;
                            Y = ppc.YOffset;
                        }
                        ret.Add(new sPositionedParagraphChunk(ppc.XOffset, ppc.YOffset + curY, ppc.Font, ppc.URL, ppc.Text, ppc.Height));
                    }
                    curY = maxHeight + height;
                    pcs = new List<sParagraphChunk>();
                    TableCellImage img = (TableCellImage)obj;
                    switch (alignment)
                    {
                        case Alignments.Left:
                            ret.Add(new sPositionedImage(0, curY, img.GetHeight(gfx), Math.Min(img.GetWidth(gfx), width), img.DrawingImage));
                            break;
                        case Alignments.Right:
                            ret.Add(new sPositionedImage(Math.Min(img.GetWidth(gfx), width) - Math.Min(img.GetWidth(gfx), width), curY, img.GetHeight(gfx), Math.Min(img.GetWidth(gfx), width), img.DrawingImage));
                            break;
                        case Alignments.Center:
                            ret.Add(new sPositionedImage((Math.Min(img.GetWidth(gfx), width) - Math.Min(img.GetWidth(gfx), width))/2, curY, img.GetHeight(gfx), Math.Min(img.GetWidth(gfx), width), img.DrawingImage));
                            break;
                    }
                    curY += img.GetHeight(gfx);
                }
            }
            if (pcs.Count > 0)
            {
                foreach (sPositionedParagraphChunk ppc in AlignText(pcs, 0, width, alignment, gfx, page))
                    ret.Add(new sPositionedParagraphChunk(ppc.XOffset, ppc.YOffset + curY, ppc.Font, ppc.URL, ppc.Text, ppc.Height));
            }
            return ret;
        }

        internal static List<sPositionedParagraphChunk> AlignText(List<sParagraphChunk> chunks, double startOffset,double maxWidth,Alignments alignment,PDFGraphics gfx,ReportPage page)
        {
            List<sPositionedParagraphChunk> ret = new List<sPositionedParagraphChunk>();
            List<sParagraphChunk> curChunks = new List<sParagraphChunk>();
            List<List<sParagraphChunk>> lines = new List<List<sParagraphChunk>>();
            List<sParagraphChunk> newChunks = new List<sParagraphChunk>();
            double curWid = 0;
            double curOffset = startOffset;
            foreach (sParagraphChunk chunk in chunks)
            {
                if (chunk.Text.TrimStart().Split(new char[] { '\r', '\n' }).Length > 1)
                {
                    foreach (string str in chunk.Text.TrimStart().Split(new char[] { '\r', '\n' }))
                    {
                        if (str.Trim().Length > 0)
                            newChunks.Add(new sParagraphChunk(str.Trim().Replace("\\s", " ").Replace("\\t", "\t").Replace("\\n", "\n") + (!chunk.Text.EndsWith(str) ? "\n" : ""), chunk.Font, chunk.URL));
                    }
                }
                else
                    newChunks.Add(chunk);
            }
            foreach (sParagraphChunk chunk in newChunks)
            {
                PDFFont fnt = page.GetFont(chunk.Font);
                sSize mes = gfx.MeasureString(chunk.Text, fnt);
                if (curOffset + curWid + mes.Width > maxWidth)
                {
                    string tmp = "";
                    int tindex = -1;
                    while (true)
                    {
                        tindex++;
                        if (tindex >= chunk.Text.Length)
                            break;
                        else
                        {
                            tmp += chunk.Text[tindex];
                            mes = gfx.MeasureString(tmp, fnt);
                            if (curOffset + curWid + mes.Width >= maxWidth)
                            {
                                curOffset = 0;
                                curWid = 0;
                                if (tmp.Contains(" ")&&!tmp.EndsWith(" ")){
                                    curChunks.Add(new sParagraphChunk(
                                        (curOffset + curWid + mes.Width == maxWidth ? tmp : tmp.Substring(0, tmp.TrimEnd().LastIndexOf(" "))), chunk.Font, chunk.URL));
                                    tmp = (curOffset + curWid + mes.Width == maxWidth ? "" : tmp.Substring(tmp.TrimEnd().LastIndexOf(" "))).TrimStart();
                                }else{
                                    curChunks.Add(new sParagraphChunk(
                                        (curOffset + curWid + mes.Width == maxWidth ? tmp : tmp.Substring(0, tmp.Length-1)), chunk.Font, chunk.URL));
                                    tmp = (curOffset + curWid + mes.Width == maxWidth ? "" : tmp.Substring(tmp.Length-1));
                                }
                                lines.Add(curChunks);
                                curChunks = new List<sParagraphChunk>();
                            }
                        }
                    }
                    if (tmp != "")
                    {
                        curOffset = 0;
                        curChunks.Add(new sParagraphChunk(tmp, chunk.Font, chunk.URL));
                        if (tmp.EndsWith("\n"))
                        {
                            lines.Add(curChunks);
                            curChunks = new List<sParagraphChunk>();
                            curWid = 0;
                        }
                        else
                            curWid = gfx.MeasureString(tmp, fnt).Width;
                    }
                }
                else if (!chunk.Text.EndsWith("\n"))
                {
                    curChunks.Add(chunk);
                    curWid += mes.Width;
                }
                else
                {
                    curWid = 0;
                    curOffset = 0;
                    curChunks.Add(chunk);
                    lines.Add(curChunks);
                    curChunks = new List<sParagraphChunk>();
                }
            }
            if (curChunks.Count > 0)
                lines.Add(curChunks);
            curOffset = startOffset;
            double curHieght=0;
            double maxHeight=0;
            switch (alignment)
            {
                case Alignments.Left:
                    foreach (List<sParagraphChunk> cks in lines)
                    {
                        foreach (sParagraphChunk ck in cks)
                        {
                            PDFFont fnt = page.GetFont(ck.Font);
                            sSize mes = gfx.MeasureString(ck.Text, fnt);
                            maxHeight = Math.Max(mes.Height, maxHeight);
                            ret.Add(new sPositionedParagraphChunk(curOffset, curHieght, ck.Font, ck.URL, ck.Text,mes.Height));
                            curOffset += mes.Width;
                        }
                        curHieght += maxHeight;
                        maxHeight = 0;
                        curOffset = 0;
                    }
                    break;
                case Alignments.Right:
                    foreach (List<sParagraphChunk> cks in lines)
                    {
                        cks.Reverse();
                        curOffset = maxWidth;
                        foreach (sParagraphChunk ck in cks)
                        {
                            PDFFont fnt = page.GetFont(ck.Font);
                            sSize mes = gfx.MeasureString(ck.Text, fnt);
                            curOffset -= mes.Width;
                            maxHeight = Math.Max(mes.Height, maxHeight);
                            ret.Add(new sPositionedParagraphChunk(curOffset, curHieght, ck.Font, ck.URL, ck.Text,mes.Height));
                        }
                        curHieght += maxHeight;
                        maxHeight = 0;
                        curOffset = 0;
                    }
                    break;
                case Alignments.Center:
                    foreach (List<sParagraphChunk> cks in lines)
                    {
                        curWid = 0;
                        foreach (sParagraphChunk ck in cks)
                            curWid += gfx.MeasureString(ck.Text, page.GetFont(ck.Font)).Width;
                        curOffset = (maxWidth - curWid) / 2;
                        foreach (sParagraphChunk ck in cks)
                        {
                            PDFFont fnt = page.GetFont(ck.Font);
                            sSize mes = gfx.MeasureString(ck.Text, fnt);
                            maxHeight = Math.Max(mes.Height, maxHeight);
                            ret.Add(new sPositionedParagraphChunk(curOffset, curHieght, ck.Font, ck.URL, ck.Text,mes.Height));
                            curOffset += mes.Width;
                        }
                        curHieght += maxHeight;
                        maxHeight = 0;
                        curOffset = 0;
                    }
                    break;
            }
            return ret;
        }

        private static Regex _POINT_REG = new Regex("(\\d+(\\.\\d+)?)pt", RegexOptions.Compiled | RegexOptions.ECMAScript);

        internal static string ReplacePoints(string p, GraphicsUnit graphicsUnit)
        {
            if (_POINT_REG.IsMatch(p))
            {
                MatchCollection col = _POINT_REG.Matches(p);
                foreach (Match m in col)
                    p = p.Replace(m.Value, ExtractPointSize(m.Value, graphicsUnit).ToString());
            }
            return p;
        }
    }
}
