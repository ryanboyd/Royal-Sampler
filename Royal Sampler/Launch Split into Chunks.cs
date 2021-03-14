using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace royalsampler
{

    public partial class RoyalSamplerForm
    {


        private void LaunchSplitIntoChunks()
        {

            ulong numFileParameter = 0;
            ulong numRowParameter = 0;

            NumSubsamplesTextbox.Text = NumSubsamplesTextbox.Text.Trim();
            RowsPerSampleTextbox.Text = RowsPerSampleTextbox.Text.Trim();
            RandomSeedTextBox.Text = RandomSeedTextBox.Text.Trim();

            string segmentationStrategy = "";

            if (String.IsNullOrEmpty(RowsPerSampleTextbox.Text) && String.IsNullOrEmpty(NumSubsamplesTextbox.Text))
            {
                MessageBox.Show("You must choose to segment your file either by the number of rows or by the number of files that you would like to split it into. Please input a number into the corresponding box before proceeding.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!String.IsNullOrEmpty(NumSubsamplesTextbox.Text))
            {
                if (!ulong.TryParse(NumSubsamplesTextbox.Text, out numFileParameter) || numFileParameter < 1)
                {
                    MessageBox.Show("Your segmentation option must be a positive integer.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                segmentationStrategy = "NumFiles";

            }

            if (!String.IsNullOrEmpty(RowsPerSampleTextbox.Text))
            {
                if (!ulong.TryParse(RowsPerSampleTextbox.Text, out numRowParameter) || numRowParameter < 1)
                {
                    MessageBox.Show("Your segmentation option must be a positive integer.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                segmentationStrategy = "NumRows";
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


                
                if (segmentationStrategy == "NumFiles")
                {
                    numRowParameter = (ulong)Math.Ceiling(hoju.GetRowCount() / (double)numFileParameter);
                }
                else if (segmentationStrategy == "NumRows")
                {
                    numFileParameter = (ulong)Math.Ceiling(hoju.GetRowCount() / (double)numRowParameter);
                }

                hoju.SetOutputFolder(folderBrowser.SelectedPath);
                
                hoju.numberOfSamples = numFileParameter;
                hoju.rowsPerSample = numRowParameter;

                hoju.retainedIndices = new HashSet<int>();

                foreach (int index in ColumnsToRetainCheckedListBox.CheckedIndices) hoju.retainedIndices.Add(index);




                DisableControls();
                ChangeStartToCancelButton();

                theDealer = new BackgroundWorker();

                theDealer.DoWork += new DoWorkEventHandler(backgroundWorker_SplitIntoChunks);

                theDealer.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_SubSampleProgressChanged);
                theDealer.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_SubSampleRunWorkerCompleted);
                theDealer.WorkerReportsProgress = true;
                theDealer.WorkerSupportsCancellation = true;

                StatusLabel.Text = "Preparing to chunkify...";
                EnableProgBarNeverEnding();
                theDealer.RunWorkerAsync(hoju);


            }



        }



    }

}