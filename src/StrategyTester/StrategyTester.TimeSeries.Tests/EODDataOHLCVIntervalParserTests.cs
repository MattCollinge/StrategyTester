using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using StrategyTester.TimeSeries;

namespace StrategyTester.TimeSeries.Tests
{
   [TestFixture]
   public class EODDataOHLCVIntervalParserTests
    {
        [Test]
        public void Can_ParseFromString()
        {
            string testString = "ABL,02-Jan-96,21.75,21.95,21.55,21.70,12345";
            EODDataOHLVCIntervalParser parser = new EODDataOHLVCIntervalParser();
            OHLCVInterval OHLCV = parser.Parse(testString, 4);
           
            Assert.IsNotNull(OHLCV);
            Assert.AreEqual(new DateTime(1996, 01, 02), OHLCV.DateTime);
            Assert.IsFalse(Math.Abs(21.75 - OHLCV.Open) > double.Epsilon);
            Assert.IsFalse(Math.Abs(21.95 - OHLCV.High) > double.Epsilon);
            Assert.IsFalse(Math.Abs(21.55 - OHLCV.Low) > double.Epsilon);
            Assert.IsFalse(Math.Abs(21.70 - OHLCV.Close) > double.Epsilon);
            Assert.IsFalse(Math.Abs(12345 - OHLCV.Volume) > double.Epsilon);
            Assert.AreEqual(4, OHLCV.Index);
            Assert.AreEqual("ABL", OHLCV.Instrument);
            Assert.AreEqual("ABL" + new DateTime(1996, 01, 02).Ticks.ToString(), OHLCV.Id);

        }
    }
}
