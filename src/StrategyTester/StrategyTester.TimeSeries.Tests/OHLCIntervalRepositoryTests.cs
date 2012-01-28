using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using StrategyTester.TimeSeries;

namespace StrategyTester.TimeSeries.Tests
{
    class OHLCIntervalRepositoryTests
    {
        [Test]
        public void CanSaveOHLCInterval()
        {
            OHLCVInterval newOHLCVInterval = new OHLCVInterval();

            newOHLCVInterval.DateTime = DateTime.Now;
            newOHLCVInterval.Open = 1234d;
            newOHLCVInterval.High =1235d;
            newOHLCVInterval.Low = 1233d;
            newOHLCVInterval.Close = 1234.5d;
            newOHLCVInterval.Volume = 1234567890;
            newOHLCVInterval.Index = 0;
            newOHLCVInterval.DataSource = "EODData";
            newOHLCVInterval.Instrument ="OHLCIntervalRepositoryTests";

            newOHLCVInterval.Id = newOHLCVInterval.Instrument + newOHLCVInterval.DateTime.Ticks.ToString();

            OHLCIntevalRepository repository = new OHLCIntevalRepository();
            repository.Save(newOHLCVInterval);
        }

        [Test]
        public void CanGetByTimeSpan()
        {

            OHLCIntevalRepository repository = new OHLCIntevalRepository();
          var intervals =  repository.GetByTimeSpan("OHLCIntervalRepositoryTests", DateTime.MinValue , DateTime.MaxValue);
           var intevalList = intervals.ToList<OHLCVInterval>();
           Assert.That(intevalList.Count > 0);
        }
    }
}
