using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Org.Reddragonit.PDFReports.PDF;

namespace Org.Reddragonit.PDFReports.Interfaces
{
    internal abstract class ReportElement : XmlLoadableElement
    {
        private Report _owningReport;
        public Report OwningReport
        {
            get { return _owningReport; }
        }

        internal abstract void AppendToDocument(ref PDFGraphics doc,Report rep);

        public ReportElement(Report owner, XmlNode node)
            : base(node)
        {
            _owningReport = owner;
        }
    }
}
