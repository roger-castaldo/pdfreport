using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.PDFReports.Exceptions
{
    public class InvalidAttributeExceptionValue : Exception
    {
        public InvalidAttributeExceptionValue(string name, string value, XmlNode node)
            : base("The value ("+value+") for the attribute "+name+" is invalid at path "+Utility.FindXPath(node))
        {
        }
    }
}
