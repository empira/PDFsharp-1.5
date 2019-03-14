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

using PdfSharp.Pdf.Advanced;

namespace PdfSharp.Pdf.Structure
{
    /// <summary>
    /// Represents the root of a structure tree.
    /// </summary>
    public sealed class PdfStructureTreeRoot : PdfDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStructureTreeRoot"/> class.
        /// </summary>
        public PdfStructureTreeRoot()
        {
            Elements.SetName(Keys.Type, "/StructTreeRoot");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStructureTreeRoot"/> class.
        /// </summary>
        /// <param name="document">The document that owns this object.</param>
        public PdfStructureTreeRoot(PdfDocument document)
            : base(document)
        {
            Elements.SetName(Keys.Type, "/StructTreeRoot");
        }

        internal override void PrepareForSave()
        {
            foreach (var k in PdfStructureElement.GetKids(Elements))
                k.PrepareForSave();
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal class Keys : KeysBase
        {
            // Reference: TABLE 10.9  Entries in the structure tree root / Page 857

            // ReSharper disable InconsistentNaming

            /// <summary>
            /// (Required) The type of PDF object that this dictionary describes;
            /// must be StructTreeRoot for a structure tree root.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required, FixedValue = "StructTreeRoot")]
            public const string Type = "/Type";

            /// <summary>
            /// (Optional) The immediate child or children of the structure tree root
            /// in the structure hierarchy. The value may be either a dictionary
            /// representing a single structure element or an array of such dictionaries.
            /// </summary>
            [KeyInfo(KeyType.ArrayOrDictionary | KeyType.Optional)]
            public const string K = "/K";

            /// <summary>
            /// (Required if any structure elements have element identifiers)
            /// A name tree that maps element identifiers to the structure elements they denote.
            /// </summary>
            [KeyInfo(KeyType.Optional)]
            public const string IDTree = "/IDTree";

            /// <summary>
            /// (Required if any structure element contains content items) A number tree
            /// used in finding the structure elements to which content items belong.
            /// Each integer key in the number tree corresponds to a single page of the
            /// document or to an individual object (such as an annotation or an XObject)
            /// that is a content item in its own right. The integer key is given as the
            /// value of the StructParent or StructParents entry in that object.
            /// The form of the associated value depends on the nature of the object:
            ///     • For an object that is a content item in its own right, the value is an
            ///       indirect reference to the object’s parent element (the structure element
            ///       that contains it as a content item).
            ///     • For a page object or content stream containing marked-content sequences
            ///       that are content items, the value is an array of references to the parent
            ///       elements of those marked-content sequences.
            /// </summary>
            [KeyInfo(KeyType.NumberTree | KeyType.Optional)]
            public const string ParentTree = "/ParentTree";

            /// <summary>
            /// (Optional) An integer greater than any key in the parent tree, to be used as a
            /// key for the next entry added to the tree.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string ParentTreeNextKey = "/ParentTreeNextKey";

            /// <summary>
            /// (Optional) A dictionary that maps the names of structure types used in the
            /// document to their approximate equivalents in the set of standard structure types.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string RoleMap = "/RoleMap";

            /// <summary>
            /// (Optional) A dictionary that maps name objects designating attribute
            /// classes to the corresponding attribute objects or arrays of attribute objects.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string ClassMap = "/ClassMap";

            // ReSharper restore InconsistentNaming
        }
    }
}
