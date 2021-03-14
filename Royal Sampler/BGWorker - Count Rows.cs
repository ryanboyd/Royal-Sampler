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
    public partial class RoyalSamplerForm
    {


        #region Just for Counting Rows
        private void backgroundWorker_CountRows(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

            TimeSpan reportPeriod = TimeSpan.FromMinutes(0.01);
            using (new System.Threading.Timer(
                           _ => (sender as BackgroundWorker).ReportProgress((int)((Homer)e.Argument).GetRowCount()), null, reportPeriod, reportPeriod))
            {
                e.Result = ((Homer)e.Argument).CountRows();
            }


            return;
        }

        private void backgroundWorker_CountRowsProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            StatusLabel.Text = "Counting rows... " + ToKMB((ulong)e.ProgressPercentage) + " thus far...";
        }

        private void backgroundWorker_CountRowsRunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

            FileDetails fdet = (FileDetails)e.Result;

            hoju.SetRowCount(fdet.totalNumberOfRows, fdet.rowErrorCount);
            hoju.SetColNames(fdet.colNames);

            foreach (string colName in hoju.GetColNames())
            {
                ColumnsToRetainCheckedListBox.Items.Add(colName);
                ColumnsToRetainCheckedListBox.SetItemChecked(ColumnsToRetainCheckedListBox.Items.IndexOf(colName), true);
            }


            EnableControls();
            StartButton.Enabled = true;
            DisableProgBar();
            StatusLabel.Text = "Dataset contains " + ToKMB(hoju.GetRowCount()) + " rows";

            MessageBox.Show("Your file has been scanned. It contains " + ToKMB(hoju.GetRowCount()) + " rows.", "Woohoo!", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        #endregion


    }


}

