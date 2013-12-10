using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.PDFReports.Exceptions
{
    public class MissingElementException : Exception
    {
        public MissingElementException(string name,string parentName):
            base((parentName == null ? "Unable to load the report, the element "+name+" is missing." :
            String.Format("Unable to load the report, the element {0} is missing from the element {1}",name,parentName)
            )) {}
    }
}
