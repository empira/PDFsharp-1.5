#region PDFsharp Charting - A .NET charting library based on PDFsharp
//
// Authors:
//   Niklas Schneider
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

using PdfSharp.Drawing;

namespace PdfSharp.Charting.Renderers
{
    /// <summary>
    /// Represents the base class for all X axis renderer.
    /// </summary>
    internal abstract class XAxisRenderer : AxisRenderer
    {
        /// <summary>
        /// Initializes a new instance of the XAxisRenderer class with the specified renderer parameters.
        /// </summary>
        internal XAxisRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Returns the default tick labels format string.
        /// </summary>
        protected override string GetDefaultTickLabelsFormat()
        {
            return "0";
        }

        protected XMatrix RotateByDegrees(double rotateAngle, XPoint center)
        {
            double rotateRadians = rotateAngle * System.Math.PI / 180;
            XMatrix transformation = XMatrix.Identity;
            transformation.RotateAtPrepend(rotateRadians, center);
            return transformation;
        }

        protected XRect DrawTickLabel(XGraphics gfx, string tickLabel, XPoint point, XSize size, AxisRendererInfo xari)
        {
            XRect labelArea = new XRect(point, size);
            double rotateAngle = xari._axis.TickLabelAngle;

            // Draw rotated text.
            gfx.RotateAtTransform(rotateAngle, labelArea.Center);
            gfx.DrawString(tickLabel, xari.TickLabelsFont, xari.TickLabelsBrush, point);
            gfx.RotateAtTransform(-rotateAngle, labelArea.Center);

            // Simulate rotation to get the rotated bounding box
            var transformation = RotateByDegrees(rotateAngle, labelArea.Center);
            labelArea.Transform(transformation);
            return labelArea;
        }
    }
}
