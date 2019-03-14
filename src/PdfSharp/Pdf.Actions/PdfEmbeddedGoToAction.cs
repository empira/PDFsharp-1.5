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

using System;
using System.Diagnostics;
using PdfSharp.Pdf.IO;

namespace PdfSharp.Pdf.Actions
{
    /// <summary>
    /// Represents a PDF Embedded Goto action.
    /// </summary>
    public sealed class PdfEmbeddedGoToAction : PdfAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEmbeddedGoToAction"/> class.
        /// </summary>
        public PdfEmbeddedGoToAction()
        {
            Inititalize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfEmbeddedGoToAction"/> class.
        /// </summary>
        /// <param name="document">The document that owns this object.</param>
        public PdfEmbeddedGoToAction(PdfDocument document)
            : base(document)
        {
            Inititalize();
        }

        /// <summary>
        /// Creates a link to an embedded document.
        /// </summary>
        /// <param name="destinationPath">The path to the named destination through the embedded documents.
        /// The path is separated by '\' and the last segment is the name of the named nestination.
        /// The other segments describe the route from the current (root or embedded) document to the embedded document holding the destination.
        /// ".." references to the parent, other strings refer to a child with this name in the EmbeddedFiles name dictionary.</param>
        /// <param name="newWindow">True, if the destination document shall be opened in a new window.
        /// If not set, the viewer application should behave in accordance with the current user preference.</param>
        public static PdfEmbeddedGoToAction CreatePdfEmbeddedGoToAction(string destinationPath, bool? newWindow = null)
        {
            return CreatePdfEmbeddedGoToAction(null, destinationPath, newWindow);
        }

        /// <summary>
        /// Creates a link to an embedded document in another document.
        /// </summary>
        /// <param name="documentPath">The path to the target document.</param>
        /// <param name="destinationPath">The path to the named destination through the embedded documents in the target document.
        /// The path is separated by '\' and the last segment is the name of the named destination.
        /// The other segments describe the route from the root document to the embedded document.
        /// Each segment name refers to a child with this name in the EmbeddedFiles name dictionary.</param>
        /// <param name="newWindow">True, if the destination document shall be opened in a new window.
        /// If not set, the viewer application should behave in accordance with the current user preference.</param>
        public static PdfEmbeddedGoToAction CreatePdfEmbeddedGoToAction(string documentPath, string destinationPath, bool? newWindow = null)
        {
            PdfEmbeddedGoToAction action = new PdfEmbeddedGoToAction();
            action._documentPath = documentPath;
            action._destinationPath = destinationPath;
            action._newWindow = newWindow;
            return action;
        }
        string _documentPath;
        string _destinationPath;
        bool? _newWindow;

        void Inititalize()
        {
            Elements.SetName(PdfAction.Keys.Type, "/Action");
            Elements.SetName(PdfAction.Keys.S, "/GoToE");
        }

        internal override void WriteObject(PdfWriter writer)
        {
            if (!string.IsNullOrEmpty(_documentPath))
            {
                var encodedPath = EncodePath(_documentPath);
                Elements.SetString(Keys.F, encodedPath);
            }

            ParseDestinationName();

            if (_newWindow.HasValue)
                Elements.SetBoolean(Keys.NewWindow, _newWindow.Value);

            base.WriteObject(writer);
        }

        string EncodePath(string path)
        {
            var result = path.Replace("\\", "/");
            return result;
        }

        void ParseDestinationName()
        {
            // _destinationPath may contain a path routing through embedded documents.
            var segments = _destinationPath.Split(Separator);

            Debug.Assert(segments.Length > 0);

            var currentElementsObject = Elements;

            // Target dictionaries to create for the path: Create a row of TargetDictionaries as we step through the segments.
            for (var i = 0; i < segments.Length - 1; i++)
            {
                var segment = segments[i];
                var target = segment == ParentString
                    ? TargetDictionary.CreateTargetParent()
                    : TargetDictionary.CreateTargetChild(segment);

                currentElementsObject.SetObject(Keys.T, target);

                currentElementsObject = target.Elements;
            }

            // The destination is the last segment of the path. It has to be saved in the embedded GoTo-Action's Elements.
            var destination = segments[segments.Length - 1];
            Elements.SetString(Keys.D, destination);
        }
        /// <summary>
        /// Seperator for splitting destination path segments ans destination name.
        /// </summary>
        public const char Separator = '\\';
        /// <summary>
        /// Path segment string used to move to the parent document.
        /// </summary>
        public const string ParentString = "..";

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal new class Keys : PdfAction.Keys
        {
            ///// <summary>
            ///// (Required) The type of action that this dictionary describes;
            ///// must be GoToE for an embedded go-to action.
            ///// </summary>
            //[KeyInfo(KeyType.Name | KeyType.Required, FixedValue = "GoToE")]
            //public const string S = "/S";

            /// <summary>
            /// (Optional) The root document of the target relative to the root document of the source.
            /// If this entry is absent, the source and target share the same root document.
            /// </summary>
            [KeyInfo(KeyType.FileSpecification | KeyType.Optional)]
            public const string F = "/F";

            /// <summary>
            /// (Required) The destination in the target to jump to (see Section 8.2.1, “Destinations”).
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.ByteString | KeyType.Array | KeyType.Required)]
            public const string D = "/D";

            /// <summary>
            /// (Optional) If true, the destination document should be opened in a new window;
            /// if false, the destination document should replace the current document in the same window.
            /// If this entry is absent, the viewer application should honor the current user preference.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string NewWindow = "/NewWindow";

            /// <summary>
            /// (Optional if F is present; otherwise required) A target dictionary (see Table 8.52)
            /// specifying path information to the target document. Each target dictionary specifies
            /// one element in the full path to the target and may have nested target dictionaries
            /// specifying additional elements.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string T = "/T";
        }

        internal class TargetDictionary : PdfDictionary
        {
            TargetDictionary()
            { }

            public static TargetDictionary CreateTargetChild(string name)
            {
                var target = new TargetDictionary();

                target.Elements.SetName(Keys.R, "/C");
                target.Elements.SetString(Keys.N, name);

                return target;
            }

            public static TargetDictionary CreateTargetParent()
            {
                var target = new TargetDictionary();

                target.Elements.SetName(Keys.R, "/P");

                return target;
            }

            /// <summary>
            /// Predefined keys of this dictionary.
            /// </summary>
            internal class Keys : KeysBase
            {
                /// <summary>
                /// (Required) Specifies the relationship between the current document and the target
                /// (which may be an intermediate target). Valid values are P (the target is the parent
                /// of the current document) and C (the target is a child of the current document).
                /// </summary>
                [KeyInfo(KeyType.Name | KeyType.Required)]
                public const string R = "/R";

                /// <summary>
                /// (Required if the value of R is C and the target is located in the EmbeddedFiles name tree;
                /// otherwise, it must be absent) The name of the file in the EmbeddedFiles name tree.
                /// </summary>
                [KeyInfo(KeyType.ByteString | KeyType.Optional)]
                public const string N = "/N";

                ///// <summary>
                ///// (Required if the value of R is C and the target is associated with a file attachment annotation;
                ///// otherwise, it must be absent) If the value is an integer, it specifies the page number (zero-based)
                ///// in the current document containing the file attachment annotation. If the value is a string,
                ///// it specifies a named destination in the current document that provides the page number of the
                ///// file attachment annotation.
                ///// </summary>
                //[KeyInfo(KeyType.Integer | KeyType.ByteString | KeyType.Optional)]
                //public const string P = "/P";

                ///// <summary>
                ///// (Required if the value of R is C and the target is associated with a file attachment annotation;
                ///// otherwise, it must be absent) If the value is an integer, it specifies the index (zero-based)
                ///// of the annotation in the Annots array (see Table 3.27) of the page specified by P. If the value
                ///// is a text string, it specifies the value of NM in the annotation dictionary (see Table 8.15).
                ///// </summary>
                //[KeyInfo(KeyType.Integer | KeyType.TextString | KeyType.Optional)]
                //public const string A = "/A";

                /// <summary>
                /// (Optional) A target dictionary specifying additional path information to the target document.
                /// If this entry is absent, the current document is the target file containing the destination.
                /// </summary>
                [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
                public const string T = "/T";
            }
        }
    }
}
