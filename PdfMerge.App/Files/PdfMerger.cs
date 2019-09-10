using PdfMerge.App.Writers;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Linq;

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
            statusWriter.WriteLine("Merging process started.");
            using (var finalDocument = new PdfDocument())
            {
                for (var inputIndex = 0; inputIndex < _inputs.Count; inputIndex++)
                {
                    AdfPdfFromInput(statusWriter, inputIndex, finalDocument);
                }
                return SaveFinalPdfWithStatus(statusWriter, finalDocument);
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

        private void AdfPdfFromInput(OutWriter statusWriter, int inputIndex, PdfDocument finalDocument)
        {
            var pdf = _inputs.ElementAt(inputIndex);
            statusWriter.WriteLine($"{inputIndex + 1}/{_inputs.Count} Handling file {pdf}...");
            try
            {
                AddPagesFromPdf(statusWriter, pdf, finalDocument);
            }
            catch (Exception ex)
            {
                statusWriter.WriteLine(
                    $"[ERROR] An error occured while merging: {ex.Message}. This pdf or some of its pages will be skipped.");
            }
        }

        private static void AddPagesFromPdf(OutWriter statusWriter, string pdf, PdfDocument finalDocument)
        {
            using (var pdfDoc = PdfReader.Open(pdf, PdfDocumentOpenMode.Import))
            {
                for (var i = 0; i < pdfDoc.PageCount; i++)
                {
                    AddPageFromPdf(statusWriter, finalDocument, pdfDoc, i);
                }
                statusWriter.WriteLine($"Added {pdfDoc.PageCount} page(s) from {pdf}.");
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
