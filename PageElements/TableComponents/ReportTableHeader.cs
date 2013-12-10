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
    internal class ReportTableHeader : TableElement
    {
        private List<ReportTableRow> _rows;
        public List<ReportTableRow> Rows
        {
            get { return _rows; }
        }

        public int Cols
        {
            get
            {
                int ret = 0;
                foreach (ReportTableRow row in _rows)
                    ret = (ret < row.Cols ? ret = row.Cols : ret);
                return ret;
            }
        }

        public double CurX
        {
            get { return OwningTable.CurX; }
        }

        private double _curY;
        public double CurY
        {
            get { return _curY; }
        }

        internal string Style
        {
            get
            {
                if (this["Style"] == null)
                    return OwningTable.Style;
                return this["Style"];
            }
        }

        internal ReportTableHeader(ReportTable table, XmlNode node,ReportPage page)
            :base(table,node,page)
        {
            if (node.Name != "Header")
                throw new UnexpectedElementException("Header", node.Name, node);
            _rows = new List<ReportTableRow>();
            foreach (XmlNode n in node.ChildNodes)
            {
                if (n.Name == "Row")
                    _rows.Add(new ReportTableRow(table, n, page,this,_rows.Count));
                else if (n.Name != "Style")
                    throw new UnexpectedElementException("Style or Row", n.Name, n);
            }
            for (int x = 0; x < _rows.Count; x++)
            {
                foreach (ReportTableCell rtc in _rows[x].Cells)
                {
                    if (rtc.Rowspan > 1)
                    {
                        for (int z = 1; z < rtc.Rowspan; z++)
                            _rows[x + z].ShiftCellsRight(rtc.Col, rtc.Colspan);
                    }
                }
            }
        }

        protected override void _LoadData(XmlNode node)
        {
        }

        internal override void AppendToPage(ref PDFGraphics gfx, out double width, out double height)
        {
            int curCount = 0;
            _curY = OwningTable.CurY;
            height = 0;
            width = 0;
            double ht = 0;
            foreach (ReportTableRow rtr in _rows)
            {
                rtr.AppendToPage(ref gfx, out width, out ht);
                height += ht;
                _curY += ht;
                curCount++;
                if (curCount == 3)
                {
                    _curY += Utility.ExtractPointSize("1pt", OwningPage.PageUnit);
                    curCount = 0;
                }
            }
        }

        internal double GetHeight(double[] _widths, PDFGraphics gfx)
        {
            double ret = 0;
            foreach (ReportTableRow row in _rows)
                ret += row.GetHeight(_widths, gfx);
            return ret;
        }
    }
}
