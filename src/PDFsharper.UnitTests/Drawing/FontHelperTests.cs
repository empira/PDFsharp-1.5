using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfSharper.Drawing;
using System;

namespace PDFsharper.UnitTests.Drawing
{
    [TestClass]
    public class FontHelperTests
    {
        [TestMethod]
        public void MeasureString_Courier()
        {
            XFont font = new XFont("Courier", 10.0);
            XSize size = FontHelper.MeasureString("test", font);

            Assert.IsNotNull(size, "size should not be null");
            Assert.IsTrue(Math.Round(size.Width, 3) == 24, "size.Width is not correct.  Expected 24 but Got {0}", Math.Round(size.Width, 3));
            Assert.IsTrue(Math.Round(size.Height, 3) == 10.55, "size.Height is not correct.  Expected 10.55 but Got {0}", Math.Round(size.Height, 3));
        }

        [TestMethod]
        public void MeasureString_CourierBoldOblique()
        {
            XFont font = new XFont("Courier", 10.0, XFontStyle.BoldItalic);
            XSize size = FontHelper.MeasureString("test", font);

            Assert.IsNotNull(size, "size should not be null");
            Assert.IsTrue(Math.Round(size.Width, 3) == 24, "size.Width is not correct.  Expected 24.000f but Got {0}", Math.Round(size.Width, 3));
            Assert.IsTrue(Math.Round(size.Height, 3) == 10.51, "size.Height is not correct.  Expected 10.51 but Got {0}", Math.Round(size.Height, 3));
        }

        [TestMethod]
        public void MeasureString_CourierBold()
        {
            XFont font = new XFont("Courier", 10.0, XFontStyle.Bold);
            XSize size = FontHelper.MeasureString("test", font);

            Assert.IsNotNull(size, "size should not be null");

            double width = Math.Round(size.Width, 3);
            double height = Math.Round(size.Height, 3);
            Assert.IsTrue(width == 24, "size.Width is not correct.  Expected 24.000f but Got {0}", width);
            Assert.IsTrue(height == 10.51, "size.Height is not correct.  Expected 10.51 but Got {0}", height);
        }

        [TestMethod]
        public void MeasureString_CourierOblique()
        {
            XFont font = new XFont("Courier", 10.0, XFontStyle.Italic);
            XSize size = FontHelper.MeasureString("test", font);

            Assert.IsNotNull(size, "size should not be null");

            double width = Math.Round(size.Width, 3);
            double height = Math.Round(size.Height, 3);
            Assert.IsTrue(width == 24, "size.Width is not correct.  Expected 24.000f but Got {0}", width);
            Assert.IsTrue(height == 10.55, "size.Height is not correct.  Expected 10.55 but Got {0}", height);
        }

        [TestMethod]
        public void MeasureString_Helvetica()
        {
            XFont font = new XFont("Helvetica", 10.0);
            XSize size = FontHelper.MeasureString("test", font);

            Assert.IsNotNull(size, "size should not be null");

            double width = Math.Round(size.Width, 3);
            double height = Math.Round(size.Height, 3);
            Assert.IsTrue(width == 16.12, "size.Width is not correct.  Expected 16.12 but Got {0}", width);
            Assert.IsTrue(height == 11.56, "size.Height is not correct.  Expected 11.56 but Got {0}", height);
        }

        [TestMethod]
        public void MeasureString_HelveticaBold()
        {
            XFont font = new XFont("Helvetica", 10.0, XFontStyle.Bold);
            XSize size = FontHelper.MeasureString("test", font);

            Assert.IsNotNull(size, "size should not be null");

            double width = Math.Round(size.Width, 3);
            double height = Math.Round(size.Height, 3);
            Assert.IsTrue(width == 17.78, "size.Width is not correct.  Expected 17.78 but Got {0}", width);
            Assert.IsTrue(height == 11.9, "size.Height is not correct.  Expected 11.9 but Got {0}", height);
        }

        [TestMethod]
        public void MeasureString_HelveticaBoldOblique()
        {
            XFont font = new XFont("Helvetica", 10.0, XFontStyle.BoldItalic);
            XSize size = FontHelper.MeasureString("test", font);

            Assert.IsNotNull(size, "size should not be null");

            double width = Math.Round(size.Width, 3);
            double height = Math.Round(size.Height, 3);
            Assert.IsTrue(width == 17.78, "size.Width is not correct.  Expected 17.78 but Got {0}", width);
            Assert.IsTrue(height == 11.9, "size.Height is not correct.  Expected 11.9 but Got {0}", height);
        }

        [TestMethod]
        public void MeasureString_HelveticaOblique()
        {
            XFont font = new XFont("Helvetica", 10.0, XFontStyle.Italic);
            XSize size = FontHelper.MeasureString("test", font);

            Assert.IsNotNull(size, "size should not be null");

            double width = Math.Round(size.Width, 3);
            double height = Math.Round(size.Height, 3);
            Assert.IsTrue(width == 16.12, "size.Width is not correct.  Expected 16.12 but Got {0}", width);
            Assert.IsTrue(height == 11.56, "size.Height is not correct.  Expected 11.56 but Got {0}", height);
        }

        [TestMethod]
        public void MeasureString_Symbol()
        {
            XFont font = new XFont("Symbol", 10.0);
            XSize size = FontHelper.MeasureString("test", font);

            Assert.IsNotNull(size, "size should not be null");
            Assert.IsTrue(Math.Round(size.Width, 3) == 19.2, "size.Width is not correct.  Expected 19.2 but Got {0}", Math.Round(size.Width, 3));
            Assert.IsTrue(Math.Round(size.Height, 3) == 12, "size.Height is not correct.  Expected 12 but Got {0}", Math.Round(size.Height, 3));
        }

        [TestMethod]
        public void MeasureString_TimesBold()
        {
            XFont font = new XFont("Times-Roman", 10.0, XFontStyle.Bold);
            XSize size = FontHelper.MeasureString("test", font);

            Assert.IsNotNull(size, "size should not be null");

            double width = Math.Round(size.Width, 3);
            double height = Math.Round(size.Height, 3);
            Assert.IsTrue(width == 14.99, "size.Width is not correct.  Expected 14.99 but Got {0}", width);
            Assert.IsTrue(height == 11.53, "size.Height is not correct.  Expected 11.53 but Got {0}", height);
        }

        [TestMethod]
        public void MeasureString_TimesBoldItalic()
        {
            XFont font = new XFont("Times-Roman", 10.0, XFontStyle.BoldItalic);
            XSize size = FontHelper.MeasureString("test", font);

            Assert.IsNotNull(size, "size should not be null");

            double width = Math.Round(size.Width, 3);
            double height = Math.Round(size.Height, 3);
            Assert.IsTrue(width == 13.89, "size.Width is not correct.  Expected 13.89 but Got {0}", width);
            Assert.IsTrue(height == 11.39, "size.Height is not correct.  Expected 11.39 but Got {0}", height);
        }

        [TestMethod]
        public void MeasureString_TimesItalic()
        {
            XFont font = new XFont("Times-Roman", 10.0, XFontStyle.Italic);
            XSize size = FontHelper.MeasureString("test", font);

            Assert.IsNotNull(size, "size should not be null");

            double width = Math.Round(size.Width, 3);
            double height = Math.Round(size.Height, 3);
            Assert.IsTrue(width == 13.89, "size.Width is not correct.  Expected 13.89 but Got {0}", width);
            Assert.IsTrue(height == 11, "size.Height is not correct.  Expected 11 but Got {0}", height);
        }

        [TestMethod]
        public void MeasureString_TimesNewRoman()
        {
            XFont font = new XFont("Times New Roman", 10.0);
            XSize size = FontHelper.MeasureString("test", font);

            Assert.IsNotNull(size, "size should not be null");

            double width = Math.Round(size.Width, 3);
            double height = Math.Round(size.Height, 3);
            Assert.IsTrue(width == 33.72, "size.Width is not correct.  Expected 33.72 but Got {0}", width);
            Assert.IsTrue(height == 12, "size.Height is not correct.  Expected 12 but Got {0}", height);
        }

        [TestMethod]
        public void MeasureString_TimesRoman()
        {
            XFont font = new XFont("Times-Roman", 10.0);
            XSize size = FontHelper.MeasureString("test", font);

            Assert.IsNotNull(size, "size should not be null");

            double width = Math.Round(size.Width, 3);
            double height = Math.Round(size.Height, 3);
            Assert.IsTrue(width == 13.89, "size.Width is not correct.  Expected 13.89 but Got {0}", width);
            Assert.IsTrue(height == 11.16, "size.Height is not correct.  Expected 11.16 but Got {0}", height);
        }

        [TestMethod]
        public void MeasureString_ZapfDingbats()
        {
            XFont font = new XFont("ZapfDingbats", 10.0);
            XSize size = FontHelper.MeasureString("test", font);

            Assert.IsNotNull(size, "size should not be null");

            double width = Math.Round(size.Width, 3);
            double height = Math.Round(size.Height, 3);
            Assert.IsTrue(width == 33.72, "size.Width is not correct.  Expected 33.72 but Got {0}", width);
            Assert.IsTrue(height == 12, "size.Height is not correct.  Expected 12 but Got {0}", height);
        }

        [TestMethod]
        public void MeasureString_CombinationsOfLettersAndSpaces()
        {
            double width = 0;
            double height = 0;

            XFont font = new XFont("Helvetica", 10.0);

            XSize size1 = FontHelper.MeasureString("test", font);
            Assert.IsNotNull(size1, "size1 should not be null");

            width = Math.Round(size1.Width, 3);
            height = Math.Round(size1.Height, 3);
            Assert.IsTrue(width == 16.12, "size1 - Width is not correct.  Expected 16.12 but Got {0}", width);
            Assert.IsTrue(height == 11.56, "size1 - Height is not correct.  Expected 11.56 but Got {0}", height);

            XSize size2 = FontHelper.MeasureString("T ", font);
            Assert.IsNotNull(size2, "size2 should not be null");

            width = Math.Round(size2.Width, 3);
            height = Math.Round(size2.Height, 3);
            Assert.IsTrue(width == 8.89, "size2 - Width is not correct.  Expected 8.89 but Got {0}", width);
            Assert.IsTrue(height == 11.56, "size2 - Height is not correct.  Expected 11.56 but Got {0}", height);

            XSize size3 = FontHelper.MeasureString("e ", font);
            Assert.IsNotNull(size3, "size3 should not be null");

            width = Math.Round(size3.Width, 3);
            height = Math.Round(size3.Height, 3);
            Assert.IsTrue(width == 8.34, "size3 - Width is not correct.  Expected 8.34 but Got {0}", width);
            Assert.IsTrue(height == 11.56, "size3 - Height is not correct.  Expected 11.56 but Got {0}", height);

            XSize size4 = FontHelper.MeasureString("s ", font);
            Assert.IsNotNull(size4, "size4 should not be null");

            width = Math.Round(size4.Width, 3);
            height = Math.Round(size4.Height, 3);
            Assert.IsTrue(width == 7.78, "size4 - Width is not correct.  Expected 7.78 but Got {0}", width);
            Assert.IsTrue(height == 11.56, "size4 - Height is not correct.  Expected 11.56 but Got {0}", height);

            XSize size5 = FontHelper.MeasureString("t ", font);
            Assert.IsNotNull(size5, "size5 should not be null");

            width = Math.Round(size5.Width, 3);
            height = Math.Round(size5.Height, 3);
            Assert.IsTrue(width == 5.56, "size5 - Width is not correct.  Expected 5.56 but Got {0}", width);
            Assert.IsTrue(height == 11.56, "size5 - Height is not correct.  Expected 11.56 but Got {0}", height);

            XSize sizeSpace = FontHelper.MeasureString(" ", font);
            Assert.IsNotNull(sizeSpace, "sizeSpace should not be null");

            width = Math.Round(sizeSpace.Width, 3);
            height = Math.Round(sizeSpace.Height, 3);
            Assert.IsTrue(width == 2.78, "sizeSpace - Width is not correct.  Expected 2.78 but Got {0}", width);
            Assert.IsTrue(height == 11.56, "sizeSpace - Height is not correct.  Expected 11.56 but Got {0}", height);

            XSize sizeEmpty = FontHelper.MeasureString("", font);
            Assert.IsNotNull(sizeEmpty, "sizeEmpty should not be null");

            width = Math.Round(sizeEmpty.Width, 3);
            height = Math.Round(sizeEmpty.Height, 3);
            Assert.IsTrue(width == 0, "sizeEmpty - Width is not correct.  Expected 0 but Got {0}", width);
            Assert.IsTrue(height == 0, "sizeEmpty - Height is not correct.  Expected 0 but Got {0}", height);
        }

        [TestMethod]
        public void MeasureString_Text_Empty()
        {
            XFont font = new XFont("Times New Roman", 10.0);
            XSize size = FontHelper.MeasureString(string.Empty, font);

            Assert.IsTrue(size != null, "size should not be null");
            Assert.IsTrue(size.Width == 0, "Width should be 0");
            Assert.IsTrue(size.Height == 0, "Height should be 0");
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Test fails because ArgumentNullException was not thrown.")]
        public void MeasureString_Text_Null()
        {
            XFont font = new XFont("Times New Roman", 10.0);
            XSize size = FontHelper.MeasureString(null, font);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Test fails because ArgumentNullException was not thrown.")]
        public void MeasureString_Font_Null()
        {
            XSize size = FontHelper.MeasureString("test", null);
        }

        [TestMethod]
        public void CalcChecksum_AllZeroBuffer()
        {
            byte[] stream = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            ulong checkSum = FontHelper.CalcChecksum(stream);

            Assert.IsTrue(checkSum == 10, "checkSum should be 10, but {0} returned", checkSum);
        }

        [TestMethod]
        public void CalcChecksum_EmptyBuffer()
        {
            byte[] stream = new byte[0];

            ulong checkSum = FontHelper.CalcChecksum(stream);

            Assert.IsTrue(checkSum == 0, "checkSum should be 0");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Test fails because ArgumentNullException was not thrown.")]
        public void CalcChecksum_NullBuffer()
        {
            FontHelper.CalcChecksum(null);
        }

        [TestMethod]
        public void CreateStyle_NotBold_NotItalic()
        {
            bool isBold = false;
            bool isItalic = false;

            XFontStyle fontStyle = FontHelper.CreateStyle(isBold, isItalic);

            Assert.IsTrue(fontStyle == 0, "fontStyle should be 0");
        }

        [TestMethod]
        public void CreateStyle_IsBold_NotItalic()
        {
            bool isBold = true;
            bool isItalic = false;

            XFontStyle fontStyle = FontHelper.CreateStyle(isBold, isItalic);

            Assert.IsTrue(fontStyle == XFontStyle.Bold, "fontStyle should be Bold");
        }

        [TestMethod]
        public void CreateStyle_NotBold_IsItalic()
        {
            bool isBold = false;
            bool isItalic = true;

            XFontStyle fontStyle = FontHelper.CreateStyle(isBold, isItalic);

            Assert.IsTrue(fontStyle == XFontStyle.Italic, "fontStyle should be Italic");
        }

        [TestMethod]
        public void CreateStyle_IsBold_IsItalic()
        {
            bool isBold = true;
            bool isItalic = true;

            XFontStyle fontStyle = FontHelper.CreateStyle(isBold, isItalic);

            Assert.IsTrue(fontStyle == XFontStyle.BoldItalic, "fontStyle should be BoldItalic");
        }
    }
}
