using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PdfMerge.App.Files
{
    internal class PdfFilesFetcher
    {
        public static IEnumerable<string> GetFiles(string directory)
        {
            var directoryInfo = new DirectoryInfo(directory);
            var fileInfoCollection = directoryInfo.GetFiles("*.pdf");
            return fileInfoCollection.Select(fileInfo => fileInfo.FullName);
        }
    }
}
