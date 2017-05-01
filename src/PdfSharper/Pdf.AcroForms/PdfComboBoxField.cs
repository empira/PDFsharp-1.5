﻿#region PDFsharp - A .NET library for processing PDF
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

using PdfSharper.Drawing;
using System;

namespace PdfSharper.Pdf.AcroForms
{
    /// <summary>
    /// Represents the combo box field.
    /// </summary>
    public sealed class PdfComboBoxField : PdfChoiceField
    {
        /// <summary>
        /// Initializes a new instance of PdfComboBoxField.
        /// </summary>
        public PdfComboBoxField(PdfDocument document, bool needsAppearance = false)
            : base(document, needsAppearance)
        { }

        public PdfComboBoxField(PdfDictionary dict)
            : base(dict)
        { }

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                string value = Elements.GetString(Keys.V);
                // try export value first
                var index = IndexInOptArray(value, true);
                if (index < 0)
                    index = IndexInOptArray(value, false);
                return index;
            }
            set
            {
                // xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx		  
                if (value != -1) //R080325
                {
                    string key = ValueInOptArray(value, true);
                    Elements.SetString(Keys.V, key);
                    Elements.SetInteger("/I", value); //R080304 !!!!!!! sonst reagiert die Combobox �berhaupt nicht !!!!!
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of the field.
        /// </summary>
        // xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx		  
        public override PdfItem Value //R080304
        {
            get { return Elements[Keys.V]; }
            set
            {
                if (ReadOnly)
                    throw new InvalidOperationException("The field is read only.");
                if (value is PdfString || value is PdfName)
                {
                    Elements[Keys.V] = value;
                    SelectedIndex = SelectedIndex; //R080304 !!!
                    if (SelectedIndex == -1)
                    {
                        //R080317 noch nicht rund
                        try
                        {
                            //anh�ngen
                            ((PdfArray)(((PdfItem[])(Elements.Values))[2])).Elements.Add(Value);
                            SelectedIndex = SelectedIndex;
                        }
                        catch { }
                    }
                }
                else
                    throw new NotImplementedException("Values other than string cannot be set.");
            }
        }

        internal override void Flatten()
        {
            base.Flatten();

            var index = SelectedIndex;
            if (index >= 0)
            {
                var text = ValueInOptArray(index, false);
                if (text.Length > 0)
                {
                    var rect = Rectangle;
                    if (!rect.IsEmpty)
                    {
                        var xRect = new XRect(rect.X1, Page.Height.Point - rect.Y2, rect.Width, rect.Height);
                        using (var gfx = XGraphics.FromPdfPage(Page))
                        {
                            gfx.Save();
                            gfx.IntersectClip(xRect);
                            // Note: Page origin [0,0] is bottom left !
                            gfx.DrawString(text, Font, new XSolidBrush(ForeColor), xRect + new XPoint(2, 2), XStringFormats.TopLeft);
                            gfx.Restore();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Predefined keys of this dictionary. 
        /// The description comes from PDF 1.4 Reference.
        /// </summary>
        public new class Keys : PdfAcroField.Keys
        {
            // Combo boxes have no additional entries.

            internal static new DictionaryMeta Meta
            {
                get
                {
                    if (Keys._meta == null)
                        Keys._meta = CreateMeta(typeof(Keys));
                    return Keys._meta;
                }
            }
            static DictionaryMeta _meta;
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
