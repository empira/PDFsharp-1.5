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
    /// Represents a pie chart renderer.
    /// </summary>
    internal class PieChartRenderer : ChartRenderer
    {
        /// <summary>
        /// Initializes a new instance of the PieChartRenderer class with the
        /// specified renderer parameters.
        /// </summary>
        internal PieChartRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Returns an initialized and renderer specific rendererInfo.
        /// </summary>
        internal override RendererInfo Init()
        {
            ChartRendererInfo cri = new ChartRendererInfo();
            cri._chart = (Chart)_rendererParms.DrawingItem;
            _rendererParms.RendererInfo = cri;

            InitSeries(cri);

            LegendRenderer lr = new PieLegendRenderer(_rendererParms);
            cri.legendRendererInfo = (LegendRendererInfo)lr.Init();

            PlotArea plotArea = cri._chart.PlotArea;
            PlotAreaRenderer renderer = GetPlotAreaRenderer();
            cri.plotAreaRendererInfo = (PlotAreaRendererInfo)renderer.Init();

            DataLabelRenderer dlr = new PieDataLabelRenderer(_rendererParms);
            dlr.Init();

            return cri;
        }

        /// <summary>
        /// Layouts and calculates the space used by the pie chart.
        /// </summary>
        internal override void Format()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;

            LegendRenderer lr = new PieLegendRenderer(_rendererParms);
            lr.Format();

            // Calculate rects and positions.
            XRect chartRect = LayoutLegend();
            cri.plotAreaRendererInfo.Rect = chartRect;
            double edge = Math.Min(chartRect.Width, chartRect.Height);
            cri.plotAreaRendererInfo.X += (chartRect.Width - edge) / 2;
            cri.plotAreaRendererInfo.Y += (chartRect.Height - edge) / 2;
            cri.plotAreaRendererInfo.Width = edge;
            cri.plotAreaRendererInfo.Height = edge;

            DataLabelRenderer dlr = new PieDataLabelRenderer(_rendererParms);
            dlr.Format();

            // Calculated remaining plot area, now it's safe to format.
            PlotAreaRenderer renderer = GetPlotAreaRenderer();
            renderer.Format();

            dlr.CalcPositions();
        }

        /// <summary>
        /// Draws the pie chart.
        /// </summary>
        internal override void Draw()
        {
            LegendRenderer lr = new PieLegendRenderer(_rendererParms);
            lr.Draw();

            WallRenderer wr = new WallRenderer(_rendererParms);
            wr.Draw();

            PlotAreaBorderRenderer pabr = new PlotAreaBorderRenderer(_rendererParms);
            pabr.Draw();

            PlotAreaRenderer renderer = GetPlotAreaRenderer();
            renderer.Draw();

            DataLabelRenderer dlr = new PieDataLabelRenderer(_rendererParms);
            dlr.Draw();
        }

        /// <summary>
        /// Returns the specific plot area renderer.
        /// </summary>
        private PlotAreaRenderer GetPlotAreaRenderer()
        {
            Chart chart = (Chart)_rendererParms.DrawingItem;
            switch (chart._type)
            {
                case ChartType.Pie2D:
                    return new PieClosedPlotAreaRenderer(_rendererParms);

                case ChartType.PieExploded2D:
                    return new PieExplodedPlotAreaRenderer(_rendererParms);
            }
            return null;
        }

        /// <summary>
        /// Initializes all necessary data to draw a series for a pie chart.
        /// </summary>
        protected void InitSeries(ChartRendererInfo rendererInfo)
        {
            SeriesCollection seriesColl = rendererInfo._chart.SeriesCollection;
            rendererInfo.seriesRendererInfos = new SeriesRendererInfo[seriesColl.Count];
            for (int idx = 0; idx < seriesColl.Count; ++idx)
            {
                SeriesRendererInfo sri = new SeriesRendererInfo();
                rendererInfo.seriesRendererInfos[idx] = sri;
                sri._series = seriesColl[idx];

                sri.LineFormat = Converter.ToXPen(sri._series._lineFormat, XColors.Black, ChartRenderer.DefaultSeriesLineWidth);
                sri.FillFormat = Converter.ToXBrush(sri._series._fillFormat, ColumnColors.Item(idx));

                sri._pointRendererInfos = new SectorRendererInfo[sri._series._seriesElements.Count];
                for (int pointIdx = 0; pointIdx < sri._pointRendererInfos.Length; ++pointIdx)
                {
                    PointRendererInfo pri = new SectorRendererInfo();
                    Point point = sri._series._seriesElements[pointIdx];
                    pri.Point = point;
                    if (point != null)
                    {
                        pri.LineFormat = sri.LineFormat;
                        if (point._lineFormat != null && !point._lineFormat._color.IsEmpty)
                            pri.LineFormat = new XPen(point._lineFormat._color);
                        if (point._fillFormat != null && !point._fillFormat._color.IsEmpty)
                            pri.FillFormat = new XSolidBrush(point._fillFormat._color);
                        else
                            pri.FillFormat = new XSolidBrush(PieColors.Item(pointIdx));
                        pri.LineFormat.LineJoin = XLineJoin.Round;
                    }
                    sri._pointRendererInfos[pointIdx] = pri;
                }
            }
        }
    }
}
