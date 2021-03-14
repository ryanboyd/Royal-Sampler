using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;

namespace royalsampler
{

    public partial class RoyalSamplerForm
    {


        private void LaunchTargetedSubsampler()
        {

            ulong startRow = 0;
            ulong endRow = 0;

            NumSubsamplesTextbox.Text = NumSubsamplesTextbox.Text.Trim();
            RowsPerSampleTextbox.Text = RowsPerSampleTextbox.Text.Trim();
            RandomSeedTextBox.Text = RandomSeedTextBox.Text.Trim();


            if (!ulong.TryParse(NumSubsamplesTextbox.Text, out startRow) || startRow < 1)
            {
                MessageBox.Show("Your Starting Row # must be a positive integer.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!ulong.TryParse(RowsPerSampleTextbox.Text, out endRow) || endRow < 1)
            {
                MessageBox.Show("Your Ending Row Number must be a positive integer.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (startRow >= endRow)
            {
                MessageBox.Show("Your Starting Row # must be smaller than your Ending Row #.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            

            SaveFileDialog fileDialog = new SaveFileDialog();

            fileDialog.Title = "Please choose the output location for your subsampled CSV file";
            fileDialog.FileName = Path.GetFileNameWithoutExtension(InputFileTextbox.Text) + "_subsampled.csv";
            fileDialog.Filter = "Comma-Separated Values (CSV) File (*.csv)|*.csv";
            fileDialog.InitialDirectory = Path.GetFullPath(InputFileTextbox.Text);
            fileDialog.OverwritePrompt = true;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {

                if (fileDialog.FileName == InputFileTextbox.Text)
                {
                    MessageBox.Show("You cannot overwrite your input file.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                hoju.SetOutputFolder(fileDialog.FileName);
                hoju.startRow = startRow;
                hoju.endRow = endRow;
                hoju.retainedIndices = new HashSet<int>();

                foreach (int index in ColumnsToRetainCheckedListBox.CheckedIndices) hoju.retainedIndices.Add(index);




                DisableControls();
                ChangeStartToCancelButton();

                theDealer = new BackgroundWorker();

                theDealer.DoWork += new DoWorkEventHandler(backgroundWorker_TargetedSubsampling);

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