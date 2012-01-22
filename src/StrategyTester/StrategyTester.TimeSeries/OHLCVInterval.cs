﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrategyTester.TimeSeries
{
    public class OHLCVInterval
    {
        public DateTime DateTime { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public long Volume { get; set; }
        //public int OpenInterest { get; set; }

        public int Index { get; set; }
        public string Interval { get; set; }
        public string Instrument { get; set; }
    }
}