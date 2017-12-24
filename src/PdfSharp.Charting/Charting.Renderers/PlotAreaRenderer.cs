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
    /// Base class for all plot area renderers.
    /// </summary>
    internal abstract class PlotAreaRenderer : Renderer
    {
        /// <summary>
        /// Initializes a new instance of the PlotAreaRenderer class with the specified renderer parameters.
        /// </summary>
        internal PlotAreaRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Returns an initialized PlotAreaRendererInfo.
        /// </summary>
        internal override RendererInfo Init()
        {
            PlotAreaRendererInfo pari = new PlotAreaRendererInfo();
            pari._plotArea = ((ChartRendererInfo)_rendererParms.RendererInfo)._chart._plotArea;
            InitLineFormat(pari);
            InitFillFormat(pari);
            return pari;
        }

        /// <summary>
        /// Initializes the plot area's line format common to all derived plot area renderers.
        /// If line format is given all uninitialized values will be set.
        /// </summary>
        protected void InitLineFormat(PlotAreaRendererInfo rendererInfo)
        {
            if (rendererInfo._plotArea._lineFormat != null)
                rendererInfo.LineFormat = Converter.ToXPen(rendererInfo._plotArea._lineFormat, XColors.Black, DefaultLineWidth);
        }

        /// <summary>
        /// Initializes the plot area's fill format common to all derived plot area renderers.
        /// If fill format is given all uninitialized values will be set.
        /// </summary>
        protected void InitFillFormat(PlotAreaRendererInfo rendererInfo)
        {
            if (rendererInfo._plotArea._fillFormat != null)
                rendererInfo.FillFormat = Converter.ToXBrush(rendererInfo._plotArea._fillFormat, XColors.White);
        }

        /// <summary>
        /// Represents the default line width for the plot area's border.
        /// </summary>
        protected const double DefaultLineWidth = 0.15;
    }
}
