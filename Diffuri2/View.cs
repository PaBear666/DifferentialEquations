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
    public partial class View : Form
    {
        Controller controller;
        public View()
        {
            InitializeComponent();
            controller = new Controller(0, 1, 10, 2, 4, -4, 2);
            PointD[] gridMethod = controller.GetResultGridMethod();
            PointD[] exactSolution = controller.GetPointDExactSolution();
            DrawGraphic(gridMethod, exactSolution);
            FillTable(gridMethod, exactSolution);
            
        }

        public void DrawGraphic(PointD[] GridMethod,PointD[] ExactSolution)
        {
            for (int i = 0; i < GridMethod.Length; i++)
            {
                chart.Series[0].Points.AddXY(ExactSolution[i].X,ExactSolution[i].Y);
                chart.Series[1].Points.AddXY(GridMethod[i].X, GridMethod[i].Y);
            }
        }
        public void FillTable(PointD[] gridMethod, PointD[] exactSolution)
        {
            var controller2H = new Controller(0, 1, 5, 2, 4, -4, 2);
            var esp = Controller.GetEspRunge(gridMethod, controller2H.GetResultGridMethod());
            table1.Rows.Add(gridMethod.Length);
            for (int i = 0; i < gridMethod.Length; i++)
            {
                table1.Rows[i].Cells[0].Value = exactSolution[i].X;
                table1.Rows[i].Cells[1].Value = exactSolution[i].Y;
                table1.Rows[i].Cells[2].Value = gridMethod[i].Y;
                table1.Rows[i].Cells[3].Value = Math.Abs(gridMethod[i].Y - exactSolution[i].Y);
                table1.Rows[0].Cells[4].Value = i.Equals(0) ? table1.Rows[0].Cells[3].Value : 
                                                (double)table1.Rows[0].Cells[4].Value < (double)table1.Rows[i].Cells[3].Value ? 
                                                    (double)table1.Rows[i].Cells[3].Value : (double)table1.Rows[0].Cells[4].Value;
                
            }
            table1.Rows[0].Cells[5].Value = esp.Max();
            
        }

        private void View_Load(object sender, EventArgs e)
        {
            
        }
    }
}
