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
    /// Provides functions which converts Charting.DOM objects into PdfSharp.Drawing objects.
    /// </summary>
    internal class Converter
    {
        /// <summary>
        /// Creates a XFont based on the font. Missing attributes will be taken from the defaultFont
        /// parameter.
        /// </summary>
        internal static XFont ToXFont(Font font, XFont defaultFont)
        {
            XFont xfont = defaultFont;
            if (font != null)
            {
                string fontFamily = font.Name;
                if (fontFamily == "")
                    fontFamily = defaultFont.FontFamily.Name;

                XFontStyle fontStyle = defaultFont.Style;
                if (font._bold)
                    fontStyle |= XFontStyle.Bold;
                if (font._italic)
                    fontStyle |= XFontStyle.Italic;

                double size = font._size.Point; //emSize???
                if (size == 0)
                    size = defaultFont.Size;

                xfont = new XFont(fontFamily, size, fontStyle);
            }
            return xfont;
        }

        /// <summary>
        /// Creates a XPen based on the specified line format. If not specified color and width will be taken
        /// from the defaultColor and defaultWidth parameter.
        /// </summary>
        internal static XPen ToXPen(LineFormat lineFormat, XColor defaultColor, double defaultWidth)
        {
            return ToXPen(lineFormat, defaultColor, defaultWidth, XDashStyle.Solid);
        }

        /// <summary>
        /// Creates a XPen based on the specified line format. If not specified color and width will be taken
        /// from the defaultPen parameter.
        /// </summary>
        internal static XPen ToXPen(LineFormat lineFormat, XPen defaultPen)
        {
            return ToXPen(lineFormat, defaultPen.Color, defaultPen.Width, defaultPen.DashStyle);
        }

        /// <summary>
        /// Creates a XPen based on the specified line format. If not specified color, width and dash style
        /// will be taken from the defaultColor, defaultWidth and defaultDashStyle parameters.
        /// </summary>
        internal static XPen ToXPen(LineFormat lineFormat, XColor defaultColor, double defaultWidth, XDashStyle defaultDashStyle)
        {
            XPen pen = null;
            if (lineFormat == null)
            {
                pen = new XPen(defaultColor, defaultWidth);
                pen.DashStyle = defaultDashStyle;
            }
            else
            {
                XColor color = defaultColor;
                if (!lineFormat.Color.IsEmpty)
                    color = lineFormat.Color;

                double width = lineFormat.Width.Point;
                if (!lineFormat.Visible)
                    width = 0;
                if (lineFormat.Visible && width == 0)
                    width = defaultWidth;

                pen = new XPen(color, width);
                pen.DashStyle = lineFormat._dashStyle;
                pen.DashOffset = 10 * width;
            }
            return pen;
        }

        /// <summary>
        /// Creates a XBrush based on the specified fill format. If not specified, color will be taken
        /// from the defaultColor parameter.
        /// </summary>
        internal static XBrush ToXBrush(FillFormat fillFormat, XColor defaultColor)
        {
            if (fillFormat == null || fillFormat._color.IsEmpty)
                return new XSolidBrush(defaultColor);
            return new XSolidBrush(fillFormat._color);
        }

        /// <summary>
        /// Creates a XBrush based on the specified font color. If not specified, color will be taken
        /// from the defaultColor parameter.
        /// </summary>
        internal static XBrush ToXBrush(Font font, XColor defaultColor)
        {
            if (font == null || font._color.IsEmpty)
                return new XSolidBrush(defaultColor);
            return new XSolidBrush(font._color);
        }
    }
}
