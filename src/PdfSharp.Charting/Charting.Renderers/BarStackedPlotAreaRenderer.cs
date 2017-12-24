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

namespace PdfSharp.Charting.Renderers
{
    /// <summary>
    /// Represents a plot area renderer of stacked bars, i. e. all bars are drawn one on another.
    /// </summary>
    internal class BarStackedPlotAreaRenderer : BarPlotAreaRenderer
    {
        /// <summary>
        /// Initializes a new instance of the BarStackedPlotAreaRenderer class with the
        /// specified renderer parameters.
        /// </summary>
        internal BarStackedPlotAreaRenderer(RendererParameters parms)
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
            double xMajorTick = cri.xAxisRendererInfo.MajorTick;

            int maxPoints = 0;
            foreach (SeriesRendererInfo sri in cri.seriesRendererInfos)
                maxPoints = Math.Max(maxPoints, sri._series._seriesElements.Count);

            // Space used by one bar.
            double x = xMax - xMajorTick / 2;
            double columnWidth = xMajorTick * 0.75 / 2;

            XPoint[] points = new XPoint[2];
            for (int pointIdx = 0; pointIdx < maxPoints; ++pointIdx)
            {
                double yMin = 0, yMax = 0, y0 = 0, y1 = 0;
                double x0 = x - columnWidth;
                double x1 = x + columnWidth;

                foreach (SeriesRendererInfo sri in cri.seriesRendererInfos)
                {
                    if (sri._pointRendererInfos.Length <= pointIdx)
                        break;

                    ColumnRendererInfo column = (ColumnRendererInfo)sri._pointRendererInfos[pointIdx];
                    if (column.Point != null && !double.IsNaN(column.Point._value))
                    {
                        double y = column.Point._value;
                        if (y < 0)
                        {
                            y0 = yMin + y;
                            y1 = yMin;
                            yMin += y;
                        }
                        else
                        {
                            y0 = yMax;
                            y1 = yMax + y;
                            yMax += y;
                        }

                        points[0].Y = x0; // oben links
                        points[0].X = y0;
                        points[1].Y = x1; // unten rechts
                        points[1].X = y1;

                        cri.plotAreaRendererInfo._matrix.TransformPoints(points);

                        column.Rect = new XRect(points[0].X,
                                                points[0].Y,
                                                points[1].X - points[0].X,
                                                points[1].Y - points[0].Y);
                    }
                }
                x--; // Next stacked column.
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
