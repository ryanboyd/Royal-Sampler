using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;
using System.IO;

namespace royalsampler
{
    public partial class RoyalSamplerForm
    {


        private void backgroundWorker_SubSampleWithoutReplacement(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Homer homer = (Homer)e.Argument;
            Random random = new Random();

            if (!String.IsNullOrEmpty(homer.randSeedString)) random = new Random(int.Parse(homer.randSeedString));

            string filenamePadding = "D" + homer.numberOfSamples.ToString().Length.ToString();
            string quoteString = homer.GetQuote().ToString();
            string escapedQuoteString = homer.GetQuote().ToString() + homer.GetQuote().ToString();
            int numCols = homer.retainedIndices.Count;


            int actualSamplesToBeWritten;

            if (homer.numberOfSamples * homer.rowsPerSample > homer.GetRowCount())
            {
                actualSamplesToBeWritten = (int)Math.Round((homer.GetRowCount() / (double)homer.rowsPerSample) * 100, 0, MidpointRounding.AwayFromZero);
            }
            else 
            {
                actualSamplesToBeWritten = homer.numberOfSamples;
            }




            HashSet<int> cardsToDraw;
            int[] rowsToSample = new int[homer.GetRowCount()];


            #region Randomize order of sample
            for (int i = 0; i < homer.GetRowCount(); i++) rowsToSample[i] = i + 1;
            rowsToSample = rowsToSample.OrderBy(x => random.Next()).ToArray<int>();
            #endregion





            for (int sampleNumber = 0; sampleNumber < homer.numberOfSamples; sampleNumber++)
            {

                if ((sender as BackgroundWorker).CancellationPending) break;

                //report progress
                int pctDone = (int)Math.Round((((double)sampleNumber / actualSamplesToBeWritten) * 10000), 0, MidpointRounding.AwayFromZero);
                (sender as BackgroundWorker).ReportProgress(pctDone);


                int skipToVal = (sampleNumber * homer.rowsPerSample);
                int takeVal = homer.rowsPerSample;

                if (skipToVal > homer.GetRowCount()) break;

                if (skipToVal + takeVal > rowsToSample.Length) takeVal = rowsToSample.Length - skipToVal;

                int[] subsample = rowsToSample.Skip(skipToVal).Take(takeVal).ToArray();

                cardsToDraw = subsample.ToHashSet<int>();


                #region Get Busy Writin' or Get Busy Dyin'

                int rowsWritten = 0;

                //first we need to open up our output filename
                string filenameOut;
                if (String.IsNullOrEmpty(homer.randSeedString))
                {
                    filenameOut = Path.Combine(homer.GetOutputFolder(), "subsample" + (sampleNumber + 1).ToString(filenamePadding) + ".csv");
                }
                else
                {
                    filenameOut = Path.Combine(homer.GetOutputFolder(), homer.randSeedString + "_subsample" + (sampleNumber + 1).ToString(filenamePadding) + ".csv");
                }



                using (FileStream fileStreamOut = new FileStream(filenameOut, FileMode.Create, FileAccess.Write, FileShare.None))
                using (StreamWriter streamWriter = new StreamWriter(fileStreamOut, homer.GetEncoding()))
                {
                    if (homer.HasHeader())
                    {
                        string[] headerRow;

                        using (var fileStreamIn = File.OpenRead(homer.GetInputFile()))
                        using (var streamReader = new StreamReader(fileStreamIn, encoding: homer.GetEncoding()))
                        {
                            var csvDat = CsvParser.ParseHeadAndTail(streamReader, homer.GetDelim(), homer.GetQuote());

                            headerRow = csvDat.Item1.ToArray<string>();

                            //write the header row
                            streamWriter.Write(RowCleaner.CleanRow(headerRow, homer.GetDelim(), quoteString, escapedQuoteString, numCols, hoju.retainedIndices));

                            int rowNumber = 0;
                            string rowToWriteString;

                            foreach (var line in csvDat.Item2)
                            {
                                rowNumber++;
                                if (cardsToDraw.Contains(rowNumber))
                                {
                                    rowToWriteString = RowCleaner.CleanRow(line.ToArray<string>(), homer.GetDelim(), quoteString, escapedQuoteString, numCols, hoju.retainedIndices);
                                    streamWriter.Write(rowToWriteString);

                                    rowsWritten++;

                                    if (rowsWritten == homer.rowsPerSample) break;

                                }
                            }
                        }
                   }
                    else
                    {

                        using (var fileStreamIn = File.OpenRead(homer.GetInputFile()))
                        using (var streamReader = new StreamReader(fileStreamIn, encoding: homer.GetEncoding()))
                        {

                            var csvDat = CsvParser.Parse(streamReader, homer.GetDelim(), homer.GetQuote());
                            int rowNumber = 0;
                            foreach (var line in csvDat)
                            {
                                rowNumber++;
                                if (cardsToDraw.Contains(rowNumber))
                                {

                                    string rowToWriteString = RowCleaner.CleanRow(line.ToArray<string>(), homer.GetDelim(), quoteString, escapedQuoteString, numCols, hoju.retainedIndices);
                                    streamWriter.Write(rowToWriteString);

                                    rowsWritten++;

                                    if (rowsWritten == homer.rowsPerSample) break;

                                }
                            }


                        }
                    }
                }
                #endregion


            }



            return;
        }

    }
}

