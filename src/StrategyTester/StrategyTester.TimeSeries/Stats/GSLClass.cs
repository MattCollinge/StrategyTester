using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace StrategyTester.TimeSeries.Stats
{
    public class dummy
    {
        public int j=5;

        [DllImport("libgsl_d.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gsl_ieee_env_setup();

        public int dotest()
        {
            gsl_ieee_env_setup();
            return 565;
        }
    }
    public class Win32APIs
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpszLib);

    }

    public class GSLHelper
    {
         public static void MatrixDivideByElem(IntPtr xMat, double val)
        {
            int nr = GSL.gsl_matrix_get_size1(xMat);
            int nc = GSL.gsl_matrix_get_size2(xMat);
            IntPtr tmpMat = GSL.gsl_matrix_alloc(nr, nc);
            GSL.gsl_matrix_set_all(tmpMat, val);
            GSL.gsl_matrix_div_elements(xMat, tmpMat);
            GSL.gsl_matrix_free(tmpMat);
        }
        public static IntPtr MatrixDivide(IntPtr xMat, IntPtr yMat)
        {
              //Dim xTranspose As Variant
            IntPtr xTranspose = MatrixTransposeImpl(xMat);
            IntPtr tmp1 = MatrixMultiply(xTranspose, xMat);
            IntPtr tmp2 = GetMatrixInverse(tmp1);
            IntPtr tmp3 = MatrixMultiply(tmp2, xTranspose);
            IntPtr tmp4 = MatrixMultiply(tmp3, yMat);
  //MYMATRIXDIVIDE = MMYMULTIPLY(MMYMULTIPLY(MYMATRIXINVERSE(MMYMULTIPLY(xTranspose, xMat)), xTranspose), yMat)
            return tmp4;
        }

        /*
        public static IntPtr MatrixSubtract(IntPtr xMat, IntPtr yMat, int nr, int nc)
        {
            IntPtr outMat = GSL.gsl_matrix_alloc(nr, nc);
            double[,] xmat2 = GSLToNetMatrix(xMat);
            double[,] ymat2 = GSLToNetMatrix(yMat);
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    GSL.gsl_matrix_set(outMat, i, j, xmat2[i, j] - ymat2[i, j]);
                }
            }
            return outMat;
        }
          */

        public static IntPtr GetMatrixInverse(IntPtr inMat)
        {
            int size1 = GSL.gsl_matrix_get_size1(inMat);
            IntPtr outMat = GSL.gsl_matrix_alloc(size1, size1);
            IntPtr invert_me = GSL.gsl_matrix_alloc(size1, size1);
            IntPtr perm = GSL.gsl_permutation_alloc(size1);
            int signum;
            GSL.gsl_matrix_memcpy(invert_me, inMat);
            GSL.gsl_linalg_LU_decomp(invert_me, perm, out signum);
            GSL.gsl_linalg_LU_invert(invert_me, perm, outMat);
            GSL.gsl_matrix_free(invert_me);
            GSL.gsl_permutation_free(perm);
            return outMat;
        }
        public static double[] GSLVecToDotNetVector(IntPtr x )
        {
            int n = GSL.gsl_vector_get_size(x);
            double[] retvec = new double[n];
            for (int i = 0; i < n; i++)
                retvec[i] = GSL.gsl_vector_get(x, i);
            return retvec;
        }

        public static double[] GSLComplexVecToAbsDotNetVector(IntPtr x)
        {
            int n = GSL.gsl_vector_complex_getsize(x);
            double[] retvec = new double[n];
            for (int i = 0; i < n; i++)
            {
                double realval, imagval;
                GSL.gsl_vector_complex_getboth(x, i, out realval, out imagval);
                retvec[i] = realval;
            }
            return retvec;
        }

        public static IntPtr GetGSLMatrixFromGSLVector(IntPtr v, int nr, int nc)
        {
            int n = nr * nc;
            int counter = 0;
            IntPtr outMat = GSL.gsl_matrix_alloc(nr, nc);
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    GSL.gsl_matrix_set(outMat, i, j, GSL.gsl_vector_get(v, counter));
                    counter++;
                }
            }
            return outMat;
        }

        public static void DoMultiParameterFit(double[] obs, double[,] indepmat,
            out double[] betaVec, out double residualSumOfSquares, out double[] stdErrorResiduals)
        {
            int n = obs.Length;
            int tmpn = indepmat.Length;
            int np = tmpn / n;
            double[] retbeta = new double[np];
            IntPtr y = GSL.gsl_vector_alloc(n);
            IntPtr X = GSL.gsl_matrix_alloc(n, np);
            for (int i = 0; i < n; i++)
            {
                GSL.gsl_vector_set(y, i, obs[i]);
                for (int j = 0; j < np; j++)
                {
                    GSL.gsl_matrix_set(X, i, j, indepmat[i, j]);
                }
            }

            IntPtr c = GSL.gsl_vector_alloc(np);
            IntPtr cov = GSL.gsl_matrix_alloc(np, np);
            IntPtr covinv = GSL.gsl_matrix_alloc(np, np);

            IntPtr workspace = GSL.gsl_multifit_linear_alloc(n, np);

            int test = GSL.gsl_multifit_linear(X,
                     y, c, cov, out residualSumOfSquares, workspace);

            GSL.gsl_multifit_linear_free(workspace);

            betaVec = GSLVecToDotNetVector(c);

            int df = n - np;
            double sigma2 = residualSumOfSquares / ((double)df);
            IntPtr tmp1 = MatrixTransposeImpl(X);
            IntPtr tmp2 = MatrixMultiply(tmp1, X);
            IntPtr tmp3 = GetMatrixInverse(tmp2);
            double[,] tmp4 = GSLToNetMatrix(tmp3);
            stdErrorResiduals = new double[np];
            for (int i = 0; i < np; i++)
            {
                stdErrorResiduals[i] = Math.Sqrt(tmp4[i, i] * sigma2);
            }
        }

        public static double[,] GSLToNetMatrix(IntPtr x)
        {
            int nr = GSL.gsl_matrix_get_size1(x);
            int nc = GSL.gsl_matrix_get_size2(x);
            double[,] retmat = new double[nr, nc];
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    retmat[i, j] = GSL.gsl_matrix_get(x, i, j);
                }
            }
            return retmat;
        }

        public static double[,] GSLComplexToAbsNetMatrix(IntPtr x)
        {
            int nr = GSL.gsl_matrix_complex_getsize1(x);
            int nc = GSL.gsl_matrix_complex_getsize2(x);
            double[,] retmat = new double[nr, nc];
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    double realval,imagval;
                    GSL.gsl_matrix_complex_getboth(x, i, j, out realval, out imagval);

                    retmat[i, j] = realval;
                }
            }
            return retmat;
        }

        public static IntPtr DotNetToGSLMatrix(double[,] inMat, int nr, int nc)
        {
            IntPtr x = GSL.gsl_matrix_alloc(nr, nc);
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    GSL.gsl_matrix_set(x, i, j, inMat[i, j]);
                }
            }
            return x;
        }

        public static IntPtr ListDotNetToGSLMatrix(List<double[]> inMat)
        {
            int nc = inMat.Count;
            int nr = 0;
            if (nc > 0)
                nr = inMat[0].Length;
            IntPtr x = GSL.gsl_matrix_alloc(nr, nc);
            for (int i = 0; i < nc; i++)
            {
                double[] currentSeries = inMat[i];
                for (int j = 0; j < nr; j++)
                {
                    GSL.gsl_matrix_set(x, j, i, currentSeries[j]);
                }
            }
            return x;
        }

        public static double[,] MatrixTranspose(double[,] m, int nr, int nc)
        {
            IntPtr tmpm = DotNetToGSLMatrix(m, nr, nc);
            return GSLToNetMatrix(MatrixTransposeImpl(tmpm));
        }

        public static IntPtr MatrixTransposeImpl(IntPtr m)
        {
            int nr = GSL.gsl_matrix_get_size1(m);
            int nc = GSL.gsl_matrix_get_size2(m);

            IntPtr retmat = GSL.gsl_matrix_alloc(nc, nr);
            GSL.gsl_matrix_transpose_memcpy(retmat, m);
            return retmat;
        }

        public static IntPtr MatrixMultiply(IntPtr A, IntPtr B)
        {
            int nrA = GSL.gsl_matrix_get_size1(A);
            int ncB = GSL.gsl_matrix_get_size2(B);

            IntPtr resMat = GSL.gsl_matrix_alloc(nrA, ncB);
            GSL.gsl_blas_dgemm((int)GSL.CBLAS_TRANSPOSE.CblasNoTrans,
                (int)GSL.CBLAS_TRANSPOSE.CblasNoTrans,
                  1.0, A, B,
                  0.0, resMat);
            return resMat;
        }

    }

    public class GSL
    {
        [DllImport("libgsl_d.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gsl_ieee_env_setup();

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct gsl_block_struct
        {

            /// size_t->unsigned int
            public uint size;

            /// double*
            public System.IntPtr data;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct gsl_vector
        {

            /// size_t->unsigned int
            public uint size;

            /// size_t->unsigned int
            public uint stride;

            /// double*
            public System.IntPtr data;

            /// gsl_block*
            public System.IntPtr block;

            /// int
            public int owner;
        }

        /// Return Type: gsl_vector*
        ///n: size_t->unsigned int
        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_vector_alloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern System.IntPtr gsl_vector_allocintern(int n);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_matrix_alloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern System.IntPtr gsl_matrix_allocintern(int m, int n);

        //TODO: do we need these dictionaries??
        static Dictionary<int, IntPtr> _allocatedMatrices = new Dictionary<int, IntPtr>();
        static Dictionary<int, IntPtr> _allocatedVectors = new Dictionary<int, IntPtr>();

        public static System.IntPtr gsl_vector_alloc(int n)
        {
            //TODO: get rid of ToString() !!!
            IntPtr tmp = gsl_vector_allocintern(n);
            string tmpstr = tmp.ToString();
            int tmpint = Convert.ToInt32(tmpstr);
            _allocatedVectors.Add(tmpint, tmp);
            return tmp;
        }

        public static System.IntPtr gsl_matrix_alloc(int m, int n)
        {
            //TODO: get rid of ToString() !!!
            IntPtr tmp = gsl_matrix_allocintern(m, n);
            string tmpstr = tmp.ToString();
            int tmpint = Convert.ToInt32(tmpstr);
            _allocatedMatrices.Add(tmpint, tmp);
            return tmp;
        }

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_matrix_get_size1", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_matrix_get_size1(IntPtr m);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_vector_get_size", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_vector_get_size(IntPtr v);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_matrix_get_size2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_matrix_get_size2(IntPtr m);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_vector_complex_alloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr gsl_vector_complex_alloc(int n);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_multifit_linear_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gsl_multifit_linear_free(IntPtr workspace);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_matrix_complex_alloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr gsl_matrix_complex_alloc(int nr, int nc);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_eigen_nonsymmv_alloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr gsl_eigen_nonsymmv_alloc(int n);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_eigen_nonsymmv", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_eigen_nonsymmv(IntPtr A,
            IntPtr eval, IntPtr evec, IntPtr w);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_eigen_nonsymmv_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gsl_eigen_nonsymmv_free(IntPtr w);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_vector_complex_getsize", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_vector_complex_getsize(IntPtr v);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_matrix_complex_getsize1", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_matrix_complex_getsize1(IntPtr v);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_matrix_complex_getsize2", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_matrix_complex_getsize2(IntPtr v);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_vector_complex_getboth", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gsl_vector_complex_getboth(IntPtr v, int i, out double realval, out double imagval);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_matrix_complex_getboth", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gsl_matrix_complex_getboth(IntPtr m, int i, int j, out double realval, out double imagval);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_eigen_nonsymmv_sort", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_eigen_nonsymmv_sort(
            IntPtr eval, IntPtr evec, gsl_eigen_sort_t sortorder);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_eigen_jacobi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_eigen_jacobi(IntPtr matrix,
                      IntPtr eval,
                      IntPtr evec,
                      int max_rot, 
                      out int nrot);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_multifit_linear_alloc")]
        public static extern System.IntPtr gsl_multifit_linear_alloc(int n, int p);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_multifit_linear", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_multifit_linear(IntPtr X,
                     IntPtr y, IntPtr c, IntPtr cov, out double chisq, IntPtr work);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_permutation_alloc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr gsl_permutation_alloc(int n);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_matrix_memcpy", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_matrix_memcpy(IntPtr dest, IntPtr src);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_linalg_LU_decomp", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_linalg_LU_decomp(IntPtr A, IntPtr p, out int signum);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_linalg_LU_invert",CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_linalg_LU_invert(IntPtr LU, IntPtr p, IntPtr inverse);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_matrix_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gsl_matrix_freeimp(IntPtr m);

        public static void gsl_matrix_free(IntPtr m)
        {
            int tmpint = m.ToInt32();
            if (_allocatedMatrices.ContainsKey(tmpint))
                _allocatedMatrices.Remove(tmpint);
            gsl_matrix_freeimp(m);
        }

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_vector_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gsl_vector_freeimp(IntPtr m);

        public static void gsl_vector_free(IntPtr v)
        {
            int tmpint = v.ToInt32();
            if (_allocatedVectors.ContainsKey(tmpint))
                _allocatedVectors.Remove(tmpint);
            gsl_vector_freeimp(v);
        }
        
        public static void ResetMemory()
        {
            foreach (KeyValuePair<int, IntPtr> pair in _allocatedMatrices)
            {
                IntPtr m = pair.Value;
                gsl_matrix_freeimp(m);
            }
            _allocatedMatrices.Clear();

            foreach (KeyValuePair<int, IntPtr> pair in _allocatedVectors)
            {
                IntPtr v  = pair.Value;
                gsl_vector_freeimp(v);
            }
            _allocatedVectors.Clear();
        }

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_permutation_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gsl_permutation_free(IntPtr p);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_matrix_get",CallingConvention = CallingConvention.Cdecl)]
        public static extern double gsl_matrix_get(IntPtr m, int i, int j);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_blas_dgemm",CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_blas_dgemm(int enumTransA, int enumTransB,
                double alpha, IntPtr A, IntPtr B,
                double beta, IntPtr C);


        public enum CBLAS_TRANSPOSE { CblasNoTrans = 111, CblasTrans = 112, CblasConjTrans = 113 };


        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_vector_set", CallingConvention = CallingConvention.Cdecl)]
        //public static extern void gsl_vector_set(ref gsl_vector v, [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.SysUInt)] uint i, double x);
        public static extern void gsl_vector_set(IntPtr v, int i, double x);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_matrix_set", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gsl_matrix_set(IntPtr v, int i, int j, double x);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_matrix_set_all", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gsl_matrix_set_all(IntPtr v, double x);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_matrix_sub")]
        public static extern int gsl_matrix_sub(IntPtr m1, IntPtr m2);


        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_eigen_symmv_alloc")]
        public static extern IntPtr gsl_eigen_symmv_alloc(int n);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_eigen_symmv")]
        public static extern int gsl_eigen_symmv(IntPtr A, IntPtr eigval, IntPtr eigmat, IntPtr workspace);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_eigen_symmv_free")]
        public static extern void gsl_eigen_symmv_free(IntPtr workspace);

        public enum gsl_eigen_sort_t
        {
  GSL_EIGEN_SORT_VAL_ASC,
  GSL_EIGEN_SORT_VAL_DESC,
  GSL_EIGEN_SORT_ABS_ASC,
  GSL_EIGEN_SORT_ABS_DESC
};

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_eigen_symmv_sort")]
        public static extern int gsl_eigen_symmv_sort(IntPtr eigval, IntPtr eigmat, gsl_eigen_sort_t sort_type);


        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_vector_set_all")]
        public static extern void gsl_vector_set_all(IntPtr v, double x);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_matrix_div_elements", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_matrix_div_elements(IntPtr m1, IntPtr m2);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_multimin_fminimizer_set", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_multimin_fminimizer_set(IntPtr s, IntPtr f, IntPtr x, IntPtr step_size);


        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_matrix_transpose_memcpy",CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_matrix_transpose_memcpy(IntPtr dest, IntPtr src);

        /// Return Type: double
        ///x: gsl_vector*
        ///params: void*
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate double gsl_multimin_delegate(IntPtr x, System.IntPtr myparams);
        public static double gsl_multimin_function_struct_f_fn(IntPtr x, System.IntPtr myparams) { return 0; }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public class gsl_multimin_function_struct
        {

            /// gsl_multimin_function_struct_f

            public gsl_multimin_delegate f;

            /// size_t->unsigned int
            public uint n;

            /// void*
            public System.IntPtr voidparams;
        }

        /// Return Type: int
        ///state: void*
        ///n: size_t->unsigned int
        public delegate int _alloc_mintype(System.IntPtr state, System.IntPtr n);
        public static int _alloc_mintype_fn(System.IntPtr state, System.IntPtr n) { return 0; }

        public delegate int _set_mintype(System.IntPtr state, ref gsl_multimin_function_struct f, ref gsl_vector x, ref double size, ref gsl_vector step_size);
        public static int _set_mintype_fn(System.IntPtr state, ref gsl_multimin_function_struct f, ref gsl_vector x, ref double size, ref gsl_vector step_size) { return 0; }

        public delegate int _iterate_mintype(System.IntPtr state, ref gsl_multimin_function_struct f, ref gsl_vector x, ref double size, ref double fval);
        public static int _iterate_mintype_fn(System.IntPtr state, ref gsl_multimin_function_struct f, ref gsl_vector x, ref double size, ref double fval) { return 0; }

        /// Return Type: void
        ///state: void*
        public delegate void _free_mintype(System.IntPtr state);
        public static void _free_mintype_fn(System.IntPtr state) { }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public class gsl_multimin_fminimizer_type
        {

            /// char*
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)]
            public string name;

            /// size_t->unsigned int
            public uint size;

            /// _alloc
            public _alloc_mintype AnonymousMember1 = _alloc_mintype_fn;

            /// _set
            public _set_mintype AnonymousMember2 = _set_mintype_fn;

            /// _iterate
            public _iterate_mintype AnonymousMember3 = _iterate_mintype_fn;

            /// _free
            public _free_mintype AnonymousMember4 = _free_mintype_fn;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public class gsl_multimin_fminimizer
        {

            //const gsl_multimin_fminimizer_type *type;
            IntPtr type;

            //gsl_multimin_function *f;
            IntPtr f;

            double fval;

            //gsl_vector * x;
            IntPtr x;

            double size;

            //void *state;
            IntPtr state;
        }




        public static IntPtr get_gsl_multimin_fminimizer_nmsimplex()
        {
            IntPtr retPtr = IntPtr.Zero;
            //gsl_multimin_fminimizer_type gsl_multimin_fminimizer_nmsimplex = new gsl_multimin_fminimizer_type();
            IntPtr hgslDll = Win32APIs.LoadLibrary("libgsl_d.dll");
            IntPtr addrUnmanagedMtGen = Win32APIs.GetProcAddress(hgslDll, "gsl_multimin_fminimizer_nmsimplex");
            if (addrUnmanagedMtGen != IntPtr.Zero)
            {
                //convert unmanaged pointer to managed pointer 
                IntPtr addrManagedMtGen = Marshal.ReadIntPtr(addrUnmanagedMtGen);

                //create and initialize a managed object instance from the managed pointer to the unmanaged memory!
                gsl_multimin_fminimizer_type gsl_multimin_fminimizer_nmsimplex = new gsl_multimin_fminimizer_type();
                Marshal.PtrToStructure(addrManagedMtGen, gsl_multimin_fminimizer_nmsimplex);

                //Now make our managed object ready for passing into unmanaged code
                IntPtr ptrUnmanaged = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(gsl_multimin_fminimizer_type)));
                Marshal.StructureToPtr(gsl_multimin_fminimizer_nmsimplex, ptrUnmanaged, false);
                retPtr = ptrUnmanaged;

            }

            return retPtr;
        }


        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_vector_get",CallingConvention = CallingConvention.Cdecl)]
        public static extern double gsl_vector_get(IntPtr v, int i);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_multimin_fminimizer_alloc",CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr gsl_multimin_fminimizer_alloc(IntPtr T, uint i);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_multimin_fminimizer_iterate",CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_multimin_fminimizer_iterate(IntPtr s);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_multimin_fminimizer_size",CallingConvention = CallingConvention.Cdecl)]
        public static extern double gsl_multimin_fminimizer_size(IntPtr s);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_multimin_test_size",CallingConvention = CallingConvention.Cdecl)]
        public static extern int gsl_multimin_test_size(double size, double epsabs);

        public static double expacdlikelihood_fn(IntPtr x, IntPtr p)
        {
            return 0;
        }


        /// Return Type: void
        ///state: void*
        ///seed: int
        public delegate void _set(System.IntPtr state, int seed);
        public static void _setfn(System.IntPtr state, int seed) { }

        /// Return Type: int
        ///state: void*
        public delegate int _get(System.IntPtr state);
        public static int _getfn(System.IntPtr state) { return 0; }

        /// Return Type: double
        ///state: void*
        public delegate double _get_double(System.IntPtr state);
        public static double _get_doublefn(System.IntPtr state) { return 0; }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public class gsl_rng_type
        {

            /// char*
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPStr)]
            public string name;

            /// int
            public uint max;
            public int min;

            /// size_t->unsigned int
            public uint size;

            /// _set
            public _set setfn = _setfn;
            /// _get
            public _get getfn = _getfn;

            /// _get_double
            public _get_double get_doublefn = _get_doublefn;

        }

        /// Return Type: gsl_rng_type*
        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_rng_env_setup",CallingConvention = CallingConvention.Cdecl)]
        public static extern System.IntPtr gsl_rng_env_setup();

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct gsl_rng
        {
            /// gsl_rng_type*
            public System.IntPtr type;

            /// void*
            public System.IntPtr state;
        }

        /// Return Type: gsl_rng*
        ///T: gsl_rng_type*
        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_rng_alloc",CallingConvention = CallingConvention.Cdecl)]
        public static extern System.IntPtr gsl_rng_alloc(IntPtr T);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_rng_set",CallingConvention = CallingConvention.Cdecl)]
        public static extern void gsl_rng_set(IntPtr T, int seed);

        [System.Runtime.InteropServices.DllImportAttribute("libgsl_d.dll", EntryPoint = "gsl_ran_gaussian",CallingConvention = CallingConvention.Cdecl)]
        public static extern double gsl_ran_gaussian(IntPtr T, double sigma);


    }

   
}



