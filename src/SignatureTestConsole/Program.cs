using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing.Layout;
using System.IO;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.Annotations;
using TestConsole.Properties;
using PdfSharp.Pdf.Signatures;

namespace TestConsole
{
    class Program
    {
        private class SignAppearenceHandler : ISignatureAppearanceHandler
        {
            private XImage Image = XImage.FromFile("..\\..\\resources\\logo.jpg");
            public void DrawAppearance(XGraphics gfx, XRect rect)
            {
                XColor empty = XColor.Empty;
                string text = "Signed by Napoleon \nLocation: Paris \nDate: " + DateTime.Now.ToString();
                XFont font = new XFont("Verdana", 7.0, XFontStyle.Regular);
                XTextFormatter xTextFormatter = new XTextFormatter(gfx);
                int num = this.Image.PixelWidth / this.Image.PixelHeight;
                XPoint xPoint = new XPoint(0.0, 0.0);
                bool flag = this.Image != null;
                if (flag)
                {
                    gfx.DrawImage(this.Image, xPoint.X, xPoint.Y, rect.Width / 4.0, rect.Width / 4.0 / (double)num);
                    xPoint = new XPoint(rect.Width / 4.0, 0.0);
                }
                xTextFormatter.DrawString(text, font, new XSolidBrush(XColor.FromKnownColor(XKnownColor.Black)), new XRect(xPoint.X, xPoint.Y, rect.Width - xPoint.X, rect.Height), XStringFormats.TopLeft);
            }
        }
        private static void Main(string[] args)
        {
            Program.CreateAndSign();
            Program.SignExisting();
            Program.AttachFile();
        }
        private static void CreateAndSign()
        {
            string text = "CreateAndSign.pdf";
            XFont font = new XFont("Verdana", 10.0, XFontStyle.Regular);
            PdfDocument pdfDocument = new PdfDocument();
            PdfPage pdfPage = pdfDocument.AddPage();
            XGraphics xGraphics = XGraphics.FromPdfPage(pdfPage);
            XRect layoutRectangle = new XRect(0.0, 0.0, pdfPage.Width, pdfPage.Height);
            xGraphics.DrawString("Sample document", font, XBrushes.Black, layoutRectangle, XStringFormats.TopCenter);
            PdfSignatureOptions options = new PdfSignatureOptions
            {
                ContactInfo = "Contact Info",
                Location = "Paris",
                Reason = "Test signatures",
                Rectangle = new XRect(36.0, 700.0, 200.0, 50.0)
            };
            PdfSignatureHandler pdfSignatureHandler = new PdfSignatureHandler( new DefaultSigner(Program.GetCertificate()), null, options);
            //PdfSignatureHandler pdfSignatureHandler = new PdfSignatureHandler(new BouncySigner(Program.GetCertificate()), null, options);
            pdfSignatureHandler.AttachToDocument(pdfDocument);
            pdfDocument.Save(text);
            Process.Start(text);
        }
        private static void SignExisting()
        {
            string text = string.Format("SignExisting.pdf", new object[0]);
            PdfDocument pdfDocument = PdfReader.Open(new MemoryStream(Resources.doc1));
            PdfSignatureOptions options = new PdfSignatureOptions
            {
                ContactInfo = "Contact Info",
                Location = "Paris",
                Reason = "Test signatures",
                Rectangle = new XRect(36.0, 735.0, 200.0, 50.0),
                AppearanceHandler = new Program.SignAppearenceHandler()
            };
            //PdfSignatureHandler pdfSignatureHandler = new PdfSignatureHandler(new DefaultSigner(Program.GetCertificate()), null, options);
            PdfSignatureHandler pdfSignatureHandler = new PdfSignatureHandler(new BouncySigner(Program.GetCertificate()), null, options);
            pdfSignatureHandler.AttachToDocument(pdfDocument);
            pdfDocument.Save(text);
            Process.Start(text);
        }
        private static void AttachFile()
        {
            string text = "AttachFile.pdf";
            XFont font = new XFont("Verdana", 10.0, XFontStyle.Regular);
            PdfDocument pdfDocument = new PdfDocument();
            PdfPage pdfPage = pdfDocument.AddPage();
            XGraphics xGraphics = XGraphics.FromPdfPage(pdfPage);
            xGraphics.DrawString("Open with Adobe Acrobat Reader and click on the icon", font, XBrushes.Black, new XRect(10.0, 100.0, 300.0, 10.0), XStringFormats.TopLeft);
            pdfPage.Annotations.Add(new PdfFileAttachmentAnnotation(pdfDocument)
            {
                Rectangle = new PdfRectangle(new XRect(10.0, pdfPage.Height - 100.0, 10.0, 10.0)),
                File = new PdfFileSpecification(pdfDocument, "logo.jpg", new PdfEmbeddedFile(pdfDocument, File.ReadAllBytes("..\\..\\resources\\logo.jpg"), null))
            });
            pdfDocument.Save(text);
            Process.Start(text);
        }
        private static X509Certificate2 GetCertificate()
        {
            throw new NotImplementedException("Put your certificate path here");
            //return new X509Certificate2(....);
        }




    }
}
