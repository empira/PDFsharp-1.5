using PdfSharp.Drawing;

namespace PdfSharp.Pdf.Signatures
{
    public interface ISignatureAppearanceHandler
    {
        void DrawAppearance(XGraphics gfx, XRect rect);
    }
}
