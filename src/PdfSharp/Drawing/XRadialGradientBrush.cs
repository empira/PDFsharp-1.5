#region PDFsharp - A .NET library for processing PDF

//
// Authors:
//   Stefan Lange (mailto:Stefan.Lange@pdfsharp.com)
//
// Copyright (c) 2005-2019 empira Software GmbH, Cologne Area (Germany)
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

#endregion PDFsharp - A .NET library for processing PDF

using System;
using System.ComponentModel;
using PdfSharp.Internal;
#if GDI
using System.Drawing;
using System.Drawing.Drawing2D;
using GdiPoint = System.Drawing.Point;
using GdiLinearGradientBrush = System.Drawing.Drawing2D.LinearGradientBrush;
#endif
#if WPF
using System.Windows;
using System.Windows.Media;
using SysPoint = System.Windows.Point;
using SysSize = System.Windows.Size;
using SysRect = System.Windows.Rect;
using WpfBrush = System.Windows.Media.Brush;
using WpfRadialGradientBrush = System.Windows.Media.RadialGradientBrush;
#endif
#if UWP
using Windows.UI;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
#endif

// ReSharper disable RedundantNameQualifier because it is required for hybrid build

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Defines a Brush with a radial gradient.
    /// </summary>
    public sealed class XRadialGradientBrush : XGradientBrush
    {
#if GDI
/// <summary>
/// Initializes a new instance of the <see cref="XRadialGradientBrush"/> class.
/// </summary>
        public XRadialGradientBrush(GdiPoint point1, GdiPoint point2, double innerRadius, double outerRadius, XColor color1, XColor color2)
          : this(new XPoint(point1), new XPoint(point2), innerRadius, outerRadius, color1, color2)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XRadialGradientBrush"/> class.
        /// </summary>
        public XRadialGradientBrush(PointF point1, PointF point2, double innerRadius, double outerRadius, XColor color1, XColor color2)
          : this(new XPoint(point1), new XPoint(point2), innerRadius, outerRadius, color1, color2)
        { }
#endif

#if WPF
        /// <summary>
        /// Initializes a new instance of the <see cref="XRadialGradientBrush"/> class.
        /// </summary>
        public XRadialGradientBrush(System.Windows.Point point1, System.Windows.Point point2, double innerRadius,
            double outerRadius, XColor color1, XColor color2)
            : this(new XPoint(point1), new XPoint(point2), innerRadius, outerRadius, color1, color2)
        {
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="XRadialGradientBrush"/> class.
        /// </summary>
        public XRadialGradientBrush(XPoint point1, XPoint point2, double innerRadius, double outerRadius, XColor color1,
            XColor color2)
        {
            _point1 = point1;
            _point2 = point2;
            _innerRadius = innerRadius;
            _outerRadius = outerRadius;
            _color1 = color1;
            _color2 = color2;
        }

        /// <summary>
        /// Gets or sets an XMatrix that defines a local geometric transform for this RadialGradientBrush.
        /// </summary>
        public XMatrix Transform
        {
            get { return _matrix; }
            set { _matrix = value; }
        }

        /// <summary>
        /// Gets or sets the inner radius.
        /// </summary>
        public double InnerRadius
        {
            get { return _innerRadius; }
            set { _innerRadius = value; }
        }

        /// <summary>
        /// Gets or sets the outer radius.
        /// </summary>
        public double OuterRadius
        {
            get { return _outerRadius; }
            set { _outerRadius = value; }
        }

        /// <summary>
        /// Translates the brush with the specified offset.
        /// </summary>
        public void TranslateTransform(double dx, double dy)
        {
            _matrix.TranslatePrepend(dx, dy);
        }

        /// <summary>
        /// Translates the brush with the specified offset.
        /// </summary>
        public void TranslateTransform(double dx, double dy, XMatrixOrder order)
        {
            _matrix.Translate(dx, dy, order);
        }

        /// <summary>
        /// Scales the brush with the specified scalars.
        /// </summary>
        public void ScaleTransform(double sx, double sy)
        {
            _matrix.ScalePrepend(sx, sy);
        }

        /// <summary>
        /// Scales the brush with the specified scalars.
        /// </summary>
        public void ScaleTransform(double sx, double sy, XMatrixOrder order)
        {
            _matrix.Scale(sx, sy, order);
        }

        /// <summary>
        /// Rotates the brush with the specified angle.
        /// </summary>
        public void RotateTransform(double angle)
        {
            _matrix.RotatePrepend(angle);
        }

        /// <summary>
        /// Rotates the brush with the specified angle.
        /// </summary>
        public void RotateTransform(double angle, XMatrixOrder order)
        {
            _matrix.Rotate(angle, order);
        }

        /// <summary>
        /// Multiply the brush transformation matrix with the specified matrix.
        /// </summary>
        public void MultiplyTransform(XMatrix matrix)
        {
            _matrix.Prepend(matrix);
        }

        /// <summary>
        /// Multiply the brush transformation matrix with the specified matrix.
        /// </summary>
        public void MultiplyTransform(XMatrix matrix, XMatrixOrder order)
        {
            _matrix.Multiply(matrix, order);
        }

        /// <summary>
        /// Resets the brush transformation matrix with identity matrix.
        /// </summary>
        public void ResetTransform()
        {
            _matrix = new XMatrix(); //XMatrix.Identity;
        }

        //public void SetBlendTriangularShape(double focus);
        //public void SetBlendTriangularShape(double focus, double scale);
        //public void SetSigmaBellShape(double focus);
        //public void SetSigmaBellShape(double focus, double scale);

#if GDI
//TODO: Change from LinearGradient to RadialGradient
        internal override System.Drawing.Brush RealizeGdiBrush()
        {
            //if (dirty)
            //{
            //  if (brush == null)
            //    brush = new SolidBrush(color.ToGdiColor());
            //  else
            //  {
            //    brush.Color = color.ToGdiColor();
            //  }
            //  dirty = false;
            //}

#if not_implemented
            // TODO: use dirty to optimize code
            GdiLinearGradientBrush brush;
            try
            {
                Lock.EnterGdiPlus();
                if (_useRect)
                {
                    brush = new GdiLinearGradientBrush(_rect.ToRectangleF(),
                        _color1.ToGdiColor(), _color2.ToGdiColor(), (LinearGradientMode)_linearGradientMode);
                }
                else
                {
                    brush = new GdiLinearGradientBrush(
                        _point1.ToPointF(), _point2.ToPointF(),
                        _color1.ToGdiColor(), _color2.ToGdiColor());
                }
                if (!_matrix.IsIdentity)
                    brush.Transform = _matrix.ToGdiMatrix();
                //brush.WrapMode = WrapMode.Clamp;
            }
            finally { Lock.ExitGdiPlus(); }
            return brush;
#else
            return null;
#endif
        }
#endif

#if WPF
        internal override WpfBrush RealizeWpfBrush()
        {
            //if (this.dirty)
            //{
            //  if (this.brush == null)
            //    this.brush = new SolidBrush(this.color.ToGdiColor());
            //  else
            //  {
            //    this.brush.Color = this.color.ToGdiColor();
            //  }
            //  this.dirty = false;
            //}

            WpfRadialGradientBrush brush;
#if !SILVERLIGHT
            brush = new WpfRadialGradientBrush(_color1.ToWpfColor(), _color2.ToWpfColor());
            brush.RadiusX = OuterRadius;
            brush.RadiusY = OuterRadius;
            //brush = new System.Drawing.Drawing2D.LinearGradientBrush(
            //  this.point1.ToPointF(), this.point2.ToPointF(),
            //  this.color1.ToGdiColor(), this.color2.ToGdiColor());
#else
            GradientStop gs1 = new GradientStop();
            gs1.Color = _color1.ToWpfColor();
            gs1.Offset = 0;

            GradientStop gs2 = new GradientStop();
            gs2.Color = _color2.ToWpfColor();
            gs2.Offset = 1;

            GradientStopCollection gsc = new GradientStopCollection();
            gsc.Add(gs1);
            gsc.Add(gs2);

            brush = new WpfRadialGradientBrush(_color1.ToWpfColor(), _color2.ToWpfColor());
            brush.RadiusX = OuterRadius;
            brush.RadiusY = OuterRadius;
#endif
            if (!_matrix.IsIdentity)
            {
#if !SILVERLIGHT
                brush.Transform = new MatrixTransform(_matrix.ToWpfMatrix());
#else
                MatrixTransform transform = new MatrixTransform();
                transform.Matrix = _matrix.ToWpfMatrix();
                brush.Transform = transform;
#endif
            }
            return brush; //this.brush;
        }
#endif

#if UWP
        internal override ICanvasBrush RealizeCanvasBrush()
        {
            ICanvasBrush brush;

            brush = new CanvasSolidColorBrush(CanvasDevice.GetSharedDevice(), Colors.RoyalBlue);

            return brush;
        }
#endif

        internal XPoint _point1, _point2;
        internal XColor _color1, _color2;
        internal double _innerRadius, _outerRadius;
        internal XMatrix _matrix;
    }
}