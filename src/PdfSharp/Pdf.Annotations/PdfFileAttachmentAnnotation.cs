using PdfSharp.Pdf.Advanced;
using System;

namespace PdfSharp.Pdf.Annotations
{
    /// <summary>
    /// Represent a file that is attached to the PDF
    /// </summary>
    public class PdfFileAttachmentAnnotation : PdfAnnotation
    {
        /// <summary>
        /// Name of icons used in displaying the annotation.
        /// </summary>
        public enum IconType
        {
            Graph,
            PushPin,
            Paperclip,
            Tag
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFileAttachmentAnnotation"/> class.
        /// </summary>
        public PdfFileAttachmentAnnotation()
        {
            Elements.SetName(Keys.Subtype, "/FileAttachment");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfFileAttachmentAnnotation"/> class.
        /// </summary>
        public PdfFileAttachmentAnnotation(PdfDocument document)
          : base(document)
        {
            Elements.SetName(Keys.Subtype, "/FileAttachment");
            Flags = PdfAnnotationFlags.Locked;
        }

        public IconType Icon
        {
            get
            {
                var iconName = Elements.GetName(Keys.Name);

                if (iconName == null)
                    return IconType.PushPin;

                return (IconType)(Enum.Parse(typeof(IconType), iconName));
            }
            set { Elements.SetName(Keys.Name, value.ToString()); }
        }

        public PdfFileSpecification File
        {
            get
            {
                var reference = Elements.GetReference(Keys.FS);

                return reference?.Value as PdfFileSpecification;
            }
            set
            {
                if (value == null)
                {
                    Elements.Remove(Keys.FS);
                }
                else
                {
                    if (!value.IsIndirect)
                        Owner._irefTable.Add(value);

                    Elements.SetReference(Keys.FS, value);
                }
            }
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal new class Keys : PdfAnnotation.Keys
        {
            /// <summary>
            /// (Required) The file associated with this annotation.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Required)]
            public const string FS = "/FS";

            /// <summary>
            /// (Optional) The name of an icon to be used in displaying the annotation. 
            /// Viewer applications should provide predefined icon appearances for at least 
            /// the following standard names:
            /// 
            /// Graph
            /// PushPin
            /// Paperclip
            /// Tag
            /// 
            /// Additional names may be supported as well. Default value: PushPin.
            /// Note: The annotation dictionary’s AP entry, if present, takes precedence over 
            /// the Name entry; see Table 8.15 on page 606 and Section 8.4.4, “Appearance Streams.”
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string Name = "/Name";

            /// <summary>
            /// Gets the KeysMeta for these keys.
            /// </summary>
            public static DictionaryMeta Meta
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
