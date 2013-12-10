using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Org.Reddragonit.PDFReports.Exceptions;
using System.Drawing;
using System.Reflection;

namespace Org.Reddragonit.PDFReports
{
    internal struct sParagraphChunk
    {
        private string _text;
        public string Text
        {
            get { return _text; }
        }
        private string _font;
        public string Font
        {
            get { return _font; }
        }
        private string _url;
        public string URL
        {
            get { return _url; }
        }

        public sParagraphChunk(XmlNode node, string font)
        {
            if (node.NodeType == XmlNodeType.Text)
            {
                _text = node.InnerText.Trim(new char[] { '\r', '\n','\t' }).TrimStart();
                _font = font;
                _url = null;
            }
            else
            {
                if (node.Name != "Chunk")
                    throw new UnexpectedElementException("Chunk", node.Name, node);
                _text = node.InnerText.Trim();
                _font = (node.Attributes["FontStyle"] != null ? node.Attributes["FontStyle"].Value : font);
                _url = (node.Attributes["URL"] != null ? node.Attributes["URL"].Value : null);
            }
        }

        public sParagraphChunk(string content, string font,string url)
        {
            _text = content;
            _font = font;
            _url = url;
        }

        public sParagraphChunk(string content)
        {
            _text = content;
            _font = null;
            _url = null;
        }
    }

    internal struct sPositionedImage
    {
        private double _xOffset;
        public double XOffset
        {
            get { return _xOffset; }
        }

        private double _yOffset;
        public double YOffset
        {
            get { return _yOffset; }
        }

        private double _height;
        public double Height
        {
            get { return _height; }
        }

        private double _width;
        public double Width
        {
            get { return _width; }
        }

        private Image _img;
        public Image Img
        {
            get { return _img; }
        }

        public sPositionedImage(double xOffset, double yOffset, double height, double width, Image img)
        {
            _xOffset = xOffset;
            _yOffset = yOffset;
            _height = height;
            _width = width;
            _img = img;
        }
    }

    internal struct sPositionedParagraphChunk
    {
        private double _xOffset;
        public double XOffset
        {
            get { return _xOffset; }
        }

        private double _yOffset;
        public double YOffset
        {
            get { return _yOffset; }
        }

        private string _font;
        public string Font
        {
            get { return _font; }
        }

        private string _url;
        public string URL
        {
            get { return _url; }
        }

        private string _text;
        public string Text{
            get{return _text;}
        }

        private double _height;
        public double Height
        {
            get { return _height; }
        }

        public sPositionedParagraphChunk(double xOffset, double yOffset, string font, string url, string text,double height)
        {
            _xOffset = xOffset;
            _yOffset = yOffset;
            _font = font;
            _url = url;
            _text = text;
            _height = height;
        }
    }

    internal struct sSize
    {

        private double _height;
        public double Height
        {
            get { return _height; }
        }

        private double _width;
        public double Width
        {
            get { return _width; }
        }

        public sSize(double width, double height)
        {
            _height = height;
            _width = width;
        }
    }
}
