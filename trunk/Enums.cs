using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.PDFReports
{
    public enum GraphicsUnit
    {
        Centimeter,
        Inch,
        Millimeter,
        Point
    }

    public enum FontStyle
    {
        Bold,
        BoldItalic,
        Italic,
        Regular,
        Strikeout,
        Underline
    }

    public enum Fonts
    {
        Courier,
        Helvetica,
        TimesRoman,
        Symbol,
        Zapfdingbats
    }

    internal enum Alignments
    {
        Left,
        Right,
        Center
    }

    internal enum VerticalAlignments
    {
        Top,
        Center,
        Bottom
    }

    internal enum PageSizes
    {
        A0,
        A1,
        A2,
        A3,
        A4,
        A5,
        RA0,
        RA1,
        RA2,
        RA3,
        RA4,
        RA5,
        B0,
        B1,
        B2,
        B3,
        B4,
        B5,
        Quarto,
        Foolscap,
        Executive,
        GovernmentLetter,
        Letter,
        Legal,
        Ledger,
        Tabloid,
        Post,
        Crown,
        LargePost,
        Demy,
        Medium,
        Royal,
        Elephant,
        DoubleDemy,
        QuadDemy,
        STMT,
        Folio,
        Statement,
        Size10x14
    }
}
