using System.IO;

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

        internal void AddEmbeddedFile(string name, Stream stream)
        {
            if (_embeddedFiles == null)
            {
                _embeddedFiles = new PdfNameTreeNode(true);
                Owner.Internals.AddObject(_embeddedFiles);
                Elements.SetReference(Keys.EmbeddedFiles, _embeddedFiles.Reference);
            }

            var embeddedFileStream = new PdfEmbeddedFileStream(Owner, stream);
            var fileSpecification = new PdfFileSpecification(Owner, embeddedFileStream, name);
            Owner.Internals.AddObject(fileSpecification);

            _embeddedFiles.AddName(name, fileSpecification.Reference);
        }
        private PdfNameTreeNode _embeddedFiles;

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

            ///// <summary>
            ///// (Optional; PDF 1.3) A name tree mapping name strings to annotation appearance streams
            ///// (see Section 8.4.4, “Appearance Streams”).
            ///// </summary>
            //[KeyInfo("1.3", KeyType.NameTree | KeyType.Optional)]
            //public const string AP = "/AP";

            ///// <summary>
            ///// (Optional; PDF 1.3) A name tree mapping name strings to document-level JavaScript actions
            ///// (see “JavaScript Actions” on page 709).
            ///// </summary>
            //[KeyInfo("1.3", KeyType.NameTree | KeyType.Optional)]
            //public const string JavaScript = "/JavaScript";

            ///// <summary>
            ///// (Optional; PDF 1.3) A name tree mapping name strings to visible pages for use in interactive forms
            ///// (see Section 8.6.5, “Named Pages”).
            ///// </summary>
            //[KeyInfo("1.3", KeyType.NameTree | KeyType.Optional)]
            //public const string Pages = "/Pages";

            ///// <summary>
            ///// (Optional; PDF 1.3) A name tree mapping name strings to invisible (template) pages for use in
            ///// interactive forms (see Section 8.6.5, “Named Pages”).
            ///// </summary>
            //[KeyInfo("1.3", KeyType.NameTree | KeyType.Optional)]
            //public const string Templates = "/Templates";

            ///// <summary>
            ///// (Optional; PDF 1.3) A name tree mapping digital identifiers to Web Capture content sets
            ///// (see Section 10.9.3, “Content Sets”).
            ///// </summary>
            //[KeyInfo("1.3", KeyType.NameTree | KeyType.Optional)]
            //public const string IDS = "/IDS";

            ///// <summary>
            ///// (Optional; PDF 1.3) A name tree mapping uniform resource locators (URLs) to Web Capture content sets
            ///// (see Section 10.9.3, “Content Sets”).
            ///// </summary>
            //[KeyInfo("1.3", KeyType.NameTree | KeyType.Optional)]
            //public const string URLS = "/URLS";

            /// <summary>
            /// (Optional; PDF 1.4) A name tree mapping name strings to file specifications for embedded file streams
            /// (see Section 3.10.3, “Embedded File Streams”).
            /// </summary>
            [KeyInfo("1.4", KeyType.NameTree | KeyType.Optional)]
            public const string EmbeddedFiles = "/EmbeddedFiles";

            ///// <summary>
            ///// (Optional; PDF 1.4) A name tree mapping name strings to alternate presentations
            ///// (see Section 9.4, “Alternate Presentations”).
            ///// </summary>
            //[KeyInfo("1.4", KeyType.NameTree | KeyType.Optional)]
            //public const string AlternatePresentations = "/AlternatePresentations";

            ///// <summary>
            ///// (Optional; PDF 1.5) A name tree mapping name strings (which must have Unicode encoding) to
            ///// rendition objects (see Section 9.1.2, “Renditions”).
            ///// </summary>
            //[KeyInfo("1.5", KeyType.NameTree | KeyType.Optional)]
            //public const string Renditions = "/Renditions";

            // ReSharper restore InconsistentNaming
        }

    }
}
