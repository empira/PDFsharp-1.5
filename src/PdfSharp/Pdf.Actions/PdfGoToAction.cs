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
    /// Represents a PDF Goto action.
    /// </summary>
    public sealed class PdfGoToAction : PdfAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGoToAction"/> class.
        /// </summary>
        public PdfGoToAction()
        {
            Inititalize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGoToAction"/> class.
        /// </summary>
        /// <param name="document">The document that owns this object.</param>
        public PdfGoToAction(PdfDocument document)
            : base(document)
        {
            Inititalize();
        }

        /// <summary>
        /// Creates a link within the current document.
        /// </summary>
        /// <param name="destinationName">The Named Destination's name in the target document.</param>
        public static PdfGoToAction CreateGoToAction(string destinationName)
        {
            PdfGoToAction action = new PdfGoToAction();
            action._destinationName = destinationName;
            return action;
        }
        string _destinationName;

        void Inititalize()
        {
            Elements.SetName(PdfAction.Keys.Type, "/Action");
            Elements.SetName(PdfAction.Keys.S, "/GoTo");
        }

        internal override void WriteObject(PdfWriter writer)
        {
            Elements.SetString(PdfRemoteGoToAction.Keys.D, _destinationName);

            base.WriteObject(writer);
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal new class Keys : PdfAction.Keys
        {
            ///// <summary>
            ///// (Required) The type of action that this dictionary describes;
            ///// must be GoTo for a go-to action.
            ///// </summary>
            //[KeyInfo(KeyType.Name | KeyType.Required, FixedValue = "GoTo")]
            //public const string S = "/S";

            /// <summary>
            /// (Required) The destination to jump to (see Section 8.2.1, “Destinations”).
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.ByteString | KeyType.Array | KeyType.Required)]
            public const string D = "/D";
        }
    }
}
