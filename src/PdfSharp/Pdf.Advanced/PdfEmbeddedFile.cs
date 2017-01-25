namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// Represent a file stream embedded in the PDF document
    /// </summary>
    public class PdfEmbeddedFile : PdfDictionary
    {
        private readonly PdfDictionary paramsDictionary;

        public PdfEmbeddedFile(PdfDocument document)
          : base(document)
        {
            this.paramsDictionary = new PdfDictionary();

            Elements.SetName(Keys.Type, "/EmbeddedFile");
            Elements.SetObject(Keys.Params, paramsDictionary);
        }

        public PdfEmbeddedFile(PdfDocument document, byte[] bytes, string checksum = null) 
            : this(document)
        {
            this.CreateStreamAndSetProperties(bytes, checksum);
        }

        public void CreateStreamAndSetProperties(byte[] bytes, string checksum = null)
        {
            this.CreateStream(bytes);

            this.paramsDictionary.Elements.SetInteger(Keys.Size, bytes.Length);

            if (string.IsNullOrEmpty(checksum))
                this.paramsDictionary.Elements.Remove(Keys.CheckSum);
            else
                this.paramsDictionary.Elements.SetString(Keys.CheckSum, checksum);
        }

        public string MimeType
        {
            get { return Elements.GetName(Keys.Subtype); }
            set { Elements.SetName(Keys.Subtype, value); }
        }        

        // TODO : Add properties for the subdictionnary Params and the subsubdictionnary Mac

        /// <summary>
        /// Predefined keys of this embedded file.
        /// </summary>
        public class Keys : PdfDictionary.PdfStream.Keys
        {
            /// <summary>
            /// (Optional) The type of PDF object that this dictionary describes; if present, 
            /// must be EmbeddedFile for an embedded file stream.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional, FixedValue = "EmbeddedFile")]
            public const string Type = "/Type";

            /// <summary>
            /// (Optional) The subtype of the embedded file. The value of this entry must be a 
            /// first-class name, as defined in Appendix E. Names without a registered prefix 
            /// must conform to the MIME media type names defined in Internet RFC 2046, 
            /// Multipurpose Internet Mail Extensions (MIME), Part Two: Media Types(see the 
            /// Bibliography), with the provision that characters not allowed in names must 
            /// use the 2-character hexadecimal code format described in Section 3.2.4, 
            /// “Name Objects.”
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string Subtype = "/Subtype";

            /// <summary>
            /// (Optional) An embedded file parameter dictionary containing additional, 
            /// file-specific information (see Table 3.43).
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string Params = "/Params";

            /// <summary>
            /// (Optional) The size of the embedded file, in bytes.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string Size = "/Size";

            /// <summary>
            /// (Optional) The date and time when the embedded file was created.
            /// </summary>
            [KeyInfo(KeyType.Date | KeyType.Optional)]
            public const string CreationDate = "/CreationDate";

            /// <summary>
            /// (Optional) The date and time when the embedded file was last modified.
            /// </summary>
            [KeyInfo(KeyType.Date | KeyType.Optional)]
            public const string ModDate = "/ModDate";

            /// <summary>
            /// (Optional) A subdictionary containing additional information specific to Mac OS files (see Table 3.44).
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string Mac = "/Mac";

            /// <summary>
            /// (Optional) A 16-byte string that is the checksum of the bytes of the uncompressed 
            /// embedded file. The checksum is calculated by applying the standard MD5 message-digest 
            /// algorithm (described in Internet RFC 1321, The MD5 Message-Digest Algorithm; see the 
            /// Bibliography) to the bytes of the embedded file stream.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string CheckSum = "/CheckSum";

            /// <summary>
            /// Gets the KeysMeta for these keys.
            /// </summary>
            internal static DictionaryMeta Meta
            {
                get
                {
                    if (Keys.meta == null)
                        Keys.meta = CreateMeta(typeof(Keys));
                    return Keys.meta;
                }
            }
            static DictionaryMeta meta;
        }

        /// <summary>
        /// Gets the KeysMeta of this dictionary type.
        /// </summary>
        internal override DictionaryMeta Meta
        {
            get { return Keys.Meta; }
        }
    }
}
