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
using System.Runtime.InteropServices;
using System.Text;
using System.Security;
//using System.Security.Permissions;

#if GDI
namespace PdfSharp.Forms
{
    /// <summary>
    /// Contains information about a physical device like a display or a printer.
    /// </summary>
    public struct DeviceInfos
    {
        /// <summary>
        /// Width, in millimeters, of the physical screen or device.
        /// </summary>
        public int HorizontalSize;

        /// <summary>
        /// Height, in millimeters, of the physical screen or device.
        /// </summary>
        public int VerticalSize;

        /// <summary>
        /// Width, in pixels, of the screen or device.
        /// </summary>
        public int HorizontalResolution;

        /// <summary>
        /// Height, in pixels, of the screen or device.
        /// </summary>
        public int VerticalResolution;

        /// <summary>
        /// Number of pixels per logical inch along the screen or device width.
        /// </summary>
        public int LogicalDpiX;

        /// <summary>
        /// Number of pixels per logical inch along the screen or device height.
        /// </summary>
        public int LogicalDpiY;

        /// <summary>
        /// Number of pixels per physical inch along the screen or device width.
        /// </summary>
        public float PhysicalDpiX;

        /// <summary>
        /// Number of pixels per physical inch along the screen or device height.
        /// </summary>
        public float PhysicalDpiY;

        /// <summary>
        /// The ratio of LogicalDpiX and PhysicalDpiX.
        /// </summary>
        public float ScaleX;

        /// <summary>
        /// The ratio of LogicalDpiY and PhysicalDpiY.
        /// </summary>
        public float ScaleY;

        /// <summary>
        /// Gets a DeviceInfo for the specifed device context.
        /// </summary>
        [SuppressUnmanagedCodeSecurity]
        public static DeviceInfos GetInfos(IntPtr hdc)
        {
            DeviceInfos devInfo;

            devInfo.HorizontalSize = GetDeviceCaps(hdc, HORZSIZE);
            devInfo.VerticalSize = GetDeviceCaps(hdc, VERTSIZE);
            devInfo.HorizontalResolution = GetDeviceCaps(hdc, HORZRES);
            devInfo.VerticalResolution = GetDeviceCaps(hdc, VERTRES);
            devInfo.LogicalDpiX = GetDeviceCaps(hdc, LOGPIXELSX);
            devInfo.LogicalDpiY = GetDeviceCaps(hdc, LOGPIXELSY);
            devInfo.PhysicalDpiX = devInfo.HorizontalResolution * 25.4f / devInfo.HorizontalSize;
            devInfo.PhysicalDpiY = devInfo.VerticalResolution * 25.4f / devInfo.VerticalSize;
            devInfo.ScaleX = devInfo.LogicalDpiX / devInfo.PhysicalDpiX;
            devInfo.ScaleY = devInfo.LogicalDpiY / devInfo.PhysicalDpiY;

            return devInfo;
        }

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int capability);
        // ReSharper disable InconsistentNaming
        const int HORZSIZE = 4;
        const int VERTSIZE = 6;
        const int HORZRES = 8;
        const int VERTRES = 10;
        const int LOGPIXELSX = 88;
        const int LOGPIXELSY = 90;
        // ReSharper restore InconsistentNaming
    }
}
#endif
