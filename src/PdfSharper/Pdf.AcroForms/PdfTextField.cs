#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
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

using PdfSharper.Drawing;
using PdfSharper.Drawing.Layout;
using PdfSharper.Pdf.Advanced;
using PdfSharper.Pdf.Annotations;
using PdfSharper.Pdf.Internal;
using System;

namespace PdfSharper.Pdf.AcroForms
{
    /// <summary>
    /// Represents the text field.
    /// </summary>
    public sealed class PdfTextField : PdfAcroField
    {
        /// <summary>
        /// Initializes a new instance of PdfTextField.
        /// </summary>
        public PdfTextField(PdfDocument document)
            : base(document)
        {
            Elements.SetName(Keys.FT, PdfAcroFieldTypes.Text);
            Elements.SetString(Keys.TU, string.Empty);
            Elements.SetInteger(Keys.Ff, 0);

            //annotation elements            
            Elements.SetInteger(PdfAnnotation.Keys.F, (int)PdfAnnotationFlags.Print);
            Elements.Add(PdfWidgetAnnotation.Keys.MK, new PdfDictionary(document));
            Elements.SetName(PdfAnnotation.Keys.Subtype, "/Widget");
            Elements.SetName(PdfAnnotation.Keys.Type, "/Annot");

        }

        public PdfTextField(PdfDictionary dict)
            : base(dict)
        { }

        /// <summary>
        /// Gets or sets the text value of the text field.
        /// </summary>
        public string Text
        {
            get { return Elements.GetString(Keys.V); }
            set
            {
                Elements.SetString(Keys.V, value);
                _needsAppearances = true;
            }
        }


        public XStringFormat Alignment
        {
            get
            {
                XStringFormat _alignment;
                if (MultiLine)
                {
                    _alignment = XStringFormats.TopLeft;

                    switch (Elements.GetInteger(Keys.Q))
                    {
                        case 0:
                            _alignment = XStringFormats.TopLeft;
                            break;
                        case 1:
                            _alignment = XStringFormats.TopCenter;
                            break;
                        case 2:
                            _alignment = XStringFormats.TopRight;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    _alignment = XStringFormats.CenterLeft;
                    switch (Elements.GetInteger(Keys.Q))
                    {
                        case 0:
                            _alignment = XStringFormats.CenterLeft;
                            break;
                        case 1:
                            _alignment = XStringFormats.Center;
                            break;
                        case 2:
                            _alignment = XStringFormats.CenterRight;
                            break;
                        default:
                            break;
                    }
                }
                return _alignment;
            }
            set
            {

                if (XStringFormats.Equals(value, XStringFormats.CenterLeft) || XStringFormats.Equals(value, XStringFormats.BottomLeft) || XStringFormats.Equals(value, XStringFormats.TopLeft))
                {
                    Elements.SetInteger(Keys.Q, 0);
                }
                else if (XStringFormats.Equals(value, XStringFormats.Center) || XStringFormats.Equals(value, XStringFormats.TopCenter) || XStringFormats.Equals(value, XStringFormats.BottomCenter))
                {
                    Elements.SetInteger(Keys.Q, 1);
                }
                else if (XStringFormats.Equals(value, XStringFormats.CenterRight) || XStringFormats.Equals(value, XStringFormats.TopRight) || XStringFormats.Equals(value, XStringFormats.BottomRight))
                {
                    Elements.SetInteger(Keys.Q, 2);
                }
            }
        }

        public double TopMargin
        {
            get { return _topMargin; }
            set { _topMargin = value; }
        }
        double _topMargin = 0;

        public double BottomMargin
        {
            get { return _bottomMargin; }
            set { _bottomMargin = value; }
        }
        double _bottomMargin = 0;

        public double LeftMargin
        {
            get { return _leftMargin; }
            set { _leftMargin = value; }
        }
        double _leftMargin = 0;

        public double RightMargin
        {
            get { return _rightMargin; }
            set { _rightMargin = value; }
        }
        double _rightMargin = 0;

        /// <summary>
        /// Gets or sets the maximum length of the field.
        /// </summary>
        /// <value>The length of the max.</value>
        public int MaxLength
        {
            get { return Elements.GetInteger(Keys.MaxLen); }
            set { Elements.SetInteger(Keys.MaxLen, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the field has multiple lines.
        /// </summary>
        public bool MultiLine
        {
            get { return (FieldFlags & PdfAcroFieldFlags.Multiline) != 0; }
            set
            {
                if (value)
                    SetFlags |= PdfAcroFieldFlags.Multiline;
                else
                    SetFlags &= ~PdfAcroFieldFlags.Multiline;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this field is used for passwords.
        /// </summary>
        public bool Password
        {
            get { return (FieldFlags & PdfAcroFieldFlags.Password) != 0; }
            set
            {
                if (value)
                    SetFlags |= PdfAcroFieldFlags.Password;
                else
                    SetFlags &= ~PdfAcroFieldFlags.Password;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this field is a combined field.
        /// A combined field is a text field made up of multiple "combs" of equal width. The number of combs is determined by <see cref="MaxLength"/>.
        /// </summary>
        public bool Combined
        {
            get { return (FieldFlags & PdfAcroFieldFlags.Comb) != 0; }
            set
            {
                if (value)
                    SetFlags |= PdfAcroFieldFlags.Comb;
                else
                    SetFlags &= ~PdfAcroFieldFlags.Comb;
            }
        }

        /// <summary>
        /// Creates the normal appearance form X object for the annotation that represents
        /// this acro form text field.
        /// </summary>
        protected override void RenderAppearance()
        {
#if true_
            PdfFormXObject xobj = new PdfFormXObject(Owner);
            Owner.Internals.AddObject(xobj);
            xobj.Elements["/BBox"] = new PdfLiteral("[0 0 122.653 12.707]");
            xobj.Elements["/FormType"] = new PdfLiteral("1");
            xobj.Elements["/Matrix"] = new PdfLiteral("[1 0 0 1 0 0]");
            PdfDictionary res = new PdfDictionary(Owner);
            xobj.Elements["/Resources"] = res;
            res.Elements["/Font"] = new PdfLiteral("<< /Helv 28 0 R >> /ProcSet [/PDF /Text]");
            xobj.Elements["/Subtype"] = new PdfLiteral("/Form");
            xobj.Elements["/Type"] = new PdfLiteral("/XObject");

            string s =
              "/Tx BMC " + '\n' +
              "q" + '\n' +
              "1 1 120.653 10.707 re" + '\n' +
              "W" + '\n' +
              "n" + '\n' +
              "BT" + '\n' +
              "/Helv 7.93 Tf" + '\n' +
              "0 g" + '\n' +
              "2 3.412 Td" + '\n' +
              "(Hello ) Tj" + '\n' +
              "20.256 0 Td" + '\n' +
              "(XXX) Tj" + '\n' +
              "ET" + '\n' +
              "Q" + '\n' +
              "";//"EMC";
            int length = s.Length;
            byte[] stream = new byte[length];
            for (int idx = 0; idx < length; idx++)
                stream[idx] = (byte)s[idx];
            xobj.CreateStream(stream);

            // Get existing or create new appearance dictionary
            PdfDictionary ap = Elements[PdfAnnotation.Keys.AP] as PdfDictionary;
            if (ap == null)
            {
                ap = new PdfDictionary(_document);
                Elements[PdfAnnotation.Keys.AP] = ap;
            }

            // Set XRef to normal state
            ap.Elements["/N"] = xobj.Reference;




            //// HACK
            //string m =
            //"<?xpacket begin=\"﻿\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?>" + '\n' +
            //"<x:xmpmeta xmlns:x=\"adobe:ns:meta/\" x:xmptk=\"Adobe XMP Core 4.0-c321 44.398116, Tue Aug 04 2009 14:24:39\"> " + '\n' +
            //"   <rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\"> " + '\n' +
            //"      <rdf:Description rdf:about=\"\" " + '\n' +
            //"            xmlns:pdf=\"http://ns.adobe.com/pdf/1.3/\"> " + '\n' +
            //"         <pdf:Producer>PDFsharp 1.40.2150-g (www.PdfSharper.com) (Original: Powered By Crystal)</pdf:Producer> " + '\n' +
            //"      </rdf:Description> " + '\n' +
            //"      <rdf:Description rdf:about=\"\" " + '\n' +
            //"            xmlns:xap=\"http://ns.adobe.com/xap/1.0/\"> " + '\n' +
            //"         <xap:ModifyDate>2011-07-11T23:15:09+02:00</xap:ModifyDate> " + '\n' +
            //"         <xap:CreateDate>2011-05-19T16:26:51+03:00</xap:CreateDate> " + '\n' +
            //"         <xap:MetadataDate>2011-07-11T23:15:09+02:00</xap:MetadataDate> " + '\n' +
            //"         <xap:CreatorTool>Crystal Reports</xap:CreatorTool> " + '\n' +
            //"      </rdf:Description> " + '\n' +
            //"      <rdf:Description rdf:about=\"\" " + '\n' +
            //"            xmlns:dc=\"http://purl.org/dc/elements/1.1/\"> " + '\n' +
            //"         <dc:format>application/pdf</dc:format> " + '\n' +
            //"      </rdf:Description> " + '\n' +
            //"      <rdf:Description rdf:about=\"\" " + '\n' +
            //"            xmlns:xapMM=\"http://ns.adobe.com/xap/1.0/mm/\"> " + '\n' +
            //"         <xapMM:DocumentID>uuid:68249d89-baed-4384-9a2d-fbf8ace75c45</xapMM:DocumentID> " + '\n' +
            //"         <xapMM:InstanceID>uuid:3d5f2f46-c140-416f-baf2-7f9c970cef1d</xapMM:InstanceID> " + '\n' +
            //"      </rdf:Description> " + '\n' +
            //"   </rdf:RDF> " + '\n' +
            //"</x:xmpmeta> " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"<?xpacket end=\"w\"?>";

            //PdfDictionary mdict = (PdfDictionary)_document.Internals.GetObject(new PdfObjectID(32));

            //length = m.Length;
            //stream = new byte[length];
            //for (int idx = 0; idx < length; idx++)
            //  stream[idx] = (byte)m[idx];

            //mdict.Stream.Value = stream;




#else
            PdfRectangle rect = Elements.GetRectangle(PdfAnnotation.Keys.Rect);
            XForm form = new XForm(_document, rect.Size);
            XGraphics gfx = XGraphics.FromForm(form);
            XRect xrect = (rect.ToXRect() - rect.Location);

            if (BackColor != XColor.Empty)
                gfx.DrawRectangle(new XSolidBrush(BackColor), rect.ToXRect() - rect.Location);
            // Draw Border
            if (!BorderColor.IsEmpty)
                gfx.DrawRectangle(new XPen(BorderColor), rect.ToXRect() - rect.Location);

            string text = Text;

            if (text.Length > 0)
            {
                xrect.Y = xrect.Y + TopMargin;
                xrect.X = xrect.X + LeftMargin;
                xrect.Width = xrect.Width + RightMargin;
                xrect.Height = xrect.Height + BottomMargin;

                if ((FieldFlags & PdfAcroFieldFlags.Comb) != 0 && MaxLength > 0)
                {
                    var combWidth = xrect.Width / MaxLength;
                    var format = XStringFormats.TopLeft;
                    format.Comb = true;
                    format.CombWidth = combWidth;
                    gfx.Save();
                    gfx.IntersectClip(xrect);
                    if (this.MultiLine)
                    {
                        XTextFormatter formatter = new XTextFormatter(gfx);
                        formatter.Text = text;

                        formatter.DrawString(Text, Font, new XSolidBrush(ForeColor), xrect, Alignment);
                    }
                    else
                    {
                        gfx.DrawString(text, Font, new XSolidBrush(ForeColor), xrect + new XPoint(0, 1.5), format);
                    }
                    gfx.Restore();
                }
                else
                {
                    XTextFormatter formatter = new XTextFormatter(gfx);
                    formatter.Text = text;

                    formatter.DrawString(text, Font, new XSolidBrush(ForeColor), rect.ToXRect() - rect.Location, Alignment);
                }
            }

            form.DrawingFinished();
            form.PdfForm.Elements.Add("/FormType", new PdfLiteral("1"));

            // Get existing or create new appearance dictionary.
            PdfDictionary ap = Elements[PdfAnnotation.Keys.AP] as PdfDictionary;
            if (ap == null)
            {
                ap = new PdfDictionary(_document);
                Elements[PdfAnnotation.Keys.AP] = ap;
            }

            // Set XRef to normal state
            ap.Elements["/N"] = PdfObject.DeepCopyClosure(Owner, form.PdfForm);

            var normalStateDict = ap.Elements.GetDictionary("/N");
            var resourceDict = new PdfDictionary(Owner);
            resourceDict.Elements[PdfResources.Keys.ProcSet] = new PdfArray(Owner, new PdfName("/PDF"), new PdfName("/Text"));

            var defaultFormResources = Owner.AcroForm.Elements.GetDictionary(PdfAcroForm.Keys.DR);
            if (defaultFormResources != null && defaultFormResources.Elements.ContainsKey(PdfResources.Keys.Font))
            {
                var fontResourceItem = XForm.GetFontResourceItem(Font.FamilyName, defaultFormResources);
                PdfDictionary fontDict = new PdfDictionary(Owner);
                resourceDict.Elements[PdfResources.Keys.Font] = fontDict;
                fontDict.Elements[fontResourceItem.Key] = fontResourceItem.Value;
            }

            normalStateDict.Elements.SetObject(PdfPage.Keys.Resources, resourceDict);

            PdfFormXObject xobj = form.PdfForm;
            if (xobj.Stream == null)
                xobj.CreateStream(new byte[] { });

            string s = xobj.Stream.ToString();
            // Thank you Adobe: Without putting the content in 'EMC brackets'
            // the text is not rendered by PDF Reader 9 or higher.
            s = "/Tx BMC\n" + s + "\nEMC";
            ap.Elements.GetDictionary("/N").Stream.Value = new RawEncoding().GetBytes(s);
#endif
        }


        internal override void Flatten()
        {
            base.Flatten();

            if (HasKids)
            {
                for (int i = 0; i < Fields.Elements.Count; i++)
                {
                    var rect = Fields[i].Rectangle;
                    var page = Fields[i].Page;
                    var font = GetFontFromElement(Fields[i]);
                    XStringFormat format = GetAlignment(Fields[i].Elements);
                    DrawToPDF(rect, page, font, format);
                }
            }
            else
            {
                var rect = Rectangle;
                var page = Page;
                XStringFormat format = GetAlignment(Elements);
                DrawToPDF(rect, page, Font, format);
            }
        }

        internal void DrawToPDF(PdfRectangle rect, PdfPage elementPage, XFont font, XStringFormat format)
        {
            if (!rect.IsEmpty)
            {
                using (var gfx = XGraphics.FromPdfPage(elementPage))
                {
                    // Note: Page origin [0,0] is bottom left !
                    var text = Text;
                    if (text.Length > 0)
                    {
                        var xRect = new XRect(rect.X1, elementPage.Height.Point - rect.Y2, rect.Width, rect.Height);
                        if ((FieldFlags & PdfAcroFieldFlags.Comb) != 0 && MaxLength > 0)
                        {
                            var combWidth = xRect.Width / MaxLength;
                            format.Comb = true;
                            format.CombWidth = combWidth;
                            gfx.Save();
                            gfx.IntersectClip(xRect);
                            gfx.DrawString(text, font, new XSolidBrush(ForeColor), xRect + new XPoint(0, 1.5), format);
                            gfx.Restore();
                        }
                        else
                        {
                            gfx.Save();
                            gfx.IntersectClip(xRect);
                            gfx.DrawString(text, font, new XSolidBrush(ForeColor), xRect + new XPoint(2, 2), format);
                            gfx.Restore();
                        }
                    }
                }
            }
        }

        internal XFont GetFontFromElement(PdfAcroField element)
        {
            string[] name = element.Font.FamilyName.Split(',');
            double size = element.Font.Size;
            XFontStyle style;

            if (name.Length > 1)
            {
                switch (name[1])
                {
                    case "Bold":
                        style = XFontStyle.Bold;
                        break;
                    case "Italic":
                        style = XFontStyle.Italic;
                        break;
                    case "BoldItalic":
                        style = XFontStyle.BoldItalic;
                        break;
                    default:
                        style = XFontStyle.Regular;
                        break;
                }
            }
            else
            {
                style = XFontStyle.Regular;
            }

            return new XFont(name[0], size, style);
        }

        internal XStringFormat GetAlignment(DictionaryElements dict)
        {
            PdfItem item = dict.GetValue("/Q");
            if (item != null)
            {
                int alignment = Int32.Parse(item.ToString());

                switch (alignment)
                {
                    case 0:
                        return XStringFormats.TopLeft;
                    case 1:
                        return XStringFormats.TopCenter;
                    case 2:
                        return XStringFormats.TopRight;
                    default:
                        return XStringFormats.TopLeft;
                }
            }
            else
            {
                return XStringFormats.TopLeft;
            }
        }

        /// <summary>
        /// Predefined keys of this dictionary. 
        /// The description comes from PDF 1.4 Reference.
        /// </summary>
        public new class Keys : PdfAcroField.Keys
        {
            /// <summary>
            /// (Optional; inheritable) The maximum length of the field�s text, in characters.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string MaxLen = "/MaxLen";

            /// <summary>
            /// Gets the KeysMeta for these keys.
            /// </summary>
            internal static new DictionaryMeta Meta
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
