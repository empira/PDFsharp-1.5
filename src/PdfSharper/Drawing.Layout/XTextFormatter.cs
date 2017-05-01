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
using System.Linq;
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
        private readonly XGraphics _gfx;
        private string _text = string.Empty;
        private bool _wrapText = false;
        private XFont _font;
        private XRect _layoutRectangle;
        private XParagraphAlignment _alignment = XParagraphAlignment.Left;
        private readonly List<Block> _blocks = new List<Block>();

        /// <summary>
        /// Initializes a new instance of the <see cref="XTextFormatter"/> class.
        /// </summary>
        public XTextFormatter(XGraphics gfx) 
        {
            if (gfx == null)
                throw new ArgumentNullException("gfx");
            _gfx = gfx;
        }
        
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /// <summary>
        /// Gets or sets the WrapText
        /// </summary>
        public bool WrapText
        {
            get { return _wrapText; }
            set { _wrapText = value; }
        }

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
            }
        }

        /// <summary>
        /// Gets or sets the bounding box of the layout.
        /// </summary>
        public XRect LayoutRectangle
        {
            get { return _layoutRectangle; }
            set { _layoutRectangle = value; }
        }

        /// <summary>
        /// Gets or sets the alignment of the text.
        /// </summary>
        public XParagraphAlignment Alignment
        {
            get { return _alignment; }
            set { _alignment = value; }
        }
        
        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The text brush.</param>
        /// <param name="layoutRectangle">The layout rectangle.</param>
        public void DrawString(string text, XFont font, XBrush brush, XRect layoutRectangle)
        {
            this.DrawString(text, false, font, brush, layoutRectangle, XStringFormats.TopLeft);
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The text brush.</param>
        /// <param name="layoutRectangle">The layout rectangle.</param>
        /// <param name="format"></param>
        public void DrawString(string text, XFont font, XBrush brush, XRect layoutRectangle, XStringFormat format)
        {
            this.DrawString(text, false, font, brush, layoutRectangle, XStringFormats.TopLeft);
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="wrapText">Should the text be wrapped.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The text brush.</param>
        /// <param name="layoutRectangle">The layout rectangle.</param>
        /// <param name="format">The format.</param>
        public void DrawString(string text, bool wrapText, XFont font, XBrush brush, XRect layoutRectangle, XStringFormat format)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (font == null)
                throw new ArgumentNullException("font");
            if (brush == null)
                throw new ArgumentNullException("brush");
            if (layoutRectangle == null)
                throw new ArgumentNullException("layoutRectangle");
            if (format == null)
                throw new ArgumentNullException("format");

            Text = text;
            Font = font;
            LayoutRectangle = layoutRectangle;
            WrapText = wrapText;

            if (text.Length == 0)
                return;

            CreateBlocks(format);
            CreateLayout(format);
            
            for (int idx = 0; idx < _blocks.Count; idx++)
            {
                Block block = (Block)_blocks[idx];
                if (block.Stop)
                    break;
                
                _gfx.DrawString(block.Text, font, brush, block.Location.X, block.Location.Y);
            }
        }

        /// <summary>
        /// Creates the Blocks to be Drawn
        /// </summary>
        /// <param name="format">The format</param>
        private void CreateBlocks(XStringFormat format)
        {
            _blocks.Clear();
            int length = _text.Length;
            bool inNonWhiteSpace = false;
            int startIndex = 0, blockLength = 0;
            for (int idx = 0; idx < length; idx++)
            {
                char ch = _text[idx];

                // Treat CR and CRLF as LF
                if (ch == Chars.CR)
                {
                    if (idx < length - 1 && _text[idx + 1] == Chars.LF)
                        idx++;
                    ch = Chars.LF;
                }

                if (ch == Chars.LF)
                {
                    if (blockLength != 0)
                    {
                        Block block = this.BuildBlock(_text, startIndex, blockLength, format);
                        _blocks.Add(block);
                    }

                    startIndex = idx + 1;
                    blockLength = 0;
                    _blocks.Add(new Block(BlockType.LineBreak));
                }
                else if (char.IsWhiteSpace(ch))
                {
                    if (inNonWhiteSpace)
                    {
                        blockLength = (length - (startIndex + blockLength)) > 0 ? blockLength + 1 : blockLength;
                        Block block = this.BuildBlock(_text, startIndex, blockLength, format);
                        _blocks.Add(block);

                        startIndex = idx + 1;
                        blockLength = 0;
                    }
                    else
                    {
                        blockLength++;
                    }
                }
                else
                {
                    inNonWhiteSpace = true;
                    blockLength++;

                    //Handle text that needs to be wrapped for Multiline TextFields.  Blocks created when Rectangle Width is hit
                    if (this.WrapText && idx < (length - 1))
                    {
                        string token = _text.Substring(startIndex, blockLength + 1);
                        XSize tokenSize = _gfx.MeasureString(token, _font);
                        
                        double availableWidth = _layoutRectangle.Width;
                        if (tokenSize.Width > availableWidth)   //Estimate the width of the rectangle including margins
                        {
                            Block block = this.BuildBlock(_text, startIndex, blockLength, format);
                            _blocks.Add(block);

                            startIndex = idx + 1;
                            blockLength = 0;
                        }
                    }
                }
            }

            if (blockLength != 0)
            {
                Block block = this.BuildBlock(_text, startIndex, blockLength, format);
                _blocks.Add(block);
            }
        }

        /// <summary>
        /// Builds Blocks for Text to be Drawn
        /// </summary>
        /// <param name="text">The Text</param>
        /// <param name="startIndex">The start index</param>
        /// <param name="blockLength">The block length</param>
        /// <param name="format">The format</param>
        /// <returns></returns>
        private Block BuildBlock(string text, int startIndex, int blockLength, XStringFormat format)
        {
            string token = _text.Substring(startIndex, blockLength);
            XSize tokenSize = _gfx.MeasureString(token, _font);
            Block block = new Block(token, BlockType.Text, tokenSize.Width, tokenSize.Height);

            switch (format.Alignment)
            {
                case XStringAlignment.Near:
                    block.Alignment = XParagraphAlignment.Left;
                    break;
                case XStringAlignment.Center:
                    block.Alignment = XParagraphAlignment.Center;
                    break;
                case XStringAlignment.Far:
                    block.Alignment = XParagraphAlignment.Right;
                    break;
                default:
                    block.Alignment = XParagraphAlignment.Default;
                    break;
            }

            return block;
        }

        /// <summary>
        /// Creates the Layout
        /// </summary>
        /// <param name="format">The format</param>
        private void CreateLayout(XStringFormat format)
        {
            double lineSpace = _blocks.Any(b => b.Height > 0) ? _blocks.Where(b => b.Height > 0).FirstOrDefault().Height : _font.GetHeight();
            double cyAscent = lineSpace * _font.CellAscent / _font.CellSpace;
            double cyDescent = lineSpace * _font.CellDescent / _font.CellSpace;

            double rectWidth = LayoutRectangle.Width;
            double rectHeight = LayoutRectangle.Height - cyAscent - cyDescent;
            int firstIndex = 0;
            double x = 0, y = 0;
            int count = _blocks.Count;
            for (int idx = 0; idx < count; idx++)
            {
                Block block = _blocks[idx];
                if (block.Type == BlockType.LineBreak)
                {
                    if (Alignment == XParagraphAlignment.Justify)
                    {
                        _blocks[firstIndex].Alignment = XParagraphAlignment.Left;
                    }

                    AlignLine(firstIndex, idx - 1, rectWidth);
                    firstIndex = idx + 1;
                    x = 0;
                    y += lineSpace;
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
                        y += lineSpace;
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
            {
                AlignLine(firstIndex, count - 1, rectWidth);
            }

            //This has to be done last because it requires knowing the Total Height of all text.
            double dy = CalculateFormattedOffsetY(format, cyAscent, _blocks.FindAll(b => b.Stop == true).Count > 0);
            for (int idx = 0; idx < count; idx++)
            {
                Block block = _blocks[idx];
                if (block != null && block.Location != null)
                {
                    block.Location.Y = block.Location.Y + dy;
                }
            }
        }

        /// <summary>
        /// Align center, right, or justify.
        /// </summary>
        /// <param name="firstIndex">First Index</param>
        /// <param name="lastIndex">The Last Index</param>
        /// <param name="layoutWidth">The Width of the Layout</param>
        private void AlignLine(int firstIndex, int lastIndex, double layoutWidth)
        {
            XParagraphAlignment blockAlignment = _blocks[firstIndex].Alignment;

            int count = lastIndex - firstIndex + 1;
            if (count == 0)
                return;
            
            double totalWidth = 0;
            for (int idx = firstIndex; idx <= lastIndex; idx++)
                totalWidth += _blocks[idx].Width;

            double dx = LayoutRectangle.Location.X;
            double remaining = Math.Max(layoutWidth - totalWidth, 0);
            if (blockAlignment != XParagraphAlignment.Justify)
            {
                if (blockAlignment == XParagraphAlignment.Center)
                {
                    dx = remaining / 2;
                }
                else if (blockAlignment == XParagraphAlignment.Right)
                {
                    dx = remaining;
                }

                for (int idx = firstIndex; idx <= lastIndex; idx++)
                {
                    Block block = _blocks[idx];
                    block.Location += new XSize(dx, 0);
                }
            }
            else if (count > 1) // case: justify
            {
                dx = remaining / (count - 1);
                for (int idx = firstIndex + 1, i = 1; idx <= lastIndex; idx++, i++)
                {
                    Block block = _blocks[idx];
                    block.Location += new XSize(dx * i, 0);
                }
            }
        }

        /// <summary>
        /// Calculate the offset for the y-axis based on the Rectangle and the Alignment
        /// </summary>
        /// <param name="format">The Text Format</param>
        /// <param name="cyAscent"></param>
        /// <param name="moreTextThanHeight">Is there more text than height</param>
        /// <returns></returns>
        private double CalculateFormattedOffsetY(XStringFormat format, double cyAscent, bool moreTextThanHeight)
        {
            double dy = LayoutRectangle.Location.Y + cyAscent;

            if (format.LineAlignment != XLineAlignment.Near && !moreTextThanHeight)
            {
                Block last_block = (Block)_blocks[_blocks.Count - 1];

                //gets the height of the text
                double text_height = (last_block.Location.Y + last_block.Height);

                //the difference between the size of the block and the size fo the text
                double rest = LayoutRectangle.Height - text_height;

                if (format.LineAlignment == XLineAlignment.BaseLine)
                {
                    //if the text is in the botton, the rest is in the top
                    dy += rest;
                }
                else if (format.LineAlignment == XLineAlignment.Center)
                {
                    //If the text is in the middle half the rest is in the top
                    dy += (rest / 2D);
                }
            }

            return dy;
        }
        
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

        /// <summary>
        /// The metrics for a Line - a collection of blocks
        /// </summary>
        class Line
        {
            /// <summary>
            /// The width of the line
            /// </summary>
            public double Width { get; set; }

            /// <summary>
            /// The start of the line
            /// </summary>
            public double Start { get; set; }

            public IEnumerable<Block> Blocks { get; set; }
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