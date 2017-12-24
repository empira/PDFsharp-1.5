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
    /// Represents the renderer for a legend entry.
    /// </summary>
    internal class LegendEntryRenderer : Renderer
    {
        /// <summary>
        /// Initializes a new instance of the LegendEntryRenderer class with the specified renderer
        /// parameters.
        /// </summary>
        internal LegendEntryRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Calculates the space used by the legend entry.
        /// </summary>
        internal override void Format()
        {
            XGraphics gfx = _rendererParms.Graphics;
            LegendEntryRendererInfo leri = (LegendEntryRendererInfo)_rendererParms.RendererInfo;

            // Initialize
            leri.MarkerArea.Width = MaxLegendMarkerWidth;
            leri.MarkerArea.Height = MaxLegendMarkerHeight;
            leri.MarkerSize = new XSize();
            leri.MarkerSize.Width = leri.MarkerArea.Width;
            leri.MarkerSize.Height = leri.MarkerArea.Height;
            if (leri._seriesRendererInfo._series._chartType == ChartType.Line)
                leri.MarkerArea.Width *= 3;
            leri.Width = leri.MarkerArea.Width;
            leri.Height = leri.MarkerArea.Height;

            if (leri.EntryText != "")
            {
                leri.TextSize = gfx.MeasureString(leri.EntryText, leri._legendRendererInfo.Font);
                if (leri._seriesRendererInfo._series._chartType == ChartType.Line)
                {
                    leri.MarkerSize.Width = leri._seriesRendererInfo._markerRendererInfo.MarkerSize.Value;
                    leri.MarkerArea.Width = Math.Max(3 * leri.MarkerSize.Width, leri.MarkerArea.Width);
                }

                leri.MarkerArea.Height = Math.Min(leri.MarkerArea.Height, leri.TextSize.Height);
                leri.MarkerSize.Height = Math.Min(leri.MarkerSize.Height, leri.TextSize.Height);
                leri.Width = leri.TextSize.Width + leri.MarkerArea.Width + SpacingBetweenMarkerAndText;
                leri.Height = leri.TextSize.Height;
            }
        }

        /// <summary>
        /// Draws one legend entry.
        /// </summary>
        internal override void Draw()
        {
            XGraphics gfx = _rendererParms.Graphics;
            LegendEntryRendererInfo leri = (LegendEntryRendererInfo)_rendererParms.RendererInfo;

            XRect rect;
            if (leri._seriesRendererInfo._series._chartType == ChartType.Line)
            {
                // Draw line.
                XPoint posLineStart = new XPoint(leri.X, leri.Y + leri.Height / 2);
                XPoint posLineEnd = new XPoint(leri.X + leri.MarkerArea.Width, leri.Y + leri.Height / 2);
                gfx.DrawLine(new XPen(((XSolidBrush)leri.MarkerBrush).Color), posLineStart, posLineEnd);

                // Draw marker.
                double x = leri.X + leri.MarkerArea.Width / 2;
                XPoint posMarker = new XPoint(x, leri.Y + leri.Height / 2);
                MarkerRenderer.Draw(gfx, posMarker, leri._seriesRendererInfo._markerRendererInfo);
            }
            else
            {
                // Draw series rectangle for column, bar or pie charts.
                rect = new XRect(leri.X, leri.Y, leri.MarkerArea.Width, leri.MarkerArea.Height);
                rect.Y += (leri.Height - leri.MarkerArea.Height) / 2;
                gfx.DrawRectangle(leri.MarkerPen, leri.MarkerBrush, rect);
            }

            // Draw text
            if (leri.EntryText.Length > 0)
            {
                rect = leri.Rect;
                rect.X += leri.MarkerArea.Width + LegendEntryRenderer.SpacingBetweenMarkerAndText;
                XStringFormat format = new XStringFormat();
                format.LineAlignment = XLineAlignment.Near;
                gfx.DrawString(leri.EntryText, leri._legendRendererInfo.Font,
                               leri._legendRendererInfo.FontColor, rect, format);
            }
        }

        /// <summary>
        /// Absolute width for markers (including line) in point.
        /// </summary>
        private const double MarkerWidth = 4.3; // 1.5 mm

        /// <summary>
        /// Maximum legend marker width in point.
        /// </summary>
        private const double MaxLegendMarkerWidth = 7; // 2.5 mm

        /// <summary>
        /// Maximum legend marker height in point.
        /// </summary>
        private const double MaxLegendMarkerHeight = 7; // 2.5 mm

        /// <summary>
        /// Insert spacing between marker and text in point.
        /// </summary>
        private const double SpacingBetweenMarkerAndText = 4.3; // 1.5 mm
    }
}
