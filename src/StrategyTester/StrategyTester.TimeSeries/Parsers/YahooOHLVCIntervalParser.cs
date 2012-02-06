using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrategyTester.TimeSeries
{
    public class YahooOHLVCIntervalParser : IParseIntervals
    {
        private readonly string instrument;
        const int date = 0, open = 1, high = 2, low = 3, close = 4, vol = 5;

        public YahooOHLVCIntervalParser(string instrument)
        {
            // TODO: Complete member initialization
            this.instrument = instrument;
        }

        public OHLCVInterval Parse(string rawData, int index)
        {
            OHLCVInterval newOHLCVInterval = new OHLCVInterval();

            string[] fields = rawData.Split(",".ToCharArray());
            newOHLCVInterval.DateTime = DateTime.Parse(fields[date]);
            newOHLCVInterval.Open = Single.Parse(fields[open]);
            newOHLCVInterval.High = Single.Parse(fields[high]);
            newOHLCVInterval.Low = Single.Parse(fields[low]);
            newOHLCVInterval.Close = Single.Parse(fields[close]);
            newOHLCVInterval.Volume = long.Parse(fields[vol]);
            newOHLCVInterval.Index = index;
            //newOHLCVInterval.DataSource = "Yahoo";
            newOHLCVInterval.Instrument = this.instrument;
            newOHLCVInterval.Id = newOHLCVInterval.Instrument + newOHLCVInterval.DateTime.ToShortDateString();

            return newOHLCVInterval;
        }
    }
}
