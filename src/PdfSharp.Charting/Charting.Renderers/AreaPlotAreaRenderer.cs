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
    /// Represents a plot area renderer of areas.
    /// </summary>
    internal class AreaPlotAreaRenderer : ColumnLikePlotAreaRenderer
    {
        /// <summary>
        /// Initializes a new instance of the AreaPlotAreaRenderer class
        /// with the specified renderer parameters.
        /// </summary>
        internal AreaPlotAreaRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Draws the content of the area plot area.
        /// </summary>
        internal override void Draw()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;
            XRect plotAreaRect = cri.plotAreaRendererInfo.Rect;
            if (plotAreaRect.IsEmpty)
                return;

            XGraphics gfx = _rendererParms.Graphics;
            XGraphicsState state = gfx.Save();
            //gfx.SetClip(plotAreaRect, XCombineMode.Intersect);
            gfx.IntersectClip(plotAreaRect);

            XMatrix matrix = cri.plotAreaRendererInfo._matrix;
            double xMajorTick = cri.xAxisRendererInfo.MajorTick;
            foreach (SeriesRendererInfo sri in cri.seriesRendererInfos)
            {
                int count = sri._series.Elements.Count;
                XPoint[] points = new XPoint[count + 2];
                points[0] = new XPoint(xMajorTick / 2, 0);
                for (int idx = 0; idx < count; idx++)
                {
                    double pointValue = sri._series.Elements[idx].Value;
                    if (double.IsNaN(pointValue))
                        pointValue = 0;
                    points[idx + 1] = new XPoint(idx + xMajorTick / 2, pointValue);
                }
                points[count + 1] = new XPoint(count - 1 + xMajorTick / 2, 0);
                matrix.TransformPoints(points);
                gfx.DrawPolygon(sri.LineFormat, sri.FillFormat, points, XFillMode.Winding);
            }

            //gfx.ResetClip();
            gfx.Restore(state);
        }
    }
}
