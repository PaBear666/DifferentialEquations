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

namespace Diffuri2
{
    public partial class Okno : Form
    {
        Methods methods;
        public Okno()
        {
            InitializeComponent();
            methods = new Methods(0, 1, 0.1, 0, 1, 0, 2);
            PointD[] gridMethod = methods.GridMethod();
            PointD[] analiticSolution = methods.AnaliticSolution();
            Graphic(gridMethod, analiticSolution);
            Table(gridMethod, analiticSolution);     
        }
        public void Graphic(PointD[] gridMethod, PointD[] analiticSolution)
        {
            for (int i = 0; i < gridMethod.Length; i++)
            {
                chart.Series[0].Points.AddXY(analiticSolution[i].X, analiticSolution[i].Y);
                chart.Series[1].Points.AddXY(gridMethod[i].X, gridMethod[i].Y);
                chart1.Series[0].Points.AddXY(gridMethod[i].X, Math.Abs(analiticSolution[i].Y - gridMethod[i].Y));
            }
        }
        public void Table(PointD[] gridMethod, PointD[] analiticSolution)
        {
            var eps = Methods.EpsRunge(methods);
            var methods2h = Methods.Create2H(methods).GridMethod();
            table1.Rows.Add(gridMethod.Length);
            table1.Rows[0].Cells[5].Value = eps.Max();
            for (int i = 0; i < gridMethod.Length; i++)
            {
                table1.Rows[i].Cells[0].Value = analiticSolution[i].X;
                table1.Rows[i].Cells[1].Value = analiticSolution[i].Y;
                table1.Rows[i].Cells[2].Value = gridMethod[i].Y;
                table1.Rows[i].Cells[3].Value = Math.Abs(gridMethod[i].Y - analiticSolution[i].Y);
                if (i == 0)
                    table1.Rows[0].Cells[4].Value = table1.Rows[i].Cells[3].Value;
                else if((double)table1.Rows[i].Cells[3].Value > (double)table1.Rows[0].Cells[4].Value)
                {
                    table1.Rows[0].Cells[4].Value = table1.Rows[i].Cells[3].Value;
                }
                table1.Rows[i].Cells[6].Value = i % 2 == 0 ? methods2h[i / 2].Y : 0;
            }
            
        }
    }
}
