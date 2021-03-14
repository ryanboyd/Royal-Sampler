namespace royalsampler
{
    partial class RoyalSamplerForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoyalSamplerForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ContainsHeaderCheckbox = new System.Windows.Forms.CheckBox();
            this.labelCurrentFile = new System.Windows.Forms.Label();
            this.OpenFileButton = new System.Windows.Forms.Button();
            this.labelFileEncoding = new System.Windows.Forms.Label();
            this.EncodingComboBox = new System.Windows.Forms.ComboBox();
            this.labelQuote = new System.Windows.Forms.Label();
            this.labelDelimiter = new System.Windows.Forms.Label();
            this.InputFileTextbox = new System.Windows.Forms.TextBox();
            this.QuoteTextBox = new System.Windows.Forms.TextBox();
            this.DelimiterTextBox = new System.Windows.Forms.TextBox();
            this.SubsamplingGroupBox = new System.Windows.Forms.GroupBox();
            this.SubsamplingModeComboBox = new System.Windows.Forms.ComboBox();
            this.StartButton = new System.Windows.Forms.Button();
            this.labelRandSeed = new System.Windows.Forms.Label();
            this.RandomSeedTextBox = new System.Windows.Forms.TextBox();
            this.labelRowsPerSample = new System.Windows.Forms.Label();
            this.RowsPerSampleTextbox = new System.Windows.Forms.TextBox();
            this.labelNumSubsamples = new System.Windows.Forms.Label();
            this.AllowReplacementsCheckbox = new System.Windows.Forms.CheckBox();
            this.NumSubsamplesTextbox = new System.Windows.Forms.TextBox();
            this.MainProgressBar = new System.Windows.Forms.ProgressBar();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.ColumnsToRetainCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.retainColsLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SubsamplingGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ContainsHeaderCheckbox);
            this.groupBox1.Controls.Add(this.labelCurrentFile);
            this.groupBox1.Controls.Add(this.OpenFileButton);
            this.groupBox1.Controls.Add(this.labelFileEncoding);
            this.groupBox1.Controls.Add(this.EncodingComboBox);
            this.groupBox1.Controls.Add(this.labelQuote);
            this.groupBox1.Controls.Add(this.labelDelimiter);
            this.groupBox1.Controls.Add(this.InputFileTextbox);
            this.groupBox1.Controls.Add(this.QuoteTextBox);
            this.groupBox1.Controls.Add(this.DelimiterTextBox);
            this.groupBox1.Font = new System.Drawing.Font("MS Reference Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(298, 434);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CSV File Format Settings";
            // 
            // ContainsHeaderCheckbox
            // 
            this.ContainsHeaderCheckbox.AutoSize = true;
            this.ContainsHeaderCheckbox.Location = new System.Drawing.Point(15, 194);
            this.ContainsHeaderCheckbox.Name = "ContainsHeaderCheckbox";
            this.ContainsHeaderCheckbox.Size = new System.Drawing.Size(225, 23);
            this.ContainsHeaderCheckbox.TabIndex = 4;
            this.ContainsHeaderCheckbox.Text = "CSV Contains Header Row";
            this.ContainsHeaderCheckbox.UseVisualStyleBackColor = true;
            // 
            // labelCurrentFile
            // 
            this.labelCurrentFile.AutoSize = true;
            this.labelCurrentFile.Location = new System.Drawing.Point(15, 244);
            this.labelCurrentFile.Name = "labelCurrentFile";
            this.labelCurrentFile.Size = new System.Drawing.Size(83, 19);
            this.labelCurrentFile.TabIndex = 0;
            this.labelCurrentFile.Text = "Input File:";
            // 
            // OpenFileButton
            // 
            this.OpenFileButton.Location = new System.Drawing.Point(15, 298);
            this.OpenFileButton.Name = "OpenFileButton";
            this.OpenFileButton.Size = new System.Drawing.Size(115, 44);
            this.OpenFileButton.TabIndex = 6;
            this.OpenFileButton.Text = "Open File";
            this.OpenFileButton.UseVisualStyleBackColor = true;
            this.OpenFileButton.Click += new System.EventHandler(this.OpenFileButton_Click);
            // 
            // labelFileEncoding
            // 
            this.labelFileEncoding.AutoSize = true;
            this.labelFileEncoding.Location = new System.Drawing.Point(15, 114);
            this.labelFileEncoding.Name = "labelFileEncoding";
            this.labelFileEncoding.Size = new System.Drawing.Size(104, 19);
            this.labelFileEncoding.TabIndex = 5;
            this.labelFileEncoding.Text = "File Encoding";
            // 
            // EncodingComboBox
            // 
            this.EncodingComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.EncodingComboBox.FormattingEnabled = true;
            this.EncodingComboBox.Location = new System.Drawing.Point(15, 136);
            this.EncodingComboBox.Name = "EncodingComboBox";
            this.EncodingComboBox.Size = new System.Drawing.Size(265, 27);
            this.EncodingComboBox.TabIndex = 3;
            // 
            // labelQuote
            // 
            this.labelQuote.AutoSize = true;
            this.labelQuote.Location = new System.Drawing.Point(156, 38);
            this.labelQuote.Name = "labelQuote";
            this.labelQuote.Size = new System.Drawing.Size(62, 19);
            this.labelQuote.TabIndex = 0;
            this.labelQuote.Text = "Quote:";
            // 
            // labelDelimiter
            // 
            this.labelDelimiter.AutoSize = true;
            this.labelDelimiter.Location = new System.Drawing.Point(15, 38);
            this.labelDelimiter.Name = "labelDelimiter";
            this.labelDelimiter.Size = new System.Drawing.Size(81, 19);
            this.labelDelimiter.TabIndex = 0;
            this.labelDelimiter.Text = "Delimiter:";
            // 
            // InputFileTextbox
            // 
            this.InputFileTextbox.Enabled = false;
            this.InputFileTextbox.Location = new System.Drawing.Point(15, 266);
            this.InputFileTextbox.Name = "InputFileTextbox";
            this.InputFileTextbox.Size = new System.Drawing.Size(265, 26);
            this.InputFileTextbox.TabIndex = 5;
            // 
            // QuoteTextBox
            // 
            this.QuoteTextBox.Location = new System.Drawing.Point(156, 60);
            this.QuoteTextBox.Name = "QuoteTextBox";
            this.QuoteTextBox.Size = new System.Drawing.Size(100, 26);
            this.QuoteTextBox.TabIndex = 2;
            this.QuoteTextBox.Text = "\"";
            this.QuoteTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // DelimiterTextBox
            // 
            this.DelimiterTextBox.Location = new System.Drawing.Point(15, 60);
            this.DelimiterTextBox.Name = "DelimiterTextBox";
            this.DelimiterTextBox.Size = new System.Drawing.Size(100, 26);
            this.DelimiterTextBox.TabIndex = 1;
            this.DelimiterTextBox.Text = ",";
            this.DelimiterTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SubsamplingGroupBox
            // 
            this.SubsamplingGroupBox.Controls.Add(this.SubsamplingModeComboBox);
            this.SubsamplingGroupBox.Controls.Add(this.StartButton);
            this.SubsamplingGroupBox.Controls.Add(this.labelRandSeed);
            this.SubsamplingGroupBox.Controls.Add(this.RandomSeedTextBox);
            this.SubsamplingGroupBox.Controls.Add(this.labelRowsPerSample);
            this.SubsamplingGroupBox.Controls.Add(this.RowsPerSampleTextbox);
            this.SubsamplingGroupBox.Controls.Add(this.labelNumSubsamples);
            this.SubsamplingGroupBox.Controls.Add(this.AllowReplacementsCheckbox);
            this.SubsamplingGroupBox.Controls.Add(this.NumSubsamplesTextbox);
            this.SubsamplingGroupBox.Font = new System.Drawing.Font("MS Reference Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.SubsamplingGroupBox.Location = new System.Drawing.Point(333, 12);
            this.SubsamplingGroupBox.Name = "SubsamplingGroupBox";
            this.SubsamplingGroupBox.Size = new System.Drawing.Size(298, 434);
            this.SubsamplingGroupBox.TabIndex = 0;
            this.SubsamplingGroupBox.TabStop = false;
            this.SubsamplingGroupBox.Text = "Subsampling Settings";
            // 
            // SubsamplingModeComboBox
            // 
            this.SubsamplingModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SubsamplingModeComboBox.FormattingEnabled = true;
            this.SubsamplingModeComboBox.Location = new System.Drawing.Point(13, 35);
            this.SubsamplingModeComboBox.Name = "SubsamplingModeComboBox";
            this.SubsamplingModeComboBox.Size = new System.Drawing.Size(265, 27);
            this.SubsamplingModeComboBox.TabIndex = 7;
            this.SubsamplingModeComboBox.SelectedIndexChanged += new System.EventHandler(this.SubsamplingModeComboBox_SelectedIndexChanged);
            // 
            // StartButton
            // 
            this.StartButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.StartButton.Font = new System.Drawing.Font("MS Reference Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.StartButton.Location = new System.Drawing.Point(57, 369);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(180, 44);
            this.StartButton.TabIndex = 104;
            this.StartButton.Text = "Begin Subsampling!";
            this.StartButton.UseVisualStyleBackColor = false;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // labelRandSeed
            // 
            this.labelRandSeed.AutoSize = true;
            this.labelRandSeed.Location = new System.Drawing.Point(13, 297);
            this.labelRandSeed.Name = "labelRandSeed";
            this.labelRandSeed.Size = new System.Drawing.Size(169, 19);
            this.labelRandSeed.TabIndex = 103;
            this.labelRandSeed.Text = "Randomization Seed:";
            // 
            // RandomSeedTextBox
            // 
            this.RandomSeedTextBox.Location = new System.Drawing.Point(13, 319);
            this.RandomSeedTextBox.Name = "RandomSeedTextBox";
            this.RandomSeedTextBox.Size = new System.Drawing.Size(265, 26);
            this.RandomSeedTextBox.TabIndex = 103;
            this.RandomSeedTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelRowsPerSample
            // 
            this.labelRowsPerSample.AutoSize = true;
            this.labelRowsPerSample.Location = new System.Drawing.Point(13, 167);
            this.labelRowsPerSample.Name = "labelRowsPerSample";
            this.labelRowsPerSample.Size = new System.Drawing.Size(230, 19);
            this.labelRowsPerSample.TabIndex = 0;
            this.labelRowsPerSample.Text = "Number of Rows per Sample:";
            // 
            // RowsPerSampleTextbox
            // 
            this.RowsPerSampleTextbox.Location = new System.Drawing.Point(13, 189);
            this.RowsPerSampleTextbox.Name = "RowsPerSampleTextbox";
            this.RowsPerSampleTextbox.Size = new System.Drawing.Size(265, 26);
            this.RowsPerSampleTextbox.TabIndex = 101;
            this.RowsPerSampleTextbox.Text = "100000";
            this.RowsPerSampleTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.RowsPerSampleTextbox.Enter += new System.EventHandler(this.NumRowsPerSampleTextbox_Enter);
            // 
            // labelNumSubsamples
            // 
            this.labelNumSubsamples.AutoSize = true;
            this.labelNumSubsamples.Location = new System.Drawing.Point(13, 91);
            this.labelNumSubsamples.Name = "labelNumSubsamples";
            this.labelNumSubsamples.Size = new System.Drawing.Size(254, 19);
            this.labelNumSubsamples.TabIndex = 0;
            this.labelNumSubsamples.Text = "Number of Subsamples to Draw:";
            // 
            // AllowReplacementsCheckbox
            // 
            this.AllowReplacementsCheckbox.AutoSize = true;
            this.AllowReplacementsCheckbox.Location = new System.Drawing.Point(13, 247);
            this.AllowReplacementsCheckbox.Name = "AllowReplacementsCheckbox";
            this.AllowReplacementsCheckbox.Size = new System.Drawing.Size(246, 23);
            this.AllowReplacementsCheckbox.TabIndex = 102;
            this.AllowReplacementsCheckbox.Text = "Subsample with Replacement";
            this.AllowReplacementsCheckbox.UseVisualStyleBackColor = true;
            // 
            // NumSubsamplesTextbox
            // 
            this.NumSubsamplesTextbox.Location = new System.Drawing.Point(13, 113);
            this.NumSubsamplesTextbox.Name = "NumSubsamplesTextbox";
            this.NumSubsamplesTextbox.Size = new System.Drawing.Size(265, 26);
            this.NumSubsamplesTextbox.TabIndex = 100;
            this.NumSubsamplesTextbox.Text = "1000";
            this.NumSubsamplesTextbox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumSubsamplesTextbox.Enter += new System.EventHandler(this.NumSubsamplesTextbox_Enter);
            // 
            // MainProgressBar
            // 
            this.MainProgressBar.Location = new System.Drawing.Point(12, 490);
            this.MainProgressBar.Name = "MainProgressBar";
            this.MainProgressBar.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.MainProgressBar.Size = new System.Drawing.Size(954, 33);
            this.MainProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.MainProgressBar.TabIndex = 0;
            // 
            // StatusLabel
            // 
            this.StatusLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.StatusLabel.Location = new System.Drawing.Point(12, 465);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(954, 22);
            this.StatusLabel.TabIndex = 0;
            this.StatusLabel.Text = "Waiting...";
            this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ColumnsToRetainCheckedListBox
            // 
            this.ColumnsToRetainCheckedListBox.CheckOnClick = true;
            this.ColumnsToRetainCheckedListBox.Font = new System.Drawing.Font("MS Reference Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ColumnsToRetainCheckedListBox.FormattingEnabled = true;
            this.ColumnsToRetainCheckedListBox.Location = new System.Drawing.Point(652, 34);
            this.ColumnsToRetainCheckedListBox.Name = "ColumnsToRetainCheckedListBox";
            this.ColumnsToRetainCheckedListBox.ScrollAlwaysVisible = true;
            this.ColumnsToRetainCheckedListBox.Size = new System.Drawing.Size(302, 412);
            this.ColumnsToRetainCheckedListBox.TabIndex = 200;
            // 
            // retainColsLabel
            // 
            this.retainColsLabel.AutoSize = true;
            this.retainColsLabel.Font = new System.Drawing.Font("MS Reference Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.retainColsLabel.Location = new System.Drawing.Point(652, 12);
            this.retainColsLabel.Name = "retainColsLabel";
            this.retainColsLabel.Size = new System.Drawing.Size(153, 19);
            this.retainColsLabel.TabIndex = 105;
            this.retainColsLabel.Text = "Columns to Retain:";
            // 
            // RoyalSamplerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(978, 533);
            this.Controls.Add(this.retainColsLabel);
            this.Controls.Add(this.ColumnsToRetainCheckedListBox);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.MainProgressBar);
            this.Controls.Add(this.SubsamplingGroupBox);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RoyalSamplerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Royal Sampler";
            this.Load += new System.EventHandler(this.RoyalSamplerForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.SubsamplingGroupBox.ResumeLayout(false);
            this.SubsamplingGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelQuote;
        private System.Windows.Forms.Label labelDelimiter;
        private System.Windows.Forms.TextBox InputFileTextbox;
        private System.Windows.Forms.TextBox QuoteTextBox;
        private System.Windows.Forms.TextBox DelimiterTextBox;
        private System.Windows.Forms.ComboBox EncodingComboBox;
        private System.Windows.Forms.Label labelFileEncoding;
        private System.Windows.Forms.Label labelCurrentFile;
        private System.Windows.Forms.Button OpenFileButton;
        private System.Windows.Forms.CheckBox ContainsHeaderCheckbox;
        private System.Windows.Forms.GroupBox SubsamplingGroupBox;
        private System.Windows.Forms.CheckBox AllowReplacementsCheckbox;
        private System.Windows.Forms.ProgressBar MainProgressBar;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Label labelRowsPerSample;
        private System.Windows.Forms.TextBox RowsPerSampleTextbox;
        private System.Windows.Forms.Label labelNumSubsamples;
        private System.Windows.Forms.TextBox NumSubsamplesTextbox;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Label labelRandSeed;
        private System.Windows.Forms.TextBox RandomSeedTextBox;
        private System.Windows.Forms.CheckedListBox ColumnsToRetainCheckedListBox;
        private System.Windows.Forms.Label retainColsLabel;
        private System.Windows.Forms.ComboBox SubsamplingModeComboBox;
    }
}

