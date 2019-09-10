# PdfMerge
A tool that merges multiple PDF files into one. 

## Usage
Run the following command an inside directory that contains PDF files you want to merge:
```PS
pdfmerge output.pdf
```

You can also specify files manually:
```PS
pdfmerge output.pdf --files file1.pdf file2.pdf
```

The absolute file path works as well:
```PS
pdfmerge output.pdf --files C:\file1.pdf C:\file2.pdf
```

You can override workinf directory if you want:
```PS
pdfmerge output.pdf --path c:\ --files file1.pdf file2.pdf
```
Then, the files C:\file1.pdf and C:\file2.psd will be saved to C:\output.pdf regardless of the working directory.

## Program arguments
```PS
   --Output, -o  File name of the output, merged file.
   --Path, -p   (optional) The directory path to get files from. Default value is current working working directory.
   --Files, -f  (optional) The collection (separated by a space) of input pdf files. When only file names used, the directory for path or working directory as used as base.
```
