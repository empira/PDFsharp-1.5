#region PDFsharp Charting - A .NET charting library based on PDFsharp
//
// Authors:
//   Niklas Schneider
//
// Copyright (c) 2005-2017 empira Software GmbH, Cologne Area (Germany)
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
using PdfSharp.Drawing;

namespace PdfSharp.Charting
{
    /// <summary>
    /// Font represents the formatting of characters in a paragraph.
    /// </summary>
    public sealed class Font : DocumentObject
    {
        /// <summary>
        /// Initializes a new instance of the Font class that can be used as a template.
        /// </summary>
        public Font()
        { }

        /// <summary>
        /// Initializes a new instance of the Font class with the specified parent.
        /// </summary>
        internal Font(DocumentObject parent)
            : base(parent)
        { }

        /// <summary>
        /// Initializes a new instance of the Font class with the specified name and size.
        /// </summary>
        public Font(string name, XUnit size)
            : this()
        {
            _name = name;
            _size = size;
        }

        #region Methods
        /// <summary>
        /// Creates a copy of the Font.
        /// </summary>
        public new Font Clone()
        {
            return (Font)DeepCopy();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of the font.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        internal string _name = String.Empty;

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        public XUnit Size
        {
            get { return _size; }
            set { _size = value; }
        }
        internal XUnit _size;

        /// <summary>
        /// Gets or sets the bold property.
        /// </summary>
        public bool Bold
        {
            get { return _bold; }
            set { _bold = value; }
        }
        internal bool _bold;

        /// <summary>
        /// Gets or sets the italic property.
        /// </summary>
        public bool Italic
        {
            get { return _italic; }
            set { _italic = value; }
        }
        internal bool _italic;

        /// <summary>
        /// Gets or sets the underline property.
        /// </summary>
        public Underline Underline
        {
            get { return _underline; }
            set { _underline = value; }
        }
        internal Underline _underline;

        /// <summary>
        /// Gets or sets the color property.
        /// </summary>
        public XColor Color
        {
            get { return _color; }
            set { _color = value; }
        }
        internal XColor _color = XColor.Empty;

        /// <summary>
        /// Gets or sets the superscript property.
        /// </summary>
        public bool Superscript
        {
            get { return _superscript; }
            set
            {
                if (_superscript != value)
                {
                    _superscript = value;
                    _subscript = false;
                }
            }
        }
        internal bool _superscript;

        /// <summary>
        /// Gets or sets the subscript property.
        /// </summary>
        public bool Subscript
        {
            get { return _subscript; }
            set
            {
                if (_subscript != value)
                {
                    _subscript = value;
                    _superscript = false;
                }
            }
        }
        internal bool _subscript;
        #endregion
    }
}
