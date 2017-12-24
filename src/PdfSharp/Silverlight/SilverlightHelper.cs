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

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.InteropServices;

namespace PdfSharp.Silverlight
{
    /// <summary>
    /// Useful stuff to show PDF files in Silverlight applications for 
    /// testing and debugging purposes.
    /// Some functions require elevated trust.
    /// </summary>
    public class SilverlightHelper
    {
        /// <summary>
        /// Gets the full path of UserStore for application.
        /// </summary>
        public static string FullPathOfUserStoreForApplication
        {
            get
            {
                if (_fullPathOfUserStoreForApplication != null)
                    return _fullPathOfUserStoreForApplication;

                // More simple than I expected...
                string root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"Low\Microsoft\Silverlight\is";
                IsolatedStorageFile userStore = IsolatedStorageFile.GetUserStoreForApplication();
                string markerFile = Guid.NewGuid().ToString();
                userStore.CreateFile(markerFile).Close();
                // Won't use Linq here (FirstOrDefault).
                IEnumerator<string> enumerator = Directory.EnumerateFileSystemEntries(root, markerFile, SearchOption.AllDirectories).GetEnumerator();
                enumerator.MoveNext();
                _fullPathOfUserStoreForApplication = Path.GetDirectoryName(enumerator.Current);
                userStore.DeleteFile(markerFile);
                return _fullPathOfUserStoreForApplication;
            }
        }
        static string _fullPathOfUserStoreForApplication;

        /// <summary>
        /// Uses ShellExecute to open a PDF file in UserStore.
        /// </summary>
        public static void ShowPdfFileFromUserStore(string path)
        {
            string fullPath = Path.Combine(FullPathOfUserStoreForApplication, path);
            ShellExecute(IntPtr.Zero, "open", fullPath, IntPtr.Zero, null, 5);
        }

        [DllImport("Shell32.dll")]
        static extern UInt32 ShellExecute(IntPtr hwnd, string pOperation, string pFile,
            IntPtr pParameters, string pDirectory, UInt32 nShowCmd);
    }
}
