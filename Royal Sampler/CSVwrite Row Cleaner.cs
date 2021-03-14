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


    public static class RowCleaner
    {
        public static string CleanRow(string[] rowIn, char delim, string quote, string escQuote, int numCols, HashSet<int> retainedIndices)
        {

        
            string[] rowToWrite = new string[numCols];
            
            int outputRowIndexCounter = 0;
            for (int index = 0; index < rowIn.Length; index++)
            {
                if (retainedIndices.Contains(index))
                {
                    rowToWrite[outputRowIndexCounter] = rowIn[index];
                    outputRowIndexCounter++;
                }
            }


            for (int i = 0; i < numCols; i++)
            {
                if (rowToWrite[i].Contains(quote)) rowToWrite[i] = rowToWrite[i].Replace(quote, escQuote);
                if (rowToWrite[i].Contains(delim) || rowToWrite[i].Contains('\r') || rowToWrite[i].Contains('\n')) rowToWrite[i] = quote + rowToWrite[i] + quote;
            }


            string cleanedRow = String.Join(delim, rowToWrite) + Environment.NewLine;

            return cleanedRow;
        }
    }


}

