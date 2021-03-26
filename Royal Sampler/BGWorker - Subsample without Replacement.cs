using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
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
            int pctDone = 0;


            ulong actualSamplesToBeWritten;

            if (homer.numberOfSamples * homer.rowsPerSample > homer.GetRowCount())
            {
                actualSamplesToBeWritten = (ulong)Math.Round((homer.GetRowCount() / (double)homer.rowsPerSample) * 100, 0, MidpointRounding.AwayFromZero);
            }
            else 
            {
                actualSamplesToBeWritten = homer.numberOfSamples;
            }




            HashSet<ulong> rowsToKeep;
            ulong[] rowsToSample = new ulong[homer.GetRowCount()];


            #region Randomize order of sample
            for (ulong i = 0; i < homer.GetRowCount(); i++) rowsToSample[i] = i + 1;
            rowsToSample = rowsToSample.OrderBy(x => random.NextLong()).ToArray<ulong>();
            #endregion



            //this is our outermost block within the bgworker: the timer that we use to report progress
            TimeSpan reportPeriod = TimeSpan.FromMinutes(0.01);
            using (new System.Threading.Timer(
                           _ => (sender as BackgroundWorker).ReportProgress(pctDone), null, reportPeriod, reportPeriod))
            {

                for (ulong sampleNumber = 0; sampleNumber < homer.numberOfSamples; sampleNumber++)
                {

                    if ((sender as BackgroundWorker).CancellationPending)
                    {
                        e.Result = "Cancelled";
                        break;
                    }

                    
                    ulong skipToVal = (sampleNumber * homer.rowsPerSample);
                    ulong takeVal = homer.rowsPerSample;

                    if (skipToVal > homer.GetRowCount()) break;

                    if (skipToVal + takeVal > (ulong)rowsToSample.Length) takeVal = (ulong)rowsToSample.Length - skipToVal;

                    ulong[] subsample = rowsToSample.Skip((int)skipToVal).Take((int)takeVal).ToArray();

                    rowsToKeep = subsample.ToHashSet<ulong>();


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

                                    //write the header row
                                    streamWriter.Write(RowCleaner.CleanRow(headerRow, homer.GetDelim(), quoteString, escapedQuoteString, numCols, hoju.retainedIndices));

                                    ulong rowNumber = 0;
                                    string rowToWriteString;

                                    foreach (var line in csvDat.Item2)
                                    {
                                        rowNumber++;
                                        //calculate how far long we are
                                        if (rowNumber % 1000 == 0)
                                        {
                                            pctDone = calcPctDone(rowsWritten, homer.rowsPerSample, sampleNumber, homer.numberOfSamples);
                                            if ((sender as BackgroundWorker).CancellationPending)
                                            {
                                                e.Result = "Cancelled";
                                                break;
                                            }
                                        }


                                        if (rowsToKeep.Contains(rowNumber))
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
                                    ulong rowNumber = 0;
                                    foreach (var line in csvDat)
                                    {
                                        rowNumber++;
                                        //calculate how far long we are
                                        if (rowNumber % 1000 == 0)
                                        {
                                            pctDone = calcPctDone(rowsWritten, homer.rowsPerSample, sampleNumber, homer.numberOfSamples);
                                            if ((sender as BackgroundWorker).CancellationPending)
                                            {
                                                e.Result = "Cancelled";
                                                break;
                                            }
                                        }

                                        if (rowsToKeep.Contains(rowNumber))
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
                    catch
                    {
                        MessageBox.Show(genericProcessingError, "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Result = "Cancelled";
                        return;
                    }

                }


            }

            



            return;
        }

    }
}

