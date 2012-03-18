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
        public void PerformAnalysis()
        {
          TripletAnalyser tripletAnalyser = new TripletAnalyser();
           
          MaxEigenResults result =  tripletAnalyser.PerformAnalysis(GetJohnsenSeries());
           
        }

        private List<double[]> GetJohnsenSeries()
        {
            int seriesLength = 500;
            List<double[]> johnsenSeries = new List<double[]>();
            johnsenSeries.Add(new double[seriesLength]);
            johnsenSeries.Add(new double[seriesLength]);
            johnsenSeries.Add(new double[seriesLength]);
            
            Random rnd = new Random();
           double datapoint = 0;

            //Random walk
            for (int i = 0; i < seriesLength; i++)
            {
              datapoint = rnd.NextDouble() + -1*(int)Math.Round(rnd.NextDouble());
                
                if(i>0)
                    datapoint = johnsenSeries[0][i-1] + datapoint;

                johnsenSeries[0][i] = datapoint;
            }

            //Random Walk
            for (int i = 0; i < seriesLength; i++)
            {
                datapoint = rnd.NextDouble() + -1 * (int)Math.Round(rnd.NextDouble());

                if (i > 0)
                    datapoint = johnsenSeries[1][i - 1] + datapoint;

                johnsenSeries[1][i] = datapoint;
            }

            //Random walk + 0.3 of johansenSeries[0]
            for (int i = 0; i < seriesLength; i++)
            {
                datapoint = rnd.NextDouble() + -1 * (int)Math.Round(rnd.NextDouble());

                datapoint = datapoint + 0.3 * johnsenSeries[0][i];

                johnsenSeries[2][i] = datapoint;
            }
            return johnsenSeries;
        }

        [Test]
        public void PrepareInputSeries_TwoSeries_LastOne_EndsSooner()
        {
            TripletAnalyser tripletAnalyser = new TripletAnalyser();
            IList<string> symbolList = new List<string>();
            DateTime minDate = new DateTime(2000,1,1);
            DateTime maxDate =new DateTime(2001,1,1);
            string exchange = "Exchange";
            string symbol = "Symbol_1";
            
            IStoreOHLCVIntervals intervalRepository = MockRepository.GenerateStub<IStoreOHLCVIntervals>();

            intervalRepository.Stub(r => r.GetByTimeSpan(exchange,symbol, minDate, maxDate))
                .Return(GenerateInputSeriesList(symbol, minDate, maxDate.Subtract(minDate).Days));

           symbol = "Symbol_2";
           //maxDate = maxDate.AddDays(-10);
           intervalRepository.Stub(r => r.GetByTimeSpan(exchange, symbol, minDate, maxDate))
                .Return(GenerateInputSeriesList(symbol, minDate, maxDate.AddDays(-10).Subtract(minDate).Days));

           List<double[]> intersection = tripletAnalyser.PrepareInputSeries(new List<string>() { "Exchange:Symbol_1", "Exchange:Symbol_2" }, minDate, maxDate, intervalRepository);

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
            string exchange = "Exchange";
            string symbol = "Symbol_1";

            IStoreOHLCVIntervals intervalRepository = MockRepository.GenerateStub<IStoreOHLCVIntervals>();

            intervalRepository.Stub(r => r.GetByTimeSpan(exchange,symbol, minDate, maxDate))
                .Return(GenerateInputSeriesList(symbol, minDate, maxDate.Subtract(minDate).Days));

            symbol = "Symbol_2";
            intervalRepository.Stub(r => r.GetByTimeSpan(exchange, symbol, minDate, maxDate))
                 .Return(GenerateInputSeriesList(symbol, minDate, maxDate.AddDays(10).Subtract(minDate).Days));

            List<double[]> intersection = tripletAnalyser.PrepareInputSeries(new List<string>() { "Exchange:Symbol_1", "Exchange:Symbol_2" }, minDate, maxDate, intervalRepository);

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
            string exchange = "Exchange";
            string symbol = "Symbol_1";

            IStoreOHLCVIntervals intervalRepository = MockRepository.GenerateStub<IStoreOHLCVIntervals>();

            intervalRepository.Stub(r => r.GetByTimeSpan(exchange,symbol, minDate, maxDate))
                .Return(GenerateInputSeriesList(symbol, minDate.AddDays(10), maxDate.Subtract(minDate).Days));

            symbol = "Symbol_2";
            intervalRepository.Stub(r => r.GetByTimeSpan(exchange, symbol, minDate, maxDate))
                 .Return(GenerateInputSeriesList(symbol, minDate, maxDate.Subtract(minDate).Days));

            List<double[]> intersection = tripletAnalyser.PrepareInputSeries(new List<string>() { "Exchange:Symbol_1", "Exchange:Symbol_2" }, minDate, maxDate, intervalRepository);

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
            string exchange = "Exchange";
            string symbol = "Symbol_1";

            IStoreOHLCVIntervals intervalRepository = MockRepository.GenerateStub<IStoreOHLCVIntervals>();

            intervalRepository.Stub(r => r.GetByTimeSpan(exchange,symbol, minDate, maxDate))
                .Return(GenerateInputSeriesList(symbol, minDate, maxDate.Subtract(minDate).Days));

            symbol = "Symbol_2";
            intervalRepository.Stub(r => r.GetByTimeSpan(exchange,symbol, minDate, maxDate))
                 .Return(GenerateInputSeriesList(symbol, minDate.AddDays(10), maxDate.Subtract(minDate).Days));

            List<double[]> intersection = tripletAnalyser.PrepareInputSeries(new List<string>() { "Exchange:Symbol_1", "Exchange:Symbol_2" }, minDate, maxDate, intervalRepository);

            Assert.AreEqual(2, intersection.Count);
            Assert.AreEqual(356, intersection[0].Length);
            Assert.AreEqual(356, intersection[1].Length);
            Assert.LessOrEqual(Double.Epsilon, Math.Abs(minDate.AddDays(10).Ticks - intersection[0][0]));
            Assert.LessOrEqual(Double.Epsilon, Math.Abs(maxDate.Ticks - intersection[0][355]));
         
        }

        [Test]
        public void PrepareInputSeries_ThreeSeries_LastOne_StartsLater()
        {
            TripletAnalyser tripletAnalyser = new TripletAnalyser();
            IList<string> symbolList = new List<string>();
            DateTime minDate = new DateTime(2000, 1, 1);
            DateTime maxDate = new DateTime(2001, 1, 1);
            string exchange = "Exchange";
            string symbol = "Symbol_1";

            IStoreOHLCVIntervals intervalRepository = MockRepository.GenerateStub<IStoreOHLCVIntervals>();

            intervalRepository.Stub(r => r.GetByTimeSpan(exchange, symbol, minDate, maxDate))
                .Return(GenerateInputSeriesList(symbol, minDate, maxDate.Subtract(minDate).Days));

            symbol = "Symbol_2";
            intervalRepository.Stub(r => r.GetByTimeSpan(exchange, symbol, minDate, maxDate))
                 .Return(GenerateInputSeriesList(symbol, minDate, maxDate.Subtract(minDate).Days));
           
            symbol = "Symbol_3";
            intervalRepository.Stub(r => r.GetByTimeSpan(exchange, symbol, minDate, maxDate))
                 .Return(GenerateInputSeriesList(symbol, minDate.AddDays(10), maxDate.Subtract(minDate).Days));

            List<double[]> intersection = tripletAnalyser.PrepareInputSeries(new List<string>() { "Exchange:Symbol_1", "Exchange:Symbol_2", "Exchange:Symbol_3" }, minDate, maxDate, intervalRepository);

            Assert.AreEqual(3, intersection.Count);
            Assert.AreEqual(356, intersection[0].Length);
            Assert.AreEqual(356, intersection[1].Length);
            Assert.AreEqual(356, intersection[2].Length);
            Assert.LessOrEqual(Double.Epsilon, Math.Abs(minDate.AddDays(10).Ticks - intersection[0][0]));
            Assert.LessOrEqual(Double.Epsilon, Math.Abs(maxDate.Ticks - intersection[0][355]));

        }

        [Test]
        [Ignore]
        public void SplitSeriesTest()
        {
          
            TripletAnalyser tripletAnalyser = new TripletAnalyser();
            List<double[]> seriesList = new List<double[]>();

            seriesList.Add(new double[52]);
            seriesList.Add(new double[52]);
            seriesList.Add(new double[52]);

            for (int i = 0; i < 52; i++)
            {
                seriesList[0][i] = i;
                seriesList[1][i] = i * 100;
                seriesList[2][i] = i * 1000;

            }

           List<List<double[]>> splitseries = tripletAnalyser.SplitSeries(seriesList, 10);
           Assert.AreEqual(6, splitseries.Count);

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
