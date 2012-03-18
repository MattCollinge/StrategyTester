using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace StrategyTester.TimeSeries.Tests
{
   [TestFixture]
   public class YahooOHLCVIntervalParserTests
    {
        [Test]
        public void Can_ParseFromString()
        {
            string testString = "26/09/2008,1204.47,1215.77,1187.54,1213.27,5383610000,1213.27";
            YahooOHLVCIntervalParser parser = new YahooOHLVCIntervalParser("TestInstrument");
            OHLCVInterval OHLCV = parser.Parse(testString, 5);
           
            Assert.IsNotNull(OHLCV);
            Assert.AreEqual(new DateTime(2008, 09, 26), OHLCV.DateTime);
            Assert.IsFalse(Math.Abs(1204.47f - OHLCV.Open) > Single.Epsilon);
            Assert.IsFalse(Math.Abs(1215.77f - OHLCV.High) > Single.Epsilon);
            Assert.IsFalse(Math.Abs(1187.54f - OHLCV.Low) > Single.Epsilon);
            Assert.IsFalse(Math.Abs(1213.27f - OHLCV.Close) > Single.Epsilon);
            Assert.AreEqual(5383610000, OHLCV.Volume);
            Assert.AreEqual(5, OHLCV.Index);
            //Assert.AreEqual("Yahoo", OHLCV.DataSource);
            Assert.AreEqual("TestInstrument", OHLCV.Instrument);
           // Assert.AreEqual("TestInstrument" + new DateTime(2008, 09, 26).Ticks.ToString(), OHLCV.Id);

            

        }
    }
}
