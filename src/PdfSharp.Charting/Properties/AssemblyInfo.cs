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

using System.Reflection;
using System.Runtime.InteropServices;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("PDFsharp Charting")]
[assembly: AssemblyVersion(PdfSharp.ProductVersionInfo.Version)]
[assembly: AssemblyDescription("A .NET charting library based on PDFsharp.")]
[assembly: AssemblyConfiguration(PdfSharp.ProductVersionInfo.Configuration)]
[assembly: AssemblyCompany(PdfSharp.ProductVersionInfo.Company)]
#if DEBUG
  [assembly: AssemblyProduct(PdfSharp.ProductVersionInfo.Product + " (Debug Build)")]
#else
  [assembly: AssemblyProduct(PdfSharp.ProductVersionInfo.Product)]
#endif
[assembly: AssemblyCopyright(PdfSharp.ProductVersionInfo.Copyright)]
[assembly: AssemblyTrademark(PdfSharp.ProductVersionInfo.Trademark)]
[assembly: AssemblyCulture(PdfSharp.ProductVersionInfo.Culture)]

[assembly: ComVisible(false)]
//[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyName("")]
