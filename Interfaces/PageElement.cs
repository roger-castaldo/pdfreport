using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.ReportElements;
using System.Xml;
using System.Drawing;
using Org.Reddragonit.PDFReports.PDF;
using Org.Reddragonit.PDFReports.TemplateExtensions.EvalComponents;
using System.Text.RegularExpressions;

namespace Org.Reddragonit.PDFReports.Interfaces
{
    internal abstract class PageElement : XmlLoadableElement
    {
        private static readonly Color _DEFAULT_STROKE_COLOR = Color.Black;
        internal static readonly Regex _REG_FROM_CUR = new Regex("^(\\+|-)\\d+(\\.\\d+)?", RegexOptions.Compiled | RegexOptions.ECMAScript);

        public double X
        {
            get { return (this["X"] == null ? OwningPage.CurrentX : Eval.Execute(Utility.ReplacePoints((_REG_FROM_CUR.IsMatch(this["X"]) ? OwningPage.CurrentX.ToString() + "-" + (ignoreMargins ? 0 : OwningPage.LeftMargin).ToString()+ this["X"] : this["X"]), OwningPage.PageUnit)) + (ignoreMargins ? 0 : OwningPage.LeftMargin)); }
        }

        public double Y
        {
            get { return (this["Y"] == null ? OwningPage.CurrentY : Eval.Execute(Utility.ReplacePoints((_REG_FROM_CUR.IsMatch(this["Y"]) ? OwningPage.CurrentY.ToString() + "-" + (ignoreMargins ? 0 : OwningPage.TopMargin).ToString() + this["Y"] : this["Y"]), OwningPage.PageUnit)) + (ignoreMargins ? 0 : OwningPage.TopMargin)); }
        }

        private ReportPage _owningPage;
        internal ReportPage OwningPage
        {
            get { return _owningPage; }
        }

        protected bool ignoreMargins
        {
            get { return (this["IgnoreMargins"] == null ? false : bool.Parse(this["IgnoreMargins"])); }
        }

        public PageElement(XmlNode node, ReportPage page) : base(node) {
            _owningPage = page;
            _LoadData(node);
        }

        protected override sealed void _LoadAdditionalData(XmlNode node) { }

        protected abstract void _LoadData(XmlNode node);
        internal abstract void AppendToPage(ref PDFGraphics gfx, out double width, out double height);
    }
}
