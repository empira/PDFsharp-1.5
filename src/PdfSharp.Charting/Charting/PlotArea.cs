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
    /// Represents the area where the actual chart is drawn.
    /// </summary>
    public class PlotArea : ChartObject
    {
        /// <summary>
        /// Initializes a new instance of the PlotArea class.
        /// </summary>
        internal PlotArea()
        { }

        /// <summary>
        /// Initializes a new instance of the PlotArea class with the specified parent.
        /// </summary>
        internal PlotArea(DocumentObject parent) : base(parent) { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new PlotArea Clone()
        {
            return (PlotArea)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            PlotArea plotArea = (PlotArea)base.DeepCopy();
            if (plotArea._lineFormat != null)
            {
                plotArea._lineFormat = plotArea._lineFormat.Clone();
                plotArea._lineFormat._parent = plotArea;
            }
            if (plotArea._fillFormat != null)
            {
                plotArea._fillFormat = plotArea._fillFormat.Clone();
                plotArea._fillFormat._parent = plotArea;
            }
            return plotArea;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the line format of the plot area's border.
        /// </summary>
        public LineFormat LineFormat
        {
            get { return _lineFormat ?? (_lineFormat = new LineFormat(this)); }
        }
        internal LineFormat _lineFormat;

        /// <summary>
        /// Gets the background filling of the plot area.
        /// </summary>
        public FillFormat FillFormat
        {
            get { return _fillFormat ?? (_fillFormat = new FillFormat(this)); }
        }
        internal FillFormat _fillFormat;

        /// <summary>
        /// Gets or sets the left padding of the area.
        /// </summary>
        public XUnit LeftPadding
        {
            get { return _leftPadding; }
            set { _leftPadding = value; }
        }
        internal XUnit _leftPadding;

        /// <summary>
        /// Gets or sets the right padding of the area.
        /// </summary>
        public XUnit RightPadding
        {
            get { return _rightPadding; }
            set { _rightPadding = value; }
        }
        internal XUnit _rightPadding;

        /// <summary>
        /// Gets or sets the top padding of the area.
        /// </summary>
        public XUnit TopPadding
        {
            get { return _topPadding; }
            set { _topPadding = value; }
        }
        internal XUnit _topPadding;

        /// <summary>
        /// Gets or sets the bottom padding of the area.
        /// </summary>
        public XUnit BottomPadding
        {
            get { return _bottomPadding; }
            set { _bottomPadding = value; }
        }
        internal XUnit _bottomPadding;
        #endregion
    }
}
