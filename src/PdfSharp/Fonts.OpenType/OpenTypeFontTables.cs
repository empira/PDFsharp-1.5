
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
using System.Diagnostics;
using System.Text;

using Fixed = System.Int32;
using FWord = System.Int16;
using UFWord = System.UInt16;

// ReSharper disable InconsistentNaming

namespace PdfSharp.Fonts.OpenType
{
    internal enum PlatformId
    {
        Apple, Mac, Iso, Win
    }

    /// <summary>
    /// Only Symbol and Unicode is used by PDFsharp.
    /// </summary>
    internal enum WinEncodingId
    {
        Symbol, Unicode
    }

    /// <summary>
    /// CMap format 4: Segment mapping to delta values.
    /// The Windows standard format.
    /// </summary>
    internal class CMap4 : OpenTypeFontTable
    {
        public WinEncodingId encodingId; // Windows encoding ID.
        public ushort format; // Format number is set to 4.
        public ushort length; // This is the length in bytes of the subtable. 
        public ushort language; // This field must be set to zero for all cmap subtables whose platform IDs are other than Macintosh (platform ID 1). 
        public ushort segCountX2; // 2 x segCount.
        public ushort searchRange; // 2 x (2**floor(log2(segCount)))
        public ushort entrySelector; // log2(searchRange/2)
        public ushort rangeShift;
        public ushort[] endCount; // [segCount] / End characterCode for each segment, last=0xFFFF.
        public ushort[] startCount; // [segCount] / Start character code for each segment.
        public short[] idDelta; // [segCount] / Delta for all character codes in segment.
        public ushort[] idRangeOffs; // [segCount] / Offsets into glyphIdArray or 0
        public int glyphCount; // = (length - (16 + 4 * 2 * segCount)) / 2;
        public ushort[] glyphIdArray;     // Glyph index array (arbitrary length)

        public CMap4(OpenTypeFontface fontData, WinEncodingId encodingId)
            : base(fontData, "----")
        {
            this.encodingId = encodingId;
            Read();
        }

        internal void Read()
        {
            try
            {
                // m_EncodingID = encID;
                format = _fontData.ReadUShort();
                Debug.Assert(format == 4, "Only format 4 expected.");
                length = _fontData.ReadUShort();
                language = _fontData.ReadUShort();  // Always null in Windows
                segCountX2 = _fontData.ReadUShort();
                searchRange = _fontData.ReadUShort();
                entrySelector = _fontData.ReadUShort();
                rangeShift = _fontData.ReadUShort();

                int segCount = segCountX2 / 2;
                glyphCount = (length - (16 + 8 * segCount)) / 2;

                //ASSERT_CONDITION(0 <= m_NumGlyphIds && m_NumGlyphIds < m_Length, "Invalid Index");

                endCount = new ushort[segCount];
                startCount = new ushort[segCount];
                idDelta = new short[segCount];
                idRangeOffs = new ushort[segCount];

                glyphIdArray = new ushort[glyphCount];

                for (int idx = 0; idx < segCount; idx++)
                    endCount[idx] = _fontData.ReadUShort();

                //ASSERT_CONDITION(m_EndCount[segs - 1] == 0xFFFF, "Out of Index");

                // Read reserved pad.
                _fontData.ReadUShort();

                for (int idx = 0; idx < segCount; idx++)
                    startCount[idx] = _fontData.ReadUShort();

                for (int idx = 0; idx < segCount; idx++)
                    idDelta[idx] = _fontData.ReadShort();

                for (int idx = 0; idx < segCount; idx++)
                    idRangeOffs[idx] = _fontData.ReadUShort();

                for (int idx = 0; idx < glyphCount; idx++)
                    glyphIdArray[idx] = _fontData.ReadUShort();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PSSR.ErrorReadingFontData, ex);
            }
        }
    }

    /// <summary>
    /// This table defines the mapping of character codes to the glyph index values used in the font.
    /// It may contain more than one subtable, in order to support more than one character encoding scheme.
    /// </summary>
    internal class CMapTable : OpenTypeFontTable
    {
        public const string Tag = TableTagNames.CMap;

        public ushort version;
        public ushort numTables;

        /// <summary>
        /// Is true for symbol font encoding.
        /// </summary>
        public bool symbol;

        public CMap4 cmap4;

        /// <summary>
        /// Initializes a new instance of the <see cref="CMapTable"/> class.
        /// </summary>
        public CMapTable(OpenTypeFontface fontData)
            : base(fontData, Tag)
        {
            Read();
        }

        internal void Read()
        {
            try
            {
                int tableOffset = _fontData.Position;

                version = _fontData.ReadUShort();
                numTables = _fontData.ReadUShort();
#if DEBUG_
                if (_fontData.Name == "Cambria")
                    Debug-Break.Break();
#endif

                bool success = false;
                for (int idx = 0; idx < numTables; idx++)
                {
                    PlatformId platformId = (PlatformId)_fontData.ReadUShort();
                    WinEncodingId encodingId = (WinEncodingId)_fontData.ReadUShort();
                    int offset = _fontData.ReadLong();

                    int currentPosition = _fontData.Position;

                    // Just read Windows stuff.
                    if (platformId == PlatformId.Win && (encodingId == WinEncodingId.Symbol || encodingId == WinEncodingId.Unicode))
                    {
                        symbol = encodingId == WinEncodingId.Symbol;

                        _fontData.Position = tableOffset + offset;
                        cmap4 = new CMap4(_fontData, encodingId);
                        _fontData.Position = currentPosition;
                        // We have found what we are looking for, so break.
                        success = true;
                        break;
                    }
                }
                if (!success)
                    throw new InvalidOperationException("Font has no usable platform or encoding ID. It cannot be used with PDFsharp.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PSSR.ErrorReadingFontData, ex);
            }
        }
    }

    /// <summary>
    /// This table gives global information about the font. The bounding box values should be computed using 
    /// only glyphs that have contours. Glyphs with no contours should be ignored for the purposes of these calculations.
    /// </summary>
    internal class FontHeaderTable : OpenTypeFontTable
    {
        public const string Tag = TableTagNames.Head;

        public Fixed version; // 0x00010000 for Version 1.0.
        public Fixed fontRevision;
        public uint checkSumAdjustment;
        public uint magicNumber; // Set to 0x5F0F3CF5
        public ushort flags;
        public ushort unitsPerEm; // Valid range is from 16 to 16384. This value should be a power of 2 for fonts that have TrueType outlines.
        public long created;
        public long modified;
        public short xMin, yMin; // For all glyph bounding boxes.
        public short xMax, yMax; // For all glyph bounding boxes.
        public ushort macStyle;
        public ushort lowestRecPPEM;
        public short fontDirectionHint;
        public short indexToLocFormat; // 0 for short offsets, 1 for long
        public short glyphDataFormat; // 0 for current format

        public FontHeaderTable(OpenTypeFontface fontData)
            : base(fontData, Tag)
        {
            Read();
        }

        public void Read()
        {
            try
            {
                version = _fontData.ReadFixed();
                fontRevision = _fontData.ReadFixed();
                checkSumAdjustment = _fontData.ReadULong();
                magicNumber = _fontData.ReadULong();
                flags = _fontData.ReadUShort();
                unitsPerEm = _fontData.ReadUShort();
                created = _fontData.ReadLongDate();
                modified = _fontData.ReadLongDate();
                xMin = _fontData.ReadShort();
                yMin = _fontData.ReadShort();
                xMax = _fontData.ReadShort();
                yMax = _fontData.ReadShort();
                macStyle = _fontData.ReadUShort();
                lowestRecPPEM = _fontData.ReadUShort();
                fontDirectionHint = _fontData.ReadShort();
                indexToLocFormat = _fontData.ReadShort();
                glyphDataFormat = _fontData.ReadShort();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PSSR.ErrorReadingFontData, ex);
            }
        }
    }

    /// <summary>
    /// This table contains information for horizontal layout. The values in the minRightSidebearing, 
    /// MinLeftSideBearing and xMaxExtent should be computed using only glyphs that have contours.
    /// Glyphs with no contours should be ignored for the purposes of these calculations.
    /// All reserved areas must be set to 0. 
    /// </summary>
    internal class HorizontalHeaderTable : OpenTypeFontTable
    {
        public const string Tag = TableTagNames.HHea;

        public Fixed version; // 0x00010000 for Version 1.0.
        public FWord ascender; // Typographic ascent. (Distance from baseline of highest Ascender) 
        public FWord descender; // Typographic descent. (Distance from baseline of lowest Descender) 
        public FWord lineGap; // Typographic line gap. Negative LineGap values are treated as zero in Windows 3.1, System 6, and System 7.
        public UFWord advanceWidthMax;
        public FWord minLeftSideBearing;
        public FWord minRightSideBearing;
        public FWord xMaxExtent;
        public short caretSlopeRise;
        public short caretSlopeRun;
        public short reserved1;
        public short reserved2;
        public short reserved3;
        public short reserved4;
        public short reserved5;
        public short metricDataFormat;
        public ushort numberOfHMetrics;

        public HorizontalHeaderTable(OpenTypeFontface fontData)
            : base(fontData, Tag)
        {
            Read();
        }

        public void Read()
        {
            try
            {
                version = _fontData.ReadFixed();
                ascender = _fontData.ReadFWord();
                descender = _fontData.ReadFWord();
                lineGap = _fontData.ReadFWord();
                advanceWidthMax = _fontData.ReadUFWord();
                minLeftSideBearing = _fontData.ReadFWord();
                minRightSideBearing = _fontData.ReadFWord();
                xMaxExtent = _fontData.ReadFWord();
                caretSlopeRise = _fontData.ReadShort();
                caretSlopeRun = _fontData.ReadShort();
                reserved1 = _fontData.ReadShort();
                reserved2 = _fontData.ReadShort();
                reserved3 = _fontData.ReadShort();
                reserved4 = _fontData.ReadShort();
                reserved5 = _fontData.ReadShort();
                metricDataFormat = _fontData.ReadShort();
                numberOfHMetrics = _fontData.ReadUShort();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PSSR.ErrorReadingFontData, ex);
            }
        }
    }

    internal class HorizontalMetrics : OpenTypeFontTable
    {
        public const string Tag = "----";

        public ushort advanceWidth;
        public short lsb;

        public HorizontalMetrics(OpenTypeFontface fontData)
            : base(fontData, Tag)
        {
            Read();
        }

        public void Read()
        {
            try
            {
                advanceWidth = _fontData.ReadUFWord();
                lsb = _fontData.ReadFWord();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PSSR.ErrorReadingFontData, ex);
            }
        }
    }

    /// <summary>
    /// The type longHorMetric is defined as an array where each element has two parts:
    /// the advance width, which is of type USHORT, and the left side bearing, which is of type SHORT.
    /// These fields are in font design units.
    /// </summary>
    internal class HorizontalMetricsTable : OpenTypeFontTable
    {
        public const string Tag = TableTagNames.HMtx;

        public HorizontalMetrics[] Metrics;
        public FWord[] LeftSideBearing;

        public HorizontalMetricsTable(OpenTypeFontface fontData)
            : base(fontData, Tag)
        {
            Read();
        }

        public void Read()
        {
            try
            {
                HorizontalHeaderTable hhea = _fontData.hhea;
                MaximumProfileTable maxp = _fontData.maxp;
                if (hhea != null && maxp != null)
                {
                    int numMetrics = hhea.numberOfHMetrics; //->NumberOfHMetrics();
                    int numLsbs = maxp.numGlyphs - numMetrics;

                    Debug.Assert(numMetrics != 0);
                    Debug.Assert(numLsbs >= 0);

                    Metrics = new HorizontalMetrics[numMetrics];
                    for (int idx = 0; idx < numMetrics; idx++)
                        Metrics[idx] = new HorizontalMetrics(_fontData);

                    if (numLsbs > 0)
                    {
                        LeftSideBearing = new FWord[numLsbs];
                        for (int idx = 0; idx < numLsbs; idx++)
                            LeftSideBearing[idx] = _fontData.ReadFWord();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PSSR.ErrorReadingFontData, ex);
            }
        }
    }

    // UNDONE
    internal class VerticalHeaderTable : OpenTypeFontTable
    {
        public const string Tag = TableTagNames.VHea;

        // code comes from HorizontalHeaderTable
        public Fixed Version; // 0x00010000 for Version 1.0.
        public FWord Ascender; // Typographic ascent. (Distance from baseline of highest Ascender) 
        public FWord Descender; // Typographic descent. (Distance from baseline of lowest Descender) 
        public FWord LineGap; // Typographic line gap. Negative LineGap values are treated as zero in Windows 3.1, System 6, and System 7.
        public UFWord AdvanceWidthMax;
        public FWord MinLeftSideBearing;
        public FWord MinRightSideBearing;
        public FWord xMaxExtent;
        public short caretSlopeRise;
        public short caretSlopeRun;
        public short reserved1;
        public short reserved2;
        public short reserved3;
        public short reserved4;
        public short reserved5;
        public short metricDataFormat;
        public ushort numberOfHMetrics;

        public VerticalHeaderTable(OpenTypeFontface fontData)
            : base(fontData, Tag)
        {
            Read();
        }

        public void Read()
        {
            try
            {
                Version = _fontData.ReadFixed();
                Ascender = _fontData.ReadFWord();
                Descender = _fontData.ReadFWord();
                LineGap = _fontData.ReadFWord();
                AdvanceWidthMax = _fontData.ReadUFWord();
                MinLeftSideBearing = _fontData.ReadFWord();
                MinRightSideBearing = _fontData.ReadFWord();
                xMaxExtent = _fontData.ReadFWord();
                caretSlopeRise = _fontData.ReadShort();
                caretSlopeRun = _fontData.ReadShort();
                reserved1 = _fontData.ReadShort();
                reserved2 = _fontData.ReadShort();
                reserved3 = _fontData.ReadShort();
                reserved4 = _fontData.ReadShort();
                reserved5 = _fontData.ReadShort();
                metricDataFormat = _fontData.ReadShort();
                numberOfHMetrics = _fontData.ReadUShort();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PSSR.ErrorReadingFontData, ex);
            }
        }
    }

    internal class VerticalMetrics : OpenTypeFontTable
    {
        public const string Tag = "----";

        // code comes from HorizontalMetrics
        public ushort advanceWidth;
        public short lsb;

        public VerticalMetrics(OpenTypeFontface fontData)
            : base(fontData, Tag)
        {
            Read();
        }

        public void Read()
        {
            try
            {
                advanceWidth = _fontData.ReadUFWord();
                lsb = _fontData.ReadFWord();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PSSR.ErrorReadingFontData, ex);
            }
        }
    }

    /// <summary>
    /// The vertical Metrics table allows you to specify the vertical spacing for each glyph in a
    /// vertical font. This table consists of either one or two arrays that contain metric
    /// information (the advance heights and top sidebearings) for the vertical layout of each
    /// of the glyphs in the font.
    /// </summary>
    internal class VerticalMetricsTable : OpenTypeFontTable
    {
        // UNDONE
        public const string Tag = TableTagNames.VMtx;

        // code comes from HorizontalMetricsTable
        public HorizontalMetrics[] metrics;
        public FWord[] leftSideBearing;

        public VerticalMetricsTable(OpenTypeFontface fontData)
            : base(fontData, Tag)
        {
            Read();
            throw new NotImplementedException("VerticalMetricsTable");
        }

        public void Read()
        {
            try
            {
                HorizontalHeaderTable hhea = _fontData.hhea;
                MaximumProfileTable maxp = _fontData.maxp;
                if (hhea != null && maxp != null)
                {
                    int numMetrics = hhea.numberOfHMetrics; //->NumberOfHMetrics();
                    int numLsbs = maxp.numGlyphs - numMetrics;

                    Debug.Assert(numMetrics != 0);
                    Debug.Assert(numLsbs >= 0);

                    metrics = new HorizontalMetrics[numMetrics];
                    for (int idx = 0; idx < numMetrics; idx++)
                        metrics[idx] = new HorizontalMetrics(_fontData);

                    if (numLsbs > 0)
                    {
                        leftSideBearing = new FWord[numLsbs];
                        for (int idx = 0; idx < numLsbs; idx++)
                            leftSideBearing[idx] = _fontData.ReadFWord();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PSSR.ErrorReadingFontData, ex);
            }
        }
    }

    /// <summary>
    /// This table establishes the memory requirements for this font.
    /// Fonts with CFF data must use Version 0.5 of this table, specifying only the numGlyphs field.
    /// Fonts with TrueType outlines must use Version 1.0 of this table, where all data is required.
    /// Both formats of OpenType require a 'maxp' table because a number of applications call the 
    /// Windows GetFontData() API on the 'maxp' table to determine the number of glyphs in the font.
    /// </summary>
    internal class MaximumProfileTable : OpenTypeFontTable
    {
        public const string Tag = TableTagNames.MaxP;

        public Fixed version;
        public ushort numGlyphs;
        public ushort maxPoints;
        public ushort maxContours;
        public ushort maxCompositePoints;
        public ushort maxCompositeContours;
        public ushort maxZones;
        public ushort maxTwilightPoints;
        public ushort maxStorage;
        public ushort maxFunctionDefs;
        public ushort maxInstructionDefs;
        public ushort maxStackElements;
        public ushort maxSizeOfInstructions;
        public ushort maxComponentElements;
        public ushort maxComponentDepth;

        public MaximumProfileTable(OpenTypeFontface fontData)
            : base(fontData, Tag)
        {
            Read();
        }

        public void Read()
        {
            try
            {
                version = _fontData.ReadFixed();
                numGlyphs = _fontData.ReadUShort();
                maxPoints = _fontData.ReadUShort();
                maxContours = _fontData.ReadUShort();
                maxCompositePoints = _fontData.ReadUShort();
                maxCompositeContours = _fontData.ReadUShort();
                maxZones = _fontData.ReadUShort();
                maxTwilightPoints = _fontData.ReadUShort();
                maxStorage = _fontData.ReadUShort();
                maxFunctionDefs = _fontData.ReadUShort();
                maxInstructionDefs = _fontData.ReadUShort();
                maxStackElements = _fontData.ReadUShort();
                maxSizeOfInstructions = _fontData.ReadUShort();
                maxComponentElements = _fontData.ReadUShort();
                maxComponentDepth = _fontData.ReadUShort();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PSSR.ErrorReadingFontData, ex);
            }
        }
    }

    /// <summary>
    /// The naming table allows multilingual strings to be associated with the OpenTypeTM font file.
    /// These strings can represent copyright notices, font names, family names, style names, and so on.
    /// To keep this table short, the font manufacturer may wish to make a limited set of entries in some
    /// small set of languages; later, the font can be "localized" and the strings translated or added.
    /// Other parts of the OpenType font file that require these strings can then refer to them simply by
    /// their index number. Clients that need a particular string can look it up by its platform ID, character
    /// encoding ID, language ID and name ID. Note that some platforms may require single byte character
    /// strings, while others may require double byte strings. 
    ///
    /// For historical reasons, some applications which install fonts perform Version control using Macintosh
    /// platform (platform ID 1) strings from the 'name' table. Because of this, we strongly recommend that
    /// the 'name' table of all fonts include Macintosh platform strings and that the syntax of the Version
    /// number (name id 5) follows the guidelines given in this document.
    /// </summary>
    internal class NameTable : OpenTypeFontTable
    {
        public const string Tag = TableTagNames.Name;

        /// <summary>
        /// Get the font family name.
        /// </summary>
        public string Name = String.Empty;

        /// <summary>
        /// Get the font subfamily name.
        /// </summary>
        public string Style = String.Empty;

        /// <summary>
        /// Get the full font name.
        /// </summary>
        public string FullFontName = String.Empty;

        public ushort format;
        public ushort count;
        public ushort stringOffset;

        byte[] bytes;

        public NameTable(OpenTypeFontface fontData)
            : base(fontData, Tag)
        {
            Read();
        }

        public void Read()
        {
            try
            {
#if DEBUG
                _fontData.Position = DirectoryEntry.Offset;
#endif
                bytes = new byte[DirectoryEntry.PaddedLength];
                Buffer.BlockCopy(_fontData.FontSource.Bytes, DirectoryEntry.Offset, bytes, 0, DirectoryEntry.Length);

                format = _fontData.ReadUShort();
                count = _fontData.ReadUShort();
                stringOffset = _fontData.ReadUShort();

                for (int idx = 0; idx < count; idx++)
                {
                    NameRecord nrec = ReadNameRecord();
                    byte[] value = new byte[nrec.length];
                    Buffer.BlockCopy(_fontData.FontSource.Bytes, DirectoryEntry.Offset + stringOffset + nrec.offset, value, 0, nrec.length);

                    //Debug.WriteLine(nrec.platformID.ToString());

                    // Read font name and style in US English.
                    if (nrec.platformID == 0 || nrec.platformID == 3)
                    {
                        // Font Family name. Up to four fonts can share the Font Family name, 
                        // forming a font style linking group (regular, italic, bold, bold italic - 
                        // as defined by OS/2.fsSelection bit settings).
                        if (nrec.nameID == 1 && nrec.languageID == 0x0409)
                        {
                            if (String.IsNullOrEmpty(Name))
                                Name = Encoding.BigEndianUnicode.GetString(value, 0, value.Length);
                        }

                        // Font Subfamily name. The Font Subfamily name distinguishes the font in a 
                        // group with the same Font Family name (name ID 1). This is assumed to
                        // address style (italic, oblique) and weight (light, bold, black, etc.).
                        // A font with no particular differences in weight or style (e.g. medium weight,
                        // not italic and fsSelection bit 6 set) should have the string “Regular” stored in 
                        // this position.
                        if (nrec.nameID == 2 && nrec.languageID == 0x0409)
                        {
                            if (String.IsNullOrEmpty(Style))
                                Style = Encoding.BigEndianUnicode.GetString(value, 0, value.Length);
                        }

                        // Full font name; a combination of strings 1 and 2, or a similar human-readable
                        // variant. If string 2 is "Regular", it is sometimes omitted from name ID 4.
                        if (nrec.nameID == 4 && nrec.languageID == 0x0409)
                        {
                            if (String.IsNullOrEmpty(FullFontName))
                                FullFontName = Encoding.BigEndianUnicode.GetString(value, 0, value.Length);
                        }
                    }
                }
                Debug.Assert(!String.IsNullOrEmpty(Name));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PSSR.ErrorReadingFontData, ex);
            }
        }

        NameRecord ReadNameRecord()
        {
            NameRecord nrec = new NameRecord();
            nrec.platformID = _fontData.ReadUShort();
            nrec.encodingID = _fontData.ReadUShort();
            nrec.languageID = _fontData.ReadUShort();
            nrec.nameID = _fontData.ReadUShort();
            nrec.length = _fontData.ReadUShort();
            nrec.offset = _fontData.ReadUShort();
            return nrec;
        }

        class NameRecord
        {
            public ushort platformID;
            public ushort encodingID;
            public ushort languageID;
            public ushort nameID;
            public ushort length;
            public ushort offset;
        }
    }

    /// <summary>
    /// The OS/2 table consists of a set of Metrics that are required in OpenType fonts. 
    /// </summary>
    internal class OS2Table : OpenTypeFontTable
    {
        public const string Tag = TableTagNames.OS2;

        [Flags]
        public enum FontSelectionFlags : ushort
        {
            Italic = 1 << 0,
            Bold = 1 << 5,
            Regular = 1 << 6,
        }

        public ushort version;
        public short xAvgCharWidth;
        public ushort usWeightClass;
        public ushort usWidthClass;
        public ushort fsType;
        public short ySubscriptXSize;
        public short ySubscriptYSize;
        public short ySubscriptXOffset;
        public short ySubscriptYOffset;
        public short ySuperscriptXSize;
        public short ySuperscriptYSize;
        public short ySuperscriptXOffset;
        public short ySuperscriptYOffset;
        public short yStrikeoutSize;
        public short yStrikeoutPosition;
        public short sFamilyClass;
        public byte[] panose; // = new byte[10];
        public uint ulUnicodeRange1; // Bits 0-31
        public uint ulUnicodeRange2; // Bits 32-63
        public uint ulUnicodeRange3; // Bits 64-95
        public uint ulUnicodeRange4; // Bits 96-127
        public string achVendID; // = "";
        public ushort fsSelection;
        public ushort usFirstCharIndex;
        public ushort usLastCharIndex;
        public short sTypoAscender;
        public short sTypoDescender;
        public short sTypoLineGap;
        public ushort usWinAscent;
        public ushort usWinDescent;
        // Version >= 1
        public uint ulCodePageRange1; // Bits 0-31
        public uint ulCodePageRange2; // Bits 32-63
        // Version >= 2
        public short sxHeight;
        public short sCapHeight;
        public ushort usDefaultChar;
        public ushort usBreakChar;
        public ushort usMaxContext;

        public OS2Table(OpenTypeFontface fontData)
            : base(fontData, Tag)
        {
            Read();
        }

        public void Read()
        {
            try
            {
                version = _fontData.ReadUShort();
                xAvgCharWidth = _fontData.ReadShort();
                usWeightClass = _fontData.ReadUShort();
                usWidthClass = _fontData.ReadUShort();
                fsType = _fontData.ReadUShort();
                ySubscriptXSize = _fontData.ReadShort();
                ySubscriptYSize = _fontData.ReadShort();
                ySubscriptXOffset = _fontData.ReadShort();
                ySubscriptYOffset = _fontData.ReadShort();
                ySuperscriptXSize = _fontData.ReadShort();
                ySuperscriptYSize = _fontData.ReadShort();
                ySuperscriptXOffset = _fontData.ReadShort();
                ySuperscriptYOffset = _fontData.ReadShort();
                yStrikeoutSize = _fontData.ReadShort();
                yStrikeoutPosition = _fontData.ReadShort();
                sFamilyClass = _fontData.ReadShort();
                panose = _fontData.ReadBytes(10);
                ulUnicodeRange1 = _fontData.ReadULong();
                ulUnicodeRange2 = _fontData.ReadULong();
                ulUnicodeRange3 = _fontData.ReadULong();
                ulUnicodeRange4 = _fontData.ReadULong();
                achVendID = _fontData.ReadString(4);
                fsSelection = _fontData.ReadUShort();
                usFirstCharIndex = _fontData.ReadUShort();
                usLastCharIndex = _fontData.ReadUShort();
                sTypoAscender = _fontData.ReadShort();
                sTypoDescender = _fontData.ReadShort();
                sTypoLineGap = _fontData.ReadShort();
                usWinAscent = _fontData.ReadUShort();
                usWinDescent = _fontData.ReadUShort();

                if (version >= 1)
                {
                    ulCodePageRange1 = _fontData.ReadULong();
                    ulCodePageRange2 = _fontData.ReadULong();

                    if (version >= 2)
                    {
                        sxHeight = _fontData.ReadShort();
                        sCapHeight = _fontData.ReadShort();
                        usDefaultChar = _fontData.ReadUShort();
                        usBreakChar = _fontData.ReadUShort();
                        usMaxContext = _fontData.ReadUShort();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PSSR.ErrorReadingFontData, ex);
            }
        }

        public bool IsBold
        {
            get { return (fsSelection & (ushort)FontSelectionFlags.Bold) != 0; }
        }

        public bool IsItalic
        {
            get { return (fsSelection & (ushort)FontSelectionFlags.Italic) != 0; }
        }
    }

    /// <summary>
    /// This table contains additional information needed to use TrueType or OpenTypeTM fonts
    /// on PostScript printers. 
    /// </summary>
    internal class PostScriptTable : OpenTypeFontTable
    {
        public const string Tag = TableTagNames.Post;

        public Fixed formatType;
        public float italicAngle;
        public FWord underlinePosition;
        public FWord underlineThickness;
        public ulong isFixedPitch;
        public ulong minMemType42;
        public ulong maxMemType42;
        public ulong minMemType1;
        public ulong maxMemType1;

        public PostScriptTable(OpenTypeFontface fontData)
            : base(fontData, Tag)
        {
            Read();
        }

        public void Read()
        {
            try
            {
                formatType = _fontData.ReadFixed();
                italicAngle = _fontData.ReadFixed() / 65536f;
                underlinePosition = _fontData.ReadFWord();
                underlineThickness = _fontData.ReadFWord();
                isFixedPitch = _fontData.ReadULong();
                minMemType42 = _fontData.ReadULong();
                maxMemType42 = _fontData.ReadULong();
                minMemType1 = _fontData.ReadULong();
                maxMemType1 = _fontData.ReadULong();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PSSR.ErrorReadingFontData, ex);
            }
        }
    }

    /// <summary>
    /// This table contains a list of values that can be referenced by instructions.
    /// They can be used, among other things, to control characteristics for different glyphs.
    /// The length of the table must be an integral number of FWORD units. 
    /// </summary>
    internal class ControlValueTable : OpenTypeFontTable
    {
        public const string Tag = TableTagNames.Cvt;

        FWord[] array; // List of n values referenceable by instructions. n is the number of FWORD items that fit in the size of the table.

        public ControlValueTable(OpenTypeFontface fontData)
            : base(fontData, Tag)
        {
            DirectoryEntry.Tag = TableTagNames.Cvt;
            DirectoryEntry = fontData.TableDictionary[TableTagNames.Cvt];
            Read();
        }

        public void Read()
        {
            try
            {
                int length = DirectoryEntry.Length / 2;
                array = new FWord[length];
                for (int idx = 0; idx < length; idx++)
                    array[idx] = _fontData.ReadFWord();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PSSR.ErrorReadingFontData, ex);
            }
        }
    }

    /// <summary>
    /// This table is similar to the CVT Program, except that it is only run once, when the font is first used.
    /// It is used only for FDEFs and IDEFs. Thus the CVT Program need not contain function definitions.
    /// However, the CVT Program may redefine existing FDEFs or IDEFs. 
    /// </summary>
    internal class FontProgram : OpenTypeFontTable
    {
        public const string Tag = TableTagNames.Fpgm;

        byte[] bytes; // Instructions. n is the number of BYTE items that fit in the size of the table.

        public FontProgram(OpenTypeFontface fontData)
            : base(fontData, Tag)
        {
            DirectoryEntry.Tag = TableTagNames.Fpgm;
            DirectoryEntry = fontData.TableDictionary[TableTagNames.Fpgm];
            Read();
        }

        public void Read()
        {
            try
            {
                int length = DirectoryEntry.Length;
                bytes = new byte[length];
                for (int idx = 0; idx < length; idx++)
                    bytes[idx] = _fontData.ReadByte();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PSSR.ErrorReadingFontData, ex);
            }
        }
    }

    /// <summary>
    /// The Control Value Program consists of a set of TrueType instructions that will be executed whenever the font or 
    /// point size or transformation matrix change and before each glyph is interpreted. Any instruction is legal in the
    /// CVT Program but since no glyph is associated with it, instructions intended to move points within a particular
    /// glyph outline cannot be used in the CVT Program. The name 'prep' is anachronistic. 
    /// </summary>
    internal class ControlValueProgram : OpenTypeFontTable
    {
        public const string Tag = TableTagNames.Prep;

        byte[] bytes; // Set of instructions executed whenever point size or font or transformation change. n is the number of BYTE items that fit in the size of the table.

        public ControlValueProgram(OpenTypeFontface fontData)
            : base(fontData, Tag)
        {
            DirectoryEntry.Tag = TableTagNames.Prep;
            DirectoryEntry = fontData.TableDictionary[TableTagNames.Prep];
            Read();
        }

        public void Read()
        {
            try
            {
                int length = DirectoryEntry.Length;
                bytes = new byte[length];
                for (int idx = 0; idx < length; idx++)
                    bytes[idx] = _fontData.ReadByte();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PSSR.ErrorReadingFontData, ex);
            }
        }
    }

    /// <summary>
    /// This table contains information that describes the glyphs in the font in the TrueType outline format.
    /// Information regarding the rasterizer (scaler) refers to the TrueType rasterizer. 
    /// </summary>
    internal class GlyphSubstitutionTable : OpenTypeFontTable
    {
        public const string Tag = TableTagNames.GSUB;

        public GlyphSubstitutionTable(OpenTypeFontface fontData)
            : base(fontData, Tag)
        {
            DirectoryEntry.Tag = TableTagNames.GSUB;
            DirectoryEntry = fontData.TableDictionary[TableTagNames.GSUB];
            Read();
        }

        public void Read()
        {
            try
            {
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(PSSR.ErrorReadingFontData, ex);
            }
        }
    }
}
