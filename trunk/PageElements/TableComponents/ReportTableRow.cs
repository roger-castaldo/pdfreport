using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.Interfaces;
using System.Xml;
using Org.Reddragonit.PDFReports.ReportElements;
using Org.Reddragonit.PDFReports.Exceptions;
using Org.Reddragonit.PDFReports.PDF;

namespace Org.Reddragonit.PDFReports.PageElements.TableComponents
{
    internal class ReportTableRow : TableElement
    {
        private List<ReportTableCell> _cells;
        public List<ReportTableCell> Cells
        {
            get { return _cells; }
        }

        private int _index;
        public int Index
        {
            get { return _index; }
        }

        public int Cols
        {
            get {
                int ret = 0;
                foreach (ReportTableCell rtc in _cells)
                {
                    ret += rtc.Colspan;
                }
                return ret; 
            }
        }

        private double _curX;
        public double CurX
        {
            get { return _curX; }
        }

        private double _curY;
        public double CurY
        {
            get { return _curY; }
        }

        internal string Style
        {
            get {
                if (this["Style"] == null)
                    return (_header == null ? OwningTable.Style : _header.Style);
                return this["Style"];
            }
        }

        private double _height=-1;
        private ReportTableHeader _header;

        public ReportTableRow(ReportTable table, XmlNode node, ReportPage page, ReportTableHeader header,int index)
            : this(table,node,page,index)
        {
            _header = header;
            _Init(table, node, page);
        }

        public ReportTableRow(ReportTable table,XmlNode node,ReportPage page,int index)
            : base(table,node,page)
        {
            if (node.Name != "Row")
                throw new UnexpectedElementException("Row", node.Name, node);
            _Init(table, node, page);
            _index = index;
        }

        private void _Init(ReportTable table, XmlNode node, ReportPage page)
        {
            _cells = new List<ReportTableCell>();
            int curCol = 0;
            foreach (XmlNode n in node.ChildNodes)
            {
                if (n.Name == "Cell")
                {
                    _cells.Add(new ReportTableCell(table, n, page, this, curCol, _header));
                    curCol += _cells[_cells.Count - 1].Colspan;
                }
                else if (n.Name != "Style")
                    throw new UnexpectedElementException("Style or Cell", n.Name, n);
            }
        }

        internal void ShiftCellsRight(int startingCol,int cols)
        {
            foreach (ReportTableCell rtc in _cells)
            {
                if (rtc.Col >= startingCol)
                    rtc.Col += cols;
            }
        }

        protected override void _LoadData(XmlNode node)
        {
        }

        internal override void AppendToPage(ref PDFGraphics gfx, out double width, out double height)
        {
            width = 0;
            height = (_height == -1 ? GetHeight(OwningTable.Widths, gfx) : _height);
            _curY = (_header==null ? OwningTable.CurY : _header.CurY)+OwningTable.CellSpacing;
            _curX = OwningTable.CurX +OwningTable.CellSpacing;
            int curCol = 0;
            foreach (ReportTableCell rtc in _cells)
            {
                double wid;
                double ht;
                while (curCol < rtc.Col)
                {
                    _curX += OwningTable.Widths[curCol] + OwningTable.CellSpacing;
                    curCol++;
                }
                rtc.AppendToPage(ref gfx, out wid, out ht);
                curCol += rtc.Colspan;
                for (int x = 0; x < rtc.Colspan; x++)
                    _curX += OwningTable.Widths[x+rtc.Col] + OwningTable.CellSpacing;
            }
        }

        internal double GetHeight(double[] _widths, PDFGraphics gfx)
        {
            double ret = 0;
            foreach (ReportTableCell rtc in _cells)
            {
                if (rtc.Rowspan == 1)
                    ret = Math.Max(ret, rtc.GetHeight(_widths, gfx));
            }
            _height = ret;
            return ret+(2 * OwningTable.CellSpacing);
        }
    }
}
