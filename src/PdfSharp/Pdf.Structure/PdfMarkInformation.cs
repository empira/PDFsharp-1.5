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
    /// Represents a mark information dictionary.
    /// </summary>
    public sealed class PdfMarkInformation : PdfDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMarkInformation"/> class.
        /// </summary>
        public PdfMarkInformation()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMarkInformation"/> class.
        /// </summary>
        /// <param name="document">The document that owns this object.</param>
        public PdfMarkInformation(PdfDocument document)
            : base(document)
        { }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal class Keys : KeysBase
        {
            // Reference: TABLE 10.8  Entries in the mark information dictionary / Page 856

            // ReSharper disable InconsistentNaming

            /// <summary>
            /// (Optional) A flag indicating whether the document conforms to Tagged PDF conventions.
            /// Default value: false.
            /// Note: If Suspects is true, the document may not completely conform to Tagged PDF conventions.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string Marked = "/Marked";

            /// <summary>
            /// (Optional; PDF 1.6) A flag indicating the presence of structure elements
            /// that contain user properties attributes.
            /// Default value: false.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string UserProperties = "/UserProperties";

            /// <summary>
            /// (Optional; PDF 1.6) A flag indicating the presence of tag suspects.
            /// Default value: false.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string Suspects = "/Suspects";

            // ReSharper restore InconsistentNaming
        }
    }
}
