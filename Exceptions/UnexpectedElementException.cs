using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.PDFReports.Exceptions
{
    public class UnexpectedElementException : UnableToLoadException
    {
        public UnexpectedElementException(string expected, string found,XmlNode node) :
            base(string.Format("expected a {0} element but recieved {1} element.(FULL PATH: {2})", expected, found,Utility.FindXPath(node))) { }
    }
}
