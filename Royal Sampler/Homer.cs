
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
        public ulong numberOfSamples { get; set; }
        public ulong rowsPerSample { get; set; }
        public ulong startRow { get; set; }
        public ulong endRow { get; set; }
        public bool allowReplacement { get; set; }
        public string randSeedString { get; set; }
        public HashSet<int> retainedIndices { get; set; }


        /// <summary>
        /// This is used to initialize/set up your FileDetails object — i.e., your "deck of cards"
        /// </summary>
        public void InitializeFileDetails(string fileIn, bool allowRepl, bool containsHead, Encoding fEncode, char quotechar, char delimchar)
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

            using (var stream = new FileStream(this.GetInputFile(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream, encoding: this.fileDetails.fileEncoding))
            {
                if (fileDetails.containsHeader)
                {
                    var csvDat = CsvParser.ParseHeadAndTail(reader, fileDetails.delimiter, fileDetails.quote);

                    fileDetails.colNames = csvDat.Item1.ToList<string>();

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

                    int numCols = 0;

                    var csvDat = CsvParser.Parse(reader, fileDetails.delimiter, fileDetails.quote);
                    try
                    {
                        foreach (var line in csvDat) 
                        { 
                            this.fileDetails.totalNumberOfRows++;
                            int numColsOnLine = line.Count;

                            if (numColsOnLine > numCols) numCols = numColsOnLine;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("There was an error parsing your CSV file.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    List<string> colNames = new List<string>();
                    for (int colNum = 0; colNum < numCols; colNum++) colNames.Add("V" + colNum.ToString());

                    this.fileDetails.colNames = colNames;

                }
            }

            return this.fileDetails;


        }




        /// <summary>
        /// Retrieve the already-determined number of rows.
        /// </summary>
        public ulong GetRowCount()
        {
            return this.fileDetails.totalNumberOfRows;
        }

        public void SetRowCount(ulong nrow, int nerr)
        {
            this.fileDetails.totalNumberOfRows = nrow;
            this.fileDetails.rowErrorCount = nerr;
        }

        public void SetOutputFolder(string folderOut)
        {
            this.fileDetails.outputFolder = folderOut;
        }

        public string GetOutputLocation()
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

        public void SetColNames(List<string> colNames)
        {
            this.fileDetails.colNames = colNames;
        }

        public List<string> GetColNames()
        {
            return this.fileDetails.colNames;
        }


    }


    public class FileDetails
    {
        internal string inputFileLocation { get; set; }
        internal string outputFolder { get; set; }
        internal ulong totalNumberOfRows { get; set; }
        internal int rowErrorCount { get; set; }
        internal char delimiter { get; set; }
        internal char quote { get; set; }
        internal List<string> colNames { get; set; }
        internal Encoding fileEncoding { get; set; }
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
