using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.PDFReports.PDF
{
    internal class PDFOutlines : ADocumentPiece
    {
        public PDFOutlines(int objectID)
            : base(objectID) { }

        protected override void _AppendContent(System.IO.StreamWriter bw)
        {
            bw.Write("/Type /Outlines"+(char)13);
            bw.Write("/Count 0" + (char)13);
        }
    }
}
