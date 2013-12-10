using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.PDFReports.Interfaces
{
    internal abstract class XmlLoadableElement 
    {
        private Dictionary<string, string> _properties;
        protected string this[string name]
        {
            get
            {
                if (_properties.ContainsKey(name))
                    return _properties[name];
                return null;
            }
        }

        protected abstract void _LoadAdditionalData(XmlNode node);

        public XmlLoadableElement(XmlNode node)
        {
            _properties = new Dictionary<string, string>();
            foreach (XmlAttribute att in node.Attributes)
                _properties.Add(att.Name, att.Value);
            _LoadAdditionalData(node);
        }
    }
}
