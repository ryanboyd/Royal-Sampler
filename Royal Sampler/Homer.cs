using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace royalsampler
{

    /// <summary>
    /// Homer does all of the heavy lifting. He looks into the CSV files, draws random rows, etc. etc.
    /// </summary>
    class Homer
    {

        /// <summary>
        /// This is an object that contains all of the file input details, including things like file encoding and delimiter characters.
        /// </summary>
        private FileDetails fileDetails;
        public int numberOfSamples { get; set; }
        public int rowsPerSample { get; set; }
        public bool allowReplacement { get; set; }


        /// <summary>
        /// This is used to initialize/set up your FileDetails object — i.e., your "deck of cards"
        /// </summary>
        public void ArrangeDeck(string fileIn, bool allowRepl, bool containsHead, Encoding fEncode, char quotechar, char delimchar)
        {
            fileDetails = new FileDetails(fileIn, containsHead, fEncode, quotechar, delimchar);
        }


        /// <summary>
        /// Counts the number of rows within the CSV file.
        /// </summary>
        public FileDetails CountRows()
        {


            this.fileDetails.totalNumberOfRows = 0;
            this.fileDetails.rowErrorCount = 0;

            using (var stream = File.OpenRead(this.fileDetails.inputFileLocation))
            using (var reader = new StreamReader(stream, encoding: this.fileDetails.fileEncoding))
            {
                if (fileDetails.containsHeader)
                {
                    var csvDat = CsvParser.ParseHeadAndTail(reader, fileDetails.delimiter, fileDetails.quote);
                    try
                    {
                        foreach (var line in csvDat.Item2) { this.fileDetails.totalNumberOfRows++; } 
                    }
                    catch
                    {
                        MessageBox.Show("There was an error parsing your CSV file.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var csvDat = CsvParser.Parse(reader, fileDetails.delimiter, fileDetails.quote);
                    try
                    {
                        foreach (var line in csvDat) { this.fileDetails.totalNumberOfRows++; }
                    }
                    catch
                    {
                        MessageBox.Show("There was an error parsing your CSV file.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            return this.fileDetails;


        }




        /// <summary>
        /// Retrieve the already-determined number of rows.
        /// </summary>
        public int GetRowCount()
        {
            return this.fileDetails.totalNumberOfRows;
        }

        public void SetRowCount(int nrow, int nerr)
        {
            this.fileDetails.totalNumberOfRows = nrow;
            this.fileDetails.rowErrorCount = nerr;
        }

        public void SetOutputFolder(string folderOut)
        {
            this.fileDetails.outputFolder = folderOut;
        }

        public string GetOutputFolder()
        {
            return this.fileDetails.outputFolder;
        }
        public string GetInputFile()
        {
            return this.fileDetails.inputFileLocation;
        }

        public Encoding GetEncoding()
        {
            return this.fileDetails.fileEncoding;
        }

        public bool HasHeader()
        {
            return this.fileDetails.containsHeader;
        }
        public char GetDelim()
        {
            return this.fileDetails.delimiter;
        }

        public char GetQuote()
        {
            return this.fileDetails.quote;
        }


    }


    public class FileDetails
    {
        internal string inputFileLocation { get; set; }
        internal string outputFolder { get; set; }
        internal int totalNumberOfRows { get; set; }
        internal int rowErrorCount { get; set; }
        internal char delimiter { get; set; }
        internal char quote { get; set; }

        internal Encoding fileEncoding { get; set; }
        /// <summary>
        /// This can be referred to in order to figure out which rows have already been sampled. Only useful for if we're not allowing replacement.
        /// </summary>
        /// 
        
        internal bool containsHeader { get; set; }
        

        internal FileDetails(string fileIn, bool containsHead, Encoding fEncode, char quotechar, char delimchar)
        {
            inputFileLocation = fileIn;
            containsHeader = containsHead;
            fileEncoding = fEncode;
            quote = quotechar;
            delimiter = delimchar;

        }
    }



}
