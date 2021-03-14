using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace royalsampler
{

    public partial class RoyalSamplerForm : Form
    {


        private void LaunchRandomSubsampler()
        {

            int numSamples = 0;
            int numRowsPerSample = 0;
            int randomSeed = 0;

            NumSubsamplesTextbox.Text = NumSubsamplesTextbox.Text.Trim();
            RowsPerSampleTextbox.Text = RowsPerSampleTextbox.Text.Trim();
            RandomSeedTextBox.Text = RandomSeedTextBox.Text.Trim();


            if (!int.TryParse(NumSubsamplesTextbox.Text, out numSamples) || numSamples < 1)
            {
                MessageBox.Show("Your \"Number of Subsamples\" must be a positive integer.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(RowsPerSampleTextbox.Text, out numRowsPerSample) || numRowsPerSample < 1)
            {
                MessageBox.Show("Your \"Number of Rows per Sample\" must be a positive integer.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(RandomSeedTextBox.Text, out randomSeed) && !String.IsNullOrEmpty(RandomSeedTextBox.Text))
            {
                MessageBox.Show("Your random seed must be an integer. If you do not want to use a randomization seed, you can leave the \"Random Seed\" box blank.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (String.IsNullOrEmpty(InputFileTextbox.Text))
            {
                MessageBox.Show("Your must first select a file that you would like to subsample.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (String.IsNullOrEmpty(DelimiterTextBox.Text))
            {
                MessageBox.Show("Your must select your delimiter character for this CSV file.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (String.IsNullOrEmpty(QuoteTextBox.Text))
            {
                MessageBox.Show("Your must select your quoting character for this CSV file.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (ColumnsToRetainCheckedListBox.CheckedIndices.Count < 1)
            {
                MessageBox.Show("Your must select at least one column to retain for your subsampling.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.UseDescriptionForTitle = true;
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = "Please choose the OUTPUT location for your files";

            string inputPath = Path.GetDirectoryName(InputFileTextbox.Text);
            if (!Path.EndsInDirectorySeparator(inputPath)) inputPath += Path.DirectorySeparatorChar;

            folderBrowser.SelectedPath = inputPath;

            if (folderBrowser.ShowDialog() != DialogResult.Cancel)
            {

                if (folderBrowser.SelectedPath == Path.GetDirectoryName(InputFileTextbox.Text))
                {
                    MessageBox.Show("You cannot save your subsampled output files to the same folder as your input file.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                hoju.SetOutputFolder(folderBrowser.SelectedPath);
                hoju.numberOfSamples = ulong.Parse(NumSubsamplesTextbox.Text);
                hoju.rowsPerSample = ulong.Parse(RowsPerSampleTextbox.Text);
                hoju.allowReplacement = AllowReplacementsCheckbox.Checked;
                hoju.randSeedString = RandomSeedTextBox.Text;
                hoju.retainedIndices = new HashSet<int>();

                foreach (int index in ColumnsToRetainCheckedListBox.CheckedIndices) hoju.retainedIndices.Add(index);




                DisableControls();
                ChangeStartToCancelButton();

                theDealer = new BackgroundWorker();

                if (AllowReplacementsCheckbox.Checked)
                {
                    theDealer.DoWork += new DoWorkEventHandler(backgroundWorker_SubSampleWithReplacement);
                }
                else
                {
                    theDealer.DoWork += new DoWorkEventHandler(backgroundWorker_SubSampleWithoutReplacement);
                }

                theDealer.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_SubSampleProgressChanged);
                theDealer.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_SubSampleRunWorkerCompleted);
                theDealer.WorkerReportsProgress = true;
                theDealer.WorkerSupportsCancellation = true;

                StatusLabel.Text = "Preparing to subsample...";
                EnableProgBarNeverEnding();
                theDealer.RunWorkerAsync(hoju);


            }



        }



    }

}