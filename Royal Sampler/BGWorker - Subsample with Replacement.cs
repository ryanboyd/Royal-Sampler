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




        private void backgroundWorker_SubSampleWithReplacement(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Homer homer = (Homer)e.Argument;
            Random random = new Random();

            if (!String.IsNullOrEmpty(homer.randSeedString)) random = new Random(int.Parse(homer.randSeedString));

            string filenamePadding = "D" + homer.numberOfSamples.ToString().Length.ToString();
            string quoteString = homer.GetQuote().ToString();
            string escapedQuoteString = homer.GetQuote().ToString() + homer.GetQuote().ToString();
            int numCols = homer.retainedIndices.Count;

            for (int sampleNumber = 0; sampleNumber < homer.numberOfSamples; sampleNumber++)
            {

                if ((sender as BackgroundWorker).CancellationPending) break;

                //report progress
                //MessageBox.Show((((double)sampleNumber / homer.numberOfSamples) * 100).ToString());
                int pctDone = (int)Math.Round((((double)sampleNumber / homer.numberOfSamples) * 10000), 0, MidpointRounding.AwayFromZero);
                (sender as BackgroundWorker).ReportProgress(pctDone);


                Dictionary<int, int> cardsToDraw = new Dictionary<int, int>();
                
                #region Determine Our Samples Needed
           
                    
                int cardsDrawnCount = 0;

                //decimal pctSample = ((ulong)homer.rowsPerSample / homer.GetCardCount()) * 100;
                //int drawLikelihood = (int)Math.Round(pctSample, 0, MidpointRounding.AwayFromZero);

                while (cardsDrawnCount < homer.rowsPerSample)
                {
                    int randomDraw = random.Next(1, homer.GetRowCount());

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
                            string rowToWriteString = RowCleaner.CleanRow(headerRow, homer.GetDelim(), quoteString, escapedQuoteString, numCols, hoju.retainedIndices);

                            //write the header row
                            streamWriter.Write(rowToWriteString);
                            

                            int rowNumber = 0;

                            foreach (var line in csvDat.Item2)
                            {
                                rowNumber++;
                                if (cardsToDraw.ContainsKey(rowNumber))
                                {

                                    rowToWriteString = RowCleaner.CleanRow(line.ToArray<string>(), homer.GetDelim(), quoteString, escapedQuoteString, numCols, hoju.retainedIndices);
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

                                    string rowToWriteString = RowCleaner.CleanRow(line.ToArray<string>(), homer.GetDelim(), quoteString, escapedQuoteString, numCols, hoju.retainedIndices);
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


    }


}

