
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

using System.Diagnostics;

//using Fixed = System.Int32;
//using FWord = System.Int16;
//using UFWord = System.UInt16;

namespace PdfSharp.Fonts.OpenType
{
    /// <summary>
    /// Represents an entry in the fonts table dictionary.
    /// </summary>
    internal class TableDirectoryEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableDirectoryEntry"/> class.
        /// </summary>
        public TableDirectoryEntry()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDirectoryEntry"/> class.
        /// </summary>
        public TableDirectoryEntry(string tag)
        {
            Debug.Assert(tag.Length == 4);
            Tag = tag;
            //CheckSum = 0;
            //Offset = 0;
            //Length = 0;
            //FontTable = null;
        }

        /// <summary>
        /// 4 -byte identifier.
        /// </summary>
        public string Tag;

        /// <summary>
        /// CheckSum for this table.
        /// </summary>
        public uint CheckSum;

        /// <summary>
        /// Offset from beginning of TrueType font file.
        /// </summary>
        public int Offset;

        /// <summary>
        /// Actual length of this table in bytes.
        /// </summary>
        public int Length;

        /// <summary>
        /// Gets the length rounded up to a multiple of four bytes.
        /// </summary>
        public int PaddedLength
        {
            get { return (Length + 3) & ~3; }
        }

        /// <summary>
        /// Associated font table.
        /// </summary>
        public OpenTypeFontTable FontTable;

        /// <summary>
        /// Creates and reads a TableDirectoryEntry from the font image.
        /// </summary>
        public static TableDirectoryEntry ReadFrom(OpenTypeFontface fontData)
        {
            TableDirectoryEntry entry = new TableDirectoryEntry();
            entry.Tag = fontData.ReadTag();
            entry.CheckSum = fontData.ReadULong();
            entry.Offset = fontData.ReadLong();
            entry.Length = (int)fontData.ReadULong();
            return entry;
        }

        public void Read(OpenTypeFontface fontData)
        {
            Tag = fontData.ReadTag();
            CheckSum = fontData.ReadULong();
            Offset = fontData.ReadLong();
            Length = (int)fontData.ReadULong();
        }

        public void Write(OpenTypeFontWriter writer)
        {
            Debug.Assert(Tag.Length == 4);
            Debug.Assert(Offset != 0);
            Debug.Assert(Length != 0);
            writer.WriteTag(Tag);
            writer.WriteUInt(CheckSum);
            writer.WriteInt(Offset);
            writer.WriteUInt((uint)Length);
        }
    }
}
