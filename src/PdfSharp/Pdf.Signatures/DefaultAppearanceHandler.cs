using System;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;

namespace PdfSharp.Signatures
{
    internal class DefaultAppearanceHandler : ISignatureAppearanceHandler
    {
        public string Location { get; set; }
        public string Reason { get; set; }
        public string Signer { get; set; }
       

        public void DrawAppearance(XGraphics gfx, XRect rect)
        {
            var backColor = XColor.Empty;
            var defaultText = string.Format("Signed by: {0}\nLocation: {1}\nReason: {2}\nDate: {3}", Signer, Location, Reason, DateTime.Now);            

            XFont font = new XFont("Verdana", 7, XFontStyle.Regular);                     
            
            XTextFormatter txtFormat = new XTextFormatter(gfx); 

            var currentPosition = new XPoint(0, 0);

            txtFormat.DrawString(defaultText, 
                font, 
                new XSolidBrush(XColor.FromKnownColor(XKnownColor.Black)), 
                new XRect(currentPosition.X, currentPosition.Y, rect.Width - currentPosition.X, rect.Height),  
                XStringFormats.TopLeft);           
        }
    }
}
