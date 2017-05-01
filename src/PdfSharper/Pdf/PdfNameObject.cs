﻿#region PDFsharp - A .NET library for processing PDF
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
using System.Diagnostics;
using PdfSharper.Pdf.IO;

namespace PdfSharper.Pdf
{
    /// <summary>
    /// Represents an indirect name value. This type is not used by PdfSharper. If it is imported from
    /// an external PDF file, the value is converted into a direct object. Acrobat sometime uses indirect
    /// names to save space, because an indirect reference to a name may be shorter than a long name.
    /// </summary>
    [DebuggerDisplay("({Value})")]
    public sealed class PdfNameObject : PdfObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfNameObject"/> class.
        /// </summary>
        public PdfNameObject()
        {
            _value = "/";  // Empty name.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfNameObject"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="value">The value.</param>
        public PdfNameObject(PdfDocument document, string value)
            : base(document)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (value.Length == 0 || value[0] != '/')
                throw new ArgumentException(PSSR.NameMustStartWithSlash);

            _value = value;
        }

        /// <summary>
        /// Serves as a hash function for this type.
        /// </summary>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <summary>
        /// Returns whether this object is equal to the passed in value.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if it matches by reference or value.</returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Gets or sets the name value.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
        string _value;

        /// <summary>
        /// Returns the name. The string always begins with a slash.
        /// </summary>
        public override string ToString()
        {
            // TODO: Encode characters.
            return _value;
        }

        /// <summary>
        /// Determines whether a name is equal to a string.
        /// </summary>
        public static bool operator ==(PdfNameObject name, string str)
        {
            if (ReferenceEquals(name, null))
                return str != null;

            return name._value == str;
        }

        /// <summary>
        /// Determines whether a name is not equal to a string.
        /// </summary>
        public static bool operator !=(PdfNameObject name, string str)
        {
            if (ReferenceEquals(name, null))
                return str != null;

            return name._value != str;
        }

#if leads_to_ambiguity
        public static bool operator ==(string str, PdfName name)
        {
            if (ReferenceEquals(name, null))
                return str != null;

            return str == name.value;
        }

        public static bool operator !=(string str, PdfName name)
        {
            if (ReferenceEquals(name, null))
                return str != null;

            return str == name.value;
        }

        public static bool operator ==(PdfName name1, PdfName name2)
        {
            if (ReferenceEquals(name1, null))
                return name2 != null;

            if (ReferenceEquals(name2, null))
                return name1 != null;

            return name1.value == name2.value;
        }

        public static bool operator !=(PdfName name1, PdfName name2)
        {
            if (ReferenceEquals(name1, null))
                return name2 != null;

            if (ReferenceEquals(name2, null))
                return name1 != null;

            return name1.value != name2.value;
        }
#endif

        /// <summary>
        /// Writes the name including the leading slash.
        /// </summary>
        protected override void WriteObject(PdfWriter writer)
        {
            writer.WriteBeginObject(this);
            writer.Write(new PdfName(_value));
            writer.WriteEndObject();
        }
    }
}
