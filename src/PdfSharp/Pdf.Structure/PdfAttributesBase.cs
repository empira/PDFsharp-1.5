#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2019 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

namespace PdfSharp.Pdf.Structure
{
    /// <summary>
    /// Base class for PDF attributes objects.
    /// </summary>
    public abstract class PdfAttributesBase : PdfDictionary
    {
        /// <summary>
        /// Constructor of the abstract <see cref="PdfSharp.Pdf.Structure.PdfAttributesBase"/> class.
        /// </summary>
        /// <param name="document">The document that owns this object.</param>
        internal PdfAttributesBase(PdfDocument document)
            : base(document)
        { }

        /// <summary>
        /// Constructor of the abstract <see cref="PdfSharp.Pdf.Structure.PdfAttributesBase"/> class.
        /// </summary>
        protected PdfAttributesBase()
        { }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        public class Keys : KeysBase
        {
            // Reference: TABLE 10.14  Entry common to all attribute object dictionaries / Page 873
            // Reference: TABLE 10.28  Standard attribute owners / Page 914

            // ReSharper disable InconsistentNaming

            /// <summary>
            /// (Required) The name of the application or plug-in extension owning the attribute data.
            /// The name must conform to the guidelines described in Appendix E
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required)]
            public const string O = "/O";

            // ReSharper restore InconsistentNaming
        }
    }
}
