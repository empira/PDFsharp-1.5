using System;

namespace PdfSharp.Pdf.Security
{
    /// <summary>
    /// The method that should be used when decrypting data.
    /// </summary>
    internal enum CFM
    {
        /// <summary>
        /// Applications that encounter this value shall report that the file is encrypted with an unsupported algorithm.
        /// </summary>
        Unknown,

        /// <summary>
        /// The application shall not decrypt data but shall direct the input stream to the security handler for decryption.
        /// </summary>
        None,

        /// <summary>
        /// The application shall ask the security handler for the encryption key and shall implicitly decrypt data with
        /// "Algorithm 1: Encryption of data using the RC4 or AES algorithms", using the RC4 algorithm.
        /// </summary>
        V2,

        /// <summary>
        /// (PDF 1.6) The application shall ask the security handler for the encryption key and shall implicitly decrypt data with
        /// "Algorithm 1: Encryption of data using the RC4 or AES algorithms", using the AES algorithm in Cipher Block Chaining (CBC)
        /// mode with a 16-byte block size and an initialization vector that shall be randomly generated and placed as the first
        /// 16 bytes in the stream or string.
        /// </summary>
        AESV2
    }

    /// <summary>
    /// The event that will trigger authorization.
    /// </summary>
    internal enum AuthEvent
    {
        /// <summary>
        /// Authorization shall be required when a document is opened.
        /// </summary>
        DocOpen,

        /// <summary>
        /// Authorization shall be required when accessing embedded files.
        /// </summary>
        EFOpen
    }

    /// <summary>
    /// Represents a crypt filter dictionary used for providing granular control of encryption.
    /// </summary>
    internal class CryptFilterDictionary : PdfDictionary
    {
        /// <summary>
        /// Constructs a new CryptFilterDictionary with the required Type name.
        /// </summary>
        public CryptFilterDictionary()
        {
            Type = "CryptFilter";
        }

        /// <summary>
        /// (Optional) If present, shall be CryptFilter for a crypt filter dictionary.
        /// </summary>
        public string Type
        {
            get { return Elements.GetName("/Type"); }
            private set { Elements.SetName("/Type", value); }
        }

        /// <summary>
        /// (Optional) The method used, if any, by the conforming reader to decrypt data. The following
        /// values shall be supported:
        /// • None The application shall not decrypt data but shall direct the input stream to the security
        ///   handler for decryption.
        /// • V2 The application shall ask the security handler for the encryption key and shall implicity
        ///   decrypt data with "Algorithm 1: Encryption of data using the RC4 or AES algorithms", using the
        ///   RC4 algorithm.
        /// • AESV2 (PDF 1.6) The application shall ask the security handler for the encryption key and shall
        ///   implicitly decrypt data with "Algorithm 1: Encryption of data using the RC4 or AES algorithms",
        ///   using the AES algorithm in Cipher Block Chaining (CBC) mode with a 16-byte block size and an
        ///   initialization vector that shall be randomly generated and placed as the first 16 bytes in the
        ///   stream or string.
        /// When the value is V2 or AESV2, the application may ask once for this encryption key and cache the key
        /// for subsequent use for streams that use the same crypt filter. Therefore, there shall be a one-to-one
        /// relationship between a crypt filter name and the corresponding encryption key.
        /// Only the values listed here shall be supported. Applications that encounter other values shall report
        /// that the file is encrypted with an unsupported algorithm.
        /// Default value: None.
        /// </summary>
        public CFM CFM
        {
            get
            {
                string cfmName = Elements.GetName("/CFM");
                switch (cfmName)
                {
                    case "/None":
                        return CFM.None;
                    case "/V2":
                        return CFM.V2;
                    case "/AESV2":
                        return CFM.AESV2;
                    default:
                        return CFM.Unknown;
                }
            }
            set
            {
                string cfmName;
                switch (value)
                {
                    case CFM.None:
                        cfmName = "None";
                        break;
                    case CFM.V2:
                        cfmName = "V2";
                        break;
                    case CFM.AESV2:
                        cfmName = "AESV2";
                        break;
                    case CFM.Unknown:
                    default:
                        throw new ArgumentOutOfRangeException("value", "The CFM must be a valid value.");
                }
                Elements.SetName("/CFM", cfmName);
            }
        }

        /// <summary>
        /// (Optional) The event to be used to trigger the authorization that is required to access encryption
        /// keys used by this filter. If authorization fails, the event shall fail. Valid values shall be:
        /// • DocOpen: Authorization shall be required when a document is opened.
        /// • EFOpen: Authorization shall be required when accessing embedded files.
        /// Default value: DocOpen.
        /// If this filter is used as the value of StrF or StmF in the encryption dictionary, the conforming
        /// reader shall ignore this key and behave as if the value is DocOpen.
        /// </summary>
        public AuthEvent AuthEvent
        {
            get
            {
                string authEventName = Elements.GetName("/AuthEvent");
                switch (authEventName)
                {
                    case "/DocOpen":
                        return AuthEvent.DocOpen;
                    case "/EFOpen":
                        return AuthEvent.EFOpen;
                    default:
                        return AuthEvent.DocOpen;
                }
            }
            set
            {
                string authEventName;
                switch (value)
                {
                    case AuthEvent.DocOpen:
                        authEventName = "DocOpen";
                        break;
                    case AuthEvent.EFOpen:
                        authEventName = "EFOpen";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value", "The AuthEvent must be a valid value.");
                }
                Elements.SetName("/AuthEvent", authEventName);
            }
        }

        /// <summary>
        /// (Optional) The bit length of the encryption key. It shall be a multiple of 8 in the range of 40 to 128.
        /// Security handleres may define their own use of the Length entry and should use it to define the bit length
        /// of the encryption key. Standard security handler expresses the length in multiples of 8 (16 means 128)
        /// and public-key security handler express it as is (128 means 128)
        /// </summary>
        public int Length
        {
            get { return Elements.GetInteger("/Length"); }
            set
            {
                int valueToTest = value;
                if (value < 40)
                {
                    // Value can be in bits or bytes.
                    valueToTest = value * 8;
                }

                if (valueToTest < 40 || valueToTest > 128)
                    throw new ArgumentOutOfRangeException("value", "The Length must be between 40 and 128 bits inclusive.");
                if (valueToTest % 8 != 0)
                    throw new ArgumentException("The Length must be a multiple of 8 bits.", "value");

                Elements.SetInteger("/Length", value);
            }
        }
    }
}
