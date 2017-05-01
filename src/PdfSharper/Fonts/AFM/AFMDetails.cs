using System.Collections.Generic;

namespace PdfSharper.Fonts.AFM
{
    public class AFMDetails
    {
        public string FontName { get; set; }
        public int Ascender { get; set; }
        public int Descender { get; set; }
        public int CapHeight { get; set; }
        public int BBoxLLX { get; set; }
        public int BBoxLLY { get; set; }
        public int BBoxURX { get; set; }
        public int BBoxURY { get; set; }
        public Dictionary<char, int> CharacterWidths { get; set; }
    }
}
