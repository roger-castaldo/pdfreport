using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Org.Reddragonit.PDFReports.PDF.PageComponents;

namespace Org.Reddragonit.PDFReports.PDF
{
    internal class PDFTrailer : IComponent
    {
        private int _lastObjectID;
        public int LastObjectID
        {
            set { _lastObjectID = value; }
        }
        
        private int _xrefOffset;
        public int XRefOffset
        {
            get { return _xrefOffset; }
        }

        private List<IComponent> _pieces;

        public PDFTrailer(List<IComponent> pieces,int objectID){
            _pieces = pieces;
            _lastObjectID=objectID;
        }

        #region IComponent Members

        public long Offset
        {
            get { return 0; }
        }

        public void Append(StreamWriter bw)
        {
            bw.Flush();
            long startxref = bw.BaseStream.Position;
            bw.Write("xref"+(char)13);
            bw.Write("0 " + _lastObjectID.ToString() + ""+(char)13);
            bw.Write("0000000000 65535 f"+(char)13);
            foreach (ADocumentPiece adp in _pieces)
            {
                bw.Write(string.Format("{0:0000000000}  00000 n"+(char)13, adp.Offset));
                if (adp is PDFImage)
                    bw.Write(string.Format("{0:0000000000}  00000 n"+(char)13, ((PDFImage)adp).XObjectOffset));
            }
            bw.Write("trailer"+(char)13);
            bw.Write("<<"+(char)13);
            bw.Write("/Size " + _lastObjectID.ToString() + ""+(char)13);
            bw.Write("/Root 1 0 R"+(char)13);
            bw.Write("/Info 2 0 R"+(char)13);
            bw.Write(">>"+(char)13);
            bw.Write("startxref"+(char)13);
            bw.Write(startxref.ToString() +(char)13);
            bw.Write("%%EOF");
        }

        #endregion
    }
}
