using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrategyTester.TimeSeries;
using System.Diagnostics;

namespace EoDDataLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            HistoricalDataLoader loader = new HistoricalDataLoader();
            Console.WriteLine("Started Parsing Exchange Data");
            loader.EnumerateExchangesFromRootFolder(@"C:\EoD");
            sw.Stop();
            Console.WriteLine("Finished! Took: {0}", sw.ElapsedMilliseconds / 1000);
            Console.ReadLine();
        }
    }
}
