using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace royalsampler
{
    public partial class RoyalSamplerForm
    {




        private void backgroundWorker_TargetedSubsampling(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Homer homer = (Homer)e.Argument;

            string quoteString = homer.GetQuote().ToString();
            string escapedQuoteString = homer.GetQuote().ToString() + homer.GetQuote().ToString();
            int numCols = homer.retainedIndices.Count;







            #region Get Busy Writin' or Get Busy Dyin'
            try
            {
                using (FileStream fileStreamOut = new FileStream(homer.GetOutputLocation(), FileMode.Create, FileAccess.Write, FileShare.None))
                using (StreamWriter streamWriter = new StreamWriter(fileStreamOut, homer.GetEncoding()))
                {

                    if (homer.HasHeader())
                    {
                        string[] headerRow;

                        using (var fileStreamIn = new FileStream(homer.GetInputFile(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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

                                if (rowNumber % 1000 == 0)
                                {
                                    if ((sender as BackgroundWorker).CancellationPending)
                                    {
                                        e.Result = "Cancelled";
                                        break;
                                    }

                                    //report progress
                                    //MessageBox.Show((((double)sampleNumber / homer.numberOfSamples) * 100).ToString());
                                    int pctDone = (int)Math.Round((((double)rowNumber / homer.endRow) * 10000), 0, MidpointRounding.AwayFromZero);
                                    (sender as BackgroundWorker).ReportProgress(pctDone);

                                }


                                if (rowNumber >= homer.startRow && rowNumber <= homer.endRow)
                                {
                                    rowToWriteString = RowCleaner.CleanRow(line.ToArray<string>(), homer.GetDelim(), quoteString, escapedQuoteString, numCols, hoju.retainedIndices);
                                    streamWriter.Write(rowToWriteString);
                                }

                                if (rowNumber == homer.endRow) break;

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
                            string rowToWriteString;

                            foreach (var line in csvDat)
                            {
                                rowNumber++;

                                if (rowNumber % 1000 == 0)
                                {
                                    if ((sender as BackgroundWorker).CancellationPending)
                                    {
                                        e.Result = "Cancelled";
                                        break;
                                    }

                                    //report progress
                                    //MessageBox.Show((((double)sampleNumber / homer.numberOfSamples) * 100).ToString());
                                    int pctDone = (int)Math.Round((((double)rowNumber / homer.endRow) * 10000), 0, MidpointRounding.AwayFromZero);
                                    (sender as BackgroundWorker).ReportProgress(pctDone);
                                }


                                if (rowNumber >= homer.startRow && rowNumber <= homer.endRow)
                                {
                                    rowToWriteString = RowCleaner.CleanRow(line.ToArray<string>(), homer.GetDelim(), quoteString, escapedQuoteString, numCols, hoju.retainedIndices);
                                    streamWriter.Write(rowToWriteString);
                                }

                                if (rowNumber == homer.endRow) break;
                            }
                        }
                    }

                }
                #endregion

            }
            catch
            {
                MessageBox.Show(genericProcessingError, "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Result = "Cancelled";
                return;
            }


            return;
        }


    }


}

