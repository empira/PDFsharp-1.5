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
    /// Represents a axis title renderer used for x and y axis titles.
    /// </summary>
    internal class AxisTitleRenderer : Renderer
    {
        /// <summary>
        /// Initializes a new instance of the AxisTitleRenderer class with the
        /// specified renderer parameters.
        /// </summary>
        internal AxisTitleRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Calculates the space used for the axis title.
        /// </summary>
        internal override void Format()
        {
            XGraphics gfx = _rendererParms.Graphics;

            AxisTitleRendererInfo atri = ((AxisRendererInfo)_rendererParms.RendererInfo)._axisTitleRendererInfo;
            if (atri.AxisTitleText != "")
            {
                XSize size = gfx.MeasureString(atri.AxisTitleText, atri.AxisTitleFont);
                if (atri.AxisTitleOrientation != 0)
                {
                    XPoint[] points = new XPoint[2];
                    points[0].X = 0;
                    points[0].Y = 0;
                    points[1].X = size.Width;
                    points[1].Y = size.Height;

                    XMatrix matrix = new XMatrix();
                    matrix.RotatePrepend(-atri.AxisTitleOrientation);
                    matrix.TransformPoints(points);

                    size.Width = Math.Abs(points[1].X - points[0].X);
                    size.Height = Math.Abs(points[1].Y - points[0].Y);
                }

                atri.X = 0;
                atri.Y = 0;
                atri.Height = size.Height;
                atri.Width = size.Width;
            }
        }

        /// <summary>
        /// Draws the axis title.
        /// </summary>
        internal override void Draw()
        {
            AxisRendererInfo ari = (AxisRendererInfo)_rendererParms.RendererInfo;
            AxisTitleRendererInfo atri = ari._axisTitleRendererInfo;
            if (atri.AxisTitleText != "")
            {
                XGraphics gfx = _rendererParms.Graphics;
                if (atri.AxisTitleOrientation != 0)
                {
                    XRect layout = atri.Rect;
                    layout.X = -(layout.Width / 2);
                    layout.Y = -(layout.Height / 2);

                    double x = 0;
                    switch (atri.AxisTitleAlignment)
                    {
                        case HorizontalAlignment.Center:
                            x = atri.X + atri.Width / 2;
                            break;

                        case HorizontalAlignment.Right:
                            x = atri.X + atri.Width - layout.Width / 2;
                            break;

                        case HorizontalAlignment.Left:
                        default:
                            x = atri.X;
                            break;
                    }

                    double y = 0;
                    switch (atri.AxisTitleVerticalAlignment)
                    {
                        case VerticalAlignment.Center:
                            y = atri.Y + atri.Height / 2;
                            break;

                        case VerticalAlignment.Bottom:
                            y = atri.Y + atri.Height - layout.Height / 2;
                            break;

                        case VerticalAlignment.Top:
                        default:
                            y = atri.Y;
                            break;
                    }

                    XStringFormat xsf = new XStringFormat();
                    xsf.Alignment = XStringAlignment.Center;
                    xsf.LineAlignment = XLineAlignment.Center;

                    XGraphicsState state = gfx.Save();
                    gfx.TranslateTransform(x, y);
                    gfx.RotateTransform(-atri.AxisTitleOrientation);
                    gfx.DrawString(atri.AxisTitleText, atri.AxisTitleFont, atri.AxisTitleBrush, layout, xsf);
                    gfx.Restore(state);
                }
                else
                {
                    XStringFormat format = new XStringFormat();
                    switch (atri.AxisTitleAlignment)
                    {
                        case HorizontalAlignment.Center:
                            format.Alignment = XStringAlignment.Center;
                            break;

                        case HorizontalAlignment.Right:
                            format.Alignment = XStringAlignment.Far;
                            break;

                        case HorizontalAlignment.Left:
                        default:
                            format.Alignment = XStringAlignment.Near;
                            break;
                    }

                    switch (atri.AxisTitleVerticalAlignment)
                    {
                        case VerticalAlignment.Center:
                            format.LineAlignment = XLineAlignment.Center;
                            break;

                        case VerticalAlignment.Bottom:
                            format.LineAlignment = XLineAlignment.Far;
                            break;

                        case VerticalAlignment.Top:
                        default:
                            format.LineAlignment = XLineAlignment.Near;
                            break;
                    }

                    gfx.DrawString(atri.AxisTitleText, atri.AxisTitleFont, atri.AxisTitleBrush, atri.Rect, format);
                }
            }
        }
    }
}
