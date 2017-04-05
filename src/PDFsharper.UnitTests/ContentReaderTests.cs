using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfSharper.Pdf.Content;
using PdfSharper.Pdf.Internal;
using System.IO;

namespace PDFsharper.UnitTests
{
    [TestClass]
    public class ContentReaderTests
    {
        [TestMethod]
        public void OperatorAtEndOfStream()
        {
            string da = "/Arial 6 Tf 0 g";
            var content = ContentReader.ReadContent(new RawEncoding().GetBytes(da));

            string actual = new RawEncoding().GetString(content.ToContent()).Replace("\n", " ").Trim();

            Assert.AreEqual(da, actual, "ContentReader missed an operator");
        }


        [TestMethod]
        public void MultiCharOperatorAtEndOfStream()
        {
            string da = "/Helv 9 Tf 0 0 0 rg";
            var content = ContentReader.ReadContent(new RawEncoding().GetBytes(da));

            string actual = new RawEncoding().GetString(content.ToContent()).Replace("\n", " ").Trim();

            Assert.AreEqual(da, actual, "ContentReader missed an operator");
        }
    }
}
