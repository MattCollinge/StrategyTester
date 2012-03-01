using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrategyTester.TimeSeries.Stats
{
    public class JohansenHelper
    {
        //maximum eigen value statistics for constant term assumption
        static double[,] maxEigenCriticalvalues = new double[,]{
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

        static List<double[]> DeMean(List<double[]> xMat)
        {
            List<double[]> retList = new List<double[]>();
            for (int i = 0; i < xMat.Count; i++)
            {
                double[] currentSeries = xMat[i];
                double average = GetAverage(currentSeries);
                for (int j = 0; j < currentSeries.Length; j++)
                    currentSeries[j] = currentSeries[j] - average;
                retList.Add(currentSeries);
            }
            return retList;
        }

        static List<double[]> GetMatrixDifference(List<double[]> xMat)
        {
            List<double[]> retList = new List<double[]>();
            for (int i = 0; i < xMat.Count; i++)
            {
                double[] currentSeries = xMat[i];
                double[] newSeries = new double[currentSeries.Length - 1];
                for (int j = 0; j < newSeries.Length; j++)
                    newSeries[j] = currentSeries[j + 1] - currentSeries[j];
                retList.Add(newSeries);
            }
            return retList;
        }

        static List<double[]> GetSubMatrix(List<double[]> xMat,
            int nBeginRow, int nEndRow,
            int nBeginCol, int nEndCol)
        {
            if (nBeginRow == -1) nBeginRow = 0;
            if (nEndRow == -1) nEndRow = xMat[0].Length - 1;
            if (nBeginCol == -1) nBeginCol = 0;
            if (nEndCol == -1) nEndCol = xMat.Count - 1;
            List<double[]> retList = new List<double[]>();
            for (int i = nBeginCol; i <= nEndCol; i++)
            {
                double[] currentSeries = xMat[i];
                double[] newSeries = new double[nEndRow - nBeginRow + 1];
                for (int j = 0; j < newSeries.Length; j++)
                    newSeries[j] = currentSeries[j + nBeginRow];
                retList.Add(newSeries);
            }
            return retList;
        }

        static List<double[]> GetMatrixLagged(List<double[]> xMat, int nlags)
        {
            List<double[]> retList = new List<double[]>();
            int nCols = xMat.Count * nlags;
            int counter = 0;
            int counter2 = 0;
            for (int i = 0; i < nCols; i++)
            {
                double[] currentSeries = xMat[counter2];
                double[] newSeries = new double[currentSeries.Length - nlags];
                for (int j = 0; j < newSeries.Length; j++)
                    newSeries[j] = currentSeries[j + nlags - counter - 1];
                retList.Add(newSeries);
                counter++;
                if (counter >= nlags)
                {
                    counter = 0;
                    counter2++;
                }
            }
            return retList;
        }


        static double GetAverage(double[] x)
        {
            double total = 0;
            for (int i = 0; i < x.Length; i++)
                total += x[i];
            return total / ((double)x.Length);
        }

        public static string GetmaxEigenSummaryText(List<MaxEigenData> maxEigStats)
        {
            string summary = "no conclusion - input series may be stationary";
            List<string> rankChoice = new List<string>();
            for (int i = 0; i < maxEigStats.Count; i++)
            {
                MaxEigenData eigData = maxEigStats[i];
                double testStat = Convert.ToDouble(eigData.TestStatistic);
                double critical90 = Convert.ToDouble(eigData.CriticalValue90);
                double critical95 = Convert.ToDouble(eigData.CriticalValue95);
                double critical99 = Convert.ToDouble(eigData.CriticalValue99);
                if (testStat < critical90)
                {
                    //we are confident about the coinegration relationship!
                    summary = "No of cointegrating vectors is " + eigData.No;
                    break;
                }
                else if (testStat > critical95)
                {
                    //continue and check for next 
                }
                else
                {
                    //test in not able to decide how many coint vectors
                    rankChoice.Add(eigData.No);
                }
            }
            if (rankChoice.Count > 0)
            {
                summary = "No of cointegrating vectors is ";
                for (int i=0; i< rankChoice.Count;i++)
                {
                    if (i == rankChoice.Count-1)
                        summary += rankChoice[i];
                    else
                        summary += rankChoice[i] + " or ";

                }
            }
            return summary;
        }

        public static double[] GetSignificantEvals(List<MaxEigenData> maxEigStats, double[] eigenValuesVec)
        {
            List<double> significantEvals = new List<double>();
            int noOfCointVectors = 0;
            bool hasCointegration = false;
            for (int i = 0; i < maxEigStats.Count; i++)
            {
                MaxEigenData eigData = maxEigStats[i];
                double testStat = Convert.ToDouble(eigData.TestStatistic);
                double critical90 = Convert.ToDouble(eigData.CriticalValue90);
                double critical95 = Convert.ToDouble(eigData.CriticalValue95);
                double critical99 = Convert.ToDouble(eigData.CriticalValue99);
                if (testStat < critical99)
                {
                    hasCointegration = true;
                    //do not look further down
                    break;
                }
                else
                {
                    noOfCointVectors++;
                }
            }
            if (hasCointegration)
            {
                for (int i = 0; i < noOfCointVectors; i++)
                    significantEvals.Add(eigenValuesVec[i]);

            }
            return significantEvals.ToArray();
        }

        public static void DoMaxEigenValueTest(List<double[]> xMat, int nlags, out List<MaxEigenData> outStats,
            out double[] eigenValuesVec, out double[,] eigenVecMatrix)
        {
            IntPtr test1 = GSLHelper.ListDotNetToGSLMatrix(xMat);
            int t1 = GSL.gsl_matrix_get_size1(test1);
            int t2 = GSL.gsl_matrix_get_size2(test1);


            
            xMat = DeMean(xMat);
            List<double[]> dxMat = GetMatrixDifference(xMat);
            List<double[]> dxLaggedMatrix = GetMatrixLagged(dxMat, nlags);
            List<double[]> dxLaggedDemeanedMatrix = DeMean(dxLaggedMatrix);
            List<double[]> dxDemeanedMatrix = DeMean(GetSubMatrix(dxMat, nlags, -1, -1, -1));
            int nrx = dxLaggedDemeanedMatrix[0].Length;
            int ncx = dxLaggedDemeanedMatrix.Count;
            int nry = dxDemeanedMatrix[0].Length;
            int ncy = dxDemeanedMatrix.Count;

            IntPtr tmp1 = GSLHelper.MatrixDivide(GSLHelper.ListDotNetToGSLMatrix(dxLaggedDemeanedMatrix),
                GSLHelper.ListDotNetToGSLMatrix(dxDemeanedMatrix));

            nrx = dxLaggedDemeanedMatrix[0].Length;
            ncy = dxDemeanedMatrix.Count;
            IntPtr fittedRegressionDX = GSLHelper.MatrixMultiply(GSLHelper.ListDotNetToGSLMatrix(dxLaggedDemeanedMatrix),
                tmp1);
            double[,] tetsdbl = GSLHelper.GSLToNetMatrix(fittedRegressionDX);

            int nr = dxDemeanedMatrix[0].Length;
            int nc = dxDemeanedMatrix.Count;

            IntPtr ResidualsRegressionDX = GSLHelper.ListDotNetToGSLMatrix(dxDemeanedMatrix);
            GSL.gsl_matrix_sub(ResidualsRegressionDX, fittedRegressionDX);


            nrx = xMat[0].Length;
            List<double[]> tmp4 = GetSubMatrix(xMat, 1, nrx - nlags - 1, -1, -1);
            List<double[]> xDemeanedMatrix = DeMean(tmp4);

            IntPtr tmp6 = GSLHelper.MatrixDivide(GSLHelper.ListDotNetToGSLMatrix(dxLaggedDemeanedMatrix),
                GSLHelper.ListDotNetToGSLMatrix(xDemeanedMatrix));

            IntPtr fittedRegressionX = GSLHelper.MatrixMultiply(GSLHelper.ListDotNetToGSLMatrix(dxLaggedDemeanedMatrix),
                tmp6);

            IntPtr ResidualsRegressionX = GSLHelper.ListDotNetToGSLMatrix(xDemeanedMatrix);
            GSL.gsl_matrix_sub(ResidualsRegressionX, fittedRegressionX);




            IntPtr tmp8 = GSLHelper.MatrixTransposeImpl(ResidualsRegressionX);
            IntPtr tmp9 = GSLHelper.MatrixMultiply(GSLHelper.MatrixTransposeImpl(ResidualsRegressionX),
               ResidualsRegressionX);
            GSLHelper.MatrixDivideByElem(tmp9, (double)GSL.gsl_matrix_get_size1(ResidualsRegressionX));
            IntPtr Skk = tmp9;
            double[,] tetsdbl2 = GSLHelper.GSLToNetMatrix(Skk);


            tmp9 = GSLHelper.MatrixMultiply(GSLHelper.MatrixTransposeImpl(ResidualsRegressionX),
               ResidualsRegressionDX);
            GSLHelper.MatrixDivideByElem(tmp9, (double)GSL.gsl_matrix_get_size1(ResidualsRegressionX));
            IntPtr Sk0 = tmp9;
            tetsdbl2 = GSLHelper.GSLToNetMatrix(Sk0);

            tmp9 = GSLHelper.MatrixMultiply(GSLHelper.MatrixTransposeImpl(ResidualsRegressionDX),
               ResidualsRegressionDX);
            GSLHelper.MatrixDivideByElem(tmp9, (double)GSL.gsl_matrix_get_size1(ResidualsRegressionDX));
            IntPtr S00 = tmp9;
            tetsdbl2 = GSLHelper.GSLToNetMatrix(S00);


            //eigenObj.SetMatrix MMYMULTIPLY(MYMATRIXINVERSE(Skk), MMYMULTIPLY(MMYMULTIPLY(Sk0, MYMATRIXINVERSE(S00)), MTRANSPOSE(Sk0)))
            IntPtr eigenInputMat = GSLHelper.MatrixMultiply(GSLHelper.GetMatrixInverse(Skk), GSLHelper.MatrixMultiply(GSLHelper.MatrixMultiply(Sk0, GSLHelper.GetMatrixInverse(S00)), GSLHelper.MatrixTransposeImpl(Sk0)));
            tetsdbl2 = GSLHelper.GSLToNetMatrix(eigenInputMat);

            int n = GSL.gsl_matrix_get_size1(eigenInputMat);


            //double[,] data = new double[,]{ {-1.0, 1.0, -1.0, 1.0},
              //           {-8.0, 4.0, -2.0, 1.0},
                //         {27.0, 9.0, 3.0, 1.0},
                  //       {64.0, 16.0, 4.0, 1.0} };
            //eigenInputMat = GSLHelper.DotNetToGSLMatrix(data, 4, 4);
            IntPtr evalPtr = GSL.gsl_vector_complex_alloc(n);
            IntPtr ematPtr = GSL.gsl_matrix_complex_alloc(n, n);
            IntPtr worspacePtr = GSL.gsl_eigen_nonsymmv_alloc(n);
            GSL.gsl_eigen_nonsymmv(eigenInputMat, evalPtr, ematPtr, worspacePtr);
            GSL.gsl_eigen_nonsymmv_free(worspacePtr);

            GSL.gsl_eigen_nonsymmv_sort(evalPtr, ematPtr,
                                GSL.gsl_eigen_sort_t.GSL_EIGEN_SORT_ABS_DESC);


            eigenValuesVec = GSLHelper.GSLComplexVecToAbsDotNetVector(evalPtr);
            eigenVecMatrix = GSLHelper.GSLComplexToAbsNetMatrix(ematPtr);
            
            int nSamples = GSL.gsl_matrix_get_size1(ResidualsRegressionX);
            int nVariables = GSL.gsl_matrix_get_size2(ResidualsRegressionX);

            outStats = new List<MaxEigenData>();
            int counter=0;
            for (int i = 0; i < eigenValuesVec.Length; i++)
            {
                MaxEigenData eigData = new MaxEigenData();
                eigData.No = i.ToString();
                double LR_maxeigenvalue = -nSamples * Math.Log(1 - eigenValuesVec[i]);
                eigData.TestStatistic = LR_maxeigenvalue.ToString();
                eigData.CriticalValue90 = maxEigenCriticalvalues[nVariables - counter - 1, 0].ToString();
                eigData.CriticalValue95 = maxEigenCriticalvalues[nVariables - counter - 1, 1].ToString();
                eigData.CriticalValue99 = maxEigenCriticalvalues[nVariables - counter - 1, 2].ToString();
                counter++;
                outStats.Add(eigData);
            }
            //Skk = MMYMATRIXELEMENTDIVIDE(MMYMULTIPLY(MTRANSPOSE(ResidualsRegressionX), ResidualsRegressionX), UBound(ResidualsRegressionX, 1))

        }

    }
}
