using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using StrategyTester.TimeSeries.Stats;

namespace StrategyTester.TimeSeries.Tests
{
    [TestFixture]
    public class JohansenHelperTests
    {
        double[,] criticalEigenValues = new double[,]{
            {2.7055,	3.8415,	    6.6349},
            {12.2971,	14.2639,	18.52},
            {18.8928,	21.1314,    25.865},
            {25.1236,	27.5858,	32.7172},
            {31.2379,	33.8777,	39.3693},
            {37.2786,	40.0763,	45.8662},
            {43.2947,	46.2299,	52.3069},
            {49.2855,	52.3622,	58.6634},
            {55.2412,	58.4332,	64.996},
            {61.2041,	64.504,	    71.2525},
            {67.1307,	70.5392,	77.4877},
            {73.0563,	76.5734,	83.7105}};

        [Test]
        public void GetSignificantEigenValuesFor2EigenStats()
        {
            List<MaxEigenData> testEigenStats = new List<MaxEigenData>() 
            { 
                new MaxEigenData(){ 
                     No="2", 
                    CriticalValue90=criticalEigenValues[0,0].ToString(), 
                    CriticalValue95=criticalEigenValues[0,1].ToString(),
                    CriticalValue99=criticalEigenValues[0,2].ToString(),
                    TestStatistic="128.3"
                },
                new MaxEigenData(){ 
                    No="1", 
                    CriticalValue90=criticalEigenValues[1,0].ToString(), 
                    CriticalValue95=criticalEigenValues[1,1].ToString(),
                    CriticalValue99=criticalEigenValues[1,2].ToString(),
                    TestStatistic="16.4"
                }
            };

            double[] testEigenValueVectors = new double[]{
                0.2,
                0.3
            };

          double[] significantEigenValues = JohansenHelper.GetSignificantEvals(testEigenStats, testEigenValueVectors);

          Assert.AreEqual(1, significantEigenValues.Length);

        }

        [Test]
        public void GetmaxEigenSummaryTextFor2Series0CointVector()
        {
            List<MaxEigenData> testEigenStats = new List<MaxEigenData>() 
            { 
                new MaxEigenData(){ 
                     No="0", 
                    CriticalValue90=criticalEigenValues[0,0].ToString(), 
                    CriticalValue95=criticalEigenValues[0,1].ToString(),
                    CriticalValue99=criticalEigenValues[0,2].ToString(),
                    TestStatistic="3.1"
                },
                new MaxEigenData(){ 
                    No="1", 
                    CriticalValue90=criticalEigenValues[1,0].ToString(), 
                    CriticalValue95=criticalEigenValues[1,1].ToString(),
                    CriticalValue99=criticalEigenValues[1,2].ToString(),
                    TestStatistic="11.4"
                }
            };

            string result = JohansenHelper.GetmaxEigenSummaryText(testEigenStats);

            Assert.AreEqual("No of cointegrating vectors is 0", result);
        }
        
        [Test]
        public void GetmaxEigenSummaryTextFor2Series1CointVector()
        {
            List<MaxEigenData> testEigenStats = new List<MaxEigenData>() 
            { 
                new MaxEigenData(){ 
                     No="0", 
                    CriticalValue90=criticalEigenValues[0,0].ToString(), 
                    CriticalValue95=criticalEigenValues[0,1].ToString(),
                    CriticalValue99=criticalEigenValues[0,2].ToString(),
                    TestStatistic="128.3"
                },
                new MaxEigenData(){ 
                    No="1", 
                    CriticalValue90=criticalEigenValues[1,0].ToString(), 
                    CriticalValue95=criticalEigenValues[1,1].ToString(),
                    CriticalValue99=criticalEigenValues[1,2].ToString(),
                    TestStatistic="11.4"
                }
            };

            string result = JohansenHelper.GetmaxEigenSummaryText(testEigenStats);

            Assert.AreEqual("No of cointegrating vectors is 1", result);
        }

        [Test]
        public void GetmaxEigenSummaryTextFor3Series0CointVector()
        {
            List<MaxEigenData> testEigenStats = new List<MaxEigenData>() 
            { 
                new MaxEigenData(){ 
                     No="0", 
                    CriticalValue90=criticalEigenValues[0,0].ToString(), 
                    CriticalValue95=criticalEigenValues[0,1].ToString(),
                    CriticalValue99=criticalEigenValues[0,2].ToString(),
                    TestStatistic="3"
                },
                new MaxEigenData(){ 
                     No="1", 
                    CriticalValue90=criticalEigenValues[1,0].ToString(), 
                    CriticalValue95=criticalEigenValues[1,1].ToString(),
                    CriticalValue99=criticalEigenValues[1,2].ToString(),
                    TestStatistic="8.4"
                },
                new MaxEigenData(){ 
                    No="2", 
                    CriticalValue90=criticalEigenValues[2,0].ToString(), 
                    CriticalValue95=criticalEigenValues[2,1].ToString(),
                    CriticalValue99=criticalEigenValues[2,2].ToString(),
                    TestStatistic="11.4"
                }
            };

            string result = JohansenHelper.GetmaxEigenSummaryText(testEigenStats);

            Assert.AreEqual("No of cointegrating vectors is 0", result);
        }
       
        [Test]
        public void GetmaxEigenSummaryTextFor3Series1CointVector()
        {
            List<MaxEigenData> testEigenStats = new List<MaxEigenData>() 
            { 
                new MaxEigenData(){ 
                     No="0", 
                    CriticalValue90=criticalEigenValues[0,0].ToString(), 
                    CriticalValue95=criticalEigenValues[0,1].ToString(),
                    CriticalValue99=criticalEigenValues[0,2].ToString(),
                    TestStatistic="128.3"
                },
                new MaxEigenData(){ 
                     No="1", 
                    CriticalValue90=criticalEigenValues[1,0].ToString(), 
                    CriticalValue95=criticalEigenValues[1,1].ToString(),
                    CriticalValue99=criticalEigenValues[1,2].ToString(),
                    TestStatistic="11.4"
                },
                new MaxEigenData(){ 
                    No="2", 
                    CriticalValue90=criticalEigenValues[2,0].ToString(), 
                    CriticalValue95=criticalEigenValues[2,1].ToString(),
                    CriticalValue99=criticalEigenValues[2,2].ToString(),
                    TestStatistic="11.4"
                }
            };

            string result = JohansenHelper.GetmaxEigenSummaryText(testEigenStats);

            Assert.AreEqual("No of cointegrating vectors is 1", result);
        }
       
        [Test]
        public void GetmaxEigenSummaryTextFor3Series2CointVector()
        {
            List<MaxEigenData> testEigenStats = new List<MaxEigenData>() 
            { 
                new MaxEigenData(){ 
                     No="0", 
                    CriticalValue90=criticalEigenValues[0,0].ToString(), 
                    CriticalValue95=criticalEigenValues[0,1].ToString(),
                    CriticalValue99=criticalEigenValues[0,2].ToString(),
                    TestStatistic="128.3"
                },
                new MaxEigenData(){ 
                     No="1", 
                    CriticalValue90=criticalEigenValues[1,0].ToString(), 
                    CriticalValue95=criticalEigenValues[1,1].ToString(),
                    CriticalValue99=criticalEigenValues[1,2].ToString(),
                    TestStatistic="14.4"
                },
                new MaxEigenData(){ 
                    No="2", 
                    CriticalValue90=criticalEigenValues[2,0].ToString(), 
                    CriticalValue95=criticalEigenValues[2,1].ToString(),
                    CriticalValue99=criticalEigenValues[2,2].ToString(),
                    TestStatistic="11.4"
                }
            };

            string result = JohansenHelper.GetmaxEigenSummaryText(testEigenStats);

            Assert.AreEqual("No of cointegrating vectors is 2", result);
        }

        [Test]
        public void GetmaxEigenSummaryTextFor3Series2CointVector99PC()
        {
            List<MaxEigenData> testEigenStats = new List<MaxEigenData>() 
            { 
                new MaxEigenData(){ 
                     No="0", 
                    CriticalValue90=criticalEigenValues[0,0].ToString(), 
                    CriticalValue95=criticalEigenValues[0,1].ToString(),
                    CriticalValue99=criticalEigenValues[0,2].ToString(),
                    TestStatistic="128.3"
                },
                new MaxEigenData(){ 
                     No="1", 
                    CriticalValue90=criticalEigenValues[1,0].ToString(), 
                    CriticalValue95=criticalEigenValues[1,1].ToString(),
                    CriticalValue99=criticalEigenValues[1,2].ToString(),
                    TestStatistic="19.4"
                },
                new MaxEigenData(){ 
                    No="2", 
                    CriticalValue90=criticalEigenValues[2,0].ToString(), 
                    CriticalValue95=criticalEigenValues[2,1].ToString(),
                    CriticalValue99=criticalEigenValues[2,2].ToString(),
                    TestStatistic="11.4"
                }
            };

            string result = JohansenHelper.GetmaxEigenSummaryText(testEigenStats);

            Assert.AreEqual("No of cointegrating vectors is 2", result);
        }

        [Test]
        public void PerformMaxEigenValueTest()
        {
            List<double[]> johSeries = GetInputVectors();
            int nlags = 5;
            List<MaxEigenData> outStats = null;
            double[] eigenValuesVec = null;
            double[,] eigenVecMatrix = null;
            JohansenHelper.DoMaxEigenValueTest(johSeries, nlags, out outStats,
                out eigenValuesVec, out eigenVecMatrix);
        }

        List<double[]> GetInputVectors()
        {
            List<double[]> retlist = new List<double[]>();
            double[] series1 = new double[]{
-0.131893,
-0.181316,
-0.139308,
-0.111646,
-0.160503,
-0.130598,
-0.087075,
-0.053157,
-0.028243,
-0.008974,
-0.056845,
-0.076655,
-0.121625,
-0.081869,
-0.07252,
-0.110351,
-0.155463,
-0.130554,
-0.160936,
-0.146968,
-0.181607,
-0.18084,
-0.152553,
-0.178725,
-0.222393,
-0.233073,
-0.27256,
-0.287742,
-0.290583,
-0.259709,
-0.268764,
-0.318154,
-0.32306,
-0.311131,
-0.26479,
-0.236684,
-0.237449,
-0.255186,
-0.252223,
-0.263117,
-0.302741,
-0.255261,
-0.231425,
-0.190533,
-0.207056,
-0.194796,
-0.228801,
-0.237032,
-0.214936,
-0.168245,
-0.149781,
-0.107441,
-0.123029,
-0.087834,
-0.068994,
-0.061543,
-0.025197,
0.018776,
0.059368,
0.057816,
0.044875,
0.002026,
-0.043236,
-0.056781,
-0.038748,
-0.054285,
-0.077223,
-0.126977,
-0.12543,
-0.083396,
-0.060828,
-0.098936,
-0.076374,
-0.112632,
-0.074257,
-0.047372,
-0.038157,
-0.001133,
0.047998,
0.034731,
0.02029,
0.000765,
0.042367,
0.049474,
0.083411,
0.042005,
0.028332,
0.047504,
0.080992,
0.085216,
0.127083,
0.111753,
0.139508,
0.187621,
0.219698,
0.183297,
0.172848,
0.19041,
0.227759,
0.249823,
};
            retlist.Add(series1);
            double[] series2 = new double[]{
-0.078484,
-0.074062,
-0.087389,
-0.024263,
-0.068754,
-0.044437,
-0.059141,
-0.053006,
0.003709,
-0.002865,
-0.009529,
-0.013289,
-0.055955,
-0.068118,
0.00445,
-0.015686,
-0.115975,
-0.043764,
-0.092771,
-0.116714,
-0.137711,
-0.10966,
-0.078889,
-0.106939,
-0.085692,
-0.178341,
-0.121926,
-0.150197,
-0.19764,
-0.199885,
-0.113529,
-0.141927,
-0.148289,
-0.191835,
-0.141365,
-0.109319,
-0.115676,
-0.130611,
-0.183625,
-0.158801,
-0.201512,
-0.107079,
-0.150577,
-0.112133,
-0.073878,
-0.102028,
-0.0922,
-0.140197,
-0.083034,
-0.15002,
-0.056907,
-0.092578,
-0.052293,
-0.030933,
-0.065277,
-0.053308,
-0.060182,
0.005725,
0.010082,
-0.014375,
0.042171,
-0.003401,
-0.048771,
-0.073633,
-0.043301,
-0.072813,
-0.070145,
-0.063863,
-0.028461,
-0.060836,
-0.038678,
-0.061771,
-0.09021,
-0.0583,
-0.02714,
-0.027032,
0.005475,
-0.042084,
0.037206,
0.05603,
-0.006519,
-0.04576,
0.034835,
0.050458,
0.063703,
-0.016534,
-0.017048,
-0.019565,
0.091659,
0.084079,
0.056569,
0.033291,
0.115934,
0.099697,
0.096548,
0.093301,
0.104403,
0.138091,
0.140239,
0.178275,
};
            retlist.Add(series2);

 return retlist;

        }

    }
}
