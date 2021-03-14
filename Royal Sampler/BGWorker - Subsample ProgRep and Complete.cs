using System.Windows.Forms;


namespace royalsampler
{
    public partial class RoyalSamplerForm
    {


        private void backgroundWorker_SubSampleProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            //MainProgressBar.Value = e.ProgressPercentage;
            StatusLabel.Text = "Fetching and Writing subsample(s)... " + ((double)(e.ProgressPercentage / (double)100)).ToString(".00") + "% complete...";
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

            if (e.Result != "Cancelled")
            {
                MessageBox.Show("Your file has successfully subsampled. Hooray!" + " rows.", "Woohoo!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Your subsampling process has been cancelled.", "Woohoo?", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

    }
}


