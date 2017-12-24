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

namespace PdfSharp.Charting
{
    /// <summary>
    /// Represents the collection of the values in a data series.
    /// </summary>
    public class SeriesElements : DocumentObjectCollection
    {
        /// <summary>
        /// Initializes a new instance of the SeriesElements class.
        /// </summary>
        internal SeriesElements()
        { }

        /// <summary>
        /// Initializes a new instance of the SeriesElements class with the specified parent.
        /// </summary>
        internal SeriesElements(DocumentObject parent) : base(parent) { }

        /// <summary>
        /// Gets a point by its index.
        /// </summary>
        public new Point this[int index]
        {
            get { return (Point)base[index]; }
        }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new SeriesElements Clone()
        {
            return (SeriesElements)DeepCopy();
        }

        /// <summary>
        /// Adds a blank to the series.
        /// </summary>
        public void AddBlank()
        {
            base.Add(null);
        }

        /// <summary>
        /// Adds a new point with a real value to the series.
        /// </summary>
        public Point Add(double value)
        {
            Point point = new Point(value);
            Add(point);
            return point;
        }

        /// <summary>
        /// Adds an array of new points with real values to the series.
        /// </summary>
        public void Add(params double[] values)
        {
            foreach (double val in values)
                Add(val);
        }
        #endregion
    }
}