# PDFsharp
A .NET library for processing PDF

# Resources

The official project web site:  
http://pdfsharp.net/

The official peer-to-peer support forum:  
http://forum.pdfsharp.net/

# Release Notes for PDFsharp/MigraDoc 1.50 (stable)

The stable version of PDFsharp 1.32 was published in 2013.  
So a new stable version is long overdue.

I really hope the stable version does not have any regressions versus 1.50 beta 3b or later.

And I hope there are no regressions versus version 1.32 stable. But several bugs have been fixed.  
There are a few breaking changes that require code updates.

To use PDFsharp with Medium Trust you have to get the source code and make some changes. The NuGet packages do not support Medium Trust.  
Azure servers do not require Medium Trust.

I'm afraid that many users who never tried any beta version of PDFsharp 1.50 will now switch from version 1.32 stable to version 1.50 stable.  
Nothing wrong about that. I hope we don't get an avalanche of bug reports now.


# Which Version to Get?

The naming convention for the packages has changed.

If you are using "PdfSharp -Version 1.32.3057" then the version you need now is "PDFsharp-gdi -Version 1.50".  
Or get the corresponding package " PDFsharp-MigraDoc-GDI -Version 1.50".

