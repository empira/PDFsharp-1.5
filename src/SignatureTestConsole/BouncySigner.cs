using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class BouncySigner : PdfSharp.Pdf.Signatures.ISigner
    {
        private X509Certificate2 Certificate { get; set; }
        public string GetName()
        {
            return Certificate.GetNameInfo(X509NameType.SimpleName, false);
        }

        public BouncySigner(X509Certificate2 certificate)
        {
            this.Certificate = certificate;
        }

        public byte[] GetSignedCms(Stream rangedStream)
        {
            rangedStream.Position = 0;
            
            CmsSignedDataGenerator signedDataGenerator = new CmsSignedDataGenerator();

            var cert = DotNetUtilities.FromX509Certificate(Certificate);
            var key = DotNetUtilities.GetKeyPair(Certificate.PrivateKey);

            IX509Store x509Certs = X509StoreFactory.Create(
            "Certificate/Collection",
            new X509CollectionStoreParameters(new[] { cert }));

            signedDataGenerator.AddSigner(key.Private, cert, CmsSignedDataGenerator.DigestSha1);
            signedDataGenerator.AddCertificates(x509Certs);

            CmsProcessableInputStream msg = new CmsProcessableInputStream(rangedStream);

            CmsSignedData signedData = signedDataGenerator.Generate(msg, false);

            return signedData.GetEncoded();

        }


    }
}
