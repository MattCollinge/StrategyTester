using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;
using System.IO;

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

       private void EnumerateExchangesFromRootFolder(string rootFolder)
       {
           DirectoryInfo folder = new DirectoryInfo(rootFolder);
           foreach(DirectoryInfo exchange in folder.EnumerateDirectories())
           {
                //Enumerate Zip files...
               foreach (FileInfo yearlyData in exchange.EnumerateFiles())
               { 
                   //ExtractZipFile;
                   //Parse & Save OHLCVIntervals
                   
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
