using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.PDF.PageComponents;

namespace Org.Reddragonit.PDFReports.PDF
{
    internal class PDFHeader : ADocumentPiece
    {
        private PDFPageTree _tree;
        public PDFPageTree Tree
        {
            get { return _tree; }
        }

        public PDFHeader(PDFPageTree tree, int objectID)
            : base(objectID)
        {
            _tree = tree;
        }

        protected override void _AppendContent(System.IO.StreamWriter bw)
        {
            bw.Write("/Type /Catalog" + (char)13);
            bw.Write("/Version /" + PDFDocument._VERSION.ToString() + (char)13);
            bw.Write("/Pages 4 0 R" + (char)13);
            bw.Write("/Outlines 3 0 R" + (char)13);
        }
    }
}
