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
    /// Represents a PDF table attributes object.
    /// </summary>
    public class PdfTableAttributes : PdfAttributesBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTableAttributes"/> class.
        /// </summary>
        /// <param name="document">The document that owns this object.</param>
        internal PdfTableAttributes(PdfDocument document)
            : base(document)
        {
            SetOwner();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTableAttributes"/> class.
        /// </summary>
        public PdfTableAttributes()
        {
            SetOwner();
        }

        private void SetOwner()
        {
            Elements.SetName(PdfAttributesBase.Keys.O, "/Table");
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        public new class Keys : PdfAttributesBase.Keys
        {
            // Reference: TABLE 10.36  Standard table attributes / Page 935

            // ReSharper disable InconsistentNaming

            /// <summary>
            /// (Optional; not inheritable) The number of rows in the enclosing table that are spanned
            /// by the cell. The cell expands by adding rows in the block-progression direction
            /// specified by the table’s WritingMode attribute. Default value: 1.
            /// This entry applies only to table cells that have structure types TH or TD or that are
            /// role mapped to structure types TH or TD.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string RowSpan = "/RowSpan";

            /// <summary>
            /// (Optional; not inheritable) The number of columns in the enclosing table that are spanned
            /// by the cell. The cell expands by adding columns in the inline-progression direction
            /// specified by the table’s WritingMode attribute. Default value: 1.
            /// This entry applies only to table cells that have structure types TH or TD or that are
            /// role mapped to structure types TH or TD.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string ColSpan = "/ColSpan";

            // ReSharper restore InconsistentNaming
        }
    }
}
