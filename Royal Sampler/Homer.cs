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
        private FileDetails deckOfCards;
        public int numberOfSamples { get; set; }
        public int rowsPerSample { get; set; }
        private bool allowReplacement { get; set; }


        /// <summary>
        /// This is used to initialize/set up your FileDetails object — i.e., your "deck of cards"
        /// </summary>
        public void ArrangeDeck(string fileIn, bool allowRepl, bool containsHead, Encoding fEncode, char quotechar, char delimchar)
        {
            deckOfCards = new FileDetails(fileIn, containsHead, fEncode, quotechar, delimchar);
        }


        /// <summary>
        /// Counts the number of rows within the CSV file.
        /// </summary>
        public FileDetails CountCards()
        {


            this.deckOfCards.totalNumberOfRows = 0;
            this.deckOfCards.rowErrorCount = 0;

            using (var stream = File.OpenRead(this.deckOfCards.inputFileLocation))
            using (var reader = new StreamReader(stream, encoding: this.deckOfCards.fileEncoding))
            {
                if (deckOfCards.containsHeader)
                {
                    var csvDat = CsvParser.ParseHeadAndTail(reader, deckOfCards.delimiter, deckOfCards.quote);
                    try
                    {
                        foreach (var line in csvDat.Item2) { this.deckOfCards.totalNumberOfRows++; } 
                    }
                    catch
                    {
                        MessageBox.Show("There was an error parsing your CSV file.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var csvDat = CsvParser.Parse(reader, deckOfCards.delimiter, deckOfCards.quote);
                    try
                    {
                        foreach (var line in csvDat) { this.deckOfCards.totalNumberOfRows++; }
                    }
                    catch
                    {
                        MessageBox.Show("There was an error parsing your CSV file.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            return this.deckOfCards;


        }




        /// <summary>
        /// Retrieve the already-determined number of rows.
        /// </summary>
        public ulong GetCardCount()
        {
            return this.deckOfCards.totalNumberOfRows;
        }

        public void SetCardCount(ulong nrow, ulong nerr)
        {
            this.deckOfCards.totalNumberOfRows = nrow;
            this.deckOfCards.rowErrorCount = nerr;
        }

        public void SetOutputFolder(string folderOut)
        {
            this.deckOfCards.outputFolder = folderOut;
        }

        public string GetOutputFolder()
        {
            return this.deckOfCards.outputFolder;
        }
        public string GetInputFolder()
        {
            return this.deckOfCards.inputFileLocation;
        }

        public Encoding GetEncoding()
        {
            return this.deckOfCards.fileEncoding;
        }

        public bool HasHeader()
        {
            return this.deckOfCards.containsHeader;
        }
        public char GetDelim()
        {
            return this.deckOfCards.delimiter;
        }

        public char GetQuote()
        {
            return this.deckOfCards.quote;
        }


    }


    public class FileDetails
    {
        internal string inputFileLocation { get; set; }
        internal string outputFolder { get; set; }
        internal ulong totalNumberOfRows { get; set; }
        internal ulong rowErrorCount { get; set; }
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
