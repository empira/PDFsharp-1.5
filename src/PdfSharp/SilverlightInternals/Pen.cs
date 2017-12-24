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
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace System.Windows.Media
{
    /// <summary>
    /// Imitates the WPF Pen object.
    /// </summary>
    internal sealed class Pen
    {
        public Pen()
        {
            Thickness = 1;
            DashArray = new DoubleCollection();
            StartLineCap = PenLineCap.Flat;
            EndLineCap = PenLineCap.Flat;
            DashCap = PenLineCap.Flat;
            LineJoin = PenLineJoin.Miter;  // TODO: check default values
            MiterLimit = 10;  // TODO: check default values
        }

        //public static Pen FromShape(Shape shape)
        //{
        //  Pen pen = new Pen();
        //  pen.Brush = shape.Stroke;
        //  pen.Thickness = shape.StrokeThickness;
        //  // Todo
        //  pen.DashArray = new DoubleCollection();
        //  pen.StartLineCap = PenLineCap.Flat;
        //  pen.EndLineCap = PenLineCap.Flat;
        //  pen.DashCap = PenLineCap.Flat;
        //  pen.LineJoin = PenLineJoin.Miter;  // TODO: check default values
        //  pen.MiterLimit = 10;  // TODO: check default values

        //  return pen;
        //}

        public Brush Brush { get; set; }

        public double Thickness { get; set; }

        //public DashStyle DashStyle { get; set; }

        public DoubleCollection DashArray { get; set; }

        public double DashOffset { get; set; }

        public PenLineCap StartLineCap { get; set; }

        public PenLineCap EndLineCap { get; set; }

        public PenLineCap DashCap { get; set; }

        public PenLineJoin LineJoin { get; set; }

        public double MiterLimit { get; set; }

        //public Pen Clone();
        //public Pen CloneCurrentValue();
    }
}
#endif
