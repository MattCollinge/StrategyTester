using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrategyTester.TimeSeries
{
    public interface IStoreOHLCVIntervals
    {
         IEnumerable<OHLCVInterval> GetByTimeSpan(string exchange, string instrument, DateTime from, DateTime to);
         void Save(OHLCVInterval intervalToSave);
    
    }
}
