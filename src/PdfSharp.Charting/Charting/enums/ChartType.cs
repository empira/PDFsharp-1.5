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
    /// Specifies with type of chart will be drawn.
    /// </summary>
    public enum ChartType
    {
        /// <summary>
        /// A line chart.
        /// </summary>
        Line,

        /// <summary>
        /// A clustered 2d column chart.
        /// </summary>
        Column2D,

        /// <summary>
        /// A stacked 2d column chart.
        /// </summary>
        ColumnStacked2D,

        /// <summary>
        /// A 2d area chart.
        /// </summary>
        Area2D,

        /// <summary>
        /// A clustered 2d bar chart.
        /// </summary>
        Bar2D,

        /// <summary>
        /// A stacked 2d bar chart.
        /// </summary>
        BarStacked2D,

        /// <summary>
        /// A 2d pie chart.
        /// </summary>
        Pie2D,

        /// <summary>
        /// An exploded 2d pie chart.
        /// </summary>
        PieExploded2D,
    }
}
