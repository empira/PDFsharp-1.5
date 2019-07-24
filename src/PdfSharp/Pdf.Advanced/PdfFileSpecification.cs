namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// Represents a file specification dictionary.
    /// </summary>
    public class PdfFileSpecification : PdfDictionary
    {
        /// <summary>
        /// Initializes a new instance of PdfFileSpecification refering an embedded file stream.
        /// </summary>
        public PdfFileSpecification(PdfDocument document, PdfEmbeddedFileStream embeddedFileStream, string name) : base(document)
        {
            _embeddedFileStream = embeddedFileStream;
            _name = name;

            Initialize();
        }

        private void Initialize()
        {
            Elements.SetName(Keys.Type, "/Filespec");

            Elements.SetString(Keys.F, _name);
            Elements.SetString(Keys.UF, _name);

            var embeddedFileDictionary = new PdfDictionary(Owner);

            Owner.Internals.AddObject(_embeddedFileStream);
            embeddedFileDictionary.Elements.SetReference(Keys.F, _embeddedFileStream.Reference);
            embeddedFileDictionary.Elements.SetReference(Keys.UF, _embeddedFileStream.Reference);

            Elements.SetObject(Keys.EF, embeddedFileDictionary);
        }
        private readonly PdfEmbeddedFileStream _embeddedFileStream;
        private readonly string _name;

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        public class Keys : KeysBase
        {
            // ReSharper disable InconsistentNaming

            /// <summary>
            /// (Required if an EF or RF entry is present; recommended always)
            /// The type of PDF object that this dictionary describes; must be Filespec
            /// for a file specification dictionary (see implementation note 45 in Appendix H).
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string Type = "/Type";

            ///// <summary>
            ///// (Optional) The name of the file system to be used to interpret this file specification.
            ///// If this entry is present, all other entries in the dictionary are interpreted by the
            ///// designated file system. PDF defines only one standard file system name, URL
            ///// (see Section 3.10.4, “URL Specifications”); an application or plug-in extension can
            ///// register other names (see Appendix E). This entry is independent of the F, UF, DOS,
            ///// Mac, and Unixentries.
            ///// </summary>
            //[KeyInfo(KeyType.Name | KeyType.Optional)]
            //public const string FS = "/FS";

            /// <summary>
            /// (Required if the DOS, Mac, and Unix entries are all absent; amended with the UF entry
            /// for PDF 1.7) A file specification string of the form described in Section 3.10.1,
            /// “File Specification Strings,” or (if the file system is URL) a uniform resource locator,
            /// as described in Section 3.10.4, “URL Specifications.”
            /// Note: It is recommended that the UF entry be used in addition to the F entry.The UF entry
            /// provides cross-platform and cross-language compatibility and the F entry provides
            /// backwards compatibility.
            /// </summary>
            [KeyInfo(KeyType.String | KeyType.Optional)]
            public const string F = "/F";

            /// <summary>
            /// (Optional, but recommended if the F entry exists in the dictionary; PDF 1.7) A Unicode
            /// text string that provides file specification of the form described in Section 3.10.1,
            /// “File Specification Strings.” Note that this is a Unicode text string encoded using
            /// PDFDocEncoding or UTF-16BE with a leading byte-order marker (as defined in Section ,
            /// “Text String Type”). The F entry should always be included along with this entry for
            /// backwards compatibility reasons.
            /// </summary>
            [KeyInfo(KeyType.TextString | KeyType.Optional)]
            public const string UF = "/UF";

            ///// <summary>
            ///// (Optional) A file specification string (see Section 3.10.1, “File Specification Strings”)
            ///// representing a DOS file name.
            ///// Note: Beginning with PDF 1.7, use of the F entry and optionally the UF entry is recommended
            ///// in place of the DOS, Mac or Unix entries.
            ///// </summary>
            //[KeyInfo(KeyType.ByteString | KeyType.Optional)]
            //public const string DOS = "/DOS";

            ///// <summary>
            ///// (Optional) A file specification string (see Section 3.10.1, “File Specification Strings”)
            ///// representing a Mac OS file name.
            ///// Note: Beginning with PDF 1.7, use of the F entry and optionally the UF entry is recommended
            ///// in place of the DOS, Mac or Unix entries.
            ///// </summary>
            //[KeyInfo(KeyType.ByteString | KeyType.Optional)]
            //public const string Mac = "/Mac";

            ///// <summary>
            ///// (Optional) A file specification string (see Section 3.10.1, “File Specification Strings”)
            ///// representing a UNIX file name.
            ///// Note: Beginning with PDF 1.7, use of the F entry and optionally the UF entry is recommended
            ///// in place of the DOS, Mac or Unix entries.
            ///// </summary>
            //[KeyInfo(KeyType.ByteString | KeyType.Optional)]
            //public const string Unix = "/Unix";

            ///// <summary>
            ///// (Optional) An array of two byte strings constituting a file identifier (see Section 10.3,
            ///// “File Identifiers”) that is also included in the referenced file. The use of this entry
            ///// improves an application’s chances of finding the intended file and allows it to warn the
            ///// user if the file has changed since the link was made.
            ///// </summary>
            //[KeyInfo(KeyType.Array | KeyType.Optional)]
            //public const string ID = "/ID";

            ///// <summary>
            ///// (Optional; PDF 1.2) A flag indicating whether the file referenced by the file specification
            ///// is volatile (changes frequently with time). If the value is true, applications should never
            ///// cache a copy of the file. For example, a movie annotation referencing a URL to a live video
            ///// camera could set this flag to true to notify the application that it should reacquire the
            ///// movie each time it is played. Default value: false.
            ///// </summary>
            //[KeyInfo(KeyType.Boolean | KeyType.Optional)]
            //public const string V = "/V";

            /// <summary>
            /// (Required if RF is present; PDF 1.3; amended to include the UF key in PDF 1.7) A dictionary
            /// containing a subset of the keys F, UF, DOS, Mac, and Unix, corresponding to the entries by
            /// those names in the file specification dictionary. The value of each such key is an embedded
            /// file stream (see Section 3.10.3, “Embedded File Streams”) containing the corresponding file.
            /// If this entry is present, the Type entry is required and the file specification dictionary
            /// must be indirectly referenced. (See implementation note 46in Appendix H.)
            /// Note: It is recommended that the F and UF entries be used in place of the DOS, Mac, or Unix
            /// entries.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string EF = "/EF";

            ///// <summary>
            ///// (Optional; PDF 1.3) A dictionary with the same structure as the EF dictionary, which must also
            ///// be present. Each key in the RF dictionary must also be present in the EF dictionary. Each value
            ///// is a related files array (see “Related Files Arrays” on page 186) identifying files that are
            ///// related to the corresponding file in the EF dictionary. If this entry is present, the Type entry
            ///// is required and the file specification dictionary must be indirectly referenced.
            ///// </summary>
            //[KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            //public const string RF = "/RF";

            ///// <summary>
            ///// (Optional; PDF 1.6) Descriptive text associated with the file specification. It is used for
            ///// files in the EmbeddedFiles name tree (see Section 3.6.3, “Name Dictionary”).
            ///// </summary>
            //[KeyInfo(KeyType.TextString | KeyType.Optional)]
            //public const string Desc = "/Desc";

            ///// <summary>
            ///// (Optional; must be indirect reference; PDF 1.7) A collection item dictionary, which is used to
            ///// create the user interface for portable collections (see Section 3.10.5, “Collection Items).
            ///// </summary>
            //[KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            //public const string CI = "/CI";

            // ReSharper restore InconsistentNaming
        }
    }
}
