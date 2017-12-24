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
#if !SILVERLIGHT
using System.ComponentModel;
#endif

namespace PdfSharp.Charting
{
    /// <summary>
    /// Represents a series of data on the chart.
    /// </summary>
    public class Series : ChartObject
    {
        /// <summary>
        /// Initializes a new instance of the Series class.
        /// </summary>
        public Series()
        { }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Series Clone()
        {
            return (Series)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Series series = (Series)base.DeepCopy();
            if (series._seriesElements != null)
            {
                series._seriesElements = series._seriesElements.Clone();
                series._seriesElements._parent = series;
            }
            if (series._lineFormat != null)
            {
                series._lineFormat = series._lineFormat.Clone();
                series._lineFormat._parent = series;
            }
            if (series._fillFormat != null)
            {
                series._fillFormat = series._fillFormat.Clone();
                series._fillFormat._parent = series;
            }
            if (series._dataLabel != null)
            {
                series._dataLabel = series._dataLabel.Clone();
                series._dataLabel._parent = series;
            }
            return series;
        }

        /// <summary>
        /// Adds a blank to the series.
        /// </summary>
        public void AddBlank()
        {
            Elements.AddBlank();
        }

        /// <summary>
        /// Adds a real value to the series.
        /// </summary>
        public Point Add(double value)
        {
            return Elements.Add(value);
        }

        /// <summary>
        /// Adds an array of real values to the series.
        /// </summary>
        public void Add(params double[] values)
        {
            Elements.Add(values);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The actual value container of the series.
        /// </summary>
        public SeriesElements Elements
        {
            get { return _seriesElements ?? (_seriesElements = new SeriesElements(this)); }
        }
        internal SeriesElements _seriesElements;

        /// <summary>
        /// Gets or sets the name of the series which will be used in the legend.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        internal string _name = String.Empty;

        /// <summary>
        /// Gets the line format of the border of each data.
        /// </summary>
        public LineFormat LineFormat
        {
            get { return _lineFormat ?? (_lineFormat = new LineFormat(this)); }
        }
        internal LineFormat _lineFormat;

        /// <summary>
        /// Gets the background filling of the data.
        /// </summary>
        public FillFormat FillFormat
        {
            get { return _fillFormat ?? (_fillFormat = new FillFormat(this)); }
        }
        internal FillFormat _fillFormat;

        /// <summary>
        /// Gets or sets the size of the marker in a line chart.
        /// </summary>
        public XUnit MarkerSize
        {
            get { return _markerSize; }
            set { _markerSize = value; }
        }
        internal XUnit _markerSize;

        /// <summary>
        /// Gets or sets the style of the marker in a line chart.
        /// </summary>
        public MarkerStyle MarkerStyle
        {
            get { return _markerStyle; }
            set
            {
                if (!Enum.IsDefined(typeof(MarkerStyle), value))
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(MarkerStyle));

                _markerStyle = value;
                _markerStyleInitialized = true;
            }
        }
        internal MarkerStyle _markerStyle;
        internal bool _markerStyleInitialized;

        /// <summary>
        /// Gets or sets the foreground color of the marker in a line chart.
        /// </summary>
        public XColor MarkerForegroundColor
        {
            get { return _markerForegroundColor; }
            set { _markerForegroundColor = value; }
        }
        internal XColor _markerForegroundColor = XColor.Empty;

        /// <summary>
        /// Gets or sets the background color of the marker in a line chart.
        /// </summary>
        public XColor MarkerBackgroundColor
        {
            get { return _markerBackgroundColor; }
            set { _markerBackgroundColor = value; }
        }
        internal XColor _markerBackgroundColor = XColor.Empty;

        /// <summary>
        /// Gets or sets the chart type of the series if it's intended to be different than the
        /// global chart type.
        /// </summary>
        public ChartType ChartType
        {
            get { return _chartType; }
            set
            {
                if (!Enum.IsDefined(typeof(ChartType), value))
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(ChartType));

                _chartType = value;
            }
        }
        internal ChartType _chartType;

        /// <summary>
        /// Gets the DataLabel of the series.
        /// </summary>
        public DataLabel DataLabel
        {
            get { return _dataLabel ?? (_dataLabel = new DataLabel(this)); }
        }
        internal DataLabel _dataLabel;

        /// <summary>
        /// Gets or sets whether the series has a DataLabel.
        /// </summary>
        public bool HasDataLabel
        {
            get { return _hasDataLabel; }
            set { _hasDataLabel = value; }
        }
        internal bool _hasDataLabel;

        /// <summary>
        /// Gets the element count of the series.
        /// </summary>
        public int Count
        {
            get
            {
                return _seriesElements != null ? _seriesElements.Count : 0;
            }
        }
        #endregion
    }
}
