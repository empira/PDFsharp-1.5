#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   David Stephensen
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
#if GDI
using System.Drawing;
using System.Drawing.Imaging;
#endif
#if WPF
using System.Windows;
using System.Windows.Media;
#endif



// ========================================================================================
// ========================================================================================
// ===== THIS CLASS IS A FAKE. THE OPEN SOURCE VERSION OF PDFSHARP DOES NOT IMPLEMENT =====
// ===== A DATAMATRIX CODE. THIS IS BECAUSE OF THE ISO COPYRIGHT.                     =====  
// ========================================================================================
// ========================================================================================

// Even if it looks like a datamatrix code it is just random

namespace PdfSharp.Drawing.BarCodes
{
    /// <summary>
    /// Creates the XImage object for a DataMatrix.
    /// Important note for OpenSource version of PDFsharp:
    ///   The generated image object only contains random data.
    ///   If you need the correct implementation as defined in the ISO/IEC 16022:2000 specification,
    ///   please contact empira Software GmbH via www.pdfsharp.com.
    /// </summary>
    internal class DataMatrixImage
    {
        public static XImage GenerateMatrixImage(string text, string encoding, int rows, int columns)
        {
            DataMatrixImage dataMatrixImage = new DataMatrixImage(text, encoding, rows, columns);
            return dataMatrixImage.DrawMatrix();
        }

        public DataMatrixImage(string text, string encoding, int rows, int columns)
        {
            this.text = text;
            this.encoding = encoding;
            this.rows = rows;
            this.columns = columns;
        }

        string text;
        string encoding;
        int rows;
        int columns;

        /// <summary>
        /// Possible ECC200 Matrixes
        /// </summary>
        static Ecc200Block[] ecc200Sizes =
    {
      new Ecc200Block( 10,  10, 10, 10,    3,   3,  5),    //
      new Ecc200Block( 12,  12, 12, 12,    5,   5,  7),    //
      new Ecc200Block(  8,  18,  8, 18,    5,   5,  7),    //
      new Ecc200Block( 14,  14, 14, 14,    8,   8, 10),    //
      new Ecc200Block(  8,  32,  8, 16,   10,  10, 11),    //
      new Ecc200Block( 16,  16, 16, 16,   12,  12, 12),    //
      new Ecc200Block( 12,  26, 12, 26,   16,  16, 14),    //
      new Ecc200Block( 18,  18, 18, 18,   18,  18, 14),    //
      new Ecc200Block( 20,  20, 20, 20,   22,  22, 18),    //
      new Ecc200Block( 12,  36, 12, 18,   22,  22, 18),    //
      new Ecc200Block( 22,  22, 22, 22,   30,  30, 20),    //
      new Ecc200Block( 16,  36, 16, 18,   32,  32, 24),    //
      new Ecc200Block( 24,  24, 24, 24,   36,  36, 24),    //
      new Ecc200Block( 26,  26, 26, 26,   44,  44, 28),    //
      new Ecc200Block( 16,  48, 16, 24,   49,  49, 28),    //
      new Ecc200Block( 32,  32, 16, 16,   62,  62, 36),    //
      new Ecc200Block( 36,  36, 18, 18,   86,  86, 42),    //
      new Ecc200Block( 40,  40, 20, 20,  114, 114, 48),    //
      new Ecc200Block( 44,  44, 22, 22,  144, 144, 56),    //
      new Ecc200Block( 48,  48, 24, 24,  174, 174, 68),    //
      new Ecc200Block( 52,  52, 26, 26,  204, 102, 42),    //
      new Ecc200Block( 64,  64, 16, 16,  280, 140, 56),    //
      new Ecc200Block( 72,  72, 18, 18,  368,  92, 36),    //
      new Ecc200Block( 80,  80, 20, 20,  456, 114, 48),    //
      new Ecc200Block( 88,  88, 22, 22,  576, 144, 56),    //
      new Ecc200Block( 96,  96, 24, 24,  696, 174, 68),    //
      new Ecc200Block(104, 104, 26, 26,  816, 136, 56),    //
      new Ecc200Block(120, 120, 20, 20, 1050, 175, 68),    //
      new Ecc200Block(132, 132, 22, 22, 1304, 163, 62),    //
      new Ecc200Block(144, 144, 24, 24, 1558, 156, 62),    // 156*4+155*2
      new Ecc200Block(  0,   0,  0,  0,    0,    0, 0)     // terminate
    };

        public XImage DrawMatrix()
        {
            return CreateImage(DataMatrix(), this.rows, this.columns);
        }

        /// <summary>
        /// Creates the DataMatrix code.
        /// </summary>
        internal char[] DataMatrix()
        {
            int matrixColumns = this.columns;
            int matrixRows = this.rows;
            Ecc200Block matrix = new Ecc200Block(0, 0, 0, 0, 0, 0, 0);

            foreach (Ecc200Block eccmatrix in ecc200Sizes)
            {
                matrix = eccmatrix;
                if (matrix.Width != columns || matrix.Height != rows)
                    continue;
                else
                    break;
            }

            char[] grid = new char[matrixColumns * matrixRows];
            Random rand = new Random();

            for (int ccol = 0; ccol < matrixColumns; ccol++)
                grid[ccol] = (char)1;

            for (int rrows = 1; rrows < matrixRows; rrows++)
            {
                grid[rrows * matrixRows] = (char)1;
                for (int ccol = 1; ccol < matrixColumns; ccol++)
                    grid[rrows * matrixRows + ccol] = (char)rand.Next(2);
            }

            if (grid == null || matrixColumns == 0)
                return null; //No barcode produced;
            return grid;
        }

        /// <summary>
        /// Encodes the DataMatrix.
        /// </summary>
        internal char[] Iec16022Ecc200(int columns, int rows, string encoding, int barcodelen, string barcode, int len, int max, int ecc)
        {
            return null;
        }

        /// <summary>
        /// Creates a DataMatrix image object.
        /// </summary>
        /// <param name="code">A hex string like "AB 08 C3...".</param>
        /// <param name="size">I.e. 26 for a 26x26 matrix</param>
        public XImage CreateImage(char[] code, int size)//(string code, int size)
        {
            return CreateImage(code, size, size, 10);
        }

        /// <summary>
        /// Creates a DataMatrix image object.
        /// </summary>
        public XImage CreateImage(char[] code, int rows, int columns)
        {
            return CreateImage(code, rows, columns, 10);
        }

        /// <summary>
        /// Creates a DataMatrix image object.
        /// </summary>
        public XImage CreateImage(char[] code, int rows, int columns, int pixelsize)
        {
#if GDI
      Bitmap bm = new Bitmap(columns * pixelsize, rows * pixelsize);
      using (Graphics gfx = Graphics.FromImage(bm))
      {
        gfx.FillRectangle(System.Drawing.Brushes.White, new Rectangle(0, 0, columns * pixelsize, rows * pixelsize));

        for (int i = rows - 1; i >= 0; i--)
        {
          for (int j = 0; j < columns; j++)
          {
            if (code[((rows - 1) - i) * columns + j] == (char)1)
              gfx.FillRectangle(System.Drawing.Brushes.Black, j * pixelsize, i * pixelsize, pixelsize, pixelsize);
          }
        }
        System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Firebrick, pixelsize);
        gfx.DrawLine(pen, 0, 0, rows * pixelsize, columns * pixelsize);
        gfx.DrawLine(pen, columns * pixelsize, 0, 0, rows * pixelsize);
      }
      XImage image = XImage.FromGdiPlusImage(bm);
      image.Interpolate = false;
      return image;
#elif WPF
            return null;
#endif
        }
    }

    struct Ecc200Block
    {
        public int Height;
        public int Width;
        public int CellHeight;
        public int CellWidth;
        public int Bytes;
        public int DataBlock;
        public int RSBlock;

        public Ecc200Block(int h, int w, int ch, int cw, int bytes, int datablock, int rsblock)
        {
            Height = h;
            Width = w;
            CellHeight = ch;
            CellWidth = cw;
            Bytes = bytes;
            DataBlock = datablock;
            RSBlock = rsblock;
        }
    }
}
