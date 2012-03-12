using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StrategyTester.TimeSeries.Stats;
using Rhino.Mocks;

namespace StrategyTester.TimeSeries.Tests
{
    [TestFixture]
    public class TripletAnalyserTests
    {
        [Test]
        [Ignore]
        public void PerformAnalysis()
        {
            TripletAnalyser tripletAnalyser = new TripletAnalyser();
            IList<string> symbolList = new List<string>();
            DateTime minDate = DateTime.MinValue;
            DateTime maxDate = DateTime.MaxValue;

            MockRepository mocks = new MockRepository();

            IStoreOHLCVIntervals intervalRepository =  mocks.Stub<IStoreOHLCVIntervals>();

            using (mocks.Record())
            {
                SetupResult
                    .For(intervalRepository.GetByTimeSpan("", minDate, maxDate))
                     .IgnoreArguments()
                    .Return(new List<OHLCVInterval>(){
                        new OHLCVInterval() 
                        {
                             Close = 1213.27f,
                             // DataSource="Stub",
                               DateTime=new DateTime(2008,09,26),
                                High=1215.77f,
                                 Index=0,
                                  Instrument="StubInstrument",
                                   //Interval="Day",
                                   Exchange="StubExchange",
                                     Low=1187.54f,
                                      Open=1204.47f,
                                       Volume=5383610000,
                                       //Id = Guid.NewGuid()//"StubInstrument" + new DateTime(2008,09,26).Ticks.ToString()
                        }
                    }
                        );
            }

            //tripletAnalyser.PerformAnalysis(symbolList, minDate, maxDate, intervalRepository);
        }

        [Test]
        public void PrepareInputSeries_TwoSeries_LastOne_EndsSooner()
        {
            TripletAnalyser tripletAnalyser = new TripletAnalyser();
            IList<string> symbolList = new List<string>();
            DateTime minDate = new DateTime(2000,1,1);
            DateTime maxDate =new DateTime(2001,1,1);
            string symbol = "Symbol_1";
            
            IStoreOHLCVIntervals intervalRepository = MockRepository.GenerateStub<IStoreOHLCVIntervals>();

            intervalRepository.Stub(r => r.GetByTimeSpan(symbol, minDate, maxDate))
                .Return(GenerateInputSeriesList(symbol, minDate, maxDate.Subtract(minDate).Days));

           symbol = "Symbol_2";
           //maxDate = maxDate.AddDays(-10);
           intervalRepository.Stub(r => r.GetByTimeSpan(symbol, minDate, maxDate))
                .Return(GenerateInputSeriesList(symbol, minDate, maxDate.AddDays(-10).Subtract(minDate).Days));

           List<double[]> intersection = tripletAnalyser.PrepareInputSeries(new List<string>() { "Symbol_1", "Symbol_2" }, minDate, maxDate, intervalRepository);

           Assert.AreEqual(2, intersection.Count);
           Assert.AreEqual(356, intersection[0].Length);
           Assert.AreEqual(356, intersection[1].Length);
           Assert.LessOrEqual(Double.Epsilon, Math.Abs(minDate.Ticks - intersection[0][0]));
           Assert.LessOrEqual(Double.Epsilon, Math.Abs(maxDate.AddDays(-10).Ticks - intersection[0][355]));
          
        
        }

        [Test]
        public void PrepareInputSeries_TwoSeries_FirstOne_EndsSooner()
        {
            TripletAnalyser tripletAnalyser = new TripletAnalyser();
            IList<string> symbolList = new List<string>();
            DateTime minDate = new DateTime(2000, 1, 1);
            DateTime maxDate = new DateTime(2001, 1, 1);
            string symbol = "Symbol_1";

            IStoreOHLCVIntervals intervalRepository = MockRepository.GenerateStub<IStoreOHLCVIntervals>();

            intervalRepository.Stub(r => r.GetByTimeSpan(symbol, minDate, maxDate))
                .Return(GenerateInputSeriesList(symbol, minDate, maxDate.Subtract(minDate).Days));

            symbol = "Symbol_2";
            intervalRepository.Stub(r => r.GetByTimeSpan(symbol, minDate, maxDate))
                 .Return(GenerateInputSeriesList(symbol, minDate, maxDate.AddDays(10).Subtract(minDate).Days));

            List<double[]> intersection = tripletAnalyser.PrepareInputSeries(new List<string>() { "Symbol_1", "Symbol_2" }, minDate, maxDate, intervalRepository);

            Assert.AreEqual(2, intersection.Count);
            Assert.AreEqual(366, intersection[0].Length);
            Assert.AreEqual(366, intersection[1].Length);
            Assert.LessOrEqual(Double.Epsilon, Math.Abs(minDate.Ticks - intersection[0][0]));
            Assert.LessOrEqual(Double.Epsilon, Math.Abs(maxDate.Ticks - intersection[0][355]));
         
        }

        [Test]
        public void PrepareInputSeries_TwoSeries_FirstOne_StartsLater()
        {
            TripletAnalyser tripletAnalyser = new TripletAnalyser();
            IList<string> symbolList = new List<string>();
            DateTime minDate = new DateTime(2000, 1, 1);
            DateTime maxDate = new DateTime(2001, 1, 1);
            string symbol = "Symbol_1";

            IStoreOHLCVIntervals intervalRepository = MockRepository.GenerateStub<IStoreOHLCVIntervals>();

            intervalRepository.Stub(r => r.GetByTimeSpan(symbol, minDate, maxDate))
                .Return(GenerateInputSeriesList(symbol, minDate.AddDays(10), maxDate.Subtract(minDate).Days));

            symbol = "Symbol_2";
            intervalRepository.Stub(r => r.GetByTimeSpan(symbol, minDate, maxDate))
                 .Return(GenerateInputSeriesList(symbol, minDate, maxDate.Subtract(minDate).Days));

            List<double[]> intersection = tripletAnalyser.PrepareInputSeries(new List<string>() { "Symbol_1", "Symbol_2" }, minDate, maxDate, intervalRepository);

            Assert.AreEqual(2, intersection.Count);
            Assert.AreEqual(356, intersection[0].Length);
            Assert.AreEqual(356, intersection[1].Length);
            Assert.LessOrEqual(Double.Epsilon, Math.Abs(minDate.AddDays(10).Ticks - intersection[0][0]));
            Assert.LessOrEqual(Double.Epsilon, Math.Abs(maxDate.Ticks - intersection[0][355]));
         
        }

        [Test]
        public void PrepareInputSeries_TwoSeries_LastOne_StartsLater()
        {
            TripletAnalyser tripletAnalyser = new TripletAnalyser();
            IList<string> symbolList = new List<string>();
            DateTime minDate = new DateTime(2000, 1, 1);
            DateTime maxDate = new DateTime(2001, 1, 1);
            string symbol = "Symbol_1";

            IStoreOHLCVIntervals intervalRepository = MockRepository.GenerateStub<IStoreOHLCVIntervals>();

            intervalRepository.Stub(r => r.GetByTimeSpan(symbol, minDate, maxDate))
                .Return(GenerateInputSeriesList(symbol, minDate, maxDate.Subtract(minDate).Days));

            symbol = "Symbol_2";
            intervalRepository.Stub(r => r.GetByTimeSpan(symbol, minDate, maxDate))
                 .Return(GenerateInputSeriesList(symbol, minDate.AddDays(10), maxDate.Subtract(minDate).Days));

            List<double[]> intersection = tripletAnalyser.PrepareInputSeries(new List<string>() { "Symbol_1", "Symbol_2" }, minDate, maxDate, intervalRepository);

            Assert.AreEqual(2, intersection.Count);
            Assert.AreEqual(356, intersection[0].Length);
            Assert.AreEqual(356, intersection[1].Length);
            Assert.LessOrEqual(Double.Epsilon, Math.Abs(minDate.AddDays(10).Ticks - intersection[0][0]));
            Assert.LessOrEqual(Double.Epsilon, Math.Abs(maxDate.Ticks - intersection[0][355]));
         
        }



        private static List<OHLCVInterval> GenerateInputSeriesList(string symbol, DateTime minDate, int count)
        {
            List<OHLCVInterval> list =  new List<OHLCVInterval>();
            
            for(int i=0;i<count;i++)
            {
              list.Add(new OHLCVInterval() 
                        {
                            Instrument=symbol,
                            Close = float.Parse(minDate.AddDays(i).Ticks.ToString()),
                               DateTime=minDate.AddDays(i),
                         });
            }

            return list;
        }
    }
}
