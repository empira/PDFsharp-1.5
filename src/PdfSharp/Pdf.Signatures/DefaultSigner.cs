using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PdfSharp.Pdf.Signatures
{
    public class DefaultSigner : ISigner
    {
        public X509Certificate2 Certificate { get; private set; }

        public DefaultSigner(X509Certificate2 Certificate)
        {
            this.Certificate = Certificate;
        }

        public byte[] GetSignedCms(Stream stream)
        {
            var range = new byte[stream.Length];

            stream.Position = 0;
            stream.Read(range, 0, range.Length);

           

            var contentInfo = new ContentInfo(range);

            SignedCms signedCms = new SignedCms(contentInfo, true);
            CmsSigner signer = new CmsSigner(Certificate);
            signer.UnsignedAttributes.Add(new Pkcs9SigningTime());

            signedCms.ComputeSignature(signer, true);
            var bytes = signedCms.Encode();

            return bytes;
        }

       

        public string GetName()
        {
            return Certificate.GetNameInfo(X509NameType.SimpleName, false);
        }
    }
}
