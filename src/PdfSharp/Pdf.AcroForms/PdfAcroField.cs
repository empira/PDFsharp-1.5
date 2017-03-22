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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Drawing;
using PdfSharp.Pdf.Annotations;
using PdfSharp.Pdf.Content;
using System.IO;
using PdfSharp.Pdf.Internal;

namespace PdfSharp.Pdf.AcroForms
{
    /// <summary>
    /// Represents the base class for all interactive field dictionaries.
    /// </summary>
    public abstract class PdfAcroField : PdfDictionary
    {
        /// <summary>
        /// Initializes a new instance of PdfAcroField.
        /// </summary>
        internal PdfAcroField(PdfDocument document)
            : base(document)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAcroField"/> class. Used for type transformation.
        /// </summary>
        protected PdfAcroField(PdfDictionary dict)
            : base(dict)
        {
            DetermineAppearance();
        }

        /// <summary>
        /// Gets the name of this field.
        /// </summary>
        public string Name
        {
            get
            {
                string name = Elements.GetString(Keys.T);
                return name;
            }
        }

        /// <summary>
        /// Gets the alternative Name of the Field (/TU)
        /// </summary>
        public string AlternateName
        {
            get { return Elements.GetString(Keys.TU); }
        }

        /// <summary>
        /// Gets the mapping Name of the Field (/TM)
        /// </summary>
        public string MappingName
        {
            get { return Elements.GetString(Keys.TM); }
        }

        /// <summary>
        /// Gets the Parent of this field or null, if the field has no parent
        /// </summary>
        public PdfAcroField Parent
        {
            get
            {
                if (_parent == null)
                {
                    var parentRef = Elements.GetReference(Keys.Parent);
                    if (parentRef != null)
                        _parent = PdfAcroFieldCollection.CreateAcroField(parentRef.Value as PdfDictionary);
                }
                return _parent;
            }
        }
        private PdfAcroField _parent;

        /// <summary>
        /// Gets the field flags of this instance.
        /// </summary>
        public PdfAcroFieldFlags Flags
        {
            // TODO: This entry is inheritable, thus the implementation is incorrect...
            get { return (PdfAcroFieldFlags)Elements.GetInteger(Keys.Ff); }
        }

        internal PdfAcroFieldFlags SetFlags
        {
            get { return (PdfAcroFieldFlags)Elements.GetInteger(Keys.Ff); }
            set { Elements.SetInteger(Keys.Ff, (int)value); }
        }

        /// <summary>
        /// Gets or sets the font used to draw the text of the field.
        /// </summary>
        public XFont Font
        {
            get { return this.font; }
            set { this.font = value; }
        }
        XFont font = new XFont("Arial", 10);

        /// <summary>
        /// Gets the font name that was obtained by analyzing the Fields' content-stream.
        /// </summary>
        public string ContentFontName { get; private set; }

        /// <summary>
        /// Gets the base font name that was obtained by analyzing the Fields' content-stream.
        /// </summary>
        public string BaseContentFontName { get; private set; }

        /// <summary>
        /// Gets or sets the foreground color of the field.
        /// </summary>
        public XColor ForeColor
        {
            get { return this.foreColor; }
            set { this.foreColor = value; }
        }
        XColor foreColor = XColors.Black;

        /// <summary>
        /// Gets or sets the background color of the field.
        /// </summary>
        public XColor BackColor
        {
            get { return this.backColor; }
            set { this.backColor = value; }
        }
        XColor backColor = XColor.Empty;

        /// <summary>
        /// Gets or sets the border color of the field.
        /// </summary>
        public XColor BorderColor
        {
            get { return this.borderColor; }
            set { this.borderColor = value; }
        }
        XColor borderColor = XColor.Empty;

        /// <summary>
        /// Gets or sets the value of the field.
        /// </summary>
        public virtual PdfItem Value
        {
            get { return Elements[Keys.V]; }
            set
            {
                if (ReadOnly)
                    throw new InvalidOperationException("The field is read only.");
                if (value is PdfString || value is PdfName)
                    Elements[Keys.V] = value;
                else
                    throw new NotImplementedException("Values other than string cannot be set.");
            }
        }

        /// <summary>
        /// Gets or sets the default value of the field.
        /// </summary>
        public virtual PdfItem DefaultValue
        {
            get { return Elements[Keys.DV]; }
            set { Elements[Keys.DV] = value; }
        }

        /// <summary>
        /// Gets the Rectangle of the field
        /// </summary>
        public PdfRectangle Rectangle
        {
            get { return Elements.GetRectangle(PdfAnnotation.Keys.Rect); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the field is read only.
        /// </summary>
        public bool ReadOnly
        {
            get { return (Flags & PdfAcroFieldFlags.ReadOnly) != 0; }
            set
            {
                if (value)
                    SetFlags |= PdfAcroFieldFlags.ReadOnly;
                else
                    SetFlags &= ~PdfAcroFieldFlags.ReadOnly;
            }
        }

        /// <summary>
        /// Gets the field with the specified name.
        /// </summary>
        public PdfAcroField this[string name]
        {
            get { return GetValue(name); }
        }

        /// <summary>
        /// Gets a child field by name.
        /// </summary>
        protected virtual PdfAcroField GetValue(string name)
        {
            if (String.IsNullOrEmpty(name))
                return this;
            if (HasKids)
                return Fields.GetValue(name);
            return null;
        }

        /// <summary>
        /// Indicates whether the field has child fields.
        /// </summary>
        public bool HasKids
        {
            get
            {
                PdfItem item = Elements[Keys.Kids];
                if (item == null)
                    return false;
                if (item is PdfArray)
                    return ((PdfArray)item).Elements.Count > 0;
                return false;
            }
        }

        /// <summary>
        /// Gets the names of all descendants of this field.
        /// </summary>
        [Obsolete("Use GetDescendantNames")]
        public string[] DescendantNames  // Properties should not return arrays.
        {
            get { return GetDescendantNames(); }
        }

        /// <summary>
        /// Gets the names of all descendants of this field.
        /// </summary>
        public string[] GetDescendantNames()
        {
            List<string> names = new List<string>();
            if (HasKids)
            {
                PdfAcroFieldCollection fields = Fields;
                fields.GetDescendantNames(ref names, null);
            }
            List<string> temp = new List<string>();
            foreach (string name in names)
                temp.Add(name);
            return temp.ToArray();
        }

        /// <summary>
        /// Gets the names of all appearance dictionaries of this AcroField.
        /// </summary>
        public string[] GetAppearanceNames()
        {
            Dictionary<string, object> names = new Dictionary<string, object>();
            PdfDictionary dict = Elements["/AP"] as PdfDictionary;
            if (dict != null)
            {
                AppDict(dict, names);

                if (HasKids)
                {
                    PdfItem[] kids = Fields.Elements.Items;
                    foreach (PdfItem pdfItem in kids)
                    {
                        if (pdfItem is PdfReference)
                        {
                            PdfDictionary xxx = ((PdfReference)pdfItem).Value as PdfDictionary;
                            if (xxx != null)
                                AppDict(xxx, names);
                        }
                    }
                    //((PdfDictionary)(((PdfReference)(Fields.Elements.Items[1])).Value)).Elements.SetName(Keys.V, name1);

                }
            }
            string[] array = new string[names.Count];
            names.Keys.CopyTo(array, 0);
            return array;
        }

        //static string[] AppearanceNames(PdfDictionary dictIn)
        //{
        //  Dictionary<string, object> names = new Dictionary<string, object>();
        //  PdfDictionary dict = dictIn["/AP"] as PdfDictionary;
        //  if (dict != null)
        //  {
        //    AppDict(dict, names);

        //    if (HasKids)
        //    {
        //      PdfItem[] kids = Fields.Elements.Items;
        //      foreach (PdfItem pdfItem in kids)
        //      {
        //        if (pdfItem is PdfReference)
        //        {
        //          PdfDictionary xxx = ((PdfReference)pdfItem).Value as PdfDictionary;
        //          if (xxx != null)
        //            AppDict(xxx, names);
        //        }
        //      }
        //      //((PdfDictionary)(((PdfReference)(Fields.Elements.Items[1])).Value)).Elements.SetName(Keys.V, name1);

        //    }
        //  }
        //  string[] array = new string[names.Count];
        //  names.Keys.CopyTo(array, 0);
        //  return array;
        //}

        static void AppDict(PdfDictionary dict, Dictionary<string, object> names)
        {
            PdfDictionary sub;
            if ((sub = dict.Elements["/D"] as PdfDictionary) != null)
                AppDict2(sub, names);
            if ((sub = dict.Elements["/N"] as PdfDictionary) != null)
                AppDict2(sub, names);
        }

        static void AppDict2(PdfDictionary dict, Dictionary<string, object> names)
        {
            foreach (string key in dict.Elements.Keys)
            {
                if (!names.ContainsKey(key))
                    names.Add(key, null);
            }
        }

        internal virtual void GetDescendantNames(ref List<string> names, string partialName)
        {
            if (HasKids)
            {
                PdfAcroFieldCollection fields = Fields;
                string t = Elements.GetString(Keys.T);
                Debug.Assert(t != "");
                if (t.Length > 0)
                {
                    if (!String.IsNullOrEmpty(partialName))
                        partialName += "." + t;
                    else
                        partialName = t;
                    fields.GetDescendantNames(ref names, partialName);
                }
            }
            else
            {
                string t = Elements.GetString(Keys.T);
                Debug.Assert(t != "");
                if (t.Length > 0)
                {
                    if (!String.IsNullOrEmpty(partialName))
                        names.Add(partialName + "." + t);
                    else
                        names.Add(t);
                }
            }
        }

        /// <summary>
        /// Gets the collection of fields within this field.
        /// </summary>
        public PdfAcroFieldCollection Fields
        {
            get
            {
                if (_fields == null)
                {
                    object o = Elements.GetValue(Keys.Kids, VCF.CreateIndirect);
                    _fields = (PdfAcroFieldCollection)o;
                }
                return _fields;
            }
        }
        PdfAcroFieldCollection _fields;

        /// <summary>
        /// Gets a reference to the Page object this field belongs to
        /// </summary>
        public PdfReference PageReference
        {
            get
            {
                if (pageReference == null)
                    DeterminePage();
                return pageReference;
            }
        }
        private PdfReference pageReference;

        /// <summary>
        /// Gets the Page this Field is a member of
        /// </summary>
        public PdfPage Page
        {
            get { return PageReference != null ? (PdfPage)PageReference.Value : null; }
        }

        /// <summary>
        /// Tries to find the page reference object for this field
        /// </summary>
        protected internal void DeterminePage()
        {
            if (pageReference == null)
            {
                var pageRef = Elements.GetReference(Keys.Page);
                if (pageRef == null)
                {
                    var curField = Parent;
                    // first scan up in the hierarchy
                    while (curField != null && pageRef == null)
                    {
                        pageRef = curField.Elements.GetReference(Keys.Page);
                        if (pageRef == null)
                            curField = curField.Parent;
                    }
                    if (pageRef == null)
                    {
                        curField = this;
                        // now scan down the hierarchy
                        for (var i = 0; i < curField.Fields.Names.Length; i++)
                        {
                            curField = curField.Fields[i];
                            pageRef = FindPageRefInChilds(curField);
                            if (pageRef != null)
                                break;
                        }
                    }
                }
                if (pageRef != null)
                {
                    for (var i = 0; i < _document.PageCount; i++)
                    {
                        var page = _document.Pages[i];
                        if (page.ObjectID == pageRef.ObjectID)
                        {
                            pageRef = page.Reference;
                            break;
                        }
                    }
                }
                pageReference = pageRef;
            }
        }

        private PdfReference FindPageRefInChilds(PdfAcroField startField)
        {
            var pageRef = startField.Elements.GetReference(Keys.Page);
            if (pageRef != null)
                return pageRef;
            for (var i = 0; i < startField.Fields.Names.Length; i++)
            {
                var child = startField.Fields[i];
                pageRef = child.Elements.GetReference(Keys.Page);
                if (pageRef != null)
                    return pageRef;
                pageRef = FindPageRefInChilds(child);
                if (pageRef != null)
                    return pageRef;
            }
            return null;
        }

        /// <summary>
        /// Tries to determine the Appearance of the Field by checking elements of its dictionary
        /// </summary>
        protected internal void DetermineAppearance()
        {
            string da = null;
            var field = this;
            try
            {
                while (da == null && field != null)
                {
                    da = field.Elements.GetString(Keys.DA);
                    if (String.IsNullOrEmpty(da))
                        da = null;
                    var mk = field.Elements.GetDictionary(PdfWidgetAnnotation.Keys.MK);
                    if (mk != null)
                    {
                        var bc = mk.Elements.GetArray("/BC");
                        if (bc == null || bc.Elements.Count == 0)
                            BorderColor = XColor.Empty;
                        else if (bc.Elements.Count == 3)
                            BorderColor = XColor.FromArgb((int)(bc.Elements.GetReal(0) * 255.0), (int)(bc.Elements.GetReal(1) * 255.0), (int)(bc.Elements.GetReal(2) * 255.0));
                        else if (bc.Elements.Count == 4)
                            BorderColor = XColor.FromCmyk(bc.Elements.GetReal(0), bc.Elements.GetReal(1), bc.Elements.GetReal(2), bc.Elements.GetReal(3));

                        var bg = mk.Elements.GetArray("/BG");
                        if (bg == null || bg.Elements.Count == 0)
                            BackColor = XColor.Empty;
                        else if (bg.Elements.Count == 3)
                            BackColor = XColor.FromArgb((int)(bg.Elements.GetReal(0) * 255.0), (int)(bg.Elements.GetReal(1) * 255.0), (int)(bg.Elements.GetReal(2) * 255.0));
                        else if (bg.Elements.Count == 4)
                            BackColor = XColor.FromCmyk(bg.Elements.GetReal(0), bg.Elements.GetReal(1), bg.Elements.GetReal(2), bg.Elements.GetReal(3));
                    }
                    field = field.Parent;
                }
                if (da == null)
                    return;
                string fontName = null;
                double fontSize = 0.0;
                var content = ContentReader.ReadContent(PdfEncoders.RawEncoding.GetBytes(da));
                for (var i = 0; i < content.Count; i++)
                {
                    var op = content[i] as COperator;
                    if (op != null)
                    {
                        switch (op.OpCode.OpCodeName)
                        {
                            case OpCodeName.Tf:
                                fontName = op.Operands[0].ToString();
                                fontSize = Double.Parse(op.Operands[1].ToString());
                                if (fontSize < 0.1)
                                    fontSize = Rectangle.Height * 0.8;  // TODO: don't know how to determine correct size...
                                break;
                                // TODO: Determine text-color (possibly OpCodeName.rg or RG but not sure)
                        }
                    }
                }
                if (!String.IsNullOrEmpty(fontName) && fontSize > 0.0)
                {
                    ContentFontName = fontName.Substring(1);    // e.g. "/Helv"
                    var resources = _document.AcroForm.Elements.GetDictionary(PdfAcroForm.Keys.DR);
                    if (resources != null && resources.Elements.ContainsKey("/Font"))
                    {
                        var fontList = resources.Elements.GetDictionary("/Font");
                        var fontRef = fontList.Elements.GetReference(fontName);
                        if (fontRef != null)
                        {
                            var fontDict = fontRef.Value as PdfDictionary;
                            if (fontDict != null && fontDict.Elements.ContainsKey("/BaseFont"))
                            {
                                var baseName = fontDict.Elements.GetString("/BaseFont");
                                if (!String.IsNullOrEmpty(baseName))
                                    fontName = baseName;        // e.g. "/Helvetica"
                            }
                        }
                    }
                    BaseContentFontName = fontName.Substring(1);
                    font = new XFont(BaseContentFontName, fontSize);
                }
            }
            catch
            {
                font = new XFont("Arial", 10);
            }
        }

        internal virtual void Flatten()
        {
            // Copy Font-Resources to the Page
            // This is neccessary, because Fonts used by AcroFields may be referenced only by the AcroForm, which is deleted after flattening
            if (Page != null)
            {
                var resources = _document.AcroForm.Elements.GetDictionary(PdfAcroForm.Keys.DR);
                if (!String.IsNullOrEmpty(ContentFontName) && resources != null && resources.Elements.ContainsKey(PdfResources.Keys.Font))
                {
                    var fontKey = "/" + ContentFontName;
                    var fontList = resources.Elements.GetDictionary(PdfResources.Keys.Font);
                    var fontRef = fontList.Elements.GetReference(fontKey);
                    if (fontRef != null)
                    {
                        if (!Page.Resources.Elements.ContainsKey(PdfResources.Keys.Font))
                        {
                            Page.Resources.Elements.Add(PdfResources.Keys.Font, new PdfDictionary());
                        }
                        var fontDict = Page.Resources.Elements.GetDictionary(PdfResources.Keys.Font);
                        if (fontDict != null && !fontDict.Elements.ContainsKey(fontKey))
                            fontDict.Elements.Add(fontKey, fontRef);
                    }
                }

                var rect = Rectangle;
                if (!rect.IsEmpty && (!BackColor.IsEmpty || !BorderColor.IsEmpty))
                {
                    using (var gfx = XGraphics.FromPdfPage(Page))
                    {
                        gfx.TranslateTransform(rect.X1, Page.Height.Point - rect.Y2);
                        if (BackColor != XColor.Empty)
                            gfx.DrawRectangle(new XSolidBrush(BackColor), rect.ToXRect() - rect.Location);
                        // Draw Border
                        if (!BorderColor.IsEmpty)
                            gfx.DrawRectangle(new XPen(BorderColor), rect.ToXRect() - rect.Location);
                    }
                }

                // Remove Field-Annotations from page
                for (var i = 0; i < Page.Annotations.Elements.Count; i++)
                {
                    var item = Page.Annotations.Elements[i] as PdfReference;
                    if (item == Reference)
                    {
                        Page.Annotations.Elements.RemoveAt(i);
                        break;
                    }
                }
            }
            for (var i = 0; i < Fields.Elements.Count; i++)
            {
                var field = Fields[i];
                field.Flatten();
            }

            if (Reference != null)
                _document._irefTable.Remove(Reference);
        }

        /// <summary>
        /// Renders the contents of the supplied Stream to the Page at the position specified by the Field-Rectangle
        /// </summary>
        /// <param name="stream"></param>
        protected virtual void RenderContentStream(PdfStream stream)
        {
            RenderContentStream(stream, Rectangle);
        }

        /// <summary>
        /// Renders the contents of the supplied Stream to the Page at the position specified by the provided Rectangle
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="rect"></param>
        protected virtual void RenderContentStream(PdfStream stream, PdfRectangle rect)
        {
            if (stream == null)
                return;
            var content = ContentReader.ReadContent(stream.UnfilteredValue);
            var matrix = new XMatrix();
            matrix.TranslateAppend(rect.X1, rect.Y1);
            var matElements = matrix.GetElements();
            var matrixOp = OpCodes.OperatorFromName("cm");
            foreach (var el in matElements)
                matrixOp.Operands.Add(new CReal { Value = el });
            content.Insert(0, matrixOp);

            // Save and restore Graphics state
            content.Insert(0, OpCodes.OperatorFromName("q"));
            content.Add(OpCodes.OperatorFromName("Q"));
            var appendedContent = Page.Contents.AppendContent();
            using (var ms = new MemoryStream())
            {
                var cw = new ContentWriter(ms);
                foreach (var obj in content)
                    obj.WriteObject(cw);
                appendedContent.CreateStream(ms.ToArray());
            }
        }

        /// <summary>
        /// Holds a collection of interactive fields.
        /// </summary>
        public sealed class PdfAcroFieldCollection : PdfArray
        {
            PdfAcroFieldCollection(PdfDocument document)
                : base(document)
            { }

            PdfAcroFieldCollection(PdfArray array)
                : base(array)
            { }

            /// <summary>
            /// Gets the number of elements in the array.
            /// </summary>
            public int Count
            {
                get
                {
                    return Elements.Count;
                }
            }

            /// <summary>
            /// Gets the names of all fields in the collection.
            /// </summary>
            public string[] Names
            {
                get
                {
                    int count = Elements.Count;
                    string[] names = new string[count];
                    for (int idx = 0; idx < count; idx++)
                        names[idx] = ((PdfDictionary)((PdfReference)Elements[idx]).Value).Elements.GetString(Keys.T);
                    return names;
                }
            }

            /// <summary>
            /// Gets an array of all descendant names.
            /// </summary>
            public string[] DescendantNames
            {
                get
                {
                    List<string> names = new List<string>();
                    GetDescendantNames(ref names, null);
                    //List<string> temp = new List<string>();
                    //foreach (PdfName name in names)
                    //  temp.Add(name.ToString());
                    return names.ToArray();
                }
            }

            internal void GetDescendantNames(ref List<string> names, string partialName)
            {
                int count = Elements.Count;
                for (int idx = 0; idx < count; idx++)
                {
                    PdfAcroField field = this[idx];
                    if (field != null)
                        field.GetDescendantNames(ref names, partialName);
                }
            }

            /// <summary>
            /// Gets a field from the collection. For your convenience an instance of a derived class like
            /// PdfTextField or PdfCheckBox is returned if PDFsharp can guess the actual type of the dictionary.
            /// If the actual type cannot be guessed by PDFsharp the function returns an instance
            /// of PdfGenericField.
            /// </summary>
            public PdfAcroField this[int index]
            {
                get
                {
                    PdfItem item = Elements[index];
                    Debug.Assert(item is PdfReference);
                    PdfDictionary dict = ((PdfReference)item).Value as PdfDictionary;
                    Debug.Assert(dict != null);
                    PdfAcroField field = dict as PdfAcroField;
                    if (field == null && dict != null)
                    {
                        // Do type transformation
                        field = CreateAcroField(dict);
                        //Elements[index] = field.XRef;
                    }
                    return field;
                }
            }

            /// <summary>
            /// Gets the field with the specified name.
            /// </summary>
            public PdfAcroField this[string name]
            {
                get { return GetValue(name); }
            }

            internal PdfAcroField GetValue(string name)
            {
                if (String.IsNullOrEmpty(name))
                    return null;

                int dot = name.IndexOf('.');
                string prefix = dot == -1 ? name : name.Substring(0, dot);
                string suffix = dot == -1 ? "" : name.Substring(dot + 1);

                int count = Elements.Count;
                for (int idx = 0; idx < count; idx++)
                {
                    PdfAcroField field = this[idx];
                    if (field.Name == prefix)
                        return field.GetValue(suffix);
                }
                return null;
            }

            /// <summary>
            /// Create a derived type like PdfTextField or PdfCheckBox if possible.
            /// If the actual cannot be guessed by PDFsharp the function returns an instance
            /// of PdfGenericField.
            /// </summary>
            internal static PdfAcroField CreateAcroField(PdfDictionary dict)
            {
                string ft = dict.Elements.GetName(Keys.FT);
                PdfAcroFieldFlags flags = (PdfAcroFieldFlags)dict.Elements.GetInteger(Keys.Ff);
                switch (ft)
                {
                    case "/Btn":
                        if ((flags & PdfAcroFieldFlags.Pushbutton) != 0)
                            return new PdfPushButtonField(dict);

                        if ((flags & PdfAcroFieldFlags.Radio) != 0)
                            return new PdfRadioButtonField(dict);

                        return new PdfCheckBoxField(dict);

                    case "/Tx":
                        return new PdfTextField(dict);

                    case "/Ch":
                        if ((flags & PdfAcroFieldFlags.Combo) != 0)
                            return new PdfComboBoxField(dict);
                        else
                            return new PdfListBoxField(dict);

                    case "/Sig":
                        return new PdfSignatureField(dict);

                    default:
                        return new PdfGenericField(dict);
                }
            }
        }

        /// <summary>
        /// Predefined keys of this dictionary. 
        /// The description comes from PDF 1.4 Reference.
        /// </summary>
        public class Keys : KeysBase
        {
            // ReSharper disable InconsistentNaming

            /// <summary>
            /// (Required for terminal fields; inheritable) The type of field that this dictionary
            /// describes:
            ///   Btn           Button
            ///   Tx            Text
            ///   Ch            Choice
            ///   Sig (PDF 1.3) Signature
            /// Note: This entry may be present in a nonterminal field (one whose descendants
            /// are themselves fields) in order to provide an inheritable FT value. However, a
            /// nonterminal field does not logically have a type of its own; it is merely a container
            /// for inheritable attributes that are intended for descendant terminal fields of
            /// any type.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required)]
            public const string FT = "/FT";

            /// <summary>
            /// (Required if this field is the child of another in the field hierarchy; absent otherwise)
            /// The field that is the immediate parent of this one (the field, if any, whose Kids array
            /// includes this field). A field can have at most one parent; that is, it can be included
            /// in the Kids array of at most one other field.
            /// </summary>
            [KeyInfo(KeyType.Dictionary)]
            public const string Parent = "/Parent";

            /// <summary>
            /// (Optional) An array of indirect references to the immediate children of this field.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional, typeof(PdfAcroFieldCollection))]
            public const string Kids = "/Kids";

            /// <summary>
            /// (Optional) The partial field name.
            /// </summary>
            [KeyInfo(KeyType.TextString | KeyType.Optional)]
            public const string T = "/T";

            /// <summary>
            /// (Optional; PDF 1.3) An alternate field name, to be used in place of the actual
            /// field name wherever the field must be identified in the user interface (such as
            /// in error or status messages referring to the field). This text is also useful
            /// when extracting the document’s contents in support of accessibility to disabled
            /// users or for other purposes.
            /// </summary>
            [KeyInfo(KeyType.TextString | KeyType.Optional)]
            public const string TU = "/TU";

            /// <summary>
            /// (Optional; PDF 1.3) The mapping name to be used when exporting interactive form field 
            /// data from the document.
            /// </summary>
            [KeyInfo(KeyType.TextString | KeyType.Optional)]
            public const string TM = "/TM";

            /// <summary>
            /// (Optional; inheritable) A set of flags specifying various characteristics of the field.
            /// Default value: 0.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string Ff = "/Ff";

            /// <summary>
            /// (Optional; inheritable) The field’s value, whose format varies depending on
            /// the field type; see the descriptions of individual field types for further information.
            /// </summary>
            [KeyInfo(KeyType.Various | KeyType.Optional)]
            public const string V = "/V";

            /// <summary>
            /// (Optional; inheritable) The default value to which the field reverts when a
            /// reset-form action is executed. The format of this value is the same as that of V.
            /// </summary>
            [KeyInfo(KeyType.Various | KeyType.Optional)]
            public const string DV = "/DV";

            /// <summary>
            /// (Optional; PDF 1.2) An additional-actions dictionary defining the field’s behavior
            /// in response to various trigger events. This entry has exactly the same meaning as
            /// the AA entry in an annotation dictionary.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string AA = "/AA";

            // ----- Additional entries to all fields containing variable text --------------------------

            /// <summary>
            /// (Required; inheritable) A resource dictionary containing default resources
            /// (such as fonts, patterns, or color spaces) to be used by the appearance stream.
            /// At a minimum, this dictionary must contain a Font entry specifying the resource
            /// name and font dictionary of the default font for displaying the field’s text.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Required)]
            public const string DR = "/DR";

            /// <summary>
            /// (Required; inheritable) The default appearance string, containing a sequence of
            /// valid page-content graphics or text state operators defining such properties as
            /// the field’s text size and color.
            /// </summary>
            [KeyInfo(KeyType.String | KeyType.Required)]
            public const string DA = "/DA";

            /// <summary>
            /// (Optional; inheritable) A code specifying the form of quadding (justification)
            /// to be used in displaying the text:
            ///   0 Left-justified
            ///   1 Centered
            ///   2 Right-justified
            /// Default value: 0 (left-justified).
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string Q = "/Q";

            /// <summary>
            /// Optional: Reference to the Page object containing this field
            /// </summary>
            [KeyInfo(KeyType = KeyType.Optional)]
            public const string Page = "/P";

            // ReSharper restore InconsistentNaming
        }
    }
}
