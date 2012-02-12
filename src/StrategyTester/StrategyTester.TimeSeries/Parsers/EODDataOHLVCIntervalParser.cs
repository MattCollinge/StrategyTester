using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrategyTester.TimeSeries
{
    public class EODDataOHLVCIntervalParser :IParseIntervals 
    {
        private string exchange = String.Empty;
        const int instrument = 0, date = 1, open = 2, high = 3, low = 4, close = 5, vol = 6;

        public EODDataOHLVCIntervalParser(string exchange)
        {
            this.exchange = exchange;
        }

        public OHLCVInterval Parse(string rawData, int index)
        {
            OHLCVInterval newOHLCVInterval = new OHLCVInterval();
            DateTime parsedDate;

            string[] fields = rawData.Split(",".ToCharArray());

            if (!DateTime.TryParse(fields[date], out parsedDate))
                if (!DateTime.TryParseExact(fields[date], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out parsedDate))
                    throw new System.FormatException("String was not recognized as a valid DateTime.");

            newOHLCVInterval.DateTime = parsedDate.Date;
            newOHLCVInterval.Open = Single.Parse(fields[open]);
            newOHLCVInterval.High = Single.Parse(fields[high]);
            newOHLCVInterval.Low = Single.Parse(fields[low]);
            newOHLCVInterval.Close = Single.Parse(fields[close]);
            newOHLCVInterval.Volume = long.Parse(fields[vol]);
            newOHLCVInterval.Index = index;
            newOHLCVInterval.Exchange = exchange;
            
           // newOHLCVInterval.DataSource = "EODData";
            newOHLCVInterval.Instrument =fields[instrument];
            newOHLCVInterval.Id = String.Format("{0}{1}{2}", exchange, newOHLCVInterval.Instrument ,newOHLCVInterval.DateTime.ToShortDateString());
            return newOHLCVInterval;
        }
    }
}
