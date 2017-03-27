using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcroFieldKeys = PdfSharper.Pdf.AcroForms.PdfAcroField.Keys;

namespace PdfSharper.Pdf.AcroForms.enums
{
    public enum PdfAcroFieldType
    {
        Unknown,
        PushButton,
        RadioButton,
        CheckBox,
        Text,
        ComboBox,
        ListBox,
        Signature,
    }
}
