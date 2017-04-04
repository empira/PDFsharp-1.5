#region PDFsharp - A .NET library for processing PDF
//
// Authors:
// Stefan Lange (mailto:Stefan.Lange@pdfsharp.com)
//
// Copyright (c) 2005-2009 empira Software GmbH, Cologne (Germany)
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
using System.Collections.Generic;
using PdfSharper.Pdf.IO;

namespace PdfSharper.Drawing.Layout
{
    /// <summary>
    /// Represents a very simple text formatter.
    /// If this class does not satisfy your needs on formatting paragraphs I recommend to take a look
    /// at MigraDoc Foundation. Alternatively you should copy this class in your own source code and modify it.
    /// </summary>
    public class XTextFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XTextFormatter"/> class.
        /// </summary>
        public XTextFormatter(XGraphics gfx)
        {
            if (gfx == null)
                throw new ArgumentNullException("gfx");
            _gfx = gfx;
        }
        readonly XGraphics _gfx;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        string _text;

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        public XFont Font
        {
            get { return _font; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Font");
                _font = value;

                _lineSpace = _font.GetHeight();
                _cyAscent = _lineSpace * _font.CellAscent / _font.CellSpace;
                _cyDescent = _lineSpace * _font.CellDescent / _font.CellSpace;

                // HACK in XTextFormatter
                _spaceWidth = _gfx.MeasureString("x x", value).Width;
                _spaceWidth -= _gfx.MeasureString("xx", value).Width;
            }
        }
        XFont _font;
        double _lineSpace;
        double _cyAscent;
        double _cyDescent;
        double _spaceWidth;

        /// <summary>
        /// Gets or sets the bounding box of the layout.
        /// </summary>
        public XRect LayoutRectangle
        {
            get { return _layoutRectangle; }
            set { _layoutRectangle = value; }
        }
        XRect _layoutRectangle;

        /// <summary>
        /// Gets or sets the alignment of the text.
        /// </summary>
        public XParagraphAlignment Alignment
        {
            get { return _alignment; }
            set { _alignment = value; }
        }
        XParagraphAlignment _alignment = XParagraphAlignment.Left;

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The text brush.</param>
        /// <param name="layoutRectangle">The layout rectangle.</param>
        public void DrawString(string text, XFont font, XBrush brush, XRect layoutRectangle)
        {
            DrawString(text, font, brush, layoutRectangle, XStringFormats.TopLeft);
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The text brush.</param>
        /// <param name="layoutRectangle">The layout rectangle.</param>
        /// <param name="format">The format. Must be <c>XStringFormat.TopLeft</c></param>
        public void DrawString(string text, XFont font, XBrush brush, XRect layoutRectangle, XStringFormat format)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (font == null)
                throw new ArgumentNullException("font");
            if (brush == null)
                throw new ArgumentNullException("brush");

            Text = text;
            Font = font;
            LayoutRectangle = layoutRectangle;

            if (text.Length == 0)
                return;

            CreateBlocks();

            CreateLayout();

            double dx = layoutRectangle.Location.X;
            double dy = layoutRectangle.Location.Y + _cyAscent;



            /* Alterado por Ângelo Cossa */
            List<double> Larr_lineWidth = new List<double>();
            List<double> Larr_lineStart = new List<double>();

            //If the text fills all the retangle, there is no sense in alignment
            bool ends_before_end = false;

            //Se if the last block comes before the end,
            for (int idx = 0; idx < _blocks.Count; idx++)
            {
                Block block = (Block)_blocks[idx];

                if (block.Type == BlockType.LineBreak)
                {
                    Larr_lineStart.Add(0);

                    if ((idx - 1) < 0)
                    {
                        Larr_lineWidth.Add(0);
                    }
                    else
                    {
                        Larr_lineWidth.Add(_blocks[idx - 1].Location.X + _blocks[idx - 1].Width);
                    }
                }

                if (block.Stop)
                {
                    ends_before_end = true;
                    break;
                }
            }

            //Add the last line
            Larr_lineStart.Add(0);

            if ((_blocks.Count - 1) < 0)
            {
                Larr_lineWidth.Add(0);
            }
            else
            {
                Larr_lineWidth.Add(_blocks[_blocks.Count - 1].Location.X + _blocks[_blocks.Count - 1].Width);
            }

            //Adjust the linestart
            if (format.Alignment != XStringAlignment.Near)
            {

                for (int n = 0; n < Larr_lineWidth.Count; n++)
                {
                    //gets the diference between the width of the retangle and the width of the line
                    double rest = layoutRectangle.Width - Larr_lineWidth[n];


                    if (format.Alignment == XStringAlignment.Center)
                    {

                        Larr_lineStart[n] = rest / 2D;
                    }
                    else if (format.Alignment == XStringAlignment.Far)
                    {
                        Larr_lineStart[n] = rest;
                    }
                }

            }


            if (format.LineAlignment != XLineAlignment.Near && !ends_before_end)
            {

                Block last_block = (Block)_blocks[_blocks.Count - 1];

                //gets the height of the text
                double text_height = (last_block.Location.Y + last_block.Height);

                //the diference between the size of the block and the size fo the text
                double rest = layoutRectangle.Height - text_height;


                if (format.LineAlignment == XLineAlignment.BaseLine)
                {
                    //if the text is in the botton, the rest is in the top
                    dy += rest;
                }
                else if (format.LineAlignment == XLineAlignment.Center)
                {
                    //If the text is in the middle half the rest in the top
                    dy += (rest / 2D);
                }


            }




            int count = _blocks.Count;
            int lineCount = 0;

            for (int idx = 0; idx < count; idx++)
            {
                Block block = (Block)_blocks[idx];
                if (block.Stop)
                    break;
                if (block.Type == BlockType.LineBreak)
                {
                    lineCount++;
                    continue;
                }

                //Now that the text is correctly aligned vertically align it horinzontally
                _gfx.DrawString(block.Text, font, brush, dx + Larr_lineStart[lineCount] + block.Location.X, dy + block.Location.Y);
            }
        }

        void CreateBlocks()
        {
            //TODO: make seperate blocks again when we are reading adobe afm files
            _blocks.Clear();
            string token = _text;
            _blocks.Add(new Block(token, BlockType.Text,
            _gfx.MeasureString(token, _font).Width, _gfx.MeasureString(token, _font).Height));
        }

        void CreateLayout()
        {
            double rectWidth = _layoutRectangle.Width;
            double rectHeight = _layoutRectangle.Height - _cyAscent - _cyDescent;
            int firstIndex = 0;
            double x = 0, y = 0;
            int count = _blocks.Count;
            for (int idx = 0; idx < count; idx++)
            {
                Block block = _blocks[idx];
                if (block.Type == BlockType.LineBreak)
                {
                    if (Alignment == XParagraphAlignment.Justify)
                        _blocks[firstIndex].Alignment = XParagraphAlignment.Left;
                    AlignLine(firstIndex, idx - 1, rectWidth);
                    firstIndex = idx + 1;
                    x = 0;
                    y += _lineSpace;
                }
                else
                {
                    double width = block.Width;
                    if ((x + width <= rectWidth || x == 0) && block.Type != BlockType.LineBreak)
                    {
                        block.Location = new XPoint(x, y);
                        x += width;
                    }
                    else
                    {
                        AlignLine(firstIndex, idx - 1, rectWidth);
                        firstIndex = idx;
                        y += _lineSpace;
                        if (y > rectHeight)
                        {
                            block.Stop = true;
                            break;
                        }
                        block.Location = new XPoint(0, y);
                        x = width;
                    }
                }
            }
            if (firstIndex < count && Alignment != XParagraphAlignment.Justify)
                AlignLine(firstIndex, count - 1, rectWidth);
        }

        /// <summary>
        /// Align center, right, or justify.
        /// </summary>
        void AlignLine(int firstIndex, int lastIndex, double layoutWidth)
        {
            XParagraphAlignment blockAlignment = _blocks[firstIndex].Alignment;
            if (_alignment == XParagraphAlignment.Left || blockAlignment == XParagraphAlignment.Left)
                return;

            int count = lastIndex - firstIndex + 1;
            if (count == 0)
                return;

            double totalWidth = -_spaceWidth;
            for (int idx = firstIndex; idx <= lastIndex; idx++)
                totalWidth += _blocks[idx].Width + _spaceWidth;

            double dx = Math.Max(layoutWidth - totalWidth, 0);
            //Debug.Assert(dx >= 0);
            if (_alignment != XParagraphAlignment.Justify)
            {
                if (_alignment == XParagraphAlignment.Center)
                    dx /= 2;
                for (int idx = firstIndex; idx <= lastIndex; idx++)
                {
                    Block block = _blocks[idx];
                    block.Location += new XSize(dx, 0);
                }
            }
            else if (count > 1) // case: justify
            {
                dx /= count - 1;
                for (int idx = firstIndex + 1, i = 1; idx <= lastIndex; idx++, i++)
                {
                    Block block = _blocks[idx];
                    block.Location += new XSize(dx * i, 0);
                }
            }
        }

        readonly List<Block> _blocks = new List<Block>();

        enum BlockType
        {
            Text, Space, Hyphen, LineBreak,
        }

        /// <summary>
        /// Represents a single word.
        /// </summary>
        class Block
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Block"/> class.
            /// </summary>
            /// <param name="text">The text of the block.</param>
            /// <param name="type">The type of the block.</param>
            /// <param name="width">The width of the text.</param>
            /// <param name="height">the height of the text</param>
            public Block(string text, BlockType type, double width, double height)
            {
                Text = text;
                Type = type;
                Width = width;
                Height = height;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Block"/> class.
            /// </summary>
            /// <param name="type">The type.</param>
            public Block(BlockType type)
            {
                Type = type;
            }

            /// <summary>
            /// The text represented by this block.
            /// </summary>
            public readonly string Text;

            /// <summary>
            /// The type of the block.
            /// </summary>
            public readonly BlockType Type;

            /// <summary>
            /// The width of the text.
            /// </summary>
            public readonly double Width;

            /// <summary>
            /// The Heigth of the text.
            /// </summary>
            public readonly double Height;


            /// <summary>
            /// The location relative to the upper left corner of the layout rectangle.
            /// </summary>
            public XPoint Location;

            /// <summary>
            /// The alignment of this line.
            /// </summary>
            public XParagraphAlignment Alignment;

            /// <summary>
            /// A flag indicating that this is the last block that fits in the layout rectangle.
            /// </summary>
            public bool Stop;
        }
        // TODO:
        // - more XStringFormat variations
        // - calculate bounding box
        // - left and right indent
        // - first line indent
        // - margins and paddings
        // - background color
        // - text background color
        // - border style
        // - hyphens, soft hyphens, hyphenation
        // - kerning
        // - change font, size, text color etc.
        // - line spacing
        // - underline and strike-out variation
        // - super- and sub-script
        // - ...
    }
}