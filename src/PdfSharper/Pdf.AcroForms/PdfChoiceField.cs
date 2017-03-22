#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
//
// http://www.PdfSharper.com
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
using System.Collections.Generic;
using System.Diagnostics;

namespace PdfSharper.Pdf.AcroForms
{
    /// <summary>
    /// Represents the base class for all choice field dictionaries.
    /// </summary>
    public abstract class PdfChoiceField : PdfAcroField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfChoiceField"/> class.
        /// </summary>
        protected PdfChoiceField(PdfDocument document)
            : base(document)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfChoiceField"/> class.
        /// </summary>
        protected PdfChoiceField(PdfDictionary dict)
            : base(dict)
        { }

        /// <summary>
        /// Gets or sets the Value for the Field.
        /// For fields supporting multiple values (e.g. ListBox) use <see cref="PdfListBoxField.SelectedIndices"/> instead
        /// </summary>
        public override PdfItem Value
        {
            get
            {
                var item = base.Value;
                var pdfArray = item as PdfArray;
                if (pdfArray != null)
                {
                    if (pdfArray.Elements.Count > 0)
                        item = pdfArray.Elements[0];
                }
                if (item is PdfString)
                {
                    // First try the export value
                    var idx = IndexInOptArray(item.ToString(), true);
                    // If that is not found, try the string shown in the UI
                    if (idx < 0)
                        idx = IndexInOptArray(item.ToString(), false);
                    if (idx < 0)
                        return null;
                    // return the display text
                    return new PdfString(ValueInOptArray(idx, true));
                }
                return null;
            }
            set { base.Value = value; }
        }

        /// <summary>
        /// Gets or sets the Default value for the field
        /// </summary>
        public override PdfItem DefaultValue
        {
            get
            {
                var item = base.DefaultValue;
                var pdfArray = item as PdfArray;
                if (pdfArray != null)
                {
                    if (pdfArray.Elements.Count > 0)
                        item = pdfArray.Elements[0];
                }
                if (item is PdfString)
                {
                    // First try the export value
                    var idx = IndexInOptArray(item.ToString(), true);
                    // If that is not found, try the string shown in the UI
                    if (idx < 0)
                        idx = IndexInOptArray(item.ToString(), false);
                    if (idx < 0)
                        return null;
                    // return the display text
                    return new PdfString(ValueInOptArray(idx, true));
                }
                return null;
            }
            set { base.DefaultValue = value; }
        }

        /// <summary>
        /// Gets or sets the List of options (entries) available for selection.
        /// This is the list of values shown in the UI.
        /// </summary>
        public IList<string> Options
        {
            get
            {
                var result = new List<string>();
                var options = Elements.GetArray(Keys.Opt);
                if (options != null)
                {
                    foreach (var item in options)
                    {
                        var s = item as PdfString;
                        if (s != null)
                            result.Add(s.Value);
                        else
                        {
                            var array = item as PdfArray;
                            if (array != null)
                            {
                                // Pdf Reference 1.7, Section 12.7.4.4 : Value is the SECOND entry in the Array
                                // (the first value is the exported value)
                                var v = array.Elements.GetString(array.Elements.Count > 1 ? 1 : 0);
                                if (String.IsNullOrEmpty(v))
                                    v = "";
                                result.Add(v);
                            }
                        }
                    }
                }
                return result;
            }
            set
            {
                var ary = new PdfArray(_document);
                foreach (var item in value)
                    ary.Elements.Add(new PdfString(item));
                Elements.SetObject(Keys.Opt, ary);
            }
        }

        /// <summary>
        /// Gets the list of values for this Field. May or may not be equal to <see cref="Options"/> but has always the same amount of items.
        /// </summary>
        public IList<string> Values
        {
            get
            {
                var result = new List<string>();
                var options = Elements.GetArray(Keys.Opt);
                if (options != null)
                {
                    foreach (var item in options)
                    {
                        var s = item as PdfString;
                        if (s != null)
                            result.Add(s.Value);
                        else
                        {
                            var array = item as PdfArray;
                            if (array != null)
                            {
                                var ary = array;
                                var v = ary.Elements.GetString(0);
                                if (String.IsNullOrEmpty(v))
                                    v = "";
                                result.Add(v);
                            }
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the index of the specified string in the /Opt array or -1, if no such string exists.
        /// </summary>
        protected int IndexInOptArray(string value, bool useExportValue)
        {
            PdfArray opt = Elements.GetArray(Keys.Opt);

#if DEBUG  // Check with //R080317 implemention
            PdfArray opt2 = null;
            if (Elements[Keys.Opt] is PdfArray)
                opt2 = Elements[Keys.Opt] as PdfArray;
            else if (Elements[Keys.Opt] is Advanced.PdfReference)
            {
                //falls das Array nicht direkt am Element hängt, 
                //das Array aus dem referenzierten Element holen
                opt2 = ((Advanced.PdfReference)Elements[Keys.Opt]).Value as PdfArray;
            }
            Debug.Assert(ReferenceEquals(opt, opt2));
#endif

            if (opt != null)
            {
                int count = opt.Elements.Count;
                for (int idx = 0; idx < count; idx++)
                {
                    PdfItem item = opt.Elements[idx];
                    if (item is PdfString)
                    {
                        if (item.ToString() == value)
                            return idx;
                    }
                    else if (item is PdfArray)
                    {
                        PdfArray array = (PdfArray)item;
                        // Pdf Reference 1.7, Section 12.7.4.4: Should be a 2-element Array. Second value is the text shown in the UI.
                        if ((!useExportValue && array.Elements.Count > 1 && array.Elements[1].ToString() == value) ||
                            (array.Elements.Count > 0 && array.Elements[0].ToString() == value))
                            return idx;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the value from the index in the /Opt array.
        /// </summary>
        protected string ValueInOptArray(int index, bool useExportValue)
        {
            PdfArray opt = Elements.GetArray(Keys.Opt);
            if (opt != null)
            {
                int count = opt.Elements.Count;
                if (index < 0 || index >= count)
                    throw new ArgumentOutOfRangeException("index");

                PdfItem item = opt.Elements[index];
                if (item is PdfString)
                    return item.ToString();

                if (item is PdfArray)
                {
                    PdfArray array = (PdfArray)item;
                    if (array.Elements.Count != 0)
                        return !useExportValue && array.Elements.Count > 1 ? array.Elements[1].ToString() : array.Elements[0].ToString();
                }
            }
            return "";
        }

        /// <summary>
        /// Predefined keys of this dictionary. 
        /// The description comes from PDF 1.4 Reference.
        /// </summary>
        public new class Keys : PdfAcroField.Keys
        {
            // ReSharper disable InconsistentNaming

            /// <summary>
            /// (Required; inheritable) An array of options to be presented to the user. Each element of
            /// the array is either a text string representing one of the available options or a two-element
            /// array consisting of a text string together with a default appearance string for constructing
            /// the item’s appearance dynamically at viewing time.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string Opt = "/Opt";

            /// <summary>
            /// (Optional; inheritable) For scrollable list boxes, the top index (the index in the Opt array
            /// of the first option visible in the list).
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string TI = "/TI";

            /// <summary>
            /// (Sometimes required, otherwise optional; inheritable; PDF 1.4) For choice fields that allow
            /// multiple selection (MultiSelect flag set), an array of integers, sorted in ascending order,
            /// representing the zero-based indices in the Opt array of the currently selected option
            /// items. This entry is required when two or more elements in the Opt array have different
            /// names but the same export value, or when the value of the choice field is an array; in
            /// other cases, it is permitted but not required. If the items identified by this entry differ
            /// from those in the V entry of the field dictionary (see below), the V entry takes precedence.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string I = "/I";

            /// <summary>
            /// Gets the KeysMeta for these keys.
            /// </summary>
            internal static DictionaryMeta Meta
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
    }
}
