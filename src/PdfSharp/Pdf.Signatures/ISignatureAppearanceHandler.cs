using PdfSharp.Drawing;

namespace PdfSharp.Signatures
{
    public interface ISignatureAppearanceHandler
    {
        void DrawAppearance(XGraphics gfx, XRect rect);
    }
}
