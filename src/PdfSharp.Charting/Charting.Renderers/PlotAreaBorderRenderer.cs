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
    /// Represents the border renderer for plot areas.
    /// </summary>
    internal class PlotAreaBorderRenderer : Renderer
    {
        /// <summary>
        /// Initializes a new instance of the PlotAreaBorderRenderer class with the specified
        /// renderer parameters.
        /// </summary>
        internal PlotAreaBorderRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Draws the border around the plot area.
        /// </summary>
        internal override void Draw()
        {
            ChartRendererInfo cri = (ChartRendererInfo)_rendererParms.RendererInfo;
            if (cri.plotAreaRendererInfo.LineFormat != null && cri.plotAreaRendererInfo.LineFormat.Width > 0)
            {
                XGraphics gfx = _rendererParms.Graphics;
                LineFormatRenderer lineFormatRenderer = new LineFormatRenderer(gfx, cri.plotAreaRendererInfo.LineFormat);
                lineFormatRenderer.DrawRectangle(cri.plotAreaRendererInfo.Rect);
            }
        }
    }
}
