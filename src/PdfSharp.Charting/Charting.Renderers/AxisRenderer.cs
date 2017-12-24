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
    /// Represents the base for all specialized axis renderer. Initialization common too all
    /// axis renderer should come here.
    /// </summary>
    internal abstract class AxisRenderer : Renderer
    {
        /// <summary>
        /// Initializes a new instance of the AxisRenderer class with the specified renderer parameters.
        /// </summary>
        internal AxisRenderer(RendererParameters parms)
            : base(parms)
        { }

        /// <summary>
        /// Initializes the axis title of the rendererInfo. All missing font attributes will be taken
        /// from the specified defaultFont.
        /// </summary>
        protected void InitAxisTitle(AxisRendererInfo rendererInfo, XFont defaultFont)
        {
            if (rendererInfo._axis._title != null)
            {
                AxisTitleRendererInfo atri = new AxisTitleRendererInfo();
                rendererInfo._axisTitleRendererInfo = atri;

                atri._axisTitle = rendererInfo._axis._title;
                atri.AxisTitleText = rendererInfo._axis._title._caption;
                atri.AxisTitleAlignment = rendererInfo._axis._title._alignment;
                atri.AxisTitleVerticalAlignment = rendererInfo._axis._title._verticalAlignment;
                atri.AxisTitleFont = Converter.ToXFont(rendererInfo._axis._title._font, defaultFont);
                XColor fontColor = XColors.Black;
                if (rendererInfo._axis._title._font != null && !rendererInfo._axis._title._font._color.IsEmpty)
                    fontColor = rendererInfo._axis._title._font._color;
                atri.AxisTitleBrush = new XSolidBrush(fontColor);
                atri.AxisTitleOrientation = rendererInfo._axis._title._orientation;
            }
        }

        /// <summary>
        /// Initializes the tick labels of the rendererInfo. All missing font attributes will be taken
        /// from the specified defaultFont.
        /// </summary>
        protected void InitTickLabels(AxisRendererInfo rendererInfo, XFont defaultFont)
        {
            if (rendererInfo._axis._tickLabels != null)
            {
                rendererInfo.TickLabelsFont = Converter.ToXFont(rendererInfo._axis._tickLabels._font, defaultFont);
                XColor fontColor = XColors.Black;
                if (rendererInfo._axis._tickLabels._font != null && !rendererInfo._axis._tickLabels._font._color.IsEmpty)
                    fontColor = rendererInfo._axis._tickLabels._font._color;
                rendererInfo.TickLabelsBrush = new XSolidBrush(fontColor);

                rendererInfo.TickLabelsFormat = rendererInfo._axis._tickLabels._format;
                if (rendererInfo.TickLabelsFormat == null)
                    rendererInfo.TickLabelsFormat = GetDefaultTickLabelsFormat();
            }
            else
            {
                rendererInfo.TickLabelsFont = defaultFont;
                rendererInfo.TickLabelsBrush = new XSolidBrush(XColors.Black);
                rendererInfo.TickLabelsFormat = GetDefaultTickLabelsFormat();
            }
        }

        /// <summary>
        /// Initializes the line format of the rendererInfo.
        /// </summary>
        protected void InitAxisLineFormat(AxisRendererInfo rendererInfo)
        {
            if (rendererInfo._axis._minorTickMarkInitialized)
                rendererInfo.MinorTickMark = rendererInfo._axis.MinorTickMark;

            if (rendererInfo._axis._majorTickMarkInitialized)
                rendererInfo.MajorTickMark = rendererInfo._axis.MajorTickMark;
            else
                rendererInfo.MajorTickMark = TickMarkType.Outside;

            if (rendererInfo.MinorTickMark != TickMarkType.None)
                rendererInfo.MinorTickMarkLineFormat = Converter.ToXPen(rendererInfo._axis._lineFormat, XColors.Black, DefaultMinorTickMarkLineWidth);

            if (rendererInfo.MajorTickMark != TickMarkType.None)
                rendererInfo.MajorTickMarkLineFormat = Converter.ToXPen(rendererInfo._axis._lineFormat, XColors.Black, DefaultMajorTickMarkLineWidth);

            if (rendererInfo._axis._lineFormat != null)
            {
                rendererInfo.LineFormat = Converter.ToXPen(rendererInfo._axis.LineFormat, XColors.Black, DefaultLineWidth);
                if (!rendererInfo._axis._majorTickMarkInitialized)
                    rendererInfo.MajorTickMark = TickMarkType.Outside;
            }
        }

        /// <summary>
        /// Initializes the gridlines of the rendererInfo.
        /// </summary>
        protected void InitGridlines(AxisRendererInfo rendererInfo)
        {
            if (rendererInfo._axis._minorGridlines != null)
            {
                rendererInfo.MinorGridlinesLineFormat =
                  Converter.ToXPen(rendererInfo._axis._minorGridlines._lineFormat, XColors.Black, DefaultGridLineWidth);
            }
            else if (rendererInfo._axis._hasMinorGridlines)
            {
                // No minor gridlines object are given, but user asked for.
                rendererInfo.MinorGridlinesLineFormat = new XPen(XColors.Black, DefaultGridLineWidth);
            }

            if (rendererInfo._axis._majorGridlines != null)
            {
                rendererInfo.MajorGridlinesLineFormat =
                  Converter.ToXPen(rendererInfo._axis._majorGridlines._lineFormat, XColors.Black, DefaultGridLineWidth);
            }
            else if (rendererInfo._axis._hasMajorGridlines)
            {
                // No major gridlines object are given, but user asked for.
                rendererInfo.MajorGridlinesLineFormat = new XPen(XColors.Black, DefaultGridLineWidth);
            }
        }

        /// <summary>
        /// Default width for a variety of lines.
        /// </summary>
        protected const double DefaultLineWidth = 0.4; // 0.15 mm

        /// <summary>
        /// Default width for a gridlines.
        /// </summary>
        protected const double DefaultGridLineWidth = 0.15;

        /// <summary>
        /// Default width for major tick marks.
        /// </summary>
        protected const double DefaultMajorTickMarkLineWidth = 1;

        /// <summary>
        /// Default width for minor tick marks.
        /// </summary>
        protected const double DefaultMinorTickMarkLineWidth = 1;

        /// <summary>
        /// Default width of major tick marks.
        /// </summary>
        protected const double DefaultMajorTickMarkWidth = 4.3; // 1.5 mm

        /// <summary>
        /// Default width of minor tick marks.
        /// </summary>
        protected const double DefaultMinorTickMarkWidth = 2.8; // 1 mm

        /// <summary>
        /// Default width of space between label and tick mark.
        /// </summary>
        protected const double SpaceBetweenLabelAndTickmark = 2.1; // 0.7 mm

        protected abstract string GetDefaultTickLabelsFormat();
    }
}
