using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrategyTester.TimeSeries
{
    public class OHLCVInterval
    {
        public string  Id { get; set; }
        public DateTime DateTime { get; set; }
        public Single Open { get; set; }
        public Single High { get; set; }
        public Single Low { get; set; }
        public Single Close { get; set; }
        public long Volume { get; set; }
        //public int OpenInterest { get; set; }

        public long Index { get; set; }
        //public string Interval { get; set; }
        public string Instrument { get; set; }
        //public string DataSource { get; set; }
       // public string Key { get { return String.Format("{0}{1}", Instrument, DateTime.ToShortDateString()); } }
    }
}
