#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Andrey Ryzhkov
//
#endregion

using System.Collections.Generic;
using System.Globalization;

using PdfSharp.Drawing;
using PdfSharp.Fonts;

namespace PdfSharp.Pdf.Annotations.DA
{
    class AnnotationDefaultAppearance
    {
        private static readonly IDictionary<StandardType1Font, string> stdAnnotFontNames = 
            new Dictionary<StandardType1Font, string>();

        private string color = "1 g";

        private string fontName = "/Helvetica";

        private float fontSize = 0f;

        static AnnotationDefaultAppearance()
        {
            stdAnnotFontNames.Add(StandardType1Font.Courier, "/" + StandardType1Fonts.COURIER);
            stdAnnotFontNames.Add(StandardType1Font.CourierBold, "/" + StandardType1Fonts.COURIER_BOLD);
            stdAnnotFontNames.Add(StandardType1Font.CourierOblique, "/" + StandardType1Fonts.COURIER_OBLIQUE);
            stdAnnotFontNames.Add(StandardType1Font.CourierBoldOblique, "/" + StandardType1Fonts.COURIER_BOLDOBLIQUE);

            stdAnnotFontNames.Add(StandardType1Font.Helvetica, "/" + StandardType1Fonts.HELVETICA);
            stdAnnotFontNames.Add(StandardType1Font.HelveticaBold, "/" + StandardType1Fonts.HELVETICA_BOLD);
            stdAnnotFontNames.Add(StandardType1Font.HelveticaOblique, "/" + StandardType1Fonts.HELVETICA_OBLIQUE);
            stdAnnotFontNames.Add(StandardType1Font.HelveticaBoldOblique, "/" + StandardType1Fonts.HELVETICA_BOLDOBLIQUE);

            stdAnnotFontNames.Add(StandardType1Font.TimesRoman, "/" + StandardType1Fonts.TIMES_ROMAN);
            stdAnnotFontNames.Add(StandardType1Font.TimesBold, "/" + StandardType1Fonts.TIMES_BOLD);
            stdAnnotFontNames.Add(StandardType1Font.TimesItalic, "/" + StandardType1Fonts.TIMES_ITALIC);
            stdAnnotFontNames.Add(StandardType1Font.TimesBoldItalic, "/" + StandardType1Fonts.TIMES_BOLDITALIC);

            stdAnnotFontNames.Add(StandardType1Font.Symbol, "/" + StandardType1Fonts.SYMBOL);

            stdAnnotFontNames.Add(StandardType1Font.ZapfDingbats, "/" + StandardType1Fonts.ZAPFDINGBATS);
        }

        public AnnotationDefaultAppearance()
        {
            SetFont(StandardType1Font.Helvetica);
            SetFontSize(10f);
            SetColor(XColor.FromGrayScale(1.0));
        }

        public void SetFont(StandardType1Font font)
        {
            fontName = stdAnnotFontNames[font];
        }

        public void SetFontSize(float size)
        {
            fontSize = size;
        }

        public void SetColor(XColor color)
        {
            if (color.ColorSpace.Equals(XColorSpace.GrayScale))
            {
                this.color = color.GS.ToString("0.0#####", CultureInfo.InvariantCulture) + " g";
            }
            else if (color.ColorSpace.Equals(XColorSpace.Rgb))
            {
                this.color = color.R.ToString("0.0#####", CultureInfo.InvariantCulture) + " " 
                    + color.G.ToString("0.0#####", CultureInfo.InvariantCulture) + " " 
                    + color.B.ToString("0.0#####", CultureInfo.InvariantCulture) + " rg";
            }
            else if (color.ColorSpace.Equals(XColorSpace.Cmyk))
            {
                this.color = color.C.ToString("0.0#####", CultureInfo.InvariantCulture) + " " 
                    + color.M.ToString("0.0#####", CultureInfo.InvariantCulture) + " " 
                    + color.Y.ToString("0.0#####", CultureInfo.InvariantCulture) + " " 
                    + color.K.ToString("0.0#####", CultureInfo.InvariantCulture) + " k";
            }
        }

        /// <summary>
        /// Get string value from AnnotationDefaultAppearance
        /// </summary>
        public override string ToString()
        {
            return fontName + " " + fontSize.ToString() + " Tf " + color;
        }
    }
}
