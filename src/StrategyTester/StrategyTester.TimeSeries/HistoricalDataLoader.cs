using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;
using System.IO;
using System.Threading.Tasks;

namespace StrategyTester.TimeSeries
{
   public class HistoricalDataLoader
    {
       // Start from Directory root
       //Assume format:
       //+Root
       //++Exchange1
       //+++Yearly.zip
       //+++.
       //+++.
       //++Exhange2
       //+++Yearly.zip
       //+++.
       //+++ etc

       //Need to recurse through each Exhange Dir
       //Unzip each yearly file
       //Create OHLCVIntervalReader and enumerate each row into OHLCVInterval object
       //Save each OHLCVInterval with a OHLVCIntervalRepsository
       //Next file...

       public void EnumerateExchangesFromRootFolder(string rootFolder)
       {
           DirectoryInfo folder = new DirectoryInfo(rootFolder);
           Parallel.ForEach<DirectoryInfo>(folder.EnumerateDirectories(), exchange =>
           {
               //Enumerate Zip files...
               //  foreach (FileInfo yearlyData in exchange.EnumerateFiles())
               EnumerateYearlyDataForExchange(exchange);
           });
       }

       private void EnumerateYearlyDataForExchange(DirectoryInfo exchange)
       {
           Parallel.ForEach<FileInfo>(exchange.EnumerateFiles(), yearlyData =>
           {
               var targetFolder = exchange.CreateSubdirectory(yearlyData.Name + "Extracted").FullName;
               //ExtractZipFile;
               ExtractZipFile(yearlyData.FullName, targetFolder);
               //For each file in target folder
               EnumerateExtractedDataForYear(targetFolder);
           });
       }

       private void EnumerateExtractedDataForYear(string targetFolder)
       {
           foreach (var extractedFile in new DirectoryInfo(targetFolder).EnumerateFiles())
           {
               //Parse & Save OHLCVIntervals
               OHLCVIntervalReader reader = new OHLCVIntervalReader(
                   extractedFile.OpenText(),
                   new EODDataOHLVCIntervalParser(),
                   false);

               OHLCVIntervalRepository repository = new OHLCVIntervalRepository();

               foreach (var interval in reader)
               {
                   repository.Save(interval);
               }
           }
       }

       private void ExtractZipFile(string zipFile, string targetFolder)
       {
           using (ZipFile zip1 = ZipFile.Read(zipFile))
           {
              foreach (ZipEntry e in zip1)
               {
                   e.Extract(targetFolder, ExtractExistingFileAction.OverwriteSilently);
               }
           }
       }
    }
}
