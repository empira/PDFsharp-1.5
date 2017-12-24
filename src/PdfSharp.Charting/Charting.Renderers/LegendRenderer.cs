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
    /// Represents the legend renderer for all chart types.
    /// </summary>
    internal abstract class LegendRenderer : Renderer
    {
        /// <summary>
        /// Initializes a new instance of the LegendRenderer class with the specified renderer parameters.
        /// </summary>
        internal LegendRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Layouts and calculates the space used by the legend.
        /// </summary>
        internal override void Format()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;
            LegendRendererInfo lri = cri.legendRendererInfo;
            if (lri == null)
                return;

            RendererParameters parms = new RendererParameters();
            parms.Graphics = _rendererParms.Graphics;

            bool verticalLegend = (lri._legend._docking == DockingType.Left || lri._legend._docking == DockingType.Right);
            XSize maxMarkerArea = new XSize();
            LegendEntryRenderer ler = new LegendEntryRenderer(parms);
            foreach (LegendEntryRendererInfo leri in lri.Entries)
            {
                parms.RendererInfo = leri;
                ler.Format();

                maxMarkerArea.Width = Math.Max(leri.MarkerArea.Width, maxMarkerArea.Width);
                maxMarkerArea.Height = Math.Max(leri.MarkerArea.Height, maxMarkerArea.Height);

                if (verticalLegend)
                {
                    lri.Width = Math.Max(lri.Width, leri.Width);
                    lri.Height += leri.Height;
                }
                else
                {
                    lri.Width += leri.Width;
                    lri.Height = Math.Max(lri.Height, leri.Height);
                }
            }

            // Add padding to left, right, top and bottom
            int paddingFactor = 1;
            if (lri.BorderPen != null)
                paddingFactor = 2;
            lri.Width += (LegendRenderer.LeftPadding + LegendRenderer.RightPadding) * paddingFactor;
            lri.Height += (LegendRenderer.TopPadding + LegendRenderer.BottomPadding) * paddingFactor;
            if (verticalLegend)
                lri.Height += LegendRenderer.EntrySpacing * (lri.Entries.Length - 1);
            else
                lri.Width += LegendRenderer.EntrySpacing * (lri.Entries.Length - 1);

            foreach (LegendEntryRendererInfo leri in lri.Entries)
                leri.MarkerArea = maxMarkerArea;
        }

        /// <summary>
        /// Draws the legend.
        /// </summary>
        internal override void Draw()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;
            LegendRendererInfo lri = cri.legendRendererInfo;
            if (lri == null)
                return;

            XGraphics gfx = _rendererParms.Graphics;
            RendererParameters parms = new RendererParameters();
            parms.Graphics = gfx;

            LegendEntryRenderer ler = new LegendEntryRenderer(parms);

            bool verticalLegend = (lri._legend._docking == DockingType.Left || lri._legend._docking == DockingType.Right);
            int paddingFactor = 1;
            if (lri.BorderPen != null)
                paddingFactor = 2;
            XRect legendRect = lri.Rect;
            legendRect.X += LegendRenderer.LeftPadding * paddingFactor;
            legendRect.Y += LegendRenderer.TopPadding * paddingFactor;
            foreach (LegendEntryRendererInfo leri in cri.legendRendererInfo.Entries)
            {
                XRect entryRect = legendRect;
                entryRect.Width = leri.Width;
                entryRect.Height = leri.Height;

                leri.Rect = entryRect;
                parms.RendererInfo = leri;
                ler.Draw();

                if (verticalLegend)
                    legendRect.Y += entryRect.Height + LegendRenderer.EntrySpacing;
                else
                    legendRect.X += entryRect.Width + LegendRenderer.EntrySpacing;
            }

            // Draw border around legend
            if (lri.BorderPen != null)
            {
                XRect borderRect = lri.Rect;
                borderRect.X += LegendRenderer.LeftPadding;
                borderRect.Y += LegendRenderer.TopPadding;
                borderRect.Width -= LegendRenderer.LeftPadding + LegendRenderer.RightPadding;
                borderRect.Height -= LegendRenderer.TopPadding + LegendRenderer.BottomPadding;
                gfx.DrawRectangle(lri.BorderPen, borderRect);
            }
        }

        /// <summary>
        /// Used to insert a padding on the left.
        /// </summary>
        protected const double LeftPadding = 6;

        /// <summary>
        /// Used to insert a padding on the right.
        /// </summary>
        protected const double RightPadding = 6;

        /// <summary>
        /// Used to insert a padding at the top.
        /// </summary>
        protected const double TopPadding = 6;

        /// <summary>
        /// Used to insert a padding at the bottom.
        /// </summary>
        protected const double BottomPadding = 6;

        /// <summary>
        /// Used to insert a padding between entries.
        /// </summary>
        protected const double EntrySpacing = 5;

        /// <summary>
        /// Default line width used for the legend's border.
        /// </summary>
        protected const double DefaultLineWidth = 0.14; // 0.05 mm
    }
}
