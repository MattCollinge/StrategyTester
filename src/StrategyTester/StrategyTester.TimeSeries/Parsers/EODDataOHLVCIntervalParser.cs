using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrategyTester.TimeSeries
{
    public class EODDataOHLVCIntervalParser
    {
        const int instrument = 0, date = 1, open = 2, high = 3, low = 4, close = 5, vol = 6;

        public OHLCVInterval Parse(string rawData, int index)
        {
            OHLCVInterval newOHLCVInterval = new OHLCVInterval();

            string[] fields = rawData.Split(",".ToCharArray());
            newOHLCVInterval.DateTime = DateTime.Parse(fields[date]);
            newOHLCVInterval.Open = double.Parse(fields[open]);
            newOHLCVInterval.High = double.Parse(fields[high]);
            newOHLCVInterval.Low = double.Parse(fields[low]);
            newOHLCVInterval.Close = double.Parse(fields[close]);
            newOHLCVInterval.Volume = long.Parse(fields[vol]);
            newOHLCVInterval.Index = index;
            newOHLCVInterval.DataSource = "EODData";
            newOHLCVInterval.Instrument =fields[instrument];
            newOHLCVInterval.Id = newOHLCVInterval.Instrument + newOHLCVInterval.DateTime.Ticks.ToString();
            return newOHLCVInterval;
        }
    }
}
