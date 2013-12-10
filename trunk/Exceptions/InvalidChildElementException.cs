using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.PDFReports.Exceptions
{
    public class InvalidChildElementException : UnableToLoadException
    {
        public InvalidChildElementException(string childName, string parentName,XmlNode node)
            : base(string.Format("{0} is not allowed as a valid child element of {1}. (FULL PATH: {2})", childName, parentName,Utility.FindXPath(node))) { }
    }
}
