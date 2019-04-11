#region PDFsharp - A .NET library for processing PDF
//
// Begining with PDF 1.5, some annotations(square, circle and polygon) may have
// a BE entry, which is a border effect dictionary that specifies an effect to be applied
// to the border of the annotations.
// 
// Authors:
//   Andrey Ryzhkov
//
#endregion

using System;

namespace PdfSharp.Pdf.Annotations
{
    public class PdfBorderEffect : PdfDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBorderEffect"/> class.
        /// </summary>
        public PdfBorderEffect()
        {
            Elements.SetName(Keys.S, "/S");
            Elements.SetInteger(Keys.I, 0);   
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBorderEffect"/> class.
        /// </summary>
        /// <param name="effect">Border effect <see cref="pdfBorderEffect"/></param>
        public PdfBorderEffect(pdfBorderEffect effect)
        {
            Effect = effect;
        }

        /// <summary>
        /// Get or set border effect.
        /// </summary>
        public pdfBorderEffect Effect
        {
            get
            {
                return (pdfBorderEffect)Enum.Parse(typeof(pdfBorderEffect), Enum.GetName(typeof(pdfBorderEffect), Elements.GetInteger(Keys.I)));
            }
            set
            {
                if (Enum.IsDefined(typeof(pdfBorderEffect), value))
                {
                    if (value == pdfBorderEffect.None)
                    {
                        Elements.SetName(Keys.S, "/S");
                    }
                    else if (value == pdfBorderEffect.Cloud1 || value == pdfBorderEffect.Cloud2)
                    {
                        Elements.SetName(Keys.S, "/C");
                    }
                    Elements.SetInteger(Keys.I, (int)value);
                }

            }
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        public class Keys : KeysBase
        {
            /// <summary>
            /// (Optional) A name representing the border effect to apply. Possible values are:
            ///    S - No effect: the border is as described by the annotation dictionary's BS entry.
            ///    C - The border should appear "cloudly". The width and dash array specified by BS are honored.
            /// Default value: S.
            /// </summary>
            [KeyInfo("1.5", KeyType.Name | KeyType.Optional)]
            public const string S = "/S";

            /// <summary>
            /// (Optional; valid only if the value of S is C) A number describing the intensity of the
            /// effect. Suggest values range from 0 to 2.
            /// Default value: 0.
            /// </summary>
            [KeyInfo("1.5", KeyType.Integer | KeyType.Optional)]
            public const string I = "/I";
        }
    }

    /// <summary>
    /// Border effect types.
    /// </summary>
    public enum pdfBorderEffect
    {
        None = 0,
        Cloud1 = 1,
        Cloud2 = 2
    }


}
