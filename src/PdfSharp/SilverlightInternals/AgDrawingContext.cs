#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
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

#if SILVERLIGHT
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Shapes;
using PdfSharp.Drawing;
using PdfSharp.Internal;

// ReSharper disable ConvertPropertyToExpressionBody

namespace System.Windows.Media
{
    /// <summary>
    /// The WPF graphic system has the DrawingContext class that implements the low-level
    /// primitives for retained drawing. The lowest level in Silverlight a some non-aggregated
    /// UI elements like several Shape objects, TextBlock and Glyphs.
    /// This Silverlight version of DrawingContext simplyfies the implementation of XGraphics.
    /// It converts function calls to DrawingContext into UI elements layered on Canvas
    /// objects.
    /// </summary>
    internal class AgDrawingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AgDrawingContext"/> class.
        /// Drawing creates objects that are placed on the specified canvas.
        /// </summary>
        internal AgDrawingContext(Canvas canvas)
        {
            if (canvas == null)
                throw new ArgumentNullException("canvas");

            // The size of the canvas is not used and does not matter.
            // The properties of the canvas are not modified. Instead a new
            // canvas is added to its Children list.
            _canvasStack.Push(canvas);
            PushCanvas();
        }

        public void Close()
        {
            // There is nothing to close in Silverlight.
        }

        // There are no drawing objects in Silverlight.
        //public void DrawDrawing(Drawing drawing);

        public void DrawEllipse(Brush brush, Pen pen, Point center, double radiusX, double radiusY)
        {
            Ellipse ellipse = new Ellipse();
            SetupShape(ellipse, center.X - radiusX, center.Y - radiusY, radiusX * 2, radiusY * 2, brush, pen);
            ellipse.Fill = brush;
            ActiveCanvas.Children.Add(ellipse);
        }

        //public void DrawEllipse(Brush brush, Pen pen, Point center, AnimationClock centerAnimations, double radiusX, AnimationClock radiusXAnimations, double radiusY, AnimationClock radiusYAnimations);

        public void DrawGeometry(Brush brush, Pen pen, Geometry geometry)
        {
            Path path = new Path();
            SetupShape(path, 0, 0, Double.NaN, Double.NaN, brush, pen);
            path.Data = geometry;
            ActiveCanvas.Children.Add(path);
        }

        public void DrawPath(Brush brush, Pen pen, Path path)
        {
            SetupShape(path, brush, pen);
            ActiveCanvas.Children.Add(path);
        }

        //public void DrawGlyphRun(Brush foregroundBrush, GlyphRun glyphRun);

        public void DrawImage(ImageSource imageSource, Rect rectangle)
        {
            // TODO
            Image image = new Image();
            image.Source = imageSource;
            Canvas.SetLeft(image, rectangle.X);
            Canvas.SetTop(image, rectangle.Y);
            image.Width = rectangle.Width;
            image.Height = rectangle.Height;

            ActiveCanvas.Children.Add(image);
        }

        public void DrawLine(Pen pen, Point point0, Point point1)
        {
#if true
            Line line = new Line();
            SetupShape(line, 0, 0, Double.NaN, Double.NaN, null, pen);
            line.X1 = point0.X;
            line.Y1 = point0.Y;
            line.X2 = point1.X;
            line.Y2 = point1.Y;
            ActiveCanvas.Children.Add(line);
#else
            Line line = new Line();
            double x = Math.Min(point0.X, point1.X);
            double y = Math.Min(point0.Y, point1.Y);

            // Prevent clipping thick lines parallel to shape boundaries.
            double delta = 0; // 2 * pen.Thickness;
            //SetupShape(line, x - delta, y - delta, width + 2 * delta, height + 2 * delta, null, pen);
            SetupShape(line, x - delta, y - delta, Double.NaN, Double.NaN, null, pen);
            line.X1 = point0.X - x + delta;
            line.Y1 = point0.Y - y + delta;
            line.X2 = point1.X - x + delta;
            line.Y2 = point1.Y - y + delta;
            ActiveCanvas.Children.Add(line);
#endif
        }

        public void DrawRectangle(Brush brush, Pen pen, Rect rect)
        {
            Rectangle rectangle = new Rectangle();
            SetupShape(rectangle, rect.X, rect.Y, rect.Width, rect.Height, brush, pen);
            ActiveCanvas.Children.Add(rectangle);
        }

        public void DrawRoundedRectangle(Brush brush, Pen pen, Rect rect, double radiusX, double radiusY)
        {
            Rectangle rectangle = new Rectangle();
            SetupShape(rectangle, rect.X, rect.Y, rect.Width, rect.Height, brush, pen);
            rectangle.RadiusX = radiusX;
            rectangle.RadiusY = radiusY;
            ActiveCanvas.Children.Add(rectangle);
        }

        static void SetupShape(Shape shape, double x, double y, double width, double height, Brush brush, Pen pen)
        {
            if (width < 0)  // nan < 0 is false
            {
                x += width;
                width = -width;
            }
            if (height < 0)
            {
                y += height;
                height = -height;
            }
            Canvas.SetLeft(shape, x);
            Canvas.SetTop(shape, y);

            // Setting a double dependency property to Double.NaN is not the same
            // as simply not setting it. I consider this is a bug in the Silverlight run-time.
            if (!DoubleUtil.IsNaN(width))
                shape.Width = width;
            if (!DoubleUtil.IsNaN(height))
                shape.Height = height;
            SetupShape(shape, brush, pen);
        }

        static void SetupShape(Shape shape, Brush brush, Pen pen)
        {
            shape.Fill = brush;
            if (pen != null)
            {
                DoubleCollection dashArray = new DoubleCollection();
                foreach (double value in pen.DashArray)
                    dashArray.Add(value);

                shape.Stroke = pen.Brush;
                shape.StrokeThickness = pen.Thickness;
                shape.StrokeDashArray = dashArray;
                shape.StrokeDashOffset = pen.DashOffset;
                shape.StrokeStartLineCap = pen.StartLineCap;
                shape.StrokeEndLineCap = pen.EndLineCap;
                shape.StrokeDashCap = pen.DashCap;
                shape.StrokeLineJoin = pen.LineJoin;
                shape.StrokeMiterLimit = pen.MiterLimit;
            }
        }

        //public void DrawRoundedRectangle(Brush brush, Pen pen, Rect rectangle, AnimationClock rectangleAnimations, double radiusX, AnimationClock radiusXAnimations, double radiusY, AnimationClock radiusYAnimations);
        //public void DrawText(FormattedText formattedText, Point origin);
        //public void DrawVideo(MediaPlayer player, Rect rectangle);
        //public void DrawVideo(MediaPlayer player, Rect rectangle, AnimationClock rectangleAnimations);

        public void Pop(int count)
        {
            Debug.Assert(_canvasStack.Count - 1 > count);
            for (int idx = 0; idx < count; idx++)
                _canvasStack.Pop();
        }

        public void PushClip(Geometry clipGeometry)
        {
            Canvas canvas = ActiveCanvas;
            if (canvas.Children.Count > 0 || canvas.Clip != null)
                canvas = PushCanvas();
            canvas.Clip = clipGeometry;
        }

        //public void PushEffect(BitmapEffect effect, BitmapEffectInput effectInput);
        //public void PushGuidelineSet(GuidelineSet guidelines);

        public void PushOpacity(double opacity)
        {
            Canvas canvas = ActiveCanvas;
            if (canvas.Children.Count > 0 || !DoubleUtil.IsNaN(canvas.Opacity))
                canvas = PushCanvas();
            canvas.Opacity = opacity;
        }

        //public void PushOpacity(double opacity, AnimationClock opacityAnimations);

        public void PushOpacityMask(Brush opacityMask)
        {
            Canvas canvas = ActiveCanvas;
            if (canvas.Children.Count > 0 || canvas.OpacityMask != null)
                canvas = PushCanvas();
            canvas.OpacityMask = opacityMask;
        }

        public void PushTransform(MatrixTransform transform)
        {
            Canvas canvas = ActiveCanvas;
            if (canvas.Children.Count > 0 || canvas.RenderTransform != null)
                canvas = PushCanvas();
            canvas.RenderTransform = transform;
        }

        /// <summary>
        /// Resembles the DrawString function of GDI+.
        /// </summary>
        [Obsolete("Text may be drawn at the wrong position. Requires update!")]
        public void DrawString(XGraphics gfx, string text, XFont font, XBrush brush, XRect layoutRectangle, XStringFormat format)
        {
            double x = layoutRectangle.X;
            double y = layoutRectangle.Y;

            double lineSpace = font.GetHeight(); //old: font.GetHeight(gfx);
            double cyAscent = lineSpace * font.CellAscent / font.CellSpace;
            double cyDescent = lineSpace * font.CellDescent / font.CellSpace;

            bool bold = (font.Style & XFontStyle.Bold) != 0;
            bool italic = (font.Style & XFontStyle.Italic) != 0;
            bool strikeout = (font.Style & XFontStyle.Strikeout) != 0;
            bool underline = (font.Style & XFontStyle.Underline) != 0;

            //Debug.Assert(font.GlyphTypeface != null);
            TextBlock textBlock = new TextBlock(); //FontHelper.CreateTextBlock(text, font.GlyphTypeface, font.Size, brush.RealizeWpfBrush());
            if (layoutRectangle.Width > 0)
                textBlock.Width = layoutRectangle.Width;

            switch (format.Alignment)
            {
                case XStringAlignment.Near:
                    textBlock.TextAlignment = TextAlignment.Left;
                    break;

                case XStringAlignment.Center:
                    textBlock.TextAlignment = TextAlignment.Center;
                    break;

                case XStringAlignment.Far:
                    textBlock.TextAlignment = TextAlignment.Right;
                    break;
            }

            if (gfx.PageDirection == XPageDirection.Downwards)
            {
                switch (format.LineAlignment)
                {
                    case XLineAlignment.Near:
                        //y += cyAscent;
                        break;

                    case XLineAlignment.Center:
                        // TODO use CapHeight. PDFlib also uses 3/4 of ascent
                        y += (layoutRectangle.Height - textBlock.ActualHeight) / 2;
                        //y += -formattedText.Baseline + (font.Size * font.Metrics.CapHeight / font.unitsPerEm / 2) + layoutRectangle.Height / 2;
                        break;

                    case XLineAlignment.Far:
                        y += layoutRectangle.Height - textBlock.ActualHeight;
                        //y -= textBlock.ActualHeight;  //-formattedText.Baseline - cyDescent + layoutRectangle.Height;
                        break;

                    case XLineAlignment.BaseLine:
//#if !WINDOWS_PHONE
                        y -= textBlock.BaselineOffset;
//#else
//                        // No BaselineOffset in Silverlight WP yet.
//                        //y -= textBlock.BaselineOffset;
//#endif
                        break;
                }
            }
            else
            {
                throw new NotImplementedException("XPageDirection.Downwards");
            }

            //if (bold && !descriptor.IsBoldFace)
            //{
            //  // TODO: emulate bold by thicker outline
            //}

            //if (italic && !descriptor.IsBoldFace)
            //{
            //  // TODO: emulate italic by shearing transformation
            //}

            if (underline)
                textBlock.TextDecorations = TextDecorations.Underline;

            // No strikethrough in Silverlight
            //if (strikeout)
            //{
            //  formattedText.SetTextDecorations(TextDecorations.Strikethrough);
            //  //double strikeoutPosition = lineSpace * realizedFont.FontDescriptor.descriptor.StrikeoutPosition / font.cellSpace;
            //  //double strikeoutSize = lineSpace * realizedFont.FontDescriptor.descriptor.StrikeoutSize / font.cellSpace;
            //  //DrawRectangle(null, brush, x, y - strikeoutPosition - strikeoutSize, width, strikeoutSize);
            //}

            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
            ActiveCanvas.Children.Add(textBlock);
        }

        ///// <summary>
        ///// Resembles the MeasureString function of GDI+.
        ///// </summary>
        //[Obsolete("Use XGraphics.MeasureString()")]
        //public XSize MeasureString(XGraphics gfx, string text, XFont font, XStringFormat stringFormat)
        //{
        //    //Debug.Assert(font.GlyphTypeface != null);
        //    TextBlock textBlock = new TextBlock();  //FontHelper.CreateTextBlock(text, font.GlyphTypeface, font.Size, null);
        //    // Looks very much like a hack, but is the recommended way documented by Microsoft.
        //    return new XSize(textBlock.ActualWidth, textBlock.ActualHeight);
        //}

        /// <summary>
        /// Create new canvas and add it to the children of the current canvas.
        /// </summary>
        internal Canvas PushCanvas()
        {
            Canvas canvas = new Canvas();
            ActiveCanvas.Children.Add(canvas);
            _canvasStack.Push(canvas);
            return canvas;
        }

        /// <summary>
        /// Gets the currently active canvas.
        /// </summary>
        Canvas ActiveCanvas
        {
            get { return _canvasStack.Peek(); }
        }

        /// <summary>
        /// Gets the nesting level of Canvas objects.
        /// </summary>
        internal int Level
        {
            get { return _canvasStack.Count; }
        }
        readonly Stack<Canvas> _canvasStack = new Stack<Canvas>();
    }
}
#endif
