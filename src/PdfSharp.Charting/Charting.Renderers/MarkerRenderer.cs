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
    /// Represents a renderer for markers in line charts and legends.
    /// </summary>
    internal class MarkerRenderer
    {
        /// <summary>
        /// Draws the marker given through rendererInfo at the specified position. Position specifies
        /// the center of the marker.
        /// </summary>
        internal static void Draw(XGraphics graphics, XPoint pos, MarkerRendererInfo rendererInfo)
        {
#if SILVERLIGHT
      return; // BUG: Code crashs Silverlight Path class.
#pragma warning disable 0162
#endif

            if (rendererInfo.MarkerStyle == MarkerStyle.None)
                return;

            double size = rendererInfo.MarkerSize;
            double size2 = size / 2;
            double x0, y0, x1, y1;
            double g;

            XPen foreground = new XPen(rendererInfo.MarkerForegroundColor, 0.5);
            XBrush background = new XSolidBrush(rendererInfo.MarkerBackgroundColor);

            XGraphicsPath gp = new XGraphicsPath();
            switch (rendererInfo.MarkerStyle)
            {
                case MarkerStyle.Square:
                    x0 = pos.X - size2;
                    y0 = pos.Y - size2;
                    x1 = pos.X + size2;
                    y1 = pos.Y + size2;
                    gp.AddLine(x0, y0, x1, y0);
                    gp.AddLine(x1, y0, x1, y1);
                    gp.AddLine(x1, y1, x0, y1);
                    gp.AddLine(x0, y1, x0, y0);
                    break;

                case MarkerStyle.Diamond:
                    gp.AddLine(x1 = pos.X + size2, pos.Y, pos.X, y0 = pos.Y - size2);
                    gp.AddLine(pos.X, y0, x0 = pos.X - size2, pos.Y);
                    gp.AddLine(x0, pos.Y, pos.X, y1 = pos.Y + size2);
                    gp.AddLine(pos.X, y1, x1, pos.Y);
                    break;

                case MarkerStyle.Triangle:
                    y0 = pos.Y + size / 2;
                    y1 = pos.Y - size / 2;
                    g = Math.Sqrt(size * size * 4 / 3) / 2;
                    gp.AddLine(pos.X, y1, pos.X + g, y0);
                    gp.AddLine(pos.X + g, y0, pos.X - g, y0);
                    gp.AddLine(pos.X - g, y0, pos.X, y1);
                    break;

                case MarkerStyle.Plus:
                    g = size2 / 4;
                    gp.AddLine(pos.X - size2, pos.Y + g, pos.X - g, pos.Y + g);
                    gp.AddLine(pos.X - g, pos.Y + g, pos.X - g, pos.Y + size2);
                    gp.AddLine(pos.X - g, pos.Y + size2, pos.X + g, pos.Y + size2);
                    gp.AddLine(pos.X + g, pos.Y + size2, pos.X + g, pos.Y + g);
                    gp.AddLine(pos.X + g, pos.Y + g, pos.X + size2, pos.Y + g);
                    gp.AddLine(pos.X + size2, pos.Y + g, pos.X + size2, pos.Y - g);
                    gp.AddLine(pos.X + size2, pos.Y - g, pos.X + g, pos.Y - g);
                    gp.AddLine(pos.X + g, pos.Y - g, pos.X + g, pos.Y - size2);
                    gp.AddLine(pos.X + g, pos.Y - size2, pos.X - g, pos.Y - size2);
                    gp.AddLine(pos.X - g, pos.Y - size2, pos.X - g, pos.Y - g);
                    gp.AddLine(pos.X - g, pos.Y - g, pos.X - size2, pos.Y - g);
                    gp.AddLine(pos.X - size2, pos.Y - g, pos.X - size2, pos.Y + g);
                    break;

                case MarkerStyle.Circle:
                case MarkerStyle.Dot:
                    x0 = pos.X - size2;
                    y0 = pos.Y - size2;
                    gp.AddEllipse(x0, y0, size, size);
                    break;

                case MarkerStyle.Dash:
                    x0 = pos.X - size2;
                    y0 = pos.Y - size2 / 3;
                    x1 = pos.X + size2;
                    y1 = pos.Y + size2 / 3;
                    gp.AddLine(x0, y0, x1, y0);
                    gp.AddLine(x1, y0, x1, y1);
                    gp.AddLine(x1, y1, x0, y1);
                    gp.AddLine(x0, y1, x0, y0);
                    break;

                case MarkerStyle.X:
                    g = size / 4;
                    gp.AddLine(pos.X - size2 + g, pos.Y - size2, pos.X, pos.Y - g);
                    gp.AddLine(pos.X, pos.Y - g, pos.X + size2 - g, pos.Y - size2);
                    gp.AddLine(pos.X + size2 - g, pos.Y - size2, pos.X + size2, pos.Y - size2 + g);
                    gp.AddLine(pos.X + size2, pos.Y - size2 + g, pos.X + g, pos.Y);
                    gp.AddLine(pos.X + g, pos.Y, pos.X + size2, pos.Y + size2 - g);
                    gp.AddLine(pos.X + size2, pos.Y + size2 - g, pos.X + size2 - g, pos.Y + size2);
                    gp.AddLine(pos.X + size2 - g, pos.Y + size2, pos.X, pos.Y + g);
                    gp.AddLine(pos.X, pos.Y + g, pos.X - size2 + g, pos.Y + size2);
                    gp.AddLine(pos.X - size2 + g, pos.Y + size2, pos.X - size2, pos.Y + size2 - g);
                    gp.AddLine(pos.X - size2, pos.Y + size2 - g, pos.X - g, pos.Y);
                    gp.AddLine(pos.X - g, pos.Y, pos.X - size2, pos.Y - size2 + g);
                    break;

                case MarkerStyle.Star:
                    {
                        XPoint[] points = new XPoint[10];

                        double radStep = 2 * Math.PI / 5;
                        double outerCircle = size / 2;
                        double innerCircle = size / 5;
                        // outer circle
                        double rad = -(Math.PI / 2); // 90°
                        for (int idx = 0; idx < 10; idx += 2)
                        {
                            points[idx].X = pos.X + outerCircle * Math.Cos(rad);
                            points[idx].Y = pos.Y + outerCircle * Math.Sin(rad);
                            rad += radStep;
                        }

                        // inner circle
                        rad = -(Math.PI / 4); // 45°
                        double x = innerCircle * Math.Cos(rad);
                        double y = innerCircle * Math.Sin(rad);
                        points[1].X = pos.X + x;
                        points[1].Y = pos.Y + y;
                        points[9].X = pos.X - x;
                        points[9].Y = pos.Y + y;
                        rad += radStep;
                        x = innerCircle * Math.Cos(rad);
                        y = innerCircle * Math.Sin(rad);
                        points[3].X = pos.X + x;
                        points[3].Y = pos.Y + y;
                        points[7].X = pos.X - x;
                        points[7].Y = pos.Y + y;
                        rad += radStep;
                        y = innerCircle * Math.Sin(rad);
                        points[5].X = pos.X;
                        points[5].Y = pos.Y + y;
                        gp.AddLines(points);
                    }
                    break;
            }

            gp.CloseFigure();
            if (rendererInfo.MarkerStyle != MarkerStyle.Dot)
            {
                graphics.DrawPath(background, gp);
                graphics.DrawPath(foreground, gp);
            }
        }
    }
}
