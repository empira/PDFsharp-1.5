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
    /// Represents a data label renderer for bar charts.
    /// </summary>
    internal class BarDataLabelRenderer : DataLabelRenderer
    {
        /// <summary>
        /// Initializes a new instance of the BarDataLabelRenderer class with the
        /// specified renderer parameters.
        /// </summary>
        internal BarDataLabelRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Calculates the space used by the data labels.
        /// </summary>
        internal override void Format()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;
            foreach (SeriesRendererInfo sri in cri.seriesRendererInfos)
            {
                if (sri._dataLabelRendererInfo == null)
                    continue;

                XGraphics gfx = _rendererParms.Graphics;

                sri._dataLabelRendererInfo.Entries = new DataLabelEntryRendererInfo[sri._pointRendererInfos.Length];
                int index = 0;
                foreach (ColumnRendererInfo column in sri._pointRendererInfos)
                {
                    DataLabelEntryRendererInfo dleri = new DataLabelEntryRendererInfo();
                    if (sri._dataLabelRendererInfo.Type != DataLabelType.None)
                    {
                        if (sri._dataLabelRendererInfo.Type == DataLabelType.Value)
                            dleri.Text = column.Point._value.ToString(sri._dataLabelRendererInfo.Format);
                        else if (sri._dataLabelRendererInfo.Type == DataLabelType.Percent)
                            throw new InvalidOperationException(PSCSR.PercentNotSupportedByColumnDataLabel);

                        if (dleri.Text.Length > 0)
                            dleri.Size = gfx.MeasureString(dleri.Text, sri._dataLabelRendererInfo.Font);
                    }

                    sri._dataLabelRendererInfo.Entries[index++] = dleri;
                }
            }

            CalcPositions();
        }

        /// <summary>
        /// Draws the data labels of the bar chart.
        /// </summary>
        internal override void Draw()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;

            foreach (SeriesRendererInfo sri in cri.seriesRendererInfos)
            {
                if (sri._dataLabelRendererInfo == null)
                    continue;

                XGraphics gfx = _rendererParms.Graphics;
                XFont font = sri._dataLabelRendererInfo.Font;
                XBrush fontColor = sri._dataLabelRendererInfo.FontColor;
                XStringFormat format = XStringFormats.Center;
                format.LineAlignment = XLineAlignment.Center;
                foreach (DataLabelEntryRendererInfo dataLabel in sri._dataLabelRendererInfo.Entries)
                {
                    if (dataLabel.Text != null)
                        gfx.DrawString(dataLabel.Text, font, fontColor, dataLabel.Rect, format);
                }
            }
        }

        /// <summary>
        /// Calculates the data label positions specific for column charts.
        /// </summary>
        internal override void CalcPositions()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;
            XGraphics gfx = _rendererParms.Graphics;

            foreach (SeriesRendererInfo sri in cri.seriesRendererInfos)
            {
                if (sri._dataLabelRendererInfo == null)
                    continue;

                int columnIndex = 0;
                foreach (ColumnRendererInfo bar in sri._pointRendererInfos)
                {
                    DataLabelEntryRendererInfo dleri = sri._dataLabelRendererInfo.Entries[columnIndex++];

                    dleri.Y = bar.Rect.Y + (bar.Rect.Height - dleri.Height) / 2; // Always the same...
                    switch (sri._dataLabelRendererInfo.Position)
                    {
                        case DataLabelPosition.InsideEnd:
                            // Inner border of the column.
                            dleri.X = bar.Rect.X;
                            if (bar.Point._value > 0)
                                dleri.X += bar.Rect.Width - dleri.Width;
                            break;

                        case DataLabelPosition.Center:
                            // Centered inside the column.
                            dleri.X = bar.Rect.X + (bar.Rect.Width - dleri.Width) / 2;
                            break;

                        case DataLabelPosition.InsideBase:
                            // Aligned at the base of the column.
                            dleri.X = bar.Rect.X;
                            if (bar.Point._value < 0)
                                dleri.X += bar.Rect.Width - dleri.Width;
                            break;

                        case DataLabelPosition.OutsideEnd:
                            // Outer border of the column.
                            dleri.X = bar.Rect.X;
                            if (bar.Point._value > 0)
                                dleri.X += bar.Rect.Width;
                            else
                                dleri.X -= dleri.Width;
                            break;
                    }
                }
            }
        }
    }
}