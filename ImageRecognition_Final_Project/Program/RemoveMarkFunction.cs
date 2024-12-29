using System;
using System.Drawing;
using System.Windows;
using System;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.CV.Reg;

namespace ImageRecognition_Final_Project.Program
{
    internal class RemoveMarkFunction
    {
        public Bitmap oriImage; // 原始圖像
        public Bitmap proImage; // 處理後圖像
        public Int32Rect MarkRect; // 浮水印區域
        
        public RemoveMarkFunction()
        {
            oriImage = new Bitmap(1, 1);
            proImage = new Bitmap(1, 1);
        }

        //垂直像素填充
        public Bitmap Vertical_padding()
        {
            proImage = new Bitmap(oriImage);
            //創建遮罩
            bool[,] Mask = new bool[oriImage.Width, oriImage.Height];
            Mask = ApplyErosionAndDilation();
            for (int x = MarkRect.X; x < MarkRect.X + MarkRect.Width; x++)
            {
                for (int y = MarkRect.Y; y < MarkRect.Y + MarkRect.Height; y++)
                {
                    if (Mask[x, y])
                    {
                        // 獲取上下兩行像素的平均值
                        int topY = MarkRect.Y - 1 >= 0 ? MarkRect.Y - 1 : MarkRect.Y;
                        int bottomY = MarkRect.Y + MarkRect.Height < oriImage.Height ? MarkRect.Y + MarkRect.Height : MarkRect.Y;

                        Color topPixel = oriImage.GetPixel(x, topY);
                        Color bottomPixel = oriImage.GetPixel(x, bottomY);

                        int r = (topPixel.R + bottomPixel.R) / 2;
                        int g = (topPixel.G + bottomPixel.G) / 2;
                        int b = (topPixel.B + bottomPixel.B) / 2;

                        proImage.SetPixel(x, y, Color.FromArgb(r, g, b));
                    }
                }
            }
            return proImage;
        }

        //水平像素填充
        public Bitmap Horizontal_padding()
        {
            proImage = new Bitmap(oriImage);
            //創建遮罩
            bool[,] Mask = new bool[oriImage.Width, oriImage.Height];
            Mask = ApplyErosionAndDilation();
            for (int x = MarkRect.X; x < MarkRect.X + MarkRect.Width; x++)
            {
                for (int y = MarkRect.Y; y < MarkRect.Y + MarkRect.Height; y++)
                {
                    if (Mask[x, y])
                    {
                        // 獲取左右兩列像素的平均值
                        int leftX = MarkRect.X - 1 >= 0 ? MarkRect.X - 1 : MarkRect.X;
                        int rightX = MarkRect.X + MarkRect.Width < oriImage.Width ? MarkRect.X + MarkRect.Width : MarkRect.X;

                        Color leftPixel = oriImage.GetPixel(leftX, y);
                        Color rightPixel = oriImage.GetPixel(rightX, y);

                        int r = (leftPixel.R + rightPixel.R) / 2;
                        int g = (leftPixel.G + rightPixel.G) / 2;
                        int b = (leftPixel.B + rightPixel.B) / 2;

                        proImage.SetPixel(x, y, Color.FromArgb(r, g, b));
                    }
                }
            }
            return proImage;
        }

        public Bitmap EmgucvLargeArea()
        {
            // 將 Bitmap 轉換為 Mat
            Mat image = BitmapToMat(oriImage);

            // 轉換為灰階圖像
            Mat gray = new Mat();
            CvInvoke.CvtColor(image, gray, ColorConversion.Bgr2Gray);

            // 閾值分割生成遮罩
            Mat mask = new Mat();
            CvInvoke.Threshold(gray, mask, 200, 255, ThresholdType.Binary);

            // 膨脹遮罩
            Mat dilatedMask = new Mat();
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(3, 3), new System.Drawing.Point(-1, -1));
            CvInvoke.Dilate(mask, dilatedMask, kernel, new System.Drawing.Point(-1, -1), 1, BorderType.Default, default(MCvScalar));

            // 修復圖像
            Mat result = new Mat();
            CvInvoke.Inpaint(image, dilatedMask, result, 3, InpaintType.Telea);

            // 將修復後的區域擷取出來
            System.Drawing.Rectangle roi = new System.Drawing.Rectangle(MarkRect.X, MarkRect.Y, MarkRect.Width, MarkRect.Height);
            Mat repairedRegion = new Mat(result, roi);

            // 將擷取出的區域填入到原始圖像的對應位置
            Mat originalImage = BitmapToMat(oriImage);
            repairedRegion.CopyTo(new Mat(originalImage, roi));

            // 將最終結果轉換為 Bitmap 並儲存到 proImage
            proImage = originalImage.ToBitmap();
            return proImage;
        }

        public Bitmap EmgucvSmallArea()
        {
            // 將 Bitmap 轉換為 Mat
            Mat image = BitmapToMat(oriImage);

            // 初始化遮罩為全黑（0）
            Mat mask = new Mat(image.Size, DepthType.Cv8U, 1);
            mask.SetTo(new MCvScalar(0));

            // 根據 MarkRect 在遮罩上繪製白色矩形（255）
            System.Drawing.Rectangle roi = new System.Drawing.Rectangle(MarkRect.X, MarkRect.Y, MarkRect.Width, MarkRect.Height);
            CvInvoke.Rectangle(mask, roi, new MCvScalar(255), -1); // 填充矩形

            // 修復圖像，僅對選定區域生效
            Mat result = new Mat();
            CvInvoke.Inpaint(image, mask, result, 3, InpaintType.Telea);

            // 將修復後的 Mat 轉換為 Bitmap 並儲存到 proImage
            proImage = result.ToBitmap();

            return proImage;
        }

        public Bitmap EmgucvHighContrast()
        {
            // 將 Bitmap 轉換為 Mat
            Mat image = BitmapToMat(oriImage);

            // 轉換為灰階圖像
            Mat gray = new Mat();
            CvInvoke.CvtColor(image, gray, ColorConversion.Bgr2Gray);

            // 閾值分割生成遮罩
            Mat mask = new Mat();
            CvInvoke.Threshold(gray, mask, 200, 255, ThresholdType.Binary);

            // 膨脹遮罩
            Mat dilatedMask = new Mat();
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(3, 3), new System.Drawing.Point(-1, -1));
            CvInvoke.Dilate(mask, dilatedMask, kernel, new System.Drawing.Point(-1, -1), 1, BorderType.Default, default(MCvScalar));

            // 修復圖像
            Mat result = new Mat();
            CvInvoke.Inpaint(image, dilatedMask, result, 3, InpaintType.Telea);

            // 將修復後的 Mat 轉換為 Bitmap 並儲存到 proImage
            proImage = result.ToBitmap();

            return proImage;
        }

        private Mat BitmapToMat(Bitmap bitmap)
        {
            // 使用 .Clone() 創建新 Mat，避免原始對象被意外銷毀
            using (Image<Bgr, byte> img = bitmap.ToImage<Bgr, byte>())
            {
                return img.Mat.Clone();
            }
        }

        // 邊緣檢測 Sobel
        public Bitmap ApplySobelEdgeDetection()
        {
            // 轉換圖像為灰階
            Bitmap grayImage = ConvertToGrayscale(oriImage);

            // Sobel 運算符
            int[,] sobelX = new int[,] {
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 }
            };

            int[,] sobelY = new int[,] {
                { -1, -2, -1 },
                {  0,  0,  0 },
                {  1,  2,  1 }
            };

            Bitmap resultImage = new Bitmap(oriImage.Width, oriImage.Height);

            for (int x = MarkRect.X + 1; x < MarkRect.X + MarkRect.Width; x++)
            {
                for (int y = MarkRect.Y + 1; y < MarkRect.Y + MarkRect.Height; y++)
                {
                    int pixelX = 0;
                    int pixelY = 0;

                    // 應用 Sobel X 和 Sobel Y 運算符
                    for (int j = -1; j <= 1; j++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            Color pixel = grayImage.GetPixel(x + i, y + j);
                            int grayValue = pixel.R; // 假設是灰階圖像
                            pixelX += grayValue * sobelX[j + 1, i + 1];
                            pixelY += grayValue * sobelY[j + 1, i + 1];
                        }
                    }

                    // 計算梯度幅值
                    int gradValue = (int)Math.Sqrt(pixelX * pixelX + pixelY * pixelY);
                    gradValue = Math.Min(255, gradValue); // 限制最大值為255

                    resultImage.SetPixel(x, y, Color.FromArgb(gradValue, gradValue, gradValue)); // 設置為灰階顏色
                }
            }
            return resultImage;
        }

        // 侵蝕操作
        public Bitmap Erosion(Bitmap inputImage)
        {
            Bitmap binaryImage = ConvertToBinary(inputImage);
            Bitmap erodedImage = new Bitmap(binaryImage.Width, binaryImage.Height);

            for (int x = MarkRect.X + 1; x < MarkRect.X + MarkRect.Width; x++)
            {
                for (int y = MarkRect.Y + 1; y < MarkRect.Y + MarkRect.Height; y++)
                {
                    bool isBackground = true;

                    // 檢查3x3區域內的像素，若有任意一個像素是前景則標記為前景
                    for (int j = -1; j <= 1; j++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            Color pixel = binaryImage.GetPixel(x + i, y + j);
                            if (pixel.R == 255) // 前景像素
                            {
                                isBackground = false;
                                break;
                            }
                        }
                        if (!isBackground) break;
                    }

                    // 如果整個3x3區域內的像素都不是前景，則設定為背景
                    if (isBackground)
                    {
                        erodedImage.SetPixel(x, y, Color.Black); // 背景像素
                    }
                    else
                    {
                        erodedImage.SetPixel(x, y, Color.White); // 前景像素
                    }
                }
            }
            return erodedImage;
        }

        // 膨脹操作
        public Bitmap Dilation(Bitmap inputImage)
        {
            Bitmap binaryImage = ConvertToBinary(inputImage);
            Bitmap dilatedImage = new Bitmap(binaryImage.Width, binaryImage.Height);

            for (int x = MarkRect.X; x < MarkRect.X + MarkRect.Width; x++)
            {
                for (int y = MarkRect.Y; y < MarkRect.Y + MarkRect.Height; y++)
                {
                    bool isForeground = false;

                    // 檢查3x3區域內的像素，若有任意一個像素是前景則標記為前景
                    for (int j = -1; j <= 1; j++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            if (x + i > oriImage.Width || x + i < 0 || y + j > oriImage.Height || y + j < 0)
                                continue;
                            Color pixel = binaryImage.GetPixel(x + i, y + j);
                            if (pixel.R == 255) // 前景像素
                            {
                                isForeground = true;
                                break;
                            }
                        }
                        if (isForeground) break;
                    }

                    // 如果有任意一個像素是前景，則設定為前景
                    if (isForeground)
                    {
                        dilatedImage.SetPixel(x, y, Color.White); // 前景像素
                    }
                    else
                    {
                        dilatedImage.SetPixel(x, y, Color.Black); // 背景像素
                    }
                }
            }
            return dilatedImage;
        }

        // 將圖像轉換為二值圖像
        private Bitmap ConvertToBinary(Bitmap original)
        {
            Bitmap binaryImage = new Bitmap(original.Width, original.Height);
            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color originalColor = original.GetPixel(x, y);
                    // 使用簡單的閾值方法來二值化圖像
                    int grayValue = (int)(0.3 * originalColor.R + 0.59 * originalColor.G + 0.11 * originalColor.B);
                    if (grayValue > 128) // 如果灰階值大於閾值則視為前景
                    {
                        binaryImage.SetPixel(x, y, Color.White); // 前景
                    }
                    else
                    {
                        binaryImage.SetPixel(x, y, Color.Black); // 背景
                    }
                }
            }
            return binaryImage;
        }

        // 將圖像轉換為灰階
        private Bitmap ConvertToGrayscale(Bitmap original)
        {
            Bitmap grayImage = new Bitmap(original.Width, original.Height);
            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color originalColor = original.GetPixel(x, y);
                    int grayValue = (int)(0.3 * originalColor.R + 0.59 * originalColor.G + 0.11 * originalColor.B);
                    grayImage.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                }
            }
            return grayImage;
        }

        // 創建遮罩
        public bool[,] ApplyErosionAndDilation()
        {
            // 執行 Sobel 邊緣檢測
            Bitmap sobelImage = ApplySobelEdgeDetection();

            // 執行侵蝕操作
            Bitmap erodedImage = Erosion(sobelImage);

            // 執行膨脹操作
            Bitmap dilatedImage = Dilation(erodedImage);
            bool [,]Mask = new bool[oriImage.Width, oriImage.Height];
            for (int y = 0; y < oriImage.Height; y++)
            {
                for (int x = 0; x < oriImage.Width; x++)
                {
                    Color dilatedImageColor = dilatedImage.GetPixel(x, y);
                    if (dilatedImageColor.R == 255)
                    {
                        Mask[x, y] = true;
                    }
                    else
                        Mask[x, y] = false;
                }
            }
            return Mask; 
        }
    }
}