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
    /// Represents a marked-content reference.
    /// </summary>
    public sealed class PdfObjectReference : PdfDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfObjectReference"/> class.
        /// </summary>
        public PdfObjectReference()
        {
            Elements.SetName(Keys.Type, "/OBJR");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfObjectReference"/> class.
        /// </summary>
        /// <param name="document">The document that owns this object.</param>
        public PdfObjectReference(PdfDocument document)
            : base(document)
        {
            Elements.SetName(Keys.Type, "/OBJR");
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal class Keys : KeysBase
        {
            // Reference: TABLE 10.12  Entries in an object reference dictionary / Page 868

            // ReSharper disable InconsistentNaming

            /// <summary>
            /// (Required) The type of PDF object that this dictionary describes;
            /// must be OBJR for an object reference.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required, FixedValue = "OBJR")]
            public const string Type = "/Type";

            /// <summary>
            /// (Optional; must be an indirect reference) The page object representing the page
            /// on which the object is rendered. This entry overrides any Pg entry in the
            /// structure element containing the object reference;
            /// it is required if the structure element has no such entry.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string Pg = "/Pg";

            /// <summary>
            /// (Required; must be an indirect reference) The referenced object.
            /// </summary>
            [KeyInfo(KeyType.Required)]
            public const string Obj = "/Obj";

            // ReSharper restore InconsistentNaming
        }
    }
}
