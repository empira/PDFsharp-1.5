#region PDFsharp Charting - A .NET charting library based on PDFsharp
//
// Authors:
//   Niklas Schneider
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

namespace PdfSharp.Charting
{
    /// <summary>
    /// Base class for all chart classes.
    /// </summary>
    public class DocumentObject
    {
        /// <summary>
        /// Initializes a new instance of the DocumentObject class.
        /// </summary>
        public DocumentObject()
        { }

        /// <summary>
        /// Initializes a new instance of the DocumentObject class with the specified parent.
        /// </summary>
        public DocumentObject(DocumentObject parent)
        {
            _parent = parent;
        }

        #region Methods
        /// <summary>
        /// Creates a deep copy of the DocumentObject. The parent of the new object is null.
        /// </summary>
        public object Clone()
        {
            return DeepCopy();
        }

        /// <summary>
        /// Implements the deep copy of the object.
        /// </summary>
        protected virtual object DeepCopy()
        {
            DocumentObject value = (DocumentObject)MemberwiseClone();
            value._parent = null;
            return value;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the parent object.
        /// </summary>
        public DocumentObject Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// 
        /// </summary>
        /*protected*/
        internal DocumentObject _parent;
        #endregion
    }
}
