using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ZedGraph;

namespace Diffuri
{
    public class Contoller
    {
        public double A { get; private set; }
        public double B { get; private set; }
        public double X0 { get; private set; }
        public double Y0 { get; private set; }
        public double Eps { get; private set; }

        public double H { get; private set; }

        public int N { get; private set; }
        public Contoller(double a,double b,double x0,double y0,double eps,double h0)
        {
            A = a;
            B = b;
            X0 = x0;
            Y0 = y0;
            Eps = eps;
            H = CalcStep(h0);
            N = (int)Math.Round((B - A) / H);
        }

        public double MainFunction(double x, double y)
        {
            return ((2 * Math.Pow(y,2) * Math.Log(x)) - y) / x;
            
        }

        public double CalcStep(double h0)
        {
            bool condition;
            do
            {
                double y2 = RungeKutta(A + 2*h0, h0).Last().Y;
                double y3 = RungeKutta(A + 2*h0, 2*h0).Last().Y;
                if (Math.Abs((double)(y2 - y3)) < Eps) 
                {
                    condition = false;
                }
                else
                {
                    h0 *= 2;
                    condition = true;
                }
            } while (condition);
            
            if ((B - A) / h0 % 2 != 0)
            {
                int n = (int)Math.Round(((B - A) / h0)) + 1;
                h0 = (double)((B - A) / n);
            }
            return h0;
        }
        public PointD[] Exact_Solution()
        {
            PointD[] points = new PointD[N + 1];
            for (int i = 0; i <= N; i++)
            {
                points[i].X = A + i * H;
                points[i].Y = (1 / (2 * Math.Log(points[i].X) + 2));
              
            }
            return points;
        }
        public PointD[] Eiler()
        {
            PointD[] points = new PointD[N + 1];
            double x0 = 0;
            double y0 = 0;
            for (int i = 1; i <= N; i++)
            {

                if (i == 1)
                {
                    points[i - 1].X = X0;
                    points[i - 1].Y = Y0;
                    x0 = X0;
                    y0 = Y0;
                }
                x0 += H;
                points[i].X = x0;
                y0 += H * MainFunction(points[i - 1].X, points[i - 1].Y); 
                points[i].Y = y0;
            
            }
            return points;
        }

        public PointD[] RungeKutta(double x, double h)
        {
            int n = (int)Math.Round((x - A) / h);
            PointD[] points = new PointD[n+1];
            points[0].X = X0;
            points[0].Y = Y0;
            for (int i = 1; i <= n; i++)
            {
                points[i].X = A + h * i;
                double f1 = MainFunction(points[i - 1].X, points[i - 1].Y);
                double f2 = MainFunction(points[i - 1].X + (double)(h / 2), points[i - 1].Y + (h * f1 / 2));
                double f3 = MainFunction(points[i - 1].X + (double)(h / 2), points[i - 1].Y + (h*f2 / 2));
                double f4 = MainFunction(points[i - 1].X + h, points[i - 1].Y + h*f3);

                points[i].Y = points[i - 1].Y + h * (double)(f1 + 2 * f2 + 2 * f3 + f4) / 6;
            }
            return points;
        }
    }
}