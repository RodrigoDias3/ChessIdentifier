using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ZedGraph;
using System.Drawing;
using System.IO;
using System.Linq;
using Emgu.CV.CvEnum;
using System.Runtime.Remoting.Channels;
using System.Data;

namespace CG_OpenCV
{
    class ImageClass
    {

        /// <summary>
        /// Image Negative using EmguCV library
        /// Slower method
        /// </summary>
        /// <param name="img">Image</param>
        public static void Negative(Image<Bgr, byte> img)
        {
            int x, y;

            Bgr aux;
            for (y = 0; y < img.Height; y++)
            {
                for (x = 0; x < img.Width; x++)
                {
                    // acesso directo : mais lento 
                    aux = img[y, x];
                    img[y, x] = new Bgr(255 - aux.Blue, 255 - aux.Green, 255 - aux.Red);
                }
            }
        }

        /// <summary>
        /// Convert to gray
        /// Direct access to memory - faster method
        /// </summary>
        /// <param name="img">image</param>
        public static void ConvertToGray(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red, gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrive 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            // convert to gray
                            gray = (byte)Math.Round(((int)blue + green + red) / 3.0);

                            // store in the image
                            dataPtr[0] = gray;
                            dataPtr[1] = gray;
                            dataPtr[2] = gray;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        public static void BrightContrast(Image<Bgr, byte> img, int bright, double contrast)
        {
            unsafe
            {

                MIplImage m = img.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;

                byte blue, green, red;
                int blue_bright, green_bright, red_bright;
                int x, y;

                if (nChannel == 3)
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            blue_bright = (int)Math.Round((contrast * blue) + bright);
                            if (blue_bright > 255)
                                blue_bright = 255;
                            if (blue_bright < 0)
                                blue_bright = 0;

                            green_bright = (int)Math.Round((contrast * green) + bright);
                            if (green_bright > 255)
                                green_bright = 255;
                            if (green_bright < 0)
                                green_bright = 0;

                            red_bright = (int)Math.Round((contrast * red) + bright);
                            if (red_bright > 255)
                                red_bright = 255;
                            if (red_bright < 0)
                                red_bright = 0;

                            dataPtr[0] = (byte)blue_bright;
                            dataPtr[1] = (byte)green_bright;
                            dataPtr[2] = (byte)red_bright;

                            dataPtr += nChannel;
                        }
                        dataPtr += padding;
                    }
                }
            }
        }

        public static void RedChannel(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;

                int x, y;
                byte red;

                if (nChannel == 3)
                {
                    for (y = 0; y < img.Height; y++)
                    {
                        for (x = 0; x < img.Width; x++)
                        {
                            red = dataPtr[2];

                            dataPtr[0] = red;
                            dataPtr[1] = red;
                            dataPtr[2] = red;

                            dataPtr += nChannel;
                        }
                        dataPtr += padding;
                    }
                }
            }
        }

        public static void BlueChannel(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;

                int x, y;
                byte blue;

                if (nChannel == 3)
                {
                    for (y = 0; y < img.Height; y++)
                    {
                        for (x = 0; x < img.Width; x++)
                        {
                            blue = dataPtr[0];

                            dataPtr[0] = blue;
                            dataPtr[1] = blue;
                            dataPtr[2] = blue;

                            dataPtr += nChannel;
                        }
                        dataPtr += padding;
                    }
                }
            }
        }

        public static void Translation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int dx, int dy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrCopy = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChannel = mUndo.nChannels;
                int padding = mUndo.widthStep - mUndo.nChannels * mUndo.width;
                int widthstep = mUndo.widthStep;

                byte red, green, blue;
                int x_o, y_o;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x_o = x - dx;
                        y_o = y - dy;

                        if (x_o < width && x_o >= 0 && y_o < height && y_o >= 0)
                        {
                            blue = (dataPtrCopy + y_o * widthstep + x_o * nChannel)[0];
                            green = (dataPtrCopy + y_o * widthstep + x_o * nChannel)[1];
                            red = (dataPtrCopy + y_o * widthstep + x_o * nChannel)[2];
                        }
                        else
                        {
                            red = green = blue = 0;
                        }

                        (dataPtr + y * widthstep + x * nChannel)[0] = blue;
                        (dataPtr + y * widthstep + x * nChannel)[1] = green;
                        (dataPtr + y * widthstep + x * nChannel)[2] = red;
                    }
                }
            }
        }

        public static void Rotation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, double angle)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrCopy = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChannel = mUndo.nChannels;
                int padding = mUndo.widthStep - mUndo.nChannels * mUndo.width;
                int widthstep = mUndo.widthStep;

                byte red, green, blue;
                int x_o, y_o;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x_o = (int)Math.Round((x - width / 2.0) * Math.Cos(angle) - (height / 2.0 - y) * Math.Sin(angle) + width / 2.0);
                        y_o = (int)Math.Round(height / 2.0 - (x - width / 2.0) * Math.Sin(angle) - (height / 2.0 - y) * Math.Cos(angle));

                        if (x_o < width && x_o >= 0 && y_o < height && y_o >= 0)
                        {
                            blue = (dataPtrCopy + y_o * widthstep + x_o * nChannel)[0];
                            green = (dataPtrCopy + y_o * widthstep + x_o * nChannel)[1];
                            red = (dataPtrCopy + y_o * widthstep + x_o * nChannel)[2];
                        }
                        else
                        {
                            red = green = blue = 0;
                        }

                        (dataPtr + y * widthstep + x * nChannel)[0] = blue;
                        (dataPtr + y * widthstep + x * nChannel)[1] = green;
                        (dataPtr + y * widthstep + x * nChannel)[2] = red;
                    }
                }
            }
        }

        public static void Scale(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrCopy = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChannel = mUndo.nChannels;
                int widthstep = mUndo.widthStep;

                byte red, green, blue;
                int x_o, y_o;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x_o = (int)(x / scaleFactor);
                        y_o = (int)(y / scaleFactor);

                        if (x_o < mUndo.width && y_o < mUndo.height && x_o >= 0 && y_o >= 0)
                        {
                            blue = dataPtrCopy[y_o * widthstep + x_o * nChannel];
                            green = dataPtrCopy[y_o * widthstep + x_o * nChannel + 1];
                            red = dataPtrCopy[y_o * widthstep + x_o * nChannel + 2];
                        }
                        else
                        {
                            red = green = blue = 0;
                        }

                        dataPtr[y * widthstep + x * nChannel] = blue;
                        dataPtr[y * widthstep + x * nChannel + 1] = green;
                        dataPtr[y * widthstep + x * nChannel + 2] = red;
                    }
                }
            }
        }

        public static void Scale_point_xy(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor, int centerX, int centerY)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrCopy = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChannel = mUndo.nChannels;
                int widthstep = mUndo.widthStep;
                int padding = m.widthStep - m.nChannels * m.width;

                int x0, y0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x0 = (int)Math.Round((x / scaleFactor) + (centerX - (width / 2) / scaleFactor));
                        y0 = (int)Math.Round((y / scaleFactor) + (centerY - (height / 2) / scaleFactor));

                        if (x0 < mUndo.width && y0 < mUndo.height && x0 >= 0 && y0 >= 0)
                        {
                            dataPtr[0] = (byte)(dataPtrCopy + y0 * m.widthStep + x0 * nChannel)[0];
                            dataPtr[1] = (byte)(dataPtrCopy + y0 * m.widthStep + x0 * nChannel)[1];
                            dataPtr[2] = (byte)(dataPtrCopy + y0 * m.widthStep + x0 * nChannel)[2];
                        }
                        else
                        {
                            dataPtr[0] = (byte)0;
                            dataPtr[1] = (byte)0;
                            dataPtr[2] = (byte)0;
                        }


                        dataPtr += nChannel;
                    }
                    dataPtr += padding;
                }
            }
        }

        public static void Mean(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtringCopy = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrImg = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChannel = mUndo.nChannels;
                int widthstep = mUndo.widthStep;
                int padding = mUndo.widthStep - mUndo.nChannels * mUndo.width;

                int last_h = height - 1;
                int last_w = width - 1;

                dataPtrImg[1] = (byte)Math.Round((4 * dataPtringCopy[1] + 2 * (dataPtringCopy + nChannel)[1] + 2 * (dataPtringCopy + m.widthStep)[1] + (dataPtringCopy + nChannel + m.widthStep)[1]) / 9.0);
                dataPtrImg[2] = (byte)Math.Round((4 * dataPtringCopy[2] + 2 * (dataPtringCopy + nChannel)[2] + 2 * (dataPtringCopy + m.widthStep)[2] + (dataPtringCopy + nChannel + m.widthStep)[2]) / 9.0);
                dataPtrImg[0] = (byte)Math.Round((4 * dataPtringCopy[0] + 2 * (dataPtringCopy + nChannel)[0] + 2 * (dataPtringCopy + m.widthStep)[0] + (dataPtringCopy + nChannel + m.widthStep)[0]) / 9.0);

                dataPtrImg += nChannel;
                dataPtringCopy += nChannel;

                for (int x = 1; x < last_w; x++)
                {
                    dataPtrImg[0] = (byte)Math.Round((2 * (dataPtringCopy - nChannel)[0] + 2 * dataPtringCopy[0] + 2 * (dataPtringCopy + nChannel)[0] + (dataPtringCopy - nChannel + m.widthStep)[0] + (dataPtringCopy + m.widthStep)[0] + (dataPtringCopy + nChannel + m.widthStep)[0]) / 9.0);
                    dataPtrImg[1] = (byte)Math.Round((2 * (dataPtringCopy - nChannel)[1] + 2 * dataPtringCopy[1] + 2 * (dataPtringCopy + nChannel)[1] + (dataPtringCopy - nChannel + m.widthStep)[1] + (dataPtringCopy + m.widthStep)[1] + (dataPtringCopy + nChannel + m.widthStep)[1]) / 9.0);
                    dataPtrImg[2] = (byte)Math.Round((2 * (dataPtringCopy - nChannel)[2] + 2 * dataPtringCopy[2] + 2 * (dataPtringCopy + nChannel)[2] + (dataPtringCopy - nChannel + m.widthStep)[2] + (dataPtringCopy + m.widthStep)[2] + (dataPtringCopy + nChannel + m.widthStep)[2]) / 9.0);

                    dataPtrImg += nChannel;
                    dataPtringCopy += nChannel;
                }

                dataPtrImg[0] = (byte)Math.Round((4 * dataPtringCopy[0] + 2 * (dataPtringCopy - nChannel)[0] + 2 * (dataPtringCopy + m.widthStep)[0] + (dataPtringCopy - nChannel + m.widthStep)[0]) / 9.0);
                dataPtrImg[1] = (byte)Math.Round((4 * dataPtringCopy[1] + 2 * (dataPtringCopy - nChannel)[1] + 2 * (dataPtringCopy + m.widthStep)[1] + (dataPtringCopy - nChannel + m.widthStep)[1]) / 9.0);
                dataPtrImg[2] = (byte)Math.Round((4 * dataPtringCopy[2] + 2 * (dataPtringCopy - nChannel)[2] + 2 * (dataPtringCopy + m.widthStep)[2] + (dataPtringCopy - nChannel + m.widthStep)[2]) / 9.0);

                dataPtrImg += nChannel;
                dataPtringCopy += nChannel;
                dataPtrImg += padding;
                dataPtringCopy += padding;

                for (int y = 1; y < last_h; ++y)
                {
                    dataPtrImg[0] = (byte)Math.Round((2 * (dataPtringCopy - m.widthStep)[0] + (dataPtringCopy + nChannel - m.widthStep)[0] + 2 * (dataPtringCopy)[0] + (dataPtringCopy + nChannel)[0] + 2 * (dataPtringCopy + m.widthStep)[0] + (dataPtringCopy + nChannel + m.widthStep)[0]) / 9.0);
                    dataPtrImg[1] = (byte)Math.Round((2 * (dataPtringCopy - m.widthStep)[1] + (dataPtringCopy + nChannel - m.widthStep)[1] + 2 * (dataPtringCopy)[1] + (dataPtringCopy + nChannel)[1] + 2 * (dataPtringCopy + m.widthStep)[1] + (dataPtringCopy + nChannel + m.widthStep)[1]) / 9.0);
                    dataPtrImg[2] = (byte)Math.Round((2 * (dataPtringCopy - m.widthStep)[2] + (dataPtringCopy + nChannel - m.widthStep)[2] + 2 * (dataPtringCopy)[2] + (dataPtringCopy + nChannel)[2] + 2 * (dataPtringCopy + m.widthStep)[2] + (dataPtringCopy + nChannel + m.widthStep)[2]) / 9.0);

                    dataPtrImg += nChannel;
                    dataPtringCopy += nChannel;

                    for (int x = 1; x < last_w; x++)
                    {

                        dataPtrImg[0] = (byte)Math.Round(((dataPtringCopy - nChannel - m.widthStep)[0] + (dataPtringCopy - m.widthStep)[0] + (dataPtringCopy + nChannel - m.widthStep)[0] + (dataPtringCopy - nChannel)[0] + dataPtringCopy[0] + (dataPtringCopy + nChannel)[0] + (dataPtringCopy - nChannel + m.widthStep)[0] + (dataPtringCopy + m.widthStep)[0] + (dataPtringCopy + nChannel + m.widthStep)[0]) / 9.0);
                        dataPtrImg[1] = (byte)Math.Round(((dataPtringCopy - nChannel - m.widthStep)[1] + (dataPtringCopy - m.widthStep)[1] + (dataPtringCopy + nChannel - m.widthStep)[1] + (dataPtringCopy - nChannel)[1] + dataPtringCopy[1] + (dataPtringCopy + nChannel)[1] + (dataPtringCopy - nChannel + m.widthStep)[1] + (dataPtringCopy + m.widthStep)[1] + (dataPtringCopy + nChannel + m.widthStep)[1]) / 9.0);
                        dataPtrImg[2] = (byte)Math.Round(((dataPtringCopy - nChannel - m.widthStep)[2] + (dataPtringCopy - m.widthStep)[2] + (dataPtringCopy + nChannel - m.widthStep)[2] + (dataPtringCopy - nChannel)[2] + dataPtringCopy[2] + (dataPtringCopy + nChannel)[2] + (dataPtringCopy - nChannel + m.widthStep)[2] + (dataPtringCopy + m.widthStep)[2] + (dataPtringCopy + nChannel + m.widthStep)[2]) / 9.0);

                        dataPtrImg += nChannel;
                        dataPtringCopy += nChannel;
                    }

                    dataPtrImg[0] = (byte)Math.Round(((dataPtringCopy - nChannel - m.widthStep)[0] + 2 * (dataPtringCopy - m.widthStep)[0] + 2 * (dataPtringCopy)[0] + (dataPtringCopy - nChannel)[0] + (dataPtringCopy - nChannel + m.widthStep)[0] + 2 * (dataPtringCopy + m.widthStep)[0]) / 9.0);
                    dataPtrImg[1] = (byte)Math.Round(((dataPtringCopy - nChannel - m.widthStep)[1] + 2 * (dataPtringCopy - m.widthStep)[1] + 2 * (dataPtringCopy)[1] + (dataPtringCopy - nChannel)[1] + (dataPtringCopy - nChannel + m.widthStep)[1] + 2 * (dataPtringCopy + m.widthStep)[1]) / 9.0);
                    dataPtrImg[2] = (byte)Math.Round(((dataPtringCopy - nChannel - m.widthStep)[2] + 2 * (dataPtringCopy - m.widthStep)[2] + 2 * (dataPtringCopy)[2] + (dataPtringCopy - nChannel)[2] + (dataPtringCopy - nChannel + m.widthStep)[2] + 2 * (dataPtringCopy + m.widthStep)[2]) / 9.0);

                    dataPtrImg += nChannel;
                    dataPtringCopy += nChannel;
                    dataPtrImg += padding;
                    dataPtringCopy += padding;

                }

                dataPtrImg[1] = (byte)Math.Round((4 * dataPtringCopy[1] + 2 * (dataPtringCopy + nChannel)[1] + 2 * (dataPtringCopy - m.widthStep)[1] + (dataPtringCopy + nChannel - m.widthStep)[1]) / 9.0);
                dataPtrImg[2] = (byte)Math.Round((4 * dataPtringCopy[2] + 2 * (dataPtringCopy + nChannel)[2] + 2 * (dataPtringCopy - m.widthStep)[2] + (dataPtringCopy + nChannel - m.widthStep)[2]) / 9.0);
                dataPtrImg[0] = (byte)Math.Round((4 * dataPtringCopy[0] + 2 * (dataPtringCopy + nChannel)[0] + 2 * (dataPtringCopy - m.widthStep)[0] + (dataPtringCopy + nChannel - m.widthStep)[0]) / 9.0);

                dataPtrImg += nChannel;
                dataPtringCopy += nChannel;

                for (int x = 1; x < last_w; x++)
                {
                    dataPtrImg[0] = (byte)Math.Round((2 * (dataPtringCopy - nChannel)[0] + 2 * dataPtringCopy[0] + 2 * (dataPtringCopy + nChannel)[0] + (dataPtringCopy - nChannel - m.widthStep)[0] + (dataPtringCopy - m.widthStep)[0] + (dataPtringCopy + nChannel - m.widthStep)[0]) / 9.0);
                    dataPtrImg[1] = (byte)Math.Round((2 * (dataPtringCopy - nChannel)[1] + 2 * dataPtringCopy[1] + 2 * (dataPtringCopy + nChannel)[1] + (dataPtringCopy - nChannel - m.widthStep)[1] + (dataPtringCopy - m.widthStep)[1] + (dataPtringCopy + nChannel - m.widthStep)[1]) / 9.0);
                    dataPtrImg[2] = (byte)Math.Round((2 * (dataPtringCopy - nChannel)[2] + 2 * dataPtringCopy[2] + 2 * (dataPtringCopy + nChannel)[2] + (dataPtringCopy - nChannel - m.widthStep)[2] + (dataPtringCopy - m.widthStep)[2] + (dataPtringCopy + nChannel - m.widthStep)[2]) / 9.0);

                    dataPtrImg += nChannel;
                    dataPtringCopy += nChannel;
                }

                dataPtrImg[0] = (byte)Math.Round((4 * dataPtringCopy[0] + 2 * (dataPtringCopy - nChannel)[0] + 2 * (dataPtringCopy - m.widthStep)[0] + (dataPtringCopy - nChannel - m.widthStep)[0]) / 9.0);
                dataPtrImg[1] = (byte)Math.Round((4 * dataPtringCopy[1] + 2 * (dataPtringCopy - nChannel)[1] + 2 * (dataPtringCopy - m.widthStep)[1] + (dataPtringCopy - nChannel - m.widthStep)[1]) / 9.0);
                dataPtrImg[2] = (byte)Math.Round((4 * dataPtringCopy[2] + 2 * (dataPtringCopy - nChannel)[2] + 2 * (dataPtringCopy - m.widthStep)[2] + (dataPtringCopy - nChannel - m.widthStep)[2]) / 9.0);

            }
        }

        public static void NonUniform(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float[,] matrix, float matrixWeight)
        {
            unsafe
            {
                MIplImage m = imgCopy.MIplImage;
                MIplImage mUndo = img.MIplImage;

                byte* dataPtrImg = (byte*)m.imageData.ToPointer();
                byte* dataPtringCopy = (byte*)mUndo.imageData.ToPointer();

                int widht = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int widthstep = m.widthStep;
                int padding = mUndo.widthStep - mUndo.nChannels * mUndo.width;

                int blue, red, green;

                dataPtrImg += widthstep + nChannel;
                dataPtringCopy += widthstep + nChannel;

                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < widht - 1; x++)
                    {
                        blue = (int)Math.Round(((dataPtrImg - widthstep - nChannel)[0] * matrix[0, 0] + (dataPtrImg - widthstep)[0] * matrix[1, 0] + (dataPtrImg - widthstep + nChannel)[0] * matrix[2, 0] + (dataPtrImg - nChannel)[0] * matrix[0, 1] + dataPtrImg[0] * matrix[1, 1] + (dataPtrImg + nChannel)[0] * matrix[2, 1] + (dataPtrImg + widthstep - nChannel)[0] * matrix[0, 2] + (dataPtrImg + widthstep)[0] * matrix[1, 2] + (dataPtrImg + widthstep + nChannel)[0] * matrix[2, 2]) / matrixWeight);
                        green = (int)Math.Round(((dataPtrImg - widthstep - nChannel)[1] * matrix[0, 0] + (dataPtrImg - widthstep)[1] * matrix[1, 0] + (dataPtrImg - widthstep + nChannel)[1] * matrix[2, 0] + (dataPtrImg - nChannel)[1] * matrix[0, 1] + dataPtrImg[1] * matrix[1, 1] + (dataPtrImg + nChannel)[1] * matrix[2, 1] + (dataPtrImg + widthstep - nChannel)[1] * matrix[0, 2] + (dataPtrImg + widthstep)[1] * matrix[1, 2] + (dataPtrImg + widthstep + nChannel)[1] * matrix[2, 2]) / matrixWeight);
                        red = (int)Math.Round(((dataPtrImg - widthstep - nChannel)[2] * matrix[0, 0] + (dataPtrImg - widthstep)[2] * matrix[1, 0] + (dataPtrImg - widthstep + nChannel)[2] * matrix[2, 0] + (dataPtrImg - nChannel)[2] * matrix[0, 1] + dataPtrImg[2] * matrix[1, 1] + (dataPtrImg + nChannel)[2] * matrix[2, 1] + (dataPtrImg + widthstep - nChannel)[2] * matrix[0, 2] + (dataPtrImg + widthstep)[2] * matrix[1, 2] + (dataPtrImg + widthstep + nChannel)[2] * matrix[2, 2]) / matrixWeight);

                        if (blue > 255) blue = 255;
                        if (blue < 0) blue = 0;

                        if (green > 255) green = 255;
                        if (green < 0) green = 0;

                        if (red > 255) red = 255;
                        if (red < 0) red = 0;

                        dataPtringCopy[0] = (byte)blue;
                        dataPtringCopy[1] = (byte)green;
                        dataPtringCopy[2] = (byte)red;

                        dataPtringCopy += nChannel;
                        dataPtrImg += nChannel;
                    }
                    dataPtringCopy += padding + 2 * nChannel;
                    dataPtrImg += padding + 2 * nChannel;
                }

                dataPtrImg = (byte*)m.imageData.ToPointer();
                dataPtringCopy = (byte*)mUndo.imageData.ToPointer();

                blue = (int)Math.Round(((dataPtrImg)[0] * matrix[0, 0] + (dataPtrImg)[0] * matrix[1, 0] + (dataPtrImg + nChannel)[0] * matrix[2, 0] + (dataPtrImg)[0] * matrix[0, 1] + dataPtrImg[0] * matrix[1, 1] + (dataPtrImg + nChannel)[0] * matrix[2, 1] + (dataPtrImg + widthstep)[0] * matrix[0, 2] + (dataPtrImg + widthstep)[0] * matrix[1, 2] + (dataPtrImg + widthstep + nChannel)[0] * matrix[2, 2]) / matrixWeight);
                green = (int)Math.Round(((dataPtrImg)[1] * matrix[0, 0] + (dataPtrImg)[1] * matrix[1, 0] + (dataPtrImg + nChannel)[1] * matrix[2, 0] + (dataPtrImg)[1] * matrix[0, 1] + dataPtrImg[1] * matrix[1, 1] + (dataPtrImg + nChannel)[1] * matrix[2, 1] + (dataPtrImg + widthstep)[1] * matrix[0, 2] + (dataPtrImg + widthstep)[1] * matrix[1, 2] + (dataPtrImg + widthstep + nChannel)[1] * matrix[2, 2]) / matrixWeight);
                red = (int)Math.Round(((dataPtrImg)[2] * matrix[0, 0] + (dataPtrImg)[2] * matrix[1, 0] + (dataPtrImg + nChannel)[2] * matrix[2, 0] + (dataPtrImg)[2] * matrix[0, 1] + dataPtrImg[2] * matrix[1, 1] + (dataPtrImg + nChannel)[2] * matrix[2, 1] + (dataPtrImg + widthstep)[2] * matrix[0, 2] + (dataPtrImg + widthstep)[2] * matrix[1, 2] + (dataPtrImg + widthstep + nChannel)[2] * matrix[2, 2]) / matrixWeight);

                if (blue > 255) blue = 255;
                if (blue < 0) blue = 0;

                if (green > 255) green = 255;
                if (green < 0) green = 0;

                if (red > 255) red = 255;
                if (red < 0) red = 0;

                dataPtringCopy[0] = (byte)blue;
                dataPtringCopy[1] = (byte)green;
                dataPtringCopy[2] = (byte)red;

                dataPtringCopy += nChannel;
                dataPtrImg += nChannel;

                for (int x_ms = 1; x_ms < widht - 1; x_ms++)
                {
                    blue = (int)Math.Round(((dataPtrImg - nChannel)[0] * matrix[0, 0] + (dataPtrImg)[0] * matrix[1, 0] + (dataPtrImg + nChannel)[0] * matrix[2, 0] + (dataPtrImg - nChannel)[0] * matrix[0, 1] + dataPtrImg[0] * matrix[1, 1] + (dataPtrImg + nChannel)[0] * matrix[2, 1] + (dataPtrImg + widthstep - nChannel)[0] * matrix[0, 2] + (dataPtrImg + widthstep)[0] * matrix[1, 2] + (dataPtrImg + widthstep + nChannel)[0] * matrix[2, 2]) / matrixWeight);
                    green = (int)Math.Round(((dataPtrImg - nChannel)[1] * matrix[0, 0] + (dataPtrImg)[1] * matrix[1, 0] + (dataPtrImg + nChannel)[1] * matrix[2, 0] + (dataPtrImg - nChannel)[1] * matrix[0, 1] + dataPtrImg[1] * matrix[1, 1] + (dataPtrImg + nChannel)[1] * matrix[2, 1] + (dataPtrImg + widthstep - nChannel)[1] * matrix[0, 2] + (dataPtrImg + widthstep)[1] * matrix[1, 2] + (dataPtrImg + widthstep + nChannel)[1] * matrix[2, 2]) / matrixWeight);
                    red = (int)Math.Round(((dataPtrImg - nChannel)[2] * matrix[0, 0] + (dataPtrImg)[2] * matrix[1, 0] + (dataPtrImg + nChannel)[2] * matrix[2, 0] + (dataPtrImg - nChannel)[2] * matrix[0, 1] + dataPtrImg[2] * matrix[1, 1] + (dataPtrImg + nChannel)[2] * matrix[2, 1] + (dataPtrImg + widthstep - nChannel)[2] * matrix[0, 2] + (dataPtrImg + widthstep)[2] * matrix[1, 2] + (dataPtrImg + widthstep + nChannel)[2] * matrix[2, 2]) / matrixWeight);

                    if (blue > 255) blue = 255;
                    if (blue < 0) blue = 0;

                    if (green > 255) green = 255;
                    if (green < 0) green = 0;

                    if (red > 255) red = 255;
                    if (red < 0) red = 0;

                    dataPtringCopy[0] = (byte)blue;
                    dataPtringCopy[1] = (byte)green;
                    dataPtringCopy[2] = (byte)red;

                    dataPtringCopy += nChannel;
                    dataPtrImg += nChannel;
                }

                blue = (int)Math.Round(((dataPtrImg - nChannel)[0] * matrix[0, 0] + (dataPtrImg)[0] * matrix[1, 0] + (dataPtrImg)[0] * matrix[2, 0] + (dataPtrImg - nChannel)[0] * matrix[0, 1] + dataPtrImg[0] * matrix[1, 1] + (dataPtrImg)[0] * matrix[2, 1] + (dataPtrImg + widthstep - nChannel)[0] * matrix[0, 2] + (dataPtrImg + widthstep)[0] * matrix[1, 2] + (dataPtrImg + widthstep)[0] * matrix[2, 2]) / matrixWeight);
                green = (int)Math.Round(((dataPtrImg - nChannel)[1] * matrix[0, 0] + (dataPtrImg)[1] * matrix[1, 0] + (dataPtrImg)[1] * matrix[2, 0] + (dataPtrImg - nChannel)[1] * matrix[0, 1] + dataPtrImg[1] * matrix[1, 1] + (dataPtrImg)[1] * matrix[2, 1] + (dataPtrImg + widthstep - nChannel)[1] * matrix[0, 2] + (dataPtrImg + widthstep)[1] * matrix[1, 2] + (dataPtrImg + widthstep)[1] * matrix[2, 2]) / matrixWeight);
                red = (int)Math.Round(((dataPtrImg - nChannel)[2] * matrix[0, 0] + (dataPtrImg)[2] * matrix[1, 0] + (dataPtrImg)[2] * matrix[2, 0] + (dataPtrImg - nChannel)[2] * matrix[0, 1] + dataPtrImg[2] * matrix[1, 1] + (dataPtrImg)[2] * matrix[2, 1] + (dataPtrImg + widthstep - nChannel)[2] * matrix[0, 2] + (dataPtrImg + widthstep)[2] * matrix[1, 2] + (dataPtrImg + widthstep)[2] * matrix[2, 2]) / matrixWeight);

                if (blue > 255) blue = 255;
                if (blue < 0) blue = 0;

                if (green > 255) green = 255;
                if (green < 0) green = 0;

                if (red > 255) red = 255;
                if (red < 0) red = 0;

                dataPtringCopy[0] = (byte)blue;
                dataPtringCopy[1] = (byte)green;
                dataPtringCopy[2] = (byte)red;

                dataPtringCopy += widthstep;
                dataPtrImg += widthstep;

                for (int y_md = 1; y_md < height - 1; y_md++)
                {
                    blue = (int)Math.Round(((dataPtrImg - nChannel - widthstep)[0] * matrix[0, 0] + (dataPtrImg - widthstep)[0] * matrix[1, 0] + (dataPtrImg - widthstep)[0] * matrix[2, 0] + (dataPtrImg - nChannel)[0] * matrix[0, 1] + dataPtrImg[0] * matrix[1, 1] + (dataPtrImg)[0] * matrix[2, 1] + (dataPtrImg + widthstep - nChannel)[0] * matrix[0, 2] + (dataPtrImg + widthstep)[0] * matrix[1, 2] + (dataPtrImg + widthstep)[0] * matrix[2, 2]) / matrixWeight);
                    green = (int)Math.Round(((dataPtrImg - nChannel - widthstep)[1] * matrix[0, 0] + (dataPtrImg - widthstep)[1] * matrix[1, 0] + (dataPtrImg - widthstep)[1] * matrix[2, 0] + (dataPtrImg - nChannel)[1] * matrix[0, 1] + dataPtrImg[1] * matrix[1, 1] + (dataPtrImg)[1] * matrix[2, 1] + (dataPtrImg + widthstep - nChannel)[1] * matrix[0, 2] + (dataPtrImg + widthstep)[1] * matrix[1, 2] + (dataPtrImg + widthstep)[1] * matrix[2, 2]) / matrixWeight);
                    red = (int)Math.Round(((dataPtrImg - nChannel - widthstep)[2] * matrix[0, 0] + (dataPtrImg - widthstep)[2] * matrix[1, 0] + (dataPtrImg - widthstep)[2] * matrix[2, 0] + (dataPtrImg - nChannel)[2] * matrix[0, 1] + dataPtrImg[2] * matrix[1, 1] + (dataPtrImg)[2] * matrix[2, 1] + (dataPtrImg + widthstep - nChannel)[2] * matrix[0, 2] + (dataPtrImg + widthstep)[2] * matrix[1, 2] + (dataPtrImg + widthstep)[2] * matrix[2, 2]) / matrixWeight);

                    if (blue > 255) blue = 255;
                    if (blue < 0) blue = 0;

                    if (green > 255) green = 255;
                    if (green < 0) green = 0;

                    if (red > 255) red = 255;
                    if (red < 0) red = 0;

                    dataPtringCopy[0] = (byte)blue;
                    dataPtringCopy[1] = (byte)green;
                    dataPtringCopy[2] = (byte)red;

                    dataPtringCopy += widthstep;
                    dataPtrImg += widthstep;
                }

                blue = (int)Math.Round(((dataPtrImg - nChannel - widthstep)[0] * matrix[0, 0] + (dataPtrImg - widthstep)[0] * matrix[1, 0] + (dataPtrImg - widthstep)[0] * matrix[2, 0] + (dataPtrImg - nChannel)[0] * matrix[0, 1] + dataPtrImg[0] * matrix[1, 1] + (dataPtrImg)[0] * matrix[2, 1] + (dataPtrImg - nChannel)[0] * matrix[0, 2] + (dataPtrImg)[0] * matrix[1, 2] + (dataPtrImg)[0] * matrix[2, 2]) / matrixWeight);
                green = (int)Math.Round(((dataPtrImg - nChannel - widthstep)[1] * matrix[0, 0] + (dataPtrImg - widthstep)[1] * matrix[1, 0] + (dataPtrImg - widthstep)[1] * matrix[2, 0] + (dataPtrImg - nChannel)[1] * matrix[0, 1] + dataPtrImg[1] * matrix[1, 1] + (dataPtrImg)[1] * matrix[2, 1] + (dataPtrImg - nChannel)[1] * matrix[0, 2] + (dataPtrImg)[1] * matrix[1, 2] + (dataPtrImg)[1] * matrix[2, 2]) / matrixWeight);
                red = (int)Math.Round(((dataPtrImg - nChannel - widthstep)[2] * matrix[0, 0] + (dataPtrImg - widthstep)[2] * matrix[1, 0] + (dataPtrImg - widthstep)[2] * matrix[2, 0] + (dataPtrImg - nChannel)[2] * matrix[0, 1] + dataPtrImg[2] * matrix[1, 1] + (dataPtrImg)[2] * matrix[2, 1] + (dataPtrImg - nChannel)[2] * matrix[0, 2] + (dataPtrImg)[2] * matrix[1, 2] + (dataPtrImg)[2] * matrix[2, 2]) / matrixWeight);

                if (blue > 255) blue = 255;
                if (blue < 0) blue = 0;

                if (green > 255) green = 255;
                if (green < 0) green = 0;

                if (red > 255) red = 255;
                if (red < 0) red = 0;

                dataPtringCopy[0] = (byte)blue;
                dataPtringCopy[1] = (byte)green;
                dataPtringCopy[2] = (byte)red;

                dataPtringCopy -= nChannel;
                dataPtrImg -= nChannel;

                for (int x_mi = 1; x_mi < widht - 1; x_mi++)
                {
                    blue = (int)Math.Round(((dataPtrImg - nChannel - widthstep)[0] * matrix[0, 0] + (dataPtrImg - widthstep)[0] * matrix[1, 0] + (dataPtrImg - widthstep + nChannel)[0] * matrix[2, 0] + (dataPtrImg - nChannel)[0] * matrix[0, 1] + dataPtrImg[0] * matrix[1, 1] + (dataPtrImg + nChannel)[0] * matrix[2, 1] + (dataPtrImg - nChannel)[0] * matrix[0, 2] + (dataPtrImg)[0] * matrix[1, 2] + (dataPtrImg + nChannel)[0] * matrix[2, 2]) / matrixWeight);
                    green = (int)Math.Round(((dataPtrImg - nChannel - widthstep)[1] * matrix[0, 0] + (dataPtrImg - widthstep)[1] * matrix[1, 0] + (dataPtrImg - widthstep + nChannel)[1] * matrix[2, 0] + (dataPtrImg - nChannel)[1] * matrix[0, 1] + dataPtrImg[1] * matrix[1, 1] + (dataPtrImg + nChannel)[1] * matrix[2, 1] + (dataPtrImg - nChannel)[1] * matrix[0, 2] + (dataPtrImg)[1] * matrix[1, 2] + (dataPtrImg + nChannel)[1] * matrix[2, 2]) / matrixWeight);
                    red = (int)Math.Round(((dataPtrImg - nChannel - widthstep)[2] * matrix[0, 0] + (dataPtrImg - widthstep)[2] * matrix[1, 0] + (dataPtrImg - widthstep + nChannel)[2] * matrix[2, 0] + (dataPtrImg - nChannel)[2] * matrix[0, 1] + dataPtrImg[2] * matrix[1, 1] + (dataPtrImg + nChannel)[2] * matrix[2, 1] + (dataPtrImg - nChannel)[2] * matrix[0, 2] + (dataPtrImg)[2] * matrix[1, 2] + (dataPtrImg + nChannel)[2] * matrix[2, 2]) / matrixWeight);

                    if (blue > 255) blue = 255;
                    if (blue < 0) blue = 0;

                    if (green > 255) green = 255;
                    if (green < 0) green = 0;

                    if (red > 255) red = 255;
                    if (red < 0) red = 0;

                    dataPtringCopy[0] = (byte)blue;
                    dataPtringCopy[1] = (byte)green;
                    dataPtringCopy[2] = (byte)red;

                    dataPtringCopy -= nChannel;
                    dataPtrImg -= nChannel;
                }

                blue = (int)Math.Round(((dataPtrImg - widthstep)[0] * matrix[0, 0] + (dataPtrImg - widthstep)[0] * matrix[1, 0] + (dataPtrImg - widthstep + nChannel)[0] * matrix[2, 0] + (dataPtrImg)[0] * matrix[0, 1] + dataPtrImg[0] * matrix[1, 1] + (dataPtrImg + nChannel)[0] * matrix[2, 1] + (dataPtrImg)[0] * matrix[0, 2] + (dataPtrImg)[0] * matrix[1, 2] + (dataPtrImg + nChannel)[0] * matrix[2, 2]) / matrixWeight);
                green = (int)Math.Round(((dataPtrImg - widthstep)[1] * matrix[0, 0] + (dataPtrImg - widthstep)[1] * matrix[1, 0] + (dataPtrImg - widthstep + nChannel)[1] * matrix[2, 0] + (dataPtrImg)[1] * matrix[0, 1] + dataPtrImg[1] * matrix[1, 1] + (dataPtrImg + nChannel)[1] * matrix[2, 1] + (dataPtrImg)[1] * matrix[0, 2] + (dataPtrImg)[1] * matrix[1, 2] + (dataPtrImg + nChannel)[1] * matrix[2, 2]) / matrixWeight);
                red = (int)Math.Round(((dataPtrImg - widthstep)[2] * matrix[0, 0] + (dataPtrImg - widthstep)[2] * matrix[1, 0] + (dataPtrImg - widthstep + nChannel)[2] * matrix[2, 0] + (dataPtrImg)[2] * matrix[0, 1] + dataPtrImg[2] * matrix[1, 1] + (dataPtrImg + nChannel)[2] * matrix[2, 1] + (dataPtrImg)[2] * matrix[0, 2] + (dataPtrImg)[2] * matrix[1, 2] + (dataPtrImg + nChannel)[2] * matrix[2, 2]) / matrixWeight);

                if (blue > 255) blue = 255;
                if (blue < 0) blue = 0;

                if (green > 255) green = 255;
                if (green < 0) green = 0;

                if (red > 255) red = 255;
                if (red < 0) red = 0;

                dataPtringCopy[0] = (byte)blue;
                dataPtringCopy[1] = (byte)green;
                dataPtringCopy[2] = (byte)red;

                dataPtringCopy -= widthstep;
                dataPtrImg -= widthstep;

                for (int y_me = 1; y_me < height - 1; y_me++)
                {
                    blue = (int)Math.Round(((dataPtrImg - widthstep)[0] * matrix[0, 0] + (dataPtrImg - widthstep)[0] * matrix[1, 0] + (dataPtrImg - widthstep + nChannel)[0] * matrix[2, 0] + (dataPtrImg)[0] * matrix[0, 1] + dataPtrImg[0] * matrix[1, 1] + (dataPtrImg + nChannel)[0] * matrix[2, 1] + (dataPtrImg + widthstep)[0] * matrix[0, 2] + (dataPtrImg + widthstep)[0] * matrix[1, 2] + (dataPtrImg + nChannel + widthstep)[0] * matrix[2, 2]) / matrixWeight);
                    green = (int)Math.Round(((dataPtrImg - widthstep)[1] * matrix[0, 0] + (dataPtrImg - widthstep)[1] * matrix[1, 0] + (dataPtrImg - widthstep + nChannel)[1] * matrix[2, 0] + (dataPtrImg)[1] * matrix[0, 1] + dataPtrImg[1] * matrix[1, 1] + (dataPtrImg + nChannel)[1] * matrix[2, 1] + (dataPtrImg + widthstep)[1] * matrix[0, 2] + (dataPtrImg + widthstep)[1] * matrix[1, 2] + (dataPtrImg + nChannel + widthstep)[1] * matrix[2, 2]) / matrixWeight);
                    red = (int)Math.Round(((dataPtrImg - widthstep)[2] * matrix[0, 0] + (dataPtrImg - widthstep)[2] * matrix[1, 0] + (dataPtrImg - widthstep + nChannel)[2] * matrix[2, 0] + (dataPtrImg)[2] * matrix[0, 1] + dataPtrImg[2] * matrix[1, 1] + (dataPtrImg + nChannel)[2] * matrix[2, 1] + (dataPtrImg + widthstep)[2] * matrix[0, 2] + (dataPtrImg + widthstep)[2] * matrix[1, 2] + (dataPtrImg + nChannel + widthstep)[2] * matrix[2, 2]) / matrixWeight);

                    if (blue > 255) blue = 255;
                    if (blue < 0) blue = 0;

                    if (green > 255) green = 255;
                    if (green < 0) green = 0;

                    if (red > 255) red = 255;
                    if (red < 0) red = 0;

                    dataPtringCopy[0] = (byte)blue;
                    dataPtringCopy[1] = (byte)green;
                    dataPtringCopy[2] = (byte)red;

                    dataPtringCopy -= widthstep;
                    dataPtrImg -= widthstep;
                }
            }
        }

        public static void Sobel(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = imgCopy.MIplImage;
                MIplImage m_d = img.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer();
                byte* dataPtr_d = (byte*)m_d.imageData.ToPointer();

                int widht = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int widthstep = m.widthStep;
                int padding = m_d.widthStep - m_d.nChannels * m_d.width;

                int SxBlue, SxGreen, SxRed;
                int SyBlue, SyGreen, SyRed;
                int SBlue, SGreen, SRed;
                int y, x;

                dataPtr += widthstep + nChannel;
                dataPtr_d += widthstep + nChannel;

                for (y = 1; y < height - 1; y++)
                {
                    for (x = 1; x < widht - 1; x++)
                    {
                        SxBlue = ((dataPtr - nChannel - widthstep)[0] + (dataPtr - nChannel)[0] * 2 + (dataPtr - nChannel + widthstep)[0]) - ((dataPtr + nChannel - widthstep)[0] + (dataPtr + nChannel)[0] * 2 + (dataPtr + nChannel + widthstep)[0]);
                        SxGreen = ((dataPtr - nChannel - widthstep)[1] + (dataPtr - nChannel)[1] * 2 + (dataPtr - nChannel + widthstep)[1]) - ((dataPtr + nChannel - widthstep)[1] + (dataPtr + nChannel)[1] * 2 + (dataPtr + nChannel + widthstep)[1]);
                        SxRed = ((dataPtr - nChannel - widthstep)[2] + (dataPtr - nChannel)[2] * 2 + (dataPtr - nChannel + widthstep)[2]) - ((dataPtr + nChannel - widthstep)[2] + (dataPtr + nChannel)[2] * 2 + (dataPtr + nChannel + widthstep)[2]);

                        SyBlue = ((dataPtr - nChannel + widthstep)[0] + (dataPtr + widthstep)[0] * 2 + (dataPtr + nChannel + widthstep)[0]) - ((dataPtr - nChannel - widthstep)[0] + (dataPtr - widthstep)[0] * 2 + (dataPtr + nChannel - widthstep)[0]);
                        SyGreen = ((dataPtr - nChannel + widthstep)[1] + (dataPtr + widthstep)[1] * 2 + (dataPtr + nChannel + widthstep)[1]) - ((dataPtr - nChannel - widthstep)[1] + (dataPtr - widthstep)[1] * 2 + (dataPtr + nChannel - widthstep)[1]);
                        SyRed = ((dataPtr - nChannel + widthstep)[2] + (dataPtr + widthstep)[2] * 2 + (dataPtr + nChannel + widthstep)[2]) - ((dataPtr - nChannel - widthstep)[2] + (dataPtr - widthstep)[2] * 2 + (dataPtr + nChannel - widthstep)[2]);

                        SBlue = Math.Abs(SxBlue) + Math.Abs(SyBlue);
                        SGreen = Math.Abs(SxGreen) + Math.Abs(SyGreen);
                        SRed = Math.Abs(SxRed) + Math.Abs(SyRed);

                        if (SBlue > 255) SBlue = 255;
                        if (SGreen > 255) SGreen = 255;
                        if (SRed > 255) SRed = 255;

                        dataPtr_d[0] = (byte)SBlue;
                        dataPtr_d[1] = (byte)SGreen;
                        dataPtr_d[2] = (byte)SRed;

                        dataPtr += nChannel;
                        dataPtr_d += nChannel;
                    }
                    dataPtr_d += padding + 2 * nChannel;
                    dataPtr += padding + 2 * nChannel;
                }

                dataPtr = (byte*)m.imageData.ToPointer();
                dataPtr_d = (byte*)m_d.imageData.ToPointer();

                SxBlue = ((dataPtr)[0] + (dataPtr)[0] * 2 + (dataPtr + widthstep)[0]) - ((dataPtr + nChannel)[0] + (dataPtr + nChannel)[0] * 2 + (dataPtr + nChannel + widthstep)[0]);
                SxGreen = ((dataPtr)[1] + (dataPtr)[1] * 2 + (dataPtr + widthstep)[1]) - ((dataPtr + nChannel)[1] + (dataPtr + nChannel)[1] * 2 + (dataPtr + nChannel + widthstep)[1]);
                SxRed = ((dataPtr)[2] + (dataPtr)[2] * 2 + (dataPtr + widthstep)[2]) - ((dataPtr + nChannel)[2] + (dataPtr + nChannel)[2] * 2 + (dataPtr + nChannel + widthstep)[2]);

                SyBlue = ((dataPtr + widthstep)[0] + (dataPtr + widthstep)[0] * 2 + (dataPtr + nChannel + widthstep)[0]) - ((dataPtr)[0] + (dataPtr)[0] * 2 + (dataPtr + nChannel)[0]);
                SyGreen = ((dataPtr + widthstep)[1] + (dataPtr + widthstep)[1] * 2 + (dataPtr + nChannel + widthstep)[1]) - ((dataPtr)[1] + (dataPtr)[1] * 2 + (dataPtr + nChannel)[1]);
                SyRed = ((dataPtr + widthstep)[2] + (dataPtr + widthstep)[2] * 2 + (dataPtr + nChannel + widthstep)[2]) - ((dataPtr)[2] + (dataPtr)[2] * 2 + (dataPtr + nChannel)[2]);

                SBlue = Math.Abs(SxBlue) + Math.Abs(SyBlue);
                SGreen = Math.Abs(SxGreen) + Math.Abs(SyGreen);
                SRed = Math.Abs(SxRed) + Math.Abs(SyRed);

                if (SBlue > 255) SBlue = 255;
                if (SGreen > 255) SGreen = 255;
                if (SRed > 255) SRed = 255;

                dataPtr_d[0] = (byte)SBlue;
                dataPtr_d[1] = (byte)SGreen;
                dataPtr_d[2] = (byte)SRed;

                dataPtr += nChannel;
                dataPtr_d += nChannel;

                for (int x_sup = 1; x_sup < widht - 1; x_sup++)
                {
                    SxBlue = ((dataPtr - nChannel)[0] + (dataPtr - nChannel)[0] * 2 + (dataPtr + widthstep - nChannel)[0]) - ((dataPtr + nChannel)[0] + (dataPtr + nChannel)[0] * 2 + (dataPtr + nChannel + widthstep)[0]);
                    SxGreen = ((dataPtr - nChannel)[1] + (dataPtr - nChannel)[1] * 2 + (dataPtr + widthstep - nChannel)[1]) - ((dataPtr + nChannel)[1] + (dataPtr + nChannel)[1] * 2 + (dataPtr + nChannel + widthstep)[1]);
                    SxRed = ((dataPtr - nChannel)[2] + (dataPtr - nChannel)[2] * 2 + (dataPtr + widthstep - nChannel)[2]) - ((dataPtr + nChannel)[2] + (dataPtr + nChannel)[2] * 2 + (dataPtr + nChannel + widthstep)[2]);

                    SyBlue = ((dataPtr + widthstep - nChannel)[0] + (dataPtr + widthstep)[0] * 2 + (dataPtr + nChannel + widthstep)[0]) - ((dataPtr - nChannel)[0] + (dataPtr)[0] * 2 + (dataPtr + nChannel)[0]);
                    SyGreen = ((dataPtr + widthstep - nChannel)[1] + (dataPtr + widthstep)[1] * 2 + (dataPtr + nChannel + widthstep)[1]) - ((dataPtr - nChannel)[1] + (dataPtr)[1] * 2 + (dataPtr + nChannel)[1]);
                    SyRed = ((dataPtr + widthstep - nChannel)[2] + (dataPtr + widthstep)[2] * 2 + (dataPtr + nChannel + widthstep)[2]) - ((dataPtr - nChannel)[2] + (dataPtr)[2] * 2 + (dataPtr + nChannel)[2]);

                    SBlue = Math.Abs(SxBlue) + Math.Abs(SyBlue);
                    SGreen = Math.Abs(SxGreen) + Math.Abs(SyGreen);
                    SRed = Math.Abs(SxRed) + Math.Abs(SyRed);

                    if (SBlue > 255) SBlue = 255;
                    if (SGreen > 255) SGreen = 255;
                    if (SRed > 255) SRed = 255;

                    dataPtr_d[0] = (byte)SBlue;
                    dataPtr_d[1] = (byte)SGreen;
                    dataPtr_d[2] = (byte)SRed;

                    dataPtr += nChannel;
                    dataPtr_d += nChannel;
                }

                SxBlue = ((dataPtr - nChannel)[0] + (dataPtr - nChannel)[0] * 2 + (dataPtr + widthstep - nChannel)[0]) - ((dataPtr)[0] + (dataPtr)[0] * 2 + (dataPtr + widthstep)[0]);
                SxGreen = ((dataPtr - nChannel)[1] + (dataPtr - nChannel)[1] * 2 + (dataPtr + widthstep - nChannel)[1]) - ((dataPtr)[1] + (dataPtr)[1] * 2 + (dataPtr + widthstep)[1]);
                SxRed = ((dataPtr - nChannel)[2] + (dataPtr - nChannel)[2] * 2 + (dataPtr + widthstep - nChannel)[2]) - ((dataPtr)[2] + (dataPtr)[2] * 2 + (dataPtr + widthstep)[2]);

                SyBlue = ((dataPtr + widthstep - nChannel)[0] + (dataPtr + widthstep)[0] * 2 + (dataPtr + widthstep)[0]) - ((dataPtr - nChannel)[0] + (dataPtr)[0] * 2 + (dataPtr)[0]);
                SyGreen = ((dataPtr + widthstep - nChannel)[1] + (dataPtr + widthstep)[1] * 2 + (dataPtr + widthstep)[1]) - ((dataPtr - nChannel)[1] + (dataPtr)[1] * 2 + (dataPtr)[1]);
                SyRed = ((dataPtr + widthstep - nChannel)[2] + (dataPtr + widthstep)[2] * 2 + (dataPtr + widthstep)[2]) - ((dataPtr - nChannel)[2] + (dataPtr)[2] * 2 + (dataPtr)[2]);

                SBlue = Math.Abs(SxBlue) + Math.Abs(SyBlue);
                SGreen = Math.Abs(SxGreen) + Math.Abs(SyGreen);
                SRed = Math.Abs(SxRed) + Math.Abs(SyRed);

                if (SBlue > 255) SBlue = 255;
                if (SGreen > 255) SGreen = 255;
                if (SRed > 255) SRed = 255;

                dataPtr_d[0] = (byte)SBlue;
                dataPtr_d[1] = (byte)SGreen;
                dataPtr_d[2] = (byte)SRed;

                dataPtr += widthstep;
                dataPtr_d += widthstep;

                for (int y_dir = 1; y_dir < height - 1; y_dir++)
                {
                    SxBlue = ((dataPtr - nChannel - widthstep)[0] + (dataPtr - nChannel)[0] * 2 + (dataPtr + widthstep - nChannel)[0]) - ((dataPtr - widthstep)[0] + (dataPtr)[0] * 2 + (dataPtr + widthstep)[0]);
                    SxGreen = ((dataPtr - nChannel - widthstep)[1] + (dataPtr - nChannel)[1] * 2 + (dataPtr + widthstep - nChannel)[1]) - ((dataPtr - widthstep)[1] + (dataPtr)[1] * 2 + (dataPtr + widthstep)[1]);
                    SxRed = ((dataPtr - nChannel - widthstep)[2] + (dataPtr - nChannel)[2] * 2 + (dataPtr + widthstep - nChannel)[2]) - ((dataPtr - widthstep)[2] + (dataPtr)[2] * 2 + (dataPtr + widthstep)[2]);

                    SyBlue = ((dataPtr + widthstep - nChannel)[0] + (dataPtr + widthstep)[0] * 2 + (dataPtr + widthstep)[0]) - ((dataPtr - nChannel - widthstep)[0] + (dataPtr - widthstep)[0] * 2 + (dataPtr - widthstep)[0]);
                    SyGreen = ((dataPtr + widthstep - nChannel)[1] + (dataPtr + widthstep)[1] * 2 + (dataPtr + widthstep)[1]) - ((dataPtr - nChannel - widthstep)[1] + (dataPtr - widthstep)[1] * 2 + (dataPtr - widthstep)[1]);
                    SyRed = ((dataPtr + widthstep - nChannel)[2] + (dataPtr + widthstep)[2] * 2 + (dataPtr + widthstep)[2]) - ((dataPtr - nChannel - widthstep)[2] + (dataPtr - widthstep)[2] * 2 + (dataPtr - widthstep)[2]);

                    SBlue = Math.Abs(SxBlue) + Math.Abs(SyBlue);
                    SGreen = Math.Abs(SxGreen) + Math.Abs(SyGreen);
                    SRed = Math.Abs(SxRed) + Math.Abs(SyRed);

                    if (SBlue > 255) SBlue = 255;
                    if (SGreen > 255) SGreen = 255;
                    if (SRed > 255) SRed = 255;

                    dataPtr_d[0] = (byte)SBlue;
                    dataPtr_d[1] = (byte)SGreen;
                    dataPtr_d[2] = (byte)SRed;

                    dataPtr += widthstep;
                    dataPtr_d += widthstep;
                }

                SxBlue = ((dataPtr - nChannel - widthstep)[0] + (dataPtr - nChannel)[0] * 2 + (dataPtr - nChannel)[0]) - ((dataPtr - widthstep)[0] + (dataPtr)[0] * 2 + (dataPtr)[0]);
                SxGreen = ((dataPtr - nChannel - widthstep)[1] + (dataPtr - nChannel)[1] * 2 + (dataPtr - nChannel)[1]) - ((dataPtr - widthstep)[1] + (dataPtr)[1] * 2 + (dataPtr)[1]);
                SxRed = ((dataPtr - nChannel - widthstep)[2] + (dataPtr - nChannel)[2] * 2 + (dataPtr - nChannel)[2]) - ((dataPtr - widthstep)[2] + (dataPtr)[2] * 2 + (dataPtr)[2]);

                SyBlue = ((dataPtr - nChannel)[0] + (dataPtr)[0] * 2 + (dataPtr)[0]) - ((dataPtr - nChannel - widthstep)[0] + (dataPtr - widthstep)[0] * 2 + (dataPtr - widthstep)[0]);
                SyGreen = ((dataPtr - nChannel)[1] + (dataPtr)[1] * 2 + (dataPtr)[1]) - ((dataPtr - nChannel - widthstep)[1] + (dataPtr - widthstep)[1] * 2 + (dataPtr - widthstep)[1]);
                SyRed = ((dataPtr - nChannel)[2] + (dataPtr)[2] * 2 + (dataPtr)[2]) - ((dataPtr - nChannel - widthstep)[2] + (dataPtr - widthstep)[2] * 2 + (dataPtr - widthstep)[2]);

                SBlue = Math.Abs(SxBlue) + Math.Abs(SyBlue);
                SGreen = Math.Abs(SxGreen) + Math.Abs(SyGreen);
                SRed = Math.Abs(SxRed) + Math.Abs(SyRed);

                if (SBlue > 255) SBlue = 255;
                if (SGreen > 255) SGreen = 255;
                if (SRed > 255) SRed = 255;

                dataPtr_d[0] = (byte)SBlue;
                dataPtr_d[1] = (byte)SGreen;
                dataPtr_d[2] = (byte)SRed;

                dataPtr -= nChannel;
                dataPtr_d -= nChannel;

                for (int x_inf = 1; x_inf < widht - 1; x_inf++)
                {
                    SxBlue = ((dataPtr - nChannel - widthstep)[0] + (dataPtr - nChannel)[0] * 2 + (dataPtr - nChannel)[0]) - ((dataPtr - widthstep + nChannel)[0] + (dataPtr + nChannel)[0] * 2 + (dataPtr + nChannel)[0]);
                    SxGreen = ((dataPtr - nChannel - widthstep)[1] + (dataPtr - nChannel)[1] * 2 + (dataPtr - nChannel)[1]) - ((dataPtr - widthstep + nChannel)[1] + (dataPtr + nChannel)[1] * 2 + (dataPtr + nChannel)[1]);
                    SxRed = ((dataPtr - nChannel - widthstep)[2] + (dataPtr - nChannel)[2] * 2 + (dataPtr - nChannel)[2]) - ((dataPtr - widthstep + nChannel)[2] + (dataPtr + nChannel)[2] * 2 + (dataPtr + nChannel)[2]);

                    SyBlue = ((dataPtr - nChannel)[0] + (dataPtr)[0] * 2 + (dataPtr + nChannel)[0]) - ((dataPtr - nChannel - widthstep)[0] + (dataPtr - widthstep)[0] * 2 + (dataPtr - widthstep + nChannel)[0]);
                    SyGreen = ((dataPtr - nChannel)[1] + (dataPtr)[1] * 2 + (dataPtr + nChannel)[1]) - ((dataPtr - nChannel - widthstep)[1] + (dataPtr - widthstep)[1] * 2 + (dataPtr - widthstep + nChannel)[1]);
                    SyRed = ((dataPtr - nChannel)[2] + (dataPtr)[2] * 2 + (dataPtr + nChannel)[2]) - ((dataPtr - nChannel - widthstep)[2] + (dataPtr - widthstep)[2] * 2 + (dataPtr - widthstep + nChannel)[2]);

                    SBlue = Math.Abs(SxBlue) + Math.Abs(SyBlue);
                    SGreen = Math.Abs(SxGreen) + Math.Abs(SyGreen);
                    SRed = Math.Abs(SxRed) + Math.Abs(SyRed);

                    if (SBlue > 255) SBlue = 255;
                    if (SGreen > 255) SGreen = 255;
                    if (SRed > 255) SRed = 255;

                    dataPtr_d[0] = (byte)SBlue;
                    dataPtr_d[1] = (byte)SGreen;
                    dataPtr_d[2] = (byte)SRed;

                    dataPtr -= nChannel;
                    dataPtr_d -= nChannel;
                }

                SxBlue = ((dataPtr - widthstep)[0] + (dataPtr)[0] * 2 + (dataPtr)[0]) - ((dataPtr - widthstep + nChannel)[0] + (dataPtr + nChannel)[0] * 2 + (dataPtr + nChannel)[0]);
                SxGreen = ((dataPtr - widthstep)[1] + (dataPtr)[1] * 2 + (dataPtr)[1]) - ((dataPtr - widthstep + nChannel)[1] + (dataPtr + nChannel)[1] * 2 + (dataPtr + nChannel)[1]);
                SxRed = ((dataPtr - widthstep)[2] + (dataPtr)[2] * 2 + (dataPtr)[2]) - ((dataPtr - widthstep + nChannel)[2] + (dataPtr + nChannel)[2] * 2 + (dataPtr + nChannel)[2]);

                SyBlue = ((dataPtr)[0] + (dataPtr)[0] * 2 + (dataPtr + nChannel)[0]) - ((dataPtr - widthstep)[0] + (dataPtr - widthstep)[0] * 2 + (dataPtr - widthstep + nChannel)[0]);
                SyGreen = ((dataPtr)[1] + (dataPtr)[1] * 2 + (dataPtr + nChannel)[1]) - ((dataPtr - widthstep)[1] + (dataPtr - widthstep)[1] * 2 + (dataPtr - widthstep + nChannel)[1]);
                SyRed = ((dataPtr)[2] + (dataPtr)[2] * 2 + (dataPtr + nChannel)[2]) - ((dataPtr - widthstep)[2] + (dataPtr - widthstep)[2] * 2 + (dataPtr - widthstep + nChannel)[2]);

                SBlue = Math.Abs(SxBlue) + Math.Abs(SyBlue);
                SGreen = Math.Abs(SxGreen) + Math.Abs(SyGreen);
                SRed = Math.Abs(SxRed) + Math.Abs(SyRed);

                if (SBlue > 255) SBlue = 255;
                if (SGreen > 255) SGreen = 255;
                if (SRed > 255) SRed = 255;

                dataPtr_d[0] = (byte)SBlue;
                dataPtr_d[1] = (byte)SGreen;
                dataPtr_d[2] = (byte)SRed;

                dataPtr -= widthstep;
                dataPtr_d -= widthstep;

                for (int y_esq = 1; y_esq < height - 1; y_esq++)
                {
                    SxBlue = ((dataPtr - widthstep)[0] + (dataPtr)[0] * 2 + (dataPtr + widthstep)[0]) - ((dataPtr - widthstep + nChannel)[0] + (dataPtr + nChannel)[0] * 2 + (dataPtr + nChannel + widthstep)[0]);
                    SxGreen = ((dataPtr - widthstep)[1] + (dataPtr)[1] * 2 + (dataPtr + widthstep)[1]) - ((dataPtr - widthstep + nChannel)[1] + (dataPtr + nChannel)[1] * 2 + (dataPtr + nChannel + widthstep)[1]);
                    SxRed = ((dataPtr - widthstep)[2] + (dataPtr)[2] * 2 + (dataPtr + widthstep)[2]) - ((dataPtr - widthstep + nChannel)[2] + (dataPtr + nChannel)[2] * 2 + (dataPtr + nChannel + widthstep)[2]);

                    SyBlue = ((dataPtr + widthstep)[0] + (dataPtr + widthstep)[0] * 2 + (dataPtr + nChannel + widthstep)[0]) - ((dataPtr - widthstep)[0] + (dataPtr - widthstep)[0] * 2 + (dataPtr - widthstep + nChannel)[0]);
                    SyGreen = ((dataPtr + widthstep)[1] + (dataPtr + widthstep)[1] * 2 + (dataPtr + nChannel + widthstep)[1]) - ((dataPtr - widthstep)[1] + (dataPtr - widthstep)[1] * 2 + (dataPtr - widthstep + nChannel)[1]);
                    SyRed = ((dataPtr + widthstep)[2] + (dataPtr + widthstep)[2] * 2 + (dataPtr + nChannel + widthstep)[2]) - ((dataPtr - widthstep)[2] + (dataPtr - widthstep)[2] * 2 + (dataPtr - widthstep + nChannel)[2]);

                    SBlue = Math.Abs(SxBlue) + Math.Abs(SyBlue);
                    SGreen = Math.Abs(SxGreen) + Math.Abs(SyGreen);
                    SRed = Math.Abs(SxRed) + Math.Abs(SyRed);

                    if (SBlue > 255) SBlue = 255;
                    if (SGreen > 255) SGreen = 255;
                    if (SRed > 255) SRed = 255;

                    dataPtr_d[0] = (byte)SBlue;
                    dataPtr_d[1] = (byte)SGreen;
                    dataPtr_d[2] = (byte)SRed;

                    dataPtr -= widthstep;
                    dataPtr_d -= widthstep;
                }
            }
        }

        public static void Diferentiation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = imgCopy.MIplImage;
                MIplImage m_d = img.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer();
                byte* dataPtr_d = (byte*)m_d.imageData.ToPointer();

                int widht = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int widthstep = m.widthStep;
                int padding = m_d.widthStep - m_d.nChannels * m_d.width;

                int y, x;
                int blue, green, red;

                for (y = 0; y < height - 1; y++)
                {
                    for (x = 0; x < widht - 1; x++)
                    {
                        blue = Math.Abs(dataPtr[0] - (dataPtr + nChannel)[0]) + Math.Abs(dataPtr[0] - (dataPtr + widthstep)[0]);
                        green = Math.Abs(dataPtr[1] - (dataPtr + nChannel)[1]) + Math.Abs(dataPtr[1] - (dataPtr + widthstep)[1]);
                        red = Math.Abs(dataPtr[2] - (dataPtr + nChannel)[2]) + Math.Abs(dataPtr[2] - (dataPtr + widthstep)[2]);

                        if (blue > 255) blue = 255;
                        if (green > 255) green = 255;
                        if (red > 255) red = 255;

                        dataPtr_d[0] = (byte)blue;
                        dataPtr_d[1] = (byte)green;
                        dataPtr_d[2] = (byte)red;

                        dataPtr += nChannel;
                        dataPtr_d += nChannel;
                    }
                    dataPtr_d += padding + nChannel;
                    dataPtr += padding + nChannel;
                }

                dataPtr = (byte*)m.imageData.ToPointer();
                dataPtr_d = (byte*)m_d.imageData.ToPointer();

                dataPtr += nChannel * (widht - 1);
                dataPtr_d += nChannel * (widht - 1);

                for (int y_dir = 0; y_dir < height - 1; y_dir++)
                {
                    blue = Math.Abs(dataPtr[0] - (dataPtr)[0]) + Math.Abs(dataPtr[0] - (dataPtr + widthstep)[0]);
                    green = Math.Abs(dataPtr[1] - (dataPtr)[1]) + Math.Abs(dataPtr[1] - (dataPtr + widthstep)[1]);
                    red = Math.Abs(dataPtr[2] - (dataPtr)[2]) + Math.Abs(dataPtr[2] - (dataPtr + widthstep)[2]);

                    if (blue > 255) blue = 255;
                    if (green > 255) green = 255;
                    if (red > 255) red = 255;

                    dataPtr_d[0] = (byte)blue;
                    dataPtr_d[1] = (byte)green;
                    dataPtr_d[2] = (byte)red;

                    dataPtr += widthstep;
                    dataPtr_d += widthstep;
                }

                dataPtr_d[0] = 0;
                dataPtr_d[1] = 0;
                dataPtr_d[2] = 0;

                dataPtr -= nChannel;
                dataPtr_d -= nChannel;

                for (int x_inf = 1; x_inf < widht; x_inf++)
                {
                    blue = Math.Abs(dataPtr[0] - (dataPtr + nChannel)[0]) + Math.Abs(dataPtr[0] - (dataPtr)[0]);
                    green = Math.Abs(dataPtr[1] - (dataPtr + nChannel)[1]) + Math.Abs(dataPtr[1] - (dataPtr)[1]);
                    red = Math.Abs(dataPtr[2] - (dataPtr + nChannel)[2]) + Math.Abs(dataPtr[2] - (dataPtr)[2]);

                    if (blue > 255) blue = 255;
                    if (green > 255) green = 255;
                    if (red > 255) red = 255;

                    dataPtr_d[0] = (byte)blue;
                    dataPtr_d[1] = (byte)green;
                    dataPtr_d[2] = (byte)red;

                    dataPtr -= nChannel;
                    dataPtr_d -= nChannel;
                }
            }
        }

        public static void Roberts(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = imgCopy.MIplImage;
                MIplImage m_d = img.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer();
                byte* dataPtr_d = (byte*)m_d.imageData.ToPointer();

                int widht = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int widthstep = m.widthStep;
                int padding = m_d.widthStep - m_d.nChannels * m_d.width;

                int y, x;
                int GBlue, GGreen, GRed;

                for (y = 0; y < height - 1; y++)
                {
                    for (x = 0; x < widht - 1; x++)
                    {
                        GBlue = Math.Abs(dataPtr[0] - (dataPtr + nChannel + widthstep)[0]) + Math.Abs((dataPtr + nChannel)[0] - (dataPtr + widthstep)[0]);
                        GGreen = Math.Abs(dataPtr[1] - (dataPtr + nChannel + widthstep)[1]) + Math.Abs((dataPtr + nChannel)[1] - (dataPtr + widthstep)[1]);
                        GRed = Math.Abs(dataPtr[2] - (dataPtr + nChannel + widthstep)[2]) + Math.Abs((dataPtr + nChannel)[2] - (dataPtr + widthstep)[2]);

                        if (GBlue > 255) GBlue = 255;
                        if (GGreen > 255) GGreen = 255;
                        if (GRed > 255) GRed = 255;

                        dataPtr_d[0] = (byte)GBlue;
                        dataPtr_d[1] = (byte)GGreen;
                        dataPtr_d[2] = (byte)GRed;

                        dataPtr += nChannel;
                        dataPtr_d += nChannel;
                    }
                    dataPtr_d += padding + nChannel;
                    dataPtr += padding + nChannel;
                }

                dataPtr = (byte*)m.imageData.ToPointer();
                dataPtr_d = (byte*)m_d.imageData.ToPointer();

                dataPtr += nChannel * (widht - 1);
                dataPtr_d += nChannel * (widht - 1);

                for (int y_dir = 0; y_dir < height - 1; y_dir++)
                {
                    GBlue = Math.Abs(dataPtr[0] - (dataPtr + widthstep)[0]) + Math.Abs((dataPtr)[0] - (dataPtr + widthstep)[0]);
                    GGreen = Math.Abs(dataPtr[1] - (dataPtr + widthstep)[1]) + Math.Abs((dataPtr)[1] - (dataPtr + widthstep)[1]);
                    GRed = Math.Abs(dataPtr[2] - (dataPtr + widthstep)[2]) + Math.Abs((dataPtr)[2] - (dataPtr + widthstep)[2]);

                    if (GBlue > 255) GBlue = 255;
                    if (GGreen > 255) GGreen = 255;
                    if (GRed > 255) GRed = 255;

                    dataPtr_d[0] = (byte)GBlue;
                    dataPtr_d[1] = (byte)GGreen;
                    dataPtr_d[2] = (byte)GRed;

                    dataPtr += widthstep;
                    dataPtr_d += widthstep;
                }

                dataPtr_d[0] = 0;
                dataPtr_d[1] = 0;
                dataPtr_d[2] = 0;

                dataPtr -= nChannel;
                dataPtr_d -= nChannel;

                for (int x_inf = 1; x_inf < widht; x_inf++)
                {
                    GBlue = Math.Abs(dataPtr[0] - (dataPtr + nChannel)[0]) + Math.Abs((dataPtr + nChannel)[0] - (dataPtr)[0]);
                    GGreen = Math.Abs(dataPtr[1] - (dataPtr + nChannel)[1]) + Math.Abs((dataPtr + nChannel)[1] - (dataPtr)[1]);
                    GRed = Math.Abs(dataPtr[2] - (dataPtr + nChannel)[2]) + Math.Abs((dataPtr + nChannel)[2] - (dataPtr)[2]);

                    if (GBlue > 255) GBlue = 255;
                    if (GGreen > 255) GGreen = 255;
                    if (GRed > 255) GRed = 255;

                    dataPtr_d[0] = (byte)GBlue;
                    dataPtr_d[1] = (byte)GGreen;
                    dataPtr_d[2] = (byte)GRed;

                    dataPtr -= nChannel;
                    dataPtr_d -= nChannel;
                }
            }
        }

        public static void Median(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = imgCopy.MIplImage;
                MIplImage m_d = img.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer();
                byte* dataPtr_d = (byte*)m_d.imageData.ToPointer();

                int widht = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int widthstep = m.widthStep;
                int padding = m_d.widthStep - m_d.nChannels * m_d.width;

                int y, x, blue, green, red, blue_2, green_2, red_2;
                int d1, d2, d3, d4, d5, d6, d7, d8, dTotal;

                int min;
                int pixel;

                int[] distancias = new int[9];

                dataPtr += widthstep + nChannel;
                dataPtr_d += widthstep + nChannel;

                //centro
                for (y = 1; y < height - 1; y++)
                {
                    for (x = 1; x < widht - 1; x++)
                    {
                        min = 0;
                        pixel = 4;


                        // 1º PIXEL //
                        blue = (dataPtr - nChannel - widthstep)[0];
                        green = (dataPtr - nChannel - widthstep)[1];
                        red = (dataPtr - nChannel - widthstep)[2];

                        blue_2 = (dataPtr + widthstep)[0];
                        green_2 = (dataPtr + widthstep)[1];
                        red_2 = (dataPtr + widthstep)[2];

                        d1 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel - widthstep)[0];
                        green_2 = (dataPtr + nChannel - widthstep)[1];
                        red_2 = (dataPtr + nChannel - widthstep)[2];

                        d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - nChannel)[0];
                        green_2 = (dataPtr - nChannel)[1];
                        red_2 = (dataPtr - nChannel)[2];

                        d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = dataPtr[0];
                        green_2 = dataPtr[1];
                        red_2 = dataPtr[2];

                        d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel)[0];
                        green_2 = (dataPtr + nChannel)[1];
                        red_2 = (dataPtr + nChannel)[2];

                        d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - nChannel + widthstep)[0];
                        green_2 = (dataPtr - nChannel + widthstep)[1];
                        red_2 = (dataPtr - nChannel + widthstep)[2];

                        d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + widthstep)[0];
                        green_2 = (dataPtr + widthstep)[1];
                        red_2 = (dataPtr + widthstep)[2];

                        d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel + widthstep)[0];
                        green_2 = (dataPtr + nChannel + widthstep)[1];
                        red_2 = (dataPtr + nChannel + widthstep)[2];

                        d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                        distancias[0] = dTotal;

                        // 2º PIXEL //
                        blue = (dataPtr - widthstep)[0];
                        green = (dataPtr - widthstep)[1];
                        red = (dataPtr - widthstep)[2];

                        blue_2 = (dataPtr + nChannel - widthstep)[0];
                        green_2 = (dataPtr + nChannel - widthstep)[1];
                        red_2 = (dataPtr + nChannel - widthstep)[2];

                        d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - nChannel)[0];
                        green_2 = (dataPtr - nChannel)[1];
                        red_2 = (dataPtr - nChannel)[2];

                        d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = dataPtr[0];
                        green_2 = dataPtr[1];
                        red_2 = dataPtr[2];

                        d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel)[0];
                        green_2 = (dataPtr + nChannel)[1];
                        red_2 = (dataPtr + nChannel)[2];

                        d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - nChannel + widthstep)[0];
                        green_2 = (dataPtr - nChannel + widthstep)[1];
                        red_2 = (dataPtr - nChannel + widthstep)[2];

                        d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + widthstep)[0];
                        green_2 = (dataPtr + widthstep)[1];
                        red_2 = (dataPtr + widthstep)[2];

                        d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel + widthstep)[0];
                        green_2 = (dataPtr + nChannel + widthstep)[1];
                        red_2 = (dataPtr + nChannel + widthstep)[2];

                        d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                        distancias[1] = dTotal;

                        // 3º PIXEL //
                        blue = (dataPtr + nChannel - widthstep)[0];
                        green = (dataPtr + nChannel - widthstep)[1];
                        red = (dataPtr + nChannel - widthstep)[2];

                        d1 = d2;

                        blue_2 = (dataPtr - nChannel - widthstep)[0];
                        green_2 = (dataPtr - nChannel - widthstep)[1];
                        red_2 = (dataPtr - nChannel - widthstep)[2];

                        d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - nChannel)[0];
                        green_2 = (dataPtr - nChannel)[1];
                        red_2 = (dataPtr - nChannel)[2];

                        d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = dataPtr[0];
                        green_2 = dataPtr[1];
                        red_2 = dataPtr[2];

                        d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel)[0];
                        green_2 = (dataPtr + nChannel)[1];
                        red_2 = (dataPtr + nChannel)[2];

                        d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - nChannel + widthstep)[0];
                        green_2 = (dataPtr - nChannel + widthstep)[1];
                        red_2 = (dataPtr - nChannel + widthstep)[2];

                        d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + widthstep)[0];
                        green_2 = (dataPtr + widthstep)[1];
                        red_2 = (dataPtr + widthstep)[2];

                        d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel + widthstep)[0];
                        green_2 = (dataPtr + nChannel + widthstep)[1];
                        red_2 = (dataPtr + nChannel + widthstep)[2];

                        d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                        distancias[2] = dTotal;

                        // 4º PIXEL //
                        blue = (dataPtr - nChannel)[0];
                        green = (dataPtr - nChannel)[1];
                        red = (dataPtr - nChannel)[2];

                        d1 = d3;

                        blue_2 = (dataPtr - nChannel - widthstep)[0];
                        green_2 = (dataPtr - nChannel - widthstep)[1];
                        red_2 = (dataPtr - nChannel - widthstep)[2];

                        d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - widthstep)[0];
                        green_2 = (dataPtr - widthstep)[1];
                        red_2 = (dataPtr - widthstep)[2];

                        d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = dataPtr[0];
                        green_2 = dataPtr[1];
                        red_2 = dataPtr[2];

                        d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel)[0];
                        green_2 = (dataPtr + nChannel)[1];
                        red_2 = (dataPtr + nChannel)[2];

                        d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - nChannel + widthstep)[0];
                        green_2 = (dataPtr - nChannel + widthstep)[1];
                        red_2 = (dataPtr - nChannel + widthstep)[2];

                        d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + widthstep)[0];
                        green_2 = (dataPtr + widthstep)[1];
                        red_2 = (dataPtr + widthstep)[2];

                        d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel + widthstep)[0];
                        green_2 = (dataPtr + nChannel + widthstep)[1];
                        red_2 = (dataPtr + nChannel + widthstep)[2];

                        d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                        distancias[3] = dTotal;

                        // 5º PIXEL //
                        blue = dataPtr[0];
                        green = dataPtr[1];
                        red = dataPtr[2];

                        d1 = d4;

                        blue_2 = (dataPtr - nChannel - widthstep)[0];
                        green_2 = (dataPtr - nChannel - widthstep)[1];
                        red_2 = (dataPtr - nChannel - widthstep)[2];

                        d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - widthstep)[0];
                        green_2 = (dataPtr - widthstep)[1];
                        red_2 = (dataPtr - widthstep)[2];

                        d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel - widthstep)[0];
                        green_2 = (dataPtr + nChannel - widthstep)[1];
                        red_2 = (dataPtr + nChannel - widthstep)[2];

                        d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel)[0];
                        green_2 = (dataPtr + nChannel)[1];
                        red_2 = (dataPtr + nChannel)[2];

                        d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - nChannel + widthstep)[0];
                        green_2 = (dataPtr - nChannel + widthstep)[1];
                        red_2 = (dataPtr - nChannel + widthstep)[2];

                        d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + widthstep)[0];
                        green_2 = (dataPtr + widthstep)[1];
                        red_2 = (dataPtr + widthstep)[2];

                        d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel + widthstep)[0];
                        green_2 = (dataPtr + nChannel + widthstep)[1];
                        red_2 = (dataPtr + nChannel + widthstep)[2];

                        d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                        distancias[4] = dTotal;

                        // 6º PIXEL //
                        blue = (dataPtr + nChannel)[0];
                        green = (dataPtr + nChannel)[1];
                        red = (dataPtr + nChannel)[2];

                        d1 = d5;

                        blue_2 = (dataPtr - nChannel - widthstep)[0];
                        green_2 = (dataPtr - nChannel - widthstep)[1];
                        red_2 = (dataPtr - nChannel - widthstep)[2];

                        d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - widthstep)[0];
                        green_2 = (dataPtr - widthstep)[1];
                        red_2 = (dataPtr - widthstep)[2];

                        d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel - widthstep)[0];
                        green_2 = (dataPtr + nChannel - widthstep)[1];
                        red_2 = (dataPtr + nChannel - widthstep)[2];

                        d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - nChannel)[0];
                        green_2 = (dataPtr - nChannel)[1];
                        red_2 = (dataPtr - nChannel)[2];

                        d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - nChannel + widthstep)[0];
                        green_2 = (dataPtr - nChannel + widthstep)[1];
                        red_2 = (dataPtr - nChannel + widthstep)[2];

                        d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + widthstep)[0];
                        green_2 = (dataPtr + widthstep)[1];
                        red_2 = (dataPtr + widthstep)[2];

                        d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel + widthstep)[0];
                        green_2 = (dataPtr + nChannel + widthstep)[1];
                        red_2 = (dataPtr + nChannel + widthstep)[2];

                        d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                        distancias[5] = dTotal;

                        // 7º PIXEL //
                        blue = (dataPtr - nChannel + widthstep)[0];
                        green = (dataPtr - nChannel + widthstep)[1];
                        red = (dataPtr - nChannel + widthstep)[2];

                        d1 = d6;

                        blue_2 = (dataPtr - nChannel - widthstep)[0];
                        green_2 = (dataPtr - nChannel - widthstep)[1];
                        red_2 = (dataPtr - nChannel - widthstep)[2];

                        d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - widthstep)[0];
                        green_2 = (dataPtr - widthstep)[1];
                        red_2 = (dataPtr - widthstep)[2];

                        d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel - widthstep)[0];
                        green_2 = (dataPtr + nChannel - widthstep)[1];
                        red_2 = (dataPtr + nChannel - widthstep)[2];

                        d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - nChannel)[0];
                        green_2 = (dataPtr - nChannel)[1];
                        red_2 = (dataPtr - nChannel)[2];

                        d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = dataPtr[0];
                        green_2 = dataPtr[1];
                        red_2 = dataPtr[2];

                        d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + widthstep)[0];
                        green_2 = (dataPtr + widthstep)[1];
                        red_2 = (dataPtr + widthstep)[2];

                        d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel + widthstep)[0];
                        green_2 = (dataPtr + nChannel + widthstep)[1];
                        red_2 = (dataPtr + nChannel + widthstep)[2];

                        d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                        distancias[6] = dTotal;

                        // 8º PIXEL //
                        blue = (dataPtr + widthstep)[0];
                        green = (dataPtr + widthstep)[1];
                        red = (dataPtr + widthstep)[2];

                        d1 = d7;

                        blue_2 = (dataPtr - nChannel - widthstep)[0];
                        green_2 = (dataPtr - nChannel - widthstep)[1];
                        red_2 = (dataPtr - nChannel - widthstep)[2];

                        d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - widthstep)[0];
                        green_2 = (dataPtr - widthstep)[1];
                        red_2 = (dataPtr - widthstep)[2];

                        d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel - widthstep)[0];
                        green_2 = (dataPtr + nChannel - widthstep)[1];
                        red_2 = (dataPtr + nChannel - widthstep)[2];

                        d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - nChannel)[0];
                        green_2 = (dataPtr - nChannel)[1];
                        red_2 = (dataPtr - nChannel)[2];

                        d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = dataPtr[0];
                        green_2 = dataPtr[1];
                        red_2 = dataPtr[2];

                        d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel)[0];
                        green_2 = (dataPtr + nChannel)[1];
                        red_2 = (dataPtr + nChannel)[2];

                        d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel + widthstep)[0];
                        green_2 = (dataPtr + nChannel + widthstep)[1];
                        red_2 = (dataPtr + nChannel + widthstep)[2];

                        d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                        distancias[7] = dTotal;

                        // 9º PIXEL //
                        blue = (dataPtr + nChannel + widthstep)[0];
                        green = (dataPtr + nChannel + widthstep)[1];
                        red = (dataPtr + nChannel + widthstep)[2];

                        d1 = d8;

                        blue_2 = (dataPtr - nChannel - widthstep)[0];
                        green_2 = (dataPtr - nChannel - widthstep)[1];
                        red_2 = (dataPtr - nChannel - widthstep)[2];

                        d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - widthstep)[0];
                        green_2 = (dataPtr - widthstep)[1];
                        red_2 = (dataPtr - widthstep)[2];

                        d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel - widthstep)[0];
                        green_2 = (dataPtr + nChannel - widthstep)[1];
                        red_2 = (dataPtr + nChannel - widthstep)[2];

                        d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - nChannel)[0];
                        green_2 = (dataPtr - nChannel)[1];
                        red_2 = (dataPtr - nChannel)[2];

                        d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = dataPtr[0];
                        green_2 = dataPtr[1];
                        red_2 = dataPtr[2];

                        d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr + nChannel)[0];
                        green_2 = (dataPtr + nChannel)[1];
                        red_2 = (dataPtr + nChannel)[2];

                        d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        blue_2 = (dataPtr - nChannel + widthstep)[0];
                        green_2 = (dataPtr - nChannel + widthstep)[1];
                        red_2 = (dataPtr - nChannel + widthstep)[2];

                        d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                        dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                        distancias[8] = dTotal;

                        for (int i = 0; i < 9; i++)
                        {
                            if(min == 0)
                            {
                                min = distancias[i];
                                pixel = i;
                            }
                            else if (distancias[i] < min)
                            {
                                min = distancias[i];
                                pixel = i;
                            }
                        }

                        switch (pixel)
                        {
                            case 0:
                                {
                                    dataPtr[0] = (dataPtr - nChannel - widthstep)[0];
                                    dataPtr[1] = (dataPtr - nChannel - widthstep)[1];
                                    dataPtr[2] = (dataPtr - nChannel - widthstep)[2];
                                    break;
                                }
                            case 1:
                                {
                                    dataPtr[0] = (dataPtr - widthstep)[0];
                                    dataPtr[1] = (dataPtr - widthstep)[1];
                                    dataPtr[2] = (dataPtr - widthstep)[3];
                                    break;
                                }
                            case 2:
                                {
                                    dataPtr[0] = (dataPtr + nChannel - widthstep)[0];
                                    dataPtr[1] = (dataPtr + nChannel - widthstep)[1];
                                    dataPtr[2] = (dataPtr + nChannel - widthstep)[2];
                                    break;
                                }
                            case 3:
                                {
                                    dataPtr[0] = (dataPtr - nChannel)[0];
                                    dataPtr[1] = (dataPtr - nChannel)[1];
                                    dataPtr[2] = (dataPtr - nChannel)[2];
                                    break;
                                }
                            case 5:
                                {
                                    dataPtr[0] = (dataPtr + nChannel)[0];
                                    dataPtr[1] = (dataPtr + nChannel)[1];
                                    dataPtr[2] = (dataPtr + nChannel)[2];
                                    break;
                                }
                            case 6:
                                {
                                    dataPtr[0] = (dataPtr - nChannel + widthstep)[0];
                                    dataPtr[1] = (dataPtr - nChannel + widthstep)[1];
                                    dataPtr[2] = (dataPtr - nChannel + widthstep)[2];
                                    break;
                                }
                            case 7:
                                {
                                    dataPtr[0] = (dataPtr + widthstep)[0];
                                    dataPtr[1] = (dataPtr + widthstep)[1];
                                    dataPtr[2] = (dataPtr + widthstep)[2];
                                    break;
                                }
                            case 8:
                                {
                                    dataPtr[0] = (dataPtr + nChannel + widthstep)[0];
                                    dataPtr[1] = (dataPtr + nChannel + widthstep)[1];
                                    dataPtr[2] = (dataPtr + nChannel + widthstep)[2];
                                    break;
                                }
                            default:
                                break;
                        }

                        dataPtr += nChannel;
                        dataPtr_d += nChannel;
                    }
                    dataPtr_d += padding + 2 * nChannel;
                    dataPtr += padding + 2 * nChannel;
                }

                //canto superior esquerdo
                min = 0;
                pixel = 4;

                dataPtr = (byte*)m.imageData.ToPointer();
                dataPtr_d = (byte*)m_d.imageData.ToPointer();

                // 5º PIXEL //
                blue = dataPtr[0];
                green = dataPtr[1];
                red = dataPtr[2];

                d1 = d2 = d4 = 0;

                blue_2 = (dataPtr + nChannel)[0];
                green_2 = (dataPtr + nChannel)[1];
                red_2 = (dataPtr + nChannel)[2];

                d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                d3 = d5;

                blue_2 = (dataPtr + widthstep)[0];
                green_2 = (dataPtr + widthstep)[1];
                red_2 = (dataPtr + widthstep)[2];

                d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                d6 = d7;

                blue_2 = (dataPtr + nChannel + widthstep)[0];
                green_2 = (dataPtr + nChannel + widthstep)[1];
                red_2 = (dataPtr + nChannel + widthstep)[2];

                d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                distancias[0] = distancias[1] = distancias[3] = distancias[4] = dTotal;


                // 6º PIXEL //
                blue = (dataPtr + nChannel)[0];
                green = (dataPtr + nChannel)[1];
                red = (dataPtr + nChannel)[2];

                d1 = d2 = d4 = d5;

                d3 = 0;

                blue_2 = (dataPtr + widthstep)[0];
                green_2 = (dataPtr + widthstep)[1];
                red_2 = (dataPtr + widthstep)[2];

                d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                d7 = d6;

                blue_2 = (dataPtr + nChannel + widthstep)[0];
                green_2 = (dataPtr + nChannel + widthstep)[1];
                red_2 = (dataPtr + nChannel + widthstep)[2];

                d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                distancias[2] = distancias[5] = dTotal;


                // 8º PIXEL //
                blue = (dataPtr + widthstep)[0];
                green = (dataPtr + widthstep)[1];
                red = (dataPtr + widthstep)[2];

                d3 = d6;

                d7 = 0;

                blue_2 = dataPtr[0];
                green_2 = dataPtr[1];
                red_2 = dataPtr[2];

                d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                d1 = d2 = d4 = d5;

                blue_2 = (dataPtr + nChannel + widthstep)[0];
                green_2 = (dataPtr + nChannel + widthstep)[1];
                red_2 = (dataPtr + nChannel + widthstep)[2];

                d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                distancias[6] = distancias[7] = dTotal;

                // 9º PIXEL //
                blue = (dataPtr + nChannel + widthstep)[0];
                green = (dataPtr + nChannel + widthstep)[1];
                red = (dataPtr + nChannel + widthstep)[2];

                blue_2 = (dataPtr + nChannel)[0];
                green_2 = (dataPtr + nChannel)[1];
                red_2 = (dataPtr + nChannel)[2];

                d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                d3 = d6;


                blue_2 = dataPtr[0];
                green_2 = dataPtr[1];
                red_2 = dataPtr[2];

                d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                d1 = d2 = d4 = d5;

                blue_2 = (dataPtr + widthstep)[0];
                green_2 = (dataPtr + widthstep)[1];
                red_2 = (dataPtr + widthstep)[2];

                d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                d7 = d8;

                dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                distancias[8] = dTotal;

                for (int i = 0; i < 9; i++)
                {
                    if (min == 0)
                    {
                        min = distancias[i];
                        pixel = i;
                    }
                    else if (distancias[i] < min)
                    {
                        min = distancias[i];
                        pixel = i;
                    }
                }

                switch (pixel)
                {
                    case 0:
                        {
                            dataPtr[0] = (dataPtr - nChannel - widthstep)[0];
                            dataPtr[1] = (dataPtr - nChannel - widthstep)[1];
                            dataPtr[2] = (dataPtr - nChannel - widthstep)[2];
                            break;
                        }
                    case 1:
                        {
                            dataPtr[0] = (dataPtr - widthstep)[0];
                            dataPtr[1] = (dataPtr - widthstep)[1];
                            dataPtr[2] = (dataPtr - widthstep)[3];
                            break;
                        }
                    case 2:
                        {
                            dataPtr[0] = (dataPtr + nChannel - widthstep)[0];
                            dataPtr[1] = (dataPtr + nChannel - widthstep)[1];
                            dataPtr[2] = (dataPtr + nChannel - widthstep)[2];
                            break;
                        }
                    case 3:
                        {
                            dataPtr[0] = (dataPtr - nChannel)[0];
                            dataPtr[1] = (dataPtr - nChannel)[1];
                            dataPtr[2] = (dataPtr - nChannel)[2];
                            break;
                        }
                    case 5:
                        {
                            dataPtr[0] = (dataPtr + nChannel)[0];
                            dataPtr[1] = (dataPtr + nChannel)[1];
                            dataPtr[2] = (dataPtr + nChannel)[2];
                            break;
                        }
                    case 6:
                        {
                            dataPtr[0] = (dataPtr - nChannel + widthstep)[0];
                            dataPtr[1] = (dataPtr - nChannel + widthstep)[1];
                            dataPtr[2] = (dataPtr - nChannel + widthstep)[2];
                            break;
                        }
                    case 7:
                        {
                            dataPtr[0] = (dataPtr + widthstep)[0];
                            dataPtr[1] = (dataPtr + widthstep)[1];
                            dataPtr[2] = (dataPtr + widthstep)[2];
                            break;
                        }
                    case 8:
                        {
                            dataPtr[0] = (dataPtr + nChannel + widthstep)[0];
                            dataPtr[1] = (dataPtr + nChannel + widthstep)[1];
                            dataPtr[2] = (dataPtr + nChannel + widthstep)[2];
                            break;
                        }
                    default:
                        break;
                }


                dataPtr += nChannel;
                dataPtr_d += nChannel;

                //margem superior
                for (int x_sup = 1; x_sup < widht - 1; x_sup++)
                {
                    min = 0;
                    pixel = 4;

                    // 4º PIXEL //
                    blue = (dataPtr - nChannel)[0];
                    green = (dataPtr - nChannel)[1];
                    red = (dataPtr - nChannel)[2];

                    d1 = 0;

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d4 = d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel)[0];
                    green_2 = (dataPtr + nChannel)[1];
                    red_2 = (dataPtr + nChannel)[2];

                    d3 = d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel + widthstep)[0];
                    green_2 = (dataPtr - nChannel + widthstep)[1];
                    red_2 = (dataPtr - nChannel + widthstep)[2];

                    d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel + widthstep)[0];
                    green_2 = (dataPtr + nChannel + widthstep)[1];
                    red_2 = (dataPtr + nChannel + widthstep)[2];

                    d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[0] = distancias[3] = dTotal;

                    // 5º PIXEL //
                    blue = dataPtr[0];
                    green = dataPtr[1];
                    red = dataPtr[2];

                    d2 = 0;

                    blue_2 = (dataPtr - nChannel)[0];
                    green_2 = (dataPtr - nChannel)[1];
                    red_2 = (dataPtr - nChannel)[2];

                    d1 = d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel)[0];
                    green_2 = (dataPtr + nChannel)[1];
                    red_2 = (dataPtr + nChannel)[2];

                    d3 = d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel + widthstep)[0];
                    green_2 = (dataPtr - nChannel + widthstep)[1];
                    red_2 = (dataPtr - nChannel + widthstep)[2];

                    d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel + widthstep)[0];
                    green_2 = (dataPtr + nChannel + widthstep)[1];
                    red_2 = (dataPtr + nChannel + widthstep)[2];

                    d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[1] = distancias[4] = dTotal;

                    // 6º PIXEL //
                    blue = (dataPtr + nChannel)[0];
                    green = (dataPtr + nChannel)[1];
                    red = (dataPtr + nChannel)[2];

                    d3 = 0;

                    blue_2 = (dataPtr - nChannel)[0];
                    green_2 = (dataPtr - nChannel)[1];
                    red_2 = (dataPtr - nChannel)[2];

                    d1 = d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d2 = d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel + widthstep)[0];
                    green_2 = (dataPtr - nChannel + widthstep)[1];
                    red_2 = (dataPtr - nChannel + widthstep)[2];

                    d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel + widthstep)[0];
                    green_2 = (dataPtr + nChannel + widthstep)[1];
                    red_2 = (dataPtr + nChannel + widthstep)[2];

                    d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[2] = distancias[5] = dTotal;

                    // 7º PIXEL //
                    blue = (dataPtr - nChannel + widthstep)[0];
                    green = (dataPtr - nChannel + widthstep)[1];
                    red = (dataPtr - nChannel + widthstep)[2];

                    blue_2 = (dataPtr - nChannel)[0];
                    green_2 = (dataPtr - nChannel)[1];
                    red_2 = (dataPtr - nChannel)[2];

                    d1 = d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d2 = d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel)[0];
                    green_2 = (dataPtr + nChannel)[1];
                    red_2 = (dataPtr + nChannel)[2];

                    d3 = d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel + widthstep)[0];
                    green_2 = (dataPtr + nChannel + widthstep)[1];
                    red_2 = (dataPtr + nChannel + widthstep)[2];

                    d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[6] = dTotal;

                    // 8º PIXEL //
                    blue = (dataPtr + widthstep)[0];
                    green = (dataPtr + widthstep)[1];
                    red = (dataPtr + widthstep)[2];

                    blue_2 = (dataPtr - nChannel)[0];
                    green_2 = (dataPtr - nChannel)[1];
                    red_2 = (dataPtr - nChannel)[2];

                    d1 = d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d2 = d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel)[0];
                    green_2 = (dataPtr + nChannel)[1];
                    red_2 = (dataPtr + nChannel)[2];

                    d3 = d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel + widthstep)[0];
                    green_2 = (dataPtr - nChannel + widthstep)[1];
                    red_2 = (dataPtr - nChannel + widthstep)[2];


                    d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel + widthstep)[0];
                    green_2 = (dataPtr + nChannel + widthstep)[1];
                    red_2 = (dataPtr + nChannel + widthstep)[2];

                    d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[7] = dTotal;

                    // 9º PIXEL //
                    blue = (dataPtr + nChannel + widthstep)[0];
                    green = (dataPtr + nChannel + widthstep)[1];
                    red = (dataPtr + nChannel + widthstep)[2];

                    blue_2 = (dataPtr - nChannel)[0];
                    green_2 = (dataPtr - nChannel)[1];
                    red_2 = (dataPtr - nChannel)[2];

                    d1 = d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d2 = d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel)[0];
                    green_2 = (dataPtr + nChannel)[1];
                    red_2 = (dataPtr + nChannel)[2];

                    d3 = d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel + widthstep)[0];
                    green_2 = (dataPtr - nChannel + widthstep)[1];
                    red_2 = (dataPtr - nChannel + widthstep)[2];


                    d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[8] = dTotal;

                    for (int i = 0; i < 9; i++)
                    {
                        if (min == 0)
                        {
                            min = distancias[i];
                            pixel = i;
                        }
                        else if (distancias[i] < min)
                        {
                            min = distancias[i];
                            pixel = i;
                        }
                    }

                    switch (pixel)
                    {
                        case 0:
                            {
                                dataPtr[0] = (dataPtr - nChannel - widthstep)[0];
                                dataPtr[1] = (dataPtr - nChannel - widthstep)[1];
                                dataPtr[2] = (dataPtr - nChannel - widthstep)[2];
                                break;
                            }
                        case 1:
                            {
                                dataPtr[0] = (dataPtr - widthstep)[0];
                                dataPtr[1] = (dataPtr - widthstep)[1];
                                dataPtr[2] = (dataPtr - widthstep)[3];
                                break;
                            }
                        case 2:
                            {
                                dataPtr[0] = (dataPtr + nChannel - widthstep)[0];
                                dataPtr[1] = (dataPtr + nChannel - widthstep)[1];
                                dataPtr[2] = (dataPtr + nChannel - widthstep)[2];
                                break;
                            }
                        case 3:
                            {
                                dataPtr[0] = (dataPtr - nChannel)[0];
                                dataPtr[1] = (dataPtr - nChannel)[1];
                                dataPtr[2] = (dataPtr - nChannel)[2];
                                break;
                            }
                        case 5:
                            {
                                dataPtr[0] = (dataPtr + nChannel)[0];
                                dataPtr[1] = (dataPtr + nChannel)[1];
                                dataPtr[2] = (dataPtr + nChannel)[2];
                                break;
                            }
                        case 6:
                            {
                                dataPtr[0] = (dataPtr - nChannel + widthstep)[0];
                                dataPtr[1] = (dataPtr - nChannel + widthstep)[1];
                                dataPtr[2] = (dataPtr - nChannel + widthstep)[2];
                                break;
                            }
                        case 7:
                            {
                                dataPtr[0] = (dataPtr + widthstep)[0];
                                dataPtr[1] = (dataPtr + widthstep)[1];
                                dataPtr[2] = (dataPtr + widthstep)[2];
                                break;
                            }
                        case 8:
                            {
                                dataPtr[0] = (dataPtr + nChannel + widthstep)[0];
                                dataPtr[1] = (dataPtr + nChannel + widthstep)[1];
                                dataPtr[2] = (dataPtr + nChannel + widthstep)[2];
                                break;
                            }
                        default:
                            break;
                    }

                    dataPtr += nChannel;
                    dataPtr_d += nChannel;
                }

                //canto superior direito
                min = 0;
                pixel = 4;
                
                blue = dataPtr[0];
                green = dataPtr[1];
                red = dataPtr[2];

                d3 = d2 = d5 = 0;

                blue_2 = (dataPtr - nChannel)[0];
                green_2 = (dataPtr - nChannel)[1];
                red_2 = (dataPtr - nChannel)[2];

                d1 = d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = (dataPtr + widthstep)[0];
                green_2 = (dataPtr + widthstep)[1];
                red_2 = (dataPtr + widthstep)[2];

                d7 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = (dataPtr - nChannel + widthstep)[0];
                green_2 = (dataPtr - nChannel + widthstep)[1];
                red_2 = (dataPtr - nChannel + widthstep)[2];

                d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                distancias[1] = distancias[2] = distancias[4] = distancias[5] = dTotal;


                // 6º PIXEL //
                blue = (dataPtr - nChannel)[0];
                green = (dataPtr - nChannel)[1];
                red = (dataPtr - nChannel)[2];

                d1 = 0;

                blue_2 = dataPtr[0];
                green_2 = dataPtr[1];
                red_2 = dataPtr[2];

                d2 = d3 = d4 = d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = (dataPtr - nChannel + widthstep)[0];
                green_2 = (dataPtr - nChannel + widthstep)[1];
                red_2 = (dataPtr - nChannel + widthstep)[2];

                d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = (dataPtr + widthstep)[0];
                green_2 = (dataPtr + widthstep)[1];
                red_2 = (dataPtr + widthstep)[2];

                d7 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                distancias[0] = distancias[3] = dTotal;


                // 8º PIXEL //
                blue = (dataPtr - nChannel + widthstep)[0];
                green = (dataPtr - nChannel + widthstep)[1];
                red = (dataPtr - nChannel + widthstep)[2];

                blue_2 = dataPtr[0];
                green_2 = dataPtr[1];
                red_2 = dataPtr[2];

                d2 = d3 = d5 = d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = (dataPtr - nChannel)[0];
                green_2 = (dataPtr - nChannel)[1];
                red_2 = (dataPtr - nChannel)[2];

                d1 = d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = (dataPtr + widthstep)[0];
                green_2 = (dataPtr + widthstep)[1];
                red_2 = (dataPtr + widthstep)[2];

                d7 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);


                distancias[6] = dTotal;

                // 9º PIXEL //
                blue = (dataPtr + widthstep)[0];
                green = (dataPtr + widthstep)[1];
                red = (dataPtr + widthstep)[2];
                
                d8 = 0;
                
                blue_2 = (dataPtr - nChannel)[0];
                green_2 = (dataPtr - nChannel)[1];
                red_2 = (dataPtr - nChannel)[2];

                d1 = d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = dataPtr[0];
                green_2 = dataPtr[1];
                red_2 = dataPtr[2];

                d2 = d3 = d5 = d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = (dataPtr - nChannel + widthstep)[0];
                green_2 = (dataPtr - nChannel + widthstep)[1];
                red_2 = (dataPtr - nChannel + widthstep)[2];

                d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                distancias[8] = distancias[7] = dTotal;

                for (int i = 0; i < 9; i++)
                {
                    if (min == 0)
                    {
                        min = distancias[i];
                        pixel = i;
                    }
                    else if (distancias[i] < min)
                    {
                        min = distancias[i];
                        pixel = i;
                    }
                }

                switch (pixel)
                {
                    case 0:
                        {
                            dataPtr[0] = (dataPtr - nChannel - widthstep)[0];
                            dataPtr[1] = (dataPtr - nChannel - widthstep)[1];
                            dataPtr[2] = (dataPtr - nChannel - widthstep)[2];
                            break;
                        }
                    case 1:
                        {
                            dataPtr[0] = (dataPtr - widthstep)[0];
                            dataPtr[1] = (dataPtr - widthstep)[1];
                            dataPtr[2] = (dataPtr - widthstep)[3];
                            break;
                        }
                    case 2:
                        {
                            dataPtr[0] = (dataPtr + nChannel - widthstep)[0];
                            dataPtr[1] = (dataPtr + nChannel - widthstep)[1];
                            dataPtr[2] = (dataPtr + nChannel - widthstep)[2];
                            break;
                        }
                    case 3:
                        {
                            dataPtr[0] = (dataPtr - nChannel)[0];
                            dataPtr[1] = (dataPtr - nChannel)[1];
                            dataPtr[2] = (dataPtr - nChannel)[2];
                            break;
                        }
                    case 5:
                        {
                            dataPtr[0] = (dataPtr + nChannel)[0];
                            dataPtr[1] = (dataPtr + nChannel)[1];
                            dataPtr[2] = (dataPtr + nChannel)[2];
                            break;
                        }
                    case 6:
                        {
                            dataPtr[0] = (dataPtr - nChannel + widthstep)[0];
                            dataPtr[1] = (dataPtr - nChannel + widthstep)[1];
                            dataPtr[2] = (dataPtr - nChannel + widthstep)[2];
                            break;
                        }
                    case 7:
                        {
                            dataPtr[0] = (dataPtr + widthstep)[0];
                            dataPtr[1] = (dataPtr + widthstep)[1];
                            dataPtr[2] = (dataPtr + widthstep)[2];
                            break;
                        }
                    case 8:
                        {
                            dataPtr[0] = (dataPtr + nChannel + widthstep)[0];
                            dataPtr[1] = (dataPtr + nChannel + widthstep)[1];
                            dataPtr[2] = (dataPtr + nChannel + widthstep)[2];
                            break;
                        }
                    default:
                        break;
                }

                dataPtr += widthstep;
                dataPtr_d += widthstep;

                //margem direita
                for (int y_dir = 1; y_dir < height - 1; y_dir++)
                {
                    min = 0;
                    pixel = 4;

                    // 1º PIXEL //

                    blue = (dataPtr - nChannel - widthstep)[0];
                    green = (dataPtr - nChannel - widthstep)[1];
                    red = (dataPtr - nChannel - widthstep)[2];

                    blue_2 = (dataPtr - widthstep)[0];
                    green_2 = (dataPtr - widthstep)[1];
                    red_2 = (dataPtr - widthstep)[2];

                    d1 = d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel)[0];
                    green_2 = (dataPtr - nChannel)[1];
                    red_2 = (dataPtr - nChannel)[2];

                    d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d4 = d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel + widthstep)[0];
                    green_2 = (dataPtr - nChannel + widthstep)[1];
                    red_2 = (dataPtr - nChannel + widthstep)[2];

                    d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d7 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[0] = dTotal;

                    // 2º PIXEL //
                    blue = (dataPtr - widthstep)[0];
                    green = (dataPtr - widthstep)[1];
                    red = (dataPtr - widthstep)[2];

                    d2 = 0;

                    blue_2 = (dataPtr - nChannel - widthstep)[0];
                    green_2 = (dataPtr - nChannel - widthstep)[1];
                    red_2 = (dataPtr - nChannel - widthstep)[2];

                    d1 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel)[0];
                    green_2 = (dataPtr - nChannel)[1];
                    red_2 = (dataPtr - nChannel)[2];

                    d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d4 = d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel + widthstep)[0];
                    green_2 = (dataPtr - nChannel + widthstep)[1];
                    red_2 = (dataPtr - nChannel + widthstep)[2];

                    d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d7 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[1] = distancias[2] = dTotal;

                    // 4º PIXEL //
                    blue = (dataPtr - nChannel)[0];
                    green = (dataPtr - nChannel)[1];
                    red = (dataPtr - nChannel)[2];

                    blue_2 = (dataPtr - nChannel - widthstep)[0];
                    green_2 = (dataPtr - nChannel - widthstep)[1];
                    red_2 = (dataPtr - nChannel - widthstep)[2];

                    d1 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - widthstep)[0];
                    green_2 = (dataPtr - widthstep)[1];
                    red_2 = (dataPtr - widthstep)[2];

                    d2 = d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d4 = d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel + widthstep)[0];
                    green_2 = (dataPtr - nChannel + widthstep)[1];
                    red_2 = (dataPtr - nChannel + widthstep)[2];

                    d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d7 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[3] = dTotal;

                    // 5º PIXEL //
                    blue = dataPtr[0];
                    green = dataPtr[1];
                    red = dataPtr[2];

                    d5 = 0;

                    blue_2 = (dataPtr - nChannel - widthstep)[0];
                    green_2 = (dataPtr - nChannel - widthstep)[1];
                    red_2 = (dataPtr - nChannel - widthstep)[2];

                    d1 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - widthstep)[0];
                    green_2 = (dataPtr - widthstep)[1];
                    red_2 = (dataPtr - widthstep)[2];

                    d2 = d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel)[0];
                    green_2 = (dataPtr - nChannel)[1];
                    red_2 = (dataPtr - nChannel)[2];

                    d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel + widthstep)[0];
                    green_2 = (dataPtr - nChannel + widthstep)[1];
                    red_2 = (dataPtr - nChannel + widthstep)[2];

                    d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d7 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[4] = distancias[5] = dTotal;

                    // 7º PIXEL //
                    blue = (dataPtr - nChannel + widthstep)[0];
                    green = (dataPtr - nChannel + widthstep)[1];
                    red = (dataPtr - nChannel + widthstep)[2];

                    blue_2 = (dataPtr - nChannel - widthstep)[0];
                    green_2 = (dataPtr - nChannel - widthstep)[1];
                    red_2 = (dataPtr - nChannel - widthstep)[2];

                    d1 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - widthstep)[0];
                    green_2 = (dataPtr - widthstep)[1];
                    red_2 = (dataPtr - widthstep)[2];

                    d2 = d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel)[0];
                    green_2 = (dataPtr - nChannel)[1];
                    red_2 = (dataPtr - nChannel)[2];

                    d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d5 = d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d7 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[6] = dTotal;

                    // 8º PIXEL //
                    blue = (dataPtr + widthstep)[0];
                    green = (dataPtr + widthstep)[1];
                    red = (dataPtr + widthstep)[2];

                    d8 = 0;

                    blue_2 = (dataPtr - nChannel - widthstep)[0];
                    green_2 = (dataPtr - nChannel - widthstep)[1];
                    red_2 = (dataPtr - nChannel - widthstep)[2];

                    d1 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - widthstep)[0];
                    green_2 = (dataPtr - widthstep)[1];
                    red_2 = (dataPtr - widthstep)[2];

                    d2 = d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel)[0];
                    green_2 = (dataPtr - nChannel)[1];
                    red_2 = (dataPtr - nChannel)[2];

                    d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d5 = d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel + widthstep)[0];
                    green_2 = (dataPtr - nChannel + widthstep)[1];
                    red_2 = (dataPtr - nChannel + widthstep)[2];

                    d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[7] = distancias[8] = dTotal;

                    for (int i = 0; i < 9; i++)
                    {
                        if (min == 0)
                        {
                            min = distancias[i];
                            pixel = i;
                        }
                        else if (distancias[i] < min)
                        {
                            min = distancias[i];
                            pixel = i;
                        }
                    }

                    switch (pixel)
                    {
                        case 0:
                            {
                                dataPtr[0] = (dataPtr - nChannel - widthstep)[0];
                                dataPtr[1] = (dataPtr - nChannel - widthstep)[1];
                                dataPtr[2] = (dataPtr - nChannel - widthstep)[2];
                                break;
                            }
                        case 1:
                            {
                                dataPtr[0] = (dataPtr - widthstep)[0];
                                dataPtr[1] = (dataPtr - widthstep)[1];
                                dataPtr[2] = (dataPtr - widthstep)[3];
                                break;
                            }
                        case 2:
                            {
                                dataPtr[0] = (dataPtr + nChannel - widthstep)[0];
                                dataPtr[1] = (dataPtr + nChannel - widthstep)[1];
                                dataPtr[2] = (dataPtr + nChannel - widthstep)[2];
                                break;
                            }
                        case 3:
                            {
                                dataPtr[0] = (dataPtr - nChannel)[0];
                                dataPtr[1] = (dataPtr - nChannel)[1];
                                dataPtr[2] = (dataPtr - nChannel)[2];
                                break;
                            }
                        case 5:
                            {
                                dataPtr[0] = (dataPtr + nChannel)[0];
                                dataPtr[1] = (dataPtr + nChannel)[1];
                                dataPtr[2] = (dataPtr + nChannel)[2];
                                break;
                            }
                        case 6:
                            {
                                dataPtr[0] = (dataPtr - nChannel + widthstep)[0];
                                dataPtr[1] = (dataPtr - nChannel + widthstep)[1];
                                dataPtr[2] = (dataPtr - nChannel + widthstep)[2];
                                break;
                            }
                        case 7:
                            {
                                dataPtr[0] = (dataPtr + widthstep)[0];
                                dataPtr[1] = (dataPtr + widthstep)[1];
                                dataPtr[2] = (dataPtr + widthstep)[2];
                                break;
                            }
                        case 8:
                            {
                                dataPtr[0] = (dataPtr + nChannel + widthstep)[0];
                                dataPtr[1] = (dataPtr + nChannel + widthstep)[1];
                                dataPtr[2] = (dataPtr + nChannel + widthstep)[2];
                                break;
                            }
                        default:
                            break;
                    }

                    dataPtr += widthstep;
                    dataPtr_d += widthstep;
                }

                //canto inferior direito
                min = 0;
                pixel = 4;

                // 1º PIXEL //
                blue = (dataPtr - nChannel - widthstep)[0];
                green = (dataPtr - nChannel - widthstep)[1];
                red = (dataPtr - nChannel - widthstep)[2];

                blue_2 = (dataPtr - widthstep)[0];
                green_2 = (dataPtr - widthstep)[1];
                red_2 = (dataPtr - widthstep)[2];

                d1 = d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = (dataPtr - nChannel)[0];
                green_2 = (dataPtr - nChannel)[1];
                red_2 = (dataPtr - nChannel)[2];

                d3 = d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = dataPtr[0];
                green_2 = dataPtr[1];
                red_2 = dataPtr[2];

                d4 = d5 = d7 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                distancias[0] = dTotal;

                // 2º PIXEL //
                blue = (dataPtr - widthstep)[0];
                green = (dataPtr - widthstep)[1];
                red = (dataPtr - widthstep)[2];

                d2 = 0;

                blue_2 = (dataPtr - nChannel - widthstep)[0];
                green_2 = (dataPtr - nChannel - widthstep)[1];
                red_2 = (dataPtr - nChannel - widthstep)[2];

                d1 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = (dataPtr - nChannel)[0];
                green_2 = (dataPtr - nChannel)[1];
                red_2 = (dataPtr - nChannel)[2];

                d3 = d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = dataPtr[0];
                green_2 = dataPtr[1];
                red_2 = dataPtr[2];

                d4 = d5 = d7 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                distancias[1] = distancias[2] = dTotal;

                // 4º PIXEL //
                blue = (dataPtr - nChannel)[0];
                green = (dataPtr - nChannel)[1];
                red = (dataPtr - nChannel)[2];

                d6 = 0;

                blue_2 = (dataPtr - nChannel - widthstep)[0];
                green_2 = (dataPtr - nChannel - widthstep)[1];
                red_2 = (dataPtr - nChannel - widthstep)[2];

                d1 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = (dataPtr - widthstep)[0];
                green_2 = (dataPtr - widthstep)[1];
                red_2 = (dataPtr - widthstep)[2];

                d2 = d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = dataPtr[0];
                green_2 = dataPtr[1];
                red_2 = dataPtr[2];

                d4 = d5 = d7 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                distancias[3] = distancias[6] = dTotal;

                // 5º PIXEL //
                blue = dataPtr[0];
                green = dataPtr[1];
                red = dataPtr[2];

                d5 = d7 = d8 = 0;

                blue_2 = (dataPtr - nChannel - widthstep)[0];
                green_2 = (dataPtr - nChannel - widthstep)[1];
                red_2 = (dataPtr - nChannel - widthstep)[2];

                d1 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = (dataPtr - widthstep)[0];
                green_2 = (dataPtr - widthstep)[1];
                red_2 = (dataPtr - widthstep)[2];

                d2 = d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = (dataPtr - nChannel)[0];
                green_2 = (dataPtr - nChannel)[1];
                red_2 = (dataPtr - nChannel)[2];

                d4 = d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                distancias[4] = distancias[5] = distancias[7] = distancias[8] = dTotal;

                for (int i = 0; i < 9; i++)
                {
                    if (min == 0)
                    {
                        min = distancias[i];
                        pixel = i;
                    }
                    else if (distancias[i] < min)
                    {
                        min = distancias[i];
                        pixel = i;
                    }
                }

                switch (pixel)
                {
                    case 0:
                        {
                            dataPtr[0] = (dataPtr - nChannel - widthstep)[0];
                            dataPtr[1] = (dataPtr - nChannel - widthstep)[1];
                            dataPtr[2] = (dataPtr - nChannel - widthstep)[2];
                            break;
                        }
                    case 1:
                        {
                            dataPtr[0] = (dataPtr - widthstep)[0];
                            dataPtr[1] = (dataPtr - widthstep)[1];
                            dataPtr[2] = (dataPtr - widthstep)[3];
                            break;
                        }
                    case 2:
                        {
                            dataPtr[0] = (dataPtr + nChannel - widthstep)[0];
                            dataPtr[1] = (dataPtr + nChannel - widthstep)[1];
                            dataPtr[2] = (dataPtr + nChannel - widthstep)[2];
                            break;
                        }
                    case 3:
                        {
                            dataPtr[0] = (dataPtr - nChannel)[0];
                            dataPtr[1] = (dataPtr - nChannel)[1];
                            dataPtr[2] = (dataPtr - nChannel)[2];
                            break;
                        }
                    case 5:
                        {
                            dataPtr[0] = (dataPtr + nChannel)[0];
                            dataPtr[1] = (dataPtr + nChannel)[1];
                            dataPtr[2] = (dataPtr + nChannel)[2];
                            break;
                        }
                    case 6:
                        {
                            dataPtr[0] = (dataPtr - nChannel + widthstep)[0];
                            dataPtr[1] = (dataPtr - nChannel + widthstep)[1];
                            dataPtr[2] = (dataPtr - nChannel + widthstep)[2];
                            break;
                        }
                    case 7:
                        {
                            dataPtr[0] = (dataPtr + widthstep)[0];
                            dataPtr[1] = (dataPtr + widthstep)[1];
                            dataPtr[2] = (dataPtr + widthstep)[2];
                            break;
                        }
                    case 8:
                        {
                            dataPtr[0] = (dataPtr + nChannel + widthstep)[0];
                            dataPtr[1] = (dataPtr + nChannel + widthstep)[1];
                            dataPtr[2] = (dataPtr + nChannel + widthstep)[2];
                            break;
                        }
                    default:
                        break;
                }

                dataPtr -= nChannel;
                dataPtr_d -= nChannel;

                //margem inferior
                for (int x_inf = 1; x_inf < widht - 1; x_inf++)
                {
                    min = 0;
                    pixel = 4;

                    // 1º PIXEL //
                    blue = (dataPtr - nChannel - widthstep)[0];
                    green = (dataPtr - nChannel - widthstep)[1];
                    red = (dataPtr - nChannel - widthstep)[2];

                    blue_2 = (dataPtr - widthstep)[0];
                    green_2 = (dataPtr - widthstep)[1];
                    red_2 = (dataPtr - widthstep)[2];

                    d1 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel - widthstep)[0];
                    green_2 = (dataPtr + nChannel - widthstep)[1];
                    red_2 = (dataPtr + nChannel - widthstep)[2];

                    d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel)[0];
                    green_2 = (dataPtr - nChannel)[1];
                    red_2 = (dataPtr - nChannel)[2];

                    d3 = d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d4 = d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel)[0];
                    green_2 = (dataPtr + nChannel)[1];
                    red_2 = (dataPtr + nChannel)[2];

                    d5 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[0] = dTotal;

                    // 2º PIXEL //
                    blue = (dataPtr - widthstep)[0];
                    green = (dataPtr - widthstep)[1];
                    red = (dataPtr - widthstep)[2];

                    blue_2 = (dataPtr - widthstep - nChannel)[0];
                    green_2 = (dataPtr - widthstep - nChannel)[1];
                    red_2 = (dataPtr - widthstep - nChannel)[2];

                    d1 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel - widthstep)[0];
                    green_2 = (dataPtr + nChannel - widthstep)[1];
                    red_2 = (dataPtr + nChannel - widthstep)[2];

                    d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel)[0];
                    green_2 = (dataPtr - nChannel)[1];
                    red_2 = (dataPtr - nChannel)[2];

                    d3 = d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d4 = d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel)[0];
                    green_2 = (dataPtr + nChannel)[1];
                    red_2 = (dataPtr + nChannel)[2];

                    d5 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[1] = dTotal;

                    // 3º PIXEL //
                    blue = (dataPtr - widthstep + nChannel)[0];
                    green = (dataPtr - widthstep + nChannel)[1];
                    red = (dataPtr - widthstep + nChannel)[2];

                    blue_2 = (dataPtr - widthstep - nChannel)[0];
                    green_2 = (dataPtr - widthstep - nChannel)[1];
                    red_2 = (dataPtr - widthstep - nChannel)[2];

                    d1 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - widthstep)[0];
                    green_2 = (dataPtr - widthstep)[1];
                    red_2 = (dataPtr - widthstep)[2];

                    d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel)[0];
                    green_2 = (dataPtr - nChannel)[1];
                    red_2 = (dataPtr - nChannel)[2];

                    d3 = d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d4 = d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel)[0];
                    green_2 = (dataPtr + nChannel)[1];
                    red_2 = (dataPtr + nChannel)[2];

                    d5 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[2] = dTotal;

                    // 4º PIXEL //
                    blue = (dataPtr -  nChannel)[0];
                    green = (dataPtr - nChannel)[1];
                    red = (dataPtr - nChannel)[2];

                    d6 = 0;

                    blue_2 = (dataPtr - widthstep - nChannel)[0];
                    green_2 = (dataPtr - widthstep - nChannel)[1];
                    red_2 = (dataPtr - widthstep - nChannel)[2];

                    d1 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - widthstep)[0];
                    green_2 = (dataPtr - widthstep)[1];
                    red_2 = (dataPtr - widthstep)[2];

                    d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - widthstep + nChannel)[0];
                    green_2 = (dataPtr - widthstep + nChannel)[1];
                    red_2 = (dataPtr - widthstep + nChannel)[2];

                    d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d4 = d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel)[0];
                    green_2 = (dataPtr + nChannel)[1];
                    red_2 = (dataPtr + nChannel)[2];

                    d5 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[3] = distancias[6] = dTotal;

                    // 5º PIXEL //
                    blue = dataPtr[0];
                    green = dataPtr[1];
                    red = dataPtr[2];

                    d7 = 0;

                    blue_2 = (dataPtr - widthstep - nChannel)[0];
                    green_2 = (dataPtr - widthstep - nChannel)[1];
                    red_2 = (dataPtr - widthstep - nChannel)[2];

                    d1 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - widthstep)[0];
                    green_2 = (dataPtr - widthstep)[1];
                    red_2 = (dataPtr - widthstep)[2];

                    d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - widthstep + nChannel)[0];
                    green_2 = (dataPtr - widthstep + nChannel)[1];
                    red_2 = (dataPtr - widthstep + nChannel)[2];

                    d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d4 = d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel)[0];
                    green_2 = (dataPtr + nChannel)[1];
                    red_2 = (dataPtr + nChannel)[2];

                    d5 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[4] = distancias[7] = dTotal;

                    // 6º PIXEL //
                    blue = (dataPtr + nChannel)[0];
                    green = (dataPtr + nChannel)[1];
                    red = (dataPtr + nChannel)[2];

                    d8 = 0;

                    blue_2 = (dataPtr - widthstep - nChannel)[0];
                    green_2 = (dataPtr - widthstep - nChannel)[1];
                    red_2 = (dataPtr - widthstep - nChannel)[2];

                    d1 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - widthstep)[0];
                    green_2 = (dataPtr - widthstep)[1];
                    red_2 = (dataPtr - widthstep)[2];

                    d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - widthstep + nChannel)[0];
                    green_2 = (dataPtr - widthstep + nChannel)[1];
                    red_2 = (dataPtr - widthstep + nChannel)[2];

                    d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr - nChannel)[0];
                    green_2 = (dataPtr - nChannel)[1];
                    red_2 = (dataPtr - nChannel)[2];

                    d4 = d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d5 = d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[5] = distancias[8] = dTotal;

                    for (int i = 0; i < 9; i++)
                    {
                        if (min == 0)
                        {
                            min = distancias[i];
                            pixel = i;
                        }
                        else if (distancias[i] < min)
                        {
                            min = distancias[i];
                            pixel = i;
                        }
                    }

                    switch (pixel)
                    {
                        case 0:
                            {
                                dataPtr[0] = (dataPtr - nChannel - widthstep)[0];
                                dataPtr[1] = (dataPtr - nChannel - widthstep)[1];
                                dataPtr[2] = (dataPtr - nChannel - widthstep)[2];
                                break;
                            }
                        case 1:
                            {
                                dataPtr[0] = (dataPtr - widthstep)[0];
                                dataPtr[1] = (dataPtr - widthstep)[1];
                                dataPtr[2] = (dataPtr - widthstep)[3];
                                break;
                            }
                        case 2:
                            {
                                dataPtr[0] = (dataPtr + nChannel - widthstep)[0];
                                dataPtr[1] = (dataPtr + nChannel - widthstep)[1];
                                dataPtr[2] = (dataPtr + nChannel - widthstep)[2];
                                break;
                            }
                        case 3:
                            {
                                dataPtr[0] = (dataPtr - nChannel)[0];
                                dataPtr[1] = (dataPtr - nChannel)[1];
                                dataPtr[2] = (dataPtr - nChannel)[2];
                                break;
                            }
                        case 5:
                            {
                                dataPtr[0] = (dataPtr + nChannel)[0];
                                dataPtr[1] = (dataPtr + nChannel)[1];
                                dataPtr[2] = (dataPtr + nChannel)[2];
                                break;
                            }
                        case 6:
                            {
                                dataPtr[0] = (dataPtr - nChannel + widthstep)[0];
                                dataPtr[1] = (dataPtr - nChannel + widthstep)[1];
                                dataPtr[2] = (dataPtr - nChannel + widthstep)[2];
                                break;
                            }
                        case 7:
                            {
                                dataPtr[0] = (dataPtr + widthstep)[0];
                                dataPtr[1] = (dataPtr + widthstep)[1];
                                dataPtr[2] = (dataPtr + widthstep)[2];
                                break;
                            }
                        case 8:
                            {
                                dataPtr[0] = (dataPtr + nChannel + widthstep)[0];
                                dataPtr[1] = (dataPtr + nChannel + widthstep)[1];
                                dataPtr[2] = (dataPtr + nChannel + widthstep)[2];
                                break;
                            }
                        default:
                            break;
                    }

                    dataPtr -= nChannel;
                    dataPtr_d -= nChannel;
                }

                //canto inferior esquerdo
                min = 0;
                pixel = 4;

                // 1º PIXEL //
                blue = (dataPtr - widthstep)[0];
                green = (dataPtr - widthstep)[1];
                red = (dataPtr - widthstep)[2];

                d1 = 0;

                blue_2 = (dataPtr + nChannel - widthstep)[0];
                green_2 = (dataPtr + nChannel - widthstep)[1];
                red_2 = (dataPtr + nChannel - widthstep)[2];

                d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = dataPtr[0];
                green_2 = dataPtr[1];
                red_2 = dataPtr[2];

                d3 = d4 = d6 = d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = (dataPtr + nChannel)[0];
                green_2 = (dataPtr + nChannel)[1];
                red_2 = (dataPtr + nChannel)[2];

                d5 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                distancias[0] = distancias[1] = dTotal;

                // 2º PIXEL //
                blue = (dataPtr + nChannel - widthstep)[0];
                green = (dataPtr + nChannel - widthstep)[1];
                red = (dataPtr + nChannel - widthstep)[2];

                blue_2 = (dataPtr - widthstep)[0];
                green_2 = (dataPtr - widthstep)[1];
                red_2 = (dataPtr - widthstep)[2];

                d1 = d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = dataPtr[0];
                green_2 = dataPtr[1];
                red_2 = dataPtr[2];

                d3 = d4 = d6 = d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = (dataPtr + nChannel)[0];
                green_2 = (dataPtr + nChannel)[1];
                red_2 = (dataPtr + nChannel)[2];

                d5 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                distancias[1] = distancias[2] = dTotal;

                // 4º PIXEL //
                blue = dataPtr[0];
                green = dataPtr[1];
                red = dataPtr[2];

                d4 = d6 = d7 = 0;

                blue_2 = (dataPtr - widthstep)[0];
                green_2 = (dataPtr - widthstep)[1];
                red_2 = (dataPtr - widthstep)[2];

                d1 = d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = (dataPtr + nChannel - widthstep)[0];
                green_2 = (dataPtr + nChannel - widthstep)[1];
                red_2 = (dataPtr + nChannel - widthstep)[2];

                d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = (dataPtr + nChannel)[0];
                green_2 = (dataPtr + nChannel)[1];
                red_2 = (dataPtr + nChannel)[2];

                d5 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                distancias[3] = distancias[4] = distancias[6] = distancias[7] = dTotal;

                // 5º PIXEL //
                blue = (dataPtr + nChannel)[0];
                green = (dataPtr + nChannel)[1];
                red = (dataPtr + nChannel)[2];

                d8 = 0;

                blue_2 = (dataPtr - widthstep)[0];
                green_2 = (dataPtr - widthstep)[1];
                red_2 = (dataPtr - widthstep)[2];

                d1 = d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = (dataPtr + nChannel - widthstep)[0];
                green_2 = (dataPtr + nChannel - widthstep)[1];
                red_2 = (dataPtr + nChannel - widthstep)[2];

                d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                blue_2 = dataPtr[0];
                green_2 = dataPtr[1];
                red_2 = dataPtr[2];

                d4 = d5 = d7 = d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                distancias[4] = distancias[5] = distancias[7] = distancias[8] = dTotal;

                for (int i = 0; i < 9; i++)
                {
                    if (min == 0)
                    {
                        min = distancias[i];
                        pixel = i;
                    }
                    else if (distancias[i] < min)
                    {
                        min = distancias[i];
                        pixel = i;
                    }
                }

                switch (pixel)
                {
                    case 0:
                        {
                            dataPtr[0] = (dataPtr - nChannel - widthstep)[0];
                            dataPtr[1] = (dataPtr - nChannel - widthstep)[1];
                            dataPtr[2] = (dataPtr - nChannel - widthstep)[2];
                            break;
                        }
                    case 1:
                        {
                            dataPtr[0] = (dataPtr - widthstep)[0];
                            dataPtr[1] = (dataPtr - widthstep)[1];
                            dataPtr[2] = (dataPtr - widthstep)[3];
                            break;
                        }
                    case 2:
                        {
                            dataPtr[0] = (dataPtr + nChannel - widthstep)[0];
                            dataPtr[1] = (dataPtr + nChannel - widthstep)[1];
                            dataPtr[2] = (dataPtr + nChannel - widthstep)[2];
                            break;
                        }
                    case 3:
                        {
                            dataPtr[0] = (dataPtr - nChannel)[0];
                            dataPtr[1] = (dataPtr - nChannel)[1];
                            dataPtr[2] = (dataPtr - nChannel)[2];
                            break;
                        }
                    case 5:
                        {
                            dataPtr[0] = (dataPtr + nChannel)[0];
                            dataPtr[1] = (dataPtr + nChannel)[1];
                            dataPtr[2] = (dataPtr + nChannel)[2];
                            break;
                        }
                    case 6:
                        {
                            dataPtr[0] = (dataPtr - nChannel + widthstep)[0];
                            dataPtr[1] = (dataPtr - nChannel + widthstep)[1];
                            dataPtr[2] = (dataPtr - nChannel + widthstep)[2];
                            break;
                        }
                    case 7:
                        {
                            dataPtr[0] = (dataPtr + widthstep)[0];
                            dataPtr[1] = (dataPtr + widthstep)[1];
                            dataPtr[2] = (dataPtr + widthstep)[2];
                            break;
                        }
                    case 8:
                        {
                            dataPtr[0] = (dataPtr + nChannel + widthstep)[0];
                            dataPtr[1] = (dataPtr + nChannel + widthstep)[1];
                            dataPtr[2] = (dataPtr + nChannel + widthstep)[2];
                            break;
                        }
                    default:
                        break;
                }

                dataPtr -= widthstep;
                dataPtr_d -= widthstep;

                //margem esquerda
                for (int y_esq = 1; y_esq < height - 1; y_esq++)
                {
                    min = 0;
                    pixel = 4;

                    // 2º PIXEL //

                    blue = (dataPtr - widthstep)[0];
                    green = (dataPtr - widthstep)[1];
                    red = (dataPtr - widthstep)[2];

                    d1 = 0;

                    blue_2 = (dataPtr + nChannel - widthstep)[0];
                    green_2 = (dataPtr + nChannel - widthstep)[1];
                    red_2 = (dataPtr + nChannel - widthstep)[2];

                    d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d3 = d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel)[0];
                    green_2 = (dataPtr + nChannel)[1];
                    red_2 = (dataPtr + nChannel)[2];

                    d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d6 = d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep + nChannel)[0];
                    green_2 = (dataPtr + widthstep + nChannel)[1];
                    red_2 = (dataPtr + widthstep + nChannel)[2];

                    d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[0] = distancias[1] = dTotal;

                    // 3º PIXEL //
                    blue = (dataPtr + nChannel - widthstep)[0];
                    green = (dataPtr + nChannel - widthstep)[1];
                    red = (dataPtr + nChannel - widthstep)[2];

                    blue_2 = (dataPtr - widthstep)[0];
                    green_2 = (dataPtr - widthstep)[1];
                    red_2 = (dataPtr - widthstep)[2];

                    d1 = d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d3 = d4 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel)[0];
                    green_2 = (dataPtr + nChannel)[1];
                    red_2 = (dataPtr + nChannel)[2];

                    d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d6 = d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep + nChannel)[0];
                    green_2 = (dataPtr + widthstep + nChannel)[1];
                    red_2 = (dataPtr + widthstep + nChannel)[2];

                    d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[2] = dTotal;

                    // 5º PIXEL //
                    blue = dataPtr[0];
                    green = dataPtr[1];
                    red = dataPtr[2];

                    d4 = 0;

                    blue_2 = (dataPtr - widthstep)[0];
                    green_2 = (dataPtr - widthstep)[1];
                    red_2 = (dataPtr - widthstep)[2];

                    d1 = d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel - widthstep)[0];
                    green_2 = (dataPtr + nChannel - widthstep)[1];
                    red_2 = (dataPtr + nChannel - widthstep)[2];

                    d3 =  Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel)[0];
                    green_2 = (dataPtr + nChannel)[1];
                    red_2 = (dataPtr + nChannel)[2];

                    d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d6 = d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep + nChannel)[0];
                    green_2 = (dataPtr + widthstep + nChannel)[1];
                    red_2 = (dataPtr + widthstep + nChannel)[2];

                    d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[3] = distancias[4] = dTotal;

                    // 6º PIXEL //
                    blue = (dataPtr + nChannel)[0];
                    green = (dataPtr + nChannel)[1];
                    red = (dataPtr + nChannel)[2];

                    blue_2 = (dataPtr - widthstep)[0];
                    green_2 = (dataPtr - widthstep)[1];
                    red_2 = (dataPtr - widthstep)[2];

                    d1 = d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel - widthstep)[0];
                    green_2 = (dataPtr + nChannel - widthstep)[1];
                    red_2 = (dataPtr + nChannel - widthstep)[2];

                    d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d4 = d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d6 = d7 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep + nChannel)[0];
                    green_2 = (dataPtr + widthstep + nChannel)[1];
                    red_2 = (dataPtr + widthstep + nChannel)[2];

                    d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[5] = dTotal;

                    // 8º PIXEL //
                    blue = (dataPtr + widthstep)[0];
                    green = (dataPtr + widthstep)[1];
                    red = (dataPtr + widthstep)[2];

                    d7 = 0;

                    blue_2 = (dataPtr - widthstep)[0];
                    green_2 = (dataPtr - widthstep)[1];
                    red_2 = (dataPtr - widthstep)[2];

                    d1 = d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel - widthstep)[0];
                    green_2 = (dataPtr + nChannel - widthstep)[1];
                    red_2 = (dataPtr + nChannel - widthstep)[2];

                    d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d4 = d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel)[0];
                    green_2 = (dataPtr + nChannel)[1];
                    red_2 = (dataPtr + nChannel)[2];

                    d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep + nChannel)[0];
                    green_2 = (dataPtr + widthstep + nChannel)[1];
                    red_2 = (dataPtr + widthstep + nChannel)[2];

                    d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[6] = distancias[7] = dTotal;

                    // 9º PIXEL //
                    blue = (dataPtr + nChannel + widthstep)[0];
                    green = (dataPtr + nChannel + widthstep)[1];
                    red = (dataPtr + nChannel + widthstep)[2];

                    blue_2 = (dataPtr - widthstep)[0];
                    green_2 = (dataPtr - widthstep)[1];
                    red_2 = (dataPtr - widthstep)[2];

                    d1 = d2 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel - widthstep)[0];
                    green_2 = (dataPtr + nChannel - widthstep)[1];
                    red_2 = (dataPtr + nChannel - widthstep)[2];

                    d3 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = dataPtr[0];
                    green_2 = dataPtr[1];
                    red_2 = dataPtr[2];

                    d4 = d5 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + nChannel)[0];
                    green_2 = (dataPtr + nChannel)[1];
                    red_2 = (dataPtr + nChannel)[2];

                    d6 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    blue_2 = (dataPtr + widthstep)[0];
                    green_2 = (dataPtr + widthstep)[1];
                    red_2 = (dataPtr + widthstep)[2];

                    d7 = d8 = Math.Abs(blue - blue_2) + Math.Abs(green - green_2) + Math.Abs(red - red_2);

                    dTotal = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

                    distancias[8] = dTotal;

                    for (int i = 0; i < 9; i++)
                    {
                        if (min == 0)
                        {
                            min = distancias[i];
                            pixel = i;
                        }
                        else if (distancias[i] < min)
                        {
                            min = distancias[i];
                            pixel = i;
                        }
                    }

                    switch (pixel)
                    {
                        case 0:
                            {
                                dataPtr[0] = (dataPtr - nChannel - widthstep)[0];
                                dataPtr[1] = (dataPtr - nChannel - widthstep)[1];
                                dataPtr[2] = (dataPtr - nChannel - widthstep)[2];
                                break;
                            }
                        case 1:
                            {
                                dataPtr[0] = (dataPtr - widthstep)[0];
                                dataPtr[1] = (dataPtr - widthstep)[1];
                                dataPtr[2] = (dataPtr - widthstep)[3];
                                break;
                            }
                        case 2:
                            {
                                dataPtr[0] = (dataPtr + nChannel - widthstep)[0];
                                dataPtr[1] = (dataPtr + nChannel - widthstep)[1];
                                dataPtr[2] = (dataPtr + nChannel - widthstep)[2];
                                break;
                            }
                        case 3:
                            {
                                dataPtr[0] = (dataPtr - nChannel)[0];
                                dataPtr[1] = (dataPtr - nChannel)[1];
                                dataPtr[2] = (dataPtr - nChannel)[2];
                                break;
                            }
                        case 5:
                            {
                                dataPtr[0] = (dataPtr + nChannel)[0];
                                dataPtr[1] = (dataPtr + nChannel)[1];
                                dataPtr[2] = (dataPtr + nChannel)[2];
                                break;
                            }
                        case 6:
                            {
                                dataPtr[0] = (dataPtr - nChannel + widthstep)[0];
                                dataPtr[1] = (dataPtr - nChannel + widthstep)[1];
                                dataPtr[2] = (dataPtr - nChannel + widthstep)[2];
                                break;
                            }
                        case 7:
                            {
                                dataPtr[0] = (dataPtr + widthstep)[0];
                                dataPtr[1] = (dataPtr + widthstep)[1];
                                dataPtr[2] = (dataPtr + widthstep)[2];
                                break;
                            }
                        case 8:
                            {
                                dataPtr[0] = (dataPtr + nChannel + widthstep)[0];
                                dataPtr[1] = (dataPtr + nChannel + widthstep)[1];
                                dataPtr[2] = (dataPtr + nChannel + widthstep)[2];
                                break;
                            }
                        default:
                            break;
                    }

                    dataPtr -= widthstep;
                    dataPtr_d -= widthstep;
                }
            }
        }

        public static int[] Histogram_Gray(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                int[] histogram_gray = new int[256];

                MIplImage m = img.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;

                int x, y;
                int grey;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        grey = (int)Math.Round((dataPtr[0] + dataPtr[1] + dataPtr[2]) / 3.0);

                        histogram_gray[grey] += 1;

                        dataPtr += nChan;
                    }
                    dataPtr += padding;
                }

                HistogramaForm h = new HistogramaForm(histogram_gray);

                h.ShowDialog();

                return histogram_gray;
            }
        }

        public static int[,] Histogram_RGB(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                int[,] histogram_rgb = new int[3, 256];

                MIplImage m = img.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;

                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        histogram_rgb[0, dataPtr[0]] += 1;
                        histogram_rgb[1, dataPtr[1]] += 1;
                        histogram_rgb[2, dataPtr[2]] += 1;

                        dataPtr += nChannel;
                    }
                    dataPtr += padding;
                }

                HistogramaForm h = new HistogramaForm(histogram_rgb);

                h.ShowDialog();

                return histogram_rgb;
            }
        }

        public static int[,] Histogram_All(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                int[,] histogram_rgb_all = new int[4, 256];

                MIplImage m = img.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;

                int x, y;
                int grey;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        grey = (int)Math.Round((dataPtr[0] + dataPtr[1] + dataPtr[2]) / 3.0);

                        histogram_rgb_all[0, grey] += 1;
                        histogram_rgb_all[1, dataPtr[0]] += 1;
                        histogram_rgb_all[2, dataPtr[1]] += 1;
                        histogram_rgb_all[3, dataPtr[2]] += 1;


                        dataPtr += nChannel;
                    }
                    dataPtr += padding;
                }

                HistogramaForm h = new HistogramaForm(histogram_rgb_all, 0);

                h.ShowDialog();

                return histogram_rgb_all;
            }
        }

        public static void ConvertToBW(Emgu.CV.Image<Bgr, byte> img, int threshold)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int widthstep = m.widthStep;
                int padding = m.widthStep - m.nChannels * m.width;

                int grey;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {

                        grey = (int)Math.Round((dataPtr[0] + dataPtr[1] + dataPtr[2]) / 3.0);

                        if (grey <= threshold)
                        {
                            dataPtr[0] = 0;
                            dataPtr[1] = 0;
                            dataPtr[2] = 0;
                        }
                        else
                        {
                            dataPtr[0] = 255;
                            dataPtr[1] = 255;
                            dataPtr[2] = 255;
                        }

                        dataPtr += nChannel;
                    }
                    dataPtr += padding;
                }
            }
        }

        public static int get_threshold(int[] hist)
        {
            unsafe
            {
                int q1, q2, u1, u2, threshold;
                double max, var;
                int resolucao = 0;

                for (int i = 0; i < hist.Length; i++)
                {
                    resolucao += hist[i];
                }

                q1 = hist[0];
                q2 = resolucao - q1;

                u1 = hist[0];
                u2 = 0;

                for (int i = 1; i < hist.Length; i++)
                {
                    u2 = u2 + i * hist[i];
                }

                threshold = 0;
                if (u1 > 0 && q1 > 0 && q2 > 0)
                    max = (double)q1 / resolucao * (double)q2 / resolucao * Math.Pow((double)u1 / q1 - (double)u2 / q2, 2);
                else
                    max = 0;

                for (int i = 1; i < hist.Length; i++)
                {
                    q1 = q1 + hist[i];
                    if (q1 == 0)
                        continue;

                    q2 = q2 - hist[i];
                    if (q2 == 0)
                        break;

                    u1 = u1 + i * hist[i];
                    u2 = u2 - i * hist[i];

                    var = (double)q1 / resolucao * (double)q2 / resolucao * Math.Pow((double)u1 / q1 - (double)u2 / q2, 2);

                    if (var > max)
                    {
                        threshold = i;
                        max = var;
                    }
                }
                return threshold;
            }
        }

        public static void ConvertToBW_Otsu(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                int[] hist = Histogram_Gray(img);
                int threshold = get_threshold(hist);
                ConvertToBW(img, threshold);
            }
        }

        public static void Mean_solutionB(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = imgCopy.MIplImage;
                MIplImage m_d = img.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer();
                byte* dataPtr_d = (byte*)m_d.imageData.ToPointer();

                int widht = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int widthstep = m.widthStep;
                int padding = m_d.widthStep - m_d.nChannels * m_d.width;

                int sumBlue, sumRed, sumGreen;
                int prevSumBlue, prevSumGreen, prevSumRed;

                int y, x;
                int indexcore = height - 2;

                int[] coreBlue = new int[indexcore];
                int[] coreGreen = new int[indexcore];
                int[] coreRed = new int[indexcore];

                sumBlue = (int)(dataPtr[0] * 4 + (dataPtr + nChannel)[0] * 2 + (dataPtr + widthstep)[0] * 2 + (dataPtr + widthstep + nChannel)[0]);
                sumGreen = (int)(dataPtr[1] * 4 + (dataPtr + nChannel)[1] * 2 + (dataPtr + widthstep)[1] * 2 + (dataPtr + widthstep + nChannel)[1]);
                sumRed = (int)(dataPtr[2] * 4 + (dataPtr + nChannel)[2] * 2 + (dataPtr + widthstep)[2] * 2 + (dataPtr + widthstep + nChannel)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 9.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 9.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 9.0);

                prevSumBlue = sumBlue;
                prevSumGreen = sumGreen;
                prevSumRed = sumRed;

                dataPtr += nChannel;
                dataPtr_d += nChannel;

                sumBlue = (int)prevSumBlue - ((dataPtr - nChannel)[0] * 2 + (dataPtr - nChannel + widthstep)[0]) + ((dataPtr + nChannel)[0] * 2 + (dataPtr + nChannel + widthstep)[0]);
                sumGreen = (int)prevSumGreen - ((dataPtr - nChannel)[1] * 2 + (dataPtr - nChannel + widthstep)[1]) + ((dataPtr + nChannel)[1] * 2 + (dataPtr + nChannel + widthstep)[1]);
                sumRed = (int)prevSumRed - ((dataPtr - nChannel)[2] * 2 + (dataPtr - nChannel + widthstep)[2]) + ((dataPtr + nChannel)[2] * 2 + (dataPtr + nChannel + widthstep)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 9.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 9.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 9.0);

                prevSumBlue = sumBlue;
                prevSumGreen = sumGreen;
                prevSumRed = sumRed;

                dataPtr += nChannel;
                dataPtr_d += nChannel;

                for (int x_ms = 2; x_ms < widht - 1; x_ms++)
                {
                    sumBlue = (int)prevSumBlue - ((dataPtr - 2 * nChannel)[0] * 2 + (dataPtr - 2 * nChannel + widthstep)[0]) + ((dataPtr + nChannel)[0] * 2 + (dataPtr + nChannel + widthstep)[0]);
                    sumGreen = (int)prevSumGreen - ((dataPtr - 2 * nChannel)[1] * 2 + (dataPtr - 2 * nChannel + widthstep)[1]) + ((dataPtr + nChannel)[1] * 2 + (dataPtr + nChannel + widthstep)[1]);
                    sumRed = (int)prevSumRed - ((dataPtr - 2 * nChannel)[2] * 2 + (dataPtr - 2 * nChannel + widthstep)[2]) + ((dataPtr + nChannel)[2] * 2 + (dataPtr + nChannel + widthstep)[2]);

                    dataPtr_d[0] = (byte)Math.Round(sumBlue / 9.0);
                    dataPtr_d[1] = (byte)Math.Round(sumGreen / 9.0);
                    dataPtr_d[2] = (byte)Math.Round(sumRed / 9.0);

                    prevSumBlue = sumBlue;
                    prevSumGreen = sumGreen;
                    prevSumRed = sumRed;

                    dataPtr += nChannel;
                    dataPtr_d += nChannel;
                }

                sumBlue = (int)prevSumBlue - ((dataPtr - 2 * nChannel)[0] * 2 + (dataPtr - 2 * nChannel + widthstep)[0]) + ((dataPtr)[0] * 2 + (dataPtr + widthstep)[0]);
                sumGreen = (int)prevSumGreen - ((dataPtr - 2 * nChannel)[1] * 2 + (dataPtr - 2 * nChannel + widthstep)[1]) + ((dataPtr)[1] * 2 + (dataPtr + widthstep)[1]);
                sumRed = (int)prevSumRed - ((dataPtr - 2 * nChannel)[2] * 2 + (dataPtr - 2 * nChannel + widthstep)[2]) + ((dataPtr)[2] * 2 + (dataPtr + widthstep)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 9.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 9.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 9.0);

                prevSumBlue = sumBlue;
                prevSumGreen = sumGreen;
                prevSumRed = sumRed;

                dataPtr += widthstep;
                dataPtr_d += widthstep;

                sumBlue = (int)prevSumBlue - ((dataPtr - widthstep)[0] * 2 + (dataPtr - widthstep - nChannel)[0]) + ((dataPtr + widthstep)[0] * 2 + (dataPtr + widthstep - nChannel)[0]);
                sumGreen = (int)prevSumGreen - ((dataPtr - widthstep)[1] * 2 + (dataPtr - widthstep - nChannel)[1]) + ((dataPtr + widthstep)[1] * 2 + (dataPtr + widthstep - nChannel)[1]);
                sumRed = (int)prevSumRed - ((dataPtr - widthstep)[2] * 2 + (dataPtr - widthstep - nChannel)[2]) + ((dataPtr + widthstep)[2] * 2 + (dataPtr + widthstep - nChannel)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 9.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 9.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 9.0);

                prevSumBlue = sumBlue;
                prevSumGreen = sumGreen;
                prevSumRed = sumRed;

                dataPtr += widthstep;
                dataPtr_d += widthstep;

                for (int y_md = 2; y_md < height - 1; y_md++)
                {
                    sumBlue = (int)(prevSumBlue - ((dataPtr - 2 * widthstep)[0] * 2 + (dataPtr - 2 * widthstep - nChannel)[0]) + ((dataPtr + widthstep)[0] * 2 + (dataPtr + widthstep - nChannel)[0]));
                    sumGreen = (int)prevSumGreen - ((dataPtr - 2 * widthstep)[1] * 2 + (dataPtr - 2 * widthstep - nChannel)[1]) + ((dataPtr + widthstep)[1] * 2 + (dataPtr + widthstep - nChannel)[1]);
                    sumRed = (int)prevSumRed - ((dataPtr - 2 * widthstep)[2] * 2 + (dataPtr - 2 * widthstep - nChannel)[2]) + ((dataPtr + widthstep)[2] * 2 + (dataPtr + widthstep - nChannel)[2]);

                    dataPtr_d[0] = (byte)Math.Round(sumBlue / 9.0);
                    dataPtr_d[1] = (byte)Math.Round(sumGreen / 9.0);
                    dataPtr_d[2] = (byte)Math.Round(sumRed / 9.0);

                    prevSumBlue = sumBlue;
                    prevSumGreen = sumGreen;
                    prevSumRed = sumRed;

                    dataPtr += widthstep;
                    dataPtr_d += widthstep;
                }

                sumBlue = (int)prevSumBlue - ((dataPtr - 2 * widthstep)[0] * 2 + (dataPtr - 2 * widthstep - nChannel)[0]) + ((dataPtr)[0] * 2 + (dataPtr - nChannel)[0]);
                sumGreen = (int)prevSumGreen - ((dataPtr - 2 * widthstep)[1] * 2 + (dataPtr - 2 * widthstep - nChannel)[1]) + ((dataPtr)[1] * 2 + (dataPtr - nChannel)[1]);
                sumRed = (int)prevSumRed - ((dataPtr - 2 * widthstep)[2] * 2 + (dataPtr - 2 * widthstep - nChannel)[2]) + ((dataPtr)[2] * 2 + (dataPtr - nChannel)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 9.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 9.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 9.0);

                prevSumBlue = sumBlue;
                prevSumGreen = sumGreen;
                prevSumRed = sumRed;

                dataPtr -= nChannel;
                dataPtr_d -= nChannel;

                sumBlue = (int)prevSumBlue - ((dataPtr + nChannel)[0] * 2 + (dataPtr + nChannel - widthstep)[0]) + ((dataPtr - nChannel)[0] * 2 + (dataPtr - widthstep - nChannel)[0]);
                sumGreen = (int)prevSumGreen - ((dataPtr + nChannel)[1] * 2 + (dataPtr + nChannel - widthstep)[1]) + ((dataPtr - nChannel)[1] * 2 + (dataPtr - widthstep - nChannel)[1]);
                sumRed = (int)prevSumRed - ((dataPtr + nChannel)[2] * 2 + (dataPtr + nChannel - widthstep)[2]) + ((dataPtr - nChannel)[2] * 2 + (dataPtr - widthstep - nChannel)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 9.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 9.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 9.0);

                prevSumBlue = sumBlue;
                prevSumGreen = sumGreen;
                prevSumRed = sumRed;

                dataPtr -= nChannel;
                dataPtr_d -= nChannel;

                for (int x_mi = 2; x_mi < widht - 1; x_mi++)
                {
                    sumBlue = (int)prevSumBlue - ((dataPtr + 2 * nChannel)[0] * 2 + (dataPtr + 2 * nChannel - widthstep)[0]) + ((dataPtr - nChannel)[0] * 2 + (dataPtr - widthstep - nChannel)[0]);
                    sumGreen = (int)prevSumGreen - ((dataPtr + 2 * nChannel)[1] * 2 + (dataPtr + 2 * nChannel - widthstep)[1]) + ((dataPtr - nChannel)[1] * 2 + (dataPtr - widthstep - nChannel)[1]);
                    sumRed = (int)prevSumRed - ((dataPtr + 2 * nChannel)[2] * 2 + (dataPtr + 2 * nChannel - widthstep)[2]) + ((dataPtr - nChannel)[2] * 2 + (dataPtr - widthstep - nChannel)[2]);

                    dataPtr_d[0] = (byte)Math.Round(sumBlue / 9.0);
                    dataPtr_d[1] = (byte)Math.Round(sumGreen / 9.0);
                    dataPtr_d[2] = (byte)Math.Round(sumRed / 9.0);

                    prevSumBlue = sumBlue;
                    prevSumGreen = sumGreen;
                    prevSumRed = sumRed;

                    dataPtr -= nChannel;
                    dataPtr_d -= nChannel;
                }

                sumBlue = (int)prevSumBlue - ((dataPtr + 2 * nChannel)[0] * 2 + (dataPtr + 2 * nChannel - widthstep)[0]) + ((dataPtr)[0] * 2 + (dataPtr - widthstep)[0]);
                sumGreen = (int)prevSumGreen - ((dataPtr + 2 * nChannel)[1] * 2 + (dataPtr + 2 * nChannel - widthstep)[1]) + ((dataPtr)[1] * 2 + (dataPtr - widthstep)[1]);
                sumRed = (int)prevSumRed - ((dataPtr + 2 * nChannel)[2] * 2 + (dataPtr + 2 * nChannel - widthstep)[2]) + ((dataPtr)[2] * 2 + (dataPtr - widthstep)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 9.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 9.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 9.0);

                prevSumBlue = sumBlue;
                prevSumGreen = sumGreen;
                prevSumRed = sumRed;

                dataPtr -= widthstep;
                dataPtr_d -= widthstep;

                sumBlue = (int)prevSumBlue - ((dataPtr + widthstep)[0] * 2 + (dataPtr + nChannel + widthstep)[0]) + ((dataPtr - widthstep)[0] * 2 + (dataPtr - widthstep + nChannel)[0]);
                sumGreen = (int)prevSumGreen - ((dataPtr + widthstep)[1] * 2 + (dataPtr + nChannel + widthstep)[1]) + ((dataPtr - widthstep)[1] * 2 + (dataPtr - widthstep + nChannel)[1]);
                sumRed = (int)prevSumRed - ((dataPtr + widthstep)[2] * 2 + (dataPtr + nChannel + widthstep)[2]) + ((dataPtr - widthstep)[2] * 2 + (dataPtr - widthstep + nChannel)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 9.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 9.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 9.0);

                prevSumBlue = sumBlue;
                prevSumGreen = sumGreen;
                prevSumRed = sumRed;

                coreBlue[indexcore - 1] = sumBlue;
                coreGreen[indexcore - 1] = sumGreen;
                coreRed[indexcore - 1] = sumRed;
                indexcore--;

                dataPtr -= widthstep;
                dataPtr_d -= widthstep;

                for (int y_me = 2; y_me < height - 1; y_me++)
                {
                    sumBlue = (int)prevSumBlue - ((dataPtr + 2 * widthstep)[0] * 2 + (dataPtr + 2 * widthstep + nChannel)[0]) + ((dataPtr - widthstep)[0] * 2 + (dataPtr - widthstep + nChannel)[0]);
                    sumGreen = (int)prevSumGreen - ((dataPtr + 2 * widthstep)[1] * 2 + (dataPtr + 2 * widthstep + nChannel)[1]) + ((dataPtr - widthstep)[1] * 2 + (dataPtr - widthstep + nChannel)[1]);
                    sumRed = (int)prevSumRed - ((dataPtr + 2 * widthstep)[2] * 2 + (dataPtr + 2 * widthstep + nChannel)[2]) + ((dataPtr - widthstep)[2] * 2 + (dataPtr - widthstep + nChannel)[2]);

                    dataPtr_d[0] = (byte)Math.Round(sumBlue / 9.0);
                    dataPtr_d[1] = (byte)Math.Round(sumGreen / 9.0);
                    dataPtr_d[2] = (byte)Math.Round(sumRed / 9.0);

                    prevSumBlue = sumBlue;
                    prevSumGreen = sumGreen;
                    prevSumRed = sumRed;

                    coreBlue[indexcore - 1] = sumBlue;
                    coreGreen[indexcore - 1] = sumGreen;
                    coreRed[indexcore - 1] = sumRed;
                    indexcore--;

                    dataPtr -= widthstep;
                    dataPtr_d -= widthstep;
                }

                dataPtr = (byte*)m.imageData.ToPointer();
                dataPtr_d = (byte*)m_d.imageData.ToPointer();

                dataPtr += widthstep + nChannel;
                dataPtr_d += widthstep + nChannel;

                indexcore = 0;

                for (y = 1; y < height - 1; y++)
                {
                    prevSumBlue = coreBlue[indexcore];
                    prevSumGreen = coreGreen[indexcore];
                    prevSumRed = coreRed[indexcore];

                    sumBlue = (int)prevSumBlue - ((dataPtr - nChannel)[0] + (dataPtr - nChannel - widthstep)[0] + (dataPtr + widthstep - nChannel)[0]) + ((dataPtr + nChannel)[0] + (dataPtr + nChannel - widthstep)[0] + (dataPtr + widthstep + nChannel)[0]);
                    sumGreen = (int)prevSumGreen - ((dataPtr - nChannel)[1] + (dataPtr - nChannel - widthstep)[1] + (dataPtr + widthstep - nChannel)[1]) + ((dataPtr + nChannel)[1] + (dataPtr + nChannel - widthstep)[1] + (dataPtr + widthstep + nChannel)[1]);
                    sumRed = (int)prevSumRed - ((dataPtr - nChannel)[2] + (dataPtr - nChannel - widthstep)[2] + (dataPtr + widthstep - nChannel)[2]) + ((dataPtr + nChannel)[2] + (dataPtr + nChannel - widthstep)[2] + (dataPtr + widthstep + nChannel)[2]);

                    dataPtr_d[0] = (byte)Math.Round(sumBlue / 9.0);
                    dataPtr_d[1] = (byte)Math.Round(sumGreen / 9.0);
                    dataPtr_d[2] = (byte)Math.Round(sumRed / 9.0);

                    coreBlue[indexcore] = sumBlue;
                    coreGreen[indexcore] = sumGreen;
                    coreRed[indexcore] = sumRed;

                    indexcore++;

                    dataPtr += widthstep;
                    dataPtr_d += widthstep;
                }

                dataPtr = (byte*)m.imageData.ToPointer();
                dataPtr_d = (byte*)m_d.imageData.ToPointer();

                dataPtr += widthstep + 2 * nChannel;
                dataPtr_d += widthstep + 2 * nChannel;

                indexcore = 0;

                for (y = 1; y < height - 1; y++)
                {
                    prevSumBlue = coreBlue[indexcore];
                    prevSumGreen = coreGreen[indexcore];
                    prevSumRed = coreRed[indexcore];

                    for (x = 2; x < widht - 1; x++)
                    {
                        sumBlue = (int)prevSumBlue - ((dataPtr - 2 * nChannel)[0] + (dataPtr - 2 * nChannel - widthstep)[0] + (dataPtr + widthstep - 2 * nChannel)[0]) + ((dataPtr + nChannel)[0] + (dataPtr + nChannel - widthstep)[0] + (dataPtr + widthstep + nChannel)[0]);
                        sumGreen = (int)prevSumGreen - ((dataPtr - 2 * nChannel)[1] + (dataPtr - 2 * nChannel - widthstep)[1] + (dataPtr + widthstep - 2 * nChannel)[1]) + ((dataPtr + nChannel)[1] + (dataPtr + nChannel - widthstep)[1] + (dataPtr + widthstep + nChannel)[1]);
                        sumRed = (int)prevSumRed - ((dataPtr - 2 * nChannel)[2] + (dataPtr - 2 * nChannel - widthstep)[2] + (dataPtr + widthstep - 2 * nChannel)[2]) + ((dataPtr + nChannel)[2] + (dataPtr + nChannel - widthstep)[2] + (dataPtr + widthstep + nChannel)[2]);

                        prevSumBlue = sumBlue;
                        prevSumGreen = sumGreen;
                        prevSumRed = sumRed;

                        dataPtr_d[0] = (byte)Math.Round(sumBlue / 9.0);
                        dataPtr_d[1] = (byte)Math.Round(sumGreen / 9.0);
                        dataPtr_d[2] = (byte)Math.Round(sumRed / 9.0);

                        dataPtr += nChannel;
                        dataPtr_d += nChannel;
                    }
                    indexcore++;

                    dataPtr += padding + 3 * nChannel;
                    dataPtr_d += padding + 3 * nChannel;
                }
            }
        }

        public static void Mean_solutionC(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int size)
        {
            unsafe
            {
                MIplImage m = imgCopy.MIplImage;
                MIplImage m_d = img.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer();
                byte* dataPtr_d = (byte*)m_d.imageData.ToPointer();

                int widht = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int widthstep = m.widthStep;
                int padding = m_d.widthStep - m_d.nChannels * m_d.width;

                int sumBlue, sumRed, sumGreen;

                int[,] img_blue = new int[widht, height];
                int[,] img_green = new int[widht, height];
                int[,] img_red = new int[widht, height];

                int index_widht = 0;
                int index_height = 0;

                sumBlue = (int)(dataPtr[0] * 16 + (dataPtr + nChannel)[0] * 4 + (dataPtr + 2 * nChannel)[0] * 4 + (dataPtr + 3 * nChannel)[0] * 4
                          + (dataPtr + widthstep)[0] * 4 + (dataPtr + widthstep + nChannel)[0] + (dataPtr + widthstep + 2 * nChannel)[0] + (dataPtr + widthstep + 3 * nChannel)[0]
                          + (dataPtr + 2 * widthstep)[0] * 4 + (dataPtr + 2 * widthstep + nChannel)[0] + (dataPtr + 2 * widthstep + 2 * nChannel)[0] + (dataPtr + 2 * widthstep + 3 * nChannel)[0]
                          + (dataPtr + 3 * widthstep)[0] * 4 + (dataPtr + 3 * widthstep + nChannel)[0] + (dataPtr + 3 * widthstep + 2 * nChannel)[0] + (dataPtr + 3 * widthstep + 3 * nChannel)[0]);

                sumGreen = (int)(dataPtr[1] * 16 + (dataPtr + nChannel)[1] * 4 + (dataPtr + 2 * nChannel)[1] * 4 + (dataPtr + 3 * nChannel)[1] * 4
                          + (dataPtr + widthstep)[1] * 4 + (dataPtr + widthstep + nChannel)[1] + (dataPtr + widthstep + 2 * nChannel)[1] + (dataPtr + widthstep + 3 * nChannel)[1]
                          + (dataPtr + 2 * widthstep)[1] * 4 + (dataPtr + 2 * widthstep + nChannel)[1] + (dataPtr + 2 * widthstep + 2 * nChannel)[1] + (dataPtr + 2 * widthstep + 3 * nChannel)[1]
                          + (dataPtr + 3 * widthstep)[1] * 4 + (dataPtr + 3 * widthstep + nChannel)[1] + (dataPtr + 3 * widthstep + 2 * nChannel)[1] + (dataPtr + 3 * widthstep + 3 * nChannel)[1]);

                sumRed = (int)(dataPtr[2] * 16 + (dataPtr + nChannel)[2] * 4 + (dataPtr + 2 * nChannel)[2] * 4 + (dataPtr + 3 * nChannel)[2] * 4
                          + (dataPtr + widthstep)[2] * 4 + (dataPtr + widthstep + nChannel)[2] + (dataPtr + widthstep + 2 * nChannel)[2] + (dataPtr + widthstep + 3 * nChannel)[2]
                          + (dataPtr + 2 * widthstep)[2] * 4 + (dataPtr + 2 * widthstep + nChannel)[2] + (dataPtr + 2 * widthstep + 2 * nChannel)[2] + (dataPtr + 2 * widthstep + 3 * nChannel)[2]
                          + (dataPtr + 3 * widthstep)[2] * 4 + (dataPtr + 3 * widthstep + nChannel)[2] + (dataPtr + 3 * widthstep + 2 * nChannel)[2] + (dataPtr + 3 * widthstep + 3 * nChannel)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                img_blue[index_widht, index_height] = sumBlue;
                img_green[index_widht, index_height] = sumGreen;
                img_red[index_widht, index_height] = sumRed;

                dataPtr += nChannel;
                dataPtr_d += nChannel;
                index_widht++;

                sumBlue = (int)img_blue[index_widht - 1, index_height] - ((dataPtr - nChannel)[0] * 4 + (dataPtr - nChannel + widthstep)[0] + (dataPtr - nChannel + 2 * widthstep)[0] + (dataPtr - nChannel + 3 * widthstep)[0]) + ((dataPtr + 3 * nChannel)[0] * 4 + (dataPtr + 3 * nChannel + widthstep)[0] + (dataPtr + 3 * nChannel + 2 * widthstep)[0] + (dataPtr + 3 * nChannel + 3 * widthstep)[0]);
                sumGreen = (int)img_green[index_widht - 1, index_height] - ((dataPtr - nChannel)[1] * 4 + (dataPtr - nChannel + widthstep)[1] + (dataPtr - nChannel + 2 * widthstep)[1] + (dataPtr - nChannel + 3 * widthstep)[1]) + ((dataPtr + 3 * nChannel)[1] * 4 + (dataPtr + 3 * nChannel + widthstep)[1] + (dataPtr + 3 * nChannel + 2 * widthstep)[1] + (dataPtr + 3 * nChannel + 3 * widthstep)[1]);
                sumRed = (int)img_red[index_widht - 1, index_height] - ((dataPtr - nChannel)[2] * 4 + (dataPtr - nChannel + widthstep)[2] + (dataPtr - nChannel + 2 * widthstep)[2] + (dataPtr - nChannel + 3 * widthstep)[2]) + ((dataPtr + 3 * nChannel)[2] * 4 + (dataPtr + 3 * nChannel + widthstep)[2] + (dataPtr + 3 * nChannel + 2 * widthstep)[2] + (dataPtr + 3 * nChannel + 3 * widthstep)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                img_blue[index_widht, index_height] = sumBlue;
                img_green[index_widht, index_height] = sumGreen;
                img_red[index_widht, index_height] = sumRed;

                dataPtr += nChannel;
                dataPtr_d += nChannel;
                index_widht++;

                sumBlue = (int)img_blue[index_widht - 1, index_height] - ((dataPtr - 2 * nChannel)[0] * 4 + (dataPtr - 2 * nChannel + widthstep)[0] + (dataPtr - 2 * nChannel + 2 * widthstep)[0] + (dataPtr - 2 * nChannel + 3 * widthstep)[0]) + ((dataPtr + 3 * nChannel)[0] * 4 + (dataPtr + 3 * nChannel + widthstep)[0] + (dataPtr + 3 * nChannel + 2 * widthstep)[0] + (dataPtr + 3 * nChannel + 3 * widthstep)[0]);
                sumGreen = (int)img_green[index_widht - 1, index_height] - ((dataPtr - 2 * nChannel)[1] * 4 + (dataPtr - 2 * nChannel + widthstep)[1] + (dataPtr - 2 * nChannel + 2 * widthstep)[1] + (dataPtr - 2 * nChannel + 3 * widthstep)[1]) + ((dataPtr + 3 * nChannel)[1] * 4 + (dataPtr + 3 * nChannel + widthstep)[1] + (dataPtr + 3 * nChannel + 2 * widthstep)[1] + (dataPtr + 3 * nChannel + 3 * widthstep)[1]);
                sumRed = (int)img_red[index_widht - 1, index_height] - ((dataPtr - 2 * nChannel)[2] * 4 + (dataPtr - 2 * nChannel + widthstep)[2] + (dataPtr - 2 * nChannel + 2 * widthstep)[2] + (dataPtr - 2 * nChannel + 3 * widthstep)[2]) + ((dataPtr + 3 * nChannel)[2] * 4 + (dataPtr + 3 * nChannel + widthstep)[2] + (dataPtr + 3 * nChannel + 2 * widthstep)[2] + (dataPtr + 3 * nChannel + 3 * widthstep)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                img_blue[index_widht, index_height] = sumBlue;
                img_green[index_widht, index_height] = sumGreen;
                img_red[index_widht, index_height] = sumRed;

                dataPtr += nChannel;
                dataPtr_d += nChannel;
                index_widht++;

                sumBlue = (int)img_blue[index_widht - 1, index_height] - ((dataPtr - 3 * nChannel)[0] * 4 + (dataPtr - 3 * nChannel + widthstep)[0] + (dataPtr - 3 * nChannel + 2 * widthstep)[0] + (dataPtr - 3 * nChannel + 3 * widthstep)[0]) + ((dataPtr + 3 * nChannel)[0] * 4 + (dataPtr + 3 * nChannel + widthstep)[0] + (dataPtr + 3 * nChannel + 2 * widthstep)[0] + (dataPtr + 3 * nChannel + 3 * widthstep)[0]);
                sumGreen = (int)img_green[index_widht - 1, index_height] - ((dataPtr - 3 * nChannel)[1] * 4 + (dataPtr - 3 * nChannel + widthstep)[1] + (dataPtr - 3 * nChannel + 2 * widthstep)[1] + (dataPtr - 3 * nChannel + 3 * widthstep)[1]) + ((dataPtr + 3 * nChannel)[1] * 4 + (dataPtr + 3 * nChannel + widthstep)[1] + (dataPtr + 3 * nChannel + 2 * widthstep)[1] + (dataPtr + 3 * nChannel + 3 * widthstep)[1]);
                sumRed = (int)img_red[index_widht - 1, index_height] - ((dataPtr - 3 * nChannel)[2] * 4 + (dataPtr - 3 * nChannel + widthstep)[2] + (dataPtr - 3 * nChannel + 2 * widthstep)[2] + (dataPtr - 3 * nChannel + 3 * widthstep)[2]) + ((dataPtr + 3 * nChannel)[2] * 4 + (dataPtr + 3 * nChannel + widthstep)[2] + (dataPtr + 3 * nChannel + 2 * widthstep)[2] + (dataPtr + 3 * nChannel + 3 * widthstep)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                img_blue[index_widht, index_height] = sumBlue;
                img_green[index_widht, index_height] = sumGreen;
                img_red[index_widht, index_height] = sumRed;

                dataPtr += nChannel;
                dataPtr_d += nChannel;
                index_widht++;

                for (int x_ms = 4; x_ms < widht - 3; x_ms++)
                {
                    sumBlue = (int)img_blue[index_widht - 1, index_height] - ((dataPtr - 4 * nChannel)[0] * 4 + (dataPtr - 4 * nChannel + widthstep)[0] + (dataPtr - 4 * nChannel + 2 * widthstep)[0] + (dataPtr - 4 * nChannel + 3 * widthstep)[0]) + ((dataPtr + 3 * nChannel)[0] * 4 + (dataPtr + 3 * nChannel + widthstep)[0] + (dataPtr + 3 * nChannel + 2 * widthstep)[0] + (dataPtr + 3 * nChannel + 3 * widthstep)[0]);
                    sumGreen = (int)img_green[index_widht - 1, index_height] - ((dataPtr - 4 * nChannel)[1] * 4 + (dataPtr - 4 * nChannel + widthstep)[1] + (dataPtr - 4 * nChannel + 2 * widthstep)[1] + (dataPtr - 4 * nChannel + 3 * widthstep)[1]) + ((dataPtr + 3 * nChannel)[1] * 4 + (dataPtr + 3 * nChannel + widthstep)[1] + (dataPtr + 3 * nChannel + 2 * widthstep)[1] + (dataPtr + 3 * nChannel + 3 * widthstep)[1]);
                    sumRed = (int)img_red[index_widht - 1, index_height] - ((dataPtr - 4 * nChannel)[2] * 4 + (dataPtr - 4 * nChannel + widthstep)[2] + (dataPtr - 4 * nChannel + 2 * widthstep)[2] + (dataPtr - 4 * nChannel + 3 * widthstep)[2]) + ((dataPtr + 3 * nChannel)[2] * 4 + (dataPtr + 3 * nChannel + widthstep)[2] + (dataPtr + 3 * nChannel + 2 * widthstep)[2] + (dataPtr + 3 * nChannel + 3 * widthstep)[2]);

                    dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                    dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                    dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                    img_blue[index_widht, index_height] = sumBlue;
                    img_green[index_widht, index_height] = sumGreen;
                    img_red[index_widht, index_height] = sumRed;

                    dataPtr += nChannel;
                    dataPtr_d += nChannel;
                    index_widht++;
                }

                sumBlue = (int)img_blue[index_widht - 1, index_height] - ((dataPtr - 4 * nChannel)[0] * 4 + (dataPtr - 4 * nChannel + widthstep)[0] + (dataPtr - 4 * nChannel + 2 * widthstep)[0] + (dataPtr - 4 * nChannel + 3 * widthstep)[0]) + ((dataPtr + 2 * nChannel)[0] * 4 + (dataPtr + 2 * nChannel + widthstep)[0] + (dataPtr + 2 * nChannel + 2 * widthstep)[0] + (dataPtr + 2 * nChannel + 3 * widthstep)[0]);
                sumGreen = (int)img_green[index_widht - 1, index_height] - ((dataPtr - 4 * nChannel)[1] * 4 + (dataPtr - 4 * nChannel + widthstep)[1] + (dataPtr - 4 * nChannel + 2 * widthstep)[1] + (dataPtr - 4 * nChannel + 3 * widthstep)[1]) + ((dataPtr + 2 * nChannel)[1] * 4 + (dataPtr + 2 * nChannel + widthstep)[1] + (dataPtr + 2 * nChannel + 2 * widthstep)[1] + (dataPtr + 2 * nChannel + 3 * widthstep)[1]);
                sumRed = (int)img_red[index_widht - 1, index_height] - ((dataPtr - 4 * nChannel)[2] * 4 + (dataPtr - 4 * nChannel + widthstep)[2] + (dataPtr - 4 * nChannel + 2 * widthstep)[2] + (dataPtr - 4 * nChannel + 3 * widthstep)[2]) + ((dataPtr + 2 * nChannel)[2] * 4 + (dataPtr + 2 * nChannel + widthstep)[2] + (dataPtr + 2 * nChannel + 2 * widthstep)[2] + (dataPtr + 2 * nChannel + 3 * widthstep)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                img_blue[index_widht, index_height] = sumBlue;
                img_green[index_widht, index_height] = sumGreen;
                img_red[index_widht, index_height] = sumRed;

                dataPtr += nChannel;
                dataPtr_d += nChannel;
                index_widht++;

                sumBlue = (int)img_blue[index_widht - 1, index_height] - ((dataPtr - 4 * nChannel)[0] * 4 + (dataPtr - 4 * nChannel + widthstep)[0] + (dataPtr - 4 * nChannel + 2 * widthstep)[0] + (dataPtr - 4 * nChannel + 3 * widthstep)[0]) + ((dataPtr + nChannel)[0] * 4 + (dataPtr + nChannel + widthstep)[0] + (dataPtr + nChannel + 2 * widthstep)[0] + (dataPtr + nChannel + 3 * widthstep)[0]);
                sumGreen = (int)img_green[index_widht - 1, index_height] - ((dataPtr - 4 * nChannel)[1] * 4 + (dataPtr - 4 * nChannel + widthstep)[1] + (dataPtr - 4 * nChannel + 2 * widthstep)[1] + (dataPtr - 4 * nChannel + 3 * widthstep)[1]) + ((dataPtr + nChannel)[1] * 4 + (dataPtr + nChannel + widthstep)[1] + (dataPtr + nChannel + 2 * widthstep)[1] + (dataPtr + nChannel + 3 * widthstep)[1]);
                sumRed = (int)img_red[index_widht - 1, index_height] - ((dataPtr - 4 * nChannel)[2] * 4 + (dataPtr - 4 * nChannel + widthstep)[2] + (dataPtr - 4 * nChannel + 2 * widthstep)[2] + (dataPtr - 4 * nChannel + 3 * widthstep)[2]) + ((dataPtr + nChannel)[2] * 4 + (dataPtr + nChannel + widthstep)[2] + (dataPtr + nChannel + 2 * widthstep)[2] + (dataPtr + nChannel + 3 * widthstep)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                img_blue[index_widht, index_height] = sumBlue;
                img_green[index_widht, index_height] = sumGreen;
                img_red[index_widht, index_height] = sumRed;

                dataPtr += nChannel;
                dataPtr_d += nChannel;
                index_widht++;

                sumBlue = (int)img_blue[index_widht - 1, index_height] - ((dataPtr - 4 * nChannel)[0] * 4 + (dataPtr - 4 * nChannel + widthstep)[0] + (dataPtr - 4 * nChannel + 2 * widthstep)[0] + (dataPtr - 4 * nChannel + 3 * widthstep)[0]) + ((dataPtr)[0] * 4 + (dataPtr + widthstep)[0] + (dataPtr + 2 * widthstep)[0] + (dataPtr + 3 * widthstep)[0]);
                sumGreen = (int)img_green[index_widht - 1, index_height] - ((dataPtr - 4 * nChannel)[1] * 4 + (dataPtr - 4 * nChannel + widthstep)[1] + (dataPtr - 4 * nChannel + 2 * widthstep)[1] + (dataPtr - 4 * nChannel + 3 * widthstep)[1]) + ((dataPtr)[1] * 4 + (dataPtr + widthstep)[1] + (dataPtr + 2 * widthstep)[1] + (dataPtr + 3 * widthstep)[1]);
                sumRed = (int)img_red[index_widht - 1, index_height] - ((dataPtr - 4 * nChannel)[2] * 4 + (dataPtr - 4 * nChannel + widthstep)[2] + (dataPtr - 4 * nChannel + 2 * widthstep)[2] + (dataPtr - 4 * nChannel + 3 * widthstep)[2]) + ((dataPtr)[2] * 4 + (dataPtr + widthstep)[2] + (dataPtr + 2 * widthstep)[2] + (dataPtr + 3 * widthstep)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                img_blue[index_widht, index_height] = sumBlue;
                img_green[index_widht, index_height] = sumGreen;
                img_red[index_widht, index_height] = sumRed;

                dataPtr = (byte*)m.imageData.ToPointer();
                dataPtr_d = (byte*)m_d.imageData.ToPointer();

                dataPtr += widthstep;
                dataPtr_d += widthstep;
                index_widht = 0;
                index_height = 1;

                sumBlue = (int)img_blue[index_widht, index_height - 1] - ((dataPtr - widthstep)[0] * 4 + (dataPtr - widthstep + nChannel)[0] + (dataPtr - widthstep + 2 * nChannel)[0] + (dataPtr - widthstep + 3 * nChannel)[0]) + ((dataPtr + 3 * widthstep)[0] * 4 + (dataPtr + 3 * widthstep + nChannel)[0] + (dataPtr + 3 * widthstep + 2 * nChannel)[0] + (dataPtr + 3 * widthstep + 3 * nChannel)[0]);
                sumGreen = (int)img_green[index_widht, index_height - 1] - ((dataPtr - widthstep)[1] * 4 + (dataPtr - widthstep + nChannel)[1] + (dataPtr - widthstep + 2 * nChannel)[1] + (dataPtr - widthstep + 3 * nChannel)[1]) + ((dataPtr + 3 * widthstep)[1] * 4 + (dataPtr + 3 * widthstep + nChannel)[1] + (dataPtr + 3 * widthstep + 2 * nChannel)[1] + (dataPtr + 3 * widthstep + 3 * nChannel)[1]);
                sumRed = (int)img_red[index_widht, index_height - 1] - ((dataPtr - widthstep)[2] * 4 + (dataPtr - widthstep + nChannel)[2] + (dataPtr - widthstep + 2 * nChannel)[2] + (dataPtr - widthstep + 3 * nChannel)[2]) + ((dataPtr + 3 * widthstep)[2] * 4 + (dataPtr + 3 * widthstep + nChannel)[2] + (dataPtr + 3 * widthstep + 2 * nChannel)[2] + (dataPtr + 3 * widthstep + 3 * nChannel)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                img_blue[index_widht, index_height] = sumBlue;
                img_green[index_widht, index_height] = sumGreen;
                img_red[index_widht, index_height] = sumRed;

                dataPtr += widthstep;
                dataPtr_d += widthstep;
                index_height++;

                sumBlue = (int)img_blue[index_widht, index_height - 1] - ((dataPtr - 2 * widthstep)[0] * 4 + (dataPtr - 2 * widthstep + nChannel)[0] + (dataPtr - 2 * widthstep + 2 * nChannel)[0] + (dataPtr - 2 * widthstep + 3 * nChannel)[0]) + ((dataPtr + 3 * widthstep)[0] * 4 + (dataPtr + 3 * widthstep + nChannel)[0] + (dataPtr + 3 * widthstep + 2 * nChannel)[0] + (dataPtr + 3 * widthstep + 3 * nChannel)[0]);
                sumGreen = (int)img_green[index_widht, index_height - 1] - ((dataPtr - 2 * widthstep)[1] * 4 + (dataPtr - 2 * widthstep + nChannel)[1] + (dataPtr - 2 * widthstep + 2 * nChannel)[1] + (dataPtr - 2 * widthstep + 3 * nChannel)[1]) + ((dataPtr + 3 * widthstep)[1] * 4 + (dataPtr + 3 * widthstep + nChannel)[1] + (dataPtr + 3 * widthstep + 2 * nChannel)[1] + (dataPtr + 3 * widthstep + 3 * nChannel)[1]);
                sumRed = (int)img_red[index_widht, index_height - 1] - ((dataPtr - 2 * widthstep)[2] * 4 + (dataPtr - 2 * widthstep + nChannel)[2] + (dataPtr - 2 * widthstep + 2 * nChannel)[2] + (dataPtr - 2 * widthstep + 3 * nChannel)[2]) + ((dataPtr + 3 * widthstep)[2] * 4 + (dataPtr + 3 * widthstep + nChannel)[2] + (dataPtr + 3 * widthstep + 2 * nChannel)[2] + (dataPtr + 3 * widthstep + 3 * nChannel)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                img_blue[index_widht, index_height] = sumBlue;
                img_green[index_widht, index_height] = sumGreen;
                img_red[index_widht, index_height] = sumRed;

                dataPtr += widthstep;
                dataPtr_d += widthstep;
                index_height++;

                sumBlue = (int)img_blue[index_widht, index_height - 1] - ((dataPtr - 3 * widthstep)[0] * 4 + (dataPtr - 3 * widthstep + nChannel)[0] + (dataPtr - 3 * widthstep + 2 * nChannel)[0] + (dataPtr - 3 * widthstep + 3 * nChannel)[0]) + ((dataPtr + 3 * widthstep)[0] * 4 + (dataPtr + 3 * widthstep + nChannel)[0] + (dataPtr + 3 * widthstep + 2 * nChannel)[0] + (dataPtr + 3 * widthstep + 3 * nChannel)[0]);
                sumGreen = (int)img_green[index_widht, index_height - 1] - ((dataPtr - 3 * widthstep)[1] * 4 + (dataPtr - 3 * widthstep + nChannel)[1] + (dataPtr - 3 * widthstep + 2 * nChannel)[1] + (dataPtr - 3 * widthstep + 3 * nChannel)[1]) + ((dataPtr + 3 * widthstep)[1] * 4 + (dataPtr + 3 * widthstep + nChannel)[1] + (dataPtr + 3 * widthstep + 2 * nChannel)[1] + (dataPtr + 3 * widthstep + 3 * nChannel)[1]);
                sumRed = (int)img_red[index_widht, index_height - 1] - ((dataPtr - 3 * widthstep)[2] * 4 + (dataPtr - 3 * widthstep + nChannel)[2] + (dataPtr - 3 * widthstep + 2 * nChannel)[2] + (dataPtr - 3 * widthstep + 3 * nChannel)[2]) + ((dataPtr + 3 * widthstep)[2] * 4 + (dataPtr + 3 * widthstep + nChannel)[2] + (dataPtr + 3 * widthstep + 2 * nChannel)[2] + (dataPtr + 3 * widthstep + 3 * nChannel)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                img_blue[index_widht, index_height] = sumBlue;
                img_green[index_widht, index_height] = sumGreen;
                img_red[index_widht, index_height] = sumRed;

                dataPtr += widthstep;
                dataPtr_d += widthstep;
                index_height++;

                for (int y_me = 4; y_me < height - 3; y_me++)
                {
                    sumBlue = (int)img_blue[index_widht, index_height - 1] - ((dataPtr - 4 * widthstep)[0] * 4 + (dataPtr - 4 * widthstep + nChannel)[0] + (dataPtr - 4 * widthstep + 2 * nChannel)[0] + (dataPtr - 4 * widthstep + 3 * nChannel)[0]) + ((dataPtr + 3 * widthstep)[0] * 4 + (dataPtr + 3 * widthstep + nChannel)[0] + (dataPtr + 3 * widthstep + 2 * nChannel)[0] + (dataPtr + 3 * widthstep + 3 * nChannel)[0]);
                    sumGreen = (int)img_green[index_widht, index_height - 1] - ((dataPtr - 4 * widthstep)[1] * 4 + (dataPtr - 4 * widthstep + nChannel)[1] + (dataPtr - 4 * widthstep + 2 * nChannel)[1] + (dataPtr - 4 * widthstep + 3 * nChannel)[1]) + ((dataPtr + 3 * widthstep)[1] * 4 + (dataPtr + 3 * widthstep + nChannel)[1] + (dataPtr + 3 * widthstep + 2 * nChannel)[1] + (dataPtr + 3 * widthstep + 3 * nChannel)[1]);
                    sumRed = (int)img_red[index_widht, index_height - 1] - ((dataPtr - 4 * widthstep)[2] * 4 + (dataPtr - 4 * widthstep + nChannel)[2] + (dataPtr - 4 * widthstep + 2 * nChannel)[2] + (dataPtr - 4 * widthstep + 3 * nChannel)[2]) + ((dataPtr + 3 * widthstep)[2] * 4 + (dataPtr + 3 * widthstep + nChannel)[2] + (dataPtr + 3 * widthstep + 2 * nChannel)[2] + (dataPtr + 3 * widthstep + 3 * nChannel)[2]);

                    dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                    dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                    dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                    img_blue[index_widht, index_height] = sumBlue;
                    img_green[index_widht, index_height] = sumGreen;
                    img_red[index_widht, index_height] = sumRed;

                    dataPtr += widthstep;
                    dataPtr_d += widthstep;
                    index_height++;
                }

                sumBlue = (int)img_blue[index_widht, index_height - 1] - ((dataPtr - 4 * widthstep)[0] * 4 + (dataPtr - 4 * widthstep + nChannel)[0] + (dataPtr - 4 * widthstep + 2 * nChannel)[0] + (dataPtr - 4 * widthstep + 3 * nChannel)[0]) + ((dataPtr + 2 * widthstep)[0] * 4 + (dataPtr + 2 * widthstep + nChannel)[0] + (dataPtr + 2 * widthstep + 2 * nChannel)[0] + (dataPtr + 2 * widthstep + 3 * nChannel)[0]);
                sumGreen = (int)img_green[index_widht, index_height - 1] - ((dataPtr - 4 * widthstep)[1] * 4 + (dataPtr - 4 * widthstep + nChannel)[1] + (dataPtr - 4 * widthstep + 2 * nChannel)[1] + (dataPtr - 4 * widthstep + 3 * nChannel)[1]) + ((dataPtr + 2 * widthstep)[1] * 4 + (dataPtr + 2 * widthstep + nChannel)[1] + (dataPtr + 2 * widthstep + 2 * nChannel)[1] + (dataPtr + 2 * widthstep + 3 * nChannel)[1]);
                sumRed = (int)img_red[index_widht, index_height - 1] - ((dataPtr - 4 * widthstep)[2] * 4 + (dataPtr - 4 * widthstep + nChannel)[2] + (dataPtr - 4 * widthstep + 2 * nChannel)[2] + (dataPtr - 4 * widthstep + 3 * nChannel)[2]) + ((dataPtr + 2 * widthstep)[2] * 4 + (dataPtr + 2 * widthstep + nChannel)[2] + (dataPtr + 2 * widthstep + 2 * nChannel)[2] + (dataPtr + 2 * widthstep + 3 * nChannel)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                img_blue[index_widht, index_height] = sumBlue;
                img_green[index_widht, index_height] = sumGreen;
                img_red[index_widht, index_height] = sumRed;

                dataPtr += widthstep;
                dataPtr_d += widthstep;
                index_height++;

                sumBlue = (int)img_blue[index_widht, index_height - 1] - ((dataPtr - 4 * widthstep)[0] * 4 + (dataPtr - 4 * widthstep + nChannel)[0] + (dataPtr - 4 * widthstep + 2 * nChannel)[0] + (dataPtr - 4 * widthstep + 3 * nChannel)[0]) + ((dataPtr + widthstep)[0] * 4 + (dataPtr + widthstep + nChannel)[0] + (dataPtr + widthstep + 2 * nChannel)[0] + (dataPtr + widthstep + 3 * nChannel)[0]);
                sumGreen = (int)img_green[index_widht, index_height - 1] - ((dataPtr - 4 * widthstep)[1] * 4 + (dataPtr - 4 * widthstep + nChannel)[1] + (dataPtr - 4 * widthstep + 2 * nChannel)[1] + (dataPtr - 4 * widthstep + 3 * nChannel)[1]) + ((dataPtr + widthstep)[1] * 4 + (dataPtr + widthstep + nChannel)[1] + (dataPtr + widthstep + 2 * nChannel)[1] + (dataPtr + widthstep + 3 * nChannel)[1]);
                sumRed = (int)img_red[index_widht, index_height - 1] - ((dataPtr - 4 * widthstep)[2] * 4 + (dataPtr - 4 * widthstep + nChannel)[2] + (dataPtr - 4 * widthstep + 2 * nChannel)[2] + (dataPtr - 4 * widthstep + 3 * nChannel)[2]) + ((dataPtr + widthstep)[2] * 4 + (dataPtr + widthstep + nChannel)[2] + (dataPtr + widthstep + 2 * nChannel)[2] + (dataPtr + widthstep + 3 * nChannel)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                img_blue[index_widht, index_height] = sumBlue;
                img_green[index_widht, index_height] = sumGreen;
                img_red[index_widht, index_height] = sumRed;

                dataPtr += widthstep;
                dataPtr_d += widthstep;
                index_height++;

                sumBlue = (int)img_blue[index_widht, index_height - 1] - ((dataPtr - 4 * widthstep)[0] * 4 + (dataPtr - 4 * widthstep + nChannel)[0] + (dataPtr - 4 * widthstep + 2 * nChannel)[0] + (dataPtr - 4 * widthstep + 3 * nChannel)[0]) + ((dataPtr)[0] * 4 + (dataPtr + nChannel)[0] + (dataPtr + 2 * nChannel)[0] + (dataPtr + 3 * nChannel)[0]);
                sumGreen = (int)img_green[index_widht, index_height - 1] - ((dataPtr - 4 * widthstep)[1] * 4 + (dataPtr - 4 * widthstep + nChannel)[1] + (dataPtr - 4 * widthstep + 2 * nChannel)[1] + (dataPtr - 4 * widthstep + 3 * nChannel)[1]) + ((dataPtr)[1] * 4 + (dataPtr + nChannel)[1] + (dataPtr + 2 * nChannel)[1] + (dataPtr + 3 * nChannel)[1]);
                sumRed = (int)img_red[index_widht, index_height - 1] - ((dataPtr - 4 * widthstep)[2] * 4 + (dataPtr - 4 * widthstep + nChannel)[2] + (dataPtr - 4 * widthstep + 2 * nChannel)[2] + (dataPtr - 4 * widthstep + 3 * nChannel)[2]) + ((dataPtr)[2] * 4 + (dataPtr + nChannel)[2] + (dataPtr + 2 * nChannel)[2] + (dataPtr + 3 * nChannel)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                dataPtr = (byte*)m.imageData.ToPointer();
                dataPtr_d = (byte*)m_d.imageData.ToPointer();

                dataPtr += widthstep + nChannel;
                dataPtr_d += widthstep + nChannel;
                index_widht = 1;
                index_height = 1;

                for (int y_i = 0; y_i < 3; y_i++)
                {
                    for (int x_i = 1; x_i < widht; x_i++)
                    {
                        sumBlue = (int)img_blue[index_widht - 1, index_height] - ((dataPtr - nChannel - widthstep)[0] * 3 + (dataPtr - nChannel)[0] + (dataPtr - nChannel + widthstep)[0] + (dataPtr - nChannel + 2 * widthstep)[0] + (dataPtr - nChannel + 3 * widthstep)[0]) + ((dataPtr + 3 * nChannel)[0] * 4 + (dataPtr + 3 * nChannel + widthstep)[0] + (dataPtr + 3 * nChannel + 2 * widthstep)[0] + (dataPtr + 3 * nChannel + 3 * widthstep)[0]);
                        sumGreen = (int)img_green[index_widht - 1, index_height] - ((dataPtr - nChannel)[1] * 4 + (dataPtr - nChannel + widthstep)[1] + (dataPtr - nChannel + 2 * widthstep)[1] + (dataPtr - nChannel + 3 * widthstep)[1]) + ((dataPtr + 3 * nChannel)[1] * 4 + (dataPtr + 3 * nChannel + widthstep)[1] + (dataPtr + 3 * nChannel + 2 * widthstep)[1] + (dataPtr + 3 * nChannel + 3 * widthstep)[1]);
                        sumRed = (int)img_red[index_widht - 1, index_height] - ((dataPtr - nChannel)[2] * 4 + (dataPtr - nChannel + widthstep)[2] + (dataPtr - nChannel + 2 * widthstep)[2] + (dataPtr - nChannel + 3 * widthstep)[2]) + ((dataPtr + 3 * nChannel)[2] * 4 + (dataPtr + 3 * nChannel + widthstep)[2] + (dataPtr + 3 * nChannel + 2 * widthstep)[2] + (dataPtr + 3 * nChannel + 3 * widthstep)[2]);
                    }
                }

                dataPtr = (byte*)m.imageData.ToPointer();
                dataPtr_d = (byte*)m_d.imageData.ToPointer();

                dataPtr += 4 * nChannel + 4 * widthstep;
                dataPtr_d += 4 * nChannel + 4 * widthstep;
                index_widht = 4;
                index_height = 4;

                sumBlue = (int)(
                               (dataPtr - 3 * widthstep - 3 * nChannel)[0] + (dataPtr - 3 * widthstep - 2 * nChannel)[0] + (dataPtr - 3 * widthstep - nChannel)[0] + (dataPtr - 3 * widthstep)[0] + (dataPtr - 3 * widthstep + nChannel)[0] + (dataPtr - 3 * widthstep + 2 * nChannel)[0] + (dataPtr - 3 * widthstep + 3 * nChannel)[0] +
                              +(dataPtr - 2 * widthstep - 3 * nChannel)[0] + (dataPtr - 2 * widthstep - 2 * nChannel)[0] + (dataPtr - 2 * widthstep - nChannel)[0] + (dataPtr - 2 * widthstep)[0] + (dataPtr - 2 * widthstep + nChannel)[0] + (dataPtr - 2 * widthstep + 2 * nChannel)[0] + (dataPtr - 2 * widthstep + 3 * nChannel)[0] +
                              +(dataPtr - widthstep - 3 * nChannel)[0] + (dataPtr - widthstep - 2 * nChannel)[0] + (dataPtr - widthstep - nChannel)[0] + (dataPtr - widthstep)[0] + (dataPtr - widthstep + nChannel)[0] + (dataPtr - widthstep + 2 * nChannel)[0] + (dataPtr - widthstep + 3 * nChannel)[0] +
                              +(dataPtr - 3 * nChannel)[0] + (dataPtr - 2 * nChannel)[0] + (dataPtr - nChannel)[0] + (dataPtr)[0] + (dataPtr + nChannel)[0] + (dataPtr + 2 * nChannel)[0] + (dataPtr + 3 * nChannel)[0] +
                              +(dataPtr + widthstep - 3 * nChannel)[0] + (dataPtr + widthstep - 2 * nChannel)[0] + (dataPtr + widthstep - nChannel)[0] + (dataPtr + widthstep)[0] + (dataPtr + widthstep + nChannel)[0] + (dataPtr + widthstep + 2 * nChannel)[0] + (dataPtr + widthstep + 3 * nChannel)[0] +
                              +(dataPtr + 2 * widthstep - 3 * nChannel)[0] + (dataPtr + 2 * widthstep - 2 * nChannel)[0] + (dataPtr + 2 * widthstep - nChannel)[0] + (dataPtr + 2 * widthstep)[0] + (dataPtr + 2 * widthstep + nChannel)[0] + (dataPtr + 2 * widthstep + 2 * nChannel)[0] + (dataPtr + 2 * widthstep + 3 * nChannel)[0] +
                              +(dataPtr + 3 * widthstep - 3 * nChannel)[0] + (dataPtr + 3 * widthstep - 2 * nChannel)[0] + (dataPtr + 3 * widthstep - nChannel)[0] + (dataPtr + 3 * widthstep)[0] + (dataPtr + 3 * widthstep + nChannel)[0] + (dataPtr + 3 * widthstep + 2 * nChannel)[0] + (dataPtr + 3 * widthstep + 3 * nChannel)[0]);

                sumGreen = (int)(
                                (dataPtr - 3 * widthstep - 3 * nChannel)[1] + (dataPtr - 3 * widthstep - 2 * nChannel)[1] + (dataPtr - 3 * widthstep - nChannel)[1] + (dataPtr - 3 * widthstep)[1] + (dataPtr - 3 * widthstep + nChannel)[1] + (dataPtr - 3 * widthstep + 2 * nChannel)[1] + (dataPtr - 3 * widthstep + 3 * nChannel)[1] +
                               +(dataPtr - 2 * widthstep - 3 * nChannel)[1] + (dataPtr - 2 * widthstep - 2 * nChannel)[1] + (dataPtr - 2 * widthstep - nChannel)[1] + (dataPtr - 2 * widthstep)[1] + (dataPtr - 2 * widthstep + nChannel)[1] + (dataPtr - 2 * widthstep + 2 * nChannel)[1] + (dataPtr - 2 * widthstep + 3 * nChannel)[1] +
                               +(dataPtr - widthstep - 3 * nChannel)[1] + (dataPtr - widthstep - 2 * nChannel)[1] + (dataPtr - widthstep - nChannel)[1] + (dataPtr - widthstep)[1] + (dataPtr - widthstep + nChannel)[1] + (dataPtr - widthstep + 2 * nChannel)[1] + (dataPtr - widthstep + 3 * nChannel)[1] +
                               +(dataPtr - 3 * nChannel)[1] + (dataPtr - 2 * nChannel)[1] + (dataPtr - nChannel)[1] + (dataPtr)[1] + (dataPtr + nChannel)[1] + (dataPtr + 2 * nChannel)[1] + (dataPtr + 3 * nChannel)[1] +
                               +(dataPtr + widthstep - 3 * nChannel)[1] + (dataPtr + widthstep - 2 * nChannel)[1] + (dataPtr + widthstep - nChannel)[1] + (dataPtr + widthstep)[1] + (dataPtr + widthstep + nChannel)[1] + (dataPtr + widthstep + 2 * nChannel)[1] + (dataPtr + widthstep + 3 * nChannel)[1] +
                               +(dataPtr + 2 * widthstep - 3 * nChannel)[1] + (dataPtr + 2 * widthstep - 2 * nChannel)[1] + (dataPtr + 2 * widthstep - nChannel)[1] + (dataPtr + 2 * widthstep)[1] + (dataPtr + 2 * widthstep + nChannel)[1] + (dataPtr + 2 * widthstep + 2 * nChannel)[1] + (dataPtr + 2 * widthstep + 3 * nChannel)[1] +
                               +(dataPtr + 3 * widthstep - 3 * nChannel)[1] + (dataPtr + 3 * widthstep - 2 * nChannel)[1] + (dataPtr + 3 * widthstep - nChannel)[1] + (dataPtr + 3 * widthstep)[1] + (dataPtr + 3 * widthstep + nChannel)[1] + (dataPtr + 3 * widthstep + 2 * nChannel)[1] + (dataPtr + 3 * widthstep + 3 * nChannel)[1]);

                sumRed = (int)(
                               (dataPtr - 3 * widthstep - 3 * nChannel)[2] + (dataPtr - 3 * widthstep - 2 * nChannel)[2] + (dataPtr - 3 * widthstep - nChannel)[2] + (dataPtr - 3 * widthstep)[2] + (dataPtr - 3 * widthstep + nChannel)[2] + (dataPtr - 3 * widthstep + 2 * nChannel)[2] + (dataPtr - 3 * widthstep + 3 * nChannel)[2] +
                              +(dataPtr - 2 * widthstep - 3 * nChannel)[2] + (dataPtr - 2 * widthstep - 2 * nChannel)[2] + (dataPtr - 2 * widthstep - nChannel)[2] + (dataPtr - 2 * widthstep)[2] + (dataPtr - 2 * widthstep + nChannel)[2] + (dataPtr - 2 * widthstep + 2 * nChannel)[2] + (dataPtr - 2 * widthstep + 3 * nChannel)[2] +
                              +(dataPtr - widthstep - 3 * nChannel)[2] + (dataPtr - widthstep - 2 * nChannel)[2] + (dataPtr - widthstep - nChannel)[2] + (dataPtr - widthstep)[2] + (dataPtr - widthstep + nChannel)[2] + (dataPtr - widthstep + 2 * nChannel)[2] + (dataPtr - widthstep + 3 * nChannel)[2] +
                              +(dataPtr - 3 * nChannel)[2] + (dataPtr - 2 * nChannel)[2] + (dataPtr - nChannel)[2] + (dataPtr)[2] + (dataPtr + nChannel)[2] + (dataPtr + 2 * nChannel)[2] + (dataPtr + 3 * nChannel)[2] +
                              +(dataPtr + widthstep - 3 * nChannel)[2] + (dataPtr + widthstep - 2 * nChannel)[2] + (dataPtr + widthstep - nChannel)[2] + (dataPtr + widthstep)[2] + (dataPtr + widthstep + nChannel)[2] + (dataPtr + widthstep + 2 * nChannel)[2] + (dataPtr + widthstep + 3 * nChannel)[2] +
                              +(dataPtr + 2 * widthstep - 3 * nChannel)[2] + (dataPtr + 2 * widthstep - 2 * nChannel)[2] + (dataPtr + 2 * widthstep - nChannel)[2] + (dataPtr + 2 * widthstep)[2] + (dataPtr + 2 * widthstep + nChannel)[2] + (dataPtr + 2 * widthstep + 2 * nChannel)[2] + (dataPtr + 2 * widthstep + 3 * nChannel)[2] +
                              +(dataPtr + 3 * widthstep - 3 * nChannel)[2] + (dataPtr + 3 * widthstep - 2 * nChannel)[2] + (dataPtr + 3 * widthstep - nChannel)[2] + (dataPtr + 3 * widthstep)[2] + (dataPtr + 3 * widthstep + nChannel)[2] + (dataPtr + 3 * widthstep + 2 * nChannel)[2] + (dataPtr + 3 * widthstep + 3 * nChannel)[2]);

                dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                img_blue[index_widht, index_height] = sumBlue;
                img_green[index_widht, index_height] = sumGreen;
                img_red[index_widht, index_height] = sumRed;

                dataPtr += nChannel;
                dataPtr_d += nChannel;
                index_widht++;

                for (int x = 5; x < widht - 3; x++)
                {
                    sumBlue = (int)img_blue[index_widht - 1, index_height] - ((dataPtr - 4 * nChannel - 3 * widthstep)[0] + (dataPtr - 4 * nChannel - 2 * widthstep)[0] + (dataPtr - 4 * nChannel - widthstep)[0] + (dataPtr - 4 * nChannel)[0] + (dataPtr - 4 * nChannel + widthstep)[0] + (dataPtr - 4 * nChannel + 2 * widthstep)[0] + (dataPtr - 4 * nChannel + 3 * widthstep)[0]) + ((dataPtr + 3 * nChannel - 3 * widthstep)[0] + (dataPtr + 3 * nChannel - 2 * widthstep)[0] + (dataPtr + 3 * nChannel - widthstep)[0] + (dataPtr + 3 * nChannel)[0] + (dataPtr + 3 * nChannel + widthstep)[0] + (dataPtr + 3 * nChannel + 2 * widthstep)[0] + (dataPtr + 3 * nChannel + 3 * widthstep)[0]);
                    sumGreen = (int)img_green[index_widht - 1, index_height] - ((dataPtr - 4 * nChannel - 3 * widthstep)[1] + (dataPtr - 4 * nChannel - 2 * widthstep)[1] + (dataPtr - 4 * nChannel - widthstep)[1] + (dataPtr - 4 * nChannel)[1] + (dataPtr - 4 * nChannel + widthstep)[1] + (dataPtr - 4 * nChannel + 2 * widthstep)[1] + (dataPtr - 4 * nChannel + 3 * widthstep)[1]) + ((dataPtr + 3 * nChannel - 3 * widthstep)[1] + (dataPtr + 3 * nChannel - 2 * widthstep)[1] + (dataPtr + 3 * nChannel - widthstep)[1] + (dataPtr + 3 * nChannel)[1] + (dataPtr + 3 * nChannel + widthstep)[1] + (dataPtr + 3 * nChannel + 2 * widthstep)[1] + (dataPtr + 3 * nChannel + 3 * widthstep)[1]);
                    sumRed = (int)img_red[index_widht - 1, index_height] - ((dataPtr - 4 * nChannel - 3 * widthstep)[2] + (dataPtr - 4 * nChannel - 2 * widthstep)[2] + (dataPtr - 4 * nChannel - widthstep)[2] + (dataPtr - 4 * nChannel)[2] + (dataPtr - 4 * nChannel + widthstep)[2] + (dataPtr - 4 * nChannel + 2 * widthstep)[2] + (dataPtr - 4 * nChannel + 3 * widthstep)[2]) + ((dataPtr + 3 * nChannel - 3 * widthstep)[2] + (dataPtr + 3 * nChannel - 2 * widthstep)[2] + (dataPtr + 3 * nChannel - widthstep)[2] + (dataPtr + 3 * nChannel)[2] + (dataPtr + 3 * nChannel + widthstep)[2] + (dataPtr + 3 * nChannel + 2 * widthstep)[2] + (dataPtr + 3 * nChannel + 3 * widthstep)[2]);

                    dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                    dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                    dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                    img_blue[index_widht, index_height] = sumBlue;
                    img_green[index_widht, index_height] = sumGreen;
                    img_red[index_widht, index_height] = sumRed;

                    dataPtr += nChannel;
                    dataPtr_d += nChannel;
                    index_widht++;
                }

                dataPtr = (byte*)m.imageData.ToPointer();
                dataPtr_d = (byte*)m_d.imageData.ToPointer();

                dataPtr += 4 * nChannel + 5 * widthstep;
                dataPtr_d += 4 * nChannel + 5 * widthstep;
                index_widht = 4;
                index_height = 5;

                for (int y = 5; y < height - 3; y++)
                {
                    sumBlue = (int)img_blue[index_widht, index_height - 1] - ((dataPtr - 4 * widthstep - 3 * nChannel)[0] + (dataPtr - 4 * widthstep - 2 * nChannel)[0] + (dataPtr - 4 * widthstep - nChannel)[0] + (dataPtr - 4 * widthstep)[0] + (dataPtr - 4 * widthstep + nChannel)[0] + (dataPtr - 4 * widthstep + 2 * nChannel)[0] + (dataPtr - 4 * widthstep + 3 * nChannel)[0]) + ((dataPtr + 3 * widthstep - 3 * nChannel)[0] + (dataPtr + 3 * widthstep - 2 * nChannel)[0] + (dataPtr + 3 * widthstep - nChannel)[0] + (dataPtr + 3 * widthstep)[0] + (dataPtr + 3 * widthstep + nChannel)[0] + (dataPtr + 3 * widthstep + 2 * nChannel)[0] + (dataPtr + 3 * widthstep + 3 * nChannel)[0]);
                    sumGreen = (int)img_green[index_widht, index_height - 1] - ((dataPtr - 4 * widthstep - 3 * nChannel)[1] + (dataPtr - 4 * widthstep - 2 * nChannel)[1] + (dataPtr - 4 * widthstep - nChannel)[1] + (dataPtr - 4 * widthstep)[1] + (dataPtr - 4 * widthstep + nChannel)[1] + (dataPtr - 4 * widthstep + 2 * nChannel)[1] + (dataPtr - 4 * widthstep + 3 * nChannel)[1]) + ((dataPtr + 3 * widthstep - 3 * nChannel)[1] + (dataPtr + 3 * widthstep - 2 * nChannel)[1] + (dataPtr + 3 * widthstep - nChannel)[1] + (dataPtr + 3 * widthstep)[1] + (dataPtr + 3 * widthstep + nChannel)[1] + (dataPtr + 3 * widthstep + 2 * nChannel)[1] + (dataPtr + 3 * widthstep + 3 * nChannel)[1]);
                    sumRed = (int)img_red[index_widht, index_height - 1] - ((dataPtr - 4 * widthstep - 3 * nChannel)[2] + (dataPtr - 4 * widthstep - 2 * nChannel)[2] + (dataPtr - 4 * widthstep - nChannel)[2] + (dataPtr - 4 * widthstep)[2] + (dataPtr - 4 * widthstep + nChannel)[2] + (dataPtr - 4 * widthstep + 2 * nChannel)[2] + (dataPtr - 4 * widthstep + 3 * nChannel)[2]) + ((dataPtr + 3 * widthstep - 3 * nChannel)[2] + (dataPtr + 3 * widthstep - 2 * nChannel)[2] + (dataPtr + 3 * widthstep - nChannel)[2] + (dataPtr + 3 * widthstep)[2] + (dataPtr + 3 * widthstep + nChannel)[2] + (dataPtr + 3 * widthstep + 2 * nChannel)[2] + (dataPtr + 3 * widthstep + 3 * nChannel)[2]);

                    dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                    dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                    dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                    img_blue[index_widht, index_height] = sumBlue;
                    img_green[index_widht, index_height] = sumGreen;
                    img_red[index_widht, index_height] = sumRed;

                    dataPtr += widthstep;
                    dataPtr_d += widthstep;
                    index_height++;
                }

                dataPtr = (byte*)m.imageData.ToPointer();
                dataPtr_d = (byte*)m_d.imageData.ToPointer();

                dataPtr += 5 * nChannel + 5 * widthstep;
                dataPtr_d += 5 * nChannel + 5 * widthstep;
                index_widht = 5;
                index_height = 5;

                for (int y = 5; y < height - 3; y++)
                {
                    for (int x = 6; x < widht - 2; x++)
                    {
                        sumBlue = img_blue[index_widht, index_height - 1] - img_blue[index_widht - 1, index_height - 1] + img_blue[index_widht - 1, index_height] + (dataPtr - 4 * widthstep - 4 * nChannel)[0] - (dataPtr + 3 * widthstep - 4 * nChannel)[0] - (dataPtr - 4 * widthstep + 3 * nChannel)[0] + (dataPtr + 3 * widthstep + 3 * nChannel)[0];
                        sumGreen = img_green[index_widht, index_height - 1] - img_green[index_widht - 1, index_height - 1] + img_green[index_widht - 1, index_height] + (dataPtr - 4 * widthstep - 4 * nChannel)[1] - (dataPtr + 3 * widthstep - 4 * nChannel)[1] - (dataPtr - 4 * widthstep + 3 * nChannel)[1] + (dataPtr + 3 * widthstep + 3 * nChannel)[1];
                        sumRed = img_red[index_widht, index_height - 1] - img_red[index_widht - 1, index_height - 1] + img_red[index_widht - 1, index_height] + (dataPtr - 4 * widthstep - 4 * nChannel)[2] - (dataPtr + 3 * widthstep - 4 * nChannel)[2] - (dataPtr - 4 * widthstep + 3 * nChannel)[2] + (dataPtr + 3 * widthstep + 3 * nChannel)[2];

                        dataPtr_d[0] = (byte)Math.Round(sumBlue / 49.0);
                        dataPtr_d[1] = (byte)Math.Round(sumGreen / 49.0);
                        dataPtr_d[2] = (byte)Math.Round(sumRed / 49.0);

                        img_blue[index_widht, index_height] = sumBlue;
                        img_green[index_widht, index_height] = sumGreen;
                        img_red[index_widht, index_height] = sumRed;

                        dataPtr += nChannel;
                        dataPtr_d += nChannel;
                        index_widht++;
                    }
                    dataPtr_d += padding + 8 * nChannel;
                    dataPtr += padding + 8 * nChannel;
                    index_height++;
                    index_widht = 5;
                }
            }
        }

        public static void Rotation_Bilinear(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle)
        {
            unsafe
            { 
                MIplImage m = imgCopy.MIplImage;
                MIplImage m_d = img.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer();
                byte* dataPtr_d = (byte*)m_d.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChannel = m.nChannels;
                int widthstep = m.widthStep;
                int padding = m_d.widthStep - m_d.nChannels * m_d.width;

                byte blue, green, red;
                double x_o, y_o;
                double cos, sen, inter;
                int se, sd, ie, id;
                double offsetX, offsetY;

                cos = System.Math.Cos(angle);
                sen = System.Math.Sin(angle);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x_o = ((x - width / 2.0) * cos) - ((height / 2.0 - y) * sen) + width / 2.0;
                        y_o = height / 2.0 - ((x - width / 2.0) * sen) - ((height / 2.0 - y) * cos);

                        offsetX = x_o - (int)x_o;
                        offsetY = y_o - (int)y_o;

                        if (x_o > width - 1 || x_o < 0 || y_o > height - 1 || y_o < 0)
                        {
                            dataPtr_d[0] = 0;
                            dataPtr_d[1] = 0;
                            dataPtr_d[2] = 0;
                        }
                        else
                        {
                            se = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[0];
                            sd = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[0];
                            ie = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[0];
                            id = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[0];


                            inter = Math.Round((1 - offsetY) * ((1 - offsetX) * se + offsetX * sd) + offsetY * ((1 - offsetX) * ie + offsetX * id));
                            blue = (byte)inter;

                            se = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[1];
                            sd = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[1];
                            ie = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[1];
                            id = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[1];

                            inter = Math.Round((1 - offsetY) * ((1 - offsetX) * se + offsetX * sd) + offsetY * ((1 - offsetX) * ie + offsetX * id));
                            green = (byte)inter;

                            se = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[2];
                            sd = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[2];
                            ie = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[2];
                            id = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[2];

                            inter = Math.Round((1 - offsetY) * ((1 - offsetX) * se + offsetX * sd) + offsetY * ((1 - offsetX) * ie + offsetX * id));
                            red = (byte)inter;

                            dataPtr_d[0] = blue;
                            dataPtr_d[1] = green;
                            dataPtr_d[2] = red;
                        }
                        dataPtr_d += nChannel;
                    }
                    dataPtr_d += padding;
                }
            }
        }

        public static void Scale_Bilinear(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor)
        {
            unsafe
            {
                MIplImage m = imgCopy.MIplImage;
                MIplImage m_d = img.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer();
                byte* dataPtr_d = (byte*)m_d.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChannel = m.nChannels;
                int widthstep = m.widthStep;
                int padding = m_d.widthStep - m_d.nChannels * m_d.width;

                byte blue, green, red;
                double x_o, y_o;
                int se, sd, ie, id;
                double offsetX, offsetY, inter;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x_o = x / scaleFactor;
                        y_o = y / scaleFactor;

                        offsetX = x_o - (int)x_o;
                        offsetY = y_o - (int)y_o;

                        if (x_o > width - 1 || x_o < 0 || y_o > height - 1 || y_o < 0)
                        {
                            dataPtr_d[0] = 0;
                            dataPtr_d[1] = 0;
                            dataPtr_d[2] = 0;
                        }
                        else
                        {
                            se = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[0];
                            sd = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[0];
                            ie = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[0];
                            id = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[0];

                            inter = Math.Round((1 - offsetY) * ((1 - offsetX) * se + offsetX * sd) + offsetY * ((1 - offsetX) * ie + offsetX * id));
                            blue = (byte)inter;

                            se = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[1];
                            sd = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[1];
                            ie = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[1];
                            id = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[1];

                            inter = Math.Round((1 - offsetY) * ((1 - offsetX) * se + offsetX * sd) + offsetY * ((1 - offsetX) * ie + offsetX * id));
                            green = (byte)inter;

                            se = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[2];
                            sd = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[2];
                            ie = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[2];
                            id = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[2];

                            inter = Math.Round((1 - offsetY) * ((1 - offsetX) * se + offsetX * sd) + offsetY * ((1 - offsetX) * ie + offsetX * id));
                            red = (byte)inter;

                            dataPtr_d[0] = blue;
                            dataPtr_d[1] = green;
                            dataPtr_d[2] = red;
                        }
                        dataPtr_d += nChannel;
                    }
                    dataPtr_d += padding;
                }
            }
        }

        public static void Scale_point_xy_Bilinear(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor, int centerX, int centerY)
        {
            unsafe
            {
                MIplImage m = imgCopy.MIplImage;
                MIplImage m_d = img.MIplImage;

                byte* dataPtr = (byte*)m.imageData.ToPointer();
                byte* dataPtr_d = (byte*)m_d.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChannel = m.nChannels;
                int widthstep = m.widthStep;
                int padding = m_d.widthStep - m_d.nChannels * m_d.width;

                byte blue, green, red;
                double x_o, y_o;
                int se, sd, ie, id;
                double offsetX, offsetY, inter;
                double add_xo = centerX - (width / 2) / scaleFactor;
                double add_yo = centerY - (height / 2) / scaleFactor;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x_o = (double)x / scaleFactor + add_xo;
                        y_o = (double)y / scaleFactor + add_yo;

                        offsetX = x_o - (int)x_o;
                        offsetY = y_o - (int)y_o;

                        if (x_o > width - 1 || x_o < 0 || y_o > height - 1 || y_o < 0)
                        {
                            dataPtr_d[0] = 0;
                            dataPtr_d[1] = 0;
                            dataPtr_d[2] = 0;
                        }
                        else
                        {
                            se = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[0];
                            sd = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[0];
                            ie = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[0];
                            id = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[0];

                            inter = Math.Round((1 - offsetY) * ((1 - offsetX) * se + offsetX * sd) + offsetY * ((1 - offsetX) * ie + offsetX * id));
                            blue = (byte)inter;

                            se = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[1];
                            sd = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[1];
                            ie = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[1];
                            id = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[1];

                            inter = Math.Round((1 - offsetY) * ((1 - offsetX) * se + offsetX * sd) + offsetY * ((1 - offsetX) * ie + offsetX * id));
                            green = (byte)inter;

                            se = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[2];
                            sd = (dataPtr + (int)Math.Round(y_o - 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[2];
                            ie = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o - 0.5) * nChannel)[2];
                            id = (dataPtr + (int)Math.Round(y_o + 0.5) * widthstep + (int)Math.Round(x_o + 0.5) * nChannel)[2];

                            inter = Math.Round((1 - offsetY) * ((1 - offsetX) * se + offsetX * sd) + offsetY * ((1 - offsetX) * ie + offsetX * id));
                            red = (byte)inter;

                            dataPtr_d[0] = blue;
                            dataPtr_d[1] = green;
                            dataPtr_d[2] = red;
                        }
                        dataPtr_d += nChannel;
                    }
                    dataPtr_d += padding;
                }
            }
        }

        /*
          
         * Nivel 1    XXXXXXXXXXX MATADO XXXXXXXXXXX
        1º recortar quadradoos
        2º binarizacao nao pelo hue mas value a < 0.2
        3º funcao de comparacao a base de dados, Array de 12 diferencas escolher a min

         * Nivel 2    ************* Falta ***************
        1º verificar angulo
        2º rodar imagem se torta

         * Nivel 3    ************* Falta ***************
        1º Testar binarização e melhorar transformação de imagens

        Falta histograma de cada peca binarizada e recorte so depois fazer resiize (na nossa imagem e na base de dados)
        histograma se < 20% entao nao tem peca se + 20% faz comparacao(ver melhor a %)
        Comparacao: 
            -pixel a pixel (na bibarizacao 255 ou 0) somar as diferencas.

        Duvidas:
            -path da base de dados
            -comparacao das pecas
            -botao de input da imagem
            -resize
            -tempo de execucao de encontrar as pecas

        1 valor(conjunto com LP1:
            -Relacao com 2/3 grupos
            -Explicar 2 filtros aos grupos para programarem em C (nada de código apenas teoria)
                -Filtro negativo
                -Filtro de Média
            -Ficheiro PDF 

        Duvidas 2:
            -Erro size com as duas fotos nivel 1 com borda (é preciso aplicar fiiltros?)  JA TA
            -angulo para passar para o nivel 2  (ver pela linha da imagem nao pelo histX)  JA TA
            -como desenhar retangulo nas casas das pecas;  drraw line
            -imagens tipo creditos ou fundos;
         
        Duvidas 3:
            -Recorta ta a verificar os quadrados brancos e recortar pelo histograma (se tem lixo vai recortar o lixo) e da erro !!!   (JA TA)
            -Proposta de solução filtro de erosao e dilatacao para diminuir ruido ?! ou qq outro filtro (J FIZ EROSAO E DILATACAO MAS NAO ELIMINOU ERROS)

        Duvidas 4:
            -Erro no 2º imagem nvel 2 (rotacao): 1ª coluna não fica bem.
            -Histograma a vver se é digital;

        Contraste 2x -> Tabuleiro -> Tabuleiro -> Pecas

        */

        // RGB TO HSV //
        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));
            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        // BINARIZACAO TABULEIRO //
        public static void ConvertToBW_HSV(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                int width = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int widthstep = m.widthStep;
                int padding = m.widthStep - m.nChannels * m.width;
                double hue, saturation, value;
                Color original;


                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        original = Color.FromArgb(dataPtr[2], dataPtr[1], dataPtr[0]);
                        ColorToHSV(original, out hue, out saturation, out value);

                        if (hue < 240 & hue > 170 && saturation > 0.2 && value > 0.2/*&& saturation > 0.125 && value > 0.6*/)
                        {
                            dataPtr[0] = dataPtr[1] = dataPtr[2] = 255;
                        }
                        else
                        {
                            dataPtr[0] = dataPtr[1] = dataPtr[2] = 0;
                        }

                        dataPtr += nChannel;
                    }
                    dataPtr += padding;
                }
            }
        }

        // TESTE //
        public static Emgu.CV.Image<Bgr, byte> recortaImg(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                int width = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int widthStep = m.widthStep;
                double[] hist_y = new double[height];
                double[] hist_x = new double[width];

                double threshold = 2.5;

                for (int y = 0; y < height; y++)
                {
                    int red, green, blue;
                    for (int x = 0; x < width; x++)
                    {
                        red = (byte)(dataPtr + (nChannel * (x)) + widthStep * (y))[0];
                        green = (byte)(dataPtr + (nChannel * (x)) + widthStep * (y))[1];
                        blue = (byte)(dataPtr + (nChannel * (x)) + widthStep * (y))[2];

                        if (blue == 255 && red == 255 && green == 255)
                        {
                            hist_y[y]++;
                            hist_x[x]++;
                        }
                    }
                }

                double totalPixels = width * height;

                for (int i = 0; i < height; i++)
                {
                    hist_y[i] = 100.0 * (hist_y[i] / width);
                }

                for (int i = 0; i < width; i++)
                {
                    hist_x[i] = 100.0 * (hist_x[i] / height);
                }

                int index_y_t = 0;
                for (int y = 0; y < height; y++)
                {
                    if (hist_y[y] > threshold)
                    {
                        index_y_t = y;
                        break;
                    }
                }

                int index_y_b = height - 1;
                for (int y = height - 1; y >= 0; y--)
                {
                    if (hist_y[y] > threshold)
                    {
                        index_y_b = y;
                        break;
                    }
                }

                int index_x_l = 0;
                for (int x = 0; x < width; x++)
                {
                    if (hist_x[x] > threshold)
                    {
                        index_x_l = x;
                        break;
                    }
                }

                int index_x_r = width - 1;
                for (int x = width - 1; x >= 0; x--)
                {
                    if (hist_x[x] > threshold)
                    {
                        index_x_r = x;
                        break;
                    }
                }

                Rectangle recorte = new Rectangle(index_x_l, index_y_t, index_x_r - index_x_l, index_y_b - index_y_t);
                Emgu.CV.Image<Bgr, byte> recortada = img.Copy(recorte);
                return recortada;
            }
        }

        // VERIFICA ANGULO E RODA //
        public static int roda(Emgu.CV.Image<Bgr, byte> img, Emgu.CV.Image<Bgr, byte> imgUndo, int rodou)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                int width = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int widthStep = m.widthStep;
                double[] hist_y = new double[height];
                double[] hist_x = new double[width];

                double threshold = 1.5;


                for (int y = 0; y < height; y++)
                {
                    int red, green, blue;
                    for (int x = 0; x < width; x++)
                    {
                        red = (byte)(dataPtr + (nChannel * (x)) + widthStep * (y))[0];
                        green = (byte)(dataPtr + (nChannel * (x)) + widthStep * (y))[1];
                        blue = (byte)(dataPtr + (nChannel * (x)) + widthStep * (y))[2];

                        if (blue == 255 && red == 255 && green == 255)
                        {
                            hist_y[y]++;
                            hist_x[x]++;
                        }
                    }
                }

                double totalPixels = width * height;

                for (int i = 0; i < height; i++)
                {
                    hist_y[i] = 100.0 * (hist_y[i] / width);
                }

                for (int i = 0; i < width; i++)
                {
                    hist_x[i] = 100.0 * (hist_x[i] / height);
                }

                int index_y_t = 0;
                for (int y = 0; y < height; y++)
                {
                    if (hist_y[y] > threshold)
                    {
                        index_y_t = y;
                        break;
                    }
                }

                int index_y_b = height - 1;
                for (int y = height - 1; y >= 0; y--)
                {
                    if (hist_y[y] > threshold)
                    {
                        index_y_b = y;
                        break;
                    }
                }

                int x_l = 0;
                for (int x = 0; x < width; x++)
                {
                    if (img[index_y_b, x].Blue == 255 && img[index_y_b, x].Green == 255 && img[index_y_b, x].Red == 255)
                    {
                        x_l = x;
                        break;
                    }

                }

                int x_r = width - 1;
                for (int x = width - 1; x >= 0; x--)
                {
                    if (img[index_y_t, x].Blue == 255 && img[index_y_t, x].Green == 255 && img[index_y_t, x].Red == 255)
                    {
                        x_r = x;
                        break;
                    }

                }
                /*
                 double dx = Math.Abs(x_r - x_l);
                double dy = Math.Abs(index_y_b - index_y_t);

                double angulo = Math.Atan(dy / dx) * (180.0 / Math.PI);

                if (Math.Abs(angulo - 45.0) > 1.0)
                {
                    double ang = (angulo - 45.0) * (Math.PI / 180.0);
                    // Rotacionar a imagem
                    Rotacao_Fundo_Branco(img, img.Copy(), ang);
                    Rotacao_Fundo_Branco(imgUndo, imgUndo.Copy(), ang);
                }
                 */


                double dx = Math.Abs(x_r - x_l);
                double dy = Math.Abs(index_y_b - index_y_t);

                double angulo = Math.Atan(dy / dx) * (180.0 / Math.PI);

                if (x_l - x_r > 200.0 || x_l - x_r < -250.0)
                {
                    if (Math.Abs(angulo - 45.0) > 1.0)
                    {
                        double ang = (angulo - 45.0) * (Math.PI / 180.0);

                        if (x_l - x_r > 0) // Se dx for positivo, a imagem provavelmente está inclinada para a direita
                        {
                            // Rodar a imagem para a esquerda
                            Rotacao_Fundo_Branco(img, img.Copy(), -ang);
                            Rotacao_Fundo_Branco(imgUndo, imgUndo.Copy(), -ang);
                        }
                        else // Se dx for negativo, a imagem provavelmente está inclinada para a esquerda
                        {
                            // Rodar a imagem para a direita
                            Rotacao_Fundo_Branco(img, img.Copy(), ang);
                            Rotacao_Fundo_Branco(imgUndo, imgUndo.Copy(), ang);
                        }

                        rodou = 1;
                    }
                }

                return rodou;
            }
        }

        // RECORTE DE IMG BINARIZADA (RECORTA PELO BRANCO) //
        public static Emgu.CV.Image<Bgr, byte> recortaImg(Emgu.CV.Image<Bgr, byte> img, Emgu.CV.Image<Bgr, byte> imgUndo)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                int width = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int widthStep = m.widthStep;
                double[] hist_y = new double[height];
                double[] hist_x = new double[width];

                double threshold = 1.5;

                for (int y = 0; y < height; y++)
                {
                    int red, green, blue;
                    for (int x = 0; x < width; x++)
                    {
                        red = (byte)(dataPtr + (nChannel * (x)) + widthStep * (y))[0];
                        green = (byte)(dataPtr + (nChannel * (x)) + widthStep * (y))[1];
                        blue = (byte)(dataPtr + (nChannel * (x)) + widthStep * (y))[2];

                        if (blue == 255 && red == 255 && green == 255)
                        {
                            hist_y[y]++;
                            hist_x[x]++;
                        }
                    }
                }

                double totalPixels = width * height;

                for (int i = 0; i < height; i++)
                {
                    hist_y[i] = 100.0 * (hist_y[i] / width);
                }

                for (int i = 0; i < width; i++)
                {
                    hist_x[i] = 100.0 * (hist_x[i] / height);
                }

                int index_y_t = 0;
                for (int y = 0; y < height; y++)
                {
                    if (hist_y[y] > threshold)
                    {
                        index_y_t = y;
                        break;
                    }
                }

                int index_y_b = height - 1;
                for (int y = height - 1; y >= 0; y--)
                {
                    if (hist_y[y] > threshold)
                    {
                        index_y_b = y;
                        break;
                    }
                }

                int index_x_l = 0;
                for (int x = 0; x < width; x++)
                {
                    if (hist_x[x] > threshold)
                    {
                        index_x_l = x;
                        break;
                    }
                }

                int index_x_r = width - 1;
                for (int x = width - 1; x >= 0; x--)
                {
                    if (hist_x[x] > threshold)
                    {
                        index_x_r = x;
                        break;
                    }
                }

                Rectangle recorte = new Rectangle(index_x_l, index_y_t, index_x_r - index_x_l, index_y_b - index_y_t);
                Emgu.CV.Image<Bgr, byte> recortada = imgUndo.Copy(recorte);

                return recortada;

            }
        }

        // TESTE //
        public static Image<Bgr, byte> recorta1quadrado(Emgu.CV.Image<Bgr, byte> img, Emgu.CV.Image<Bgr, byte> imgUndo)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                int width = img.Width;
                int height = img.Height;

                int largura = width / 8;
                int altura = height / 8;

                int x = 0 * largura;
                int y = 0 * altura;

                Rectangle recorte = new Rectangle(x, y, largura, altura);
                Emgu.CV.Image<Bgr, byte> recortada = imgUndo.Copy(recorte);

                return recortada;
            }
        }

        // BINARIZACAO PECA //
        public static void ConvertPieceToBW_HSV(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                int width = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int widthstep = m.widthStep;
                int padding = m.widthStep - m.nChannels * m.width;
                double hue, saturation, value;
                Color original;


                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        original = Color.FromArgb(dataPtr[2], dataPtr[1], dataPtr[0]);
                        ColorToHSV(original, out hue, out saturation, out value);

                        if (value < 0.4)
                        {
                            dataPtr[0] = dataPtr[1] = dataPtr[2] = 255;
                        }
                        else
                        {
                            dataPtr[0] = dataPtr[1] = dataPtr[2] = 0;
                        }

                        dataPtr += nChannel;
                    }
                    dataPtr += padding;
                }
            }
        }

        // COMPARACAO COM BD //
        public static string tratamentoBaseDados(Emgu.CV.Image<Bgr, byte> img)
        {
            string[] Base_Dados = Directory.GetFiles(@"");
            int aux = Base_Dados.Length;

            Image<Bgr, byte> imgCopy = img.Copy();
            Image<Bgr, byte> img_BD;

            double[] relacoes = new double[aux];

            BrightContrast(imgCopy, -80, 1.7);

            ConvertPieceToBW_HSV(imgCopy);

            if (vefCasaVazia(imgCopy) < 2.5)
            {
                return "VAZIA";
            }

            imgCopy = corta_aux(imgCopy);

            imgCopy = recortaImg(imgCopy);

            for (int B_D = 0; B_D < aux; B_D++)
            {
                img_BD = new Image<Bgr, byte>(Base_Dados[B_D]);

                Image<Bgr, byte> img_BDCopy = img_BD.Copy();

                ConvertPieceToBW_HSV(img_BDCopy);

                img_BDCopy = recortaImg(img_BDCopy);

                relacoes[B_D] = comparaPecas(imgCopy, img_BDCopy);
            }

            double Min = relacoes.Min();
            int index = Array.IndexOf(relacoes, Min);
            string path = Base_Dados[index];
            string result = Path.GetFileNameWithoutExtension(path);

            return result;
        }

        // VERIFICA SE TEM PECA //
        public static double vefCasaVazia(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                int width = img.Width;
                int height = img.Height;
                int nChannel = m.nChannels;
                int widthStep = m.widthStep;
                double brancos = 0.0;

                for (int y = 0; y < height; y++)
                {
                    int red, green, blue;
                    for (int x = 0; x < width; x++)
                    {
                        red = (byte)(dataPtr + (nChannel * (x)) + widthStep * (y))[0];
                        green = (byte)(dataPtr + (nChannel * (x)) + widthStep * (y))[1];
                        blue = (byte)(dataPtr + (nChannel * (x)) + widthStep * (y))[2];

                        if (blue == 255 && red == 255 && green == 255)
                        {
                            brancos++;
                        }
                    }
                }

                double totalPixels = width * height;

                brancos = (brancos / totalPixels) * 100;

                return brancos;
            }
        }

        // COMPARAR 2 PECAS //
        public static double comparaPecas(Image<Bgr, byte> img1, Image<Bgr, byte> img2)
        {
            unsafe
            {
                Size newSize = new Size(12, 24);
                img1 = img1.Resize(newSize.Width, newSize.Height, INTER.CV_INTER_LINEAR);
                img2 = img2.Resize(newSize.Width, newSize.Height, INTER.CV_INTER_LINEAR);

                MIplImage m = img1.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage m2 = img2.MIplImage;
                byte* dataPtr2 = (byte*)m2.imageData.ToPointer();

                int width = img1.Width;
                int height = img1.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;

                int x, y;
                double total = width * height;
                double diferenca = 0;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        diferenca += Math.Abs(dataPtr[0] - dataPtr2[0]);

                        dataPtr += nChan;
                        dataPtr2 += nChan;
                    }
                    dataPtr += padding;
                    dataPtr2 += padding;
                }

                return diferenca;
            }
        }

        public static double comparaTabuleiros(Image<Bgr, byte> img1, Image<Bgr, byte> img2)
        {
            unsafe
            {
                Size newSize = new Size(500, 300);
                img1 = img1.Resize(newSize.Width, newSize.Height, INTER.CV_INTER_LINEAR);
                img2 = img2.Resize(newSize.Width, newSize.Height, INTER.CV_INTER_LINEAR);

                MIplImage m = img1.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage m2 = img2.MIplImage;
                byte* dataPtr2 = (byte*)m2.imageData.ToPointer();

                int width = img1.Width;
                int height = img1.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;

                int x, y;
                double total = width * height;
                double diferenca = 0;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        diferenca += Math.Abs(dataPtr[0] - dataPtr2[0]);

                        dataPtr += nChan;
                        dataPtr2 += nChan;
                    }
                    dataPtr += padding;
                    dataPtr2 += padding;
                }

                return diferenca;
            }
        }

        // PERCORRE TABULEIRO, CORTA PECAS E COMPARA //
        public static string[][] RegistaPecas(Emgu.CV.Image<Bgr, byte> img, Emgu.CV.Image<Bgr, byte> imgUndo)
        {
            unsafe
            {
                BrightContrast(imgUndo, -80, 1.6);
                //BrightContrast(imgUndo, -80, 1.7);

                MIplImage m = img.MIplImage;
                int width = img.Width;
                int height = img.Height;

                string[][] tabuleiro = new string[8][];

                int largura = width / 8;
                int altura = height / 8;

                int x, y;

                for (int i_y = 0; i_y < 8; i_y++)
                {
                    tabuleiro[i_y] = new string[8];
                    for (int i_x = 0; i_x < 8; i_x++)
                    {
                        x = i_x * largura;
                        y = i_y * altura;

                        Rectangle recorte = new Rectangle(x, y, largura, altura);
                        Emgu.CV.Image<Bgr, byte> recortada = imgUndo.Copy(recorte);

                        tabuleiro[i_y][i_x] = tratamentoBaseDados(recortada);
                    }
                }

                return tabuleiro;
            }
        }

        // FUNCOES DE MODIFICACAO IMAGEM //
        public static void dilatacao(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtringCopy = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrImg = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthstep = mUndo.widthStep;
                int padding = mUndo.widthStep - mUndo.nChannels * mUndo.width;

                int last_h = height - 1;
                int last_w = width - 1;

                //canto superior esquerdo
                if (dataPtringCopy[0] == 255 || (dataPtringCopy + nChan)[0] == 255 || (dataPtringCopy + m.widthStep)[0] == 255 || (dataPtringCopy + nChan + m.widthStep)[0] == 255)
                {
                    dataPtrImg[1] = 255;
                    dataPtrImg[2] = 255;
                    dataPtrImg[0] = 255;
                }

                dataPtrImg += nChan;
                dataPtringCopy += nChan;

                //linha de cima
                for (int x = 1; x < last_w; x++)
                {
                    if ((dataPtringCopy - nChan)[0] == 255 || (dataPtringCopy - nChan + m.widthStep)[0] == 255 || dataPtringCopy[0] == 255 || (dataPtringCopy + nChan)[0] == 255 || (dataPtringCopy + m.widthStep)[0] == 255 || (dataPtringCopy + nChan + m.widthStep)[0] == 255)
                    {
                        dataPtrImg[1] = 255;
                        dataPtrImg[2] = 255;
                        dataPtrImg[0] = 255;
                    }

                    dataPtrImg += nChan;
                    dataPtringCopy += nChan;
                }

                //canto superior direito
                if ((dataPtringCopy - nChan)[0] == 255 || (dataPtringCopy - nChan + m.widthStep)[0] == 255 || dataPtringCopy[0] == 255 || (dataPtringCopy + m.widthStep)[0] == 255)
                {
                    dataPtrImg[1] = 255;
                    dataPtrImg[2] = 255;
                    dataPtrImg[0] = 255;
                }

                dataPtrImg += nChan;
                dataPtringCopy += nChan;
                dataPtrImg += padding;
                dataPtringCopy += padding;


                //Percorre imagem
                for (int y = 1; y < last_h; ++y)
                {
                    //pixel esquerdo da linha
                    if ((dataPtringCopy + nChan)[0] == 255 || (dataPtringCopy + m.widthStep)[0] == 255 || (dataPtringCopy + nChan - m.widthStep)[0] == 255 || dataPtringCopy[0] == 255 || (dataPtringCopy + nChan)[0] == 255 || (dataPtringCopy - m.widthStep)[0] == 255 || (dataPtringCopy + nChan + m.widthStep)[0] == 255)
                    {
                        dataPtrImg[1] = 255;
                        dataPtrImg[2] = 255;
                        dataPtrImg[0] = 255;
                    }

                    dataPtrImg += nChan;
                    dataPtringCopy += nChan;

                    for (int x = 1; x < last_w; x++)
                    {
                        if ((dataPtringCopy - nChan - m.widthStep)[0] == 255 || (dataPtringCopy - m.widthStep)[0] == 255 || (dataPtringCopy + nChan - m.widthStep)[0] == 255 || (dataPtringCopy - nChan)[0] == 255 || dataPtringCopy[0] == 255 || (dataPtringCopy + nChan)[0] == 255 || (dataPtringCopy - nChan + m.widthStep)[0] == 255 || (dataPtringCopy + m.widthStep)[0] == 255 || (dataPtringCopy + nChan + m.widthStep)[0] == 255)
                        {
                            dataPtrImg[1] = 255;
                            dataPtrImg[2] = 255;
                            dataPtrImg[0] = 255;
                        }

                        dataPtrImg += nChan;
                        dataPtringCopy += nChan;
                    }

                    //pixel direito da linha
                    if ((dataPtringCopy - nChan - m.widthStep)[0] == 255 || (dataPtringCopy - m.widthStep)[0] == 255 || (dataPtringCopy - nChan)[0] == 255 || dataPtringCopy[0] == 255 || (dataPtringCopy - nChan + m.widthStep)[0] == 255 || (dataPtringCopy + m.widthStep)[0] == 255)
                    {
                        dataPtrImg[1] = 255;
                        dataPtrImg[2] = 255;
                        dataPtrImg[0] = 255;
                    }

                    dataPtrImg += nChan;
                    dataPtringCopy += nChan;
                    dataPtrImg += padding;
                    dataPtringCopy += padding;

                }

                //canto inferior esquerdo
                if ((dataPtringCopy - m.widthStep)[0] == 255 || (dataPtringCopy + nChan - m.widthStep)[0] == 255 || dataPtringCopy[0] == 255 || (dataPtringCopy + nChan)[0] == 255)
                {
                    dataPtrImg[1] = 255;
                    dataPtrImg[2] = 255;
                    dataPtrImg[0] = 255;
                }

                dataPtrImg += nChan;
                dataPtringCopy += nChan;


                //linha de baixo
                for (int x = 1; x < last_w; x++)
                {
                    if ((dataPtringCopy - nChan - m.widthStep)[0] == 255 || (dataPtringCopy - m.widthStep)[0] == 255 || (dataPtringCopy + nChan - m.widthStep)[0] == 255 || (dataPtringCopy - nChan)[0] == 255 || dataPtringCopy[0] == 255 || (dataPtringCopy + nChan)[0] == 255)
                    {
                        dataPtrImg[1] = 255;
                        dataPtrImg[2] = 255;
                        dataPtrImg[0] = 255;
                    }

                    dataPtrImg += nChan;
                    dataPtringCopy += nChan;
                }


                //canto infeiror  direito
                if ((dataPtringCopy - nChan - m.widthStep)[0] == 255 || (dataPtringCopy - m.widthStep)[0] == 255 || (dataPtringCopy - nChan)[0] == 255 || dataPtringCopy[0] == 255)
                {
                    dataPtrImg[1] = 255;
                    dataPtrImg[2] = 255;
                    dataPtrImg[0] = 255;
                }
            }
        }

        public static void erosao(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtringCopy = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrImg = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthstep = mUndo.widthStep;
                int padding = mUndo.widthStep - mUndo.nChannels * mUndo.width;

                int last_h = height - 1;
                int last_w = width - 1;

                //canto superior esquerdo
                if (dataPtringCopy[0] == 255 && (dataPtringCopy + nChan)[0] == 255 && (dataPtringCopy + m.widthStep)[0] == 255 && (dataPtringCopy + nChan + m.widthStep)[0] == 255)
                {
                    dataPtrImg[1] = 255;
                    dataPtrImg[2] = 255;
                    dataPtrImg[0] = 255;
                }
                else
                {
                    dataPtrImg[1] = 0;
                    dataPtrImg[2] = 0;
                    dataPtrImg[0] = 0;
                }

                dataPtrImg += nChan;
                dataPtringCopy += nChan;

                //linha de cima
                for (int x = 1; x < last_w; x++)
                {
                    if ((dataPtringCopy - nChan)[0] == 255 && (dataPtringCopy - nChan + m.widthStep)[0] == 255 && dataPtringCopy[0] == 255 && (dataPtringCopy + nChan)[0] == 255 && (dataPtringCopy + m.widthStep)[0] == 255 && (dataPtringCopy + nChan + m.widthStep)[0] == 255)
                    {
                        dataPtrImg[1] = 255;
                        dataPtrImg[2] = 255;
                        dataPtrImg[0] = 255;
                    }
                    else
                    {
                        dataPtrImg[1] = 0;
                        dataPtrImg[2] = 0;
                        dataPtrImg[0] = 0;
                    }

                    dataPtrImg += nChan;
                    dataPtringCopy += nChan;
                }

                //canto superior direito
                if ((dataPtringCopy - nChan)[0] == 255 && (dataPtringCopy - nChan + m.widthStep)[0] == 255 && dataPtringCopy[0] == 255 && (dataPtringCopy + m.widthStep)[0] == 255)
                {
                    dataPtrImg[1] = 255;
                    dataPtrImg[2] = 255;
                    dataPtrImg[0] = 255;
                }
                else
                {
                    dataPtrImg[1] = 0;
                    dataPtrImg[2] = 0;
                    dataPtrImg[0] = 0;
                }

                dataPtrImg += nChan;
                dataPtringCopy += nChan;
                dataPtrImg += padding;
                dataPtringCopy += padding;


                //Percorre imagem
                for (int y = 1; y < last_h; ++y)
                {
                    //pixel esquerdo da linha
                    if ((dataPtringCopy + nChan)[0] == 255 && (dataPtringCopy + m.widthStep)[0] == 255 && (dataPtringCopy + nChan - m.widthStep)[0] == 255 && dataPtringCopy[0] == 255 && (dataPtringCopy + nChan)[0] == 255 && (dataPtringCopy - m.widthStep)[0] == 255 && (dataPtringCopy + nChan + m.widthStep)[0] == 255)
                    {
                        dataPtrImg[1] = 255;
                        dataPtrImg[2] = 255;
                        dataPtrImg[0] = 255;
                    }
                    else
                    {
                        dataPtrImg[1] = 0;
                        dataPtrImg[2] = 0;
                        dataPtrImg[0] = 0;
                    }

                    dataPtrImg += nChan;
                    dataPtringCopy += nChan;

                    for (int x = 1; x < last_w; x++)
                    {
                        if ((dataPtringCopy - nChan - m.widthStep)[0] == 255 && (dataPtringCopy - m.widthStep)[0] == 255 && (dataPtringCopy + nChan - m.widthStep)[0] == 255 && (dataPtringCopy - nChan)[0] == 255 && dataPtringCopy[0] == 255 && (dataPtringCopy + nChan)[0] == 255 && (dataPtringCopy - nChan + m.widthStep)[0] == 255 && (dataPtringCopy + m.widthStep)[0] == 255 && (dataPtringCopy + nChan + m.widthStep)[0] == 255)
                        {
                            dataPtrImg[1] = 255;
                            dataPtrImg[2] = 255;
                            dataPtrImg[0] = 255;
                        }
                        else
                        {
                            dataPtrImg[1] = 0;
                            dataPtrImg[2] = 0;
                            dataPtrImg[0] = 0;
                        }

                        dataPtrImg += nChan;
                        dataPtringCopy += nChan;
                    }

                    //pixel direito da linha
                    if ((dataPtringCopy - nChan - m.widthStep)[0] == 255 && (dataPtringCopy - m.widthStep)[0] == 255 && (dataPtringCopy - nChan)[0] == 255 && dataPtringCopy[0] == 255 && (dataPtringCopy - nChan + m.widthStep)[0] == 255 && (dataPtringCopy + m.widthStep)[0] == 255)
                    {
                        dataPtrImg[1] = 255;
                        dataPtrImg[2] = 255;
                        dataPtrImg[0] = 255;
                    }
                    else
                    {
                        dataPtrImg[1] = 0;
                        dataPtrImg[2] = 0;
                        dataPtrImg[0] = 0;
                    }

                    dataPtrImg += nChan;
                    dataPtringCopy += nChan;
                    dataPtrImg += padding;
                    dataPtringCopy += padding;

                }

                //canto inferior esquerdo
                if ((dataPtringCopy - m.widthStep)[0] == 255 && (dataPtringCopy + nChan - m.widthStep)[0] == 255 && dataPtringCopy[0] == 255 && (dataPtringCopy + nChan)[0] == 255)
                {
                    dataPtrImg[1] = 255;
                    dataPtrImg[2] = 255;
                    dataPtrImg[0] = 255;
                }
                else
                {
                    dataPtrImg[1] = 0;
                    dataPtrImg[2] = 0;
                    dataPtrImg[0] = 0;
                }

                dataPtrImg += nChan;
                dataPtringCopy += nChan;


                //linha de baixo
                for (int x = 1; x < last_w; x++)
                {
                    if ((dataPtringCopy - nChan - m.widthStep)[0] == 255 && (dataPtringCopy - m.widthStep)[0] == 255 && (dataPtringCopy + nChan - m.widthStep)[0] == 255 && (dataPtringCopy - nChan)[0] == 255 && dataPtringCopy[0] == 255 && (dataPtringCopy + nChan)[0] == 255)
                    {
                        dataPtrImg[1] = 255;
                        dataPtrImg[2] = 255;
                        dataPtrImg[0] = 255;
                    }
                    else
                    {
                        dataPtrImg[1] = 0;
                        dataPtrImg[2] = 0;
                        dataPtrImg[0] = 0;
                    }

                    dataPtrImg += nChan;
                    dataPtringCopy += nChan;
                }


                //canto infeiror  direito
                if ((dataPtringCopy - nChan - m.widthStep)[0] == 255 && (dataPtringCopy - m.widthStep)[0] == 255 && (dataPtringCopy - nChan)[0] == 255 && dataPtringCopy[0] == 255)
                {
                    dataPtrImg[1] = 255;
                    dataPtrImg[2] = 255;
                    dataPtrImg[0] = 255;
                }
                else
                {
                    dataPtrImg[1] = 0;
                    dataPtrImg[2] = 0;
                    dataPtrImg[0] = 0;
                }
            }
        }

        public static void erosaoX(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtringCopy = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrImg = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthstep = mUndo.widthStep;
                int padding = mUndo.widthStep - mUndo.nChannels * mUndo.width;

                int last_h = height - 1;
                int last_w = width - 1;

                //canto superior esquerdo

                dataPtrImg += nChan;
                dataPtringCopy += nChan;

                //linha de cima
                for (int x = 1; x < last_w; x++)
                {
                    dataPtrImg += nChan;
                    dataPtringCopy += nChan;
                }

                //canto superior direito

                dataPtrImg += nChan;
                dataPtringCopy += nChan;
                dataPtrImg += padding;
                dataPtringCopy += padding;


                //Percorre imagem
                for (int y = 1; y < last_h; ++y)
                {
                    //pixel esquerdo da linha

                    dataPtrImg += nChan;
                    dataPtringCopy += nChan;

                    for (int x = 1; x < last_w; x++)
                    {
                        if ((dataPtringCopy - nChan - m.widthStep)[0] == 0 && (dataPtringCopy + nChan - m.widthStep)[0] == 0 && dataPtringCopy[0] == 255 && (dataPtringCopy - nChan + m.widthStep)[0] == 0 && (dataPtringCopy + nChan + m.widthStep)[0] == 0)
                        {
                            dataPtrImg[1] = 0;
                            dataPtrImg[2] = 0;
                            dataPtrImg[0] = 0;
                        }

                        dataPtrImg += nChan;
                        dataPtringCopy += nChan;
                    }
                }
            }
        }

        public static void limpa(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtringCopy = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtrImg = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int widthstep = mUndo.widthStep;
                int padding = mUndo.widthStep - mUndo.nChannels * mUndo.width;

                int last_h = height - 1;
                int last_w = width - 1;

                //canto superior esquerdo
                dataPtrImg[1] = 0;
                dataPtrImg[2] = 0;
                dataPtrImg[0] = 0;


                dataPtrImg += nChan;
                dataPtringCopy += nChan;

                //linha de cima
                for (int x = 1; x < last_w; x++)
                {
                    dataPtrImg[1] = 0;
                    dataPtrImg[2] = 0;
                    dataPtrImg[0] = 0;

                    dataPtrImg += nChan;
                    dataPtringCopy += nChan;
                }

                //canto superior direito
                dataPtrImg[1] = 0;
                dataPtrImg[2] = 0;
                dataPtrImg[0] = 0;

                dataPtrImg += nChan;
                dataPtringCopy += nChan;
                dataPtrImg += padding;
                dataPtringCopy += padding;


                //Percorre imagem
                for (int y = 1; y < last_h; ++y)
                {
                    //pixel esquerdo da linha
                    dataPtrImg[1] = 0;
                    dataPtrImg[2] = 0;
                    dataPtrImg[0] = 0;

                    dataPtrImg += nChan;
                    dataPtringCopy += nChan;

                    for (int x = 1; x < last_w; x++)
                    {
                        dataPtrImg += nChan;
                        dataPtringCopy += nChan;
                    }

                    //pixel direito da linha
                    dataPtrImg[1] = 0;
                    dataPtrImg[2] = 0;
                    dataPtrImg[0] = 0;

                    dataPtrImg += nChan;
                    dataPtringCopy += nChan;
                    dataPtrImg += padding;
                    dataPtringCopy += padding;

                }

                //canto inferior esquerdo
                dataPtrImg[1] = 0;
                dataPtrImg[2] = 0;
                dataPtrImg[0] = 0;

                dataPtrImg += nChan;
                dataPtringCopy += nChan;


                //linha de baixo
                for (int x = 1; x < last_w; x++)
                {
                    dataPtrImg[1] = 0;
                    dataPtrImg[2] = 0;
                    dataPtrImg[0] = 0;

                    dataPtrImg += nChan;
                    dataPtringCopy += nChan;
                }


                //canto infeiror  direito
                dataPtrImg[1] = 0;
                dataPtrImg[2] = 0;
                dataPtrImg[0] = 0;
            }
        }

        public static void testeN3(Image<Bgr, byte> img)
        {
            for (int i = 0; i < 11; i++)
            {
                erosao(img, img.Copy());
            }
            for (int i = 0; i < 10; i++)
            {
                dilatacao(img, img.Copy());
            }

        }

        public static void Rotacao_Fundo_Branco(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, double angle)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                MIplImage mUndo = imgCopy.MIplImage;

                byte* dataPtrCopy = (byte*)mUndo.imageData.ToPointer();
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = imgCopy.Width;
                int height = imgCopy.Height;
                int nChan = mUndo.nChannels;
                int padding = mUndo.widthStep - mUndo.nChannels * mUndo.width;
                int widthstep = mUndo.widthStep;

                byte red, green, blue;
                int x_o, y_o;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        x_o = (int)Math.Round((x - width / 2.0) * Math.Cos(angle) - (height / 2.0 - y) * Math.Sin(angle) + width / 2.0);
                        y_o = (int)Math.Round(height / 2.0 - (x - width / 2.0) * Math.Sin(angle) - (height / 2.0 - y) * Math.Cos(angle));

                        if (x_o < width && x_o >= 0 && y_o < height && y_o >= 0)
                        {
                            blue = (dataPtrCopy + y_o * widthstep + x_o * nChan)[0];
                            green = (dataPtrCopy + y_o * widthstep + x_o * nChan)[1];
                            red = (dataPtrCopy + y_o * widthstep + x_o * nChan)[2];
                        }
                        else
                        {
                            red = green = blue = 200;
                        }

                        (dataPtr + y * widthstep + x * nChan)[0] = blue;
                        (dataPtr + y * widthstep + x * nChan)[1] = green;
                        (dataPtr + y * widthstep + x * nChan)[2] = red;
                    }
                }
            }
        }

        public static Emgu.CV.Image<Bgr, byte> corta_aux(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                int width = img.Width;
                int height = img.Height;

                int largura = width / 20; //20
                int altura = height / 20;

                int x = largura;
                int y = altura;
                int newWidth = width - 2 * largura;
                int newHeight = height - 2 * altura;

                Rectangle recorte = new Rectangle(x, y, newWidth, newHeight);
                Emgu.CV.Image<Bgr, byte> recortada = img.Copy(recorte);

                return recortada;
            }
        }

        public static Emgu.CV.Image<Bgr, byte> corta_aux_2(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                int width = img.Width;
                int height = img.Height;

                int largura = width / 100;
                int altura = height / 100;

                int x = largura;
                int y = altura;
                int newWidth = width - 2 * largura;
                int newHeight = height - 2 * altura;

                Rectangle recorte = new Rectangle(x, y, newWidth, newHeight);
                Emgu.CV.Image<Bgr, byte> recortada = img.Copy(recorte);

                return recortada;
            }
        }

        public static bool vefUniform(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                string[] Base_Dados = Directory.GetFiles(@"");
                int aux = Base_Dados.Length;

                Image<Bgr, byte> imgCopy = img.Copy();
                Image<Bgr, byte> img_BD;

                double[] relacoes = new double[aux];

                ConvertPieceToBW_HSV(imgCopy);

                recortaImg(imgCopy);

                for (int B_D = 0; B_D < aux; B_D++)
                {
                    img_BD = new Image<Bgr, byte>(Base_Dados[B_D]);

                    Image<Bgr, byte> img_BDCopy = img_BD.Copy();

                    ConvertPieceToBW_HSV(img_BDCopy);

                    recortaImg(img_BDCopy);

                    relacoes[B_D] = comparaPecas(imgCopy, img_BDCopy);
                }

                double Min = relacoes.Min();
                int index = Array.IndexOf(relacoes, Min);
                string path = Base_Dados[index];
                string result = Path.GetFileNameWithoutExtension(path);

                if (result == "uniform")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

        }
    }
}
