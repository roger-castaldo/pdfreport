using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.PDFReports.Exceptions
{
    public class MissingAttributeException : Exception
    {
        public MissingAttributeException(string attributeName, string elementName,XmlNode node) :
            base("The element " + elementName + " in the supplied document is missing the required attribute " + attributeName+". (FULL PATH: "+Utility.FindXPath(node)+")") { }
    }
}
