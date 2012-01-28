using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrategyTester.TimeSeries
{
    public interface IParseIntervals
    {
         OHLCVInterval Parse(string stringToParse, int index);
       
    }
}
