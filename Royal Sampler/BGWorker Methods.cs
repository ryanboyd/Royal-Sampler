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
    public partial class RoyalSamplerForm : Form
    {


        #region Just for Counting Rows
        private void backgroundWorker_CountRows(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

            TimeSpan reportPeriod = TimeSpan.FromMinutes(0.01);
            using (new System.Threading.Timer(
                           _ => (sender as BackgroundWorker).ReportProgress(((Homer)e.Argument).GetCardCount()), null, reportPeriod, reportPeriod))
            {
                e.Result = ((Homer)e.Argument).CountCards();
            }

            
            return;
        }

        private void backgroundWorker_CountRowsProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            StatusLabel.Text = "Counting rows... " + ToKMB(e.ProgressPercentage) + " thus far...";
        }

        private void backgroundWorker_CountRowsRunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

            FileDetails fdet = (FileDetails)e.Result;

            hoju.SetCardCount(fdet.totalNumberOfRows, fdet.rowErrorCount);
            EnableControls();
            StartButton.Enabled = true;
            DisableProgBar();
            StatusLabel.Text = "Dataset contains " + ToKMB(hoju.GetCardCount()) + " rows";

        }
        #endregion



        private void backgroundWorker_SubSampleWithReplacement(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Homer homer = (Homer)e.Argument;
            Random random = new Random();
            string filenamePadding = "D" + homer.numberOfSamples.ToString().Length.ToString();
            string quoteString = homer.GetQuote().ToString();
            string escapedQuoteString = homer.GetQuote().ToString() + homer.GetQuote().ToString();


            for (int sampleNumber = 0; sampleNumber < homer.numberOfSamples; sampleNumber++)
            {

                if ((sender as BackgroundWorker).CancellationPending) break;

                //report progress
                //MessageBox.Show((((double)sampleNumber / homer.numberOfSamples) * 100).ToString());
                int pctDone = (int)Math.Round((((double)sampleNumber / homer.numberOfSamples) * 100), 0, MidpointRounding.AwayFromZero);
                (sender as BackgroundWorker).ReportProgress(pctDone);


                Dictionary<int, int> cardsToDraw = new Dictionary<int, int>();
                
                #region Determine Our Samples Needed
           
                    
                int cardsDrawnCount = 0;

                //decimal pctSample = ((ulong)homer.rowsPerSample / homer.GetCardCount()) * 100;
                //int drawLikelihood = (int)Math.Round(pctSample, 0, MidpointRounding.AwayFromZero);

                while (cardsDrawnCount < homer.rowsPerSample)
                {
                    int randomDraw = random.Next(1, homer.GetCardCount());

                    if (cardsToDraw.ContainsKey(randomDraw))
                    {
                        cardsToDraw[randomDraw]++;
                    }
                    else
                    {
                        cardsToDraw.Add(randomDraw, 1);
                    }

                    cardsDrawnCount++;

                }

                #endregion




                #region Get Busy Writin' or Get Busy Dyin'
                int rowsWritten = 0;

                //first we need to open up our output file
                string filenameOut = Path.Combine(homer.GetOutputFolder(), "subsample" + sampleNumber.ToString(filenamePadding) + ".csv");

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
                            string[] rowToWrite = new string[headerRow.Length];

                            //write the header row
                            streamWriter.Write(RowCleaner.CleanRow(headerRow, homer.GetDelim(), quoteString, escapedQuoteString));
                            streamWriter.Write(Environment.NewLine);


                            int rowNumber = 0;

                            foreach (var line in csvDat.Item2)
                            {
                                rowNumber++;
                                if (cardsToDraw.ContainsKey(rowNumber))
                                {

                                    string rowToWriteString = RowCleaner.CleanRow(line.ToArray<string>(), homer.GetDelim(), quoteString, escapedQuoteString) + Environment.NewLine;
                                    for (int numDraws = 0; numDraws < cardsToDraw[rowNumber]; numDraws++) streamWriter.Write(rowToWriteString);

                                    rowsWritten += cardsToDraw[rowNumber];

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
                                if (cardsToDraw.ContainsKey(rowNumber))
                                {

                                    string rowToWriteString = RowCleaner.CleanRow(line.ToArray<string>(), homer.GetDelim(), quoteString, escapedQuoteString) + Environment.NewLine;
                                    for (int numDraws = 0; numDraws < cardsToDraw[rowNumber]; numDraws++) streamWriter.Write(rowToWriteString);

                                    rowsWritten += cardsToDraw[rowNumber];

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


        private void backgroundWorker_SubSampleWithoutReplacement(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Homer homer = (Homer)e.Argument;
            Random random = new Random();
            string filenamePadding = "D" + homer.numberOfSamples.ToString().Length.ToString();
            string quoteString = homer.GetQuote().ToString();
            string escapedQuoteString = homer.GetQuote().ToString() + homer.GetQuote().ToString();


            int actualSamplesToBeWritten;

            if (homer.numberOfSamples * homer.rowsPerSample > homer.GetCardCount())
            {
                actualSamplesToBeWritten = (int)Math.Round((homer.GetCardCount() / (double)homer.rowsPerSample) * 100, 0, MidpointRounding.AwayFromZero);
            }
            else 
            {
                actualSamplesToBeWritten = homer.numberOfSamples;
            }




            HashSet<int> cardsToDraw;
            int[] rowsToSample = new int[homer.GetCardCount()];


            #region Randomize order of sample
            for (int i = 0; i < homer.GetCardCount(); i++) rowsToSample[i] = i + 1;
            rowsToSample = rowsToSample.OrderBy(x => random.Next()).ToArray<int>();
            #endregion





            for (int sampleNumber = 0; sampleNumber < homer.numberOfSamples; sampleNumber++)
            {

                if ((sender as BackgroundWorker).CancellationPending) break;

                //report progress
                int pctDone = (int)Math.Round((((double)sampleNumber / actualSamplesToBeWritten) * 100), 0, MidpointRounding.AwayFromZero);
                (sender as BackgroundWorker).ReportProgress(pctDone);


                int skipToVal = (sampleNumber * homer.rowsPerSample);
                int takeVal = homer.rowsPerSample;

                if (skipToVal > homer.GetCardCount()) break;

                if (skipToVal + takeVal > rowsToSample.Length) takeVal = rowsToSample.Length - skipToVal;

                int[] subsample = rowsToSample.Skip(skipToVal).Take(takeVal).ToArray();

                cardsToDraw = subsample.ToHashSet<int>();


                #region Get Busy Writin' or Get Busy Dyin'

                int rowsWritten = 0;

                //first we need to open up our output file
                string filenameOut = Path.Combine(homer.GetOutputFolder(), "subsample" + sampleNumber.ToString(filenamePadding) + ".csv");

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
                            string[] rowToWrite = new string[headerRow.Length];

                            //write the header row
                            streamWriter.Write(RowCleaner.CleanRow(headerRow, homer.GetDelim(), quoteString, escapedQuoteString));
                            streamWriter.Write(Environment.NewLine);


                            int rowNumber = 0;

                            foreach (var line in csvDat.Item2)
                            {
                                rowNumber++;
                                if (cardsToDraw.Contains(rowNumber))
                                {
                                    string rowToWriteString = RowCleaner.CleanRow(line.ToArray<string>(), homer.GetDelim(), quoteString, escapedQuoteString) + Environment.NewLine;
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

                                    string rowToWriteString = RowCleaner.CleanRow(line.ToArray<string>(), homer.GetDelim(), quoteString, escapedQuoteString) + Environment.NewLine;
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


        private void backgroundWorker_SubSampleProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            MainProgressBar.Value = e.ProgressPercentage;
            StatusLabel.Text = "Writing subsamples... " + e.ProgressPercentage.ToString() + "% complete...";
        }

        private void backgroundWorker_SubSampleRunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

            MainProgressBar.Value = 0;
            EnableControls();
            DisableProgBar();
            ChangeCancelToStartButton();
            StartButton.Enabled = true;
            //messagebox that it's done
            StatusLabel.Text = "Finished!";

        }



















        private string ToKMB(int num)
        {
            if (num > 999999999)// || num < -999999999)
            {
                return num.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999999)// || num < -999999)
            {
                return num.ToString("0,,.##M", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999)// || num < -999)
            {
                return num.ToString("0,.#K", CultureInfo.InvariantCulture);
            }
            else
            {
                return num.ToString(CultureInfo.InvariantCulture);
            }
        }



    }


    public static class RandGenerator
    {

        public static ulong GenRandUlong(ulong min, ulong max)
        {
            Random random = new Random();
            //Working with ulong so that modulo works correctly with values > long.MaxValue
            ulong uRange = (ulong)(max - min);

            //Prevent a modolo bias; see https://stackoverflow.com/a/10984975/238419
            //for more information.
            //In the worst case, the expected number of calls is 2 (though usually it's
            //much closer to 1) so this loop doesn't really hurt performance at all.
            ulong ulongRand;
            do
            {
                byte[] buf = new byte[8];
                random.NextBytes(buf);
                ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
            } while (ulongRand > ulong.MaxValue - ((ulong.MaxValue % uRange) + 1) % uRange);

            return (ulong)(ulongRand % uRange) + min;
        }
    }


    public static class RowCleaner
    {
        public static string CleanRow(string[] rowIn, char delim, string quote, string escQuote)
        {

        
            string[] rowOut = new string[rowIn.Length];


            for (int i = 0; i < rowIn.Length; i++)
            {
                rowOut[i] = rowIn[i];
                if (rowOut[i].Contains(quote)) rowOut[i] = rowOut[i].Replace(quote, escQuote);
                if (rowOut[i].Contains(delim)) rowOut[i] = quote + rowOut[i] + quote;
            }


            string cleanedRow = String.Join(delim, rowOut);

            return cleanedRow;
        }
    }


}

