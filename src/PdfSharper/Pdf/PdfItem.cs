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
using PdfSharper.Pdf.IO;

namespace PdfSharper.Pdf
{
    public class PdfItemEventArgs : EventArgs
    {
        public int Position { get; set; }
    }
    /// <summary>
    /// The base class of all PDF objects and simple PDF types.
    /// </summary>
    public abstract class PdfItem : PdfDirty, ICloneable
    {
        // All simple types (i.e. derived from PdfItem but not from PdfObject) must be immutable.
        internal event EventHandler<PdfItemEventArgs> BeforeWrite;
        internal event EventHandler<PdfItemEventArgs> AfterWrite;


        object ICloneable.Clone()
        {
            return Copy();
        }

        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        public PdfItem Clone()
        {
            return (PdfItem)Copy();
        }

        /// <summary>
        /// Implements the copy mechanism. Must be overridden in derived classes.
        /// </summary>
        protected virtual object Copy()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// When overridden in a derived class, appends a raw string representation of this object
        /// to the specified PdfWriter.
        /// </summary>
        protected abstract void WriteObject(PdfWriter writer);


        internal void Write(PdfWriter writer)
        {
            var startPosition = writer.Layout == PdfWriterLayout.Verbose ? writer.Position + 1 : writer.Position;

            this.BeforeWrite?.Invoke(this, new PdfItemEventArgs() { Position = startPosition });
            WriteObject(writer);
            this.AfterWrite?.Invoke(this, new PdfItemEventArgs() { Position = writer.Position });
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;

            return this.GetHashCode() == obj.GetHashCode();
        }

        public abstract override int GetHashCode();
    }
}
