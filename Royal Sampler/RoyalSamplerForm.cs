using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace royalsampler
{
    public partial class RoyalSamplerForm : Form
    {

        

        public RoyalSamplerForm()
        {
            InitializeComponent();
        }

        Homer hoju;
        BackgroundWorker theDealer;
        


        private void RoyalSamplerForm_Load(object sender, EventArgs e)
        {

            DisableProgBar();

            SubsamplingModeComboBox.Items.Add("Split File into Chunks");
            SubsamplingModeComboBox.Items.Add("Sample by Range");
            SubsamplingModeComboBox.Items.Add("Randomized Subsampling");
            
            SubsamplingModeComboBox.SelectedItem = "Split File into Chunks";

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

            this.Text = "Royal Sampler v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + ", by Ryan L. Boyd";

            DelimiterTextBox.Text = ",";
            QuoteTextBox.Text = "\"";
            ContainsHeaderCheckbox.Checked = true;
            NumberOfFilesToCreateTextBox.Text = "5";
            NumberOfFilesToCreateTextBox.MaxLength = 10;
            NumberOfSamplesPerFileTextBox.Text = "";
            NumberOfSamplesPerFileTextBox.MaxLength = 10;
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
            NumberOfFilesToCreateTextBox.Enabled = false;
            NumberOfSamplesPerFileTextBox.Enabled = false;
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
            NumberOfFilesToCreateTextBox.Enabled = true;
            NumberOfSamplesPerFileTextBox.Enabled = true;
            
            ColumnsToRetainCheckedListBox.Enabled = true;
            SubsamplingModeComboBox.Enabled = true;


            if (SubsamplingModeComboBox.GetItemText(SubsamplingModeComboBox.SelectedItem) == "Randomized Subsampling")
            {
                AllowReplacementsCheckbox.Enabled = true;
                RandomSeedTextBox.Enabled = true;
            }
            else if (SubsamplingModeComboBox.GetItemText(SubsamplingModeComboBox.SelectedItem) == "Sample by Range")
            {
                AllowReplacementsCheckbox.Enabled = false;
                RandomSeedTextBox.Enabled = false;
            }
            else if (SubsamplingModeComboBox.GetItemText(SubsamplingModeComboBox.SelectedItem) == "Split File into Chunks")
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
                else if (selectedItemText == "Sample by Range")
                {
                    LaunchTargetedSubsampler();
                }
                else if (selectedItemText == "Split File into Chunks")
                {
                    LaunchSplitIntoChunks();
                }

            }
        }

        private void SubsamplingModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            string selectedItemText = SubsamplingModeComboBox.GetItemText(SubsamplingModeComboBox.SelectedItem);

            if (selectedItemText == "Randomized Subsampling")
            {
                labelNumberOfFilesToCreate.Text = "# of Subsample Files to Create:";
                labelNumberOfSamplesPerFile.Text = "# of Samples per File:";
                NumberOfFilesToCreateTextBox.Text = "1";
                NumberOfSamplesPerFileTextBox.Text = "100000";
                AllowReplacementsCheckbox.Enabled = true;
                RandomSeedTextBox.Enabled = true;
            }
            else if (selectedItemText == "Sample by Range")
            {
                labelNumberOfFilesToCreate.Text = "Start Sampling at Row #:";
                labelNumberOfSamplesPerFile.Text = "Stop Sampling at Row #:";

                NumberOfFilesToCreateTextBox.Text = "1";
                
                if (!String.IsNullOrEmpty(InputFileTextbox.Text))
                {
                    NumberOfSamplesPerFileTextBox.Text = hoju.GetRowCount().ToString();
                }
                else
                {
                    NumberOfSamplesPerFileTextBox.Text = "10000";
                }
                    

                AllowReplacementsCheckbox.Enabled = false;
                RandomSeedTextBox.Enabled = false;

            }
            else if (selectedItemText == "Split File into Chunks")
            {
                labelNumberOfFilesToCreate.Text = "Split into N files:";
                labelNumberOfSamplesPerFile.Text = "Split with N rows per file:";

                NumberOfFilesToCreateTextBox.Text = "5";
                NumberOfSamplesPerFileTextBox.Text = "";

                AllowReplacementsCheckbox.Enabled = false;
                RandomSeedTextBox.Enabled = false;
            }

            

        }

        private void NumSubsamplesTextbox_Enter(object sender, EventArgs e)
        {
            string selectedItemText = SubsamplingModeComboBox.GetItemText(SubsamplingModeComboBox.SelectedItem);
            if (selectedItemText == "Split File into Chunks")
            {
                NumberOfSamplesPerFileTextBox.Text = "";
            }

        }

        private void NumRowsPerSampleTextbox_Enter(object sender, EventArgs e)
        {
            string selectedItemText = SubsamplingModeComboBox.GetItemText(SubsamplingModeComboBox.SelectedItem);
            if (selectedItemText == "Split File into Chunks")
            {
                NumberOfFilesToCreateTextBox.Text = "";
            }
        }

        private void AllowReplacementsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!AllowReplacementsCheckbox.Checked)
            {
                MessageBox.Show("Note that subsampling *without* replacement is not very efficient in its current form. This option will not work properly with datasets that contain > 2 billion rows of data.", "Ruh Roh!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }


}

