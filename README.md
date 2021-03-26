# Royal Sampler
Takes a large CSV file and subsamples into separate files. Can create both random subsamples (with and without replacement) or "targeted" subsamples of a CSV file. Useful for working with very large datasets that you would like to split into chunks, randomly subsample, or "peek" at specific row ranges.

Compiled binary (and/or installer) available for download at https://www.ryanboyd.io/software/toolbox/#RoyalSampler

# Settings
There are a bunch of settings in the application that are — I think — fairly self-explanatory if you understand that the purpose of the software is to take a big CSV file, then chop it down into one or more smaller CSV files. However, these things are often not nearly as obvious as the programmer things they are :)

## CSV File Format Settings
There are several different formats for CSV files, and the structure of your CSV files may depend on your computer or the region of the world that your computer comes from. For example, despite the fact that the "C" in "CSV" stands for "Comma," it is not uncommon to find datasets that use semicolons instead (see ["Comma-separated values"](https://en.wikipedia.org/wiki/Comma-separated_values)).

![CSV Settings](https://github.com/ryanboyd/Royal-Sampler/blob/main/readme%20miscellany/csv_settings.png)