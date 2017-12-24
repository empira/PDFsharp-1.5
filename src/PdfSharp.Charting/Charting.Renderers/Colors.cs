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
    /// Represents the predefined column/bar chart colors.
    /// </summary>
    internal sealed class ColumnColors
    {
        /// <summary>
        /// Gets the color for column/bar charts from the specified index.
        /// </summary>
        public static XColor Item(int index)
        {
            return XColor.FromArgb((int)_seriesColors[index]);
        }

        /// <summary>
        /// Colors for column/bar charts taken from Excel.
        /// </summary>
        static readonly uint[] _seriesColors = new uint[]
    {
      0xFF9999FF, 0xFF993366, 0xFFFFFFCC, 0xFFCCFFFF, 0xFF660066, 0xFFFF8080,
      0xFF0066CC, 0xFFCCCCFF, 0xFF000080, 0xFFFF00FF, 0xFFFFFF00, 0xFF00FFFF,
      0xFF800080, 0xFF800000, 0xFF008080, 0xFF0000FF, 0xFF00CCFF, 0xFFCCFFFF,
      0xFFCCFFCC, 0xFFFFFF99, 0xFF99CCFF, 0xFFFF99CC, 0xFFCC99FF, 0xFFFFCC99,
      0xFF3366FF, 0xFF33CCCC, 0xFF99CC00, 0xFFFFCC00, 0xFFFF9900, 0xFFFF6600,
      0xFF666699, 0xFF969696, 0xFF003366, 0xFF339966, 0xFF003300, 0xFF333300,
      0xFF993300, 0xFF993366, 0xFF333399, 0xFF333333, 0xFF000000, 0xFFFFFFFF,
      0xFFFF0000, 0xFF00FF00, 0xFF0000FF, 0xFFFFFF00, 0xFFFF00FF, 0xFF00FFFF,
      0xFF800000, 0xFF008000, 0xFF000080, 0xFF808000, 0xFF800080, 0xFF008080,
      0xFFC0C0C0, 0xFF808080
    };
    }

    /// <summary>
    /// Represents the predefined line chart colors.
    /// </summary>
    internal sealed class LineColors
    {
        /// <summary>
        /// Gets the color for line charts from the specified index.
        /// </summary>
        public static XColor Item(int index)
        {
            return XColor.FromArgb((int)_lineColors[index]);
        }

        /// <summary>
        /// Colors for line charts taken from Excel.
        /// </summary>
        static readonly uint[] _lineColors = new uint[]
    {
      0xFF000080, 0xFFFF00FF, 0xFFFFFF00, 0xFF00FFFF, 0xFF800080, 0xFF800000,
      0xFF008080, 0xFF0000FF, 0xFF00CCFF, 0xFFCCFFFF, 0xFFCCFFCC, 0xFFFFFF99,
      0xFF99CCFF, 0xFFFF99CC, 0xFFCC99FF, 0xFFFFCC99, 0xFF3366FF, 0xFF33CCCC,
      0xFF99CC00, 0xFFFFCC00, 0xFFFF9900, 0xFFFF6600, 0xFF666699, 0xFF969696,
      0xFF003366, 0xFF339966, 0xFF003300, 0xFF333300, 0xFF993300, 0xFF993366,
      0xFF333399, 0xFF000000, 0xFFFFFFFF, 0xFFFF0000, 0xFF00FF00, 0xFF0000FF,
      0xFFFFFF00, 0xFFFF00FF, 0xFF00FFFF, 0xFF800000, 0xFF008000, 0xFF000080,
      0xFF808000, 0xFF800080, 0xFF008080, 0xFFC0C0C0, 0xFF808080, 0xFF9999FF,
      0xFF993366, 0xFFFFFFCC, 0xFFCCFFFF, 0xFF660066, 0xFFFF8080, 0xFF0066CC,
      0xFFCCCCFF
    };
    }

    /// <summary>
    /// Represents the predefined pie chart colors.
    /// </summary>
    internal sealed class PieColors
    {
        /// <summary>
        /// Gets the color for pie charts from the specified index.
        /// </summary>
        public static XColor Item(int index)
        {
            return XColor.FromArgb((int)_sectorColors[index]);
        }

        /// <summary>
        /// Colors for pie charts taken from Excel.
        /// </summary>
        static readonly uint[] _sectorColors = new uint[]
    {
      0xFF9999FF, 0xFF993366, 0xFFFFFFCC, 0xFFCCFFFF, 0xFF660066, 0xFFFF8080,
      0xFF0066CC, 0xFFCCCCFF, 0xFF000080, 0xFFFF00FF, 0xFFFFFF00, 0xFF00FFFF,
      0xFF800080, 0xFF800000, 0xFF008080, 0xFF0000FF, 0xFF00CCFF, 0xFFCCFFFF,
      0xFFCCFFCC, 0xFFFFFF99, 0xFF99CCFF, 0xFFFF99CC, 0xFFCC99FF, 0xFFFFCC99,
      0xFF3366FF, 0xFF33CCCC, 0xFF99CC00, 0xFFFFCC00, 0xFFFF9900, 0xFFFF6600,
      0xFF666699, 0xFF969696, 0xFF003366, 0xFF339966, 0xFF003300, 0xFF333300,
      0xFF993300, 0xFF993366, 0xFF333399, 0xFF333333, 0xFF000000, 0xFFFFFFFF,
      0xFFFF0000, 0xFF00FF00, 0xFF0000FF, 0xFFFFFF00, 0xFFFF00FF, 0xFF00FFFF,
      0xFF800000, 0xFF008000, 0xFF000080, 0xFF808000, 0xFF800080, 0xFF008080,
      0xFFC0C0C0, 0xFF808080
    };
    }
}
