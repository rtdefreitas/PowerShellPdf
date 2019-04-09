using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text;

namespace utility.PDF
{
    public class PdfDocument
    {
        protected string FileLocation { get; set; }

        public PdfDocument(string fileLocation)
        {
            this.FileLocation = fileLocation;
        }

        public void SplitByRegex(string regx, string destinationDirectory)
        {
            try
            {
                PdfReader reader = new PdfReader(this.FileLocation);
                Regex regex = new Regex(regx);
                Document document = null;
                PdfCopy copy = null;

                int n = reader.NumberOfPages;
                int recordCount = 1;

                for (int i = 1; i <= n; i++)
                {
                    string text = PdfTextExtractor.GetTextFromPage(reader, i);
                    if (regex.IsMatch(text))
                    {
                        if (document != null && document.IsOpen())
                        {
                            document.Close();
                            recordCount++;
                        }
                        string filename = destinationDirectory + "\\record_" + recordCount.ToString() + ".pdf";
                        document = new Document();
                        copy = new PdfCopy(document, new FileStream(filename, FileMode.Create));
                        document.Open();
                        copy.AddPage(copy.GetImportedPage(reader, i));
                    }
                    else if (document != null && document.IsOpen())
                    {
                        copy.AddPage(copy.GetImportedPage(reader, i));
                    }
                }
                document.Close();
                reader.Close();
                copy.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void CombinePDF(string PdfFirectory, bool DeleteOriginals)
        {
            PdfReader reader = null;
            Document document = null;
            PdfCopy copy = null;
            string OutputFile = this.FileLocation;
            //Get all the PDF pdfFiles in the current directory
            string[] pdfFiles = Directory.GetFiles(PdfFirectory, "*.pdf", SearchOption.TopDirectoryOnly);

            string filename = OutputFile;
            document = new Document();
            copy = new PdfCopy(document, new FileStream(filename, FileMode.Create));
            document.Open();

            for (int f = 0; f < pdfFiles.Length; f++)
            {
                if (pdfFiles.Length > 0)
                {
                    if (pdfFiles[f] != filename)
                    {
                        reader = new PdfReader(pdfFiles[f]);
                        for (int p = 1; p < reader.NumberOfPages + 1; p++)
                        {
                            copy.AddPage(copy.GetImportedPage(reader, p));
                        }
                        reader.Close();
                    }

                }

                if (pdfFiles[f] != filename)
                {
                    if (DeleteOriginals)
                    {
                        File.Delete(pdfFiles[f]);
                    }    
                }
            }
            try
            {
                document.Close();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public string PageToRawText(int PageNumber)
        {
            PdfReader reader = new PdfReader(this.FileLocation);
            string rawText = PdfTextExtractor.GetTextFromPage(reader, PageNumber);
            reader.Close();
            return rawText;
        }

        public string FindMatch(string Regex)
        {
            PdfReader reader = new PdfReader(this.FileLocation);
            Regex regex = new Regex(Regex);
            string result = "Match not Found";

            int n = reader.NumberOfPages;

            for (int i = 1; i <= n; i++)
            {
                string text = PdfTextExtractor.GetTextFromPage(reader, i);
                if (regex.IsMatch(text))
                {
                    Match m = regex.Match(text);
                    result = m.Value.ToString();
                }
                if (result != "Match not Found") { break; }
            }
            reader.Close();
            return result;
        }


    }
}
