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
    /// Represents charts with different types.
    /// </summary>
    public class Chart : DocumentObject
    {
        /// <summary>
        /// Initializes a new instance of the Chart class.
        /// </summary>
        public Chart()
        { }

        /// <summary>
        /// Initializes a new instance of the Chart class with the specified parent.
        /// </summary>
        internal Chart(DocumentObject parent) : base(parent) { }

        /// <summary>
        /// Initializes a new instance of the Chart class with the specified chart type.
        /// </summary>
        public Chart(ChartType type)
            : this()
        {
            Type = type;
        }

        #region Methods
        /// <summary>
        /// Creates a deep copy of this object.
        /// </summary>
        public new Chart Clone()
        {
            return (Chart)DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected override object DeepCopy()
        {
            Chart chart = (Chart)base.DeepCopy();
            if (chart._xAxis != null)
            {
                chart._xAxis = chart._xAxis.Clone();
                chart._xAxis._parent = chart;
            }
            if (chart._yAxis != null)
            {
                chart._yAxis = chart._yAxis.Clone();
                chart._yAxis._parent = chart;
            }
            if (chart._zAxis != null)
            {
                chart._zAxis = chart._zAxis.Clone();
                chart._zAxis._parent = chart;
            }
            if (chart._seriesCollection != null)
            {
                chart._seriesCollection = chart._seriesCollection.Clone();
                chart._seriesCollection._parent = chart;
            }
            if (chart._xValues != null)
            {
                chart._xValues = chart._xValues.Clone();
                chart._xValues._parent = chart;
            }
            if (chart._plotArea != null)
            {
                chart._plotArea = chart._plotArea.Clone();
                chart._plotArea._parent = chart;
            }
            if (chart._dataLabel != null)
            {
                chart._dataLabel = chart._dataLabel.Clone();
                chart._dataLabel._parent = chart;
            }
            return chart;
        }

        /// <summary>
        /// Determines the type of the given axis.
        /// </summary>
        internal string CheckAxis(Axis axis)
        {
            if ((_xAxis != null) && (axis == _xAxis))
                return "xaxis";
            if ((_yAxis != null) && (axis == _yAxis))
                return "yaxis";
            if ((_zAxis != null) && (axis == _zAxis))
                return "zaxis";

            return "";
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the base type of the chart.
        /// ChartType of the series can be overwritten.
        /// </summary>
        public ChartType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        internal ChartType _type;

        /// <summary>
        /// Gets or sets the font for the chart. This will be the default font for all objects which are
        /// part of the chart.
        /// </summary>
        public Font Font
        {
            get { return _font ?? (_font = new Font(this)); }
        }
        internal Font _font;

        /// <summary>
        /// Gets the legend of the chart.
        /// </summary>
        public Legend Legend
        {
            get { return _legend ?? (_legend = new Legend(this)); }
        }
        internal Legend _legend;

        /// <summary>
        /// Gets the X-Axis of the Chart.
        /// </summary>
        public Axis XAxis
        {
            get { return _xAxis ?? (_xAxis = new Axis(this)); }
        }
        internal Axis _xAxis;

        /// <summary>
        /// Gets the Y-Axis of the Chart.
        /// </summary>
        public Axis YAxis
        {
            get { return _yAxis ?? (_yAxis = new Axis(this)); }
        }
        internal Axis _yAxis;

        /// <summary>
        /// Gets the Z-Axis of the Chart.
        /// </summary>
        public Axis ZAxis
        {
            get { return _zAxis ?? (_zAxis = new Axis(this)); }
        }
        internal Axis _zAxis;

        /// <summary>
        /// Gets the collection of the data series.
        /// </summary>
        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection ?? (_seriesCollection = new SeriesCollection(this)); }
        }
        internal SeriesCollection _seriesCollection;

        /// <summary>
        /// Gets the collection of the values written on the X-Axis.
        /// </summary>
        public XValues XValues
        {
            get { return _xValues ?? (_xValues = new XValues(this)); }
        }
        internal XValues _xValues;

        /// <summary>
        /// Gets the plot (drawing) area of the chart.
        /// </summary>
        public PlotArea PlotArea
        {
            get { return _plotArea ?? (_plotArea = new PlotArea(this)); }
        }
        internal PlotArea _plotArea;

        /// <summary>
        /// Gets or sets a value defining how blanks in the data series should be shown.
        /// </summary>
        public BlankType DisplayBlanksAs
        {
            get { return _displayBlanksAs; }
            set { _displayBlanksAs = value; }
        }
        internal BlankType _displayBlanksAs;

        /// <summary>
        /// Gets the DataLabel of the chart.
        /// </summary>
        public DataLabel DataLabel
        {
            get { return _dataLabel ?? (_dataLabel = new DataLabel(this)); }
        }
        internal DataLabel _dataLabel;

        /// <summary>
        /// Gets or sets whether the chart has a DataLabel.
        /// </summary>
        public bool HasDataLabel
        {
            get { return _hasDataLabel; }
            set { _hasDataLabel = value; }
        }
        internal bool _hasDataLabel;
        #endregion
    }
}
