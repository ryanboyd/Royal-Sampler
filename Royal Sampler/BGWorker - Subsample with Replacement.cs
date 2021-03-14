using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace royalsampler
{
    public partial class RoyalSamplerForm
    {




        private void backgroundWorker_SubSampleWithReplacement(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Homer homer = (Homer)e.Argument;
            Random random = new Random();

            if (!String.IsNullOrEmpty(homer.randSeedString)) random = new Random(int.Parse(homer.randSeedString));

            string filenamePadding = "D" + homer.numberOfSamples.ToString().Length.ToString();
            string quoteString = homer.GetQuote().ToString();
            string escapedQuoteString = homer.GetQuote().ToString() + homer.GetQuote().ToString();
            int numCols = homer.retainedIndices.Count;

            for (ulong sampleNumber = 0; sampleNumber < homer.numberOfSamples; sampleNumber++)
            {

                if ((sender as BackgroundWorker).CancellationPending)
                {
                    e.Result = "Cancelled";
                    break;
                }

                //report progress
                //MessageBox.Show((((double)sampleNumber / homer.numberOfSamples) * 100).ToString());
                int pctDone = (int)Math.Round((((double)sampleNumber / homer.numberOfSamples) * 10000), 0, MidpointRounding.AwayFromZero);
                (sender as BackgroundWorker).ReportProgress(pctDone);


                Dictionary<ulong, int> rowsToSample = new Dictionary<ulong, int>();
                
                #region Determine Our Samples Needed
           
                    
                ulong rowsSampledCount = 0;

                while (rowsSampledCount < homer.rowsPerSample)
                {
                    ulong randomDraw = random.NextLong(1, homer.GetRowCount());

                    if (rowsToSample.ContainsKey(randomDraw))
                    {
                        rowsToSample[randomDraw]++;
                    }
                    else
                    {
                        rowsToSample.Add(randomDraw, 1);
                    }

                    rowsSampledCount++;

                }

                #endregion




                #region Get Busy Writin' or Get Busy Dyin'
                ulong rowsWritten = 0;

                //first we need to open up our output filename
                string filenameOut;
                if (String.IsNullOrEmpty(homer.randSeedString))
                {
                    filenameOut = Path.Combine(homer.GetOutputLocation(), "subsample" + (sampleNumber + 1).ToString(filenamePadding) + ".csv");
                }
                else
                {
                    filenameOut = Path.Combine(homer.GetOutputLocation(), homer.randSeedString + "_subsample" + (sampleNumber + 1).ToString(filenamePadding) + ".csv");
                }


                try
                {

                

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
                                string rowToWriteString = RowCleaner.CleanRow(headerRow, homer.GetDelim(), quoteString, escapedQuoteString, numCols, hoju.retainedIndices);

                                //write the header row
                                streamWriter.Write(rowToWriteString);
                            

                                ulong rowNumber = 0;

                                foreach (var line in csvDat.Item2)
                                {
                                    rowNumber++;
                                    if (rowsToSample.ContainsKey(rowNumber))
                                    {

                                        rowToWriteString = RowCleaner.CleanRow(line.ToArray<string>(), homer.GetDelim(), quoteString, escapedQuoteString, numCols, hoju.retainedIndices);
                                        for (int numDraws = 0; numDraws < rowsToSample[rowNumber]; numDraws++) streamWriter.Write(rowToWriteString);

                                        rowsWritten += (ulong)rowsToSample[rowNumber];

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

                                ulong rowNumber = 0;

                                foreach (var line in csvDat)
                                {
                                    rowNumber++;
                                    if (rowsToSample.ContainsKey(rowNumber))
                                    {

                                        string rowToWriteString = RowCleaner.CleanRow(line.ToArray<string>(), homer.GetDelim(), quoteString, escapedQuoteString, numCols, hoju.retainedIndices);
                                        for (int numDraws = 0; numDraws < rowsToSample[rowNumber]; numDraws++) streamWriter.Write(rowToWriteString);

                                        rowsWritten += (ulong)rowsToSample[rowNumber];

                                        if (rowsWritten == homer.rowsPerSample) break;

                                    }
                                }
                            }
                        }

                    }
                    #endregion
                }
                catch
                {
                    MessageBox.Show("There was an error in writing your output file(s). This often occurs when your output file is already open in another application.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Result = "Cancelled";
                    return;
                }

            }

            return;
        }


    }


}

