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

// Review: Under construction - StL/14-10-05

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using PdfSharp.Drawing;
using PdfSharp.Pdf.Actions;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Internal;

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Represents an outline item in the outlines tree. An 'outline' is also known as a 'bookmark'.
    /// </summary>
    public sealed class PdfOutline : PdfDictionary
    {
        // Reference: 8.2.2��Document Outline / Page 584

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfOutline"/> class.
        /// </summary>
        public PdfOutline()
        {
            // Create _outlines on demand.
            //_outlines = new PdfOutlineCollection(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfOutline"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        internal PdfOutline(PdfDocument document)
            : base(document)
        {
            // Create _outlines on demand.
            //_outlines = new PdfOutlineCollection(this);
        }

        /// <summary>
        /// Initializes a new instance from an existing dictionary. Used for object type transformation.
        /// </summary>
        public PdfOutline(PdfDictionary dict)
            : base(dict)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfOutline"/> class.
        /// </summary>
        /// <param name="title">The outline text.</param>
        /// <param name="destinationPage">The destination page.</param>
        /// <param name="opened">Specifies whether the node is displayed expanded (opened) or collapsed.</param>
        /// <param name="style">The font style used to draw the outline text.</param>
        /// <param name="textColor">The color used to draw the outline text.</param>
        public PdfOutline(string title, PdfPage destinationPage, bool opened, PdfOutlineStyle style, XColor textColor)
        {
            Title = title;
            DestinationPage = destinationPage;
            Opened = opened;
            Style = style;
            TextColor = textColor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfOutline"/> class.
        /// </summary>
        /// <param name="title">The outline text.</param>
        /// <param name="destinationPage">The destination page.</param>
        /// <param name="opened">Specifies whether the node is displayed expanded (opened) or collapsed.</param>
        /// <param name="style">The font style used to draw the outline text.</param>
        public PdfOutline(string title, PdfPage destinationPage, bool opened, PdfOutlineStyle style)
        {
            Title = title;
            DestinationPage = destinationPage;
            Opened = opened;
            Style = style;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfOutline"/> class.
        /// </summary>
        /// <param name="title">The outline text.</param>
        /// <param name="destinationPage">The destination page.</param>
        /// <param name="opened">Specifies whether the node is displayed expanded (opened) or collapsed.</param>
        public PdfOutline(string title, PdfPage destinationPage, bool opened)
        {
            Title = title;
            DestinationPage = destinationPage;
            Opened = opened;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfOutline"/> class.
        /// </summary>
        /// <param name="title">The outline text.</param>
        /// <param name="destinationPage">The destination page.</param>
        public PdfOutline(string title, PdfPage destinationPage)
        {
            Title = title;
            DestinationPage = destinationPage;
        }

        internal int Count
        {
            get { return _count; }
            set { _count = value; }
        }
        int _count;

        /// <summary>
        /// The total number of open descendants at all lower levels.
        /// </summary>
        internal int OpenCount;

        /// <summary>
        /// Counts the open outline items. Not yet used.
        /// </summary>
        internal int CountOpen()
        {
            int count = _opened ? 1 : 0;
            if (_outlines != null)
                count += _outlines.CountOpen();
            return count;
        }

        /// <summary>
        /// Gets the parent of this outline item. The root item has no parent and returns null.
        /// </summary>
        public PdfOutline Parent
        {
            get { return _parent; }
            internal set { _parent = value; }
        }
        PdfOutline _parent;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get { return Elements.GetString(Keys.Title); }
            set
            {
                PdfString s = new PdfString(value, PdfStringEncoding.Unicode);
                Elements.SetValue(Keys.Title, s);
            }
        }

        /// <summary>
        /// Gets or sets the destination page.
        /// </summary>
        public PdfPage DestinationPage
        {
            get { return _destinationPage; }
            set { _destinationPage = value; }
        }
        PdfPage _destinationPage;

        /// <summary>
        /// Gets or sets the left position of the page positioned at the left side of the window.
        /// Applies only if PageDestinationType is Xyz, FitV, FitR, or FitBV.
        /// </summary>
        public double? Left
        {
            get { return _left; }
            set { _left = value; }
        }
        double? _left = null;

        /// <summary>
        /// Gets or sets the top position of the page positioned at the top side of the window.
        /// Applies only if PageDestinationType is Xyz, FitH, FitR, ob FitBH.
        /// </summary>
        public double? Top
        {
            get { return _top; }
            set { _top = value; }
        }
        double? _top = null;

        /// <summary>
        /// Gets or sets the right position of the page positioned at the right side of the window.
        /// Applies only if PageDestinationType is FitR.
        /// </summary>
        public double Right  // Cannot be null in a valid PDF.
        {
            get { return _right; }
            set { _right = value; }
        }
        double _right = double.NaN;

        /// <summary>
        /// Gets or sets the bottom position of the page positioned at the bottom side of the window.
        /// Applies only if PageDestinationType is FitR.
        /// </summary>
        public double Bottom  // Cannot be null in a valid PDF.
        {
            get { return _bottom; }
            set { _bottom = value; }
        }
        double _bottom = double.NaN;

        /// <summary>
        /// Gets or sets the zoom faction of the page.
        /// Applies only if PageDestinationType is Xyz.
        /// </summary>
        public double? Zoom
        {
            get { return _zoom; }
            set
            {
                if (value.HasValue && value.Value == 0)
                    _zoom = null;
                else
                    _zoom = value;
            }
        }
        double? _zoom; // PDF treats 0 and null equally.

        /// <summary>
        /// Gets or sets whether the outline item is opened (or expanded).
        /// </summary>
        public bool Opened
        {
            get { return _opened; }
#if true
            set { _opened = value; }
#else
            // TODO: adjust openCount of ascendant...
            set
            {
                if (_opened != value)
                {
                    _opened = value;
                    int sign = value ? 1 : -1;
                    PdfOutline parent = _parent;
                    if (_opened)
                    {
                        while (parent != null)
                            parent.openCount += 1 + _openCount;
                    }
                    else
                    {
                    }
                }
            }
#endif
        }
        bool _opened;

        /// <summary>
        /// Gets or sets the style of the outline text.
        /// </summary>
        public PdfOutlineStyle Style
        {
            get { return (PdfOutlineStyle)Elements.GetInteger(Keys.F); }
            set { Elements.SetInteger(Keys.F, (int)value); }
        }

        /// <summary>
        /// Gets or sets the type of the page destination.
        /// </summary>
        public PdfPageDestinationType PageDestinationType
        {
            get { return _pageDestinationType; }
            set { _pageDestinationType = value; }
        }
        PdfPageDestinationType _pageDestinationType = PdfPageDestinationType.Xyz;

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>The color of the text.</value>
        public XColor TextColor
        {
            get { return _textColor; }
            set { _textColor = value; }
        }
        XColor _textColor;

        /// <summary>
        /// Gets a value indicating whether this outline object has child items.
        /// </summary>
        public bool HasChildren
        {
            get { return _outlines != null && _outlines.Count > 0; }
        }

        /// <summary>
        /// Gets the outline collection of this node.
        /// </summary>
        public PdfOutlineCollection Outlines
        {
            get { return _outlines ?? (_outlines = new PdfOutlineCollection(Owner, this)); }
        }
        PdfOutlineCollection _outlines;

        /// <summary>
        /// Initializes this instance from an existing PDF document.
        /// </summary>
        void Initialize()
        {
            string title;
            if (Elements.TryGetString(Keys.Title, out title))
                Title = title;

            PdfReference parentRef = Elements.GetReference(Keys.Parent);
            if (parentRef != null)
            {
                PdfOutline parent = parentRef.Value as PdfOutline;
                if (parent != null)
                    Parent = parent;
            }

            Count = Elements.GetInteger(Keys.Count);

            PdfArray colors = Elements.GetArray(Keys.C);
            if (colors != null && colors.Elements.Count == 3)
            {
                double r = colors.Elements.GetReal(0);
                double g = colors.Elements.GetReal(1);
                double b = colors.Elements.GetReal(2);
                TextColor = XColor.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
            }

            // Style directly works on dictionary element.

            PdfItem dest = Elements.GetValue(Keys.Dest);
            PdfItem a = Elements.GetValue(Keys.A);
            Debug.Assert(dest == null || a == null, "Either destination or goto action.");

            PdfArray destArray = null;
            if (dest != null)
            {
                destArray = dest as PdfArray;
                if (destArray != null)
                {
                    SplitDestinationPage(destArray);
                }
                else
                {
                    Debug.Assert(false, "See what to do when this happened.");
                }
            }
            else if (a != null)
            {
                // The dictionary should be a GoTo action.
                PdfDictionary action = a as PdfDictionary;
                if (action != null && action.Elements.GetName(PdfAction.Keys.S) == "/GoTo")
                {
                    dest = action.Elements[PdfGoToAction.Keys.D];
                    destArray = dest as PdfArray;
                    if (destArray != null)
                    {
                        // Replace Action with /Dest entry.
                        Elements.Remove(Keys.A);
                        Elements.Add(Keys.Dest, destArray);
                        SplitDestinationPage(destArray);
                    }
                    else
                    {
                        throw new Exception("Destination Array expected.");
                    }
                }
                else
                {
                    Debug.Assert(false, "See what to do when this happened.");
                }
            }
            else
            {
                // Neither destination page nor GoTo action.
            }

            InitializeChildren();
        }

        void SplitDestinationPage(PdfArray destination)  // Reference: 8.2 Destination syntax / Page 582
        {
            // ReSharper disable HeuristicUnreachableCode
#pragma warning disable 162

            // The destination page may not yet have been transformed to PdfPage.
            PdfDictionary destPage = (PdfDictionary)((PdfReference)destination.Elements[0]).Value;
            PdfPage page = destPage as PdfPage;
            if (page == null)
                page = new PdfPage(destPage);

            DestinationPage = page;
            PdfName type = destination.Elements[1] as PdfName;
            if (type != null)
            {
                PageDestinationType = (PdfPageDestinationType)Enum.Parse(typeof(PdfPageDestinationType), type.Value.Substring(1), true);
                switch (PageDestinationType)
                {
                    // [page /XYZ left top zoom] -- left, top, and zoom can be null.
                    case PdfPageDestinationType.Xyz:
                        Left = destination.Elements.GetNullableReal(2);
                        Top = destination.Elements.GetNullableReal(3);
                        Zoom = destination.Elements.GetNullableReal(4); // For this parameter, null and 0 have the same meaning.
                        break;

                    // [page /Fit]
                    case PdfPageDestinationType.Fit:
                        // /Fit has no parameters.
                        break;

                    // [page /FitH top] -- top can be null.
                    case PdfPageDestinationType.FitH:
                        Top = destination.Elements.GetNullableReal(2);
                        break;

                    // [page /FitV left] -- left can be null.
                    case PdfPageDestinationType.FitV:
                        Left = destination.Elements.GetNullableReal(2);
                        break;

                    // [page /FitR left bottom right top] -- left, bottom, right, and top must not be null.
                    // TODO An exception in GetReal leads to an inconsistent document. Deal with that - e.g. by registering the corruption and preventing the user from saving the corrupted document.
                    case PdfPageDestinationType.FitR:
                        Left = destination.Elements.GetReal(2);
                        Bottom = destination.Elements.GetReal(3);
                        Right = destination.Elements.GetReal(4);
                        Top = destination.Elements.GetReal(5);
                        break;

                    // [page /FitB]
                    case PdfPageDestinationType.FitB:
                        // /Fit has no parameters.
                        break;

                    // [page /FitBH top] -- top can be null.
                    case PdfPageDestinationType.FitBH:
                        Top = destination.Elements.GetReal(2);
                        break;

                    // [page /FitBV left] -- left can be null.
                    case PdfPageDestinationType.FitBV:
                        Left = destination.Elements.GetReal(2);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

#pragma warning restore 162
            // ReSharper restore HeuristicUnreachableCode
        }

        void InitializeChildren()
        {
            PdfReference firstRef = Elements.GetReference(Keys.First);
            PdfReference lastRef = Elements.GetReference(Keys.Last);
            PdfReference current = firstRef;
            while (current != null)
            {
                // Create item and add it to outline items dictionary.
                PdfOutline item = new PdfOutline((PdfDictionary)current.Value);
                Outlines.Add(item);

                current = item.Elements.GetReference(Keys.Next);
#if DEBUG_
                if (current == null)
                {
                    if (item.Reference != lastRef)
                    {
                        // Word produces PDFs that come to this case.
                        GetType();
                    }
                }
#endif
            }
        }

        /// <summary>
        /// Creates key/values pairs according to the object structure.
        /// </summary>
        internal override void PrepareForSave()
        {
            bool hasKids = HasChildren;
            // Is something to do at all?
            if (_parent != null || hasKids)
            {
                if (_parent == null)
                {
                    // Case: This is the outline dictionary (the root).
                    // Reference: TABLE 8.3  Entries in the outline dictionary / Page 585
                    Debug.Assert(_outlines != null && _outlines.Count > 0 && _outlines[0] != null);
                    Elements[Keys.First] = _outlines[0].Reference;
                    Elements[Keys.Last] = _outlines[_outlines.Count - 1].Reference;

                    // TODO: /Count - the meaning is not completely clear to me.
                    // Get PDFs created with Acrobat and analyze what to implement.
                    if (OpenCount > 0)
                        Elements[Keys.Count] = new PdfInteger(OpenCount);
                }
                else
                {
                    // Case: This is an outline item dictionary.
                    // Reference: TABLE 8.4  Entries in the outline item dictionary / Page 585
                    Elements[Keys.Parent] = _parent.Reference;

                    int count = _parent._outlines.Count;
                    int index = _parent._outlines.IndexOf(this);
                    Debug.Assert(index != -1);

                    // Has destination?
                    if (DestinationPage != null)
                        Elements[Keys.Dest] = CreateDestArray();

                    // Not the first element?
                    if (index > 0)
                        Elements[Keys.Prev] = _parent._outlines[index - 1].Reference;

                    // Not the last element?
                    if (index < count - 1)
                        Elements[Keys.Next] = _parent._outlines[index + 1].Reference;

                    if (hasKids)
                    {
                        Elements[Keys.First] = _outlines[0].Reference;
                        Elements[Keys.Last] = _outlines[_outlines.Count - 1].Reference;
                    }
                    // TODO: /Count - the meaning is not completely clear to me
                    if (OpenCount > 0)
                        Elements[Keys.Count] = new PdfInteger((_opened ? 1 : -1) * OpenCount);

                    if (_textColor != XColor.Empty && Owner.HasVersion("1.4"))
                        Elements[Keys.C] = new PdfLiteral("[{0}]", PdfEncoders.ToString(_textColor, PdfColorMode.Rgb));

                    // if (Style != PdfOutlineStyle.Regular && Document.HasVersion("1.4"))
                    //  //pdf.AppendFormat("/F {0}\n", (int)_style);
                    //  Elements[Keys.F] = new PdfInteger((int)_style);
                }

                // Prepare child elements.
                if (hasKids)
                {
                    foreach (PdfOutline outline in _outlines)
                        outline.PrepareForSave();
                }
            }
        }

        PdfArray CreateDestArray()
        {
            PdfArray dest = null;
            switch (PageDestinationType)
            {
                // [page /XYZ left top zoom]
                case PdfPageDestinationType.Xyz:
                    dest = new PdfArray(Owner,
                        DestinationPage.Reference, new PdfLiteral(String.Format("/XYZ {0} {1} {2}", Fd(Left), Fd(Top), Fd(Zoom))));
                    break;

                // [page /Fit]
                case PdfPageDestinationType.Fit:
                    dest = new PdfArray(Owner,
                        DestinationPage.Reference, new PdfLiteral("/Fit"));
                    break;

                // [page /FitH top]
                case PdfPageDestinationType.FitH:
                    dest = new PdfArray(Owner,
                        DestinationPage.Reference, new PdfLiteral(String.Format("/FitH {0}", Fd(Top))));
                    break;

                // [page /FitV left]
                case PdfPageDestinationType.FitV:
                    dest = new PdfArray(Owner,
                        DestinationPage.Reference, new PdfLiteral(String.Format("/FitV {0}", Fd(Left))));
                    break;

                // [page /FitR left bottom right top]
                case PdfPageDestinationType.FitR:
                    dest = new PdfArray(Owner,
                        DestinationPage.Reference, new PdfLiteral(String.Format("/FitR {0} {1} {2} {3}", Fd(Left), Fd(Bottom), Fd(Right), Fd(Top))));
                    break;

                // [page /FitB]
                case PdfPageDestinationType.FitB:
                    dest = new PdfArray(Owner,
                        DestinationPage.Reference, new PdfLiteral("/FitB"));
                    break;

                // [page /FitBH top]
                case PdfPageDestinationType.FitBH:
                    dest = new PdfArray(Owner,
                        DestinationPage.Reference, new PdfLiteral(String.Format("/FitBH {0}", Fd(Top))));
                    break;

                // [page /FitBV left]
                case PdfPageDestinationType.FitBV:
                    dest = new PdfArray(Owner,
                        DestinationPage.Reference, new PdfLiteral(String.Format("/FitBV {0}", Fd(Left))));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return dest;
        }

        /// <summary>
        /// Format double.
        /// </summary>
        string Fd(double value)
        {
            if (Double.IsNaN(value))
                throw new InvalidOperationException("Value is not a valid Double.");
            return value.ToString("#.##", CultureInfo.InvariantCulture);

            //return Double.IsNaN(value) ? "null" : value.ToString("#.##", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Format nullable double.
        /// </summary>
        string Fd(double? value)
        {
            return value.HasValue ? value.Value.ToString("#.##", CultureInfo.InvariantCulture) : "null";
        }

        internal override void WriteObject(PdfWriter writer)
        {
#if DEBUG
            writer.WriteRaw("% Title = " + FilterUnicode(Title) + "\n");
#endif
            // TODO: Proof that there is nothing to do here.
            bool hasKids = HasChildren;
            if (_parent != null || hasKids)
            {
                ////// Everything done in PrepareForSave
                ////if (_parent == null)
                ////{
                ////    // This is the outline dictionary (the root)
                ////}
                ////else
                ////{
                ////    // This is an outline item dictionary
                ////}
                base.WriteObject(writer);
            }
        }

#if DEBUG
        private string FilterUnicode(string text)
        {
            StringBuilder result = new StringBuilder();
            foreach (char ch in text)
                result.Append((uint)ch < 256 ? (ch != '\r' && ch != '\n' ? ch : ' ') : '?');
            return result.ToString();
        }
#endif

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal sealed class Keys : KeysBase
        {
            // ReSharper disable InconsistentNaming

            /// <summary>
            /// (Optional) The type of PDF object that this dictionary describes; if present,
            /// must be Outlines for an outline dictionary.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional, FixedValue = "Outlines")]
            public const string Type = "/Type";

            // Outline and outline item are combined
            ///// <summary>
            ///// (Required if there are any open or closed outline entries; must be an indirect reference)
            ///// An outline item dictionary representing the first top-level item in the outline.
            ///// </summary>
            //[KeyInfo(KeyType.Dictionary)]
            //public const string First = "/First";
            //
            ///// <summary>
            ///// (Required if there are any open or closed outline entries; must be an indirect reference)
            ///// An outline item dictionary representing the last top-level item in the outline.
            ///// </summary>
            //[KeyInfo(KeyType.Dictionary)]
            //public const string Last = "/Last";
            //
            ///// <summary>
            ///// (Required if the document has any open outline entries) The total number of open items at all
            ///// levels of the outline. This entry should be omitted if there are no open outline items.
            ///// </summary>
            //[KeyInfo(KeyType.Integer)]
            //public const string Count = "/Count";

            /// <summary>
            /// (Required) The text to be displayed on the screen for this item.
            /// </summary>
            [KeyInfo(KeyType.String | KeyType.Required)]
            public const string Title = "/Title";

            /// <summary>
            /// (Required; must be an indirect reference) The parent of this item in the outline hierarchy.
            /// The parent of a top-level item is the outline dictionary itself.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Required)]
            public const string Parent = "/Parent";

            /// <summary>
            /// (Required for all but the first item at each level; must be an indirect reference)
            /// The previous item at this outline level.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Required)]
            public const string Prev = "/Prev";

            /// <summary>
            /// (Required for all but the last item at each level; must be an indirect reference)
            /// The next item at this outline level.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Required)]
            public const string Next = "/Next";

            /// <summary>
            /// (Required if the item has any descendants; must be an indirect reference)
            ///  The first of this item�s immediate children in the outline hierarchy.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Required)]
            public const string First = "/First";

            /// <summary>
            /// (Required if the item has any descendants; must be an indirect reference)
            /// The last of this item�s immediate children in the outline hierarchy.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Required)]
            public const string Last = "/Last";

            /// <summary>
            /// (Required if the item has any descendants) If the item is open, the total number of its 
            /// open descendants at all lower levels of the outline hierarchy. If the item is closed, a 
            /// negative integer whose absolute value specifies how many descendants would appear if the 
            /// item were reopened.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string Count = "/Count";

            /// <summary>
            /// (Optional; not permitted if an A entry is present) The destination to be displayed when this 
            /// item is activated.
            /// </summary>
            [KeyInfo(KeyType.ArrayOrNameOrString | KeyType.Optional)]
            public const string Dest = "/Dest";

            /// <summary>
            /// (Optional; not permitted if a Dest entry is present) The action to be performed when
            /// this item is activated.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string A = "/A";

            /// <summary>
            /// (Optional; PDF 1.3; must be an indirect reference) The structure element to which the item 
            /// refers.
            /// Note: The ability to associate an outline item with a structure element (such as the beginning 
            /// of a chapter) is a PDF 1.3 feature. For backward compatibility with earlier PDF versions, such
            /// an item should also specify a destination (Dest) corresponding to an area of a page where the
            /// contents of the designated structure element are displayed.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string SE = "/SE";

            /// <summary>
            /// (Optional; PDF 1.4) An array of three numbers in the range 0.0 to 1.0, representing the 
            /// components in the DeviceRGB color space of the color to be used for the outline entry�s text.
            /// Default value: [0.0 0.0 0.0].
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string C = "/C";

            /// <summary>
            /// (Optional; PDF 1.4) A set of flags specifying style characteristics for displaying the outline
            /// item�s text. Default value: 0.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string F = "/F";

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
    }
}
