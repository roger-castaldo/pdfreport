using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Org.Reddragonit.PDFReports.PDF
{
    internal class PDFInfo : ADocumentPiece
    {
        private string _title;
        public string Title
        {
            get { return _title; }
        }

        private string _author;
        public string Author
        {
            get { return _author; }
        }

        public PDFInfo(string title, string author, int objectID)
            : base(objectID)
        {
            _title = title;
            _author = author;
        }

        protected override void _AppendContent(StreamWriter bw)
        {
            bw.Write("/CreationDate(D:" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-04'00')"+(char)13);
            bw.Write("/Creator (PDFReports)"+(char)13);
            bw.Write("/Producer (PDFReports)"+(char)13);
            if (_title!=null)
                bw.Write("/Title (" + _title + ")"+(char)13);
            if (_author != null)
                bw.Write("/Author (" + _author + ")"+(char)13);
            
        }
    }
}
