using System.Drawing;
using System.Drawing.Imaging;

namespace ImageRecognition_Final_Project.Program
{
    internal class MyImageManager
    {
        public Bitmap oriImage;
        public Bitmap watermarkImage;
        public Bitmap proImage;
        private double[,] real;
        private double[,] imag;

        public MyImageManager()
        {
            oriImage = new Bitmap(1, 1);
            watermarkImage = new Bitmap(1, 1);
            proImage = new Bitmap(1, 1);
            real = new double[1, 1]; // 初始化 real 欄位
            imag = new double[1, 1]; // 初始化 imag 欄位
        }

        public Bitmap AddWatermark(ColorMatrix colorMatrix)
        {
            // 創建一個新的 Bitmap 來存儲合成後的圖片
            Bitmap proImage = new Bitmap(oriImage.Width, oriImage.Height);

            using (Graphics graphics = Graphics.FromImage(proImage))
            {
                graphics.DrawImage(oriImage, new Rectangle(0, 0, oriImage.Width, oriImage.Height));

                // 設置浮水印的透明度
                ImageAttributes imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                // 計算浮水印的目標大小
                int targetWidth = oriImage.Width / 3;
                int targetHeight = oriImage.Height / 3;

                // 確保浮水印的大小比例正確
                float scale = Math.Min((float)targetWidth / watermarkImage.Width, (float)targetHeight / watermarkImage.Height);
                int newWidth = (int)(watermarkImage.Width * scale);
                int newHeight = (int)(watermarkImage.Height * scale);

                // 計算浮水印的位置
                int x = oriImage.Width - newWidth - 10; // 右邊距 10px
                int y = oriImage.Height - newHeight - 10; // 下邊距 10px

                // 繪製浮水印
                graphics.DrawImage(watermarkImage, new Rectangle(x, y, newWidth, newHeight), 0, 0, watermarkImage.Width, watermarkImage.Height, GraphicsUnit.Pixel, imageAttributes);
            }
            return proImage;
        }

        public Bitmap AddTextWatermark(string watermarkText, ColorMatrix colorMatrix, int angle, int fontSize)
        {
            Bitmap bitmap = new Bitmap(oriImage.Width, oriImage.Height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawImage(oriImage, 0, 0, oriImage.Width, oriImage.Height);
                Font font = new Font("Microsoft JhengHei", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
                ImageAttributes imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                SizeF textSize = graphics.MeasureString(watermarkText, font);
                graphics.TranslateTransform(oriImage.Width / 2, oriImage.Height / 2);
                graphics.RotateTransform(angle); // 旋轉角度
                graphics.TranslateTransform(-oriImage.Width / 2, -oriImage.Height / 2);

                for (int y = -oriImage.Height; y < oriImage.Height * 2; y += (int)textSize.Height + 10)
                {
                    for (int x = -oriImage.Width; x < oriImage.Width * 2; x += (int)textSize.Width + 10)
                    {
                        // Create a brush with the specified color matrix
                        using (Brush brush = new SolidBrush(Color.FromArgb((int)(colorMatrix.Matrix33 * 255), Color.White)))
                        {
                            graphics.DrawString(watermarkText, font, brush, new PointF(x, y));
                        }
                    }
                }
            }
            return bitmap;
        }

        public void GaussianSmonnthing()
        {
            if (oriImage == null)
            {
                throw new InvalidOperationException("Original image is not set.");
            }

            // 創建一個新的 Bitmap 來存儲平滑化後的圖片
            proImage = new Bitmap(oriImage.Width, oriImage.Height);

            using (Graphics graphics = Graphics.FromImage(proImage))
            {
                // 使用高斯模糊進行平滑化
                ImageAttributes imageAttributes = new ImageAttributes();
                float[][] colorMatrixElements = {
                    new float[] {1, 0, 0, 0, 0},
                    new float[] {0, 1, 0, 0, 0},
                    new float[] {0, 0, 1, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                };
                ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                graphics.DrawImage(oriImage, new Rectangle(0, 0, oriImage.Width, oriImage.Height), 0, 0, oriImage.Width, oriImage.Height, GraphicsUnit.Pixel, imageAttributes);
            }
        }

        public void NormallySmonnthing(int filter)
        {
            if (oriImage == null)
            {
                throw new InvalidOperationException("Original image is not set.");
            }

            // 創建一個新的 Bitmap 來存儲平滑化後的圖片
            proImage = new Bitmap(oriImage.Width, oriImage.Height);

            // 使用均值濾波進行平滑化
            int filterWidth = filter;
            int filterHeight = filter;
            int w = oriImage.Width;
            int h = oriImage.Height;

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    int r = 0, g = 0, b = 0;
                    int count = 0;

                    for (int fx = -filterWidth / 2; fx <= filterWidth / 2; fx++)
                    {
                        for (int fy = -filterHeight / 2; fy <= filterHeight / 2; fy++)
                        {
                            int ix = x + fx;
                            int iy = y + fy;

                            if (ix >= 0 && ix < w && iy >= 0 && iy < h)
                            {
                                Color pixel = oriImage.GetPixel(ix, iy);
                                r += pixel.R;
                                g += pixel.G;
                                b += pixel.B;
                                count++;
                            }
                        }
                    }

                    r /= count;
                    g /= count;
                    b /= count;

                    proImage.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
        }

        public void DiscreteFourierTransform()
        {
            int M = oriImage.Width;
            int N = oriImage.Height;
            real = new double[M, N];
            imag = new double[M, N];
            double[,] magnitude = new double[M, N];
            double[,] pixel = new double[M, N];
            proImage = new Bitmap(M, N);

            double _max = 0;

            for (int y = 0; y < N; y++)
            {
                for (int x = 0; x < M; x++)
                {
                    // 獲取原圖像像素值
                    pixel[x, y] = oriImage.GetPixel(x, y).R;
                }
            }
            for (int v = 0; v < N; v++)
            {
                for (int u = 0; u < M; u++)
                {
                    for (int y = 0; y < N; y++)
                    {
                        for (int x = 0; x < M; x++)
                        {
                            // 計算實部與虛部
                            real[u, v] += pixel[x, y] * Math.Cos(2 * Math.PI * (u * x / (double)M + v * y / (double)N));
                            imag[u, v] += -pixel[x, y] * Math.Sin(2 * Math.PI * (u * x / (double)M + v * y / (double)N));
                        }
                    }

                    // 將實部和虛部/M*N
                    real[u, v] /= M * N;
                    imag[u, v] /= M * N;

                    // 平方相加開根號
                    magnitude[u, v] = Math.Sqrt(real[u, v] * real[u, v] + imag[u, v] * imag[u, v]);
                    _max = Math.Max(_max, magnitude[u, v]);
                }
            }

            // 將幅值歸一化到 0 - 255 的範圍並存入 proImage
            for (int v = 0; v < N; v++)
            {
                for (int u = 0; u < M; u++)
                {
                    int intensity = (int)(255 * (magnitude[u, v] / _max)); // 歸一化處理
                    // 限制在 0 到 255 的範圍內
                    if (intensity > 255)
                        intensity = 255;
                    else if (intensity < 0)
                        intensity = 0;
                    proImage.SetPixel(u, v, Color.FromArgb(intensity, intensity, intensity));
                }
            }

            Bitmap fftshiftImage = proImage.Clone(new Rectangle(0, 0, proImage.Width, proImage.Height), proImage.PixelFormat);
            int heightHalf = N / 2;
            int widthHalf = M / 2;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    //轉移到中心
                    int x = (i + heightHalf) % N;
                    int y = (j + widthHalf) % M;
                    Color p = proImage.GetPixel(i, j);
                    fftshiftImage.SetPixel(x, y, p);
                }
            }

            proImage = fftshiftImage;
        }

        public void InverseDiscreteFourierTransform()
        {
            int M = oriImage.Width;
            int N = oriImage.Height;
            proImage = new Bitmap(M, N);

            for (int y = 0; y < N; y++)
            {
                for (int x = 0; x < M; x++)
                {
                    double IReal = 0.0;
                    double IImag = 0.0;

                    for (int v = 0; v < N; v++)
                    {
                        for (int u = 0; u < M; u++)
                        {
                            // 計算IDFT的角度
                            double angleIDFT = 2 * Math.PI * (u * x / (double)M + v * y / (double)N);

                            // 獲取存儲的傅立葉變換後的值
                            double FuvReal = real[u, v];
                            double FuvImag = imag[u, v];

                            // 累加實部和虛部，Cos和 sin 部分
                            IReal += FuvReal * Math.Cos(angleIDFT) - FuvImag * Math.Sin(angleIDFT);
                            IImag += FuvReal * Math.Sin(angleIDFT) + FuvImag * Math.Cos(angleIDFT);
                        }
                    }

                    // 將結果除以圖像尺寸的平方根
                    IReal /= Math.Sqrt(M * N);
                    IImag /= Math.Sqrt(M * N);

                    // 平方相加開根號並*100增加影像強度
                    int p = (int)(Math.Sqrt(IReal * IReal + IImag * IImag) * 100);
                    // 確保p在0到255的範圍內
                    if (p > 255)
                        p = 255;
                    else if (p < 0)
                        p = 0;

                    proImage.SetPixel(x, y, Color.FromArgb(p, p, p));
                }
            }
        }

        //public Bitmap ResizeImage(int targetWidth, int targetHeight)
        //{

        //    Bitmap resizedImage = new Bitmap(targetWidth, targetHeight);
        //    using (Graphics graphics = Graphics.FromImage(resizedImage))
        //    {
        //        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        //        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

        //        graphics.DrawImage(oriImage, 0, 0, targetWidth, targetHeight);
        //    }
        //    return resizedImage;
        //}
    }
}