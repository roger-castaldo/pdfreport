using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.Interfaces;
using System.Xml;
using Org.Reddragonit.PDFReports.ReportElements;
using Org.Reddragonit.PDFReports.Exceptions;
using Org.Reddragonit.PDFReports.PDF;
using Org.Reddragonit.PDFReports.PDF.PageComponents;
using Org.Reddragonit.PDFReports.TemplateExtensions.EvalComponents;

namespace Org.Reddragonit.PDFReports.PageElements
{
    internal class ReportLine : PageElement
    {
        public ReportLine(XmlNode node, ReportPage page)
            : base(node, page)
        {
            if (node.Name!="Line")
                throw new UnexpectedElementException("Line", node.Name, node);
            if (this["Length"] == null)
            {
                if ((this["X2"] == null) && (this["Y2"] == null))
                    throw new MissingAttributeException("X2 or Y2 or Length", "Line", node);
            }
        }

        internal override void AppendToPage(ref PDFGraphics gfx, out double width, out double height)
        {
            double x2 = X;
            double y2 = Y;
            if (this["Length"] != null)
            {
                double len = double.Parse(this["Length"]);
                double angle = (this["Angle"] == null ? 0 : double.Parse(this["Angle"]));
                x2 += len * (float)Math.Cos((angle * Math.PI) / 180);
                y2 += len * (float)Math.Sin((angle * Math.PI) / 180);
            }
            else
            {
                x2 = (this["X2"] != null ? Eval.Execute(Utility.ReplacePoints((_REG_FROM_CUR.IsMatch(this["X2"]) ? OwningPage.CurrentX.ToString() + this["X2"] : this["X2"]), OwningPage.PageUnit)) : x2);
                y2 = (this["Y2"] != null ? Eval.Execute(Utility.ReplacePoints((_REG_FROM_CUR.IsMatch(this["Y2"]) ? OwningPage.CurrentX.ToString() + this["Y2"] : this["Y2"]), OwningPage.PageUnit)) : y2);
            }
            width = x2 - X;
            height = y2 - Y;
            gfx.DrawLine(new PDFPen((this["Color"] != null ? Utility.ExtractColor(this["Color"]) : Utility.ExtractColor("BLACK")), (this["Width"] == null ? OwningPage.DEFAULT_LINE_WIDTH : Utility.ExtractPointSize(this["Width"],OwningPage.PageUnit))),
                X,Y,x2,y2);
        }

        protected override void _LoadData(XmlNode node)
        {
            
        }
    }
}
