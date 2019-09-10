using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PdfMerge.App.Files
{
    public static class FileRooter
    {
        public static IEnumerable<string> RootFilesIfNeeded(string root, IEnumerable<string> files)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            if (files == null) throw new ArgumentNullException(nameof(files));

            foreach (var file in files)
            {
                if (file == null) yield break;
                if (Path.IsPathRooted(file)) yield return file;

                yield return Path.Combine(root, file);
            }
        }
    }
}
