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
    /// Represents a renderer specialized to draw lines in various styles, colors and widths.
    /// </summary>
    class LineFormatRenderer
    {
        /// <summary>
        /// Initializes a new instance of the LineFormatRenderer class with the specified graphics, line format
        /// and default width.
        /// </summary>
        public LineFormatRenderer(XGraphics gfx, LineFormat lineFormat, double defaultWidth)
        {
            _gfx = gfx;
            bool visible = false;
            double width = 0;

            if (lineFormat != null)
            {
                width = lineFormat._width;
                if (width == 0 && !lineFormat.Color.IsEmpty)
                    width = defaultWidth;
                visible = lineFormat.Visible || width > 0 || !lineFormat.Color.IsEmpty;
            }

            if (visible)
            {
                _pen = new XPen(lineFormat.Color, width);
                _pen.DashStyle = lineFormat.DashStyle;
            }
        }

        /// <summary>
        /// Initializes a new instance of the LineFormatRenderer class with the specified graphics and
        /// line format.
        /// </summary>
        public LineFormatRenderer(XGraphics gfx, LineFormat lineFormat) :
            this(gfx, lineFormat, 0)
        { }

        /// <summary>
        /// Initializes a new instance of the LineFormatRenderer class with the specified graphics and pen.
        /// </summary>
        public LineFormatRenderer(XGraphics gfx, XPen pen)
        {
            _gfx = gfx;
            _pen = pen;
        }

        /// <summary>
        /// Draws a line from point pt0 to point pt1.
        /// </summary>
        public void DrawLine(XPoint pt0, XPoint pt1)
        {
            if (_pen != null)
                _gfx.DrawLine(_pen, pt0, pt1);
        }

        /// <summary>
        /// Draws a line specified by rect.
        /// </summary>
        public void DrawRectangle(XRect rect)
        {
            if (_pen != null)
                _gfx.DrawRectangle(_pen, rect);
        }

        /// <summary>
        /// Draws a line specified by path.
        /// </summary>
        public void DrawPath(XGraphicsPath path)
        {
            if (_pen != null)
                _gfx.DrawPath(_pen, path);
        }

        /// <summary>
        /// Surface to draw the line.
        /// </summary>
        readonly XGraphics _gfx;

        /// <summary>
        /// Pen used to draw the line.
        /// </summary>
        readonly XPen _pen;
    }
}
