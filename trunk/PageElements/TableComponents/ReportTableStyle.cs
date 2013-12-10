using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.Interfaces;
using System.Xml;
using Org.Reddragonit.PDFReports.Exceptions;
using Org.Reddragonit.PDFReports.ReportElements;
using Org.Reddragonit.PDFReports.PDF;
using Org.Reddragonit.PDFReports.PDF.PageComponents;

namespace Org.Reddragonit.PDFReports.PageElements.TableComponents
{
    internal class ReportTableStyle : TableElement
    {

        public string Name
        {
            get { return this["Name"]; }
        }

        private ReportTableStyle Parent
        {
            get
            {
                if (this["Inherits"] != null)
                    return OwningTable.GetStyle(this["Inherits"]);
                return null;
            }
        }

        public ReportTableStyle(ReportTable table,XmlNode node,ReportPage page)
            : base(table,node,page)
        {
            if (node.Name != "Style")
                throw new UnexpectedElementException("Style", node.Name, node);
            if (this["Name"] == null)
                throw new MissingAttributeException("Name", node.Name, node);
        }

        protected override void _LoadData(XmlNode node)
        {
        }

        internal override void AppendToPage(ref PDFGraphics gfx, out double width, out double height)
        {
            width = 0;
            height = 0;
        }

        public PDFColor BackgroundColor
        {
            get { return (this["BackgroundColor"] != null ? Utility.ExtractColor(this["BackgroundColor"]) : 
                (this["GrayFill"]!=null ? Utility.ExtractGrayFill(double.Parse(this["GrayFill"])) : 
                (Parent != null ? Parent.BackgroundColor : null))); }
        }

        #region Borders
        public PDFColor BorderColor
        {
            get { return (this["BorderColor"] != null ? Utility.ExtractColor(this["BorderColor"]) : (Parent != null ? Parent.BorderColor : null)); }
        }

        public PDFColor BorderColorBottom
        {
            get { return (this["BorderColorBottom"] != null ? Utility.ExtractColor(this["BorderColorBottom"]) : (Parent != null ? Parent.BorderColorBottom : BorderColor)); }
        }

        public PDFColor BorderColorLeft
        {
            get { return (this["BorderColorLeft"] != null ? Utility.ExtractColor(this["BorderColorLeft"]) : (Parent != null ? Parent.BorderColorLeft : BorderColor)); }
        }

        public PDFColor BorderColorRight
        {
            get { return (this["BorderColorRight"] != null ? Utility.ExtractColor(this["BorderColorRight"]) : (Parent != null ? Parent.BorderColorRight : BorderColor)); }
        }

        public PDFColor BorderColorTop
        {
            get { return (this["BorderColorTop"] != null ? Utility.ExtractColor(this["BorderColorTop"]) : (Parent != null ? Parent.BorderColorTop : BorderColor)); }
        }

        public double BorderWidth
        {
            get { return (this["BorderWidth"] != null ? Utility.ExtractPointSize(this["BorderWidth"]+"pt",OwningPage.PageUnit) : (Parent != null ? Parent.BorderWidth : 0)); }
        }

        public double BorderWidthBottom
        {
            get { return (this["BorderWidthBottom"] != null ? Utility.ExtractPointSize(this["BorderWidthBottom"] + "pt", OwningPage.PageUnit) : (Parent != null ? Parent.BorderWidthBottom : BorderWidth)); }
        }

        public double BorderWidthLeft
        {
            get { return (this["BorderWidthLeft"] != null ? Utility.ExtractPointSize(this["BorderWidthLeft"] + "pt", OwningPage.PageUnit) : (Parent != null ? Parent.BorderWidthLeft : BorderWidth)); }
        }

        public double BorderWidthRight
        {
            get { return (this["BorderWidthRight"] != null ? Utility.ExtractPointSize(this["BorderWidthRight"] + "pt", OwningPage.PageUnit) : (Parent != null ? Parent.BorderWidthRight : BorderWidth)); }
        }

        public double BorderWidthTop
        {
            get { return (this["BorderWidthTop"] != null ? Utility.ExtractPointSize(this["BorderWidthTop"] + "pt", OwningPage.PageUnit) : (Parent != null ? Parent.BorderWidthTop : BorderWidth)); }
        }
        #endregion

        #region Alignments
        public Alignments Alignment
        {
            get { return (this["Alignment"] != null ? (Alignments)Enum.Parse(typeof(Alignments), this["Alignment"]) : (Parent != null ? Parent.Alignment : Alignments.Left)); }
        }

        public VerticalAlignments VerticalAlignment
        {
            get { return (this["VerticalAlignment"] != null ? (VerticalAlignments)Enum.Parse(typeof(VerticalAlignments), this["VerticalAlignment"]) : (Parent != null ? Parent.VerticalAlignment : VerticalAlignments.Center)); }
        }
        #endregion

        #region Padding
        public double Padding
        {
            get { return (this["Padding"] != null ? Utility.ExtractPointSize(this["Padding"]+"pt",OwningPage.PageUnit) : (Parent != null ? Parent.Padding : OwningTable.CellPadding)); }
        }

        public double PaddingBottom
        {
            get { return (this["PaddingBottom"] != null ? Utility.ExtractPointSize(this["PaddingBottom"]+"pt",OwningPage.PageUnit) : (Parent != null ? Parent.PaddingBottom : Padding)); }
        }

        public double PaddingLeft
        {
            get { return (this["PaddingLeft"] != null ? Utility.ExtractPointSize(this["PaddingLeft"] + "pt", OwningPage.PageUnit) : (Parent != null ? Parent.PaddingLeft : Padding)); }
        }

        public double PaddingRight
        {
            get { return (this["PaddingRight"] != null ? Utility.ExtractPointSize(this["PaddingRight"] + "pt", OwningPage.PageUnit) : (Parent != null ? Parent.PaddingRight : Padding)); }
        }

        public double PaddingTop
        {
            get { return (this["PaddingTop"] != null ? Utility.ExtractPointSize(this["PaddingTop"] + "pt", OwningPage.PageUnit) : (Parent != null ? Parent.PaddingTop : Padding)); }
        }
        #endregion

    }
}
