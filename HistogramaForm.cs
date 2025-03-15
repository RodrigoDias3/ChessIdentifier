using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ZedGraph;

namespace CG_OpenCV
{
    public partial class HistogramaForm : Form
    {
        public HistogramaForm(int[,]  histograma, int num)
        {
            InitializeComponent();
            GraphPane myPane = zedGraphControl1.GraphPane;

            PointPairList blueList = new PointPairList();
            PointPairList greenList = new PointPairList();
            PointPairList redList = new PointPairList();
            PointPairList grayList = new PointPairList();

            for (int i = 0; i < 256; i++)
            {

                grayList.Add(i, histograma[0, i]);
                blueList.Add(i,histograma[1,i]);
                greenList.Add(i, histograma[2, i]);
                redList.Add(i, histograma[3, i]);
            }

            myPane.AddCurve("Grsy", grayList, System.Drawing.Color.Gray);
            myPane.AddCurve("Blue", blueList, System.Drawing.Color.Blue);
            myPane.AddCurve("Green", greenList, System.Drawing.Color.Green);
            myPane.AddCurve("Red", redList, System.Drawing.Color.Red);

            myPane.AxisChange();
            zedGraphControl1.Refresh();
        }

        public HistogramaForm(int[,] histograma)
        {
            InitializeComponent();
            GraphPane myPane = zedGraphControl1.GraphPane;

            PointPairList blueList = new PointPairList();
            PointPairList greenList = new PointPairList();
            PointPairList redList = new PointPairList();

            for (int i = 0; i < 256; i++)
            {
                blueList.Add(i, histograma[0, i]);
                greenList.Add(i, histograma[1, i]);
                redList.Add(i, histograma[2, i]);
            }

            myPane.AddCurve("Blue", blueList, System.Drawing.Color.Blue);
            myPane.AddCurve("Green", greenList, System.Drawing.Color.Green);
            myPane.AddCurve("Red", redList, System.Drawing.Color.Red);

            myPane.AxisChange();
            zedGraphControl1.Refresh();
        }

        public HistogramaForm(int[] histograma)
        {
            InitializeComponent();
            GraphPane myPane = zedGraphControl1.GraphPane;

            PointPairList grayList = new PointPairList();

            for (int i = 0; i < 256; i++)
            {
                grayList.Add(i, histograma[i]);
            }

            myPane.AddCurve("Gray", grayList, System.Drawing.Color.Gray);

            myPane.AxisChange();
            zedGraphControl1.Refresh();
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void HistogramaForm_Load(object sender, EventArgs e)
        {

        }
    }
}
