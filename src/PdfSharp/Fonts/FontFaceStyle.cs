namespace PdfSharp.Fonts
{
    using System;
    using System.Drawing;

    internal class FontFaceStyle
    {
        public FontStyle Style { get; set; }
        public string FaceName { get; set; }

        public FontFaceStyle(string faceName, FontStyle style)
        {
            FaceName = faceName;
            Style = style;
        }

        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else {
                FontFaceStyle p = (FontFaceStyle)obj;
                return (FaceName.ToLowerInvariant() == p.FaceName.ToLowerInvariant()) && (Style == p.Style);
            }
        }

        public override int GetHashCode()
        {
            return FaceName.GetHashCode() ^ Style.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("FontFaceStyle({0}, {1})", FaceName, Style);
        }
    }
}
