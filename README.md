# PDFSharpCoreOpt 

#### PDFSharpCoreOpt 

This is modified  Version of PDFSharp , this version is compatible with .net Core 

With this package you can create or modify PDFs with well positioned strings and Images 


## How To Install ?

With Package Manager install the Nuget package 
```
Install-Package PdfSharpCoreOpt -Version 1.0.0 
```



## Sample Code 

 
 
        static void Main(string[] args)
        {
         PdfDocument pdf = new PdfDocument();
            pdf.Info.Title = "My First PDF";
            //Loop to create Number of Pages 
            for (int i = 0; i < 500; i++)
            {
                PdfPage pdfPage = pdf.AddPage();
                XGraphics graph = XGraphics.FromPdfPage(pdfPage);
                
                //Specify your Font 
                XFont font = new XFont("Amiri", 5, XFontStyle.Regular);
                

                //Position the string you want to Write into the pdf 
                graph.DrawString("SSSSSSSSS" + i.ToString(), font, XBrushes.Black, new XRect(250, 360, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.Center);

                // Insert an image if you want
                string imageLoc = "test.png"; 

                graph.DrawImage(graph, imageLoc, 535, 700, 50, 50);//Specify the image and position it 
            }


            string pdfFilename = "My First PDF.pdf";
            pdf.Save(pdfFilename);
            }
