using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.Interfaces;
using System.Xml;
using Org.Reddragonit.PDFReports.Exceptions;
using Org.Reddragonit.PDFReports.PageElements;
using Org.Reddragonit.PDFReports.PDF.PageComponents;
using Org.Reddragonit.PDFReports.PDF;

namespace Org.Reddragonit.PDFReports.ReportElements
{
    internal class ReportPage : ReportElement
    {
        Report _owner;

        internal GraphicsUnit PageUnit
        {
            get
            {
                if (this["Units"] != null)
                    return (GraphicsUnit)Enum.Parse(typeof(GraphicsUnit), this["Units"]);
                return _owner.PageUnit;
            }
        }

        private double _topMargin;
        public double TopMargin
        {
            get { return _topMargin; }
        }

        private double _bottomMargin;
        public double BottomMargin
        {
            get { return _bottomMargin; }
        }

        private double _leftMargin;
        public double LeftMargin
        {
            get { return _leftMargin; }
        }

        private double _rightMargin;
        public double RightMargin
        {
            get { return _rightMargin; }
        }

        private double _currentX;
        public double CurrentX
        {
            get { return _currentX; }
        }

        private double _currentY;
        public double CurrentY
        {
            get { return _currentY; }
        }

        internal PDFFont GetFont(string name)
        {
            return _owner[name,this]; 
        }

        internal PDFBrush GetFontBrush(string name)
        {
            return _owner[name];
        }

        internal double DEFAULT_LINE_WIDTH
        {
            get{ return Utility.ExtractPointSize("1pt", PageUnit); }
        }

        private List<PageElement> _elements;

        public ReportPage(Report owner,XmlNode node) :
            base(owner,node)
        {
            _owner = owner;
            if (node.Name != "Page")
                throw new UnexpectedElementException("Page",node.Name,node);
            LoadChildren(node);
        }

        protected void LoadChildren(XmlNode node)
        {
            _elements = new List<PageElement>();
            foreach (XmlNode n in node.ChildNodes)
            {
                switch (n.Name)
                {
                    case "Font":
                        //ignore fonts they will all be loaded at the start
                        break;
                    case "Paragraph":
                        _elements.Add(new ReportParagraph(n, this));
                        break;
                    case "Image":
                        _elements.Add(new ReportImage(n, this));
                        break;
                    case "Line":
                        _elements.Add(new ReportLine(n, this));
                        break;
                    case "Rectangle":
                        _elements.Add(new ReportRectangle(n, this));
                        break;
                    case "Table":
                        _elements.Add(new ReportTable(n, this));
                        break;
                    case "List":
                        _elements.Add(new ReportList(n, this, null));
                        break;
                    case "Barcode":
                        _elements.Add(new ReportBarCode(n, this));
                        break;
                    case "Page":
                        throw new InvalidChildElementException("Page", "Page", n);
                        break;
                    default:
                        throw new UnexpectedElementException("valid page", n.Name, n);
                        break;
                }
            }
        }

        protected override void _LoadAdditionalData(XmlNode node)
        {
        }

        internal void InitPage(ref PDFGraphics gfx)
        {
            string pageSize = (this["Size"] != null ? this["Size"] : "A4");
            gfx.NewPage((PageSizes)Enum.Parse(typeof(PageSizes), pageSize), PageUnit, (this["Landscape"] != null ? bool.Parse(this["Landscape"]) : false));
            _currentY = _topMargin;
            _currentX = _leftMargin;
        }

        internal override void  AppendToDocument(ref PDFGraphics gfx, Report rep)
        {
            switch (PageUnit)
            {
                case GraphicsUnit.Inch:
                    _topMargin = (this["TopMargin"] != null ? double.Parse(this["TopMargin"]) : 1);
                    _bottomMargin = (this["BottomMargin"]!=null ? double.Parse(this["BottomMargin"]) : 0.5);
                    _leftMargin = (this["LeftMargin"]!=null ? double.Parse(this["LeftMargin"]) : 1.25);
                    _rightMargin = (this["RightMargin"] != null ? double.Parse(this["RightMargin"]) : 1.25);
                    break;
                case GraphicsUnit.Centimeter:
                    _topMargin = (this["TopMargin"] != null ? double.Parse(this["TopMargin"]) : 2.54);
                    _bottomMargin = (this["BottomMargin"] != null ? double.Parse(this["BottomMargin"]) : 1.27);
                    _leftMargin = (this["LeftMargin"] != null ? double.Parse(this["LeftMargin"]) : 3.175);
                    _rightMargin = (this["RightMargin"] != null ? double.Parse(this["RightMargin"]) : 3.175);
                    break;
                case GraphicsUnit.Millimeter:
                    _topMargin = (this["TopMargin"] != null ? double.Parse(this["TopMargin"]) : 25.4);
                    _bottomMargin = (this["BottomMargin"] != null ? double.Parse(this["BottomMargin"]) : 12.7);
                    _leftMargin = (this["LeftMargin"] != null ? double.Parse(this["LeftMargin"]) : 31.75);
                    _rightMargin = (this["RightMargin"] != null ? double.Parse(this["RightMargin"]) : 31.75);
                    break;
                case GraphicsUnit.Point:
                    _topMargin = (this["TopMargin"] != null ? double.Parse(this["TopMargin"]) : 72);
                    _bottomMargin = (this["BottomMargin"] != null ? double.Parse(this["BottomMargin"]) : 36);
                    _leftMargin = (this["LeftMargin"] != null ? double.Parse(this["LeftMargin"]) : 90);
                    _rightMargin = (this["RightMargin"] != null ? double.Parse(this["RightMargin"]) : 90);
                    break;
            }

            _currentX = _leftMargin;
            _currentY = _topMargin;

            InitPage(ref gfx);

            foreach (PageElement elem in _elements)
            {
                double width;
                double height;
                elem.AppendToPage(ref gfx, out width, out height);
                _currentX = (elem.X!=_currentX ? elem.X : _currentX)+width;
                _currentY = (elem.Y!=_currentY ? elem.Y : _currentY)+height;
            }
        }
    }
}
