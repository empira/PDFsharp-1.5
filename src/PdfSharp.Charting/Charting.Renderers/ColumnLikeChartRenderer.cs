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

namespace PdfSharp.Charting.Renderers
{
    /// <summary>
    /// Represents column like chart renderer.
    /// </summary>
    internal abstract class ColumnLikeChartRenderer : ChartRenderer
    {
        /// <summary>
        /// Initializes a new instance of the ColumnLikeChartRenderer class with the
        /// specified renderer parameters.
        /// </summary>
        internal ColumnLikeChartRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Calculates the chart layout.
        /// </summary>
        internal void CalcLayout()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;

            // Calculate rects and positions.
            XRect chartRect = LayoutLegend();
            cri.xAxisRendererInfo.X = chartRect.Left + cri.yAxisRendererInfo.Width;
            cri.xAxisRendererInfo.Y = chartRect.Bottom - cri.xAxisRendererInfo.Height;
            cri.xAxisRendererInfo.Width = chartRect.Width - cri.yAxisRendererInfo.Width;
            cri.yAxisRendererInfo.X = chartRect.Left;
            cri.yAxisRendererInfo.Y = chartRect.Top;
            cri.yAxisRendererInfo.Height = cri.xAxisRendererInfo.Y - chartRect.Top;
            cri.plotAreaRendererInfo.X = cri.xAxisRendererInfo.X;
            cri.plotAreaRendererInfo.Y = cri.yAxisRendererInfo.InnerRect.Y;
            cri.plotAreaRendererInfo.Width = cri.xAxisRendererInfo.Width;
            cri.plotAreaRendererInfo.Height = cri.yAxisRendererInfo.InnerRect.Height;
        }
    }
}
