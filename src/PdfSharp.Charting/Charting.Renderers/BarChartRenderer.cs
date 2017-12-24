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
    /// Represents a bar chart renderer.
    /// </summary>
    internal class BarChartRenderer : ChartRenderer
    {
        /// <summary>
        /// Initializes a new instance of the BarChartRenderer class with the
        /// specified renderer parameters.
        /// </summary>
        internal BarChartRenderer(RendererParameters parms)
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

            InitSeriesRendererInfo();

            LegendRenderer lr = GetLegendRenderer();
            cri.legendRendererInfo = (LegendRendererInfo)lr.Init();

            AxisRenderer xar = new VerticalXAxisRenderer(_rendererParms);
            cri.xAxisRendererInfo = (AxisRendererInfo)xar.Init();

            AxisRenderer yar = GetYAxisRenderer();
            cri.yAxisRendererInfo = (AxisRendererInfo)yar.Init();

            PlotArea plotArea = cri._chart.PlotArea;
            PlotAreaRenderer renderer = GetPlotAreaRenderer();
            cri.plotAreaRendererInfo = (PlotAreaRendererInfo)renderer.Init();

            DataLabelRenderer dlr = new BarDataLabelRenderer(_rendererParms);
            dlr.Init();

            return cri;
        }

        /// <summary>
        /// Layouts and calculates the space used by the column chart.
        /// </summary>
        internal override void Format()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;

            LegendRenderer lr = GetLegendRenderer();
            lr.Format();

            // axes
            AxisRenderer xar = new VerticalXAxisRenderer(_rendererParms);
            xar.Format();

            AxisRenderer yar = GetYAxisRenderer();
            yar.Format();

            // Calculate rects and positions.
            XRect chartRect = LayoutLegend();
            cri.xAxisRendererInfo.X = chartRect.Left;
            cri.xAxisRendererInfo.Y = chartRect.Top;
            cri.xAxisRendererInfo.Height = chartRect.Height - cri.yAxisRendererInfo.Height;
            cri.yAxisRendererInfo.X = chartRect.Left + cri.xAxisRendererInfo.Width;
            cri.yAxisRendererInfo.Y = chartRect.Bottom - cri.yAxisRendererInfo.Height;
            cri.yAxisRendererInfo.Width = chartRect.Width - cri.xAxisRendererInfo.Width;
            cri.plotAreaRendererInfo.X = cri.yAxisRendererInfo.X;
            cri.plotAreaRendererInfo.Y = cri.xAxisRendererInfo.Y;
            cri.plotAreaRendererInfo.Width = cri.yAxisRendererInfo.InnerRect.Width;
            cri.plotAreaRendererInfo.Height = cri.xAxisRendererInfo.Height;

            // Calculated remaining plot area, now it's safe to format.
            PlotAreaRenderer renderer = GetPlotAreaRenderer();
            renderer.Format();

            DataLabelRenderer dlr = new BarDataLabelRenderer(_rendererParms);
            dlr.Format();
        }

        /// <summary>
        /// Draws the column chart.
        /// </summary>
        internal override void Draw()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;

            LegendRenderer lr = GetLegendRenderer();
            lr.Draw();

            WallRenderer wr = new WallRenderer(_rendererParms);
            wr.Draw();

            GridlinesRenderer glr = new BarGridlinesRenderer(_rendererParms);
            glr.Draw();

            PlotAreaBorderRenderer pabr = new PlotAreaBorderRenderer(_rendererParms);
            pabr.Draw();

            PlotAreaRenderer renderer = GetPlotAreaRenderer();
            renderer.Draw();

            DataLabelRenderer dlr = new BarDataLabelRenderer(_rendererParms);
            dlr.Draw();

            if (cri.xAxisRendererInfo._axis != null)
            {
                AxisRenderer xar = new VerticalXAxisRenderer(_rendererParms);
                xar.Draw();
            }

            if (cri.yAxisRendererInfo._axis != null)
            {
                AxisRenderer yar = GetYAxisRenderer();
                yar.Draw();
            }
        }

        /// <summary>
        /// Returns the specific plot area renderer.
        /// </summary>
        private PlotAreaRenderer GetPlotAreaRenderer()
        {
            Chart chart = (Chart)_rendererParms.DrawingItem;
            switch (chart._type)
            {
                case ChartType.Bar2D:
                    return new BarClusteredPlotAreaRenderer(_rendererParms);

                case ChartType.BarStacked2D:
                    return new BarStackedPlotAreaRenderer(_rendererParms);
            }
            return null;
        }

        /// <summary>
        /// Returns the specific legend renderer.
        /// </summary>
        private LegendRenderer GetLegendRenderer()
        {
            Chart chart = (Chart)_rendererParms.DrawingItem;
            switch (chart._type)
            {
                case ChartType.Bar2D:
                    return new BarClusteredLegendRenderer(_rendererParms);

                case ChartType.BarStacked2D:
                    return new ColumnLikeLegendRenderer(_rendererParms);
            }
            return null;
        }

        /// <summary>
        /// Returns the specific plot area renderer.
        /// </summary>
        private YAxisRenderer GetYAxisRenderer()
        {
            Chart chart = (Chart)_rendererParms.DrawingItem;
            switch (chart._type)
            {
                case ChartType.Bar2D:
                    return new HorizontalYAxisRenderer(_rendererParms);

                case ChartType.BarStacked2D:
                    return new HorizontalStackedYAxisRenderer(_rendererParms);
            }
            return null;
        }

        /// <summary>
        /// Initializes all necessary data to draw all series for a column chart.
        /// </summary>
        private void InitSeriesRendererInfo()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;

            SeriesCollection seriesColl = cri._chart.SeriesCollection;
            cri.seriesRendererInfos = new SeriesRendererInfo[seriesColl.Count];
            // Lowest series is the first, like in Excel 
            for (int idx = 0; idx < seriesColl.Count; ++idx)
            {
                SeriesRendererInfo sri = new SeriesRendererInfo();
                sri._series = seriesColl[idx];
                cri.seriesRendererInfos[idx] = sri;
            }

            InitSeries();
        }

        /// <summary>
        /// Initializes all necessary data to draw all series for a column chart.
        /// </summary>
        internal void InitSeries()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;

            int seriesIndex = 0;
            foreach (SeriesRendererInfo sri in cri.seriesRendererInfos)
            {
                sri.LineFormat = Converter.ToXPen(sri._series._lineFormat, XColors.Black, DefaultSeriesLineWidth);
                sri.FillFormat = Converter.ToXBrush(sri._series._fillFormat, ColumnColors.Item(seriesIndex++));

                sri._pointRendererInfos = new ColumnRendererInfo[sri._series._seriesElements.Count];
                for (int pointIdx = 0; pointIdx < sri._pointRendererInfos.Length; ++pointIdx)
                {
                    PointRendererInfo pri = new ColumnRendererInfo();
                    Point point = sri._series._seriesElements[pointIdx];
                    pri.Point = point;
                    if (point != null)
                    {
                        pri.LineFormat = sri.LineFormat;
                        pri.FillFormat = sri.FillFormat;
                        if (point._lineFormat != null && !point._lineFormat._color.IsEmpty)
                            pri.LineFormat = Converter.ToXPen(point._lineFormat, sri.LineFormat);
                        if (point._fillFormat != null && !point._fillFormat._color.IsEmpty)
                            pri.FillFormat = new XSolidBrush(point._fillFormat._color);
                    }
                    sri._pointRendererInfos[pointIdx] = pri;
                }
            }
        }
    }
}