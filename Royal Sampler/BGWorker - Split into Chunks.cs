using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace royalsampler
{
    public partial class RoyalSamplerForm
    {




        private void backgroundWorker_SplitIntoChunks(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

            Homer homer = (Homer)e.Argument;
            Random random = new Random();

            if (!String.IsNullOrEmpty(homer.randSeedString)) random = new Random(int.Parse(homer.randSeedString));

            string filenamePadding = "D" + homer.numberOfSamples.ToString().Length.ToString();
            string quoteString = homer.GetQuote().ToString();
            string escapedQuoteString = homer.GetQuote().ToString() + homer.GetQuote().ToString();
            int numCols = homer.retainedIndices.Count;



            try
            {

            


                if (homer.HasHeader())
                {
                    using (var fileStreamIn = File.OpenRead(homer.GetInputFile()))
                    using (var streamReader = new StreamReader(fileStreamIn, encoding: homer.GetEncoding()))
                    {

                        var csvDat = CsvParser.ParseHeadAndTail(streamReader, homer.GetDelim(), homer.GetQuote());

                        string[] headerRow;
                        headerRow = csvDat.Item1.ToArray<string>();
                        string headerRowToWriteString = RowCleaner.CleanRow(headerRow, homer.GetDelim(), quoteString, escapedQuoteString, numCols, hoju.retainedIndices);

                        ulong sampleNumber = 0;
                        ulong rowsWritten = 0;
                        ulong rowsWrittenTotal = 0;

                        FileStream fileStreamOut = null;
                        StreamWriter streamWriter = null;

                        
                        foreach (var line in csvDat.Item2)
                        {

                            //open up a new file to write out
                            if (rowsWritten == 0)
                            {
                                string filenameOut = Path.Combine(homer.GetOutputLocation(), "subsample" + (sampleNumber + 1).ToString(filenamePadding) + ".csv");
                                fileStreamOut = new FileStream(filenameOut, FileMode.Create, FileAccess.Write, FileShare.None);
                                streamWriter = new StreamWriter(fileStreamOut, homer.GetEncoding());
                                streamWriter.Write(headerRowToWriteString);
                            }

                            string rowToWriteString = RowCleaner.CleanRow(line.ToArray<string>(), homer.GetDelim(), quoteString, escapedQuoteString, numCols, hoju.retainedIndices);
                            streamWriter.Write(rowToWriteString);

                            rowsWritten++;
                            rowsWrittenTotal++;

                            if (rowsWritten == homer.rowsPerSample)
                            {
                                rowsWritten = 0;
                                sampleNumber++;
                                streamWriter.Close();
                                streamWriter.Dispose();
                                fileStreamOut.Close();
                                fileStreamOut.Dispose();
                            };


                            if (rowsWrittenTotal % 1000 == 0)
                            {
                                int pctDone = (int)Math.Round((((double)rowsWrittenTotal / homer.GetRowCount()) * 10000), 0, MidpointRounding.AwayFromZero);
                                (sender as BackgroundWorker).ReportProgress(pctDone);
                            }

                           


                        }



                        //everything has been written, so now we just close up shop
                        try
                        {
                            streamWriter.Close();
                            streamWriter.Dispose();
                            fileStreamOut.Close();
                            fileStreamOut.Dispose();
                        }
                        catch
                        {

                        }








                    }
                }
                else
                {
                    using (var fileStreamIn = File.OpenRead(homer.GetInputFile()))
                    using (var streamReader = new StreamReader(fileStreamIn, encoding: homer.GetEncoding()))
                    {

                        var csvDat = CsvParser.Parse(streamReader, homer.GetDelim(), homer.GetQuote());

                        ulong sampleNumber = 0;
                        ulong rowsWritten = 0;
                        ulong rowsWrittenTotal = 0;

                        FileStream fileStreamOut = null;
                        StreamWriter streamWriter = null;

                        foreach (var line in csvDat)
                        {
                            
                            //open up a new file to write out
                            if (rowsWritten == 0)
                            {
                                string filenameOut = Path.Combine(homer.GetOutputLocation(), "subsample" + (sampleNumber + 1).ToString(filenamePadding) + ".csv");
                                fileStreamOut = new FileStream(filenameOut, FileMode.Create, FileAccess.Write, FileShare.None);
                                streamWriter = new StreamWriter(fileStreamOut, homer.GetEncoding());
                            }

                            string rowToWriteString = RowCleaner.CleanRow(line.ToArray<string>(), homer.GetDelim(), quoteString, escapedQuoteString, numCols, hoju.retainedIndices);
                            streamWriter.Write(rowToWriteString);

                            rowsWritten++;
                            rowsWrittenTotal++;

                            if (rowsWritten == homer.rowsPerSample)
                            {
                                rowsWritten = 0;
                                sampleNumber++;
                                streamWriter.Close();
                                streamWriter.Dispose();
                                fileStreamOut.Close();
                                fileStreamOut.Dispose();
                            };


                            if (rowsWrittenTotal % 1000 == 0)
                            {
                                int pctDone = (int)Math.Round((((double)rowsWrittenTotal / homer.GetRowCount()) * 10000), 0, MidpointRounding.AwayFromZero);
                                (sender as BackgroundWorker).ReportProgress(pctDone);
                            }
                        }


                        //everything has been written, so now we just close up shop
                        try
                        {
                            streamWriter.Close();
                            streamWriter.Dispose();
                            fileStreamOut.Close();
                            fileStreamOut.Dispose();
                        }
                        catch
                        {

                        }




                    }
                }


            }
            catch
            {
                MessageBox.Show("There was an error in writing your output file(s). This often occurs when your output file is already open in another application.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Result = "Cancelled";
                return;
            }


            return;


        }


    }


}

