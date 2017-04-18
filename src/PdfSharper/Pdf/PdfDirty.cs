using PdfSharper.Pdf.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfSharper.Pdf
{
    public abstract class PdfDirty
    {
        public bool IsDirty { get; protected set; }
    }
}
