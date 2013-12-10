using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.PDFReports.Exceptions
{
    public class UnableToLoadException : Exception
    {
        public UnableToLoadException(string reason) : base("Unable to load Report, " + reason) { }
    }
}
