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
    public partial class RoyalSamplerForm : Form
    {

        static string versionNumber = "v1.0.0, by Ryan L. Boyd";

        public RoyalSamplerForm()
        {
            InitializeComponent();
        }

        Homer hoju;
        BackgroundWorker theDealer;
        


        private void RoyalSamplerForm_Load(object sender, EventArgs e)
        {

            DisableProgBar();

            SubsamplingModeComboBox.Items.Add("Randomized Subsampling");
            SubsamplingModeComboBox.Items.Add("Targeted Subsample");

            SubsamplingModeComboBox.SelectedItem = "Randomized Subsampling";

            foreach (var encoding in Encoding.GetEncodings())
            {
                EncodingComboBox.Items.Add(encoding.Name);
            }

            try 
            { 
                Encoding selectedEncoding = Encoding.GetEncoding("utf-8");
                EncodingComboBox.SelectedIndex = EncodingComboBox.FindStringExact(selectedEncoding.BodyName);
            }
            catch
            {
                EncodingComboBox.SelectedIndex = EncodingComboBox.FindStringExact(Encoding.Default.BodyName);
            }

            this.Text = "Royal Sampler " + versionNumber;

            DelimiterTextBox.Text = ",";
            QuoteTextBox.Text = "\"";
            ContainsHeaderCheckbox.Checked = true;
            NumSubsamplesTextbox.Text = "1000";
            NumSubsamplesTextbox.MaxLength = 10;
            RowsPerSampleTextbox.Text = "100000";
            RowsPerSampleTextbox.MaxLength = 10;
            InputFileTextbox.Select();


            InputFileTextbox.Enabled = false;
            MainProgressBar.Minimum = 0;
            MainProgressBar.Maximum = 100;
            MainProgressBar.Value = 0;
            MainProgressBar.Step = 1;
            MainProgressBar.Enabled = false;
            AllowReplacementsCheckbox.Checked = true;

            ChangeCancelToStartButton();

            theDealer = new BackgroundWorker();
            hoju = new Homer();

        }



        private void OpenFileButton_Click(object sender, EventArgs e)
        {

            InputFileTextbox.Text = "";
            ColumnsToRetainCheckedListBox.Items.Clear();

            if (DelimiterTextBox.TextLength < 1 || QuoteTextBox.TextLength < 1)
            {
                MessageBox.Show("You must enter characters for your delimiter and quotes, respectively.", "D'oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var dialog = new OpenFileDialog())
            {

                dialog.Multiselect = false;
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.ValidateNames = true;
                dialog.Title = "Please choose the CSV file that you would like to read";
                dialog.FileName = "Your Input File.csv";
                dialog.Filter = "Comma-Separated Values (CSV) File (*.csv)|*.csv";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    InputFileTextbox.Text = dialog.FileName;
                    InputFileTextbox.SelectionStart = InputFileTextbox.Text.Length;
                    InputFileTextbox.SelectionLength = 0;
                }
                else
                {
                    InputFileTextbox.Text = "";
                    return;
                }
            }
            

            hoju = new Homer();
            hoju.InitializeFileDetails(fileIn: InputFileTextbox.Text, 
                             allowRepl: AllowReplacementsCheckbox.Checked, 
                             containsHead: ContainsHeaderCheckbox.Checked, 
                             fEncode: Encoding.GetEncoding(EncodingComboBox.SelectedItem.ToString()),
                             quotechar: QuoteTextBox.Text[0],
                             delimchar: DelimiterTextBox.Text[0]);

            BackgroundWorker rowCounter = new BackgroundWorker();
            rowCounter.WorkerReportsProgress = true;
            rowCounter.DoWork += new DoWorkEventHandler(backgroundWorker_CountRows);
            rowCounter.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_CountRowsProgressChanged);
            rowCounter.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_CountRowsRunWorkerCompleted);

            DisableControls();
            StartButton.Enabled = false;
            EnableProgBarNeverEnding();
            StatusLabel.Text = "Counting rows of data...";

            

            //let's get counting, but on a background thread
            rowCounter.RunWorkerAsync(hoju);

        }






        private void DisableControls()
        {
            EncodingComboBox.Enabled = false;
            DelimiterTextBox.Enabled = false;
            QuoteTextBox.Enabled = false;
            ContainsHeaderCheckbox.Enabled = false;
            OpenFileButton.Enabled = false;
            NumSubsamplesTextbox.Enabled = false;
            RowsPerSampleTextbox.Enabled = false;
            RandomSeedTextBox.Enabled = false;
            ColumnsToRetainCheckedListBox.Enabled = false;
            SubsamplingModeComboBox.Enabled = false;

            AllowReplacementsCheckbox.Enabled = false;
            
        }

        private void EnableControls()
        {
            EncodingComboBox.Enabled = true;
            DelimiterTextBox.Enabled = true;
            QuoteTextBox.Enabled = true;
            ContainsHeaderCheckbox.Enabled = true;
            OpenFileButton.Enabled = true;
            NumSubsamplesTextbox.Enabled = true;
            RowsPerSampleTextbox.Enabled = true;
            
            ColumnsToRetainCheckedListBox.Enabled = true;
            SubsamplingModeComboBox.Enabled = true;


            if (SubsamplingModeComboBox.GetItemText(SubsamplingModeComboBox.SelectedItem) == "Randomized Subsampling")
            {
                AllowReplacementsCheckbox.Enabled = true;
                RandomSeedTextBox.Enabled = true;
            }
            else if (SubsamplingModeComboBox.GetItemText(SubsamplingModeComboBox.SelectedItem) == "Targeted Subsample")
            {
                AllowReplacementsCheckbox.Enabled = false;
                RandomSeedTextBox.Enabled = false;
            }
           
        }

        private void ChangeStartToCancelButton() 
        {
            StartButton.Text = "Cancel";
            StartButton.BackColor = Color.Salmon;
        }

        private void ChangeCancelToStartButton()
        {
            StartButton.Text = "Begin Subsampling!";
            StartButton.BackColor = Color.LightGreen;
        }




        private void EnableProgBarNeverEnding() {
            MainProgressBar.Enabled = true;
            MainProgressBar.Style = ProgressBarStyle.Marquee;
            MainProgressBar.MarqueeAnimationSpeed = 30;
        }

        //private void EnableProgBarPct()
        //{
        //    MainProgressBar.Enabled = true;
        //    MainProgressBar.Style = ProgressBarStyle.Blocks;
        //    MainProgressBar.Minimum = 0;
        //    MainProgressBar.Maximum = 100;
        //    MainProgressBar.Value = 0;
        //}

        private void DisableProgBar()
        {
            MainProgressBar.Style = ProgressBarStyle.Continuous;
            MainProgressBar.MarqueeAnimationSpeed = 0;
            MainProgressBar.Enabled = false;
        }



        
        
        private void StartButton_Click(object sender, EventArgs e)
        {

            string selectedItemText = SubsamplingModeComboBox.GetItemText(SubsamplingModeComboBox.SelectedItem);

            if (theDealer.IsBusy)
            {
                theDealer.CancelAsync();
                StartButton.Text = "Cancelling...";
            }
            else
            {

                if (selectedItemText == "Randomized Subsampling")
                {
                    LaunchRandomSubsampler();
                }
                else if (selectedItemText == "Targeted Subsample")
                {
                    LaunchTargetedSubsampler();
                }

            }
        }

        private void SubsamplingModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            string selectedItemText = SubsamplingModeComboBox.GetItemText(SubsamplingModeComboBox.SelectedItem);

            if (selectedItemText == "Randomized Subsampling")
            {
                labelNumSubsamples.Text = "Number of Subsamples to Draw:";
                labelRowsPerSample.Text = "Number of Rows per Sample";
                AllowReplacementsCheckbox.Enabled = true;
                RandomSeedTextBox.Enabled = true;
            }
            else if (selectedItemText == "Targeted Subsample")
            {
                labelNumSubsamples.Text = "Start Sampling at Row #:";
                labelRowsPerSample.Text = "Stop Sampling at Row #:";
                AllowReplacementsCheckbox.Enabled = false;
                RandomSeedTextBox.Enabled = false;

            }

        }
    }


}

