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
    /// Represents a plot area renderer of clustered bars, i. e. all bars are drawn side by side.
    /// </summary>
    internal class BarClusteredPlotAreaRenderer : BarPlotAreaRenderer
    {
        /// <summary>
        /// Initializes a new instance of the BarClusteredPlotAreaRenderer class with the
        /// specified renderer parameters.
        /// </summary>
        internal BarClusteredPlotAreaRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Calculates the position, width and height of each bar of all series.
        /// </summary>
        protected override void CalcBars()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;
            if (cri.seriesRendererInfos.Length == 0)
                return;

            double xMax = cri.xAxisRendererInfo.MaximumScale;
            double yMin = cri.yAxisRendererInfo.MinimumScale;
            double yMax = cri.yAxisRendererInfo.MaximumScale;

            int pointCount = 0;
            foreach (SeriesRendererInfo sri in cri.seriesRendererInfos)
                pointCount += sri._series._seriesElements.Count;

            // Space shared by one clustered bar.
            double groupWidth = cri.xAxisRendererInfo.MajorTick;

            // Space used by one bar.
            double columnWidth = groupWidth * 0.75 / cri.seriesRendererInfos.Length;

            int seriesIdx = 0;
            XPoint[] points = new XPoint[2];
            foreach (SeriesRendererInfo sri in cri.seriesRendererInfos)
            {
                // Set x to first clustered bar for each series.
                double x = xMax - groupWidth / 2;

                // Offset for bars of a particular series from the start of a clustered bar.
                double dx = (columnWidth * seriesIdx) - (columnWidth / 2 * cri.seriesRendererInfos.Length);
                double y0 = yMin;

                foreach (ColumnRendererInfo column in sri._pointRendererInfos)
                {
                    if (column.Point != null)
                    {
                        double x0 = x - dx;
                        double x1 = x - dx - columnWidth;
                        double y1 = column.Point.Value;

                        // Draw from zero base line, if it exists.
                        if (y0 < 0 && yMax >= 0)
                            y0 = 0;

                        // y0 should always be lower than y1, i. e. draw bar from bottom to top.
                        if (y1 < 0 && y1 < y0)
                        {
                            double y = y0;
                            y0 = y1;
                            y1 = y;
                        }

                        points[0].X = y0; // upper left
                        points[0].Y = x0;
                        points[1].X = y1; // lower right
                        points[1].Y = x1;

                        cri.plotAreaRendererInfo._matrix.TransformPoints(points);

                        column.Rect = new XRect(points[0].X,
                          points[1].Y,
                          points[1].X - points[0].X,
                          points[0].Y - points[1].Y);
                    }
                    x--; // Next clustered bar.
                }
                seriesIdx++;
            }
        }

        /// <summary>
        /// If yValue is within the range from yMin to yMax returns true, otherwise false.
        /// </summary>
        protected override bool IsDataInside(double yMin, double yMax, double yValue)
        {
            return yValue <= yMax && yValue >= yMin;
        }
    }
}
