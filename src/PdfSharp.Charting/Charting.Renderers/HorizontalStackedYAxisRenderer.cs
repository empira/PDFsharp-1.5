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

namespace PdfSharp.Charting.Renderers
{
    /// <summary>
    /// Represents a Y axis renderer used for charts of type BarStacked2D.
    /// </summary>
    internal class HorizontalStackedYAxisRenderer : HorizontalYAxisRenderer
    {
        /// <summary>
        /// Initializes a new instance of the HorizontalStackedYAxisRenderer class with the
        /// specified renderer parameters.
        /// </summary>
        internal HorizontalStackedYAxisRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Determines the sum of the smallest and the largest stacked bar
        /// from all series of the chart.
        /// </summary>
        protected override void CalcYAxis(out double yMin, out double yMax)
        {
            yMin = double.MaxValue;
            yMax = double.MinValue;

            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;

            int maxPoints = 0;
            foreach (SeriesRendererInfo sri in cri.seriesRendererInfos)
                maxPoints = Math.Max(maxPoints, sri._series._seriesElements.Count);

            for (int pointIdx = 0; pointIdx < maxPoints; ++pointIdx)
            {
                double valueSumPos = 0, valueSumNeg = 0;
                foreach (SeriesRendererInfo sri in cri.seriesRendererInfos)
                {
                    if (sri._pointRendererInfos.Length <= pointIdx)
                        break;

                    ColumnRendererInfo column = (ColumnRendererInfo)sri._pointRendererInfos[pointIdx];
                    if (column.Point != null && !double.IsNaN(column.Point._value))
                    {
                        if (column.Point._value < 0)
                            valueSumNeg += column.Point._value;
                        else
                            valueSumPos += column.Point._value;
                    }
                }
                yMin = Math.Min(valueSumNeg, yMin);
                yMax = Math.Max(valueSumPos, yMax);
            }
        }
    }
}
