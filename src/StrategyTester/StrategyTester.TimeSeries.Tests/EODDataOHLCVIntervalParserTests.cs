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
            string exchange = "TestExchange";
            EODDataOHLVCIntervalParser parser = new EODDataOHLVCIntervalParser(exchange);
            OHLCVInterval OHLCV = parser.Parse(testString, 4);
           
            Assert.IsNotNull(OHLCV);
            Assert.AreEqual(new DateTime(1996, 01, 02), OHLCV.DateTime);
            Assert.IsFalse(Math.Abs(21.75f - OHLCV.Open) > Single.Epsilon);
            Assert.IsFalse(Math.Abs(21.95f - OHLCV.High) > Single.Epsilon);
            Assert.IsFalse(Math.Abs(21.55f - OHLCV.Low) > Single.Epsilon);
            Assert.IsFalse(Math.Abs(21.70f - OHLCV.Close) > Single.Epsilon);
            Assert.AreEqual(12345, OHLCV.Volume);
            Assert.AreEqual(4, OHLCV.Index);
            Assert.AreEqual("ABL", OHLCV.Instrument);
            Assert.AreEqual(exchange + "ABL" + new DateTime(1996, 01, 02).ToShortDateString(), OHLCV.Id);

        }

        [Test]
        public void Can_ParseFromStringWithAltDateFormat()
        {
            string testString = "ABL,20110107,21.75,21.95,21.55,21.70,12345";
            string exchange = "TestExchange";
            EODDataOHLVCIntervalParser parser = new EODDataOHLVCIntervalParser(exchange);
           OHLCVInterval OHLCV = parser.Parse(testString, 4);

            Assert.IsNotNull(OHLCV);
            Assert.AreEqual(new DateTime(2011, 01, 07), OHLCV.DateTime);
            Assert.IsFalse(Math.Abs(21.75f - OHLCV.Open) > Single.Epsilon);
            Assert.IsFalse(Math.Abs(21.95f - OHLCV.High) > Single.Epsilon);
            Assert.IsFalse(Math.Abs(21.55f - OHLCV.Low) > Single.Epsilon);
            Assert.IsFalse(Math.Abs(21.70f - OHLCV.Close) > Single.Epsilon);
            Assert.AreEqual(12345, OHLCV.Volume);
            Assert.AreEqual(4, OHLCV.Index);
            Assert.AreEqual("ABL", OHLCV.Instrument);
            Assert.AreEqual(exchange + "ABL" + new DateTime(2011, 01, 07).ToShortDateString(), OHLCV.Id);

        }

        [Test]
        public void Can_ParseFromStringWithAltDateFormat2()
        {
            string testString = "ABL,20110301,21.75,21.95,21.55,21.70,12345";
            string exchange = "TestExchange";
            EODDataOHLVCIntervalParser parser = new EODDataOHLVCIntervalParser(exchange);
            OHLCVInterval OHLCV = parser.Parse(testString, 4);

            Assert.IsNotNull(OHLCV);
            Assert.AreEqual(new DateTime(2011, 03, 01), OHLCV.DateTime);
            Assert.IsFalse(Math.Abs(21.75f - OHLCV.Open) > Single.Epsilon);
            Assert.IsFalse(Math.Abs(21.95f - OHLCV.High) > Single.Epsilon);
            Assert.IsFalse(Math.Abs(21.55f - OHLCV.Low) > Single.Epsilon);
            Assert.IsFalse(Math.Abs(21.70f - OHLCV.Close) > Single.Epsilon);
            Assert.AreEqual(12345, OHLCV.Volume);
            Assert.AreEqual(4, OHLCV.Index);
            Assert.AreEqual("ABL", OHLCV.Instrument);
            Assert.AreEqual(exchange + "ABL" + new DateTime(2011, 03, 01).ToShortDateString(), OHLCV.Id);

        }
    }
}
