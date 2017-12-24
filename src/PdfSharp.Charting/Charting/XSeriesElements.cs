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
    /// Represents the collection of the value in an XSeries.
    /// </summary>
    public class XSeriesElements : DocumentObjectCollection
    {
        /// <summary>
        /// Initializes a new instance of the XSeriesElements class.
        /// </summary>
        public XSeriesElements()
        { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new XSeriesElements Clone()
        {
            return (XSeriesElements)base.DeepCopy();
        }

        /// <summary>
        /// Adds a blank to the XSeries.
        /// </summary>
        public void AddBlank()
        {
            base.Add(null);
        }

        /// <summary>
        /// Adds a value to the XSeries.
        /// </summary>
        public XValue Add(string value)
        {
            XValue xValue = new XValue(value);
            Add(xValue);
            return xValue;
        }

        /// <summary>
        /// Adds an array of values to the XSeries.
        /// </summary>
        public void Add(params string[] values)
        {
            foreach (string val in values)
                Add(val);
        }
        #endregion
    }
}
