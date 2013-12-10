using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.Interfaces;
using System.Xml;
using Org.Reddragonit.PDFReports.ReportElements;
using Org.Reddragonit.PDFReports.Exceptions;
using Org.Reddragonit.PDFReports.PDF;
using Org.Reddragonit.PDFReports.PDF.PageComponents;

namespace Org.Reddragonit.PDFReports.PageElements
{
    internal class ReportRectangle : PageElement
    {

        public ReportRectangle(XmlNode node, ReportPage page)
            : base(node, page)
        {
            if (node.Name != "Rectangle")
                throw new UnexpectedElementException("Rectangle", node.Name, node);
            if (this["Width"] == null)
            {
                if ((this["X2"] == null) && (this["Y2"] == null))
                    throw new MissingAttributeException("X2 and Y2 or Width and Height", "Rectangle", node);
            }
            else if (this["Width"] != null && this["Height"] == null)
                throw new MissingAttributeException("Height", "Rectangle", node);
        }

        internal override void AppendToPage(ref PDFGraphics gfx, out double width, out double height)
        {
            double x2 = X;
            double y2 = Y;
            if (this["Width"] != null)
            {
                x2 += double.Parse(this["Width"]);
                y2 += double.Parse(this["Height"]);
            }
            else
            {
                x2 = (this["X2"] != null ? double.Parse(this["X2"]) : x2);
                y2 = (this["Y2"] != null ? (this.ignoreMargins ? gfx.PageSize.Height : gfx.PageSize.Height - OwningPage.TopMargin) - double.Parse(this["Y2"]) : y2);
            }
            width = x2 - X;
            height = y2-Y;
            if (this["FillColor"] != null)
                gfx.DrawLine(new PDFPen(Utility.ExtractColor(this["FillColor"]), x2 - X), X, Y, x2, y2);
            gfx.DrawRectangle(new PDFPen(Utility.ExtractColor(this["LineColor"] == null ? "BLACK" : this["LineColor"]), (this["LineWidth"] == null ? Utility.ExtractPointSize("1pt", OwningPage.PageUnit) : double.Parse(this["LineWidth"]))), X, Y,width,height);
        }

        protected override void _LoadData(XmlNode node)
        {

        }
    }
}
