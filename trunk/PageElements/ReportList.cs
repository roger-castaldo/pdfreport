using System;
using System.Collections.Generic;
using System.Text;
using Org.Reddragonit.PDFReports.Interfaces;
using System.Xml;
using Org.Reddragonit.PDFReports.ReportElements;
using Org.Reddragonit.PDFReports.Exceptions;
using Org.Reddragonit.PDFReports.PDF;
using Org.Reddragonit.PDFReports.PDF.PageComponents;

namespace Org.Reddragonit.PDFReports.PageElements
{
    internal class ReportList : PageElement
    {
        private const string _LETTER_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public struct sListItem
        {
            private string _font;
            public string Font
            {
                get { return _font; }
            }

            private List<object> _content;
            public List<object> Content
            {
                get { return _content; }
            }

            private string _bullet;
            public string Bullet
            {
                get{
                    return _bullet;
                }
            }

            private ReportPage _owningPage;

            public double GetHeight(PDFGraphics gfx,PDFFont font,double textWidth)
            {
                double ret = 0;
                if (_content.Count > 0)
                {
                    double curX = 0;
                    for (int x=0;x<_content.Count;x++)
                    {
                        object obj = _content[x];
                        if (obj is sParagraphChunk)
                        {
                            sParagraphChunk sp = (sParagraphChunk)obj;
                            string text = sp.Text;
                            PDFFont fnt = (sp.Font == null ? font : _owningPage.GetFont(sp.Font));
                            double width = gfx.MeasureString(text, fnt).Width;
                            if (width + curX > textWidth)
                            {
                                _content.RemoveAt(x);
                                x--;
                                int index = 0;
                                string tmp = text[index].ToString();
                                while (true)
                                {
                                    index++;
                                    if (text[index] == '\n')
                                    {
                                        ret += gfx.MeasureString(tmp, fnt).Height;
                                        _content.Insert(x + 1, new sParagraphChunk(tmp + "\n", sp.Font, sp.URL));
                                        x++;
                                        tmp = "";
                                    }
                                    else
                                    {
                                        tmp += text[index];
                                        width = gfx.MeasureString(tmp, fnt).Width;
                                        if (index >= text.Length)
                                            break;
                                        else if (width - curX == textWidth)
                                        {
                                            ret += gfx.MeasureString(tmp, fnt).Height;
                                            _content.Insert(x + 1, new sParagraphChunk(tmp + "\n", sp.Font, sp.URL));
                                            x++;
                                            tmp = "";
                                        }
                                        else if (width - curX > textWidth)
                                        {
                                            ret += gfx.MeasureString(tmp, fnt).Height;
                                            _content.Insert(x + 1, new sParagraphChunk(tmp.Substring(0, tmp.Length - 1) + "\n", sp.Font, sp.URL));
                                            x++;
                                            tmp = tmp.Substring(tmp.Length - 1);
                                        }
                                    }
                                }
                                if (tmp != "")
                                {
                                    ret += gfx.MeasureString(tmp, fnt).Height;
                                    _content.Insert(x + 1, new sParagraphChunk(tmp, sp.Font, sp.URL));
                                    x++;
                                }
                                curX = width;
                            }
                            else
                            {
                                if (curX == 0)
                                    ret += gfx.MeasureString(text, fnt).Height;
                                curX += textWidth;
                            }
                        }
                        else
                        {
                            ReportList rl = (ReportList)obj;
                            ret += rl.GetHeight(gfx,gfx.PageSize.Width-textWidth);
                        }
                    }
                }
                else
                    ret = gfx.MeasureString(Bullet, font).Height;
                return ret;
            }

            public sListItem(string font, List<object> content,int count,ReportListTypes type,int level,ReportPage owningPage)
            {
                _font = font;
                _content = content;
                _bullet = GetBulletChar(type, level, count);
                _owningPage = owningPage;
            }
        }

        public enum ReportListTypes
        {
            Numbered,
            Lettered,
            Bulleted
        }

        private List<sListItem> _children;
        ReportList _parent;

        protected ReportListTypes ListType
        {
            get
            {
                if (this["Type"] != null)
                    return (ReportListTypes)Enum.Parse(typeof(ReportListTypes), this["Type"]);
                else if (_parent != null)
                    return _parent.ListType;
                else
                    return ReportListTypes.Bulleted;
            }
        }

        protected int Level
        {
            get
            {
                if (this["Type"] != null)
                    return 0;
                int ret = 0;
                if (_parent != null)
                {
                    ret = _parent.Level;
                    switch (ListType)
                    {
                        case ReportListTypes.Bulleted:
                            ret = (ret >= 2 ? 0 : ret);
                            break;
                        case ReportListTypes.Lettered:
                            ret = (ret >= 2 ? 0 : ret);
                            break;
                        case ReportListTypes.Numbered:
                            ret = (ret >= 3 ? 0 : ret);
                            break;
                    }
                }
                return ret+1;
            }
        }

        protected double ListIndent
        {
            get
            {
                return (this["Indent"] != null ? double.Parse(this["Indent"]) : (_parent != null ? _parent.ListIndent : 0)) + (_parent != null ? _parent.ListIndent + _parent.ItemIdent : 0);
            }
        }

        protected double ItemIdent
        {
            get
            {
                return (this["ItemIndent"] != null ? double.Parse(this["ItemIndent"]) : (_parent != null ? _parent.ListIndent : 0))+(_parent!=null ? _parent.ListIndent+_parent.ItemIdent : 0);
            }
        }

        protected string Font
        {
            get
            {
                return (this["Font"] != null ? this["Font"] : (_parent != null ? _parent.Font : null));
            }
        }

        protected string BulletFont
        {
            get
            {
                return (this["BulletFont"] != null ? this["BulletFont"] : (_parent != null ? _parent.BulletFont : null));
            }
        }

        protected bool AllowItemSplitOnPage
        {
            get { return (this["AllowItemSplitOnPage"] != null ? bool.Parse(this["AllowItemSplitOnPage"]) : (_parent != null ? _parent.AllowItemSplitOnPage : true)); }
        }

        public ReportList(XmlNode node, ReportPage page,ReportList parent)
            : base(node, page)
        {
            if (node.Name != "List")
                throw new UnexpectedElementException("List", node.Name, node);
            _parent = parent;
            _children = new List<sListItem>();
            foreach (XmlNode n in node)
            {
                if (n.Name == "ListItem")
                    _children.Add(LoadListItem(n));
                else
                    throw new UnexpectedElementException("ListItem", n.Name, n);
            }
        }

        internal double GetHeight(PDFGraphics gfx,double startX)
        {
            double ret = 0;
            double width = 0;
            if (this["Width"] != null)
                width = float.Parse(this["Width"]);
            else
                width = (ignoreMargins ? gfx.PageSize.Width : gfx.PageSize.Width - OwningPage.RightMargin) - X;
            double curY = Y;
            double numStart = startX + ListIndent;
            double textStart = 0;
            foreach (sListItem sli in _children)
            {
                double curWidth = gfx.MeasureString(sli.Bullet, OwningPage.GetFont(BulletFont)).Width;
                textStart = (textStart < curWidth ? curWidth : textStart);
            }
            textStart += ItemIdent + numStart;
            double curX = textStart;
            double textWidth = width - textStart;
            foreach (sListItem sli in _children)
                ret += sli.GetHeight(gfx, OwningPage.GetFont(Font), textWidth);
            return ret;
        }

        internal override void AppendToPage(ref PDFGraphics gfx, out double width, out double height)
        {
            height = 0;
            double startX = X;
            if (this["Width"]!=null)
                width = float.Parse(this["Width"]);
            else
                width = (ignoreMargins ? gfx.PageSize.Width : gfx.PageSize.Width-OwningPage.RightMargin)-X;
            double curY=Y;
            double startY = Y;
            double numStart = startX + ListIndent;
            double textStart=0;
            foreach (sListItem sli in _children)
            {
                double curWidth = gfx.MeasureString(sli.Bullet, OwningPage.GetFont(BulletFont)).Width;
                textStart = (textStart< curWidth ? curWidth : textStart);
            }
            textStart += ItemIdent+numStart;
            double curX = textStart;
            double textWidth = width - textStart;
            double wid = 0;
            double lineHeight = 0;
            foreach (sListItem sli in _children)
            {
                lineHeight = gfx.MeasureString(sli.Bullet, OwningPage.GetFont(Font)).Height;
                if (curY + lineHeight < gfx.PageSize.Height + (ignoreMargins ? 0 : OwningPage.BottomMargin))
                {
                    OwningPage.InitPage(ref gfx);
                    curY = (ignoreMargins ? 0 : OwningPage.TopMargin);
                    startY = (ignoreMargins ? 0 : OwningPage.TopMargin);
                }
                else if (!AllowItemSplitOnPage)
                {
                    if (sli.GetHeight(gfx, OwningPage.GetFont((sli.Font == null ? Font : sli.Font)), textWidth) > (ignoreMargins ? gfx.PageSize.Height - curY : gfx.PageSize.Height - OwningPage.BottomMargin - curY))
                    {
                        OwningPage.InitPage(ref gfx);
                        curY = (ignoreMargins ? 0 : OwningPage.TopMargin);
                        startY = (ignoreMargins ? 0 : OwningPage.TopMargin);
                    }
                }

                foreach (object obj in sli.Content)
                {
                    if (obj is sParagraphChunk)
                    {
                        sParagraphChunk sp = (sParagraphChunk)obj;
                        if (AllowItemSplitOnPage)
                        {
                            if (gfx.MeasureString(sp.Text, OwningPage.GetFont((sp.Font == null ? Font : sp.Font))).Height + curY > gfx.PageSize.Height - (ignoreMargins ? 0 : OwningPage.BottomMargin))
                            {
                                OwningPage.InitPage(ref gfx);
                                curY = (ignoreMargins ? 0 : OwningPage.TopMargin);
                                startY = (ignoreMargins ? 0 : OwningPage.TopMargin);
                            }
                        }
                        gfx.DrawString(sp.Text, OwningPage.GetFont((sp.Font == null ? Font : sp.Font)), PDFBrush.Black, curX, curY);
                        if (sp.Text.Contains("\n"))
                            curY += gfx.MeasureString(sp.Text, OwningPage.GetFont((sp.Font == null ? Font : sp.Font))).Height;
                        else
                            curX += gfx.MeasureString(sp.Text, OwningPage.GetFont((sp.Font == null ? Font : sp.Font))).Width;
                    }
                    else
                    {
                        ((ReportList)obj).AppendToPage(ref gfx, out wid, out lineHeight);
                        curX = textStart;
                        curY += lineHeight;
                    }
                }
                curY += gfx.MeasureString(sli.Bullet, OwningPage.GetFont(sli.Font == null ? Font : sli.Font)).Height;
            }
            height = curY - startY;
        }

        protected override void _LoadData(XmlNode node)
        {
        }

        private sListItem LoadListItem(XmlNode node)
        {
            List<object> content = new List<object>();
            bool endsWithLine = false;
            foreach (XmlNode n in node.ChildNodes)
            {
                if (n.NodeType == XmlNodeType.Text)
                {
                    content.Add(new sParagraphChunk(n.Value));
                    endsWithLine = n.Value.EndsWith("\n");
                }
                else if (n.NodeType == XmlNodeType.Element)
                {
                    if (n.Name == "List")
                    {
                        if ((content.Count > 0)&&!endsWithLine)
                            content.Add(new sParagraphChunk("\n"));
                        ReportList rl = new ReportList(n, OwningPage, this);
                        content.Add(rl);
                        endsWithLine = true;
                    }
                    else if (n.Name == "Chunk")
                        content.Add(new sParagraphChunk(n,this["Font"]));
                    else
                        throw new UnexpectedElementException("List", n.Name, n);
                }
            }
            if (!endsWithLine)
                content.Add(new sParagraphChunk("\n"));
            return new sListItem((node.Attributes["Font"] != null ? node.Attributes["Font"].Value : null), content,_children.Count+1,ListType,Level,OwningPage);
        }

        private static string GetBulletChar(ReportListTypes type, int level, int count)
        {
            string ret = "";
            switch (type)
            {
                case ReportListTypes.Bulleted:
                    ret = ((char)(level == 1 ? 0x2A : 0xB0)).ToString();
                    break;
                case ReportListTypes.Lettered:
                    while (count > _LETTER_CHARS.Length)
                    {
                        ret += _LETTER_CHARS[count - 1].ToString();
                        count -= _LETTER_CHARS.Length;
                    }
                    ret += _LETTER_CHARS[count];
                    if (level == 2)
                        ret = ret.ToLower();
                    ret += ".";
                    break;
                case ReportListTypes.Numbered:
                    ret = (level == 3 ? NumberToRoman(count).ToLower() : (level == 2 ? NumberToRoman(count) : count.ToString()));
                    ret += ".";
                    break;
            }
            return ret;
        }

        private static readonly int[] _ROMAN_NUMBERS = new int[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 3, 2, 1 };
        private static readonly string[] _ROMAN_NUMBERALS = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "III", "II", "I" }; 

        private static string NumberToRoman(int number)
        {
            // Validate
            if (number < 0 || number > 3999)
                throw new ArgumentException("Value must be in the range 0 - 3,999.");

            if (number == 0) return "N";

            // Initialise the string builder
            StringBuilder result = new StringBuilder();

            // Loop through each of the values to diminish the number
            for (int i = 0; i < _ROMAN_NUMBERS.Length; i++)
            {
                // If the number being converted is less than the test value, append
                // the corresponding numeral or numeral pair to the resultant string
                while (number >= _ROMAN_NUMBERS[i])
                {
                    number -= _ROMAN_NUMBERS[i];
                    result.Append(_ROMAN_NUMBERALS[i]);
                }
            }

            // Done
            return result.ToString();
        }
    }
}
