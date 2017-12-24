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
#if !WINDOWS_PHONE
using System.ComponentModel;
#endif

namespace PdfSharp.Charting
{
    /// <summary>
    /// Represents a legend of a chart.
    /// </summary>
    public class Legend : ChartObject
    {
        /// <summary>
        /// Initializes a new instance of the Legend class.
        /// </summary>
        public Legend()
        { }

        /// <summary>
        /// Initializes a new instance of the Legend class with the specified parent.
        /// </summary>
        internal Legend(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Legend Clone()
        {
            return (Legend)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Legend legend = (Legend)base.DeepCopy();
            if (legend._lineFormat != null)
            {
                legend._lineFormat = legend._lineFormat.Clone();
                legend._lineFormat._parent = legend;
            }
            if (legend._font != null)
            {
                legend._font = legend._font.Clone();
                legend._font._parent = legend;
            }
            return legend;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the line format of the legend's border.
        /// </summary>
        public LineFormat LineFormat
        {
            get { return _lineFormat ?? (_lineFormat = new LineFormat(this)); }
        }
        internal LineFormat _lineFormat;

        /// <summary>
        /// Gets the font of the legend.
        /// </summary>
        public Font Font
        {
            get { return _font ?? (_font = new Font(this)); }
        }
        internal Font _font;

        /// <summary>
        /// Gets or sets the docking type.
        /// </summary>
        public DockingType Docking
        {
            get { return _docking; }
            set
            {
                if (!Enum.IsDefined(typeof(DockingType), value))
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(DockingType));
                _docking = value;
            }
        }
        internal DockingType _docking;
        #endregion
    }
}
