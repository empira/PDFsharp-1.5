using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Security;

namespace SecurityTest {
  class Program {
    static void Main(string[] args) {
      // Get a fresh copy of the sample PDF file
      const string filenameSource = "C:\\Users\\bcallaghan\\Downloads\\Rx Design Guidelines.pdf";
      const string filenameDest = "C:\\Users\\bcallaghan\\Desktop\\Protected.pdf";
      const string filenameEnd = "C:\\Users\\bcallaghan\\Desktop\\Unprotected.pdf";

      // Open an existing document. Providing an unrequired password is ignored.
      PdfDocument document = PdfReader.Open(filenameSource, "some text");

      // Setting one of the passwords automatically sets the security level to 
      // PdfDocumentSecurityLevel.Encrypted128Bit.
      document.SecuritySettings.UserPassword = "user";
      document.SecuritySettings.OwnerPassword = "owner";
      document.SecuritySettings.DocumentSecurityLevel = PdfDocumentSecurityLevel.Encrypted128BitAes;

      // Don't use 40 bit encryption unless needed for compatibility
      //securitySettings.DocumentSecurityLevel = PdfDocumentSecurityLevel.Encrypted40Bit;

      // Restrict some rights.
      document.SecuritySettings.PermitAccessibilityExtractContent = false;
      document.SecuritySettings.PermitAnnotations = false;
      document.SecuritySettings.PermitAssembleDocument = false;
      document.SecuritySettings.PermitExtractContent = false;
      document.SecuritySettings.PermitFormsFill = true;
      document.SecuritySettings.PermitFullQualityPrint = false;
      document.SecuritySettings.PermitModifyDocument = true;
      document.SecuritySettings.PermitPrint = false;

      // Save the document...
      document.Save(filenameDest);

      // Round-trip to unprotected
      //PdfDocument secure = PdfReader.Open(filenameDest, "owner");
      //secure.SecuritySettings.DocumentSecurityLevel = PdfDocumentSecurityLevel.None;
      //secure.Save(filenameEnd);
    }
  }
}
