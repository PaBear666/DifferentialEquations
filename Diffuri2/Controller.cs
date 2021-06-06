using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;

namespace Diffuri2
{
    class Methods
    {
        public double A { get; private set; }
        public double B { get; private set; }
        public int N { get; private set; }
        public int Pparametr { get; private set; }
        public int Qparametr { get; private set; }
        public int Value1 { get; private set; }
        public int Value2 { get; private set; }
        public double H { get; private set; }

        public Methods(double a, double b, double h, int value1, int q, int p, int value2)
        {
            A = a;
            B = b;
            H = h;
            N = (int)((B - A) / H);
            Value1 = value1;
            Qparametr = q;
            Pparametr = p;
            Value2 = value2;
        }

        private double Analitic(double x)
        {
            double c1 = 1.0 / 24.0;
            double c2 = (96 + 2*Math.Sin(1)-14*Math.Sin(7)+21*Math.Cos(7)) / (48 * Math.Cos(1));
            return c1 * Math.Cos(x) + c2 * Math.Sin(x) - 1.0 / 16.0 * Math.Sin(7 * x) - 1.0 / 24.0 * Math.Cos(7 * x);
        }

        private double RightFunction(double x)
        {
            x = Math.Round(x, 4);
            return 2*Math.Cos(7*x)+3*Math.Sin(7*x);
        }
        private double[] Vector()
        {
            double[] vector = new double[3*(N-1) + 4];
            vector[0] = 1;
            vector[1] = 0;
            // (y[N+1]-y[N])/H = Y1 Краевое условие
            vector[3 * N - 1] = -1 / H;
            vector[3 * N] = 1 / H;
            for (int i = 3; i < 3*N; i += 3)
            {
                vector[i - 1] = 1 - (H / 2 * Pparametr);
                vector[i] = -(2 - H * H * Qparametr);
                vector[i + 1] = 1 + (H / 2 * Pparametr);
            }          
            return vector;
        }

        private double[] FunctionArray(double[] x)
        {
            double[] f = new double[N + 1];
            for (int k = 1; k < N; k++)
            {
                f[k] = RightFunction(x[k]) * H * H;
            }
            f[0] = Value1;
            f[N] = Value2;
            return f;
        }

        private double[] Progonka(double[] vector, double[] fk)
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
                    ak = vector[k + 1];
                    bk = vector[k];
                    b[0] = -(ak / bk);
                    z[0] = fk[k] / bk;
                }
                else if (k.Equals(N))
                {
                    bk = vector[3*k];
                    ck = vector[3*k - 1];
                    b[k] = 0;
                    z[k] = (fk[k] - ck * z[k - 1]) / (bk + ck * b[k - 1]);
                }
                else
                {
                    bk = vector[3*k];
                    ck = vector[3*k - 1];
                    ak = vector[3*k + 1];
                    b[k] = -(ak / (bk + ck * b[k - 1]));
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

        public PointD[] GridMethod()
        {
            double[] xk = new double[N + 1];
            
            for (int k = 0; k < N + 1; k++)
            {
                xk[k] = Math.Round(A + H * k,3);               
            }
            double[] fk = FunctionArray(xk);
            double[] vector = Vector();
            double[] resultY = Progonka(vector, fk);
            PointD[] result = new PointD[N + 1];
            for (int k = 0; k < N + 1; k++)
            {
                result[k].X = xk[k];
                result[k].Y = resultY[k];
            }
            return result;
        }
        public PointD[] AnaliticSolution()
        {
            double[] xk = new double[N + 1];
            PointD[] result = new PointD[N + 1];
            for (int k = 0; k < N + 1; k++)
            {
                result[k].X = Math.Round(A + H * k, 3);
                result[k].Y = Analitic(result[k].X);
            }
            return result;
        }
        static public double[] EpsRunge(Methods m0)
        {
            int p = 1;
            Methods m1 = Methods.Create2H(m0);
            var m0Grid = m0.GridMethod();
            var m1Grid = m1.GridMethod();
            double[] esp = new double[m0Grid.Length];
            for (int i = 0; i < esp.Length; i++)
                esp[i] = i % 2 == 0 ? (m0Grid[i].Y - m1Grid[i/2].Y) / (Math.Pow(2, p) - 1) : 0;
            return esp;
        }

        static public Methods Create2H(Methods methods)
        {
            return new Methods(methods.A, methods.B, methods.H * 2, methods.Value1, methods.Qparametr, methods.Pparametr, methods.Value2);
        }

        
        
    }
}
