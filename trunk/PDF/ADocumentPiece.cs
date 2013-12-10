using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Org.Reddragonit.PDFReports.PDF
{
    internal abstract class ADocumentPiece : IComponent
    {
        private int _objectID;
        public int ObjectID{
            get{return _objectID;}
        }

        private long _offset;
        public long Offset
        {
            get { return _offset; }
        }

        protected abstract void _AppendContent(StreamWriter bw);

        public ADocumentPiece(int objectID)
        {
            _objectID = objectID;
        }

        #region IComponent Members

        public virtual void Append(StreamWriter bw)
        {
            bw.Flush();
            _offset = bw.BaseStream.Position;
            bw.Write(_objectID.ToString()+" 0 obj"+(char)13+"<<"+(char)13);
            _AppendContent(bw);
            bw.Write(">>"+(char)13+"endobj"+(char)13);
        }

        #endregion
    }
}
