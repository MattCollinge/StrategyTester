﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;
using System.IO;
using System.Threading.Tasks;
using log4net;
using System.Diagnostics;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace StrategyTester.TimeSeries
{
    public class HistoricalDataLoader
    {
       private ILog logger = LogManager.GetLogger(typeof(HistoricalDataLoader));
              
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
           //Parallel.ForEach<DirectoryInfo>(folder.EnumerateDirectories(), exchange =>
           foreach(DirectoryInfo exchange in folder.EnumerateDirectories())
           {
               //Enumerate Zip files...
               //  foreach (FileInfo yearlyData in exchange.EnumerateFiles())
               EnumerateYearlyDataForExchange(exchange);
           }
           //});
       }

       private void EnumerateYearlyDataForExchange(DirectoryInfo exchange)
       {
           Stopwatch stopwatch = new Stopwatch();
           
           foreach(FileInfo yearlyData  in exchange.EnumerateFiles("*.zip"))
           {
               stopwatch.Reset();
               stopwatch.Start();
             
               var targetFolder = yearlyData.FullName + "Extracted";
             
               if (!Directory.Exists(targetFolder))
               {
                   ExtractZipFile(yearlyData.FullName, targetFolder);
               }

             int fileCount =  EnumerateExtractedDataForYear(targetFolder, exchange.Name );
                stopwatch.Stop();
               Console.WriteLine("Finished parsing {0} files in folder: {1} in: {2} seconds.", fileCount, targetFolder, stopwatch.ElapsedMilliseconds / 1000);
          
           }
       }

       private int EnumerateExtractedDataForYear(string targetFolder, string exchange)
       {
           //  Stopwatch stopwatch = new Stopwatch();
           int fileCount = 0;
           foreach (var extractedFile in new DirectoryInfo(targetFolder).EnumerateFiles())
           {
               fileCount++;
               //stopwatch.Reset();
               // stopwatch.Start();
               //Parse & Save OHLCVIntervals
               OHLCVIntervalReader reader = new OHLCVIntervalReader(
                   extractedFile.OpenText(),
                   new EODDataOHLVCIntervalParser(exchange),
                   false);

               OHLCVIntervalRepository repository = new OHLCVIntervalRepository();
               int lineCount = 0;
               try
               {
                   foreach (var interval in reader)
                   {
                       lineCount++;
                       repository.Save(interval);
                   }
               }
               catch (Exception e)
               {
                   logger.ErrorFormat("Parser Error in file: {0}, @line: {1}, exception: {2}", extractedFile.Name, lineCount, e.Message);
               }
               logger.InfoFormat("Parsed file: {0}, sucessfully parsed  {1} lines", extractedFile.Name, lineCount);

               // stopwatch.Stop();
               // Console.WriteLine("Finished parsing file: {0} in: {1} seconds.", extractedFile.Name,stopwatch.ElapsedMilliseconds/1000);
           }
           return fileCount;
       }

       private void ExtractZipFile(string zipFile, string targetFolder)
       {
           try
           {
               using (ZipFile zip1 = ZipFile.Read(zipFile))
               {
                   foreach (ZipEntry e in zip1)
                   {
                       e.Extract(targetFolder, ExtractExistingFileAction.OverwriteSilently);
                   }
               }
           }
           catch (Ionic.Zip.ZipException e)
           {
               logger.Error(string.Format("Error extracting {0}, {1}, {2}", zipFile, e.Message, e.ToString()));
           }
       }
    }
}
