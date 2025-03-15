namespace CG_OpenCV
{
    partial class ChessApp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.escolherImagemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.créditosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.funçõesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.negativeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.brightContrastToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redChannelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.translationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scalepointxyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nonUniformToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sobelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.diferentiationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.medianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.histogramGrayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.converttoBWToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToBWOtsuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.histogramRGBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.histogramAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meanSolutionBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.meanSolutionCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.robertsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotationBilinearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scaleBilinearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scalepointxyBilinearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.evalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabuleiro = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ImageViewer = new System.Windows.Forms.PictureBox();
            this.button3 = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageViewer)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Highlight;
            this.menuStrip1.Font = new System.Drawing.Font("Tahoma", 12F);
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.escolherImagemToolStripMenuItem,
            this.créditosToolStripMenuItem,
            this.funçõesToolStripMenuItem,
            this.evalToolStripMenuItem,
            this.undoToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(2414, 37);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // escolherImagemToolStripMenuItem
            // 
            this.escolherImagemToolStripMenuItem.Name = "escolherImagemToolStripMenuItem";
            this.escolherImagemToolStripMenuItem.Size = new System.Drawing.Size(213, 33);
            this.escolherImagemToolStripMenuItem.Text = "Escolher Imagem";
            this.escolherImagemToolStripMenuItem.Click += new System.EventHandler(this.escolherImagemToolStripMenuItem_Click);
            // 
            // créditosToolStripMenuItem
            // 
            this.créditosToolStripMenuItem.Name = "créditosToolStripMenuItem";
            this.créditosToolStripMenuItem.Size = new System.Drawing.Size(115, 33);
            this.créditosToolStripMenuItem.Text = "Créditos";
            this.créditosToolStripMenuItem.Click += new System.EventHandler(this.créditosToolStripMenuItem_Click);
            // 
            // funçõesToolStripMenuItem
            // 
            this.funçõesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.negativeToolStripMenuItem,
            this.brightContrastToolStripMenuItem,
            this.redChannelToolStripMenuItem,
            this.translationToolStripMenuItem,
            this.rotationToolStripMenuItem,
            this.scaleToolStripMenuItem,
            this.scalepointxyToolStripMenuItem,
            this.meanToolStripMenuItem,
            this.nonUniformToolStripMenuItem,
            this.sobelToolStripMenuItem,
            this.diferentiationToolStripMenuItem,
            this.medianToolStripMenuItem,
            this.histogramGrayToolStripMenuItem,
            this.converttoBWToolStripMenuItem,
            this.convertToBWOtsuToolStripMenuItem,
            this.histogramRGBToolStripMenuItem,
            this.histogramAllToolStripMenuItem,
            this.meanSolutionBToolStripMenuItem,
            this.meanSolutionCToolStripMenuItem,
            this.robertsToolStripMenuItem,
            this.rotationBilinearToolStripMenuItem,
            this.scaleBilinearToolStripMenuItem,
            this.scalepointxyBilinearToolStripMenuItem});
            this.funçõesToolStripMenuItem.Name = "funçõesToolStripMenuItem";
            this.funçõesToolStripMenuItem.Size = new System.Drawing.Size(116, 33);
            this.funçõesToolStripMenuItem.Text = "Funções";
            this.funçõesToolStripMenuItem.Click += new System.EventHandler(this.funçõesToolStripMenuItem_Click);
            // 
            // negativeToolStripMenuItem
            // 
            this.negativeToolStripMenuItem.Name = "negativeToolStripMenuItem";
            this.negativeToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.negativeToolStripMenuItem.Text = "Negative";
            this.negativeToolStripMenuItem.Click += new System.EventHandler(this.negativeToolStripMenuItem_Click);
            // 
            // brightContrastToolStripMenuItem
            // 
            this.brightContrastToolStripMenuItem.Name = "brightContrastToolStripMenuItem";
            this.brightContrastToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.brightContrastToolStripMenuItem.Text = "BrightContrast";
            this.brightContrastToolStripMenuItem.Click += new System.EventHandler(this.brightContrastToolStripMenuItem_Click);
            // 
            // redChannelToolStripMenuItem
            // 
            this.redChannelToolStripMenuItem.Name = "redChannelToolStripMenuItem";
            this.redChannelToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.redChannelToolStripMenuItem.Text = "RedChannel";
            this.redChannelToolStripMenuItem.Click += new System.EventHandler(this.redChannelToolStripMenuItem_Click);
            // 
            // translationToolStripMenuItem
            // 
            this.translationToolStripMenuItem.Name = "translationToolStripMenuItem";
            this.translationToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.translationToolStripMenuItem.Text = "Translation";
            this.translationToolStripMenuItem.Click += new System.EventHandler(this.translationToolStripMenuItem_Click);
            // 
            // rotationToolStripMenuItem
            // 
            this.rotationToolStripMenuItem.Name = "rotationToolStripMenuItem";
            this.rotationToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.rotationToolStripMenuItem.Text = "Rotation";
            this.rotationToolStripMenuItem.Click += new System.EventHandler(this.rotationToolStripMenuItem_Click);
            // 
            // scaleToolStripMenuItem
            // 
            this.scaleToolStripMenuItem.Name = "scaleToolStripMenuItem";
            this.scaleToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.scaleToolStripMenuItem.Text = "Scale";
            this.scaleToolStripMenuItem.Click += new System.EventHandler(this.scaleToolStripMenuItem_Click);
            // 
            // scalepointxyToolStripMenuItem
            // 
            this.scalepointxyToolStripMenuItem.Name = "scalepointxyToolStripMenuItem";
            this.scalepointxyToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.scalepointxyToolStripMenuItem.Text = "Scale_point_xy";
            this.scalepointxyToolStripMenuItem.Click += new System.EventHandler(this.scalepointxyToolStripMenuItem_Click);
            // 
            // meanToolStripMenuItem
            // 
            this.meanToolStripMenuItem.Name = "meanToolStripMenuItem";
            this.meanToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.meanToolStripMenuItem.Text = "Mean";
            this.meanToolStripMenuItem.Click += new System.EventHandler(this.meanToolStripMenuItem_Click);
            // 
            // nonUniformToolStripMenuItem
            // 
            this.nonUniformToolStripMenuItem.Name = "nonUniformToolStripMenuItem";
            this.nonUniformToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.nonUniformToolStripMenuItem.Text = "NonUniform";
            this.nonUniformToolStripMenuItem.Click += new System.EventHandler(this.nonUniformToolStripMenuItem_Click);
            // 
            // sobelToolStripMenuItem
            // 
            this.sobelToolStripMenuItem.Name = "sobelToolStripMenuItem";
            this.sobelToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.sobelToolStripMenuItem.Text = "Sobel";
            this.sobelToolStripMenuItem.Click += new System.EventHandler(this.sobelToolStripMenuItem_Click);
            // 
            // diferentiationToolStripMenuItem
            // 
            this.diferentiationToolStripMenuItem.Name = "diferentiationToolStripMenuItem";
            this.diferentiationToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.diferentiationToolStripMenuItem.Text = "Diferentiation";
            this.diferentiationToolStripMenuItem.Click += new System.EventHandler(this.diferentiationToolStripMenuItem_Click);
            // 
            // medianToolStripMenuItem
            // 
            this.medianToolStripMenuItem.Name = "medianToolStripMenuItem";
            this.medianToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.medianToolStripMenuItem.Text = "Median";
            this.medianToolStripMenuItem.Click += new System.EventHandler(this.medianToolStripMenuItem_Click);
            // 
            // histogramGrayToolStripMenuItem
            // 
            this.histogramGrayToolStripMenuItem.Name = "histogramGrayToolStripMenuItem";
            this.histogramGrayToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.histogramGrayToolStripMenuItem.Text = "Histogram_Gray";
            this.histogramGrayToolStripMenuItem.Click += new System.EventHandler(this.histogramGrayToolStripMenuItem_Click);
            // 
            // converttoBWToolStripMenuItem
            // 
            this.converttoBWToolStripMenuItem.Name = "converttoBWToolStripMenuItem";
            this.converttoBWToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.converttoBWToolStripMenuItem.Text = "ConvertToBW";
            this.converttoBWToolStripMenuItem.Click += new System.EventHandler(this.converttoBWToolStripMenuItem_Click);
            // 
            // convertToBWOtsuToolStripMenuItem
            // 
            this.convertToBWOtsuToolStripMenuItem.Name = "convertToBWOtsuToolStripMenuItem";
            this.convertToBWOtsuToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.convertToBWOtsuToolStripMenuItem.Text = "ConvertToBW_Otsu";
            this.convertToBWOtsuToolStripMenuItem.Click += new System.EventHandler(this.convertToBWOtsuToolStripMenuItem_Click);
            // 
            // histogramRGBToolStripMenuItem
            // 
            this.histogramRGBToolStripMenuItem.Name = "histogramRGBToolStripMenuItem";
            this.histogramRGBToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.histogramRGBToolStripMenuItem.Text = "Histogram_RGB";
            this.histogramRGBToolStripMenuItem.Click += new System.EventHandler(this.histogramRGBToolStripMenuItem_Click);
            // 
            // histogramAllToolStripMenuItem
            // 
            this.histogramAllToolStripMenuItem.Name = "histogramAllToolStripMenuItem";
            this.histogramAllToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.histogramAllToolStripMenuItem.Text = "Histogram_All";
            this.histogramAllToolStripMenuItem.Click += new System.EventHandler(this.histogramAllToolStripMenuItem_Click);
            // 
            // meanSolutionBToolStripMenuItem
            // 
            this.meanSolutionBToolStripMenuItem.Name = "meanSolutionBToolStripMenuItem";
            this.meanSolutionBToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.meanSolutionBToolStripMenuItem.Text = "Mean_SolutionB";
            this.meanSolutionBToolStripMenuItem.Click += new System.EventHandler(this.meanSolutionBToolStripMenuItem_Click);
            // 
            // meanSolutionCToolStripMenuItem
            // 
            this.meanSolutionCToolStripMenuItem.Name = "meanSolutionCToolStripMenuItem";
            this.meanSolutionCToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.meanSolutionCToolStripMenuItem.Text = "Mean_SolutionC";
            this.meanSolutionCToolStripMenuItem.Click += new System.EventHandler(this.meanSolutionCToolStripMenuItem_Click);
            // 
            // robertsToolStripMenuItem
            // 
            this.robertsToolStripMenuItem.Name = "robertsToolStripMenuItem";
            this.robertsToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.robertsToolStripMenuItem.Text = "Roberts";
            this.robertsToolStripMenuItem.Click += new System.EventHandler(this.robertsToolStripMenuItem_Click);
            // 
            // rotationBilinearToolStripMenuItem
            // 
            this.rotationBilinearToolStripMenuItem.Name = "rotationBilinearToolStripMenuItem";
            this.rotationBilinearToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.rotationBilinearToolStripMenuItem.Text = "Rotation_Bilinear";
            this.rotationBilinearToolStripMenuItem.Click += new System.EventHandler(this.rotationBilinearToolStripMenuItem_Click);
            // 
            // scaleBilinearToolStripMenuItem
            // 
            this.scaleBilinearToolStripMenuItem.Name = "scaleBilinearToolStripMenuItem";
            this.scaleBilinearToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.scaleBilinearToolStripMenuItem.Text = "Scale_Bilinear";
            this.scaleBilinearToolStripMenuItem.Click += new System.EventHandler(this.scaleBilinearToolStripMenuItem_Click);
            // 
            // scalepointxyBilinearToolStripMenuItem
            // 
            this.scalepointxyBilinearToolStripMenuItem.Name = "scalepointxyBilinearToolStripMenuItem";
            this.scalepointxyBilinearToolStripMenuItem.Size = new System.Drawing.Size(363, 38);
            this.scalepointxyBilinearToolStripMenuItem.Text = "Scale_point_xy_Bilinear";
            this.scalepointxyBilinearToolStripMenuItem.Click += new System.EventHandler(this.scalepointxyBilinearToolStripMenuItem_Click);
            // 
            // evalToolStripMenuItem
            // 
            this.evalToolStripMenuItem.Name = "evalToolStripMenuItem";
            this.evalToolStripMenuItem.Size = new System.Drawing.Size(72, 33);
            this.evalToolStripMenuItem.Text = "Eval";
            this.evalToolStripMenuItem.Click += new System.EventHandler(this.evalToolStripMenuItem_Click);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(84, 33);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.tabuleiro);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.ImageViewer);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 37);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2414, 1112);
            this.panel1.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(1157, 772);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(280, 78);
            this.button2.TabIndex = 14;
            this.button2.Text = "Informação Pretas";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(1157, 609);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(280, 78);
            this.button1.TabIndex = 13;
            this.button1.Text = "Informação Brancas";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Location = new System.Drawing.Point(1606, 147);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(621, 907);
            this.panel2.TabIndex = 11;
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(621, 907);
            this.textBox1.TabIndex = 1;
            // 
            // tabuleiro
            // 
            this.tabuleiro.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.tabuleiro.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabuleiro.Location = new System.Drawing.Point(1157, 281);
            this.tabuleiro.Name = "tabuleiro";
            this.tabuleiro.Size = new System.Drawing.Size(280, 78);
            this.tabuleiro.TabIndex = 10;
            this.tabuleiro.Text = "Recorta Tabuleiro";
            this.tabuleiro.UseVisualStyleBackColor = true;
            this.tabuleiro.Click += new System.EventHandler(this.tabuleiro_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(1395, 35);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(507, 43);
            this.label1.TabIndex = 9;
            this.label1.Text = "Bem-Vindo ao DEISI Chess";
            // 
            // ImageViewer
            // 
            this.ImageViewer.Location = new System.Drawing.Point(66, 147);
            this.ImageViewer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ImageViewer.Name = "ImageViewer";
            this.ImageViewer.Size = new System.Drawing.Size(951, 880);
            this.ImageViewer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ImageViewer.TabIndex = 8;
            this.ImageViewer.TabStop = false;
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(1157, 446);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(280, 76);
            this.button3.TabIndex = 15;
            this.button3.Text = "Informação Tabuleiro";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // ChessApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2414, 1149);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ChessApp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DEISI Chess";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageViewer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem escolherImagemToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox ImageViewer;
        private System.Windows.Forms.ToolStripMenuItem créditosToolStripMenuItem;
        private System.Windows.Forms.Button tabuleiro;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripMenuItem evalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem funçõesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem negativeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem brightContrastToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redChannelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem translationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scaleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scalepointxyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nonUniformToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sobelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem diferentiationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem medianToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem histogramGrayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem converttoBWToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToBWOtsuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem histogramRGBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem histogramAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meanSolutionBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem meanSolutionCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem robertsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotationBilinearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scaleBilinearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scalepointxyBilinearToolStripMenuItem;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.Button button3;
    }
}