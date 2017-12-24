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
    /// Base class for all plot area renderers.
    /// </summary>
    internal abstract class ColumnLikePlotAreaRenderer : PlotAreaRenderer
    {
        /// <summary>
        /// Initializes a new instance of the ColumnLikePlotAreaRenderer class with the
        /// specified renderer parameters.
        /// </summary>
        internal ColumnLikePlotAreaRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Layouts and calculates the space for column like plot areas.
        /// </summary>
        internal override void Format()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;

            double xMin = cri.xAxisRendererInfo.MinimumScale;
            double xMax = cri.xAxisRendererInfo.MaximumScale;
            double yMin = cri.yAxisRendererInfo.MinimumScale;
            double yMax = cri.yAxisRendererInfo.MaximumScale;

            XRect plotAreaBox = cri.plotAreaRendererInfo.Rect;

            cri.plotAreaRendererInfo._matrix = new XMatrix();
            cri.plotAreaRendererInfo._matrix.TranslatePrepend(-xMin, yMax);
            cri.plotAreaRendererInfo._matrix.Scale(plotAreaBox.Width / xMax, plotAreaBox.Height / (yMax - yMin), XMatrixOrder.Append);
            cri.plotAreaRendererInfo._matrix.ScalePrepend(1, -1);
            cri.plotAreaRendererInfo._matrix.Translate(plotAreaBox.X, plotAreaBox.Y, XMatrixOrder.Append);
        }
    }
}
