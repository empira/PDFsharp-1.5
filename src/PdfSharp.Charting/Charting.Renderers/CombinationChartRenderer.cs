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
using System.Collections.Generic;
using PdfSharp.Drawing;

namespace PdfSharp.Charting.Renderers
{
    /// <summary>
    /// Represents a renderer for combinations of charts.
    /// </summary>
    internal class CombinationChartRenderer : ChartRenderer
    {
        /// <summary>
        /// Initializes a new instance of the CombinationChartRenderer class with the
        /// specified renderer parameters.
        /// </summary>
        internal CombinationChartRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Returns an initialized and renderer specific rendererInfo.
        /// </summary>
        internal override RendererInfo Init()
        {
            CombinationRendererInfo cri = new CombinationRendererInfo();
            cri._chart = (Chart)_rendererParms.DrawingItem;
            _rendererParms.RendererInfo = cri;

            InitSeriesRendererInfo();
            DistributeSeries();

            if (cri._areaSeriesRendererInfos != null)
            {
                cri.seriesRendererInfos = cri._areaSeriesRendererInfos;
                AreaChartRenderer renderer = new AreaChartRenderer(_rendererParms);
                renderer.InitSeries();
            }
            if (cri._columnSeriesRendererInfos != null)
            {
                cri.seriesRendererInfos = cri._columnSeriesRendererInfos;
                ColumnChartRenderer renderer = new ColumnChartRenderer(_rendererParms);
                renderer.InitSeries();
            }
            if (cri._lineSeriesRendererInfos != null)
            {
                cri.seriesRendererInfos = cri._lineSeriesRendererInfos;
                LineChartRenderer renderer = new LineChartRenderer(_rendererParms);
                renderer.InitSeries();
            }
            cri.seriesRendererInfos = cri._commonSeriesRendererInfos;

            LegendRenderer lr = new ColumnLikeLegendRenderer(_rendererParms);
            cri.legendRendererInfo = (LegendRendererInfo)lr.Init();

            AxisRenderer xar = new HorizontalXAxisRenderer(_rendererParms);
            cri.xAxisRendererInfo = (AxisRendererInfo)xar.Init();

            AxisRenderer yar = new VerticalYAxisRenderer(_rendererParms);
            cri.yAxisRendererInfo = (AxisRendererInfo)yar.Init();

            PlotArea plotArea = cri._chart.PlotArea;
            PlotAreaRenderer apar = new AreaPlotAreaRenderer(_rendererParms);
            cri.plotAreaRendererInfo = (PlotAreaRendererInfo)apar.Init();

            // Draw data labels.
            if (cri._columnSeriesRendererInfos != null)
            {
                cri.seriesRendererInfos = cri._columnSeriesRendererInfos;
                DataLabelRenderer dlr = new ColumnDataLabelRenderer(_rendererParms);
                dlr.Init();
            }

            return cri;
        }

        /// <summary>
        /// Layouts and calculates the space used by the combination chart.
        /// </summary>
        internal override void Format()
        {
            CombinationRendererInfo cri = (CombinationRendererInfo)_rendererParms.RendererInfo;
            cri.seriesRendererInfos = cri._commonSeriesRendererInfos;

            LegendRenderer lr = new ColumnLikeLegendRenderer(_rendererParms);
            lr.Format();

            // axes
            AxisRenderer xar = new HorizontalXAxisRenderer(_rendererParms);
            xar.Format();

            AxisRenderer yar = new VerticalYAxisRenderer(_rendererParms);
            yar.Format();

            // Calculate rects and positions.
            XRect chartRect = LayoutLegend();
            cri.xAxisRendererInfo.X = chartRect.Left + cri.yAxisRendererInfo.Width;
            cri.xAxisRendererInfo.Y = chartRect.Bottom - cri.xAxisRendererInfo.Height;
            cri.xAxisRendererInfo.Width = chartRect.Width - cri.yAxisRendererInfo.Width;
            cri.yAxisRendererInfo.X = chartRect.Left;
            cri.yAxisRendererInfo.Y = chartRect.Top;
            cri.yAxisRendererInfo.Height = chartRect.Height - cri.xAxisRendererInfo.Height;
            cri.plotAreaRendererInfo.X = cri.xAxisRendererInfo.X;
            cri.plotAreaRendererInfo.Y = cri.yAxisRendererInfo.InnerRect.Y;
            cri.plotAreaRendererInfo.Width = cri.xAxisRendererInfo.Width;
            cri.plotAreaRendererInfo.Height = cri.yAxisRendererInfo.InnerRect.Height;

            // Calculated remaining plot area, now it's safe to format.
            PlotAreaRenderer renderer;
            if (cri._areaSeriesRendererInfos != null)
            {
                cri.seriesRendererInfos = cri._areaSeriesRendererInfos;
                renderer = new AreaPlotAreaRenderer(_rendererParms);
                renderer.Format();
            }
            if (cri._columnSeriesRendererInfos != null)
            {
                cri.seriesRendererInfos = cri._columnSeriesRendererInfos;
                //TODO Check for Clustered- or StackedPlotAreaRenderer
                renderer = new ColumnClusteredPlotAreaRenderer(_rendererParms);
                renderer.Format();
            }
            if (cri._lineSeriesRendererInfos != null)
            {
                cri.seriesRendererInfos = cri._lineSeriesRendererInfos;
                renderer = new LinePlotAreaRenderer(_rendererParms);
                renderer.Format();
            }

            // Draw data labels.
            if (cri._columnSeriesRendererInfos != null)
            {
                cri.seriesRendererInfos = cri._columnSeriesRendererInfos;
                DataLabelRenderer dlr = new ColumnDataLabelRenderer(_rendererParms);
                dlr.Format();
            }
        }

        /// <summary>
        /// Draws the column chart.
        /// </summary>
        internal override void Draw()
        {
            CombinationRendererInfo cri = (CombinationRendererInfo)_rendererParms.RendererInfo;
            cri.seriesRendererInfos = cri._commonSeriesRendererInfos;

            LegendRenderer lr = new ColumnLikeLegendRenderer(_rendererParms);
            lr.Draw();

            WallRenderer wr = new WallRenderer(_rendererParms);
            wr.Draw();

            GridlinesRenderer glr = new ColumnLikeGridlinesRenderer(_rendererParms);
            glr.Draw();

            PlotAreaBorderRenderer pabr = new PlotAreaBorderRenderer(_rendererParms);
            pabr.Draw();

            PlotAreaRenderer renderer;
            if (cri._areaSeriesRendererInfos != null)
            {
                cri.seriesRendererInfos = cri._areaSeriesRendererInfos;
                renderer = new AreaPlotAreaRenderer(_rendererParms);
                renderer.Draw();
            }
            if (cri._columnSeriesRendererInfos != null)
            {
                cri.seriesRendererInfos = cri._columnSeriesRendererInfos;
                //TODO Check for Clustered- or StackedPlotAreaRenderer
                renderer = new ColumnClusteredPlotAreaRenderer(_rendererParms);
                renderer.Draw();
            }
            if (cri._lineSeriesRendererInfos != null)
            {
                cri.seriesRendererInfos = cri._lineSeriesRendererInfos;
                renderer = new LinePlotAreaRenderer(_rendererParms);
                renderer.Draw();
            }

            // Draw data labels.
            if (cri._columnSeriesRendererInfos != null)
            {
                cri.seriesRendererInfos = cri._columnSeriesRendererInfos;
                DataLabelRenderer dlr = new ColumnDataLabelRenderer(_rendererParms);
                dlr.Draw();
            }

            // Draw axes.
            cri.seriesRendererInfos = cri._commonSeriesRendererInfos;
            if (cri.xAxisRendererInfo._axis != null)
            {
                AxisRenderer xar = new HorizontalXAxisRenderer(_rendererParms);
                xar.Draw();
            }
            if (cri.yAxisRendererInfo._axis != null)
            {
                AxisRenderer yar = new VerticalYAxisRenderer(_rendererParms);
                yar.Draw();
            }
        }

        /// <summary>
        /// Initializes all necessary data to draw series for a combination chart.
        /// </summary>
        private void InitSeriesRendererInfo()
        {
            CombinationRendererInfo cri = (CombinationRendererInfo)_rendererParms.RendererInfo;
            SeriesCollection seriesColl = cri._chart.SeriesCollection;
            cri.seriesRendererInfos = new SeriesRendererInfo[seriesColl.Count];
            for (int idx = 0; idx < seriesColl.Count; ++idx)
            {
                SeriesRendererInfo sri = new SeriesRendererInfo();
                sri._series = seriesColl[idx];
                cri.seriesRendererInfos[idx] = sri;
            }
        }

        /// <summary>
        /// Sort all series renderer info dependent on their chart type.
        /// </summary>
        private void DistributeSeries()
        {
            CombinationRendererInfo cri = (CombinationRendererInfo)_rendererParms.RendererInfo;

            List<SeriesRendererInfo> areaSeries = new List<SeriesRendererInfo>();
            List<SeriesRendererInfo> columnSeries = new List<SeriesRendererInfo>();
            List<SeriesRendererInfo> lineSeries = new List<SeriesRendererInfo>();
            foreach (SeriesRendererInfo sri in cri.seriesRendererInfos)
            {
                switch (sri._series._chartType)
                {
                    case ChartType.Area2D:
                        areaSeries.Add(sri);
                        break;

                    case ChartType.Column2D:
                        columnSeries.Add(sri);
                        break;

                    case ChartType.Line:
                        lineSeries.Add(sri);
                        break;

                    default:
                        throw new InvalidOperationException(PSCSR.InvalidChartTypeForCombination(sri._series._chartType));
                }
            }

            cri._commonSeriesRendererInfos = cri.seriesRendererInfos;
            if (areaSeries.Count > 0)
            {
                cri._areaSeriesRendererInfos = new SeriesRendererInfo[areaSeries.Count];
                areaSeries.CopyTo(cri._areaSeriesRendererInfos);
            }
            if (columnSeries.Count > 0)
            {
                cri._columnSeriesRendererInfos = new SeriesRendererInfo[columnSeries.Count];
                columnSeries.CopyTo(cri._columnSeriesRendererInfos);
            }
            if (lineSeries.Count > 0)
            {
                cri._lineSeriesRendererInfos = new SeriesRendererInfo[lineSeries.Count];
                lineSeries.CopyTo(cri._lineSeriesRendererInfos);
            }
        }
    }
}