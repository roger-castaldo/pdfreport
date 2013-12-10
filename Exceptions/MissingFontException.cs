using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.PDFReports.Exceptions
{
    public class MissingFontException : Exception
    {

        public MissingFontException(string name) : base("A call to obtain the font " + name + " failed because it is not defined in the report.") { }

    }
}
