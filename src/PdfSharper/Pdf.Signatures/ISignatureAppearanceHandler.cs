using PdfSharper.Drawing;

namespace PdfSharper.Signatures
{
    public interface ISignatureAppearanceHandler
    {
        void DrawAppearance(XGraphics gfx, XRect rect);
    }
}
