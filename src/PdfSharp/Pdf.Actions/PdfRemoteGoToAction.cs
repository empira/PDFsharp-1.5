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

using PdfSharp.Pdf.IO;

namespace PdfSharp.Pdf.Actions
{
    /// <summary>
    /// Represents a PDF Remote Goto action.
    /// </summary>
    public sealed class PdfRemoteGoToAction : PdfAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfRemoteGoToAction"/> class.
        /// </summary>
        public PdfRemoteGoToAction()
        {
            Inititalize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfRemoteGoToAction"/> class.
        /// </summary>
        /// <param name="document">The document that owns this object.</param>
        public PdfRemoteGoToAction(PdfDocument document)
            : base(document)
        {
            Inititalize();
        }

        /// <summary>
        /// Creates a link to another document.
        /// </summary>
        /// <param name="documentPath">The path to the target document.</param>
        /// <param name="destinationName">The named destination's name in the target document.</param>
        /// <param name="newWindow">True, if the destination document shall be opened in a new window.
        /// If not set, the viewer application should behave in accordance with the current user preference.</param>
        public static PdfRemoteGoToAction CreateRemoteGoToAction(string documentPath, string destinationName, bool? newWindow = null)
        {
            PdfRemoteGoToAction action = new PdfRemoteGoToAction();
            action._documentPath = documentPath;
            action._destinationName = destinationName;
            action._newWindow = newWindow;
            return action;
        }
        string _documentPath;
        string _destinationName;
        bool? _newWindow;

        void Inititalize()
        {
            Elements.SetName(PdfAction.Keys.Type, "/Action");
            Elements.SetName(PdfAction.Keys.S, "/GoToR");
        }

        internal override void WriteObject(PdfWriter writer)
        {
            var encodedPath = EncodePath(_documentPath);
            Elements.SetString(Keys.F, encodedPath);

            Elements.SetString(Keys.D, _destinationName);

            if (_newWindow.HasValue)
                Elements.SetBoolean(Keys.NewWindow, _newWindow.Value);

            base.WriteObject(writer);
        }

        string EncodePath(string path)
        {
            var result = path.Replace("\\", "/");
            return result;
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal new class Keys : PdfAction.Keys
        {
            ///// <summary>
            ///// (Required) The type of action that this dictionary describes;
            ///// must be GoToR for a remote go-to action.
            ///// </summary>
            //[KeyInfo(KeyType.Name | KeyType.Required, FixedValue = "GoToR")]
            //public const string S = "/S";

            /// <summary>
            /// (Required) The destination to jump to (see Section 8.5.3, “Action Types”).
            /// </summary>
            [KeyInfo(KeyType.String | KeyType.Dictionary | KeyType.Required)]
            //[KeyInfo(KeyType.FileSpecification | KeyType.Required)] // File Specifications are not yet implemented.
            public const string F = "/F";

            /// <summary>
            /// (Required) The destination to jump to (see Section 8.2.1, “Destinations”).
            /// If the value is an array defining an explicit destination (as described under “Explicit Destinations” on page 582),
            /// its first element must be a page number within the remote document rather than an indirect reference to a page object
            /// in the current document. The first page is numbered 0.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.ByteString | KeyType.Array | KeyType.Required)]
            public const string D = "/D";

            /// <summary>
            /// (Optional; PDF 1.2) A flag specifying whether to open the destination document in a new window.
            /// If this flag is false, the destination document replaces the current document in the same window.
            /// If this entry is absent, the viewer application should behave in accordance with the current user preference.
            /// </summary>
            [KeyInfo("1.2", KeyType.Boolean | KeyType.Optional)]
            public const string NewWindow = "/NewWindow";
        }
    }
}
