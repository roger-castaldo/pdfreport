using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.Interfaces;
using System.Xml;
using Org.Reddragonit.PDFReports.Exceptions;
using Org.Reddragonit.PDFReports.PDF;
using Org.Reddragonit.PDFReports.PDF.PageComponents;

namespace Org.Reddragonit.PDFReports.ReportElements
{
    internal class ReportFont : ReportElement
    {
        public string Name
        {
            get { return this["Name"]; }
        }

        private PDFFont _font;
        public PDFFont Font
        {
            get{return _font;}
        }

        public Fonts FontFamily
        {
            get { return (Fonts)Enum.Parse(typeof(Fonts), this["FontFamily"]); }
        }

        public FontStyle Style
        {
            get{return (this["Style"]==null ? FontStyle.Regular : (FontStyle)(FontStyle)Enum.Parse(typeof(FontStyle),this["Style"]));}
        }

        public double EM
        {
            get { return double.Parse(this["Size"]); }
        }

        public PDFBrush Brush
        {
            get { return (this["Color"] != null ? new PDFBrush(Utility.ExtractColor(this["Color"])) : PDFBrush.Black); }
        }

        public ReportFont(Report owner,XmlNode node)
            : base(owner,node)
        {
            if (node.Name != "Font")
                throw new UnexpectedElementException("Font", node.Name,node);
            if (this.Name == null)
                throw new MissingAttributeException("Name", "Font",node);
            if(this["Size"]==null)
                throw new MissingAttributeException("Size","Font",node);
            if (this["FontFamily"]==null)
                throw new MissingAttributeException("FontFamily", "Font", node);
        }

        protected override void _LoadAdditionalData(XmlNode node)
        {
        }

        internal override void AppendToDocument(ref PDFGraphics gfx, Report rep)
        {
            _font = gfx.Doc.DefineFont(FontFamily, Style, EM);
        }
    }
}
