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
            newOHLCVInterval.Open = 1234f;
            newOHLCVInterval.High =1235f;
            newOHLCVInterval.Low = 1233f;
            newOHLCVInterval.Close = 1234.5f;
            newOHLCVInterval.Volume = 1234567890;
            newOHLCVInterval.Index = 0;
            //newOHLCVInterval.DataSource = "EODData";
            newOHLCVInterval.Instrument ="OHLCIntervalRepositoryTests";
            newOHLCVInterval.Exchange = "FakeExchange";
          //  newOHLCVInterval.Id = Guid.NewGuid();// newOHLCVInterval.Instrument + newOHLCVInterval.DateTime.Ticks.ToString();

            OHLCVIntervalRepository repository = new OHLCVIntervalRepository();
            repository.Save(newOHLCVInterval);
        }

        [Test]
        public void CanGetByTimeSpan()
        {

            OHLCVIntervalRepository repository = new OHLCVIntervalRepository();
          var intervals =  repository.GetByTimeSpan("FakeExchange","OHLCIntervalRepositoryTests", DateTime.Now.AddDays(-9) , DateTime.Now);
           var intevalList = intervals.ToList<OHLCVInterval>();
           Assert.That(intevalList.Count > 0);
        }
    }
}
