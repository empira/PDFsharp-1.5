#region PDFsharp - A .NET library for processing PDF
//
// An annotation may optionally be surrounded by a border when displayed or
// printed.If present, the border is drawn completely inside the annotation rectangle.
// In PDF 1.1, the characteristics of the border are specified by the Border
// entry in the annotation dictionary. Beginning with
// PDF 1.2, some types of annotation may instead specify their border characteristics 
// in a border style dictionary designated by the annotation’s BS entry.Such dictionaries 
// are also used to specify the width and dash pattern for the lines drawn
// by line, square, circle, and ink annotations.Table 8.12 summarizes the contents
// of the border style dictionary.If neither the Border nor the BS entry is present,
// the border is drawn as a solid line with a width of 1 point.
// 
// 
// 
// Authors:
//   Andrey Ryzhkov
//
#endregion

using System;
using System.Collections.Generic;

namespace PdfSharp.Pdf.Annotations
{
    public class PdfBorderStyle : PdfDictionary
    {
        public const int DASH_3 = 1;
        public const int DASH_6_3 = 2;


        private PdfArray _dashArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBorderStyle"/> class.
        /// </summary>
        public PdfBorderStyle()
        {
            Initialize();
        }

        void Initialize()
        {
            Elements.SetName(Keys.Type, "/Border");
            Elements.SetReal(Keys.W, 1.0);
            Elements.SetName(Keys.S, "/S");

            _dashArray = new PdfArray();
            _dashArray.Elements.Add(new PdfInteger(3));
        }

        /// <summary>
        /// Get or set border width in points. If this value is 0, no border is drawn.
        /// </summary>
        public double BorderWidth
        {
            get
            {
                return Elements.GetReal(Keys.W);
            }
            set
            {
                if (value <= 0)
                {
                    Elements.SetReal(Keys.W, 0);
                    Elements.Remove(Keys.S);
                    Elements.Remove(Keys.D);
                }
                else
                {
                    Elements.SetReal(Keys.W, value);
                }
            }
        }

        /// <summary>
        /// Get or set border style.
        /// </summary>
        public StyleBorder BorderStyle
        {
            get
            {
                string value = Elements.GetName(Keys.S);
                value = value.Substring(1);
                switch (value)
                {
                    case "S":
                        return StyleBorder.Solid;
                    case "D":
                        return StyleBorder.Dashed;
                    case "B":
                        return StyleBorder.Beveled;
                    case "I":
                        return StyleBorder.Inset;
                    case "U":
                        return StyleBorder.Underline;
                }
                return StyleBorder.ERROR;
            }
            set
            {
                if (Enum.IsDefined(typeof(StyleBorder), value)
                    && value != StyleBorder.ERROR)
                {
                    char v = value.ToString("G")[0];
                    Elements.SetName(Keys.S, "/" + v);
                    if (value == StyleBorder.Dashed)
                    {
                        Elements["/D"] = _dashArray;
                    }
                    else if (value == StyleBorder.Solid)
                    {
                        Elements.Remove(Keys.D);
                        
                    }
                }
            }
        }

        /// <summary>
        /// Get or set dash array. A dash array defining a pattern of dashes
        /// an gaps to be used in drawing a dashed border.
        /// </summary>
        public PdfArray DashArray
        {
            get
            {
                return _dashArray;
            }
            set
            {
                _dashArray = value;
                Elements["/D"] = _dashArray;
            }
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        public class Keys : KeysBase
        {
            /// <summary>
            /// (Optional) The type of PDF object that this dictionary describes; if present, must be
            /// Border for a border style dictionary
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional, FixedValue = "Border")]
            public const string Type = "/Type";

            /// <summary>
            /// (Optional) The border width in points. If this value is 0, no border is drawn.
            /// Default value: 1.
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Optional)]
            public const string W = "/W";

            /// <summary>
            /// (Optional) The border style:
            /// S(Solid) A solid rectangle surrounding the annotation.
            /// D(Dashed) A dashed rectangle surrounding the annotation.The dash pattern
            ///             is specified by the D entry (see below).
            /// B(Beveled) A simulated embossed rectangle that appears to be raised above the
            ///             surface of the page.
            /// I(Inset) A simulated engraved rectangle that appears to be recessed below the
            ///             surface of the page.
            /// U(Underline) A single line along the bottom of the annotation rectangle.
            ///             Other border styles may be defined in the future. (See implementation note 64 in
            ///             Appendix H.) Default value: S.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string S = "/S";

            /// <summary>
            /// (Optional) A dash array defining a pattern of dashes and gaps to be used in drawing a
            /// dashed border(border style D above). The dash array is specified in the same format
            /// as in the line dash pattern parameter of the graphics state(see “Line Dash Pattern” on
            /// page 155). The dash phase is not specified and is assumed to be 0. For example, a D
            /// entry of[3 2] specifies a border drawn with 3-point dashes alternating with 2-point
            /// gaps.Default value: [3].
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string D = "/D";
        }
    }

    /// <summary>
    /// The border style
    /// </summary>
    public enum StyleBorder
    {
        ERROR,
        Solid,
        Dashed,
        Beveled,
        Inset,
        Underline
    }

    /// <summary>
    /// Pre-define border style array
    /// </summary>
    public class DashArray
    {
        public static readonly Dictionary<dashArray, PdfArray> Standard = new Dictionary<dashArray, PdfArray>();

        static DashArray()
        {
            PdfArray arr_2_2 = new PdfArray();
            arr_2_2.Elements.Add(new PdfInteger(2));
            arr_2_2.Elements.Add(new PdfInteger(2));
            Standard[dashArray.Dash_2_2] = arr_2_2;

            PdfArray arr_3_3 = new PdfArray();
            arr_3_3.Elements.Add(new PdfInteger(3));
            arr_3_3.Elements.Add(new PdfInteger(3));
            Standard[dashArray.Dash_3_3] = arr_3_3;

            PdfArray arr_4_4 = new PdfArray();
            arr_4_4.Elements.Add(new PdfInteger(4));
            arr_4_4.Elements.Add(new PdfInteger(4));
            Standard[dashArray.Dash_4_4] = arr_4_4;

            PdfArray arr_4_3_2_3 = new PdfArray();
            arr_4_3_2_3.Elements.Add(new PdfInteger(4));
            arr_4_3_2_3.Elements.Add(new PdfInteger(3));
            arr_4_3_2_3.Elements.Add(new PdfInteger(2));
            arr_4_3_2_3.Elements.Add(new PdfInteger(3));
            Standard[dashArray.Dash_4_3_2_3] = arr_4_3_2_3;

            PdfArray arr_4_3_16_3 = new PdfArray();
            arr_4_3_16_3.Elements.Add(new PdfInteger(4));
            arr_4_3_16_3.Elements.Add(new PdfInteger(3));
            arr_4_3_16_3.Elements.Add(new PdfInteger(16));
            arr_4_3_16_3.Elements.Add(new PdfInteger(3));
            Standard[dashArray.Dash_4_3_16_3] = arr_4_3_16_3;

            PdfArray arr_8_4_4_4 = new PdfArray();
            arr_8_4_4_4.Elements.Add(new PdfInteger(8));
            arr_8_4_4_4.Elements.Add(new PdfInteger(4));
            arr_8_4_4_4.Elements.Add(new PdfInteger(4));
            arr_8_4_4_4.Elements.Add(new PdfInteger(4));
            Standard[dashArray.Dash_8_4_4_4] = arr_8_4_4_4;
        }

        /// <summary>
        /// Dash array's types for dased border style
        /// </summary>
        public enum dashArray
        {
            Dash_2_2,
            Dash_3_3,
            Dash_4_4,
            Dash_4_3_2_3,
            Dash_4_3_16_3,
            Dash_8_4_4_4
        }
    }
}
