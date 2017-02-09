using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfSharp.Pdf.Signatures
{
    internal class PositionTracker
    {
        public PdfItem Item { get; private set; }
        public int Start { get; private set; }
        public int End { get; private set; }

        public PositionTracker(PdfItem item)
        {
            Item = item;
            Item.BeforeWrite += (s, e) =>
                this.Start = e.Position;
            Item.AfterWrite += (s, e) => 
                this.End = e.Position;
        }

       
    }
}
