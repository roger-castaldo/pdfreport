using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.Interfaces;
using System.Xml;
using Org.Reddragonit.PDFReports.ReportElements;
using Org.Reddragonit.PDFReports.Exceptions;
using System.IO;
using Org.Reddragonit.PDFReports.PDF;

namespace Org.Reddragonit.PDFReports.PageElements
{
    internal class ReportImage : PageElement
    {
        private byte[] _imageData;

        public ReportImage(XmlNode node, ReportPage page)
            : base(node, page)
        {
            if (node.Name != "Image")
                throw new UnexpectedElementException("Image", node.Name, node);
            if (node["EmbeddedPath"] != null)
            {
                if (Utility.LocateEmbededResource(node["EmbeddedPath"].InnerText) == null)
                    throw new MissingEmbeddedImageException(node["EmbeddedPath"].InnerText);
                Stream es = Utility.LocateEmbededResource(node["EmbeddedPath"].InnerText);
                int eOffset = 0;
                _imageData = new byte[es.Length];
                while (es.Position < es.Length)
                {
                    eOffset += es.Read(_imageData, eOffset, (eOffset+500<es.Length ? 500 : (int)(es.Length-es.Position)));
                }
                es.Close();
            }
            else if (node["FilePath"] != null)
            {
                if (node["FilePath"].Attributes["DirectorySeperator"] == null)
                    throw new MissingAttributeException("DirectorySeperator", "FilePath", node["FilePath"]);
                string path = node["FilePath"].InnerText.Trim(new char[]{'\r','\n'});
                path = path.Replace(node["FilePath"].Attributes["DirectorySeperator"].Value, Path.DirectorySeparatorChar.ToString());
                if (path.StartsWith("." + Path.DirectorySeparatorChar.ToString()))
                    path = Directory.GetCurrentDirectory()+path.Substring(1);
                if (!(new FileInfo(path).Exists))
                    throw new MissingImageFileException(path);
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                int fOffset = 0;
                _imageData = new byte[fs.Length];
                while (fs.Position < fs.Length)
                {
                    fOffset += fs.Read(_imageData, fOffset, (fOffset + 500 < fs.Length ? 500 : (int)(fs.Length - fs.Position)));
                }
                fs.Close();
            }
            else if (node["FileContent"] != null)
            {
                if (!Utility.IsValidRadix64(node["FileContent"].InnerText))
                    throw new InvalidImageContent(node);
                _imageData = Convert.FromBase64String(node["FileContent"].InnerText);
            }
            else
                throw new MissingElementException("EmbeddedPath OR FilePath OR FileContent", "Image");
        }

        internal override void AppendToPage(ref PDFGraphics gfx, out double width, out double height)
        {
            System.Drawing.Image img = System.Drawing.Image.FromStream(new MemoryStream(_imageData));
            System.Drawing.GraphicsUnit unit = System.Drawing.GraphicsUnit.Display;
            switch (OwningPage.PageUnit)
            {
                case GraphicsUnit.Centimeter:
                    unit = System.Drawing.GraphicsUnit.Millimeter;
                    break;
                case GraphicsUnit.Inch:
                    unit = System.Drawing.GraphicsUnit.Inch;
                    break;
                case GraphicsUnit.Millimeter:
                    unit = System.Drawing.GraphicsUnit.Millimeter;
                    break;
                case GraphicsUnit.Point:
                    unit = System.Drawing.GraphicsUnit.Point;
                    break;
            }
            System.Drawing.RectangleF rect = img.GetBounds(ref unit);
            width = (OwningPage.PageUnit == GraphicsUnit.Centimeter ? 10*rect.Width : rect.Width);
            height = (OwningPage.PageUnit == GraphicsUnit.Centimeter ? 10 * rect.Height : rect.Height);
            if (this["Width"] != null)
                width = float.Parse(this["Width"]);
            if (this["Height"] != null)
                height = float.Parse(this["Height"]);
            if (this["Rotation"] != null)
                img.RotateFlip((System.Drawing.RotateFlipType)Enum.Parse(typeof(System.Drawing.RotateFlipType), this["Rotation"]));
            gfx.DrawImage(img, X, Y, width, height);
        }

        protected override void _LoadData(XmlNode node)
        {
        }
    }
}
