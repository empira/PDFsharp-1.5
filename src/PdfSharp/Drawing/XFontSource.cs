#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using PdfSharp.Fonts;
#if CORE || GDI
using GdiFont = System.Drawing.Font;
using GdiFontStyle = System.Drawing.FontStyle;
#endif
using PdfSharp.Internal;
using PdfSharp.Fonts.OpenType;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace PdfSharp.Drawing
{
    /// <summary>
    /// The bytes of a font file.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    internal class XFontSource
    {
        // Implementation Notes
        // 
        // * XFontSource represents a single font (file) in memory.
        // * An XFontSource hold a reference to it OpenTypeFontface.
        // * To prevent large heap fragmentation this class must exists only once.
        // * TODO: ttcf

        // Signature of a true type collection font.
        const uint ttcf = 0x66637474;

        /// <summary>
        /// (fontName, fileName)
        /// </summary>
        internal static Dictionary<string, string> FontFilePaths = new Dictionary<string, string>();
        static XFontSource()
        {
            var searchingPaths = new List<string>();
            // Application
            var executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            searchingPaths.Add(executingPath);
            // Windows
            var systemFontsPath = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            if (!string.IsNullOrWhiteSpace(systemFontsPath))
            {
                searchingPaths.Add(systemFontsPath);
            }
            else
            {
                // Debian
                searchingPaths.Add("/usr/share/fonts/truetype");
                //searchingPaths.Add("/usr/share/X11/fonts");
                //searchingPaths.Add("/usr/X11R6/lib/X11/fonts");
                //searchingPaths.Add("~/.fonts");
            }

            foreach (var searchingPath in searchingPaths)
            {
                // TODO: *.ttc not supported yet!
                var fileNames = new List<string>();
                try
                {
                    fileNames = Directory.GetFiles(searchingPath, "*.ttf", SearchOption.AllDirectories).ToList();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    continue;
                }

                foreach (var fileName in fileNames)
                {
                    try
                    {
                        var fontCollection = new System.Drawing.Text.PrivateFontCollection();
                        fontCollection.AddFontFile(fileName);
                        var fontName = fontCollection.Families[0].Name;
                        if (!FontFilePaths.ContainsKey(fontName))
                        {
                            FontFilePaths.Add(fontName, fileName);
                            Debug.WriteLine($"Add font {fontName}: {fileName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
        }

        XFontSource(byte[] bytes, ulong key)
        {
            _fontName = null;
            _bytes = bytes;
            _key = key;
        }

        /// <summary>
        /// Gets an existing font source or creates a new one.
        /// A new font source is cached in font factory.
        /// </summary>
        public static XFontSource GetOrCreateFrom(byte[] bytes)
        {
            ulong key = FontHelper.CalcChecksum(bytes);
            XFontSource fontSource;
            if (!FontFactory.TryGetFontSourceByKey(key, out fontSource))
            {
                fontSource = new XFontSource(bytes, key);
                // Theoretically the font source could be created by a differend thread in the meantime.
                fontSource = FontFactory.CacheFontSource(fontSource);
            }
            return fontSource;
        }

        internal static XFontSource GetOrCreateFromGdi(string typefaceKey, GdiFont gdiFont)
        {
            byte[] bytes = ReadFontBytesFromGdi(gdiFont);
            XFontSource fontSource = GetOrCreateFrom(typefaceKey, bytes);
            return fontSource;
        }

        static byte[] ReadFontBytesFromGdi(GdiFont gdiFont)
        {
            var assembly = Assembly.GetAssembly(typeof(XFontSource));
            var fontName = gdiFont.Name;
            Stream fontStream = null;
            {
                if (FontFilePaths.ContainsKey(fontName))
                {
                    var fileName = FontFilePaths[fontName];
                    fontStream = File.OpenRead(fileName);
                    Debug.WriteLine($"{fileName} retrieved.");
                }
            }

            // 3.default
            if (fontStream == null)
            {
                fontStream = assembly.GetManifestResourceStream($"PdfSharp.Assets.stsong.ttf");
                Debug.WriteLine($"{fontName} not found, replaced by default stsong.ttf");
            }

            // to output
            var fontData = new byte[fontStream.Length];
            fontStream.Read(fontData, 0, (int)fontStream.Length);
            return fontData;
        }

        static XFontSource GetOrCreateFrom(string typefaceKey, byte[] fontBytes)
        {
            XFontSource fontSource;
            ulong key = FontHelper.CalcChecksum(fontBytes);
            if (FontFactory.TryGetFontSourceByKey(key, out fontSource))
            {
                // The font source already exists, but is not yet cached under the specified typeface key.
                FontFactory.CacheExistingFontSourceWithNewTypefaceKey(typefaceKey, fontSource);
            }
            else
            {
                // No font source exists. Create new one and cache it.
                fontSource = new XFontSource(fontBytes, key);
                FontFactory.CacheNewFontSource(typefaceKey, fontSource);
            }
            return fontSource;
        }

        public static XFontSource CreateCompiledFont(byte[] bytes)
        {
            XFontSource fontSource = new XFontSource(bytes, 0);
            return fontSource;
        }

        /// <summary>
        /// Gets or sets the fontface.
        /// </summary>
        internal OpenTypeFontface Fontface
        {
            get { return _fontface; }
            set
            {
                _fontface = value;
                _fontName = value.name.FullFontName;
            }
        }
        OpenTypeFontface _fontface;

        /// <summary>
        /// Gets the key that uniquely identifies this font source.
        /// </summary>
        internal ulong Key
        {
            get
            {
                if (_key == 0)
                    _key = FontHelper.CalcChecksum(Bytes);
                return _key;
            }
        }
        ulong _key;

        public void IncrementKey()
        {
            // HACK: Depends on implementation of CalcChecksum.
            // Increment check sum and keep length untouched.
            _key += 1ul << 32;
        }

        /// <summary>
        /// Gets the name of the font's name table.
        /// </summary>
        public string FontName
        {
            get { return _fontName; }
        }
        string _fontName;

        /// <summary>
        /// Gets the bytes of the font.
        /// </summary>
        public byte[] Bytes
        {
            get { return _bytes; }
        }
        readonly byte[] _bytes;

        public override int GetHashCode()
        {
            return (int)((Key >> 32) ^ Key);
        }

        public override bool Equals(object obj)
        {
            XFontSource fontSource = obj as XFontSource;
            if (fontSource == null)
                return false;
            return Key == fontSource.Key;
        }

        /// <summary>
        /// Gets the DebuggerDisplayAttribute text.
        /// </summary>
        // ReSha rper disable UnusedMember.Local
        internal string DebuggerDisplay
        // ReShar per restore UnusedMember.Local
        {
            // The key is converted to a value a human can remember during debugging.
            get { return String.Format(CultureInfo.InvariantCulture, "XFontSource: '{0}', keyhash={1}", FontName, Key % 99991 /* largest prime number less than 100000 */); }
        }
    }
}
