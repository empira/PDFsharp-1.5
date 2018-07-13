#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Andrey Ryzhkov
//
#endregion

using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf.Annotations.DA;

namespace PdfSharp.Pdf.Annotations
{
    /// <summary>
    /// Represents a free text annotation (PDF 1.3).
    /// </summary>
    public sealed class PdfFreeTextAnnotation : PdfAnnotation
    {
        private AnnotationDefaultAppearance _da;
        private PdfBorderStyle _borderStyle;
        private PdfBorderEffect _borderEffect;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFreeTextAnnotation"/> class.
        /// </summary>
        public PdfFreeTextAnnotation()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFreeTextAnnotation"/> class.
        /// </summary>
        /// <param name="document"></param>
        public PdfFreeTextAnnotation(PdfDocument document)
            : base(document)
        {
            Initialize();
        }

        void Initialize()
        {
            Elements.SetName(Keys.Subtype, "/FreeText");
            Elements.SetInteger(Keys.Q, 0);
            _da = new AnnotationDefaultAppearance();
            Elements.SetString(Keys.DA, _da.ToString());
            _borderStyle = new PdfBorderStyle();
            _borderEffect = new PdfBorderEffect();
        }

        /// <summary>
        /// Gets or sets justification to be used in displaying the annotation’s text:
        ///   0 Left-justified
        ///   1 Centered
        ///   2 Right-justified
        /// </summary>
        public int Justification
        {
            get { return Elements.GetInteger(Keys.Q); }
            set
            {
                if (value >= 0 && value <= 2)
                {
                    Elements.SetInteger(Keys.Q, value);
                }
            }
        }

        /// <summary>
        /// Set annotation's font from <see cref="StandardType1Font"/>.
        /// </summary>
        public void SetFont(StandardType1Font font)
        {
            _da.SetFont(font);
            Elements.SetString(Keys.DA, _da.ToString());
        }

        /// <summary>
        /// Set font size.
        /// </summary>
        /// <param name="size">Font size in points.</param>
        public void SetFontSize(float size)
        {
            _da.SetFontSize(size);
            Elements.SetString(Keys.DA, _da.ToString());
        }

        /// <summary>
        /// Set font color. <see cref="XColor"/>
        /// </summary>
        public void SetFontColor(XColor color)
        {
            _da.SetColor(color);
            Elements.SetString(Keys.DA, _da.ToString());
        }

        /// <summary>
        /// Get or set border style. <see cref="PdfBorderStyle"/>
        /// </summary>
        public PdfBorderStyle BorderStyle
        {
            get
            {
                return _borderStyle;
            }
            set
            {
                _borderStyle = value;
                Elements[Keys.BS] = _borderStyle;
            }
        }

        /// <summary>
        /// Get or set border effect. <see cref="PdfBorderEffect"/>
        /// </summary>
        public PdfBorderEffect BorderEffect
        {
            get
            {
                return _borderEffect;
            }
            set
            {
                _borderEffect = value;
                Elements[Keys.BE] = _borderEffect;
            }
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal new class Keys : PdfAnnotation.Keys
        {
            /// <summary>
            /// (Required) The default appearance string to be used in formatting the text.
            /// </summary>
            [KeyInfo(KeyType.String | KeyType.Required)]
            public const string DA = "/DA";

            /// <summary>
            /// (Optional; PDF 1.4) A code specifying the form of quadding (justification) to be
            /// used in displaying the annotation’s text:
            ///   0 Left-justified
            ///   1 Centered
            ///   2 Right-justified
            /// Default value: 0 (left-justified).
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string Q = "/Q";

            /// <summary>
            /// (Optional; PDF 1.5) Begining with PDF 1.5, some annotations (square, circle and polygon) may have
            /// a BE entry, which is a border effect dictionary that specifies an effect to be applied
            /// to the border of the annotations.
            /// </summary>
            [KeyInfo("1.5", KeyType.Dictionary | KeyType.Optional)]
            public const string BE = "/BE";
        }
    }
}
