using System;
using System.IO;

namespace PdfMerge.App.Writers
{
    internal class DatedWriter: OutWriter
    {
        private readonly TextWriter _writer;

        public DatedWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public override void WriteLine(string status) => _writer.WriteLine($"[{DateTime.Now:T}] {status}");
    }
}