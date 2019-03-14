#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2019 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using PdfSharp.Pdf.Internal;

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Represents an XML Metadata stream.
    /// </summary>
    public sealed class PdfMetadata : PdfDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMetadata"/> class.
        /// </summary>
        public PdfMetadata()
        {
            Elements.SetName(Keys.Type, "/Metadata");
            Elements.SetName(Keys.Subtype, "/XML");
            SetupStream();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfMetadata"/> class.
        /// </summary>
        /// <param name="document">The document that owns this object.</param>
        public PdfMetadata(PdfDocument document)
            : base(document)
        {
            document.Internals.AddObject(this);
            Elements.SetName(Keys.Type, "/Metadata");
            Elements.SetName(Keys.Subtype, "/XML");
            SetupStream();
        }

        void SetupStream()
        {
            var stream = GenerateXmp();
            
            byte[] bytes = PdfEncoders.RawEncoding.GetBytes(stream);
            CreateStream(bytes);
        }

        string GenerateXmp()
        {
            var instanceId = Guid.NewGuid().ToString();
            var documentId = Guid.NewGuid().ToString();

            var creationDate = _document.Info.CreationDate.ToString("o");
            var modificationDate = _document.Info.CreationDate.ToString("o");

            var creator = _document.Info.Creator;
            var producer = _document.Info.Producer;
            var title = _document.Info.Title;

            // XMP Documentation: http://wwwimages.adobe.com/content/dam/Adobe/en/devnet/xmp/pdfs/XMP%20SDK%20Release%20cc-2016-08/XMPSpecificationPart1.pdf

            var str = 
            // UTF-8 Byte order mark "ï»¿" and GUID (like in Reference) to avoid accidental usage in data stream.
            "<?xpacket begin=\"ï»¿\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?>\n" + 

            "    <x:xmpmeta xmlns:x=\"adobe:ns:meta/\"> \n" +
            "      <rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">\n" +
            "        <rdf:Description rdf:about=\"\" xmlns:xmpMM=\"http://ns.adobe.com/xap/1.0/mm/\">\n" +

            "          <xmpMM:InstanceID>uuid:" + instanceId + "</xmpMM:InstanceID>\n" +
            "          <xmpMM:DocumentID>uuid:" + documentId + "</xmpMM:DocumentID>\n" +

            "        </rdf:Description>\n" +
            "        <rdf:Description rdf:about=\"\" xmlns:pdfuaid=\"http://www.aiim.org/pdfua/ns/id/\">\n" +
            "          <pdfuaid:part>1</pdfuaid:part>\n" +
            "        </rdf:Description>\n" +
            "        <rdf:Description rdf:about=\"\" xmlns:xmp=\"http://ns.adobe.com/xap/1.0/\">\n" +

            "          <xmp:CreateDate>" + creationDate + "</xmp:CreateDate>\n" +
            "          <xmp:ModifyDate>" + modificationDate + "</xmp:ModifyDate>\n" +
            "          <xmp:CreatorTool>" + creator + "</xmp:CreatorTool>\n" +
            "          <xmp:MetadataDate>" + modificationDate + "</xmp:MetadataDate>\n" +

            "        </rdf:Description>\n" +
            "        <rdf:Description rdf:about=\"\" xmlns:pdf=\"http://ns.adobe.com/pdf/1.3/\">\n" +

            "          <pdf:Producer>" + producer + "</pdf:Producer>\n" +

            "        </rdf:Description>\n" +
            "        <rdf:Description rdf:about=\"\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\">\n" +
            "          <dc:title>\n" +
            "            <rdf:Alt>\n" +

            "              <rdf:li xml:lang=\"x-default\">" + title + "</rdf:li>\n" +

            "            </rdf:Alt>\n" +
            "          </dc:title>\n" +
            "        </rdf:Description>\n" +
            "      </rdf:RDF>\n" +
            "    </x:xmpmeta>\n" +
            "<?xpacket end=\"r\"?>\n";

            return str;
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal class Keys : KeysBase
        {
            /// <summary>
            /// (Required) The type of PDF object that this dictionary describes; must be Metadata for a metadata stream.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional, FixedValue = "Metadata")]
            public const string Type = "/Type";

            /// <summary>
            /// (Required) The type of metadata stream that this dictionary describes; must be XML.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional, FixedValue = "XML")]
            public const string Subtype = "/Subtype";
        }
    }
}
