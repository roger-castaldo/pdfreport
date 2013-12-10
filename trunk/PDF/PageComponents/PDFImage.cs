using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Org.Reddragonit.PDFReports.PDF.PageComponents
{
    internal class PDFImage : ADocumentPiece
    {
        private long _xObjectOffset;
        public long XObjectOffset
        {
            get { return _xObjectOffset; }
        }

        private int _xObjectID;
        public int XObjectID
        {
            get { return _xObjectID; }
        }

        private string _imageStream;
        private double _height;
        private double _width;
        private double _imageWidth;
        private double _imageHeight;
        private double _x;
        private double _y;
        private byte[] _content;

        public PDFImage(Image image, double x, double y, int objectID,int xObjectID)
            : this(image,x,y,(double)image.Width,(double)image.Height,objectID,xObjectID)
        {}

        public PDFImage(Image image, double x, double y, double width, double height, int objectID,int xObjectID)
            : base(objectID)
        {
            _x = x;
            _y = y+height;
            _width = width;
            _height = height*-1;
            _imageHeight = (double)image.Height;
            _imageWidth = (double)image.Width;
            _xObjectID = xObjectID;
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Jpeg);
            _content = ms.ToArray();
        }

        protected override void _AppendContent(StreamWriter bw)
        {
            _imageStream = string.Format(
@"q
{0:0.###} 0 0 {1:0.###} {2:0.###} {3:0.###} cm
/I{4} Do
Q",new object[]{_width, _height, _x, _y,_xObjectID});
            bw.Write(string.Format("/Length {0}\n",_imageStream.Length));
        }

        public override void Append(StreamWriter bw)
        {
            base.Append(bw);
            bw.Flush();
            bw.BaseStream.Position -= ("endobj\n").Length;
            bw.Write(string.Format(@"stream
{0}
endstream
endobj
", _imageStream));
            bw.Flush();
            _xObjectOffset = bw.BaseStream.Position;

            bw.Write(string.Format(
@"{0} 0 obj
<<
/Type /XObject
/Subtype /Image
/Name /I{0}
/Filter /DCTDecode
/Width {1:0.###}
/Height {2:0.###}
/BitsPerComponent 8
/ColorSpace /DeviceRGB
/Length {3}
>>
stream
",
new object[]{_xObjectID,_imageWidth,_imageHeight,_content.Length}));
            bw.Flush();
            bw.BaseStream.Write(_content, 0, _content.Length);
            bw.Write(@"
endstream
endobj
");
        }
    }
}
