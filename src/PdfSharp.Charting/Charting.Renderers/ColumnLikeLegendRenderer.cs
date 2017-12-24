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
    /// Represents the legend renderer specific to charts like column, line, or bar.
    /// </summary>
    internal class ColumnLikeLegendRenderer : LegendRenderer
    {
        /// <summary>
        /// Initializes a new instance of the ColumnLikeLegendRenderer class with the
        /// specified renderer parameters.
        /// </summary>
        internal ColumnLikeLegendRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Initializes the legend's renderer info. Each data series will be represented through
        /// a legend entry renderer info.
        /// </summary>
        internal override RendererInfo Init()
        {
            LegendRendererInfo lri = null;
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;
            if (cri._chart._legend != null)
            {
                lri = new LegendRendererInfo();
                lri._legend = cri._chart._legend;

                lri.Font = Converter.ToXFont(lri._legend._font, cri.DefaultFont);
                lri.FontColor = new XSolidBrush(XColors.Black);

                if (lri._legend._lineFormat != null)
                    lri.BorderPen = Converter.ToXPen(lri._legend._lineFormat, XColors.Black, DefaultLineWidth, XDashStyle.Solid);

                lri.Entries = new LegendEntryRendererInfo[cri.seriesRendererInfos.Length];
                int index = 0;
                foreach (SeriesRendererInfo sri in cri.seriesRendererInfos)
                {
                    LegendEntryRendererInfo leri = new LegendEntryRendererInfo();
                    leri._seriesRendererInfo = sri;
                    leri._legendRendererInfo = lri;
                    leri.EntryText = sri._series.Name;
                    if (sri._markerRendererInfo != null)
                    {
                        leri.MarkerSize.Width = leri.MarkerSize.Height = sri._markerRendererInfo.MarkerSize.Point;
                        leri.MarkerPen = new XPen(sri._markerRendererInfo.MarkerForegroundColor);
                        leri.MarkerBrush = new XSolidBrush(sri._markerRendererInfo.MarkerBackgroundColor);
                    }
                    else
                    {
                        leri.MarkerPen = sri.LineFormat;
                        leri.MarkerBrush = sri.FillFormat;
                    }

                    if (cri._chart._type == ChartType.ColumnStacked2D)
                        // Stacked columns are in reverse order.
                        lri.Entries[cri.seriesRendererInfos.Length - index++ - 1] = leri;
                    else
                        lri.Entries[index++] = leri;
                }
            }
            return lri;
        }
    }
}
