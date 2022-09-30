# Royal Sampler
Takes a large CSV file and subsamples into separate files. Can create both random subsamples (with and without replacement) or "targeted" subsamples of a CSV file. Useful for working with very large datasets that you would like to split into chunks, randomly subsample, or "peek" at specific row ranges.

Compiled binary (and/or installer) available for download at https://github.com/ryanboyd/Royal-Sampler/releases

# Settings
There are a bunch of settings in the application that are — I think — fairly self-explanatory if you understand that the purpose of the software is to take a big CSV file, then chop it down into one or more smaller CSV files. However, these things are often not nearly as obvious as the programmer things they are :)

## CSV File Format Settings
There are several different formats for CSV files, and the structure of your CSV files may depend on your computer or the region of the world that your computer comes from. For example, despite the fact that the "C" in "CSV" stands for "Comma," it is not uncommon to find datasets that use semicolons instead (see ["Comma-separated values"](https://en.wikipedia.org/wiki/Comma-separated_values)). In this section of the application, you can change how *Royal Sampler* will read your file. Once you have your settings correctly set up, you should then use the "Open File" button to select the file that you would like to subsample.

![CSV Settings](https://github.com/ryanboyd/Royal-Sampler/blob/main/readme%20miscellany/csv_settings.png)

After choosing your file, the application will read through the entire dataset to make sure that there are no errors during processing, as well as learn some other pieces of information about your file that it will later use to help subsample your dataset (such as how many rows it has and the names of each column).

## Subsampling Settings

*Royal Sampler* can create subsamples of your dataset in three different ways:

1) Splitting your file into separate, equal-sized sections;
2) Retrieving a portion of your file (for example, rows 100 through 500);
3) Building randomized subsamples of your dataset.

First, you will want to select which method of subsampling you would like to accomplish. Once you have selected the appropriate mode, you will see the other options below change to suit the subsampling type.

![Subsampling Mode](https://github.com/ryanboyd/Royal-Sampler/blob/main/readme%20miscellany/subsampling_mode_settings.png)

The settings for #1 and #2 above are fairly straight-forward, so I will only provide additional details on the settings for #3 at the moment. (This is a very fancy way of saying that I don't have enough time to write additional instructions at the moment!)

### # of Subsample Files to Create

How many unique files would you like to generate for your subsamples? For example, if I want to create 50 subsamples of a really big dataset, each with 100,000 rows, then I would set the value for this setting to 50

![Number of Samples 50x100k](https://github.com/ryanboyd/Royal-Sampler/blob/main/readme%20miscellany/subsampling_50x100k.png)

However, if you don't *need* these to be split up into separate files and just want 5,000,000 rows of randomly subsampled data, you can just set this to "1" and use the subsequent setting to get all of the rows in the same file, e.g.,

![Number of Samples 1x5M](https://github.com/ryanboyd/Royal-Sampler/blob/main/readme%20miscellany/subsampling_1x5M.png)

### # of Rows per File

This is pretty much exactly what it sounds like. If the setting above determines the number of "samples" that you want to generate (i.e., separate files), this setting determines how many rows will be included in each file.

### Subsample with Replacement

For all of the bootstrapping nerds our there, you'll already know what this means. For everyone else: subsampling with replacement means that each row in your original dataset can be selected for subsampling an unlimited number of times. Let's say that you have a CSV file with 5 rows of data in it, something that looks like this:

|Name	|Age	|Gender	|# of Friends	|Has Pets?	|
|---	|---	|---	|---	|---	|
|Ryan	|100	|M	|4	|0	|
|Natalie	|39	|F	|1000	|0	|
|Olenka	|25	|F	|1354	|1	|
|Tabea	|25	|F	|1374	|1	|
|Andrea	|25	|F	|9000	|1	|

...and let's say that you want to "oversample" this file by creating a new dataset with 10000 rows. "Sampling with Replacement" means that this is possible, because every row of the dataset can be drawn into your new sample an infinite number of times.

If you *disable* this option, however, it is impossible to oversample. Each time you draw a row from the original dataset into a new sample, it "disappears" from the original sample and cannot be selected again. If you want to split your original sample into *X* number of subsamples, but you *do not* want duplicate entries, then you will want to disable this option.

### Randomization Seed
You may want to randomly subsample your original dataset, but in a replicable way. By entering a ["seed"](https://en.wikipedia.org/wiki/Random_seed) value here, you can ensure that the randomization will occur in the same way every time you process a given dataset.

Your seed value can be any integer between -2,147,483,648 and 2,147,483,647.
