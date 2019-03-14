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
    public sealed class PdfMarkedContentReference : PdfDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMarkedContentReference"/> class.
        /// </summary>
        public PdfMarkedContentReference()
        {
            Elements.SetName(Keys.Type, "/MCR");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMarkedContentReference"/> class.
        /// </summary>
        /// <param name="document">The document that owns this object.</param>
        public PdfMarkedContentReference(PdfDocument document)
            : base(document)
        {
            Elements.SetName(Keys.Type, "/MCR");
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal class Keys : KeysBase
        {
            // Reference: TABLE 10.11  Entries in a marked-content reference dictionary / Page 863

            // ReSharper disable InconsistentNaming

            /// <summary>
            /// (Required) The type of PDF object that this dictionary describes;
            /// must be MCR for a marked-content reference.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required, FixedValue = "MCR")]
            public const string Type = "/Type";

            /// <summary>
            /// (Optional; must be an indirect reference) The page object representing
            /// the page on which the graphics objects in the marked-content sequence
            /// are rendered. This entry overrides any Pg entry in the structure element
            /// containing the marked-content reference;
            /// it is required if the structure element has no such entry.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string Pg = "/Pg";

            /// <summary>
            /// (Optional; must be an indirect reference) The content stream containing
            /// the marked-content sequence. This entry should be present only if the
            /// marked-content sequence resides in a content stream other than the
            /// content stream for the page—for example, in a form XObject or an
            /// annotation’s appearance stream. If this entry is absent, the
            /// marked-content sequence is contained in the content stream of the page
            /// identified by Pg (either in the marked-content reference dictionary or
            /// in the parent structure element).
            /// </summary>
            [KeyInfo(KeyType.Stream | KeyType.Optional)]
            public const string Stm = "/Stm";

            /// <summary>
            /// (Optional; must be an indirect reference) The PDF object owning the stream
            /// identified by Stm—for example, the annotation to which an appearance stream belongs.
            /// </summary>
            [KeyInfo(KeyType.Optional)]
            public const string StmOwn = "/StmOwn";

            /// <summary>
            /// (Required) The marked-content identifier of the marked-content sequence
            /// within its content stream.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string MCID = "/MCID";

            // ReSharper restore InconsistentNaming
        }
    }
}
