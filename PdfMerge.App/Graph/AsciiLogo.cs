using System;
using System.IO;
using Figgle;

namespace PdfMerge.App.Graph
{
    internal static class AsciiLogo
    {
        public static void WriteLogo(TextWriter @out) => @out.WriteLine(FiggleFonts.Standard.Render("pdfmerge"));
    }
}
