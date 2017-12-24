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
    /// This class represents an axis in a chart.
    /// </summary>
    public class Axis : ChartObject
    {
        /// <summary>
        /// Initializes a new instance of the Axis class with the specified parent.
        /// </summary>
        internal Axis(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Axis Clone()
        {
            return (Axis)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Axis axis = (Axis)base.DeepCopy();
            if (axis._title != null)
            {
                axis._title = axis._title.Clone();
                axis._title._parent = axis;
            }
            if (axis._tickLabels != null)
            {
                axis._tickLabels = axis._tickLabels.Clone();
                axis._tickLabels._parent = axis;
            }
            if (axis._lineFormat != null)
            {
                axis._lineFormat = axis._lineFormat.Clone();
                axis._lineFormat._parent = axis;
            }
            if (axis._majorGridlines != null)
            {
                axis._majorGridlines = axis._majorGridlines.Clone();
                axis._majorGridlines._parent = axis;
            }
            if (axis._minorGridlines != null)
            {
                axis._minorGridlines = axis._minorGridlines.Clone();
                axis._minorGridlines._parent = axis;
            }
            return axis;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the title of the axis.
        /// </summary>
        public AxisTitle Title
        {
            get { return _title ?? (_title = new AxisTitle(this)); }
        }
        internal AxisTitle _title;

        /// <summary>
        /// Gets or sets the minimum value of the axis.
        /// </summary>
        public double MinimumScale
        {
            get { return _minimumScale; }
            set { _minimumScale = value; }
        }
        internal double _minimumScale = double.NaN;

        /// <summary>
        /// Gets or sets the maximum value of the axis.
        /// </summary>
        public double MaximumScale
        {
            get { return _maximumScale; }
            set { _maximumScale = value; }
        }
        internal double _maximumScale = double.NaN;

        /// <summary>
        /// Gets or sets the interval of the primary tick.
        /// </summary>
        public double MajorTick
        {
            get { return _majorTick; }
            set { _majorTick = value; }
        }
        internal double _majorTick = double.NaN;

        /// <summary>
        /// Gets or sets the interval of the secondary tick.
        /// </summary>
        public double MinorTick
        {
            get { return _minorTick; }
            set { _minorTick = value; }
        }
        internal double _minorTick = double.NaN;

        /// <summary>
        /// Gets or sets the type of the primary tick mark.
        /// </summary>
        public TickMarkType MajorTickMark
        {
            get { return _majorTickMark; }
            set
            {
                if (!Enum.IsDefined(typeof(TickMarkType), value))
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(TickMarkType));
                _majorTickMark = value;
                _majorTickMarkInitialized = true;
            }
        }
        internal TickMarkType _majorTickMark;
        internal bool _majorTickMarkInitialized;

        /// <summary>
        /// Gets or sets the type of the secondary tick mark.
        /// </summary>
        public TickMarkType MinorTickMark
        {
            get { return _minorTickMark; }
            set
            {
                if (!Enum.IsDefined(typeof(TickMarkType), value))
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(TickMarkType));
                _minorTickMark = value;
                _minorTickMarkInitialized = true;
            }
        }
        internal TickMarkType _minorTickMark;
        internal bool _minorTickMarkInitialized;

        /// <summary>
        /// Gets the label of the primary tick.
        /// </summary>
        public TickLabels TickLabels
        {
            get { return _tickLabels ?? (_tickLabels = new TickLabels(this)); }
        }
        internal TickLabels _tickLabels;

        /// <summary>
        /// Gets the format of the axis line.
        /// </summary>
        public LineFormat LineFormat
        {
            get { return _lineFormat ?? (_lineFormat = new LineFormat(this)); }
        }
        internal LineFormat _lineFormat;

        /// <summary>
        /// Gets the primary gridline object.
        /// </summary>
        public Gridlines MajorGridlines
        {
            get { return _majorGridlines ?? (_majorGridlines = new Gridlines(this)); }
        }
        internal Gridlines _majorGridlines;

        /// <summary>
        /// Gets the secondary gridline object.
        /// </summary>
        public Gridlines MinorGridlines
        {
            get { return _minorGridlines ?? (_minorGridlines = new Gridlines(this)); }
        }
        internal Gridlines _minorGridlines;

        /// <summary>
        /// Gets or sets, whether the axis has a primary gridline object.
        /// </summary>
        public bool HasMajorGridlines
        {
            get { return _hasMajorGridlines; }
            set { _hasMajorGridlines = value; }
        }
        internal bool _hasMajorGridlines;

        /// <summary>
        /// Gets or sets, whether the axis has a secondary gridline object.
        /// </summary>
        public bool HasMinorGridlines
        {
            get { return _hasMinorGridlines; }
            set { _hasMinorGridlines = value; }
        }
        internal bool _hasMinorGridlines;
        #endregion
    }
}
