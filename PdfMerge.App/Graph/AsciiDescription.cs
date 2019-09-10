using System.IO;

namespace PdfMerge.App.Graph
{
    internal static class AsciiDescription
    {
        public static void WriteDescription(TextWriter @out)
        {
            @out.WriteLine("A tool that merges multiple pdf files into one.");
            @out.WriteLine();
        }
    }
}
