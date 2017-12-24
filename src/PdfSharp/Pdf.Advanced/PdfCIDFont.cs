
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

using PdfSharp.Drawing;
using PdfSharp.Pdf.Filters;
using PdfSharp.Fonts.OpenType;

namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// Represents a CIDFont dictionary.
    /// </summary>
    internal class PdfCIDFont : PdfFont
    {
        public PdfCIDFont(PdfDocument document)
            : base(document)
        { }

        public PdfCIDFont(PdfDocument document, PdfFontDescriptor fontDescriptor, XFont font)
            : base(document)
        {
            Elements.SetName(Keys.Type, "/Font");
            Elements.SetName(Keys.Subtype, "/CIDFontType2");
            PdfDictionary cid = new PdfDictionary();
            cid.Elements.SetString("/Ordering", "Identity");
            cid.Elements.SetString("/Registry", "Adobe");
            cid.Elements.SetInteger("/Supplement", 0);
            Elements.SetValue(Keys.CIDSystemInfo, cid);

            FontDescriptor = fontDescriptor;
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            Owner._irefTable.Add(fontDescriptor);
            Elements[Keys.FontDescriptor] = fontDescriptor.Reference;

            FontEncoding = font.PdfOptions.FontEncoding;
        }

        public PdfCIDFont(PdfDocument document, PdfFontDescriptor fontDescriptor, byte[] fontData)
            : base(document)
        {
            Elements.SetName(Keys.Type, "/Font");
            Elements.SetName(Keys.Subtype, "/CIDFontType2");
            PdfDictionary cid = new PdfDictionary();
            cid.Elements.SetString("/Ordering", "Identity");
            cid.Elements.SetString("/Registry", "Adobe");
            cid.Elements.SetInteger("/Supplement", 0);
            Elements.SetValue(Keys.CIDSystemInfo, cid);

            FontDescriptor = fontDescriptor;
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            Owner._irefTable.Add(fontDescriptor);
            Elements[Keys.FontDescriptor] = fontDescriptor.Reference;

            FontEncoding = PdfFontEncoding.Unicode;
        }

        public string BaseFont
        {
            get { return Elements.GetName(Keys.BaseFont); }
            set { Elements.SetName(Keys.BaseFont, value); }
        }

        /// <summary>
        /// Prepares the object to get saved.
        /// </summary>
        internal override void PrepareForSave()
        {
            base.PrepareForSave();

#if DEBUG_
            if (FontDescriptor._descriptor.FontFace.loca == null)
            {
                GetType();
            }
#endif
            // CID fonts must be always embedded. PDFsharp embeds automatically a subset.
            OpenTypeFontface subSet = null;
            if (FontDescriptor._descriptor.FontFace.loca == null)
                subSet = FontDescriptor._descriptor.FontFace;
            else
                subSet = FontDescriptor._descriptor.FontFace.CreateFontSubSet(_cmapInfo.GlyphIndices, true);
            byte[] fontData = subSet.FontSource.Bytes;
            PdfDictionary fontStream = new PdfDictionary(Owner);
            Owner.Internals.AddObject(fontStream);
            FontDescriptor.Elements[PdfFontDescriptor.Keys.FontFile2] = fontStream.Reference;

            fontStream.Elements["/Length1"] = new PdfInteger(fontData.Length);
            if (!Owner.Options.NoCompression)
            {
                fontData = Filtering.FlateDecode.Encode(fontData, _document.Options.FlateEncodeMode);
                fontStream.Elements["/Filter"] = new PdfName("/FlateDecode");
            }
            fontStream.Elements["/Length"] = new PdfInteger(fontData.Length);
            fontStream.CreateStream(fontData);
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        public new sealed class Keys : PdfFont.Keys
        {
            /// <summary>
            /// (Required) The type of PDF object that this dictionary describes;
            /// must be Font for a CIDFont dictionary.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required, FixedValue = "Font")]
            public new const string Type = "/Type";

            /// <summary>
            /// (Required) The type of CIDFont; CIDFontType0 or CIDFontType2.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required)]
            public new const string Subtype = "/Subtype";

            /// <summary>
            /// (Required) The PostScript name of the CIDFont. For Type 0 CIDFonts, this
            /// is usually the value of the CIDFontName entry in the CIDFont program. For
            /// Type 2 CIDFonts, it is derived the same way as for a simple TrueType font;
            /// In either case, the name can have a subset prefix if appropriate.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required)]
            public new const string BaseFont = "/BaseFont";

            /// <summary>
            /// (Required) A dictionary containing entries that define the character collection
            /// of the CIDFont.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Required)]
            public const string CIDSystemInfo = "/CIDSystemInfo";

            /// <summary>
            /// (Required; must be an indirect reference) A font descriptor describing the
            /// CIDFont’s default metrics other than its glyph widths.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.MustBeIndirect, typeof(PdfFontDescriptor))]
            public new const string FontDescriptor = "/FontDescriptor";

            /// <summary>
            /// (Optional) The default width for glyphs in the CIDFont.
            /// Default value: 1000.
            /// </summary>
            [KeyInfo(KeyType.Integer)]
            public const string DW = "/DW";

            /// <summary>
            /// (Optional) A description of the widths for the glyphs in the CIDFont. The
            /// array’s elements have a variable format that can specify individual widths
            /// for consecutive CIDs or one width for a range of CIDs.
            /// Default value: none (the DW value is used for all glyphs).
            /// </summary>
            [KeyInfo(KeyType.Array, typeof(PdfArray))]
            public const string W = "/W";

            /// <summary>
            /// (Optional; applies only to CIDFonts used for vertical writing) An array of two
            /// numbers specifying the default metrics for vertical writing.
            /// Default value: [880 −1000].
            /// </summary>
            [KeyInfo(KeyType.Array)]
            public const string DW2 = "/DW2";

            /// <summary>
            /// (Optional; applies only to CIDFonts used for vertical writing) A description
            /// of the metrics for vertical writing for the glyphs in the CIDFont.
            /// Default value: none (the DW2 value is used for all glyphs).
            /// </summary>
            [KeyInfo(KeyType.Array, typeof(PdfArray))]
            public const string W2 = "/W2";

            /// <summary>
            /// (Optional; Type 2 CIDFonts only) A specification of the mapping from CIDs
            /// to glyph indices. If the value is a stream, the bytes in the stream contain the
            /// mapping from CIDs to glyph indices: the glyph index for a particular CID
            /// value c is a 2-byte value stored in bytes 2 × c and 2 × c + 1, where the first
            /// byte is the high-order byte. If the value of CIDToGIDMap is a name, it must
            /// be Identity, indicating that the mapping between CIDs and glyph indices is
            /// the identity mapping.
            /// Default value: Identity.
            /// This entry may appear only in a Type 2 CIDFont whose associated True-Type font 
            /// program is embedded in the PDF file.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.StreamOrName)]
            public const string CIDToGIDMap = "/CIDToGIDMap";

            /// <summary>
            /// Gets the KeysMeta for these keys.
            /// </summary>
            internal static DictionaryMeta Meta
            {
                get { return _meta ?? (_meta = CreateMeta(typeof(Keys))); }
            }
            static DictionaryMeta _meta;
        }

        /// <summary>
        /// Gets the KeysMeta of this dictionary type.
        /// </summary>
        internal override DictionaryMeta Meta
        {
            get { return Keys.Meta; }
        }
    }
}
