using NCmdArgs;
using PdfMerge.App.Files;
using PdfMerge.App.Graph;
using PdfMerge.App.Writers;
using System;
using System.Collections.Generic;
using System.IO;

namespace PdfMerge.App
{
    internal class Program
    {
        private static readonly TextWriter DefaultOut = Console.Out;
        private static readonly TextReader DefaultIn = Console.In;

        private static int Main(string[] args)
        {
            AsciiLogo.WriteLogo(DefaultOut);
            AsciiDescription.WriteDescription(DefaultOut);

            if (!BuildOptions(args, out var options)) return ProgramExitCodes.InvalidArgs;

            var workingDirectory = DetermineWorkingDirectory(options);
            if (!Directory.Exists(workingDirectory))
            {
                DefaultOut.WriteLine($"[ERROR] The path {workingDirectory} does not exist.");
                return ProgramExitCodes.InvalidWorkingDirectory;
            }

            var outputFile = options.GetNormalizedOutput(workingDirectory);



            var merger =
                new PdfMerger(
                    FileRooter.RootFilesIfNeeded(workingDirectory, DetermineFilesToMerge(options, workingDirectory)),
                    outputFile)
                {
                    AskForOverride = path =>
                    {
                        DefaultOut.WriteLine($"File already '{path}' exists. Do you want to to override? [y]es or [n]o?");
                        var result = Console.ReadKey();
                        return result.KeyChar == 'y' || result.KeyChar == 'Y';
                    }
                };

            return !merger.Merge(new DatedWriter(Console.Out))
                ? ProgramExitCodes.ErrorSavingFinalPdf
                : ProgramExitCodes.Ok;
        }

        private static IEnumerable<string> DetermineFilesToMerge(ProgramOptions options, string workingDirectory) =>
            options.Files ?? PdfFilesFetcher.GetFiles(workingDirectory);

        private static string DetermineWorkingDirectory(ProgramOptions options) =>
            options.Path ?? Directory.GetCurrentDirectory();

        private static bool BuildOptions(string[] args, out ProgramOptions options)
        {
            var parser = new CommandLineParser();

            try
            {
                options = parser.Parse<ProgramOptions>(args);
            }
            catch
            {
                parser.Usage<ProgramOptions>(DefaultOut);
                options = null;
                return false;
            }

            return true;
        }
    }
}