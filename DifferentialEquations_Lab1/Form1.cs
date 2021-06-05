using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace Diffuri
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            StartProgram();
            
        }
        public void StartProgram()
        {
            double starth = 0.001;
            Methods methods = new Methods(0, 1, 0, 1, Math.Pow(starth, 4), starth);
            PointD[] Exact_Solution = methods.CalcAnalitikSolution();
            PointD[] Eiler = methods.CalcEiler();
            PointD[] RungeKutta = methods.CalcRungeKutta(methods.B, methods.H);
            FillTable(Exact_Solution, Eiler, RungeKutta, methods.H);
            DrawGraphic(Exact_Solution.Length);
        }

        private void DrawGraphic(int length)
        {       
            for (int i = 0; i < length; i++)
            {
                chart1.Series[0].Points.AddXY(dataGridView1.Rows[i].Cells[0].Value, dataGridView1.Rows[i].Cells[1].Value);
                chart1.Series[1].Points.AddXY(dataGridView1.Rows[i].Cells[0].Value, dataGridView1.Rows[i].Cells[2].Value);
                chart1.Series[2].Points.AddXY(dataGridView1.Rows[i].Cells[0].Value, dataGridView1.Rows[i].Cells[3].Value);
                chart2.Series[0].Points.AddXY(dataGridView1.Rows[i].Cells[0].Value, dataGridView1.Rows[i].Cells[5].Value);
                chart2.Series[1].Points.AddXY(dataGridView1.Rows[i].Cells[0].Value, dataGridView1.Rows[i].Cells[4].Value);
            }     
        }

        private void FillTable(PointD[] AnaliticSolution, PointD[] Eiler, PointD[] RungeKutta, double h)
        {
            int n = Eiler.Length;
            dataGridView1.Rows.Add(n);

            for (int i = 0; i < n; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = AnaliticSolution[i].X;
                dataGridView1.Rows[i].Cells[1].Value = AnaliticSolution[i].Y;
                dataGridView1.Rows[i].Cells[2].Value = Eiler[i].Y;
                dataGridView1.Rows[i].Cells[3].Value = RungeKutta[i].Y;
                dataGridView1.Rows[i].Cells[4].Value = Math.Abs((double)dataGridView1.Rows[i].Cells[1].Value - (double)dataGridView1.Rows[i].Cells[2].Value);
                dataGridView1.Rows[i].Cells[5].Value = Math.Abs((double)dataGridView1.Rows[i].Cells[1].Value - (double)dataGridView1.Rows[i].Cells[3].Value);
            }
            double maxDeltaRunge = (double)dataGridView1.Rows[0].Cells[5].Value;
            double maxDeltaEiler = (double)dataGridView1.Rows[0].Cells[4].Value;
            for (int i = 0; i < n; i++)
            {
                if (maxDeltaRunge < (double)dataGridView1.Rows[i].Cells[5].Value)
                    maxDeltaRunge = (double)dataGridView1.Rows[i].Cells[5].Value;
                if (maxDeltaEiler < (double)dataGridView1.Rows[i].Cells[4].Value)
                    maxDeltaEiler = (double)dataGridView1.Rows[i].Cells[4].Value;
            }

            dataGridView1.Rows[0].Cells[6].Value = h;
            dataGridView1.Rows[0].Cells[8].Value = maxDeltaRunge;
            dataGridView1.Rows[0].Cells[7].Value = maxDeltaEiler;
        }

    }
}