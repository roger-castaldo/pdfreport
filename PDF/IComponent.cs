using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Org.Reddragonit.PDFReports.PDF
{
    internal interface IComponent
    {
        void Append(StreamWriter bw);
        long Offset { get; }
    }
}
