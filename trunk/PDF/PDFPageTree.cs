using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Org.Reddragonit.PDFReports.PDF.PageComponents;

namespace Org.Reddragonit.PDFReports.PDF
{
    internal class PDFPageTree : ADocumentPiece
    {
        private List<PDFPage> _pages;
        public List<PDFPage> Pages
        {
            get { return _pages; }
        }

        public PDFPageTree(List<IComponent> components, int objectID)
            : base(objectID)
        {
            _pages = new List<PDFPage>();
            foreach (IComponent ic in components)
            {
                if (ic is PDFPage)
                {
                    _pages.Add((PDFPage)ic);
                    ((PDFPage)ic).PageTreeID = objectID;
                }
            }
        }

        protected override void _AppendContent(StreamWriter bw)
        {
            bw.Write("/Type /Pages"+(char)13);
            bw.Write("/Count " + _pages.Count.ToString() + ""+(char)13);
            bw.Write("/Kids [");
            foreach (PDFPage pg in _pages)
                bw.Write(pg.ObjectID.ToString() + " 0 R ");
            bw.Write("]"+(char)13);
        }
    }
}
