using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CG_OpenCV
{
    public partial class ChessApp : Form
    {
        Image<Bgr, Byte> img = null;
        Image<Bgr, Byte> imgUndo = null;
        string title_bak = "";

        string[][] tab = new string[8][];

        Dictionary<string, string> nomeDasPecas = new Dictionary<string, string>
        {
            {"cavaloBranco", "Cavalo B"},
            {"torreBranco", "Torre B"},
            {"rainhaBranco", "Rainha B"},
            {"bispoBranco", "Bispo B"},
            {"reiBranco", "Rei B"},
            {"peaoBranco", "Peão B"},
            {"cavaloPreto", "Cavalo P"},
            {"torrePreto", "Torre P"},
            {"rainhaPreto", "Rainha P"},
            {"bispoPreto", "Bispo P"},
            {"reiPreto", "Rei P"},
            {"peaoPreto", "Peão P"},
            {"VAZIA", "Vazia"}
        };

        public ChessApp()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Opens a new image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void escolherImagemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                img = new Image<Bgr, byte>(openFileDialog1.FileName);
                imgUndo = img.Copy();
                ImageViewer.Image = img.Bitmap;
                ImageViewer.Refresh();
            }
        }

        private void créditosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AuthorsForm form = new AuthorsForm();
            form.ShowDialog();
        }

        private void tabuleiro_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            int rodou = 0;

            if (ImageClass.vefUniform(imgUndo))
            {
                for (int i = 0; i < 2; i++)
                {
                    //copy Undo Image
                    imgUndo = img.Copy();

                    double tab = ImageClass.vefCasaVazia(img);

                    ImageClass.BrightContrast(img, -80, 1.6);
                    ImageClass.BrightContrast(img, -80, 1.6);
                    ImageClass.ConvertToBW_HSV(img);
                    rodou = ImageClass.roda(img, imgUndo, rodou);
                    img = ImageClass.recortaImg(img, imgUndo);
                }

                if (rodou == 1)
                {
                    img = ImageClass.corta_aux_2(img);
                }
            }
            else
            {
                imgUndo = img.Copy();

                double tab = ImageClass.vefCasaVazia(img);

                ImageClass.BrightContrast(img, -80, 1.6);
                ImageClass.BrightContrast(img, -80, 1.6);
                ImageClass.BrightContrast(img, -80, 1.6);
                ImageClass.ConvertToBW_HSV(img);
                ImageClass.limpa(img, img.Copy());
                ImageClass.testeN3(img);
                img = ImageClass.recortaImg(img, imgUndo);

            }

            tab = ImageClass.RegistaPecas(img, img.Copy());

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void ExibirTabuleiro(string[][] tabuleiro)
        {
            textBox1.Clear();

            for (int i = 0; i < tabuleiro.Length; i++)
            {
                textBox1.AppendText(" ***** LINHA " + i + " *****");
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText(Environment.NewLine);

                for (int j = 0; j < tabuleiro[i].Length; j++)
                {
                    string peçaAtual = tabuleiro[i][j];
                    if (nomeDasPecas.ContainsKey(peçaAtual))
                    {
                        peçaAtual = nomeDasPecas[peçaAtual];
                    }
                    textBox1.AppendText("casa " + j + ": " + peçaAtual);
                    textBox1.AppendText(Environment.NewLine);
                }
                textBox1.AppendText(Environment.NewLine);

                if (i < tabuleiro.Length - 1)
                {
                    textBox1.AppendText(new string('-', tabuleiro[i].Length * 5 + (tabuleiro[i].Length - 1)) + Environment.NewLine);
                    textBox1.AppendText(Environment.NewLine);
                }
            }
        }

        private void evalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EvalForm eval = new EvalForm();
            eval.ShowDialog();
        }

        private void funçõesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            InfoEquipa form = new InfoEquipa(tab,"Brancas");
            form.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            InfoEquipa form = new InfoEquipa(tab, "Pretas");
            form.ShowDialog();
        }

        private void negativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Negative(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (imgUndo == null)
                return;
            Cursor = Cursors.WaitCursor;
            img = imgUndo.Copy();

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void brightContrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.BrightContrast(img, -80, 1.6);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void redChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.RedChannel(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void translationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.Translation(img, img.Copy(), -10, -10);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void rotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.Rotation(img, img.Copy(), Math.PI/6); //30º

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void scaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.Scale(img, img.Copy(), 1.5f);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void scalepointxyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.Scale_point_xy(img, img.Copy(), 1.5f, 250, 310);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void meanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.Mean(img, img.Copy());

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void nonUniformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            // ImageClass.NonUniform(img, img.Copy());

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void sobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.Sobel(img, img.Copy());

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void diferentiationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.Diferentiation(img, img.Copy());

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void medianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.Median(img, img.Copy());

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void histogramGrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.Histogram_Gray(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void converttoBWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.ConvertToBW(img, 157);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void convertToBWOtsuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.ConvertToBW_Otsu(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void histogramRGBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.Histogram_RGB(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void histogramAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.Histogram_All(img);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void meanSolutionBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.Mean_solutionB(img,img.Copy());

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void meanSolutionCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.Mean_solutionC(img,img.Copy(),7);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void robertsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.Roberts(img,img.Copy());

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void rotationBilinearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.Rotation_Bilinear(img,img.Copy(),0.5f);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void scaleBilinearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.Scale_Bilinear(img,img.Copy(),0.5f);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void scalepointxyBilinearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null)
                return;
            Cursor = Cursors.WaitCursor;

            imgUndo = img.Copy();

            ImageClass.Scale_point_xy_Bilinear(img,img.Copy(),2.2f,250, 310);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh();

            Cursor = Cursors.Default;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ExibirTabuleiro(tab);

            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }
    }
}
