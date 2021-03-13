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

        private void backgroundWorker_CountRows(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            e.Result = ((Homer)e.Argument).CountCards();
            return;
        }

        private void backgroundWorker_CountRowsProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            //progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_CountRowsRunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

            FileDetails fdet = (FileDetails)e.Result;

            hoju.SetCardCount(fdet.totalNumberOfRows, fdet.rowErrorCount);
            EnableControls();

            NumRowsLabel.Text = "Dataset contains " + ToKMB(hoju.GetCardCount()) + " rows";

        }





        private void backgroundWorker_SubSample(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Homer homer = (Homer)e.Argument;
            Random random = new Random();
            string filenamePadding = "D" + homer.numberOfSamples.ToString().Length.ToString();
            string quoteString = homer.GetQuote().ToString();
            string escapedQuoteString = homer.GetQuote().ToString() + homer.GetQuote().ToString();


            for (int sampleNumber = 0; sampleNumber < homer.numberOfSamples; sampleNumber++)
            {


                //report progress
                (sender as BackgroundWorker).ReportProgress((int)Math.Round( (double)(sampleNumber / homer.numberOfSamples) * 100, 0, MidpointRounding.AwayFromZero));


                

                Dictionary<ulong, int> cardsToDraw = new Dictionary<ulong, int>();
                
                #region Determine Our Samples Needed
                if (homer.allowReplacement)
                {
                    

                    int cardsDrawnCount = 0;

                    decimal pctSample = ((ulong)homer.rowsPerSample / homer.GetCardCount()) * 100;
                    int drawLikelihood = (int)Math.Round(pctSample, 0, MidpointRounding.AwayFromZero);

                    while (cardsDrawnCount < homer.rowsPerSample)
                    {

                        for (ulong card = 1; card <= homer.GetCardCount(); card++)
                        {
                            if (random.Next(0, 100) <= drawLikelihood)
                            {
                                if (cardsToDraw.ContainsKey(card))
                                {
                                    cardsToDraw[card]++;
                                }
                                else
                                {
                                    cardsToDraw.Add(card, 1);
                                }

                                cardsDrawnCount++;
                                if (cardsDrawnCount == homer.rowsPerSample) break;

                            }
                        }


                    }
                }
                #endregion




                #region Get Busy Writin' or Get Busy Dyin'

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


                            ulong rowNumber = 0;

                            foreach (var line in csvDat.Item2)
                            {
                                rowNumber++;
                                if (cardsToDraw.ContainsKey(rowNumber))
                                {

                                    string rowToWriteString = RowCleaner.CleanRow(line.ToArray<string>(), homer.GetDelim(), quoteString, escapedQuoteString) + Environment.NewLine;
                                    for (int numDraws = 0; numDraws < cardsToDraw[rowNumber]; numDraws++) streamWriter.Write(rowToWriteString);

                                }
                                

                            }
                                
                            
                        }



                    }

                }
                    #endregion


            }





            using (var stream = File.OpenRead(homer.GetInputFile()))
            using (var reader = new StreamReader(stream, encoding: homer.GetEncoding()))
            {
                if (homer.HasHeader())               {
                    var csvDat = CsvParser.ParseHeadAndTail(reader, homer.GetDelim(), homer.GetQuote());
                    
                    try
                    {
                        foreach (var line in csvDat.Item2) {  }
                    }
                    catch
                    {
                        MessageBox.Show("There was an error parsing your CSV file.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var csvDat = CsvParser.Parse(reader, homer.GetDelim(), homer.GetQuote());
                    try
                    {
                        foreach (var line in csvDat) {  }
                    }
                    catch
                    {
                        MessageBox.Show("There was an error parsing your CSV file.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }


            return;
        }





        private void backgroundWorker_SubSampleProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            MainProgressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_SubSampleRunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

            
            EnableControls();
            DisableProgBar();
            //messagebox that it's done
            NumRowsLabel.Text = "Finished!";

        }



















        private string ToKMB(ulong num)
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

