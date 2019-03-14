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

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Represents a name tree node.
    /// </summary>
    [DebuggerDisplay("({DebuggerDisplay})")]
    public sealed class PdfNameTreeNode : PdfDictionary
    {
        // Reference: 3.8.5  Name Trees / Page 161

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfNameTreeNode"/> class.
        /// </summary>
        public PdfNameTreeNode()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfNameTreeNode"/> class.
        /// </summary>
        public PdfNameTreeNode(bool isRoot)  //??? 
        {
            IsRoot = isRoot;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a root node.
        /// </summary>
        public bool IsRoot
        {
            get { return _isRoot; }
            private set { _isRoot = value; }
        }
        bool _isRoot;

        /// <summary>
        /// Gets the number of Kids elements.
        /// </summary>
        public int KidsCount
        {
            get
            {
                PdfArray kids = Elements.GetArray(Keys.Kids);
                return kids != null ? kids.Elements.Count : 0;
            }
        }

        /// <summary>
        /// Gets the number of Names elements.
        /// </summary>
        public int NamesCount
        {
            get
            {
                PdfArray names = Elements.GetArray(Keys.Names);
                // Entries are key / value pairs, so divide by 2.
                return names != null ? names.Elements.Count / 2 : 0;
            }
        }

        /// <summary>
        /// Adds a child node to this node.
        /// </summary>
        public void AddKid(PdfNameTreeNode kidNode)
        {
            PdfArray kids = Elements.GetArray(Keys.Kids);
            if (kids == null)
            {
                kids = new PdfArray();
                Elements.SetObject(Keys.Kids, kids);
            }
            kids.Elements.Add(kidNode);
            _updateRequired = true;
        }

        /// <summary>
        /// Adds a key/value pair to the Names array of this node.
        /// </summary>
        public void AddName(string key, PdfItem value)
        {
            PdfArray names = Elements.GetArray(Keys.Names);
            if (names == null)
            {
                names = new PdfArray();
                Elements.SetObject(Keys.Names, names);
            }

            // Insert names sorted by key.
            int i = 0;
            while (i < names.Elements.Count && string.CompareOrdinal(names.Elements.GetString(i), key) < 0)
                // Entries are key / value pairs, so add 2.
                i += 2;

            names.Elements.Insert(i, new PdfString(key));
            names.Elements.Insert(i + 1, value);
            _updateRequired = true;
        }

        /// <summary>
        /// Gets the least key.
        /// </summary>
        public string LeastKey
        {
            get { return "todo"; }
        }

        /// <summary>
        /// Gets the greatest key.
        /// </summary>
        public string GreatestKey
        {
            get { return "todo"; }
        }

        /// <summary>
        /// Updates the limits by inspecting Kids and Names.
        /// </summary>
        void UpdateLimits()
        {
            if (_updateRequired)
            {
                //todo Recalc Limits
                _updateRequired = false;
            }
        }
        bool _updateRequired;

        internal override void PrepareForSave()
        {
            UpdateLimits();
            // Check consistence...
            base.PrepareForSave();
        }

        internal override void WriteObject(PdfWriter writer)
        {
            GetType();
            base.WriteObject(writer);
        }

        ///// <summary>
        ///// Returns the value in the PDF date format.
        ///// </summary>
        //public override string ToString()
        //{
        //    string delta = _value.ToString("zzz").Replace(':', '\'');
        //    return String.Format("D:{0:yyyyMMddHHmmss}{1}'", _value, delta);
        //}

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal sealed class Keys : KeysBase
        {
            // Reference: TABLE 3.33  Entries in a name tree node dictionary / Page 162

            // ReSharper disable InconsistentNaming

            /// <summary>
            /// (Root and intermediate nodes only; required in intermediate nodes; 
            /// present in the root node if and only if Names is not present)
            /// An array of indirect references to the immediate children of this node
            /// The children may be intermediate or leaf nodes.
            /// </summary>
            [KeyInfo(KeyType.Array)]
            public const string Kids = "/Kids";

            /// <summary>
            /// (Root and leaf nodes only; required in leaf nodes; present in the root node if and only if Kidsis not present)
            /// An array of the form
            ///      [key1 value1 key2 value2 … keyn valuen]
            /// where each keyi is a string and the corresponding valuei is the object associated with that key.
            /// The keys are sorted in lexical order, as described below.
            /// </summary>
            [KeyInfo(KeyType.Array)]
            public const string Names = "/Names";

            /// <summary>
            /// (Intermediate and leaf nodes only; required)
            /// An array of two strings, specifying the (lexically) least and greatest keys included in the Names array 
            /// of a leaf node or in the Namesarrays of any leaf nodes that are descendants of an intermediate node.
            /// </summary>
            [KeyInfo(KeyType.Array)]
            public const string Limits = "/Limits";

            /// <summary>
            /// Gets the KeysMeta for these keys.
            /// </summary>
            public static DictionaryMeta Meta
            {
                get { return _meta ?? (_meta = CreateMeta(typeof(Keys))); }
            }
            static DictionaryMeta _meta;

            // ReSharper restore InconsistentNaming
        }

        /// <summary>
        /// Gets the KeysMeta of this dictionary type.
        /// </summary>
        internal override DictionaryMeta Meta
        {
            get { return Keys.Meta; }
        }

        /// <summary>
        /// Gets the DebuggerDisplayAttribute text.
        /// </summary>
        /// <value>The debugger display.</value>
        // ReSharper disable UnusedMember.Local
        string DebuggerDisplay
        // ReSharper restore UnusedMember.Local
        {
            get
            {
                return String.Format("root:{0}", _isRoot);
            }
        }
    }
}
