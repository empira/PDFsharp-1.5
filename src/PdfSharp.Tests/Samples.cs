using NUnit.Framework;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Internal;
using PdfSharp.Pdf;
using PdfSharp.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PdfSharp.Tests
{
    [TestFixture]
    public class Samples
    {
        private Uri _outputFolder = null;
        private string _extOuput = ".pdf";

        [SetUp]
        public void Prepare2Tests()
        {
            if (_outputFolder == null)
            {
                _outputFolder = GeneralUtils.OutputTestsFolder();
            }

    
            Directory.CreateDirectory(_outputFolder.LocalPath);

        }

        private static readonly object[] TestCasesHelloWorld =
        {
            new object[]{"Arial",
                20,
                XFontStyle.BoldItalic },
            new object[]{"Arial",
                20,
                XFontStyle.Bold },
            new object[]{"Arial",
                20,
                XFontStyle.Regular },
            new object[]{"Arial",
                20,
                XFontStyle.Italic },
            new object[]{"Courier New",
                20,
                XFontStyle.BoldItalic },
            new object[]{"Courier New",
                20,
                XFontStyle.Bold },
            new object[]{"Courier New",
                20,
                XFontStyle.Italic },
            new object[]{"Courier New",
                20,
                XFontStyle.Regular },
            new object[]{"Times New Roman",
                20,
                XFontStyle.BoldItalic },
            new object[]{"Times New Roman",
                20,
                XFontStyle.Bold },
            new object[]{"Times New Roman",
                20,
                XFontStyle.Italic },
            new object[]{"Times New Roman",
                20,
                XFontStyle.Regular },
        };


        [Test, TestCaseSource("TestCasesHelloWorld")]
        public void HelloWorld(string familyName, double fontSize, XFontStyle style)
        {
            string filename = "HelloWorld";

            // Create a new PDF document.
            var document = new PdfDocument();
            document.Info.Title = "Created with PDFsharp";

            // Create an empty page in this document.
            var page = document.AddPage();

            // Get an XGraphics object for drawing on this page.
            var gfx = XGraphics.FromPdfPage(page);

            // Draw two lines with a red default pen.
            var width = page.Width;
            var height = page.Height;
            gfx.DrawLine(XPens.Red, 0, 0, width, height);
            gfx.DrawLine(XPens.Red, width, 0, 0, height);

            // Draw a circle with a red pen which is 1.5 point thick.
            var r = width / 5;
            gfx.DrawEllipse(new XPen(XColors.Red, 1.5), XBrushes.White, new XRect(width / 2 - r, height / 2 - r, 2 * r, 2 * r));
            // Create a font
            var font = new XFont(familyName, fontSize, style);

            // Draw the text.
            gfx.DrawString("Hello, PDFsharp!", font, XBrushes.Black,
                    new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);

            string fileNameOut = string.Format("{0}_{1}_{2}_{3}{4}",
                                                filename,
                                                familyName,
                                                fontSize,
                                                style,
                                                _extOuput);

            string fullOutputPath = System.IO.Path.Combine(_outputFolder.LocalPath, fileNameOut);

            // Save the document...
            document.Save(fullOutputPath);

        }


    }
}
