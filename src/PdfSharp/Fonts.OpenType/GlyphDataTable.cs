
#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
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

#define VERBOSE_

using System;
using System.Collections.Generic;

//using Fixed = System.Int32;
//using FWord = System.Int16;
//using UFWord = System.UInt16;

namespace PdfSharp.Fonts.OpenType
{
    /// <summary>
    /// This table contains information that describes the glyphs in the font in the TrueType outline format.
    /// Information regarding the rasterizer (scaler) refers to the TrueType rasterizer. 
    /// http://www.microsoft.com/typography/otspec/glyf.htm
    /// </summary>
    internal class GlyphDataTable : OpenTypeFontTable
    {
        public const string Tag = TableTagNames.Glyf;

        internal byte[] GlyphTable;

        public GlyphDataTable()
            : base(null, Tag)
        {
            DirectoryEntry.Tag = TableTagNames.Glyf;
        }

        public GlyphDataTable(OpenTypeFontface fontData)
            : base(fontData, Tag)
        {
            DirectoryEntry.Tag = TableTagNames.Glyf;
            Read();
        }

        /// <summary>
        /// Converts the bytes in a handy representation
        /// </summary>
        public void Read()
        {
            try
            {
                // not yet needed...
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the data of the specified glyph.
        /// </summary>
        public byte[] GetGlyphData(int glyph)
        {
            IndexToLocationTable loca = _fontData.loca;
            int start = GetOffset(glyph);
            int next = GetOffset(glyph + 1);
            int count = next - start;
            byte[] bytes = new byte[count];
            Buffer.BlockCopy(_fontData.FontSource.Bytes, start, bytes, 0, count);
            return bytes;
        }

        /// <summary>
        /// Gets the size of the byte array that defines the glyph.
        /// </summary>
        public int GetGlyphSize(int glyph)
        {
            IndexToLocationTable loca = _fontData.loca;
            return GetOffset(glyph + 1) - GetOffset(glyph);
        }

        /// <summary>
        /// Gets the offset of the specified glyph relative to the first byte of the font image.
        /// </summary>
        public int GetOffset(int glyph)
        {
            return DirectoryEntry.Offset + _fontData.loca.LocaTable[glyph];
        }

        /// <summary>
        /// Adds for all composite glyphs the glyphs the composite one is made of.
        /// </summary>
        public void CompleteGlyphClosure(Dictionary<int, object> glyphs)
        {
            int count = glyphs.Count;
            int[] glyphArray = new int[glyphs.Count];
            glyphs.Keys.CopyTo(glyphArray, 0);
            if (!glyphs.ContainsKey(0))
                glyphs.Add(0, null);
            for (int idx = 0; idx < count; idx++)
                AddCompositeGlyphs(glyphs, glyphArray[idx]);
        }

        /// <summary>
        /// If the specified glyph is a composite glyph add the glyphs it is made of to the glyph table.
        /// </summary>
        void AddCompositeGlyphs(Dictionary<int, object> glyphs, int glyph)
        {
            //int start = fontData.loca.GetOffset(glyph);
            int start = GetOffset(glyph);
            // Has no contour?
            if (start == GetOffset(glyph + 1))
                return;
            _fontData.Position = start;
            int numContours = _fontData.ReadShort();
            // Is not a composite glyph?
            if (numContours >= 0)
                return;
            _fontData.SeekOffset(8);
            for (; ; )
            {
                int flags = _fontData.ReadUFWord();
                int cGlyph = _fontData.ReadUFWord();
                if (!glyphs.ContainsKey(cGlyph))
                    glyphs.Add(cGlyph, null);
                if ((flags & MORE_COMPONENTS) == 0)
                    return;
                int offset = (flags & ARG_1_AND_2_ARE_WORDS) == 0 ? 2 : 4;
                if ((flags & WE_HAVE_A_SCALE) != 0)
                    offset += 2;
                else if ((flags & WE_HAVE_AN_X_AND_Y_SCALE) != 0)
                    offset += 4;
                if ((flags & WE_HAVE_A_TWO_BY_TWO) != 0)
                    offset += 8;
                _fontData.SeekOffset(offset);
            }
        }

        /// <summary>
        /// Prepares the font table to be compiled into its binary representation.
        /// </summary>
        public override void PrepareForCompilation()
        {
            base.PrepareForCompilation();

            if (DirectoryEntry.Length == 0)
                DirectoryEntry.Length = GlyphTable.Length;
            DirectoryEntry.CheckSum = CalcChecksum(GlyphTable);
        }

        /// <summary>
        /// Converts the font into its binary representation.
        /// </summary>
        public override void Write(OpenTypeFontWriter writer)
        {
            writer.Write(GlyphTable, 0, DirectoryEntry.PaddedLength);
        }

        // ReSharper disable InconsistentNaming
        // Constants from OpenType spec.
        const int ARG_1_AND_2_ARE_WORDS = 1;
        const int WE_HAVE_A_SCALE = 8;
        const int MORE_COMPONENTS = 32;
        const int WE_HAVE_AN_X_AND_Y_SCALE = 64;
        const int WE_HAVE_A_TWO_BY_TWO = 128;
        // ReSharper restore InconsistentNaming
    }
}
