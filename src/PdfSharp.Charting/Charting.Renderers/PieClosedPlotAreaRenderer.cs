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
    /// Represents a closed pie plot area renderer.
    /// </summary>
    internal class PieClosedPlotAreaRenderer : PiePlotAreaRenderer
    {
        /// <summary>
        /// Initializes a new instance of the PiePlotAreaRenderer class
        /// with the specified renderer parameters.
        /// </summary>
        internal PieClosedPlotAreaRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Calculate angles for each sector.
        /// </summary>
        protected override void CalcSectors()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;
            if (cri.seriesRendererInfos.Length == 0)
                return;

            SeriesRendererInfo sri = cri.seriesRendererInfos[0];

            double sumValues = sri.SumOfPoints;
            if (sumValues == 0)
                return;

            double textMeasure = 0;
            if (sri._dataLabelRendererInfo != null && sri._dataLabelRendererInfo.Position == DataLabelPosition.OutsideEnd)
            {
                foreach (DataLabelEntryRendererInfo dleri in sri._dataLabelRendererInfo.Entries)
                {
                    textMeasure = Math.Max(textMeasure, dleri.Width);
                    textMeasure = Math.Max(textMeasure, dleri.Height);
                }
            }

            XRect pieRect = cri.plotAreaRendererInfo.Rect;
            if (textMeasure != 0)
            {
                pieRect.X += textMeasure;
                pieRect.Y += textMeasure;
                pieRect.Width -= 2 * textMeasure;
                pieRect.Height -= 2 * textMeasure;
            }

            double startAngle = 270, sweepAngle = 0;
            foreach (SectorRendererInfo sector in sri._pointRendererInfos)
            {
                if (!double.IsNaN(sector.Point._value) && sector.Point._value != 0)
                {
                    sweepAngle = 360 / (sumValues / Math.Abs(sector.Point._value));

                    sector.Rect = pieRect;
                    sector.StartAngle = startAngle;
                    sector.SweepAngle = sweepAngle;

                    startAngle += sweepAngle;
                }
                else
                {
                    sector.StartAngle = double.NaN;
                    sector.SweepAngle = double.NaN;
                }
            }
        }
    }
}
