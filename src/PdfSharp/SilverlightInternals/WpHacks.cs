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

#if WINDOWS_PHONE
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

#pragma warning disable 436, 1591

namespace System.ComponentModel
{
    //[AttributeUsage(AttributeTargets.All)]
    //public sealed class BrowsableAttribute : Attribute
    //{
    //    public static readonly BrowsableAttribute Yes;
    //    public static readonly BrowsableAttribute No;
    //    public static readonly BrowsableAttribute Default;
    //    public BrowsableAttribute(bool browsable) { }
    //}
}

namespace System.Windows.Media
{
    //public class Typeface
    //{
    //    public bool TryGetGlyphTypeface(out GlyphTypeface glyphTypeface)
    //    {
    //        glyphTypeface = null;
    //        return false;
    //    }
    //}

    //public sealed class GlyphTypeface
    //{
    //    public double Version { get; private set; }
    //    public string FontFileName { get; private set; }
    //}
}
#endif
