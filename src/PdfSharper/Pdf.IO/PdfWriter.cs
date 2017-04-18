#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
//
// http://www.PdfSharper.com
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
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.IO;
using PdfSharper.Pdf.Advanced;
using PdfSharper.Pdf.Security;
using PdfSharper.Pdf.Internal;

namespace PdfSharper.Pdf.IO
{
    /// <summary>
    /// Represents a writer for generation of PDF streams. 
    /// </summary>
    public class PdfWriter
    {
        public PdfWriter(Stream pdfStream, PdfStandardSecurityHandler securityHandler)
        {
            _stream = pdfStream;
            _securityHandler = securityHandler;
            //System.Xml.XmlTextWriter
#if DEBUG
            _layout = PdfWriterLayout.Verbose;
#endif
        }

        public void Close(bool closeUnderlyingStream)
        {
            if (_stream != null && closeUnderlyingStream)
#if UWP
            _stream.Dispose();
#else
                _stream.Close();
#endif
            _stream = null;
        }

        public void Close()
        {
            Close(true);
        }

        public int Position
        {
            get { return (int)_stream.Position; }
        }

        /// <summary>
        /// Gets or sets the kind of layout.
        /// </summary>
        public PdfWriterLayout Layout
        {
            get { return _layout; }
            set { _layout = value; }
        }
        PdfWriterLayout _layout;

        public PdfWriterOptions Options
        {
            get { return _options; }
            set { _options = value; }
        }
        PdfWriterOptions _options;

        // -----------------------------------------------------------

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(bool value)
        {
            WriteSeparator(CharCat.Character);
            WriteRaw(value ? bool.TrueString : bool.FalseString);
            _lastCat = CharCat.Character;
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(PdfBoolean value)
        {
            WriteSeparator(CharCat.Character);
            WriteRaw(value.Value ? "true" : "false");
            _lastCat = CharCat.Character;
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(int value)
        {
            WriteSeparator(CharCat.Character);
            WriteRaw(value.ToString(CultureInfo.InvariantCulture));
            _lastCat = CharCat.Character;
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(uint value)
        {
            WriteSeparator(CharCat.Character);
            WriteRaw(value.ToString(CultureInfo.InvariantCulture));
            _lastCat = CharCat.Character;
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(PdfInteger value)
        {
            WriteSeparator(CharCat.Character);
            _lastCat = CharCat.Character;
            WriteRaw(value.Value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(PdfUInteger value)
        {
            WriteSeparator(CharCat.Character);
            _lastCat = CharCat.Character;
            WriteRaw(value.Value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(double value)
        {
            WriteSeparator(CharCat.Character);
            WriteRaw(value.ToString(Config.SignificantFigures7, CultureInfo.InvariantCulture));
            _lastCat = CharCat.Character;
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(PdfReal value)
        {
            WriteSeparator(CharCat.Character);
            WriteRaw(value.Value.ToString(Config.SignificantFigures7, CultureInfo.InvariantCulture));
            _lastCat = CharCat.Character;
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(PdfString value)
        {
            WriteSeparator(CharCat.Delimiter);
#if true
            PdfStringEncoding encoding = (PdfStringEncoding)(value.Flags & PdfStringFlags.EncodingMask);
            string pdf = (value.Flags & PdfStringFlags.HexLiteral) == 0 ?
                PdfEncoders.ToStringLiteral(value.Value, encoding, SecurityHandler) :
                PdfEncoders.ToHexStringLiteral(value.Value, encoding, SecurityHandler, value.PaddingLeft, value.HexUpperCase);
            WriteRaw(pdf);
#else
            switch (value.Flags & PdfStringFlags.EncodingMask)
            {
                case PdfStringFlags.Undefined:
                case PdfStringFlags.PDFDocEncoding:
                    if ((value.Flags & PdfStringFlags.HexLiteral) == 0)
                        WriteRaw(PdfEncoders.DocEncode(value.Value, false));
                    else
                        WriteRaw(PdfEncoders.DocEncodeHex(value.Value, false));
                    break;

                case PdfStringFlags.WinAnsiEncoding:
                    throw new NotImplementedException("Unexpected encoding: WinAnsiEncoding");

                case PdfStringFlags.Unicode:
                    if ((value.Flags & PdfStringFlags.HexLiteral) == 0)
                        WriteRaw(PdfEncoders.DocEncode(value.Value, true));
                    else
                        WriteRaw(PdfEncoders.DocEncodeHex(value.Value, true));
                    break;

                case PdfStringFlags.StandardEncoding:
                case PdfStringFlags.MacRomanEncoding:
                case PdfStringFlags.MacExpertEncoding:
                default:
                    throw new NotImplementedException("Unexpected encoding");
            }
#endif
            _lastCat = CharCat.Delimiter;
        }

        /// <summary>
        /// Writes the specified value to the PDF stream.
        /// </summary>
        public void Write(PdfName value)
        {
            WriteSeparator(CharCat.Delimiter, '/');
            string name = value.Value;

            StringBuilder pdf = new StringBuilder("/");
            for (int idx = 1; idx < name.Length; idx++)
            {
                char ch = name[idx];
                Debug.Assert(ch < 256);
                if (ch > ' ')
                    switch (ch)
                    {
                        // TODO: is this all?
                        case '%':
                        case '/':
                        case '<':
                        case '>':
                        case '(':
                        case ')':
                        case '#':
                            break;

                        default:
                            pdf.Append(name[idx]);
                            continue;
                    }
                pdf.AppendFormat("#{0:X2}", (int)name[idx]);
            }
            WriteRaw(pdf.ToString());
            _lastCat = CharCat.Character;
        }

        public void Write(PdfLiteral value)
        {
            WriteSeparator(CharCat.Character);
            WriteRaw(value.Value);
            _lastCat = CharCat.Character;
        }

        public void Write(PdfRectangle rect)
        {
            const string format = Config.SignificantFigures3;
            WriteSeparator(CharCat.Delimiter, '/');
            WriteRaw(PdfEncoders.Format("[{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "}]", rect.X1, rect.Y1, rect.X2, rect.Y2));
            _lastCat = CharCat.Delimiter;
        }

        public void Write(PdfReference iref)
        {
            WriteSeparator(CharCat.Character);
            WriteRaw(iref.ToString());
            _lastCat = CharCat.Character;
        }

        public void WriteDocString(string text, bool unicode)
        {
            WriteSeparator(CharCat.Delimiter);
            //WriteRaw(PdfEncoders.DocEncode(text, unicode));
            byte[] bytes;
            if (!unicode)
                bytes = PdfEncoders.DocEncoding.GetBytes(text);
            else
                bytes = PdfEncoders.UnicodeEncoding.GetBytes(text);
            bytes = PdfEncoders.FormatStringLiteral(bytes, unicode, true, false, _securityHandler);
            Write(bytes);
            _lastCat = CharCat.Delimiter;
        }

        public void WriteDocString(string text)
        {
            WriteSeparator(CharCat.Delimiter);
            //WriteRaw(PdfEncoders.DocEncode(text, false));
            byte[] bytes = PdfEncoders.DocEncoding.GetBytes(text);
            bytes = PdfEncoders.FormatStringLiteral(bytes, false, false, false, _securityHandler);
            Write(bytes);
            _lastCat = CharCat.Delimiter;
        }

        public void WriteDocStringHex(string text)
        {
            WriteSeparator(CharCat.Delimiter);
            //WriteRaw(PdfEncoders.DocEncodeHex(text));
            byte[] bytes = PdfEncoders.DocEncoding.GetBytes(text);
            bytes = PdfEncoders.FormatStringLiteral(bytes, false, false, true, _securityHandler);
            _stream.Write(bytes, 0, bytes.Length);
            _lastCat = CharCat.Delimiter;
        }

        /// <summary>
        /// Begins a direct or indirect dictionary or array.
        /// </summary>
        public void WriteBeginObject(PdfObject obj)
        {
            PdfDictionary dict = obj as PdfDictionary;
            bool indirect = obj.IsIndirect;
            if (indirect)
            {
                WriteObjectAddress(obj);
                if (_securityHandler != null)
                    _securityHandler.SetHashKey(obj.ObjectID);
            }
            _stack.Add(new StackItem(obj));
            if (indirect)
            {
                if (obj is PdfArray)
                {
                    WriteRaw("[\n");
                }
                else if (obj is PdfDictionary)
                {
                    WriteRaw("<<");

                    if (_layout != PdfWriterLayout.Compact)
                    {
                        NewLine();
                    }
                }
            }
            else
            {
                if (obj is PdfArray)
                {
                    WriteSeparator(CharCat.Delimiter);
                    WriteRaw('[');
                }
                else if (obj is PdfDictionary)
                {
                    if (_layout != PdfWriterLayout.Compact)
                    {
                        NewLine();
                        WriteSeparator(CharCat.Delimiter);
                        WriteRaw("<<\n");
                    }
                    else
                    {
                        WriteRaw("<<");
                    }
                }
            }
            if (_layout == PdfWriterLayout.Verbose)
                IncreaseIndent();
        }

        /// <summary>
        /// Ends a direct or indirect dictionary or array.
        /// </summary>
        public void WriteEndObject()
        {
            int count = _stack.Count;
            Debug.Assert(count > 0, "PdfWriter stack underflow.");

            StackItem stackItem = _stack[count - 1];
            _stack.RemoveAt(count - 1);

            PdfObject value = stackItem.Object;
            bool indirect = value.IsIndirect;
            if (_layout == PdfWriterLayout.Verbose)
                DecreaseIndent();
            if (value is PdfArray)
            {
                if (indirect)
                {
                    WriteRaw("\n]\n");
                    _lastCat = CharCat.NewLine;
                }
                else
                {
                    WriteRaw("]");
                    _lastCat = CharCat.Delimiter;
                }
            }
            else if (value is PdfDictionary)
            {
                if (indirect)
                {
                    if (!stackItem.HasStream)
                    {
                        if (_layout == PdfWriterLayout.Compact)
                        {
                            WriteRaw(">>\r");
                        }
                        else
                        {
                            WriteRaw(_lastCat == CharCat.NewLine ? ">>\r" : " >>\r");
                        }
                    }
                }
                else
                {
                    Debug.Assert(!stackItem.HasStream, "Direct object with stream??");
                    if (_layout != PdfWriterLayout.Compact)
                    {
                        WriteSeparator(CharCat.NewLine);
                        WriteRaw(">>\n");
                    }
                    else
                    {
                        WriteRaw(">>");
                    }

                }
            }
            if (indirect)
            {
                if (_lastCat != CharCat.NewLine)
                    NewLine();

                WriteRaw("endobj\r");
                if (_layout == PdfWriterLayout.Verbose)
                    WriteRaw("%--------------------------------------------------------------------------------------------------\n");
            }
        }

        /// <summary>
        /// Writes the stream of the specified dictionary.
        /// </summary>
        public void WriteStream(PdfDictionary value, bool omitStream)
        {
            StackItem stackItem = _stack[_stack.Count - 1];
            Debug.Assert(stackItem.Object is PdfDictionary);
            Debug.Assert(stackItem.Object.IsIndirect);
            stackItem.HasStream = true;
            if (value.IsCompact)
            {
                WriteRaw(">>stream\r\n");
            }
            else
            {
                WriteRaw(_lastCat == CharCat.NewLine ? ">>\nstream\n" : " >>\nstream\n");
            }

            if (omitStream)
            {
                WriteRaw("  �...stream content omitted...�\n");  // useful for debugging only
            }
            else
            {
                byte[] bytes = value.Stream.Value;
                if (bytes.Length != 0)
                {
                    if (_securityHandler != null)
                    {
                        bytes = (byte[])bytes.Clone();
                        bytes = _securityHandler.EncryptBytes(bytes);
                    }
                    Write(bytes);
                    if (!string.IsNullOrEmpty(value.Stream.Trailer))
                    {
                        WriteRaw(value.Stream.Trailer);
                    }
                    if (_lastCat != CharCat.NewLine)
                        WriteRaw('\r');
                }
            }
            WriteRaw("endstream\r");
        }

        public void WriteRaw(string rawString)
        {
            if (String.IsNullOrEmpty(rawString))
                return;

            byte[] bytes = PdfEncoders.RawEncoding.GetBytes(rawString);
            _stream.Write(bytes, 0, bytes.Length);
            _lastCat = GetCategory((char)bytes[bytes.Length - 1]);
        }

        public void WriteRaw(char ch)
        {
            Debug.Assert(ch < 256, "Raw character greater than 255 detected.");

            _stream.WriteByte((byte)ch);
            _lastCat = GetCategory(ch);
        }

        public void Write(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return;

            _stream.Write(bytes, 0, bytes.Length);
            _lastCat = GetCategory((char)bytes[bytes.Length - 1]);
        }

        void WriteObjectAddress(PdfObject value)
        {
            //if (_layout == PdfWriterLayout.Verbose)
            //    WriteRaw(String.Format("{0} {1} obj   % {2}\n",
            //        value.ObjectID.ObjectNumber, value.ObjectID.GenerationNumber,
            //        value.GetType().FullName));
            //else
            WriteRaw(String.Format("{0} {1} obj\r", value.ObjectID.ObjectNumber, value.ObjectID.GenerationNumber));
        }

        public void WriteFileHeader(PdfDocument document)
        {
            StringBuilder header = new StringBuilder("%PDF-");
            int version = document._version;
            header.Append((version / 10).ToString(CultureInfo.InvariantCulture) + "." +
              (version % 10).ToString(CultureInfo.InvariantCulture) + "\n%\xD3\xF4\xCC\xE1\n");
            WriteRaw(header.ToString());
        }

        public void WriteEof(PdfDocument document, int startxref)
        {
            if (_lastCat != CharCat.NewLine)
            {
                WriteRaw("\r\n");
            }
            WriteRaw("startxref\r\n");
            WriteRaw(startxref.ToString(CultureInfo.InvariantCulture));
            WriteRaw("\r\n%%EOF\r\n");
            int fileSize = (int)_stream.Position;
        }

        /// <summary>
        /// Gets or sets the indentation for a new indentation level.
        /// </summary>
        internal int Indent
        {
            get { return _indent; }
            set { _indent = value; }
        }
        int _indent = 2;
        int _writeIndent;

        /// <summary>
        /// Increases indent level.
        /// </summary>
        void IncreaseIndent()
        {
            _writeIndent += _indent;
        }

        /// <summary>
        /// Decreases indent level.
        /// </summary>
        void DecreaseIndent()
        {
            _writeIndent -= _indent;
        }

        ///// <summary>
        ///// Returns an indent string of blanks.
        ///// </summary>
        //static string Ind(int _indent)
        //{
        //  return new String(' ', _indent);
        //}

        /// <summary>
        /// Gets an indent string of current indent.
        /// </summary>
        string IndentBlanks
        {
            get { return new string(' ', _writeIndent); }
        }

        void WriteIndent()
        {
            WriteRaw(IndentBlanks);
        }

        void WriteSeparator(CharCat cat, char ch)
        {
            switch (_lastCat)
            {
                case CharCat.NewLine:
                    if (_layout == PdfWriterLayout.Verbose)
                        WriteIndent();
                    break;

                case CharCat.Delimiter:
                    break;

                case CharCat.Character:
                    if (_layout == PdfWriterLayout.Verbose)
                    {
                        //if (cat == CharCat.Character || ch == '/')
                        _stream.WriteByte((byte)' ');
                    }
                    else
                    {
                        if (cat == CharCat.Character)
                        {
                            Stream.Seek(-1, SeekOrigin.End);
                            if (Stream.ReadByte() != 32)//space 
                                _stream.WriteByte((byte)' ');
                        }
                    }
                    break;
            }
        }

        void WriteSeparator(CharCat cat)
        {
            WriteSeparator(cat, '\0');
        }

        public void NewLine()
        {
            if (_lastCat != CharCat.NewLine)
                WriteRaw('\n');
        }

        CharCat GetCategory(char ch)
        {
            if (Lexer.IsDelimiter(ch))
                return CharCat.Delimiter;
            if (ch == Chars.LF || ch == Chars.CR)
                return CharCat.NewLine;
            return CharCat.Character;
        }

        enum CharCat
        {
            NewLine,
            Character,
            Delimiter,
        }
        CharCat _lastCat;

        /// <summary>
        /// Gets the underlying stream.
        /// </summary>
        internal Stream Stream
        {
            get { return _stream; }
        }
        Stream _stream;

        internal PdfStandardSecurityHandler SecurityHandler
        {
            get { return _securityHandler; }
            set { _securityHandler = value; }
        }
        PdfStandardSecurityHandler _securityHandler;

        class StackItem
        {
            public StackItem(PdfObject value)
            {
                Object = value;
            }

            public readonly PdfObject Object;
            public bool HasStream;
        }

        readonly List<StackItem> _stack = new List<StackItem>();
    }
}
