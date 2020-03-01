using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace image_proc
{
    public partial class Form1 : Form
    {
        Bitmap Image;
        public Form1()
        {
            InitializeComponent();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files|*.png;*.jpg;*.bmp|All files(*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Image = new Bitmap(dialog.FileName);
                pictureBox1.Image = Image;
                pictureBox1.Refresh();
            }
        }

        private void инверсияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new InvertFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filters)e.Argument).processImage(Image, backgroundWorker1);
            if (backgroundWorker1.CancellationPending != true)
            {
                Image = newImage;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                pictureBox2.Image = Image;
                pictureBox2.Refresh();
            }
            progressBar1.Value = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void blurFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void gaussianFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GaussianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void grayScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayScaleFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SepiaFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void brightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new BrightFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void sobelFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SobelFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void sharpenFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SharpenFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void тиснениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new EmbossFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void wavesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new WavesFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void glassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GlassFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void motionBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MotionBlurFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void scharrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new ScharrFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void medianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MedianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void maximumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MaximumFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void brightEndToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter1 = new MedianFilter();
            Filters filter2 = new ScharrFilter();
            Filters filter3 = new MaximumFilter();
            Bitmap resultImage = filter1.processImage(Image, backgroundWorker1);
            resultImage = filter2.processImage(resultImage, backgroundWorker1);
            resultImage = filter3.processImage(resultImage, backgroundWorker1);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
            progressBar1.Value = 0;
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog savedialog = new SaveFileDialog();
            savedialog.OverwritePrompt = true;
            savedialog.CheckPathExists = true;
            savedialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
            savedialog.ShowHelp = true;
            if (savedialog.ShowDialog() == DialogResult.OK) //если в диалоговом окне нажата кнопка "ОК"
            {
                pictureBox2.Image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void contrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new ContrastFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void grayWorldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayWorldFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private float[,] structElem()
        {
            float[,] result;
            result = new float[5, 5];
            result[0, 0] = (float)Convert.ToDouble(textBox1.Text);
            result[1, 0] = (float)Convert.ToDouble(textBox2.Text);
            result[2, 0] = (float)Convert.ToDouble(textBox3.Text);
            result[3, 0] = (float)Convert.ToDouble(textBox4.Text);
            result[4, 0] = (float)Convert.ToDouble(textBox5.Text);
            result[0, 1] = (float)Convert.ToDouble(textBox6.Text);
            result[1, 1] = (float)Convert.ToDouble(textBox7.Text);
            result[2, 1] = (float)Convert.ToDouble(textBox8.Text);
            result[3, 1] = (float)Convert.ToDouble(textBox9.Text);
            result[4, 1] = (float)Convert.ToDouble(textBox10.Text);
            result[0, 0] = (float)Convert.ToDouble(textBox11.Text);
            result[1, 2] = (float)Convert.ToDouble(textBox12.Text);
            result[2, 2] = (float)Convert.ToDouble(textBox13.Text);
            result[3, 2] = (float)Convert.ToDouble(textBox14.Text);
            result[4, 2] = (float)Convert.ToDouble(textBox15.Text);
            result[0, 3] = (float)Convert.ToDouble(textBox16.Text);
            result[1, 3] = (float)Convert.ToDouble(textBox17.Text);
            result[2, 3] = (float)Convert.ToDouble(textBox18.Text); 
            result[3, 3] = (float)Convert.ToDouble(textBox19.Text);
            result[4, 3] = (float)Convert.ToDouble(textBox20.Text);
            result[0, 4] = (float)Convert.ToDouble(textBox21.Text);
            result[1, 4] = (float)Convert.ToDouble(textBox22.Text);
            result[2, 4] = (float)Convert.ToDouble(textBox23.Text);
            result[3, 4] = (float)Convert.ToDouble(textBox24.Text);
            result[4, 4] = (float)Convert.ToDouble(textBox25.Text);
            return result;
        }

        private void dilationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Dilation(structElem());
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void erosionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Erosion(structElem());
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void openingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter1 = new Erosion(structElem());
            Filters filter2 = new Dilation(structElem());
            Bitmap resultImage = filter1.processImage(Image, backgroundWorker1);
            resultImage = filter2.processImage(resultImage, backgroundWorker1);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
            progressBar1.Value = 0;
        }

        private void closingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter1 = new Dilation(structElem());
            Filters filter2 = new Erosion(structElem());
            Bitmap resultImage = filter1.processImage(Image, backgroundWorker1);
            resultImage = filter2.processImage(resultImage, backgroundWorker1);
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
            progressBar1.Value = 0;
        }

        private void gradientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter1 = new Dilation(structElem());
            Filters filter2 = new Erosion(structElem());
            Bitmap resultImage1 = filter1.processImage(Image, backgroundWorker1);
            Bitmap resultImage2 = filter2.processImage(Image, backgroundWorker1);
            Bitmap resultImage = new Bitmap(Image.Width, Image.Height);
            for (int i = 0; i < Image.Width; i++)
            {
                for (int j = 0; j < Image.Height; j++)
                {
                    resultImage.SetPixel(i, j, Color.FromArgb(resultImage1.GetPixel(i, j).R - resultImage2.GetPixel(i, j).R, resultImage1.GetPixel(i, j).G - resultImage2.GetPixel(i, j).G, resultImage1.GetPixel(i, j).B - resultImage2.GetPixel(i, j).B));
                }
            }
            pictureBox2.Image = resultImage;
            pictureBox2.Refresh();
            progressBar1.Value = 0;
        }
    }
}
