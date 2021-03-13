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
        public RoyalSamplerForm()
        {
            InitializeComponent();
        }

        Homer hoju;
        //Timer timer;


        private void RoyalSamplerForm_Load(object sender, EventArgs e)
        {

            DisableProgBar();

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

            DelimiterTextBox.Text = ",";
            QuoteTextBox.Text = "\"";
            ContainsHeaderCheckbox.Checked = true;

            InputFileTextbox.Enabled = false;
            MainProgressBar.Minimum = 0;
            MainProgressBar.Maximum = 100;
            MainProgressBar.Value = 0;
            MainProgressBar.Enabled = false;

            hoju = new Homer();

        }



        private void OpenFileButton_Click(object sender, EventArgs e)
        {

            InputFileTextbox.Text = "";

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
            hoju.ArrangeDeck(fileIn: InputFileTextbox.Text, 
                             allowRepl: AllowReplacementsCheckbox.Checked, 
                             containsHead: ContainsHeaderCheckbox.Checked, 
                             fEncode: Encoding.GetEncoding(EncodingComboBox.SelectedItem.ToString()),
                             quotechar: QuoteTextBox.Text[0],
                             delimchar: DelimiterTextBox.Text[0]);

            BackgroundWorker cardCounter = new BackgroundWorker();
            cardCounter.WorkerReportsProgress = false;
            cardCounter.DoWork += new DoWorkEventHandler(backgroundWorker_CountRows);
            cardCounter.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_CountRowsProgressChanged);
            cardCounter.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_CountRowsRunWorkerCompleted);


            DisableControls();
            EnableProgBarPct();
            NumRowsLabel.Text = "Counting rows of data...";

            //let's get counting, but on a background thread
            cardCounter.RunWorkerAsync(hoju);

            //timer = new Timer();
            //timer.Interval = (100); // 0.5 secs
            //timer.Tick += new EventHandler(timer_RowCountTick);
            //timer.Start();

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

            AllowReplacementsCheckbox.Enabled = true;
            
        }





        private void EnableProgBarNeverEnding() {
            MainProgressBar.Style = ProgressBarStyle.Marquee;
            MainProgressBar.MarqueeAnimationSpeed = 30;
        }

        private void EnableProgBarPct()
        {
            MainProgressBar.Style = ProgressBarStyle.Blocks;
            MainProgressBar.Minimum = 0;
            MainProgressBar.Maximum = 100;
            MainProgressBar.Value = 0;
        }

        private void DisableProgBar()
        {
            MainProgressBar.Style = ProgressBarStyle.Continuous;
            MainProgressBar.MarqueeAnimationSpeed = 0;
        }

        
        
        private void StartButton_Click(object sender, EventArgs e)
        {
            int numSamples = 0;
            int numRowsPerSample = 0;

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


             


            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.UseDescriptionForTitle = true;
            folderBrowser.ShowNewFolderButton = true;
            folderBrowser.Description = "Please choose the OUTPUT location for your files";

            folderBrowser.SelectedPath = Path.GetDirectoryName(InputFileTextbox.Text);


            if (folderBrowser.ShowDialog() != DialogResult.Cancel)
            {



                hoju.SetOutputFolder(folderBrowser.SelectedPath);
                hoju.numberOfSamples = int.Parse(NumSubsamplesTextbox.Text);
                hoju.rowsPerSample = int.Parse(RowsPerSampleTextbox.Text);
                hoju.allowReplacement = AllowReplacementsCheckbox.Checked;

                DisableControls();

                BackgroundWorker theDealer = new BackgroundWorker();
                theDealer.DoWork += new DoWorkEventHandler(backgroundWorker_SubSample);
                theDealer.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_SubSampleProgressChanged);
                theDealer.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_SubSampleRunWorkerCompleted);
                theDealer.WorkerReportsProgress = true;


                theDealer.RunWorkerAsync(hoju);



            }










        }





    }


}

