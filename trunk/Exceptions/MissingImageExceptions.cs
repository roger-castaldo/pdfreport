using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.PDFReports.Exceptions
{
    public class MissingEmbeddedImageException : Exception
    {
        public MissingEmbeddedImageException(string path) :
            base(String.Format("Unable to locate the embedded image. (PATH: {0})",path)) { }
    }

    public class MissingImageFileException : Exception
    {
        public MissingImageFileException(string path) :
            base(String.Format("Unable to locate the image file. (PATH: {0})",path)) { }
    }

    public class InvalidImageContent : Exception
    {
        public InvalidImageContent(XmlNode node)
            :base(String.Format("The Image Content at {0} is not a valid Radix64.",Utility.FindXPath(node)))
        {
        }
    }
}
