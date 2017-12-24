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
    /// Represents gridlines used by column or line charts, i. e. X axis grid will be rendered
    /// from top to bottom and Y axis grid will be rendered from left to right of the plot area.
    /// </summary>
    internal class ColumnLikeGridlinesRenderer : GridlinesRenderer
    {
        /// <summary>
        /// Initializes a new instance of the ColumnLikeGridlinesRenderer class with the
        /// specified renderer parameters.
        /// </summary>
        internal ColumnLikeGridlinesRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Draws the gridlines into the plot area.
        /// </summary>
        internal override void Draw()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;

            XRect plotAreaRect = cri.plotAreaRendererInfo.Rect;
            if (plotAreaRect.IsEmpty)
                return;

            AxisRendererInfo xari = cri.xAxisRendererInfo;
            AxisRendererInfo yari = cri.yAxisRendererInfo;

            double xMin = xari.MinimumScale;
            double xMax = xari.MaximumScale;
            double yMin = yari.MinimumScale;
            double yMax = yari.MaximumScale;
            double xMajorTick = xari.MajorTick;
            double yMajorTick = yari.MajorTick;
            double xMinorTick = xari.MinorTick;
            double yMinorTick = yari.MinorTick;

            XMatrix matrix = cri.plotAreaRendererInfo._matrix;

            LineFormatRenderer lineFormatRenderer;
            XGraphics gfx = _rendererParms.Graphics;

            XPoint[] points = new XPoint[2];
            if (xari.MinorGridlinesLineFormat != null)
            {
                lineFormatRenderer = new LineFormatRenderer(gfx, xari.MinorGridlinesLineFormat);
                for (double x = xMin + xMinorTick; x < xMax; x += xMinorTick)
                {
                    points[0].X = x;
                    points[0].Y = yMin;
                    points[1].X = x;
                    points[1].Y = yMax;
                    matrix.TransformPoints(points);
                    lineFormatRenderer.DrawLine(points[0], points[1]);
                }
            }

            if (xari.MajorGridlinesLineFormat != null)
            {
                lineFormatRenderer = new LineFormatRenderer(gfx, xari.MajorGridlinesLineFormat);
                for (double x = xMin; x <= xMax; x += xMajorTick)
                {
                    points[0].X = x;
                    points[0].Y = yMin;
                    points[1].X = x;
                    points[1].Y = yMax;
                    matrix.TransformPoints(points);
                    lineFormatRenderer.DrawLine(points[0], points[1]);
                }
            }

            if (yari.MinorGridlinesLineFormat != null)
            {
                lineFormatRenderer = new LineFormatRenderer(gfx, yari.MinorGridlinesLineFormat);
                for (double y = yMin + yMinorTick; y < yMax; y += yMinorTick)
                {
                    points[0].X = xMin;
                    points[0].Y = y;
                    points[1].X = xMax;
                    points[1].Y = y;
                    matrix.TransformPoints(points);
                    lineFormatRenderer.DrawLine(points[0], points[1]);
                }
            }

            if (yari.MajorGridlinesLineFormat != null)
            {
                lineFormatRenderer = new LineFormatRenderer(gfx, yari.MajorGridlinesLineFormat);
                for (double y = yMin; y <= yMax; y += yMajorTick)
                {
                    points[0].X = xMin;
                    points[0].Y = y;
                    points[1].X = xMax;
                    points[1].Y = y;
                    matrix.TransformPoints(points);
                    lineFormatRenderer.DrawLine(points[0], points[1]);
                }
            }
        }
    }
}
