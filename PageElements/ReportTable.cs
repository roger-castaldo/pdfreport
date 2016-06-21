using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.Interfaces;
using System.Xml;
using Org.Reddragonit.PDFReports.ReportElements;
using Org.Reddragonit.PDFReports.Exceptions;
using Org.Reddragonit.PDFReports.PageElements.TableComponents;
using Org.Reddragonit.PDFReports.PDF;

namespace Org.Reddragonit.PDFReports.PageElements
{
    internal class ReportTable : PageElement
    {
        private double[] _widths;
        public double[] Widths
        {
            get { return _widths; }
        }
        private List<ReportTableRow> _rows;
        public List<ReportTableRow> Rows
        {
            get { return _rows; }
        }
        private Dictionary<string, ReportTableStyle> _styles;
        public ReportTableStyle GetStyle(string name)
        {
            if (name == null)
                return null;
            if (_styles.ContainsKey(name))
                return _styles[name];
            return null;
        }
        private ReportTableHeader _header;

        internal string Style
        {
            get { return this["Style"]; }
        }

        public double CellPadding
        {
            get { return (this["CellPadding"] != null ? Utility.ExtractPointSize(this["CellPadding"]+"pt",OwningPage.PageUnit) : 0); }
        }

        public double CellSpacing
        {
            get { return (this["CellSpacing"] != null ? Utility.ExtractPointSize(this["CellSpacing"] + "pt", OwningPage.PageUnit) : 0); }
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

        //flagged when the table has been processed to ensure that no cell extends past the rows of the table
        private bool _tableFixed = false;

        public ReportTable(XmlNode node, ReportPage page)
            : base(node, page)
        {
            if (node.Name != "Table")
                throw new UnexpectedElementException("Table", node.Name, node);
        }

        internal override void AppendToPage(ref PDFGraphics gfx, out double width, out double height)
        {
            _curX = X;
            _curY = Y + Utility.ExtractPointSize("1pt", OwningPage.PageUnit);
            height = 0;
            if (this["Inline"] != null)
            {
                if (!bool.Parse(this["Inline"]))
                    _curX = OwningPage.CurrentX;
            }else
                _curX = (this.ignoreMargins ? X-OwningPage.LeftMargin : X);
            if (this["Width"]==null)
                width = (this.ignoreMargins ? gfx.PageSize.Width-_curX : gfx.PageSize.Width-OwningPage.RightMargin - _curX);
            else
                width = float.Parse(this["Width"]);
            bool repeatHeader = (this["RepeatHeader"] != null ? bool.Parse(this["RepeatHeader"]) : false);
            bool lockInPage = (this["LockInPage"] != null ? bool.Parse(this["LockInPage"]) : false);
            int cols = (_header==null ? 0 : _header.Cols);
            double wid=0;
            double ht=0;
            foreach (ReportTableRow row in _rows)
                cols = (cols < row.Cols ? cols = row.Cols : cols);
            if (_widths == null)
            {
                wid = width / (double)cols;
                _widths = new double[cols];
                for (int a = 0; a < _widths.Length; a++)
                    _widths[a] = wid;
            }
            if (_header != null)
            {
                if (!lockInPage)
                {
                    if (_curY + _header.GetHeight(_widths, gfx) > (ignoreMargins ? gfx.PageSize.Height : gfx.PageSize.Height - OwningPage.BottomMargin))
                    {
                        OwningPage.InitPage(ref gfx);
                        _curY = (ignoreMargins ? 0 : Y);
                    }
                }
                _header.AppendToPage(ref gfx, out wid, out ht);
                _curY += ht;
                height += ht;
            }
            foreach (ReportTableRow rw in _rows)
            {
                if (_curY + rw.GetHeight(Widths, gfx) > (ignoreMargins ? gfx.PageSize.Height : gfx.PageSize.Height - OwningPage.BottomMargin))
                {
                    if (lockInPage)
                        break;
                    height = 0;
                    OwningPage.InitPage(ref gfx);
                    _curY = (ignoreMargins ? 0 : OwningPage.CurrentY);
                    if (repeatHeader && _header != null)
                    {
                        _header.AppendToPage(ref gfx, out wid, out ht);
                        _curY += ht;
                        height += ht;
                    }
                }
                rw.AppendToPage(ref gfx, out wid, out ht);
                _curY += ht;
                height += ht;
            }
        }

        protected override void _LoadData(XmlNode node)
        {
            _rows = new List<ReportTableRow>();
            _styles = new Dictionary<string, ReportTableStyle>();
            LoadAllStyle(node);
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "Header":
                        _header = new ReportTableHeader(this, n,OwningPage);
                        break;
                    case "Style":
                        break;
                    case "Row":
                        _rows.Add(new ReportTableRow(this, n,OwningPage,_rows.Count));
                        break;
                    default:
                        if (n.NodeType!=XmlNodeType.Comment)
                            throw new UnexpectedElementException("Header or Style or Row", n.Name, n);
                        break;
                }
            }
            for (int x = 0; x < _rows.Count; x++)
            {
                foreach (ReportTableCell rtc in _rows[x].Cells)
                {
                    if (rtc.Rowspan > 1)
                    {
                        for (int z = 1; z < rtc.Rowspan; z++)
                        {
                            if (x + z >= Rows.Count)
                                break;
                            _rows[x + z].ShiftCellsRight(rtc.Col, rtc.Colspan);
                        }
                    }
                }
            }
            if (this["Widths"] != null)
            {
                string[] tmp = this["Widths"].Split(',');
                _widths = new double[tmp.Length];
                for (var x = 0; x < tmp.Length; x++)
                    _widths[x] = double.Parse(tmp[x]);
            }
        }

        private void LoadAllStyle(XmlNode node)
        {
            if (node.Name == "Style")
            {
                ReportTableStyle rts = new ReportTableStyle(this, node,OwningPage);
                _styles.Add(rts.Name, rts);
            }
            foreach (XmlNode n in node.ChildNodes)
            {
                if (n.NodeType == XmlNodeType.Element)
                    LoadAllStyle(n);
            }
        }
    }
}
