using System.Collections.Generic;
using NCmdArgs.Attributes;

namespace PdfMerge.App
{
    public class ProgramOptions
    {
        [CommandArgument(Description = "File name of the output, merged file.", ShortName = "o", Required = true)]
        [CommandInlineArgument(0)]
        public string Output { get; set; }

        [CommandArgument(Description = "The directory path to get files from. Default value is current working working directory.", ShortName = "p")]
        public string Path { get; set; }

        [CommandArgument(Description = "The collection (separated by a space) of input pdf files. When only file names used, the directory for path or working directory as used as base.", ShortName = "f")]
        public IEnumerable<string> Files { get; set; }


        public string GetNormalizedOutput(string contextPath)
        {
            var output = Output.EndsWith(".pdf") ? Output : $"{Output}.pdf";

            if (!System.IO.Path.IsPathRooted(output))
            {
                output = System.IO.Path.Combine(contextPath, output);
            }

            return output;
        }
    }
}
