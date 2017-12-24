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

using PdfSharp.Drawing;

namespace PdfSharp.Charting
{
    /// <summary>
    /// Defines the background filling of the shape.
    /// </summary>
    public class FillFormat : DocumentObject
    {
        /// <summary>
        /// Initializes a new instance of the FillFormat class.
        /// </summary>
        public FillFormat()
        { }

        /// <summary>
        /// Initializes a new instance of the FillFormat class with the specified parent.
        /// </summary>
        internal FillFormat(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new FillFormat Clone()
        {
            return (FillFormat)DeepCopy();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the color of the filling.
        /// </summary>
        public XColor Color
        {
            get { return _color; }
            set { _color = value; }
        }
        internal XColor _color = XColor.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the background color should be visible.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }
        internal bool _visible;
        #endregion
    }
}
