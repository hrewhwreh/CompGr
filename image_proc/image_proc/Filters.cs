using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace image_proc
{
    abstract class Filters
    {
        protected abstract Color CalculateNewPixelColor(Bitmap SourceImage, int x, int y);
        
        public Bitmap processImage(Bitmap SourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(SourceImage.Width, SourceImage.Height);
            for (int i = 0; i < SourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                {
                    return null;
                }
                for (int j = 0; j < SourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, CalculateNewPixelColor(SourceImage, i, j));
                }
            }
            return resultImage;
        }

        public int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }
    }

    class InvertFilter : Filters
    {
        protected override Color CalculateNewPixelColor(Bitmap SourceImage, int x, int y)
        {
            Color SourceColor = SourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - SourceColor.R, 255 - SourceColor.G, 255 - SourceColor.B);
            return resultColor;
        }
    }

    class GrayScaleFilter : Filters
    {
        protected override Color CalculateNewPixelColor(Bitmap SourceImage, int x, int y)
        {
            Color SourceColor = SourceImage.GetPixel(x, y);
            int Intensity = (int)(0.299 * SourceColor.R + 0.587 * SourceColor.G + 0.114 * SourceColor.B);
            Color resultColor = Color.FromArgb(Intensity, Intensity, Intensity);
            return resultColor;
        }
    }

    class SepiaFilter : Filters
    {
        protected override Color CalculateNewPixelColor(Bitmap SourceImage, int x, int y)
        {
            Color SourceColor = SourceImage.GetPixel(x, y);
            int Intensity = (int)(0.299 * SourceColor.R + 0.587 * SourceColor.G + 0.114 * SourceColor.B);
            int k = 50;
            Color resultColor = Color.FromArgb(Clamp(Intensity + 2 * k, 0, 255), Clamp((int)(Intensity + 0.5 * k), 0, 255), Clamp((int)(Intensity - k), 0, 255));
            return resultColor;
        }
    }

    class BrightFilter : Filters
    {
        protected override Color CalculateNewPixelColor(Bitmap SourceImage, int x, int y)
        {
            Color SourceColor = SourceImage.GetPixel(x, y);
            int k = 50;
            Color resultColor = Color.FromArgb(Clamp(SourceColor.R + k, 0, 255), Clamp(SourceColor.G + k, 0, 255), Clamp(SourceColor.B + k, 0, 255));
            return resultColor;
        }
    }

    class WavesFilter : Filters
    {
        protected override Color CalculateNewPixelColor(Bitmap SourceImage, int x, int y)
        {
            Color SourceColor = SourceImage.GetPixel(Clamp(x + (int)(20 * Math.Sin(2 * Math.PI * y / 60)), 0, SourceImage.Width - 1), y);
            Color resultColor = Color.FromArgb(Clamp(SourceColor.R, 0, 255), Clamp(SourceColor.G, 0, 255), Clamp(SourceColor.B, 0, 255));
            return resultColor;
        }
    }

    class GlassFilter : Filters
    {
        Random rnd = new Random();
        protected override Color CalculateNewPixelColor(Bitmap SourceImage, int x, int y)
        {
            Color SourceColor = SourceImage.GetPixel(Clamp(x + (int)((rnd.NextDouble() - 0.5) * 10), 0, SourceImage.Width - 1), Clamp(y + (int)((rnd.NextDouble() - 0.5) * 10), 0, SourceImage.Height - 1));
            Color resultColor = Color.FromArgb(Clamp(SourceColor.R, 0, 255), Clamp(SourceColor.G, 0, 255), Clamp(SourceColor.B, 0, 255));
            return resultColor;
        }
    }

    class MatrixFilters : Filters
    {
        protected float[,] kernel = null;
        protected MatrixFilters() { }
        public MatrixFilters(float[,] kernel)
        {
            this.kernel = kernel;
        }

        protected override Color CalculateNewPixelColor(Bitmap SourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, SourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, SourceImage.Height - 1);
                    Color neighborColor = SourceImage.GetPixel(idX, idY);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            }
            return Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
        }
    }

    class BlurFilter : MatrixFilters
    {
        public BlurFilter()
        {
            kernel = new float[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    kernel[i, j] = 1.0f / (float)(9);
                }
            }
        }
    }

    class GaussianFilter : MatrixFilters
    {
        public void createGaussianKernel(int radius, float sigma)
        {
            int size = 2 * radius + 1;
            kernel = new float[size, size];
            float norm = 0;
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (2 * sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    kernel[i, j] /= norm;
                }
            }
        }

        public GaussianFilter()
        {
            createGaussianKernel(3, 2);
        }
    }

    class SharpenFilter : MatrixFilters
    {
        public SharpenFilter()
        {
            kernel = new float[3, 3];
            kernel[0, 0] = 0;
            kernel[1, 0] = -1;
            kernel[2, 0] = 0;
            kernel[0, 1] = -1;
            kernel[1, 1] = 5;
            kernel[2, 1] = -1;
            kernel[0, 2] = 0;
            kernel[1, 2] = -1;
            kernel[2, 2] = 0;
        }
    }

    class EmbossFilter : MatrixFilters
    {
        public EmbossFilter()
        {
            kernel = new float[3, 3];
            kernel[0, 0] = 0;
            kernel[1, 0] = 1;
            kernel[2, 0] = 0;
            kernel[0, 1] = 1;
            kernel[1, 1] = 0;
            kernel[2, 1] = -1;
            kernel[0, 2] = 0;
            kernel[1, 2] = -1;
            kernel[2, 2] = 0;
        }

        protected override Color CalculateNewPixelColor(Bitmap SourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, SourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, SourceImage.Height - 1);
                    Color neighborColor = SourceImage.GetPixel(idX, idY);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }
            }
            return Color.FromArgb(Clamp((int)((resultR + 255) / 2), 0, 255), Clamp((int)((resultG + 255) / 2), 0, 255), Clamp((int)((resultB + 255) / 2), 0, 255));
        }
    }

    class MotionBlurFilter : MatrixFilters
    {
        public MotionBlurFilter()
        {
            kernel = new float[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    kernel[i, j] = 0;
                }
            }
            for (int i = 0; i < 9; i++)
            {
                kernel[i, i] = 1.0f / (float)(9);
            }
        }
    }

    class MedianFilter : MatrixFilters
    {
        public MedianFilter()
        {
            kernel = new float[5, 5];
        }

        protected override Color CalculateNewPixelColor(Bitmap SourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            int[] colorR, colorG, colorB;
            colorR = new int[25];
            colorG = new int[25];
            colorB = new int[25];
            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, SourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, SourceImage.Height - 1);
                    Color neighborColor = SourceImage.GetPixel(idX, idY);
                    colorR[(l + radiusY) * 5 + radiusX + k] = neighborColor.R;
                    colorG[(l + radiusY) * 5 + radiusX + k] = neighborColor.G;
                    colorB[(l + radiusY) * 5 + radiusX + k] = neighborColor.B;
                }
            }
            Array.Sort(colorR);
            Array.Sort(colorG);
            Array.Sort(colorB);
            return Color.FromArgb(colorR[12], colorG[12], colorB[12]);
        }
    }

    class MaximumFilter : MatrixFilters
    {
        public MaximumFilter()
        {
            kernel = new float[5, 5];
        }

        protected override Color CalculateNewPixelColor(Bitmap SourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            int[] colorR, colorG, colorB;
            colorR = new int[25];
            colorG = new int[25];
            colorB = new int[25];
            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, SourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, SourceImage.Height - 1);
                    Color neighborColor = SourceImage.GetPixel(idX, idY);
                    colorR[(l + radiusY) * 5 + radiusX + k] = neighborColor.R;
                    colorG[(l + radiusY) * 5 + radiusX + k] = neighborColor.G;
                    colorB[(l + radiusY) * 5 + radiusX + k] = neighborColor.B;
                }
            }
            Array.Sort(colorR);
            Array.Sort(colorG);
            Array.Sort(colorB);
            return Color.FromArgb(colorR[24], colorG[24], colorB[24]);
        }
    }

    class ManyAxisFilter : Filters
    {
        protected float[,] kernelX = null;
        protected float[,] kernelY = null;
        protected ManyAxisFilter() { }
        public ManyAxisFilter(float[,] kernelX, float[,] kernelY)
        {
            this.kernelX = kernelX;
            this.kernelY = kernelY;
        }

        protected override Color CalculateNewPixelColor(Bitmap SourceImage, int x, int y)
        {
            int radiusX_X = kernelX.GetLength(0) / 2;
            int radiusY_X = kernelX.GetLength(1) / 2;
            int radiusX_Y = kernelY.GetLength(0) / 2;
            int radiusY_Y = kernelY.GetLength(1) / 2;
            float resultR_X = 0;
            float resultG_X = 0;
            float resultB_X = 0;
            float resultR_Y = 0;
            float resultG_Y = 0;
            float resultB_Y = 0;
            for (int l = -radiusY_X; l <= radiusY_X; l++)
            {
                for (int k = -radiusX_X; k <= radiusX_X; k++)
                {
                    int idX = Clamp(x + k, 0, SourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, SourceImage.Height - 1);
                    Color neighborColor = SourceImage.GetPixel(idX, idY);
                    resultR_X += neighborColor.R * kernelX[k + radiusX_X, l + radiusY_X];
                    resultG_X += neighborColor.G * kernelX[k + radiusX_X, l + radiusY_X];
                    resultB_X += neighborColor.B * kernelX[k + radiusX_X, l + radiusY_X];
                }
            }
            for (int l = -radiusY_Y; l <= radiusY_Y; l++)
            {
                for (int k = -radiusX_Y; k <= radiusX_Y; k++)
                {
                    int idX = Clamp(x + k, 0, SourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, SourceImage.Height - 1);
                    Color neighborColor = SourceImage.GetPixel(idX, idY);
                    resultR_Y += neighborColor.R * kernelY[k + radiusX_Y, l + radiusY_Y];
                    resultG_Y += neighborColor.G * kernelY[k + radiusX_Y, l + radiusY_Y];
                    resultB_Y += neighborColor.B * kernelY[k + radiusX_Y, l + radiusY_Y];
                }
            }
            int resultR = Clamp((int)(Math.Sqrt(resultR_X * resultR_X + resultR_Y * resultR_Y)), 0, 255);
            int resultG = Clamp((int)(Math.Sqrt(resultG_X * resultG_X + resultG_Y * resultG_Y)), 0, 255);
            int resultB = Clamp((int)(Math.Sqrt(resultB_X * resultB_X + resultB_Y * resultB_Y)), 0, 255);
            return Color.FromArgb(resultR, resultG, resultB);
        }
    }

    class SobelFilter : ManyAxisFilter
    {
        public SobelFilter()
        {
            kernelX = new float[3, 3];
            kernelY = new float[3, 3];
            kernelX[0, 0] = -1;
            kernelX[1, 0] = 0;
            kernelX[2, 0] = 1;
            kernelX[0, 1] = -2;
            kernelX[1, 1] = 0;
            kernelX[2, 1] = 2;
            kernelX[0, 2] = -1;
            kernelX[1, 2] = 0;
            kernelX[2, 2] = 1;
            
            kernelY[0, 0] = -1;
            kernelY[1, 0] = -2;
            kernelY[2, 0] = -1;
            kernelY[0, 1] = 0;
            kernelY[1, 1] = 0;
            kernelY[2, 1] = 0;
            kernelY[0, 2] = 1;
            kernelY[1, 2] = 2;
            kernelY[2, 2] = 1;
        }
    }

    class ScharrFilter : ManyAxisFilter
    {
        public ScharrFilter()
        {
            kernelX = new float[3, 3];
            kernelY = new float[3, 3];
            kernelX[0, 0] = 3;
            kernelX[1, 0] = 0;
            kernelX[2, 0] = -3;
            kernelX[0, 1] = 10;
            kernelX[1, 1] = 0;
            kernelX[2, 1] = -10;
            kernelX[0, 2] = 3;
            kernelX[1, 2] = 0;
            kernelX[2, 2] = -3;
            
            kernelY[0, 0] = 3;
            kernelY[1, 0] = 10;
            kernelY[2, 0] = 3;
            kernelY[0, 1] = 0;
            kernelY[1, 1] = 0;
            kernelY[2, 1] = 0;
            kernelY[0, 2] = -3;
            kernelY[1, 2] = -10;
            kernelY[2, 2] = -3;
        }
    }
}
