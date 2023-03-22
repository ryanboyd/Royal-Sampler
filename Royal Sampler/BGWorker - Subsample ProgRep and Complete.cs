using System.Windows.Forms;
using System;

namespace royalsampler
{
    public partial class RoyalSamplerForm
    {


        private void backgroundWorker_SubSampleProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            //MainProgressBar.Value = e.ProgressPercentage;
            StatusLabel.Text = "Fetching and Writing subsample(s)... " + ((double)(e.ProgressPercentage / (double)100)).ToString("0.00") + "% complete...";
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

            if ((string)e.Result != "Cancelled")
            {
                MessageBox.Show("Your file has successfully been subsampled. Hooray!", "Woohoo!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Your subsampling process has been cancelled.", "Woohoo?", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        internal static int calcPctDone(ulong rowNumber, ulong rowsPerSample, ulong sampleNumber, ulong numberOfSamples)
        {
            int pctDone = 0;
            double pctDoneSamples = (double)sampleNumber / numberOfSamples;
            double pctDoneRows = ((double)rowNumber / rowsPerSample) / numberOfSamples;
            pctDone = (int)Math.Round(((pctDoneSamples + pctDoneRows) * 10000), 0, MidpointRounding.AwayFromZero);
            return pctDone;
        }



    }
}


