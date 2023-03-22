using System;
using System.Collections.Generic;


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

            char[] quote_as_char_array = new char[quote.Length];
            // Copy character by character into array 
            for (int i = 0; i < quote.Length; i++)
            {
                quote_as_char_array[i] = quote[i];
            }

            for (int i = 0; i < numCols; i++)
            {
                if (rowToWrite[i].Contains(quote)) rowToWrite[i] = quote + rowToWrite[i].Replace(quote, escQuote).Trim().Trim(quote_as_char_array) + quote;
                if (rowToWrite[i].Contains(delim) || rowToWrite[i].Contains('\r') || rowToWrite[i].Contains('\n')) rowToWrite[i] = quote + rowToWrite[i].Trim().Trim(quote_as_char_array) + quote;
            }


            string cleanedRow = String.Join(delim, rowToWrite) + Environment.NewLine;

            return cleanedRow;
        }
    }


}

