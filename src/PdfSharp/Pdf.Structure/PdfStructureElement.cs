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

using System.Collections.Generic;
using PdfSharp.Pdf.Advanced;

namespace PdfSharp.Pdf.Structure
{
    /// <summary>
    /// Represents the root of a structure tree.
    /// </summary>
    public sealed class PdfStructureElement : PdfDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStructureElement"/> class.
        /// </summary>
        public PdfStructureElement()
        {
            Elements.SetName(Keys.Type, "/StructElem");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStructureElement"/> class.
        /// </summary>
        /// <param name="document">The document that owns this object.</param>
        public PdfStructureElement(PdfDocument document)
            : base(document)
        {
            Elements.SetName(Keys.Type, "/StructElem");
        }

        internal override void PrepareForSave()
        {
            SimplifyKidsArray();
            SimplifyAttributes();

            foreach (var k in GetKids(Elements))
                k.PrepareForSave();
        }

        /// <summary>
        /// Returns all PdfDictionaries saved in the "/K" key.
        /// </summary>
        internal static IEnumerable<PdfDictionary> GetKids(DictionaryElements elements)
        {
            if (elements != null)
            {
                var k = elements.GetObject(Keys.K);

                var array = k as PdfArray;
                // If k is holding an array, return all elements.
                if (array != null)
                {
                    foreach (var item in array)
                    {
                        var dict = GetPdfDictionary(item);
                        if (dict != null)
                            yield return dict;
                    }
                }
                else
                {
                    var dict = GetPdfDictionary(k);
                    if (dict != null)
                        yield return dict;
                }
            }
        }

        /// <summary>
        /// Returns the PdfDictionary that is lying direct or inderect in "item".
        /// </summary>
        static PdfDictionary GetPdfDictionary(PdfItem item)
        {
            var r = item as PdfReference;
            if (r != null)
                return r.Value as PdfDictionary;
            return item as PdfDictionary;
        }

        /// <summary>
        /// Removes the array and directly adds its first item, if there is only one item.
        /// </summary>
        void SimplifyKidsArray()
        {
            var k = Elements[Keys.K] as PdfArray;
            if (k != null && k.Elements.Count == 1)
            {
                var item = k.Elements[0];
                Elements[Keys.K] = item;
            }
        }

        /// <summary>
        /// Removes unnessecary Attribute dictionaries or arrays.
        /// </summary>
        void SimplifyAttributes()
        {
            var a = Elements[Keys.A];

            var array = a as PdfArray;
            if (array != null)
            {
                // Remove attribute dictionaries that don't contain relevant entries.
                for (var i = 0; i < array.Elements.Count; i++)
                {
                    var dict = GetPdfDictionary(array.Elements[i]);
                    if (dict != null && AttributeDictionaryIsEmpty(dict))
                        array.Elements.RemoveAt(i--);
                }

                // Remove the array and directly add its first item, if there is only one item.
                if (array.Elements.Count == 1)
                {
                    var item = array.Elements[0];
                    Elements[Keys.A] = item;
                }
                // Remove the key, if the array is empty.
                else if (array.Elements.Count == 0)
                    Elements.Remove(Keys.A);
            }
            else
            {
                // Remove the attribute dictionary, if it doesn't contain relevant entries.
                var dict = GetPdfDictionary(a);
                if (dict != null && AttributeDictionaryIsEmpty(dict))
                    Elements.Remove(Keys.A);
            }
        }

        bool AttributeDictionaryIsEmpty(PdfDictionary dictionary)
        {
            if (dictionary.Elements.Count == 0)
                return true;
            // Also return true, if the only entry is the required "/O".
            if (dictionary.Elements.Count == 1 && dictionary.Elements.ContainsKey(PdfAttributesBase.Keys.O))
                return true;
            return false;
        }

        /// <summary>
        /// Gets the PdfLayoutAttributes instance in "/A". If not existing it creates one.
        /// </summary>
        public PdfLayoutAttributes LayoutAttributes
        {
            get { return GetAttributes<PdfLayoutAttributes>(); }
        }

        /// <summary>
        /// Gets the PdfTableAttributes instance in "/A". If not existing it creates one.
        /// </summary>
        public PdfTableAttributes TableAttributes
        {
            get { return GetAttributes<PdfTableAttributes>(); }
        }

        T GetAttributes<T>() where T : PdfAttributesBase, new()
        {
            var a = Elements[Keys.A];
            var array = a as PdfArray;
            if (array == null)
            {
                // If there is no PdfArray saved in "/A", create one.
                array = new PdfArray(Owner);
                // If there is anything saved in "/A", move it into the array.
                if (a != null)
                    array.Elements.Add(a);
                Elements.SetObject(Keys.A, array);
            }

            // Return the first instance of T in the array.
            for (var i = 0; i < array.Elements.Count; i++)
            {
                var c = array.Elements[i];
                if (c is T)
                    return c as T;
            }

            // Create and add a new instance of T, if there's no one.
            var t = new T { Document = Owner };
            array.Elements.Add(t);
            return t;
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal class Keys : KeysBase
        {
            // Reference: TABLE 10.10  Entries in a structure element dictionary / Page 858

            // ReSharper disable InconsistentNaming

            /// <summary>
            /// (Optional) The type of PDF object that this dictionary describes;
            /// if present, must be StructElem for a structure element.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional, FixedValue = "StructElem")]
            public const string Type = "/Type";

            /// <summary>
            /// (Required) The structure type, a name object identifying the nature of the 
            /// structure element and its role within the document, such as a chapter, 
            /// paragraph, or footnote. 
            /// Names of structure types must conform to the guidelines described in Appendix E.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required)]
            public const string S = "/S";

            /// <summary>
            /// (Required; must be an indirect reference) The structure element that
            /// is the immediate parent of this one in the structure hierarchy.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Required)]
            public const string P = "/P";

            /// <summary>
            /// (Optional) The element identifier, a byte string designating this structure element.
            /// The string must be unique among all elements in the document’s structure hierarchy.
            /// The IDTree entry in the structure tree root defines the correspondence between
            /// element identifiers and the structure elements they denote.
            /// </summary>
            [KeyInfo(KeyType.ByteString | KeyType.Optional)]
            public const string ID = "/ID";

            /// <summary>
            /// (Optional; must be an indirect reference) A page object representing a page on
            /// which some or all of the content items designated by the K entry are rendered.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string Pg = "/Pg";

            /// <summary>
            /// (Optional) The children of this structure element. The value of this entry
            /// may be one of the following objects or an array consisting of one or more
            /// of the following objects:
            ///     • A structure element dictionary denoting another structure element
            ///     • An integer marked-content identifier denoting a marked-content sequence
            ///     • A marked-content reference dictionary denoting a marked-content sequence
            ///     • An object reference dictionary denoting a PDF object
            /// Each of these objects other than the first (structure element dictionary)
            /// is considered to be a content item.
            /// Note: If the value of K is a dictionary containing no Type entry,
            /// it is assumed to be a structure element dictionary.
            /// </summary>
            [KeyInfo(KeyType.Various | KeyType.Optional)]
            public const string K = "/K";

            /// <summary>
            /// (Optional) A single attribute object or array of attribute objects associated
            /// with this structure element. Each attribute object is either a dictionary or
            /// a stream. If the value of this entry is an array, each attribute object in
            /// the array may be followed by an integer representing its revision number.
            /// </summary>
            [KeyInfo(KeyType.Various | KeyType.Optional)]
            public const string A = "/A";

            /// <summary>
            /// (Optional) An attribute class name or array of class names associated with this
            /// structure element. If the value of this entry is an array, each class name in the
            /// array may be followed by an integer representing its revision number.
            /// Note: If both the A and C entries are present and a given attribute is specified
            /// by both, the one specified by the A entry takes precedence.
            /// </summary>
            [KeyInfo(KeyType.Various | KeyType.Optional)]
            public const string C = "/C";

            /// <summary>
            /// (Optional) The title of the structure element, a text string representing it in 
            /// human-readable form. The title should characterize the specific structure element,
            /// such as Chapter 1, rather than merely a generic element type, such as Chapter.
            /// </summary>
            [KeyInfo(KeyType.TextString | KeyType.Optional)]
            public const string T = "/T";

            /// <summary>
            /// (Optional; PDF 1.4) A language identifier specifying the natural language
            /// for all text in the structure element except where overridden by language
            /// specifications for nested structure elements or marked content.
            /// If this entry is absent, the language (if any) specified in the document catalog applies.
            /// </summary>
            [KeyInfo(KeyType.TextString | KeyType.Optional)]
            public const string Lang = "/Lang";

            /// <summary>
            /// (Optional) An alternate description of the structure element and its children
            /// in human-readable form, which is useful when extracting the document’s contents
            /// in support of accessibility to users with disabilities or for other purposes.
            /// </summary>
            [KeyInfo(KeyType.TextString | KeyType.Optional)]
            public const string Alt = "/Alt";

            /// <summary>
            /// (Optional; PDF 1.5) The expanded form of an abbreviation.
            /// </summary>
            [KeyInfo(KeyType.TextString | KeyType.Optional)]
            public const string E = "/E";

            /// <summary>
            /// (Optional; PDF 1.4) Text that is an exact replacement for the structure element and
            /// its children. This replacement text (which should apply to as small a piece of
            /// content as possible) is useful when extracting the document’s contents in support
            /// of accessibility to users with disabilities or for other purposes.
            /// </summary>
            [KeyInfo(KeyType.TextString | KeyType.Optional)]
            public const string ActualText = "/ActualText";

            // ReSharper restore InconsistentNaming
        }
    }
}
