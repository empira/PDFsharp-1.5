namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// Represents the name dictionary.
    /// </summary>
    public sealed class PdfNameDictionary : PdfDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfNameDictionary"/> class.
        /// </summary>
        public PdfNameDictionary(PdfDocument document)
            : base(document)
        {
        }

        internal PdfNameDictionary(PdfDictionary dictionary)
            : base(dictionary)
        {
        }

        internal void AddNamedDestination(string destinationName, int destinationPage, PdfNamedDestinationParameters parameters)
        {
            if (_dests == null)
            {
                _dests = new PdfNameTreeNode(true);
                Owner.Internals.AddObject(_dests);
                Elements.SetReference(Keys.Dests, _dests.Reference);
            }

            // destIndex > Owner.PageCount can happen when rendering pages using PDFsharp directly.
            int destIndex = destinationPage;
            if (destIndex > Owner.PageCount)
                destIndex = Owner.PageCount;
            destIndex--;
            PdfPage dest = Owner.Pages[destIndex];

#if true
            PdfArray destination = new PdfArray(Owner,
                new PdfLiteral("{0} 0 R {1}", dest.ObjectNumber, parameters));
            _dests.AddName(destinationName, destination);
#else
// Insert reference to destination dictionary instead of inserting the destination array directly.
            PdfArray destination = new PdfArray(Owner, new PdfLiteral("{0} 0 R {1}", dest.ObjectNumber, parameters));
            PdfDictionary destinationDict = new PdfDictionary(Owner);
            destinationDict.Elements.SetObject("/D", destination);
            Owner.Internals.AddObject(destinationDict);
            _dests.AddName(destinationName, destinationDict.Reference);
#endif
        }
        private PdfNameTreeNode _dests;

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal sealed class Keys : KeysBase
        {
            // ReSharper disable InconsistentNaming

            /// <summary>
            /// (Optional; PDF 1.2) A name tree mapping name strings to destinations (see “Named Destinations” on page 583).
            /// </summary>
            [KeyInfo("1.2", KeyType.NameTree | KeyType.Optional)]
            public const string Dests = "/Dests";
        }

    }
}
