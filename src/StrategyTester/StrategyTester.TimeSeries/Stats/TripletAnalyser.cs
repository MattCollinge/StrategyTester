using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrategyTester.TimeSeries.Stats
{
    public class TripletAnalyser
    {
        class OHLCVIntervalDateComparer : IEqualityComparer<OHLCVInterval>
        {
            public bool Equals(OHLCVInterval x, OHLCVInterval y)
            {
                //Check whether the compared objects reference the same data.
                if (Object.ReferenceEquals(x, y)) return true;

                //Check whether any of the compared objects is null.
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;

                //Check whether the products' properties are equal.
                return x.DateTime == y.DateTime;
            }

            // If Equals() returns true for a pair of objects 
            // then GetHashCode() must return the same value for these objects.
            public int GetHashCode(OHLCVInterval obj)
            {
                //Check whether the object is null
                if (Object.ReferenceEquals(obj, null)) return 0;

                //Get hash code for the DateTime field if it is not null.
                return obj.DateTime == null ? 0 : obj.DateTime.GetHashCode();
            }
        }

        public List<double[]> PrepareInputSeries(IList<string> symbols, DateTime minDate, DateTime maxDate, IStoreOHLCVIntervals repository)
        {
            List<double[]> inputSeries = new List<double[]>();
            Dictionary<string, List<OHLCVInterval>> tmpData = new Dictionary<string, List<OHLCVInterval>>();

            //For each Symbol attempt to get data from minDate to maxDate
            //Work out the largest inclusive range
            long minDateTicks = 0;
            long maxDateTicks = long.MaxValue;
            foreach (var symbol in symbols)
            {
               List<OHLCVInterval> symbolSeries = repository.GetByTimeSpan(symbol, minDate, maxDate).ToList();

               tmpData.Add(symbol, symbolSeries);

               minDateTicks  = Math.Max(minDateTicks,symbolSeries.Min<OHLCVInterval>(interval => interval.DateTime.Ticks));
               maxDateTicks = Math.Min(maxDateTicks, symbolSeries.Max<OHLCVInterval>(interval => interval.DateTime.Ticks));

            }

            //combine in list
            foreach (var symbol in symbols)
            {
                inputSeries.Add(tmpData[symbol]
                    .Where<OHLCVInterval>(interval => interval.DateTime.Ticks >= minDateTicks && interval.DateTime.Ticks <= maxDateTicks)
                    .Select<OHLCVInterval, double>(interval => interval.Close)
                    .ToArray<double>()
                );
            }
            
            //return
            return inputSeries;
        }

        public double[] PerformAnalysis(List<double[]> johSeries)
       {
      
            int nlags = 5;
            List<MaxEigenData> outStats = null;
            double[] eigenValuesVec = null;
            double[,] eigenVecMatrix = null;
            JohansenHelper.DoMaxEigenValueTest(johSeries, nlags, out outStats,
                out eigenValuesVec, out eigenVecMatrix);
            double[] significantEvals = JohansenHelper.GetSignificantEvals(outStats,eigenValuesVec);

            //do not bother about cross section averaging
            if (significantEvals.Length == 0)
                return significantEvals;

            //average over the time cross sections
            List<List<double[]>> seriesList = SplitSeries(johSeries, 252);
            int noOfSeries = seriesList.Count;

            double[] cumEigValues = new double[significantEvals.Length];
            for (int i = 0; i < cumEigValues.Length; i++)
                cumEigValues[i] = 0;
            int tmpcounter = 0;
            for (int i = 0; i < noOfSeries; i++)
            {
                List<double[]> tmpSeries = seriesList[i];
                JohansenHelper.DoMaxEigenValueTest(tmpSeries, nlags, out outStats,
                    out eigenValuesVec, out eigenVecMatrix);
                for (int j = 0; j < cumEigValues.Length; j++)
                {
                    if ((eigenValuesVec[j] <= 0) || eigenValuesVec[j] > 1)
                    {
                        tmpcounter = tmpcounter;
                    }
                    else
                    {
                        cumEigValues[j] += eigenValuesVec[j];
                        tmpcounter++;
                    }
                }


                //ADFHelper.DoADFTest(tmpSeries, out dfStatistic,
                //    out pvalue, out lagOrderUsed);
                //cumPvalue += pvalue;
            }
            double[] avgEigenValue = new double[cumEigValues.Length];
            for (int i = 0; i < cumEigValues.Length; i++)
                avgEigenValue[i] = cumEigValues[i] / ((double)tmpcounter);
            //return cumPvalue / ((double)noOfSeries);
            
           return avgEigenValue;
       }

       List<List<double[]>> SplitSeries(List<double[]> series, int maxlen)
       {
           List<List<double[]>> retList = new List<List<double[]>>();
           //if (maxlen > series[0].Length)
           {
               retList.Add(series);
               return retList;
           }
           int segmentcount = (int)((double)series[0].Length / (double)maxlen);

           int offset = maxlen / 3;
           int offset1 = offset;
           int offset2 = 2 * offset;
           int nc = series.Count;
           int counter = 0;
           for (int i = 0; i < segmentcount; i++)
           {
               List<double[]> subList = new List<double[]>();
               List<double[]> offList1 = new List<double[]>();
               List<double[]> offList2 = new List<double[]>();
               for (int j = 0; j < nc; j++)
               {
                   subList.Add(new double[maxlen]);
                   offList1.Add(new double[maxlen]);
                   offList2.Add(new double[maxlen]);
               }

               //double[] offList1 = new double[maxlen];
               //double[] offList2 = new double[maxlen];
               for (int j = 0; j < maxlen; j++)
               {
                   for (int k = 0; k < nc; k++)
                   {
                       subList[k][j] = series[k][counter];
                       if (i < segmentcount - 1)
                       {
                           offList1[k][j] = series[k][counter + offset1];
                           offList2[k][j] = series[k][counter + offset2];
                       }
                   }
                   counter++;
               }
               retList.Add(subList);
               if (offList1[0].Length > 0)
                   retList.Add(offList1);
               if (offList2[0].Length > 0)
                   retList.Add(offList2);
           }
           return retList;
       }
    
    }
}
