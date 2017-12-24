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
    /// Represents the base class for all chart renderers.
    /// </summary>
    internal abstract class ChartRenderer : Renderer
    {
        /// <summary>
        /// Initializes a new instance of the ChartRenderer class with the specified renderer parameters.
        /// </summary>
        internal ChartRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Calculates the space used by the legend and returns the remaining space available for the
        /// other parts of the chart.
        /// </summary>
        protected XRect LayoutLegend()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;
            XRect remainingRect = _rendererParms.Box;
            if (cri.legendRendererInfo != null)
            {
                switch (cri.legendRendererInfo._legend.Docking)
                {
                    case DockingType.Left:
                        cri.legendRendererInfo.X = remainingRect.Left;
                        cri.legendRendererInfo.Y = remainingRect.Height / 2 - cri.legendRendererInfo.Height / 2;
                        double width = cri.legendRendererInfo.Width + LegendSpacing;
                        remainingRect.X += width;
                        remainingRect.Width -= width;
                        break;

                    case DockingType.Right:
                        cri.legendRendererInfo.X = remainingRect.Right - cri.legendRendererInfo.Width;
                        cri.legendRendererInfo.Y = remainingRect.Height / 2 - cri.legendRendererInfo.Height / 2;
                        remainingRect.Width -= cri.legendRendererInfo.Width + LegendSpacing;
                        break;

                    case DockingType.Top:
                        cri.legendRendererInfo.X = remainingRect.Width / 2 - cri.legendRendererInfo.Width / 2;
                        cri.legendRendererInfo.Y = remainingRect.Top;
                        double height = cri.legendRendererInfo.Height + LegendSpacing;
                        remainingRect.Y += height;
                        remainingRect.Height -= height;
                        break;

                    case DockingType.Bottom:
                        cri.legendRendererInfo.X = remainingRect.Width / 2 - cri.legendRendererInfo.Width / 2;
                        cri.legendRendererInfo.Y = remainingRect.Bottom - cri.legendRendererInfo.Height;
                        remainingRect.Height -= cri.legendRendererInfo.Height + LegendSpacing;
                        break;
                }
            }
            return remainingRect;
        }

        /// <summary>
        /// Used to separate the legend from the plot area.
        /// </summary>
        private const double LegendSpacing = 0;

        /// <summary>
        /// Represents the default width for all series lines, like borders in column/bar charts.
        /// </summary>
        protected static readonly double DefaultSeriesLineWidth = 0.15;
    }
}
