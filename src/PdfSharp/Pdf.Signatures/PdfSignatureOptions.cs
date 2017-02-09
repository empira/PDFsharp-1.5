using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfSharp.Pdf.Signatures
{
    public class PdfSignatureOptions
    {
        public ISignatureAppearanceHandler AppearanceHandler { get;  set; }
        public string ContactInfo { get; set; }              
        public string Location { get; set; }
        public string Reason { get; set; }
        public XRect Rectangle { get;  set; }
    }
}
