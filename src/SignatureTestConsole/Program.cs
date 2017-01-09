using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Diagnostics;
using PdfSharp.Signatures;
using System.Security.Cryptography.X509Certificates;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing.Layout;
using System.IO;

namespace TestConsole
{
    class Program
    {
        class SignAppearenceHandler : ISignatureAppearanceHandler
        {           

            private XImage Image = XImage.FromFile(@"..\..\resources\logo.jpg");
            public void DrawAppearance(XGraphics gfx, XRect rect)
            {
                var backColor = XColor.Empty;
                var defaultText = "Signed by Napoleon \nLocation: Paris \nDate: " + DateTime.Now.ToString();

                XFont font = new XFont("Verdana", 7, XFontStyle.Regular);

                XTextFormatter txtFormat = new XTextFormatter(gfx);

                var ratio = Image.PixelWidth / Image.PixelHeight;

                var currentPosition = new XPoint(0, 0);


                if (this.Image != null)
                {
                    gfx.DrawImage(this.Image, currentPosition.X, currentPosition.Y, rect.Width / 4, rect.Width / 4 / ratio);
                    currentPosition = new XPoint(rect.Width / 4, 0);
                }

                txtFormat.DrawString(defaultText,
                    font,
                    new XSolidBrush(XColor.FromKnownColor(XKnownColor.Black)),
                    new XRect(currentPosition.X, currentPosition.Y, rect.Width - currentPosition.X, rect.Height),
                    XStringFormats.TopLeft);

            }
        }

        static void Main(string[] args)
        {

            CreateAndSign();
            SignExisting();

        }

        private static void CreateAndSign()
        {
            string path = "CreateAndSign.pdf";
            XFont font = new XFont("Verdana", 10, XFontStyle.Regular);
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            var rect = new XRect(0, 0, page.Width, page.Height);
            gfx.DrawString("Sample document", font, XBrushes.Black, rect, XStringFormats.TopCenter);

            var signatureOptions = new PdfSignatureOptions()
            {
                ContactInfo = "Contact Info",
                Location = "Paris",
                Reason = "Test signatures",
                Rectangle = new XRect(36, 700, 200, 50)
            };

            PdfSignatureHandler signatureHandler = new PdfSignatureHandler(
                GetCertificate(),
                signatureOptions
                );
            signatureHandler.AttachToDocument(document);

            document.Save(path);
            Process.Start(path);
        }


        private static void SignExisting()
        {            
            string path = string.Format("SignExisting.pdf");

            PdfDocument document = PdfReader.Open(new MemoryStream(Properties.Resources.doc1));

            var signatureOptions = new PdfSignatureOptions()
            {
                ContactInfo = "Contact Info",
                Location = "Paris",
                Reason = "Test signatures",
                Rectangle = new XRect(36, 735, 200, 50),
                  
                AppearanceHandler = new SignAppearenceHandler()
            };

            PdfSignatureHandler signatureHandler = new PdfSignatureHandler(GetCertificate(), signatureOptions);
            signatureHandler.AttachToDocument(document);

            document.Save(path);
            Process.Start(path);
        }

        private static X509Certificate2 GetCertificate()
        {
            throw new NotImplementedException("Add tour own certificate here");
            //return new X509Certificate2(@"...");
        }
    }
}
