using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;

namespace Diffuri2
{
    class Controller
    {
        public double A { get; private set; }
        public double B { get; private set; }
        public double H { get; private set; }
        public int N { get; private set; }
        public int P { get; private set; }
        public int Q { get; private set; }
        public int Y0 { get; private set; }
        public int Y1 { get; private set; }

        public Controller(double a, double b, int n, int y0, int q, int p, int y1)
        {
            A = a;
            B = b;
            N = n;
            H = (B - A) / N;
            Y0 = y0;
            Q = q;
            P = p;
            Y1 = y1;
        }

        private double GetExactSolution(double x)
        {
            return 2 * Math.Exp(2 * x) + (-1.3) * x * Math.Exp(2 * x) + (1 / 36 * Math.Exp(2*x) * Math.Sin(6*x));  
        }

        private double GetFunctionResult(double x)
        {
            x = Math.Round(x, 4);
            return -Math.Exp(2 * x) * Math.Sin(6 * x);
        }

        private double[,] GetMatrix()
        {
            double[,] matrix = new double[N + 1, N + 1];
            for (int i = 1; i < N; i++)
            {
                    matrix[i, i] = -(2 - H * H * Q);
                    matrix[i, i - 1] = 1 - (H / 2 * P);
                    matrix[i, i + 1] = 1 + (H / 2 * P);
            }
            matrix[0, 0] = 1;
            matrix[N, N] =  1 / H;
            matrix[N, N - 1] = -1 / H;

            return matrix;
        }

        private double[] GetFunctionArray(double[] xk)
        {
            double[] fk = new double[N + 1];
            for (int k = 1; k < N; k++)
            {
                fk[k] = GetFunctionResult(xk[k]) * H * H;
            }
            fk[0] = Y0;
            fk[N] = Y1;
            return fk;
        }

        private double[] GetResultSweepMethod(double[,] matrix ,double[] fk)
        {
            
            double[] b = new double[N + 1];
            double[] z = new double[N + 1];
            for (int k = 0; k < N + 1; k++)
            {
                double bk;
                double ck;
                double ak;

                if (k.Equals(0))
                {
                    ak = matrix[k, k + 1];
                    bk = matrix[k, k];
                    b[0] = - (ak / bk);
                    z[0] = fk[k] / bk;
                }
                else if (k.Equals(N))
                {
                     bk = matrix[k, k];
                     ck = matrix[k, k - 1];
                    b[k] = 0;
                    z[k] = (fk[k] - ck * z[k - 1]) / (bk + ck * b[k - 1]);
                }
                else
                {
                     bk = matrix[k, k];
                     ck = matrix[k, k - 1];
                     ak = matrix[k, k + 1];
                    b[k] = -(ak/ (bk + ck * b[k-1]));
                    z[k] = (fk[k] - ck * z[k - 1]) / (bk + ck * b[k - 1]);
                }

            }
            double[] y = new double[N + 1];
            for (int k = N; k >= 0; k--)
            {
                if (k.Equals(N))
                {
                    y[k] = z[k];
                }
                else
                {
                    y[k] = b[k] * y[k + 1] + z[k];
                }
            }
            return y;
        }

        public PointD[] GetResultGridMethod()
        {
            double[] xk = new double[N + 1];
            
            for (int k = 0; k < N + 1; k++)
            {
                xk[k] = Math.Round(A + H * k,3);               
            }
            double[] fk = GetFunctionArray(xk);
            double[,] matrix = GetMatrix();
            double[] resultY = GetResultSweepMethod(matrix, fk);
            PointD[] result = new PointD[N + 1];
            for (int k = 0; k < N + 1; k++)
            {
                result[k].X = xk[k];
                result[k].Y = resultY[k];
            }
            return result;
        }
        public PointD[] GetPointDExactSolution()
        {
            double[] xk = new double[N + 1];
            PointD[] result = new PointD[N + 1];
            for (int k = 0; k < N + 1; k++)
            {
                result[k].X = Math.Round(A + H * k, 3);
                result[k].Y = GetExactSolution(result[k].X);
            }
            return result;
        }
        static public double[] GetEspRunge(PointD[] Grid1H,PointD[]Grid2H,int p = 1)
        {
            double[] esp = new double[Grid1H.Length];
            for (int i = 0; i < esp.Length; i++)
            {
                if (i % 2 == 0)
                {
                    esp[i] = (Grid1H[i].Y - Grid2H[i/2].Y) / (Math.Pow(2, p) - 1);
                }
            }
            return esp;
        }

        
        
    }
}
