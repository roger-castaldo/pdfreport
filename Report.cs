using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.Exceptions;
using System.Xml;
using Org.Reddragonit.PDFReports.ReportElements;
using System.IO;
using Org.Reddragonit.Stringtemplate;
using Org.Reddragonit.PDFReports.PDF.PageComponents;
using Org.Reddragonit.PDFReports.PDF;

namespace Org.Reddragonit.PDFReports
{
    public class Report
    {
        internal static readonly PDFColor DEFAULT_COLOR = Utility.ExtractColor("BLACK");
       
        private GraphicsUnit _pageUnit = GraphicsUnit.Point;
        internal GraphicsUnit PageUnit
        {
            get { return _pageUnit; }
        }

        private Dictionary<string, ReportFont> _fonts;
        internal PDFFont this[string name,ReportPage page]
        {
            get
            {
                name = (name == null ? _defaultFontName : name);
                if (!_fonts.ContainsKey(name))
                    throw new MissingFontException(name);
                return _fonts[name].Font;
            }
        }

        internal PDFBrush this[string name]
        {
            get
            {
                name = (name == null ? _defaultFontName : name);
                if (!_fonts.ContainsKey(name))
                    throw new MissingFontException(name);
                return _fonts[name].Brush;
            }
        }

        private string _defaultFontName;
        internal PDFFont DefaultFont
        {
            get { return this[_defaultFontName,null]; }
        }

        private string _title;
        private string _author;

        internal void DefineFont(XmlNode node)
        {
            ReportFont rf = new ReportFont(this,node);
            _fonts.Add(rf.Name, rf);
        }

        private List<ReportPage> _pages;

        private Report(XmlDocument doc)
        {
            XmlNode repNode = doc.ChildNodes[1];
            _defaultFontName = repNode.Attributes["DefaultFont"].Value;
            _pageUnit = (repNode.Attributes["PageUnits"] != null ? (GraphicsUnit)Enum.Parse(typeof(GraphicsUnit), repNode.Attributes["PageUnits"].Value) : GraphicsUnit.Point);
            _title = (repNode.Attributes["Title"] != null ? repNode.Attributes["Title"].Value : null);
            _author = (repNode.Attributes["Author"] != null ? repNode.Attributes["Author"].Value : null);
            _pages = new List<ReportPage>();
            _fonts = new Dictionary<string,ReportFont>();
            foreach (XmlNode node in doc.GetElementsByTagName("Font"))
                DefineFont(node);
            foreach (XmlNode node in doc.GetElementsByTagName("Page"))
                _pages.Add(new ReportPage(this,node));
        }

        private byte[] GetBytes()
        {
            PDFGraphics gfx = new PDFGraphics(_title, _author);
            foreach (ReportFont rf in _fonts.Values)
                rf.AppendToDocument(ref gfx, this);
            foreach (ReportPage rp in _pages)
                rp.AppendToDocument(ref gfx, this);
            MemoryStream ret = new MemoryStream();
            gfx.Save(ret);
            return ret.ToArray();
        }

        public static byte[] GenerateEmbeddedReport(string templatePath, Dictionary<string, object> parameters)
        {
            Template t = new Template(new StreamReader(Utility.LocateEmbededResource(templatePath)));
            foreach (string str in parameters.Keys)
                t.SetAttribute(str, parameters[str]);
            XmlDocument doc = new XmlDocument();
            System.Diagnostics.Debug.WriteLine(t.ToString());
            doc.LoadXml(t.ToString());
            Report rp = new Report(doc);
            return rp.GetBytes();
        }

        public static byte[] GenerateFileReport(FileInfo file, Dictionary<string, object> parameters)
        {
            StreamReader sr = new StreamReader(new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read));
            Template t = new Template(sr.ReadToEnd());
            sr.Close();
            foreach (string str in parameters.Keys)
                t.SetAttribute(str, parameters[str]);
            XmlDocument doc = new XmlDocument();
            System.Diagnostics.Debug.WriteLine(t.ToString());
            doc.LoadXml(t.ToString());
            Report rp = new Report(doc);
            return rp.GetBytes();
        }

        public static byte[] GeneratePDFFromXML(string xmlContent)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            Report rp = new Report(doc);
            return rp.GetBytes();
        }

        public static byte[] GeneratePDFFromXML(FileInfo file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file.FullName);
            Report rp = new Report(doc);
            return rp.GetBytes();
        }
    }
}
