using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfSharper.Pdf.AcroForms;
using PdfSharper.Pdf;
using System.IO;
using PdfSharper.Pdf.Annotations;
using PdfSharper.Drawing;
using PdfSharper.Pdf.IO;
using PdfSharper.Pdf.Advanced;

namespace PDFsharper.UnitTests
{
    [TestClass]
    public class PdfTextFieldTests
    {
        [TestMethod]
        public void RenderAppearance()
        {
            PdfDocument testDoc = new PdfDocument();
            SetupAcroFormForDocument(testDoc);

            PdfTextField tf = new PdfTextField(testDoc);
            tf.Rectangle = new PdfRectangle(new XRect(0, 0, 200, 20));

            PdfPage p1 = testDoc.Pages.Add();
            testDoc.Catalog.AcroForm = new PdfAcroForm(testDoc);
            testDoc.AcroForm.Elements.SetValue(PdfAcroForm.Keys.Fields, new PdfAcroField.PdfAcroFieldCollection(new PdfArray(testDoc)));
            testDoc.AcroForm.Fields.Add(tf, 1);

            tf.Text = "Some Test Text";
            tf.PrepareForSave();

            Assert.IsTrue(tf.Elements.ContainsKey(PdfAnnotation.Keys.AP), "Text Field should have rendered an appearance stream.");
        }

        [TestMethod]
        public void RenderAppearance_Empty()
        {
            PdfDocument testDoc = new PdfDocument();
            SetupAcroFormForDocument(testDoc);

            PdfTextField tf = new PdfTextField(testDoc);
            tf.Rectangle = new PdfRectangle(new XRect(0, 0, 200, 20));

            PdfPage p1 = testDoc.Pages.Add();
            testDoc.Catalog.AcroForm = new PdfAcroForm(testDoc);
            testDoc.AcroForm.Elements.SetValue(PdfAcroForm.Keys.Fields, new PdfAcroField.PdfAcroFieldCollection(new PdfArray(testDoc)));
            testDoc.AcroForm.Fields.Add(tf, 1);

            tf.Text = "Some Test Text";
            tf.PrepareForSave();

            Assert.IsTrue(tf.Elements.ContainsKey(PdfAnnotation.Keys.AP), "Text Field should have rendered an appearance stream.");

            tf.Text = string.Empty;

            tf.PrepareForSave();

            Assert.IsTrue(tf.Elements.ContainsKey(PdfAnnotation.Keys.AP), "Empty Text Field should have an appearance stream, used for background colors.");
        }
        private static void SetupAcroFormForDocument(PdfDocument testDoc)
        {
            testDoc.Catalog.AcroForm = new PdfAcroForm(testDoc);



            PdfDictionary helvFont = new PdfDictionary(testDoc);
            helvFont.Elements.SetName("/BaseFont", "Helvetica");
            helvFont.Elements.SetName("/Name", "/Helv");
            helvFont.Elements.SetName("/Subtype", "/Type1");
            helvFont.Elements.SetName("/Type", "/Font");

            testDoc.Internals.AddObject(helvFont);

            PdfDictionary fontDictionary = new PdfDictionary(testDoc);
            fontDictionary.Elements.SetReference("/Helv", helvFont);

            PdfDictionary resourceDict = new PdfDictionary(testDoc);
            resourceDict.Elements.SetObject("/Font", fontDictionary);
            testDoc.AcroForm.Elements.SetObject("/DR", resourceDict);
        }
    }
}
