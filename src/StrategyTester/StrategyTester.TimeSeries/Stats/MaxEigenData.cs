using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrategyTester.TimeSeries.Stats
{
    public class MaxEigenData
    {
        public int No { get; set; }
        public double TestStatistic { get; set; }
        public double CriticalValue90 { get; set; }
        public double CriticalValue95 { get; set; }
        public double CriticalValue99 { get; set; }
    }
}
