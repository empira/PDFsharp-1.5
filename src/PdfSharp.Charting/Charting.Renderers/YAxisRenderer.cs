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

namespace PdfSharp.Charting.Renderers
{
    /// <summary>
    /// Represents the base class for all Y axis renderer.
    /// </summary>
    internal abstract class YAxisRenderer : AxisRenderer
    {
        /// <summary>
        /// Initializes a new instance of the YAxisRenderer class with the specified renderer parameters.
        /// </summary>
        internal YAxisRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Calculates optimal minimum/maximum scale and minor/major tick based on yMin and yMax.
        /// </summary>
        protected void FineTuneYAxis(AxisRendererInfo rendererInfo, double yMin, double yMax)
        {
            if (yMin == double.MaxValue && yMax == double.MinValue)
            {
                // No series data given.
                yMin = 0.0f;
                yMax = 0.9f;
            }

            if (yMin == yMax)
            {
                if (yMin == 0)
                    yMax = 0.9f;
                else if (yMin < 0)
                    yMax = 0;
                else if (yMin > 0)
                    yMax = yMin + 1;
            }

            // If the ratio between yMax to yMin is more than 1.2, the smallest number will be set too zero.
            // It's Excel's behavior.
            if (yMin != 0)
            {
                if (yMin < 0 && yMax < 0)
                {
                    if (yMin / yMax >= 1.2)
                        yMax = 0;
                }
                else if (yMax / yMin >= 1.2)
                    yMin = 0;
            }

            double deltaYRaw = yMax - yMin;

            int digits = (int)(Math.Log(deltaYRaw, 10) + 1);
            double normed = deltaYRaw / Math.Pow(10, digits) * 10;

            double normedStepWidth = 1;
            if (normed < 2)
                normedStepWidth = 0.2f;
            else if (normed < 5)
                normedStepWidth = 0.5f;

            AxisRendererInfo yari = rendererInfo;
            double stepWidth = normedStepWidth * Math.Pow(10.0, digits - 1.0);
            if (yari._axis == null || double.IsNaN(yari._axis._majorTick))
                yari.MajorTick = stepWidth;
            else
                yari.MajorTick = yari._axis._majorTick;

            double roundFactor = stepWidth * 0.5;
            if (yari._axis == null || double.IsNaN(yari._axis.MinimumScale))
            {
                double signumMin = (yMin != 0) ? yMin / Math.Abs(yMin) : 0;
                yari.MinimumScale = (int)(Math.Abs((yMin - roundFactor) / stepWidth) - (1 * signumMin)) * stepWidth * signumMin;
            }
            else
                yari.MinimumScale = yari._axis.MinimumScale;

            if (yari._axis == null || double.IsNaN(yari._axis.MaximumScale))
            {
                double signumMax = (yMax != 0) ? yMax / Math.Abs(yMax) : 0;
                yari.MaximumScale = (int)(Math.Abs((yMax + roundFactor) / stepWidth) + (1 * signumMax)) * stepWidth * signumMax;
            }
            else
                yari.MaximumScale = yari._axis.MaximumScale;

            if (yari._axis == null || double.IsNaN(yari._axis._minorTick))
                yari.MinorTick = yari.MajorTick / 5;
            else
                yari.MinorTick = yari._axis._minorTick;
        }

        /// <summary>
        /// Returns the default tick labels format string.
        /// </summary>
        protected override string GetDefaultTickLabelsFormat()
        {
            return "0.0";
        }
    }
}
