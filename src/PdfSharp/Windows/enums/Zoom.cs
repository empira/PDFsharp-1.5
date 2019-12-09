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

namespace PdfSharp.Windows
{
    /// <summary>
    /// Defines a zoom factor used in the preview control.
    /// </summary>
    public enum Zoom
    {
        /// <summary>
        /// The smallest possible zoom factor.
        /// </summary>
        Mininum = 10,

        /// <summary>
        /// The largest possible zoom factor.
        /// </summary>
        Maximum = 800,

        /// <summary>
        /// A pre-defined zoom factor.
        /// </summary>
        Percent800 = 800,

        /// <summary>
        /// A pre-defined zoom factor.
        /// </summary>
        Percent600 = 600,

        /// <summary>
        /// A pre-defined zoom factor.
        /// </summary>
        Percent400 = 400,

        /// <summary>
        /// A pre-defined zoom factor.
        /// </summary>
        Percent200 = 200,

        /// <summary>
        /// A pre-defined zoom factor.
        /// </summary>
        Percent150 = 150,

        /// <summary>
        /// A pre-defined zoom factor.
        /// </summary>
        Percent100 = 100,

        /// <summary>
        /// A pre-defined zoom factor.
        /// </summary>
        Percent75 = 75,

        /// <summary>
        /// A pre-defined zoom factor.
        /// </summary>
        Percent50 = 50,

        /// <summary>
        /// A pre-defined zoom factor.
        /// </summary>
        Percent25 = 25,

        /// <summary>
        /// A pre-defined zoom factor.
        /// </summary>
        Percent10 = 10,

        /// <summary>
        /// Sets the percent value such that the document fits horizontally into the window.
        /// </summary>
        BestFit = -1,

        /// <summary>
        /// Sets the percent value such that the printable area of the document fits horizontally into the window.
        /// Currently not yet implemented and the same as ZoomBestFit.
        /// </summary>
        TextFit = -2,

        /// <summary>
        /// Sets the percent value such that the whole document fits completely into the window.
        /// </summary>
        FullPage = -3,

        /// <summary>
        /// Sets the percent value such that the document is displayed in its real physical size.
        /// </summary>
        OriginalSize = -4,
    }
}