namespace PdfSharp.Fonts
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    internal class FontLocalizator
    {
        private List<Uri> _fontDictionaries;
        private const string _trueTypeFileExt= ".ttf";

        #region ctor
        public FontLocalizator():this(new List<Uri>())
        {}

        public FontLocalizator(List<Uri> fontDictionaries)
        {
            _fontDictionaries = fontDictionaries;
            initialize();
        }

        #endregion
        #region public methods
        public string GetFontPath(string face, FontStyle style)
        {
            string faceName = getFaceName(face);
            FontFaceStyle fs = new FontFaceStyle(faceName, style);
            string fileFont = getFileName(fs);
            if (fileFont != null)
            {
                fileFont += _trueTypeFileExt;
                foreach (Uri d in _fontDictionaries)
                {
                    string fullPath = Path.Combine(d.AbsolutePath, fileFont);
                    if (System.IO.File.Exists(fullPath))
                        return fullPath;
                }

            }

            return null;
        }
        #endregion
        #region private methods
        private void initialize()
        {
            _fontDictionaries.AddRange(defaultOSFontDictionaries());
        }

        private List<Uri> defaultOSFontDictionaries()
        {
            int os = (int)Environment.OSVersion.Platform;
            List<Uri> result = new List<Uri>();
            if ((int)Environment.OSVersion.Platform == (int)PlatformID.Unix)
            {
                if (System.IO.Directory.Exists("/usr/share/fonts/truetype/msttcorefonts"))
                {
                    result.Add(new Uri("/usr/share/fonts/truetype/msttcorefonts"));
                }
                else if (System.IO.Directory.Exists("/usr/share/fonts/truetype"))
                {
                    result.Add(new Uri("/usr/share/fonts/truetype"));
                }


            }
            else if (os == (int)PlatformID.Win32NT)
            {
                result.Add(new Uri(Path.Combine(System.Environment.GetEnvironmentVariable("windir"), "Fonts")));
            }


            return result;
        }

        private string getFileName(FontFaceStyle fs)
        {
            string prefixFileName = getPrefixFileName(fs.FaceName);
            switch (fs.Style)
            {
                case (FontStyle.Italic | FontStyle.Bold):
                    prefixFileName += "bi";
                    break;
                case (FontStyle.Bold):
                    prefixFileName += "bd";
                    break;
                case (FontStyle.Italic):
                    prefixFileName += "i";
                    break;

            }

            return prefixFileName;
        }

      

        //normalized names
        private string getFaceName(string face)
        {
            string faceName = face;
            switch (face.ToLowerInvariant())
            {
                case "times new roman":
                    faceName = "times";
                    break;
                case "courier new":
                    faceName = "courier";
                    break;
            }
            return faceName;
        }

        private string getPrefixFileName(string faceName)
        {
            string prefixFileName = faceName;
            //exceptions
            switch (faceName.ToLowerInvariant())
            {
                case "courier":
                    prefixFileName = "cour";
                    break;
            }
            return prefixFileName;
        }

        #endregion
    }
}
