using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.Interfaces;
using System.Xml;
using Org.Reddragonit.PDFReports.Exceptions;
using System.IO;
using System.Drawing;
using Org.Reddragonit.PDFReports.ReportElements;
using Org.Reddragonit.PDFReports.PDF;

namespace Org.Reddragonit.PDFReports.PageElements.TableComponents
{
    internal class TableCellImage : TableElement
    {
        private byte[] _imageData;

        public Image DrawingImage
        {
            get
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(new MemoryStream(_imageData));
                if (this["Rotation"] != null)
                    img.RotateFlip((System.Drawing.RotateFlipType)Enum.Parse(typeof(System.Drawing.RotateFlipType), this["Rotation"]));
                return img;
            }
        }

        public TableCellImage(ReportTable table,XmlNode node,ReportPage page)
            : base(table,node,page)
        {
            if (node.Name != "Image")
                throw new UnexpectedElementException("Image", node.Name, node);
        }

        internal override void AppendToPage(ref PDFGraphics gfx, out double width, out double height)
        {
            width = GetWidth(gfx);
            height = GetHeight(gfx);
        }

        protected override void  _LoadData(XmlNode node)
        {
            if (node["EmbeddedPath"] != null)
            {
                if (Utility.LocateEmbededResource(node["EmbeddedPath"].InnerText) == null)
                    throw new MissingEmbeddedImageException(node["EmbeddedPath"].InnerText);
                Stream es = Utility.LocateEmbededResource(node["EmbeddedPath"].InnerText);
                int eOffset = 0;
                _imageData = new byte[es.Length];
                while (es.Position < es.Length)
                {
                    eOffset += es.Read(_imageData, eOffset, (eOffset + 500 < es.Length ? 500 : (int)(es.Length - es.Position)));
                }
                es.Close();
            }
            else if (node["FilePath"] != null)
            {
                if (node["FilePath"].Attributes["DirectorySeperator"] == null)
                    throw new MissingAttributeException("DirectorySeperator", "FilePath", node["FilePath"]);
                string path = node["FilePath"].InnerText;
                path = path.Replace(node["FilePath"].Attributes["DirectorySeperator"].Value,Path.DirectorySeparatorChar.ToString());
                if (!(new FileInfo(path).Exists))
                    throw new MissingImageFileException(path);
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                int fOffset = 0;
                _imageData = new byte[fs.Length];
                while (fs.Position < fs.Length)
                {
                    fOffset += fs.Read(_imageData, fOffset, (fOffset+500 < fs.Length ? 500 : (int)(fs.Length-fs.Position)));
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

        internal double GetHeight(PDFGraphics gfx)
        {
            if (this["Height"] == null)
            {
                Image img = Image.FromStream(new MemoryStream(_imageData));
                System.Drawing.GraphicsUnit unit = System.Drawing.GraphicsUnit.Point;
                switch (OwningTable.OwningPage.PageUnit)
                {
                    case GraphicsUnit.Centimeter:
                    case GraphicsUnit.Millimeter:
                        unit = System.Drawing.GraphicsUnit.Millimeter;
                        break;
                    case GraphicsUnit.Inch:
                        unit = System.Drawing.GraphicsUnit.Inch;
                        break;
                    case GraphicsUnit.Point:
                        unit = System.Drawing.GraphicsUnit.Point;
                        break;
                }
                return img.GetBounds(ref unit).Height*(OwningTable.OwningPage.PageUnit==GraphicsUnit.Centimeter ? 0.1 : 1);
            }
            else
                return double.Parse(this["Height"]);
        }

        internal double GetWidth(PDFGraphics gfx)
        {
            if (this["Width"] == null)
            {
                Image img = Image.FromStream(new MemoryStream(_imageData));
                System.Drawing.GraphicsUnit unit = System.Drawing.GraphicsUnit.Point;
                switch (OwningTable.OwningPage.PageUnit)
                {
                    case GraphicsUnit.Centimeter:
                    case GraphicsUnit.Millimeter:
                        unit = System.Drawing.GraphicsUnit.Millimeter;
                        break;
                    case GraphicsUnit.Inch:
                        unit = System.Drawing.GraphicsUnit.Inch;
                        break;
                    case GraphicsUnit.Point:
                        unit = System.Drawing.GraphicsUnit.Point;
                        break;
                }
                return img.GetBounds(ref unit).Height * (OwningTable.OwningPage.PageUnit == GraphicsUnit.Centimeter ? 0.1 : 1);
            }
            else
                return double.Parse(this["Width"]);
        }
    }
}
