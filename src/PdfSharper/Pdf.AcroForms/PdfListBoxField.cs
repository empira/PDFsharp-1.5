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

using PdfSharper.Drawing;
using System;
using System.Collections.Generic;

namespace PdfSharper.Pdf.AcroForms
{
	/// <summary>
	/// Represents the list box field.
	/// </summary>
	public sealed class PdfListBoxField : PdfChoiceField
	{
		/// <summary>
		/// Initializes a new instance of PdfListBoxField.
		/// </summary>
		internal PdfListBoxField(PdfDocument document)
			: base(document)
		{ }

		internal PdfListBoxField(PdfDictionary dict)
			: base(dict)
		{ }

		/// <summary>
		/// Gets or sets the background color for selected items of the field.
		/// </summary>
		public XColor HighlightColor
		{
			get { return this.highlightColor; }
			set { this.highlightColor = value; }
		}
		XColor highlightColor = XColors.DarkBlue;

		/// <summary>
		/// Gets or sets the text-color for selected items of the field.
		/// </summary>
		public XColor HighlightTextColor
		{
			get { return this.highlightTextColor; }
			set { this.highlightTextColor = value; }
		}
		XColor highlightTextColor = XColors.White;

		/// <summary>
		/// Gets or sets the value for this field
		/// </summary>
		public override PdfItem Value
		{
			get
			{
				if (SelectedIndices.Count > 0)
				{
					var val = ValueInOptArray(SelectedIndices[0], true);
					if (String.IsNullOrEmpty(val))
						val = ValueInOptArray(SelectedIndices[0], false);
					if (!String.IsNullOrEmpty(val))
						return new PdfString(val);
				}
				return null;
			}
			set
			{
				base.Value = value;
				if (value == null)
					SelectedIndices = new int[0];
				else
				{
					var indices = new List<int>();
					var index = IndexInOptArray(value.ToString(), true);
					if (index >= 0)
						indices.Add(index);
					SelectedIndices = indices.ToArray();
				}
			}
		}

		/// <summary>
		/// Gets or sets the Indices of the selected items of this Field
		/// </summary>
		public IList<int> SelectedIndices
		{
			get
			{
				var result = new List<int>();
				var ary = Elements.GetArray(PdfAcroField.Keys.V);       // /V takes precedence over /I
				if (ary != null)
				{
					for (var i = 0; i < ary.Elements.Count; i++)
					{
						int idx;
						var val = ary.Elements.GetString(i);
						if (val != null && (idx = IndexInOptArray(val, true)) >= 0)
							result.Add(idx);
					}
				}
				if (result.Count > 0)
					return result;

				ary = Elements.GetArray(PdfChoiceField.Keys.I);
				if (ary != null)
				{
					foreach (var item in ary.Elements)
					{
						if (item is PdfInteger)
							result.Add((item as PdfInteger).Value);
					}
				}
				return result;
			}
			set
			{
				var indices = new PdfArray(_document);
				var values = new PdfArray(_document);
				foreach (var index in value)
				{
					indices.Elements.Add(new PdfInteger(index));
					values.Elements.Add(new PdfString(ValueInOptArray(index, true)));
				}
				if (indices.Elements.Count > 0)
				{
					Elements.SetObject(PdfChoiceField.Keys.I, indices);
					Elements.SetObject(PdfAcroField.Keys.V, values);
				}
				else
				{
					Elements.Remove(PdfChoiceField.Keys.I);
					Elements.Remove(PdfAcroField.Keys.V);
				}
			}
		}

		/// <summary>
		/// Gets or sets the index of the first visible item in the ListBox
		/// </summary>
		public int TopIndex
		{
			get { return Elements.GetInteger(PdfChoiceField.Keys.TI); }
			set
			{
				if (value < 0)
					throw new ArgumentException("TopIndex must not be less than zero");
				Elements.SetInteger(PdfChoiceField.Keys.TI, value);
			}
		}

		internal override void Flatten()
		{
			base.Flatten();

			var rect = Rectangle;
			if (!rect.IsEmpty)
			{
				var yOffset = 0.0;
				using (var gfx = XGraphics.FromPdfPage(Page))
				{
					var xRect = new XRect(rect.X1, Page.Height.Point - rect.Y2, rect.Width, rect.Height);
					gfx.Save();
					gfx.IntersectClip(xRect);
					for (var index = TopIndex; index < Values.Count; index++)
					{
						var text = Values[index];
						var size = gfx.MeasureString(text, Font);
						var drawColor = ForeColor;
						if (SelectedIndices.Contains(index))
						{
							gfx.DrawRectangle(new XSolidBrush(HighlightColor), new XRect(rect.X1, Page.Height.Point - rect.Y2 + yOffset + 2.0, rect.Width, size.Height));
							drawColor = HighlightTextColor;
						}
						gfx.DrawString(text, Font, new XSolidBrush(drawColor), xRect + new XPoint(2, 2 + yOffset), XStringFormats.TopLeft);
						yOffset += size.Height + 1.0;
					}
					gfx.Restore();
				}
			}
		}

		/// <summary>
		/// Predefined keys of this dictionary. 
		/// The description comes from PDF 1.4 Reference.
		/// </summary>
		public new class Keys : PdfAcroField.Keys
		{
			// List boxes have no additional entries.

			internal static DictionaryMeta Meta
			{
				get { return _meta ?? (_meta = CreateMeta(typeof(Keys))); }
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
