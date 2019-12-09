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

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Specifies the alignment of a text string relative to its layout rectangle
    /// </summary>
    public enum XLineAlignment  // same values as System.Drawing.StringAlignment (except BaseLine)
    {
        /// <summary>
        /// Specifies the text be aligned near the layout.
        /// In a left-to-right layout, the near position is left. In a right-to-left layout, the near
        /// position is right.
        /// </summary>
        Near = 0,

        /// <summary>
        /// Specifies that text is aligned in the center of the layout rectangle.
        /// </summary>
        Center = 1,

        /// <summary>
        /// Specifies that text is aligned far from the origin position of the layout rectangle.
        /// In a left-to-right layout, the far position is right. In a right-to-left layout, the far
        /// position is left. 
        /// </summary>
        Far = 2,

        /// <summary>
        /// Specifies that text is aligned relative to its base line.
        /// With this option the layout rectangle must have a height of 0.
        /// </summary>
        BaseLine = 3,
    }
}
