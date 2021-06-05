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
        private void DrawGraphic(PointD[] Exact_Solution, PointD[] Eiler, PointD[] RungeKutta)
        {
            chart1.Series[2].BorderWidth = 2;
            chart1.Series[2].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            for (int i = 0; i < Eiler.Length; i++)
            {
                chart1.Series[0].Points.AddXY(dataGridView1.Rows[i].Cells[0].Value, dataGridView1.Rows[i].Cells[1].Value);
                chart1.Series[1].Points.AddXY(dataGridView1.Rows[i].Cells[0].Value, dataGridView1.Rows[i].Cells[2].Value);
                chart1.Series[2].Points.AddXY(dataGridView1.Rows[i].Cells[0].Value, dataGridView1.Rows[i].Cells[3].Value);
            }
            
        }

        private void FillTable(PointD[] Exact_Solution, PointD[] Eiler, PointD[] RungeKutta,double h)
        {
            int n = Eiler.Length;
            dataGridView1.Rows.Add(n);

            for (int i = 0; i < n; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = Exact_Solution[i].X;
                dataGridView1.Rows[i].Cells[1].Value = Exact_Solution[i].Y;
                dataGridView1.Rows[i].Cells[2].Value = Eiler[i].Y;
                dataGridView1.Rows[i].Cells[3].Value = RungeKutta[i].Y;
            }
            for (int i = 0; i < n; i++)
            {
                //#region
                //dataGridView1.Rows[i].Cells[2].Value = i != 0 ? Eiler[i].Y - 0.08 : Eiler[i].Y;
                //dataGridView1.Rows[i].Cells[3].Value = i != 0 ? RungeKutta[i].Y + 0.0002 : RungeKutta[i].Y;
                //#endregion
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
            dataGridView2.Rows.Add(1);
            dataGridView2.Rows[0].Cells[0].Value = h;
            dataGridView2.Rows[0].Cells[1].Value = maxDeltaRunge;
            dataGridView2.Rows[0].Cells[2].Value = maxDeltaEiler;
        }

        public void StartProgram()
        {
            double h0 = 0.001;
            Contoller controller = new Contoller(0,1,0,1,Math.Pow(h0,4),h0);
            PointD[] Exact_Solution = controller.Exact_Solution();
            PointD[] Eiler = controller.Eiler();
            PointD[] RungeKutta = controller.RungeKutta(controller.B, controller.H);       
            FillTable(Exact_Solution, Eiler, RungeKutta, controller.H);
            DrawGraphic(Exact_Solution, Eiler, RungeKutta);
        }
    }
}