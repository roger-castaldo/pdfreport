using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.Interfaces;
using System.Xml;
using Org.Reddragonit.PDFReports.ReportElements;
using Org.Reddragonit.PDFReports.Exceptions;
using Org.Reddragonit.PDFReports.PDF;

namespace Org.Reddragonit.PDFReports.PageElements
{
    internal class ReportParagraph : PageElement
    {
        private List<sParagraphChunk> _content;

        internal ReportParagraph(XmlNode node, ReportPage page)
            : base(node, page)
        {
            if (node.Name != "Paragraph")
                throw new UnexpectedElementException("Paragraph", node.Name,node);
        }

        internal override void AppendToPage(ref PDFGraphics gfx, out double width, out double height)
        {
            bool disableWordWrap = (this["DisableWordWrap"] == null ? false : bool.Parse(this["DisableWordWrap"]));
            width = (this["Width"] == null ? (
                    ignoreMargins ?
                    gfx.PageSize.Width
                    : gfx.PageSize.Width - OwningPage.LeftMargin - OwningPage.RightMargin
            ) - X : float.Parse(this["Width"]));
            bool boxed = (this["Boxed"] == null ? false : bool.Parse(this["Boxed"]));
            Alignments align = (this["Alignment"] != null ? (Alignments)Enum.Parse(typeof(Alignments), this["Alignment"]) : Alignments.Left);
            height = 0;

            double curX = X;
            double curY = Y;
            double yOffset = 0;
            List<sPositionedParagraphChunk> pchunks = Utility.AlignText(_content, (boxed ? 0 : curX - (ignoreMargins ? 0 : OwningPage.LeftMargin)), width, align, gfx,OwningPage);
            for(int x=0;x<pchunks.Count;x++)
            {
                double lineHeight = 0;
                int y = 0;
                while (true)
                {
                    if (x + y >= pchunks.Count)
                        break;
                    else
                    {
                        if (pchunks[x].YOffset != pchunks[x + y].YOffset)
                            break;
                        else
                            lineHeight = Math.Max(lineHeight, gfx.MeasureString(pchunks[x + y].Text, OwningPage.GetFont(pchunks[x + y].Font)).Height);
                    }
                    y++;
                }
                if (curY + lineHeight > (ignoreMargins ? gfx.PageSize.Height : gfx.PageSize.Height - OwningPage.BottomMargin))
                {
                    OwningPage.InitPage(ref gfx);
                    curY = Y;
                    yOffset = 0;
                    if (!boxed)
                        curX = (ignoreMargins ? 0 : OwningPage.LeftMargin);
                }
                gfx.DrawString(pchunks[x].Text, OwningPage.GetFont(pchunks[x].Font), OwningPage.GetFontBrush(pchunks[x].Font), curX + pchunks[x].XOffset, curY + pchunks[x].YOffset);
                yOffset = Math.Max(yOffset, pchunks[x].YOffset+pchunks[x].Height);
                if (!boxed && x > 0)
                {
                    if (pchunks[x - 1].YOffset != pchunks[x].YOffset)
                        curX = (ignoreMargins ? 0 : OwningPage.LeftMargin);
                }
            }
            height = yOffset;
        }

        protected override void _LoadData(XmlNode node)
        {
            _content = new List<sParagraphChunk>();
            if (node.ChildNodes.Count > 0)
            {
                if (node.ChildNodes[0].NodeType == XmlNodeType.Text)
                        _content.Add(new sParagraphChunk(node.InnerText, this["Font"], null));
                else
                {
                    foreach (XmlNode n in node.ChildNodes)
                    {
                        if (n.NodeType!=XmlNodeType.Comment)
                            _content.Add(new sParagraphChunk(n, this["Font"]));
                    }
                }
            }
        }
    }
}
