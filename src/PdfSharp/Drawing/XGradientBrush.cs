#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
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
#endregion

using System;
using System.ComponentModel;
using PdfSharp.Internal;
#if GDI
using System.Drawing;
using System.Drawing.Drawing2D;
using GdiLinearGradientBrush =System.Drawing.Drawing2D.LinearGradientBrush;
#endif
#if WPF
using System.Windows;
using System.Windows.Media;
using SysPoint = System.Windows.Point;
using SysSize = System.Windows.Size;
using SysRect = System.Windows.Rect;
using WpfBrush = System.Windows.Media.Brush;
#endif
#if UWP
using Windows.UI;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
#endif

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Defines a Brush with a linear gradient.
    /// </summary>
    public abstract class XGradientBrush : XBrush
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not to extend the gradient beyond its bounds.
        /// </summary>
        public bool ExtendLeft
        {
            get { return _extendLeft; }
            set { _extendLeft = value; }
        }
        internal bool _extendLeft;

        /// <summary>
        /// Gets or sets a value indicating whether or not to extend the gradient beyond its bounds.
        /// </summary>
        public bool ExtendRight
        {
            get { return _extendRight; }
            set { _extendRight = value; }
        }
        internal bool _extendRight;
    }
}
