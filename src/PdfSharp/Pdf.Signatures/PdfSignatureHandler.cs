using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static PdfSharp.Pdf.AcroForms.PdfAcroField;

namespace PdfSharp.Signatures
{
    public class IntEventArgs : EventArgs { public int Value { get; set; } }

    /// <summary>
    /// Handles the signature
    /// </summary>
    public class PdfSignatureHandler
    {



        private PositionTracker contentsTraker;
        private PositionTracker rangeTracker;
        private int? maximumSignatureLength;
        private const int byteRangePaddingLength = 36; // the place big enough required to replace [0 0 0 0] with the correct value

        public event EventHandler<IntEventArgs> SignatureSizeComputed = (s, e) => { };

        public PdfDocument Document { get; private set; }
        public X509Certificate2 Certificate { get; private set; }
        public PdfSignatureOptions Options { get; private set; }
        

        public void AttachToDocument(PdfDocument documentToSign)
        {            
            this.Document = documentToSign;
            this.Document.BeforeSave += AddSignatureComponents;
            this.Document.AfterSave += ComputeSignatureAndRange;

            if (!maximumSignatureLength.HasValue)
            {
                maximumSignatureLength = GetSignature(new byte[] { 0 }).Length;
                SignatureSizeComputed(this, new IntEventArgs() { Value = maximumSignatureLength.Value });
            }
        }

        public PdfSignatureHandler(X509Certificate2 certificate, int? signatureMaximumLength, PdfSignatureOptions options)
        {            
            this.Certificate = certificate;
           

            this.maximumSignatureLength = signatureMaximumLength;
            this.Options = options;           
        }

        private void ComputeSignatureAndRange(object sender, PdfDocumentEventArgs e)
        {
            var writer = e.Writer;
            writer.Stream.Position = rangeTracker.Start;
            var rangeArray = new PdfArray(new PdfInteger(0), 
                new PdfInteger(contentsTraker.Start), 
                new PdfInteger(contentsTraker.End), 
                new PdfInteger((int)writer.Stream.Length - contentsTraker.End));
            rangeArray.Write(writer);

            var rangeToSign = GetRangeToSign(writer.Stream);

            var signature = GetSignature(rangeToSign);
            if (signature.Length > maximumSignatureLength)
                throw new Exception("The signature length is bigger that the approximation made.");

            var hexFormated = Encoding.Default.GetBytes(FormatHex(signature));

            writer.Stream.Position = contentsTraker.Start+1;
            writer.Write(hexFormated);
        }


        private byte[] GetSignature(byte[] range)
        {
            var contentInfo = new ContentInfo(range);

            SignedCms signedCms = new SignedCms(contentInfo, true);
            CmsSigner signer = new CmsSigner(Certificate);
            signer.UnsignedAttributes.Add(new Pkcs9SigningTime());

            signedCms.ComputeSignature(signer, true);
            var bytes = signedCms.Encode();

            return bytes;
        }

        string FormatHex(byte[] bytes)
        {
            var retval = new StringBuilder();

            for (int idx = 0; idx < bytes.Length; idx++)
                retval.AppendFormat("{0:x2}", bytes[idx]);

            return retval.ToString();
        }

        private byte[] GetRangeToSign(Stream stream)
        {            
            stream.Position = 0;           

            var size = contentsTraker.Start + stream.Length - contentsTraker.End;
            var signRange = new byte[size];

            for (int i = 0;  i < signRange.Length; i++)
            {
                if (i == contentsTraker.Start)
                    stream.Position = contentsTraker.End;
                signRange[i] = (byte)stream.ReadByte();
                
            }
                       
            return signRange;
        }       

        private void AddSignatureComponents(object sender, EventArgs e)
        {
            var catalog = Document.Catalog;

            if (catalog.AcroForm == null)
                catalog.AcroForm = new PdfAcroForm(Document);

            catalog.AcroForm.Elements.Add(PdfAcroForm.Keys.SigFlags, new PdfInteger(3));

            var signature = new PdfSignatureField(Document);

            var paddedContents = new PdfString("", PdfStringFlags.HexLiteral, maximumSignatureLength.Value);
            var paddedRange = new PdfArray(Document, byteRangePaddingLength, new PdfInteger(0), new PdfInteger(0), new PdfInteger(0), new PdfInteger(0));

            signature.Contents = paddedContents;
            signature.ByteRange = paddedRange;
            signature.Reason = Options.Reason;
            signature.Location = Options.Location;
            signature.Rectangle = new PdfRectangle(Options.Rectangle);
            signature.AppearanceHandler = Options.AppearanceHandler ?? new DefaultAppearanceHandler()
            {                
                Location = Options.Location,
                Reason = Options.Reason,
                Signer = Certificate.GetNameInfo(X509NameType.SimpleName, false)
            };
            signature.PrepareForSave();           

            this.contentsTraker = new PositionTracker(paddedContents);
            this.rangeTracker = new PositionTracker(paddedRange);

            if (!Document.Pages[0].Elements.ContainsKey(PdfPage.Keys.Annots))
                Document.Pages[0].Elements.Add(PdfPage.Keys.Annots, new PdfArray(Document));

            (Document.Pages[0].Elements[PdfPage.Keys.Annots] as PdfArray).Elements.Add(signature);
                        
            catalog.AcroForm.Fields.Elements.Add(signature);

        }
    }
}
