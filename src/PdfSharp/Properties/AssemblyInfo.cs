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

using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if WPF
using System.Windows.Markup;
#endif

[assembly: AssemblyTitle(PdfSharp.VersionInfo.Title)]
[assembly: AssemblyVersion(PdfSharp.VersionInfo.Version)]
[assembly: AssemblyDescription(PdfSharp.VersionInfo.Description)]
[assembly: AssemblyConfiguration(PdfSharp.VersionInfo.Configuration)]
[assembly: AssemblyCompany(PdfSharp.VersionInfo.Company)]
#if DEBUG
[assembly: AssemblyProduct(PdfSharp.ProductVersionInfo.Product + " (Debug Build)")]
#else
  [assembly: AssemblyProduct(PdfSharp.ProductVersionInfo.Product)]
#endif
[assembly: AssemblyCopyright(PdfSharp.VersionInfo.Copyright)]
[assembly: AssemblyTrademark(PdfSharp.VersionInfo.Trademark)]
[assembly: AssemblyCulture(PdfSharp.VersionInfo.Culture)]

[assembly: NeutralResourcesLanguage("en-US")]

#if WPF
[assembly: XmlnsDefinition("http://schemas.empira.com/pdfsharp/2010/xaml/presentation", "PdfSharp.Windows")]
#endif

[assembly: InternalsVisibleTo("PdfSharp.UnitTest, PublicKey=00240000048000009400000006020000002400005253413100040000010001008794e803e566eccc3c9181f52c4f7044e5442cc2ce3cbba9fc11bc4186ba2e446cd31deea20c1a8f499e978417fad2bc74143a4f8398f7cf5c5c0271b0f7fe907c537cff28b9d582da41289d1dae90168a3da2a5ed1115210a18fdae832479d3e639ca4003286ba8b98dc9144615c040ed838981ac816112df3b5a9e7cab4fbb")]
#if WPF
[assembly: InternalsVisibleTo("PdfSharp.Xps, PublicKey=00240000048000009400000006020000002400005253413100040000010001008794e803e566eccc3c9181f52c4f7044e5442cc2ce3cbba9fc11bc4186ba2e446cd31deea20c1a8f499e978417fad2bc74143a4f8398f7cf5c5c0271b0f7fe907c537cff28b9d582da41289d1dae90168a3da2a5ed1115210a18fdae832479d3e639ca4003286ba8b98dc9144615c040ed838981ac816112df3b5a9e7cab4fbb")]
[assembly: InternalsVisibleTo("Edf.Xps, PublicKey=00240000048000009400000006020000002400005253413100040000010001008794e803e566eccc3c9181f52c4f7044e5442cc2ce3cbba9fc11bc4186ba2e446cd31deea20c1a8f499e978417fad2bc74143a4f8398f7cf5c5c0271b0f7fe907c537cff28b9d582da41289d1dae90168a3da2a5ed1115210a18fdae832479d3e639ca4003286ba8b98dc9144615c040ed838981ac816112df3b5a9e7cab4fbb")]
#endif
[assembly: InternalsVisibleTo("PdfSharp.Toolkit.Silverlight, PublicKey=00240000048000009400000006020000002400005253413100040000010001008794e803e566eccc3c9181f52c4f7044e5442cc2ce3cbba9fc11bc4186ba2e446cd31deea20c1a8f499e978417fad2bc74143a4f8398f7cf5c5c0271b0f7fe907c537cff28b9d582da41289d1dae90168a3da2a5ed1115210a18fdae832479d3e639ca4003286ba8b98dc9144615c040ed838981ac816112df3b5a9e7cab4fbb")]

[assembly: InternalsVisibleTo("ConsoleApplication-GDI, PublicKey=00240000048000009400000006020000002400005253413100040000010001008794e803e566eccc3c9181f52c4f7044e5442cc2ce3cbba9fc11bc4186ba2e446cd31deea20c1a8f499e978417fad2bc74143a4f8398f7cf5c5c0271b0f7fe907c537cff28b9d582da41289d1dae90168a3da2a5ed1115210a18fdae832479d3e639ca4003286ba8b98dc9144615c040ed838981ac816112df3b5a9e7cab4fbb")]
[assembly: InternalsVisibleTo("ConsoleApplication-Core, PublicKey=00240000048000009400000006020000002400005253413100040000010001008794e803e566eccc3c9181f52c4f7044e5442cc2ce3cbba9fc11bc4186ba2e446cd31deea20c1a8f499e978417fad2bc74143a4f8398f7cf5c5c0271b0f7fe907c537cff28b9d582da41289d1dae90168a3da2a5ed1115210a18fdae832479d3e639ca4003286ba8b98dc9144615c040ed838981ac816112df3b5a9e7cab4fbb")]
[assembly: InternalsVisibleTo("ConsoleApplication-WPF, PublicKey=00240000048000009400000006020000002400005253413100040000010001008794e803e566eccc3c9181f52c4f7044e5442cc2ce3cbba9fc11bc4186ba2e446cd31deea20c1a8f499e978417fad2bc74143a4f8398f7cf5c5c0271b0f7fe907c537cff28b9d582da41289d1dae90168a3da2a5ed1115210a18fdae832479d3e639ca4003286ba8b98dc9144615c040ed838981ac816112df3b5a9e7cab4fbb")]

[assembly: ComVisible(false)]
