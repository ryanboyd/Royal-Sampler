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
            
            

            
            for (int sampleNumber = 0; sampleNumber < homer.numberOfSamples; sampleNumber++)
            {
                
                if (homer.allowReplacement)
                {

                }


            }





            using (var stream = File.OpenRead(homer.GetInputFolder()))
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
            //progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_SubSampleRunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

            
            EnableControls();
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


}

