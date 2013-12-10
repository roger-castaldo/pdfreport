using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.PageElements;
using System.Xml;
using Org.Reddragonit.PDFReports.ReportElements;

namespace Org.Reddragonit.PDFReports.Interfaces
{
    internal abstract class TableElement : PageElement
    {
        private ReportTable _owningTable;
        protected ReportTable OwningTable
        {
            get { return _owningTable; }
        }

        public TableElement(ReportTable table,XmlNode node,ReportPage page)
            : base(node,page)
        {
            _owningTable = table;
        }
    }
}
