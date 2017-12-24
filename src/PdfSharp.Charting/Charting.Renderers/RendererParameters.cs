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
    /// Represents the necessary data for chart rendering.
    /// </summary>
    internal class RendererParameters
    {
        /// <summary>
        /// Initializes a new instance of the RendererParameters class.
        /// </summary>
        public RendererParameters()
        { }

        /// <summary>
        /// Initializes a new instance of the RendererParameters class with the specified graphics and
        /// coordinates.
        /// </summary>
        public RendererParameters(XGraphics gfx, double x, double y, double width, double height)
        {
            _gfx = gfx;
            _box = new XRect(x, y, width, height);
        }

        /// <summary>
        /// Initializes a new instance of the RendererParameters class with the specified graphics and
        /// rectangle.
        /// </summary>
        public RendererParameters(XGraphics gfx, XRect boundingBox)
        {
            _gfx = gfx;
            _box = boundingBox;
        }

        /// <summary>
        /// Gets or sets the graphics object.
        /// </summary>
        public XGraphics Graphics
        {
            get { return _gfx; }
            set { _gfx = value; }
        }
        XGraphics _gfx;

        /// <summary>
        /// Gets or sets the item to draw.
        /// </summary>
        public object DrawingItem
        {
            get { return _item; }
            set { _item = value; }
        }
        object _item;

        /// <summary>
        /// Gets or sets the rectangle for the drawing item.
        /// </summary>
        public XRect Box
        {
            get { return _box; }
            set { _box = value; }
        }
        XRect _box;

        /// <summary>
        /// Gets or sets the RendererInfo.
        /// </summary>
        public RendererInfo RendererInfo
        {
            get { return _rendererInfo; }
            set { _rendererInfo = value; }
        }
        RendererInfo _rendererInfo;
    }
}