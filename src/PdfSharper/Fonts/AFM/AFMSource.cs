using System.Collections.Generic;

namespace PdfSharper.Fonts.AFM
{
    public class AFMSource
    {
        private static readonly string COURIERBOLD = "PdfSharper.Fonts.AFM.Files.Courier-Bold.afm";
        private static readonly string COURIEROBLIQUE = "PdfSharper.Fonts.AFM.Files.Courier-Oblique.afm";
        private static readonly string COURIER = "PdfSharper.Fonts.AFM.Files.Courier.afm";
        private static readonly string COURIERBOLDOBLIQUE = "PdfSharper.Fonts.AFM.Files.Courier_BoldOblique.afm";
        private static readonly string HELVETICABOLD = "PdfSharper.Fonts.AFM.Files.Helvetica-Bold.afm";
        private static readonly string HELVETICABOLDOBLIQUE = "PdfSharper.Fonts.AFM.Files.Helvetica-BoldOblique.afm";
        private static readonly string HELVETICAOBLIQUE = "PdfSharper.Fonts.AFM.Files.Helvetica-Oblique.afm";
        private static readonly string HELVETICA = "PdfSharper.Fonts.AFM.Files.Helvetica.afm";
        private static readonly string SYMBOL = "PdfSharper.Fonts.AFM.Files.Symbol.afm";
        private static readonly string TIMESBOLD = "PdfSharper.Fonts.AFM.Files.Times-Bold.afm";
        private static readonly string TIMESBOLDITALIC = "PdfSharper.Fonts.AFM.Files.Times-BoldItalic.afm";
        private static readonly string TIMESITALIC = "PdfSharper.Fonts.AFM.Files.Times-Italic.afm";
        private static readonly string TIMESNEWROMAN = "PdfSharper.Fonts.AFM.Files.Times-New-Roman.afm";
        private static readonly string TIMESROMAN = "PdfSharper.Fonts.AFM.Files.Times-Roman.afm";
        private static readonly string ZAPFDINGBATS = "PdfSharper.Fonts.AFM.Files.ZapfDingbats.afm";

        public static IEnumerable<string> GetAll()
        {
            List<string> afmList = new List<string>();
            afmList.Add(COURIERBOLD);
            afmList.Add(COURIEROBLIQUE);
            afmList.Add(COURIER);
            afmList.Add(COURIERBOLDOBLIQUE);
            afmList.Add(HELVETICABOLD);
            afmList.Add(HELVETICABOLDOBLIQUE);
            afmList.Add(HELVETICAOBLIQUE);
            afmList.Add(HELVETICA);
            afmList.Add(SYMBOL);
            afmList.Add(TIMESBOLD);
            afmList.Add(TIMESBOLDITALIC);
            afmList.Add(TIMESITALIC);
            afmList.Add(TIMESNEWROMAN);
            afmList.Add(TIMESROMAN);
            afmList.Add(ZAPFDINGBATS);

            return afmList;
        }

        public static string GetSourceByName(string name)
        {
            string source = string.Empty;

            switch (name)
            {
                case "Courier-Bold":
                case "Courier Bold":
                    source = COURIERBOLD;
                    break;
                case "Courier Oblique":
                case "Courier-Oblique":
                    source = COURIEROBLIQUE;
                    break;
                case "Courier":
                    source = COURIER;
                    break;
                case "Courier-BoldOblique":
                case "Courier Bold Oblique":
                    source = COURIERBOLDOBLIQUE;
                    break;
                case "Helvetica-Bold":
                case "Helvetica Bold":
                    source = HELVETICABOLD;
                    break;
                case "Helvetica-BoldOblique":
                case "Helvetica Bold Oblique":
                    source = HELVETICABOLDOBLIQUE;
                    break;
                case "Helvetica-Oblique":
                case "Helvetica Oblique":
                    source = HELVETICAOBLIQUE;
                    break;
                case "Helvetica":
                    source = HELVETICA;
                    break;
                case "Symbol":
                    source = SYMBOL;
                    break;
                case "Times-Bold":
                case "Times Bold":
                    source = TIMESBOLD;
                    break;
                case "Times-BoldItalic":
                case "Times Bold Italic":
                    source = TIMESBOLDITALIC;
                    break;
                case "Times-Italic":
                case "Times Italic":
                    source = TIMESITALIC;
                    break;
                case "TimesNewRomanPSMT":
                case "Times New Roman":
                    source = TIMESNEWROMAN;
                    break;
                case "Times-Roman":
                case "Times Roman":
                    source = TIMESROMAN;
                    break;
                case "ZapfDingbats":
                case "ITC Zapf Dingbats":
                    source = ZAPFDINGBATS;
                    break;
                default:
                    break;
            }

            return source;
        }

        public static string GetSourceByNameAndAttributes(string name, bool isBold, bool isItalic)
        {
            string source = string.Empty;

            if (name == "Courier" && !isBold && !isItalic)
            {
                source = COURIER;
            }
            else if (name == "Courier" && isBold && !isItalic)
            {
                source = COURIERBOLD;
            }
            else if (name == "Courier" && !isBold && isItalic)
            {
                source = COURIEROBLIQUE;
            }
            else if (name == "Courier" && isBold && isItalic)
            {
                source = COURIERBOLDOBLIQUE;
            }
            else if (name == "Helvetica" && !isBold && !isItalic)
            {
                source = HELVETICA;
            }
            else if (name == "Helvetica" && isBold && !isItalic)
            {
                source = HELVETICABOLD;
            }
            else if (name == "Helvetica" && !isBold && isItalic)
            {
                source = HELVETICAOBLIQUE;
            }
            else if (name == "Helvetica" && isBold && isItalic)
            {
                source = HELVETICABOLDOBLIQUE;
            }
            else if ((name == "Times Roman" || name == "Times-Roman" || name == "Times") && !isBold && !isItalic)
            {
                source = TIMESROMAN;
            }
            else if ((name == "Times Roman" || name == "Times-Roman" || name == "Times") && isBold && !isItalic)
            {
                source = TIMESBOLD;
            }
            else if ((name == "Times Roman" || name == "Times-Roman" || name == "Times") && !isBold && isItalic)
            {
                source = TIMESITALIC;
            }
            else if ((name == "Times Roman" || name == "Times-Roman" || name == "Times") && isBold && isItalic)
            {
                source = TIMESBOLDITALIC;
            }
            else if (name == "Times New Roman" || name == "TimesNewRomanPSMT")
            {
                source = ZAPFDINGBATS;
            }
            else if (name == "Symbol")
            {
                source = SYMBOL;
            }
            else if (name == "ZapfDingbats" || name == "ITC Zapf Dingbats")
            {
                source = ZAPFDINGBATS;
            }

            return source;
        }
    }
}
