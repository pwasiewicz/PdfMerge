using PdfMerge.App.Writers;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PdfMerge.App.Files
{
    internal class PdfMerger
    {

        private readonly ICollection<string> _inputs;
        private readonly string _output;

        public PdfMerger(IEnumerable<string> inputs, string output)
        {
            if (inputs == null) throw new ArgumentNullException(nameof(inputs));
            _inputs = inputs as ICollection<string> ?? inputs.ToList();
            _output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public bool Merge(OutWriter statusWriter)
        {
            if (statusWriter == null) throw new ArgumentNullException(nameof(statusWriter));

            var options = new ProgressBarOptions
            {
                ProgressCharacter = '─',
                ProgressBarOnBottom = true
            };

            using (var progressBar = new ProgressBar(_inputs.Count, "progress bar is on the bottom now", options))
            {
                using (var finalDocument = new PdfDocument())
                {
                    for (var inputIndex = 0; inputIndex < _inputs.Count; inputIndex++)
                    {
                        AdfPdfFromInput(progressBar, statusWriter, inputIndex, finalDocument);
                    }
                    return SaveFinalPdfWithStatus(statusWriter, finalDocument);
                }
            }

        }

        private bool SaveFinalPdfWithStatus(OutWriter statusWriter, PdfDocument finalDocument)
        {
            try
            {
                statusWriter.WriteLine($"Saving to {_output}...");
                return SaveFinalPdf(statusWriter, finalDocument);
            }
            finally
            {
                statusWriter.WriteLine($"Done.");
            }
        }

        private bool SaveFinalPdf(OutWriter statusWriter, PdfDocument finalDocument)
        {
            try
            {
                finalDocument.Save(_output);
                return true;
            }
            catch (Exception ex)
            {
                statusWriter.WriteLine($"[ERROR] Unable to save final file: {ex.Message}");
                return false;
            }
        }

        private void AdfPdfFromInput(ProgressBar progressBar, OutWriter statusWriter, int inputIndex, PdfDocument finalDocument)
        {
            var pdf = _inputs.ElementAt(inputIndex);
            progressBar.Tick($"Processing {inputIndex + 1}/{_inputs.Count}: {pdf}..");
            try
            {
                AddPagesFromPdf(progressBar, statusWriter, pdf, finalDocument);
            }
            catch (Exception ex)
            {
                statusWriter.WriteLine(
                    $"[ERROR] An error occured while merging: {ex.Message}. This pdf or some of its pages will be skipped.");
            }
        }

        private static void AddPagesFromPdf(ProgressBar progressBar, OutWriter statusWriter, string pdf, PdfDocument finalDocument)
        {
            using (var pdfDoc = PdfReader.Open(pdf, PdfDocumentOpenMode.Import))
                using (var childProgressBar = progressBar.Spawn(pdfDoc.PageCount, "Processing pages"))
            {
                for (var i = 0; i < pdfDoc.PageCount; i++)
                {
                    childProgressBar.Tick($"Processing page {i+1}/{pdfDoc.PageCount}");
                    AddPageFromPdf(statusWriter, finalDocument, pdfDoc, i);

                    Thread.Sleep(50);
                }
            }
        }

        private static void AddPageFromPdf(OutWriter statusWriter, PdfDocument finalDocument, PdfDocument pdfDoc, int pageIndex)
        {
            try
            {
                finalDocument.AddPage(pdfDoc.Pages[pageIndex]);
            }
            catch (Exception ex)
            {
                statusWriter.WriteLine(
                    $"[ERROR] An error occured while merging page {pageIndex + 1}: {ex.Message}. This page will be skipped.");
            }
        }
    }
}
