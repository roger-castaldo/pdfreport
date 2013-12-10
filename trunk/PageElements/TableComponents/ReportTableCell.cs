using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.Interfaces;
using System.Xml;
using Org.Reddragonit.PDFReports.ReportElements;
using Org.Reddragonit.PDFReports.Exceptions;
using Org.Reddragonit.PDFReports.PDF;
using Org.Reddragonit.PDFReports.PDF.PageComponents;

namespace Org.Reddragonit.PDFReports.PageElements.TableComponents
{
    internal class ReportTableCell : TableElement
    {
        public int Colspan
        {
            get {
                return (this["Colspan"] != null ? int.Parse(this["Colspan"]) : 1);
            }
        }

        public int Rowspan
        {
            get
            {
                return (this["Rowspan"] != null ? int.Parse(this["Rowspan"]) : 1);
            }
        }

        internal new string this[string name]{
            get{
                if (name == null)
                    return null;
                if (name == "Style")
                {
                    if (base[name] == null)
                        return _row.Style;
                }
                return base[name];
            }
        }

        private ReportTableRow _row;
        private int _col;
        public int Col
        {
            get { return _col; }
            set { _col = value; }
        }
        private List<object> _content;
        private ReportTableHeader _header;
        private bool _converted = false;

        public ReportTableCell(ReportTable table, XmlNode node, ReportPage page, ReportTableRow row, int col,ReportTableHeader header)
            :this(table,node,page,row,col)
        {
            _header = header;
        }

        public ReportTableCell(ReportTable table,XmlNode node,ReportPage page,ReportTableRow row,int col)
            : base(table,node,page)
        {
            if (node.Name != "Cell")
                throw new UnexpectedElementException("Cell", node.Name, node);
            _row = row;
            _col = col;
        }

        protected override void  _LoadData(XmlNode node)
        {   
            _content = new List<object>();
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.NodeType)
                {
                    case XmlNodeType.Text:
                        _content.Add(new sParagraphChunk(n,this["FontStyle"]));
                        break;
                    case XmlNodeType.Element:
                        switch (n.Name)
                        {
                            case "Image":
                                _content.Add(new TableCellImage(OwningTable, n,OwningPage));
                                break;
                            case "Chunk":
                                _content.Add(new sParagraphChunk(n, this["FontStyle"]));
                                break;
                            default:
                                throw new UnexpectedElementException("Image or Chunk", n.Name, n);
                                break;
                        }
                        break;
                    case XmlNodeType.Comment:
                        break;
                    default:
                        throw new UnexpectedElementException("Image, Text or Chunk", n.Name, n);
                        break;
                }
            }
        }

        internal override void AppendToPage(ref PDFGraphics gfx, out double width, out double height)
        {
            width=0;
            double curX = _row.CurX;
            double curY = _row.CurY;
            for(int x=0;x<Colspan;x++)
                width += OwningTable.Widths[x + _col];
            ReportTableStyle stl = OwningTable.GetStyle(this["Style"]);
            double pt = Utility.ExtractPointSize("1pt", OwningPage.PageUnit);
            width -= 2*OwningTable.CellSpacing;
            curX += OwningTable.CellSpacing;
            curY += OwningTable.CellSpacing;
            height = Math.Max(_row.GetHeight(OwningTable.Widths, gfx),GetHeight(OwningTable.Widths,gfx));
            if (Rowspan > 1)
            {
                ReportTableRow[] rows = new ReportTableRow[Math.Min((_header == null ? OwningTable.Rows : _header.Rows).Count-_row.Index,Rowspan)];
                (_header == null ? OwningTable.Rows : _header.Rows).CopyTo(_row.Index, rows, 0, rows.Length);
                height = Math.Max(height, _row.GetHeight(OwningTable.Widths, gfx) - (2 * OwningTable.CellSpacing));
                double ht = 0;
                foreach (ReportTableRow row in rows)
                    ht += row.GetHeight(OwningTable.Widths, gfx);
                height = Math.Max(height, ht);
            }
            height -= 2 * OwningTable.CellSpacing;
            double cheight = height;
            if (stl != null)
            {
                if (stl.BackgroundColor != null)
                    gfx.FillRectangle(stl.BackgroundColor, curX+stl.BorderWidthLeft, curY+stl.BorderWidthTop, width-stl.BorderWidthLeft-stl.BorderWidthRight, height - stl.BorderWidthTop - stl.BorderWidthBottom);
                if (stl.BorderWidthTop > 0)
                    gfx.DrawLine(new PDFPen((stl.BorderColorTop != null ? stl.BorderColorTop : Report.DEFAULT_COLOR), stl.BorderWidthTop), curX, curY, curX + width, curY);
                if (stl.BorderWidthLeft > 0)
                    gfx.DrawLine(new PDFPen((stl.BorderColorLeft != null ? stl.BorderColorLeft : Report.DEFAULT_COLOR), stl.BorderWidthLeft), curX, curY, curX, curY + height);
                if (stl.BorderWidthBottom > 0)
                    gfx.DrawLine(new PDFPen((stl.BorderColorBottom != null ? stl.BorderColorBottom : Report.DEFAULT_COLOR), stl.BorderWidthBottom), curX, curY + height, curX + width, curY + height);
                if (stl.BorderWidthRight > 0)
                    gfx.DrawLine(new PDFPen((stl.BorderColorRight != null ? stl.BorderColorRight : Report.DEFAULT_COLOR), stl.BorderWidthRight), curX + width, curY, curX + width, curY + height);
                curX += stl.BorderWidthLeft;
                curY += stl.BorderWidthTop;
                cheight-=stl.BorderWidthTop+stl.BorderWidthBottom+stl.PaddingTop+stl.PaddingBottom;
                width -= stl.BorderWidthLeft + stl.BorderWidthRight;
            }
            curY += (stl == null ? OwningTable.CellPadding : stl.PaddingTop);
            curX += (stl == null ? OwningTable.CellPadding : stl.PaddingLeft);
            if (stl != null)
            {
                switch (stl.VerticalAlignment)
                {
                    case VerticalAlignments.Bottom:
                        curY += height-stl.BorderWidthBottom-stl.PaddingBottom - cheight;
                        break;
                    case VerticalAlignments.Center:
                        curY += (height - stl.BorderWidthBottom - stl.PaddingBottom - cheight) / 2;
                        break;
                }
            }
            curY += pt;
            foreach (object obj in _content){
                if (obj is sPositionedParagraphChunk)
                {
                    sPositionedParagraphChunk ppc = (sPositionedParagraphChunk)obj;
                    gfx.DrawString(ppc.Text, OwningPage.GetFont(ppc.Font), OwningPage.GetFontBrush(ppc.Font), curX + ppc.XOffset, curY + ppc.YOffset);
                }
                else
                {
                    sPositionedImage pi = (sPositionedImage)obj;
                    gfx.DrawImage(pi.Img, curX + pi.XOffset, curY + pi.YOffset);
                }
            }
        }

        private double _height = -1;
        internal double GetHeight(double[] _widths, PDFGraphics gfx)
        {
            if (_height != -1)
                return _height;
            double ret = 0;
            ReportTableStyle stl = OwningTable.GetStyle(this["Style"]);
            if (!_converted)
            {
                _converted = true;
                double maxWidth = _widths[_col];
                for (int x = 1; x < Colspan; x++)
                    maxWidth += _widths[_col + x];
                maxWidth -= (2 * OwningTable.CellSpacing) +
                    (stl == null ? OwningTable.CellPadding : (2 * stl.BorderWidthLeft) + stl.PaddingLeft) +
                    (stl == null ? OwningTable.CellPadding : (2 * stl.BorderWidthRight) + stl.PaddingRight);
                _content = Utility.AlignTableCell(_content, maxWidth, (stl == null ? Alignments.Left : stl.Alignment), gfx, OwningPage);
            }
            double curY = 0;
            double maxHeight = 0;
            for (int x = 0; x < _content.Count; x++)
            {
                object obj = _content[x];
                if (obj is sPositionedParagraphChunk)
                {
                    sPositionedParagraphChunk ppc = (sPositionedParagraphChunk)obj;
                    if (ppc.YOffset == curY)
                        maxHeight = Math.Max(maxHeight, ppc.Height);
                    else
                    {
                        ret += maxHeight;
                        curY = ppc.YOffset;
                        maxHeight = ppc.Height;
                    }
                }
                else
                {
                    ret += maxHeight;
                    maxHeight = 0;
                    curY = 0;
                    sPositionedImage pi = (sPositionedImage)obj;
                    ret += pi.Height;
                }
            }
            ret += (maxHeight == 0 ? 0 : maxHeight) +
                (stl == null ? (2 * OwningTable.CellPadding) : stl.PaddingTop + stl.PaddingBottom + stl.BorderWidthBottom + stl.BorderWidthTop);
            //fix for when a table cells rowspan exceeds the tables rows
            List<int> rowCounts = new List<int>();
            List<double> rowHeights = new List<double>();
            foreach (ReportTableRow row in OwningTable.Rows)
            {
                if (_row.Index == row.Index)
                    break;
                for (int y = 0; y < rowCounts.Count; y++)
                    rowCounts[y] = (rowCounts[y] > 0 ? rowCounts[y] - 1 : 0);
                double rowHeight = row.GetHeight(_widths, gfx);
                foreach (ReportTableCell rtc in row.Cells)
                {
                    for (int y = 0; y < rtc.Colspan; y++)
                    {
                        while (rowCounts.Count < rtc.Col + y + 1)
                            rowCounts.Add(0);
                        while (rowHeights.Count < rtc.Col + y + 1)
                            rowHeights.Add(0);
                        if (rtc.Rowspan > 1)
                        {
                            rowCounts[rtc.Col + y] += rtc.Rowspan - 1;
                            rowHeights[rtc.Col + y] += Math.Max(rowHeight,rtc.GetHeight(_widths, gfx));
                        }
                    }
                }
                for(int y=0;y<rowHeights.Count;y++)
                    rowHeights[y] -= Math.Min(rowHeight, rowHeights[y]);
            }
            maxHeight = 0;
            for (int y = 0; y < rowCounts.Count; y++)
            {
                if (rowCounts[y] == 1)
                    maxHeight = Math.Max(maxHeight,rowHeights[y] - ret);
            }
            ret += maxHeight;
            _height = ret;
            return ret;
        }
    }
}
