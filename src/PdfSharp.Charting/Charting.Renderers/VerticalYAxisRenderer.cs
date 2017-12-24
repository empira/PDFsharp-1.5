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
    /// Represents a Y axis renderer used for charts of type Column2D or Line.
    /// </summary>
    internal class VerticalYAxisRenderer : YAxisRenderer
    {
        /// <summary>
        /// Initializes a new instance of the VerticalYAxisRenderer class with the
        /// specified renderer parameters.
        /// </summary>
        internal VerticalYAxisRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Returns a initialized rendererInfo based on the Y axis.
        /// </summary>
        internal override RendererInfo Init()
        {
            Chart chart = (Chart)_rendererParms.DrawingItem;
            XGraphics gfx = _rendererParms.Graphics;

            AxisRendererInfo yari = new AxisRendererInfo();
            yari._axis = chart._yAxis;
            InitScale(yari);
            if (yari._axis != null)
            {
                ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;
                InitTickLabels(yari, cri.DefaultFont);
                InitAxisTitle(yari, cri.DefaultFont);
                InitAxisLineFormat(yari);
                InitGridlines(yari);
            }
            return yari;
        }

        /// <summary>
        /// Calculates the space used for the Y axis.
        /// </summary>
        internal override void Format()
        {
            AxisRendererInfo yari = ((ChartRendererInfo)_rendererParms.RendererInfo).yAxisRendererInfo;
            if (yari._axis != null)
            {
                XGraphics gfx = _rendererParms.Graphics;

                XSize size = new XSize(0, 0);

                // height of all ticklabels
                double yMin = yari.MinimumScale;
                double yMax = yari.MaximumScale;
                double yMajorTick = yari.MajorTick;
                double lineHeight = Double.MinValue;
                XSize labelSize = new XSize(0, 0);
                for (double y = yMin; y <= yMax; y += yMajorTick)
                {
                    string str = y.ToString(yari.TickLabelsFormat);
                    labelSize = gfx.MeasureString(str, yari.TickLabelsFont);
                    size.Height += labelSize.Height;
                    size.Width = Math.Max(size.Width, labelSize.Width);
                    lineHeight = Math.Max(lineHeight, labelSize.Height);
                }

                // add space for tickmarks
                size.Width += yari.MajorTickMarkWidth * 1.5;

                // Measure axis title
                XSize titleSize = new XSize(0, 0);
                if (yari._axisTitleRendererInfo != null)
                {
                    RendererParameters parms = new RendererParameters();
                    parms.Graphics = gfx;
                    parms.RendererInfo = yari;
                    AxisTitleRenderer atr = new AxisTitleRenderer(parms);
                    atr.Format();
                    titleSize.Height = yari._axisTitleRendererInfo.Height;
                    titleSize.Width = yari._axisTitleRendererInfo.Width;
                }

                yari.Height = Math.Max(size.Height, titleSize.Height);
                yari.Width = size.Width + titleSize.Width;

                yari.InnerRect = yari.Rect;
                yari.InnerRect.Y += yari.TickLabelsFont.Height / 2;
                yari.LabelSize = labelSize;
            }
        }

        /// <summary>
        /// Draws the vertical Y axis.
        /// </summary>
        internal override void Draw()
        {
            AxisRendererInfo yari = ((ChartRendererInfo)_rendererParms.RendererInfo).yAxisRendererInfo;

            double yMin = yari.MinimumScale;
            double yMax = yari.MaximumScale;
            double yMajorTick = yari.MajorTick;
            double yMinorTick = yari.MinorTick;

            XMatrix matrix = new XMatrix();
            matrix.TranslatePrepend(-yari.InnerRect.X, yMax);
            matrix.Scale(1, yari.InnerRect.Height / (yMax - yMin), XMatrixOrder.Append);
            matrix.ScalePrepend(1, -1); // mirror horizontal
            matrix.Translate(yari.InnerRect.X, yari.InnerRect.Y, XMatrixOrder.Append);

            // Draw axis.
            // First draw tick marks, second draw axis.
            double majorTickMarkStart = 0, majorTickMarkEnd = 0,
                   minorTickMarkStart = 0, minorTickMarkEnd = 0;
            GetTickMarkPos(yari, ref majorTickMarkStart, ref majorTickMarkEnd, ref minorTickMarkStart, ref minorTickMarkEnd);

            XGraphics gfx = _rendererParms.Graphics;
            LineFormatRenderer lineFormatRenderer = new LineFormatRenderer(gfx, yari.LineFormat);
            LineFormatRenderer minorTickMarkLineFormat = new LineFormatRenderer(gfx, yari.MinorTickMarkLineFormat);
            LineFormatRenderer majorTickMarkLineFormat = new LineFormatRenderer(gfx, yari.MajorTickMarkLineFormat);
            XPoint[] points = new XPoint[2];

            // Draw minor tick marks.
            if (yari.MinorTickMark != TickMarkType.None)
            {
                for (double y = yMin + yMinorTick; y < yMax; y += yMinorTick)
                {
                    points[0].X = minorTickMarkStart;
                    points[0].Y = y;
                    points[1].X = minorTickMarkEnd;
                    points[1].Y = y;
                    matrix.TransformPoints(points);
                    minorTickMarkLineFormat.DrawLine(points[0], points[1]);
                }
            }

            double lineSpace = yari.TickLabelsFont.GetHeight(); // old: yari.TickLabelsFont.GetHeight(gfx);
            int cellSpace = yari.TickLabelsFont.FontFamily.GetLineSpacing(yari.TickLabelsFont.Style);
            double xHeight = yari.TickLabelsFont.Metrics.XHeight;

            XSize labelSize = new XSize(0, 0);
            labelSize.Height = lineSpace * xHeight / cellSpace;

            int countTickLabels = (int)((yMax - yMin) / yMajorTick) + 1;
            for (int i = 0; i < countTickLabels; ++i)
            {
                double y = yMin + yMajorTick * i;
                string str = y.ToString(yari.TickLabelsFormat);

                labelSize.Width = gfx.MeasureString(str, yari.TickLabelsFont).Width;

                // Draw major tick marks.
                if (yari.MajorTickMark != TickMarkType.None)
                {
                    labelSize.Width += yari.MajorTickMarkWidth * 1.5;
                    points[0].X = majorTickMarkStart;
                    points[0].Y = y;
                    points[1].X = majorTickMarkEnd;
                    points[1].Y = y;
                    matrix.TransformPoints(points);
                    majorTickMarkLineFormat.DrawLine(points[0], points[1]);
                }
                else
                    labelSize.Width += SpaceBetweenLabelAndTickmark;

                // Draw label text.
                XPoint[] layoutText = new XPoint[1];
                layoutText[0].X = yari.InnerRect.X + yari.InnerRect.Width - labelSize.Width;
                layoutText[0].Y = y;
                matrix.TransformPoints(layoutText);
                layoutText[0].Y += labelSize.Height / 2; // Center text vertically.
                gfx.DrawString(str, yari.TickLabelsFont, yari.TickLabelsBrush, layoutText[0]);
            }

            // Draw axis.
            if (yari.LineFormat != null && yari.LineFormat.Width > 0)
            {
                points[0].X = yari.InnerRect.X + yari.InnerRect.Width;
                points[0].Y = yMin;
                points[1].X = yari.InnerRect.X + yari.InnerRect.Width;
                points[1].Y = yMax;
                matrix.TransformPoints(points);
                if (yari.MajorTickMark != TickMarkType.None)
                {
                    // yMax is at the upper side of the axis
                    points[1].Y -= yari.LineFormat.Width / 2;
                    points[0].Y += yari.LineFormat.Width / 2;
                }
                lineFormatRenderer.DrawLine(points[0], points[1]);
            }

            // Draw axis title
            if (yari._axisTitleRendererInfo != null && yari._axisTitleRendererInfo.AxisTitleText != "")
            {
                RendererParameters parms = new RendererParameters();
                parms.Graphics = gfx;
                parms.RendererInfo = yari;
                double width = yari._axisTitleRendererInfo.Width;
                yari._axisTitleRendererInfo.Rect = yari.InnerRect;
                yari._axisTitleRendererInfo.Width = width;
                AxisTitleRenderer atr = new AxisTitleRenderer(parms);
                atr.Draw();
            }
        }

        /// <summary>
        /// Calculates all values necessary for scaling the axis like minimum/maximum scale or
        /// minor/major tick.
        /// </summary>
        private void InitScale(AxisRendererInfo rendererInfo)
        {
            double yMin, yMax;
            CalcYAxis(out yMin, out yMax);
            FineTuneYAxis(rendererInfo, yMin, yMax);

            rendererInfo.MajorTickMarkWidth = DefaultMajorTickMarkWidth;
            rendererInfo.MinorTickMarkWidth = DefaultMinorTickMarkWidth;
        }

        /// <summary>
        /// Gets the top and bottom position of the major and minor tick marks depending on the
        /// tick mark type.
        /// </summary>
        private void GetTickMarkPos(AxisRendererInfo rendererInfo,
                                    ref double majorTickMarkStart, ref double majorTickMarkEnd,
                                    ref double minorTickMarkStart, ref double minorTickMarkEnd)
        {
            double majorTickMarkWidth = rendererInfo.MajorTickMarkWidth;
            double minorTickMarkWidth = rendererInfo.MinorTickMarkWidth;
            XRect rect = rendererInfo.Rect;

            switch (rendererInfo.MajorTickMark)
            {
                case TickMarkType.Inside:
                    majorTickMarkStart = rect.X + rect.Width;
                    majorTickMarkEnd = rect.X + rect.Width + majorTickMarkWidth;
                    break;

                case TickMarkType.Outside:
                    majorTickMarkStart = rect.X + rect.Width;
                    majorTickMarkEnd = rect.X + rect.Width - majorTickMarkWidth;
                    break;

                case TickMarkType.Cross:
                    majorTickMarkStart = rect.X + rect.Width - majorTickMarkWidth;
                    majorTickMarkEnd = rect.X + rect.Width + majorTickMarkWidth;
                    break;

                //TickMarkType.None:
                default:
                    majorTickMarkStart = 0;
                    majorTickMarkEnd = 0;
                    break;
            }

            switch (rendererInfo.MinorTickMark)
            {
                case TickMarkType.Inside:
                    minorTickMarkStart = rect.X + rect.Width;
                    minorTickMarkEnd = rect.X + rect.Width + minorTickMarkWidth;
                    break;

                case TickMarkType.Outside:
                    minorTickMarkStart = rect.X + rect.Width;
                    minorTickMarkEnd = rect.X + rect.Width - minorTickMarkWidth;
                    break;

                case TickMarkType.Cross:
                    minorTickMarkStart = rect.X + rect.Width - minorTickMarkWidth;
                    minorTickMarkEnd = rect.X + rect.Width + minorTickMarkWidth;
                    break;

                //TickMarkType.None:
                default:
                    minorTickMarkStart = 0;
                    minorTickMarkEnd = 0;
                    break;
            }
        }

        /// <summary>
        /// Determines the smallest and the largest number from all series of the chart.
        /// </summary>
        protected virtual void CalcYAxis(out double yMin, out double yMax)
        {
            yMin = double.MaxValue;
            yMax = double.MinValue;

            foreach (Series series in ((Chart)_rendererParms.DrawingItem).SeriesCollection)
            {
                foreach (Point point in series.Elements)
                {
                    if (!double.IsNaN(point._value))
                    {
                        yMin = Math.Min(yMin, point.Value);
                        yMax = Math.Max(yMax, point.Value);
                    }
                }
            }
        }
    }
}
