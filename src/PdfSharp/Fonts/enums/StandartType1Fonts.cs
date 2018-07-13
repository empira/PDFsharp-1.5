#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Andrey Ryzhkov
//
#endregion

using System;
using System.Collections.Generic;

namespace PdfSharp.Fonts
{
    public sealed class StandardType1Fonts
    {

        //private StandardType1Fonts()
        //{
        //}

        private static readonly ICollection<string> _FONTS = new List<string>();

        static StandardType1Fonts()
        {
            _FONTS.Add(PdfSharp.Fonts.StandardType1Fonts.COURIER);
            _FONTS.Add(PdfSharp.Fonts.StandardType1Fonts.COURIER_BOLD);
            _FONTS.Add(PdfSharp.Fonts.StandardType1Fonts.COURIER_BOLDOBLIQUE);
            _FONTS.Add(PdfSharp.Fonts.StandardType1Fonts.COURIER_OBLIQUE);
            _FONTS.Add(PdfSharp.Fonts.StandardType1Fonts.HELVETICA);
            _FONTS.Add(PdfSharp.Fonts.StandardType1Fonts.HELVETICA_BOLD);
            _FONTS.Add(PdfSharp.Fonts.StandardType1Fonts.HELVETICA_BOLDOBLIQUE);
            _FONTS.Add(PdfSharp.Fonts.StandardType1Fonts.HELVETICA_OBLIQUE);
            _FONTS.Add(PdfSharp.Fonts.StandardType1Fonts.SYMBOL);
            _FONTS.Add(PdfSharp.Fonts.StandardType1Fonts.TIMES_ROMAN);
            _FONTS.Add(PdfSharp.Fonts.StandardType1Fonts.TIMES_BOLD);
            _FONTS.Add(PdfSharp.Fonts.StandardType1Fonts.TIMES_BOLDITALIC);
            _FONTS.Add(PdfSharp.Fonts.StandardType1Fonts.TIMES_ITALIC);
            _FONTS.Add(PdfSharp.Fonts.StandardType1Fonts.ZAPFDINGBATS);
        }

        public static bool IsStandardFont(String fontName)
        {
            return _FONTS.Contains(fontName);
        }

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const string COURIER = "Courier";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const string COURIER_BOLD = "Courier-Bold";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const string COURIER_OBLIQUE = "Courier-Oblique";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const string COURIER_BOLDOBLIQUE = "Courier-BoldOblique";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const string HELVETICA = "Helvetica";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const string HELVETICA_BOLD = "Helvetica-Bold";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const string HELVETICA_OBLIQUE = "Helvetica-Oblique";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const string HELVETICA_BOLDOBLIQUE = "Helvetica-BoldOblique";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const string SYMBOL = "Symbol";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const string TIMES_ROMAN = "Times-Roman";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const string TIMES_BOLD = "Times-Bold";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const string TIMES_ITALIC = "Times-Italic";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const string TIMES_BOLDITALIC = "Times-BoldItalic";

        /// <summary>This is a possible value of a base 14 type 1 font</summary>
        public const string ZAPFDINGBATS = "ZapfDingbats";
    }

    /// <summary>
    /// The PostScript names of 14 Type 1 fonts, known as the standard fonts.
    /// These fonts, or their font metrics and suitable substitution fonts, are guaranteed
    /// to be available to the viewer application.
    /// </summary>
    public enum StandardType1Font
    {
        TimesRoman,
        TimesBold,
        TimesItalic,
        TimesBoldItalic,
        Helvetica,
        HelveticaBold,
        HelveticaOblique,
        HelveticaBoldOblique,
        Courier,
        CourierBold,
        CourierOblique,
        CourierBoldOblique,
        Symbol,
        ZapfDingbats
    }
}