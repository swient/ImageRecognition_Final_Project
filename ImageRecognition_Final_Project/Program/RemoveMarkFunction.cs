using System;
using System.Drawing;
using System.Windows;
using System;
using System.Drawing.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;


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
        //像素填補
        public Bitmap vertical_padding()
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
        public Bitmap Vague()
        {
            proImage = new Bitmap(oriImage);

            // 創建遮罩
            bool[,] Mask = new bool[oriImage.Width, oriImage.Height];
            Mask = ApplyErosionAndDilation();

            for (int x = MarkRect.X; x < MarkRect.X + MarkRect.Width; x++)
            {
                for (int y = MarkRect.Y; y < MarkRect.Y + MarkRect.Height; y++)
                {
                    if (Mask[x, y])
                    {
                        // 使用周圍像素來計算加權平均
                        int radius = 3; // 計算範圍半徑，根據需要調整
                        int r = 0, g = 0, b = 0, weightSum = 0;

                        // 計算周圍區域像素的加權平均
                        for (int dx = -radius; dx <= radius; dx++)
                        {
                            for (int dy = -radius; dy <= radius; dy++)
                            {
                                int newX = x + dx;
                                int newY = y + dy;

                                if (newX >= 0 && newX < oriImage.Width && newY >= 0 && newY < oriImage.Height)
                                {
                                    // 確保權重隨距離遠近而減少
                                    int weight = Math.Max(0, radius - Math.Abs(dx) - Math.Abs(dy)); // 距離越遠權重越小
                                    
                                    Color pixel = oriImage.GetPixel(newX, newY);
                                    r += pixel.R * weight;
                                    g += pixel.G * weight;
                                    b += pixel.B * weight;
                                    weightSum += weight;
                                }
                            }
                        }

                        // 計算加權平均顏色
                        if (weightSum > 0)
                        {
                            r /= weightSum;
                            g /= weightSum;
                            b /= weightSum;
                        }

                        // 設置新的像素顏色
                        proImage.SetPixel(x, y, Color.FromArgb(r, g, b));
                    }
                }
            }

            return proImage;
        }
        public Bitmap emgucv()
        {
            // 檢查 oriImage 是否為有效圖像
            if (oriImage == null || oriImage.Width == 1 || oriImage.Height == 1)
            {
                throw new ArgumentException("Invalid original image.");
            }

            // 將原始Bitmap轉換為Emgu.CV的Image格式
            Image<Bgr, byte> image;
            try
            {
                image = oriImage.ToImage<Bgr, byte>();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to convert Bitmap to OpenCV Image: " + ex.Message);
            }

            // 創建遮罩
            Image<Gray, byte> mask = new Image<Gray, byte>(image.Width, image.Height);

            // 用白色填充遮罩，並將MarkRect區域設為黑色
            mask.SetValue(new MCvScalar(0)); // 黑色
            mask.ROI = new System.Drawing.Rectangle(MarkRect.X, MarkRect.Y, MarkRect.Width, MarkRect.Height);
            mask.SetValue(new MCvScalar(255)); // 白色
            mask.ROI = System.Drawing.Rectangle.Empty; // 清除 ROI

            // 使用修補方法填補浮水印區域
            CvInvoke.Inpaint(image, mask, image, 3, InpaintType.Telea);

            // 將處理後的圖像轉回為Bitmap
            proImage = image.ToBitmap();

            return proImage;
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

            for (int x = MarkRect.X; x < MarkRect.X + MarkRect.Width; x++)
            {
                for (int y = MarkRect.Y; y < MarkRect.Y + MarkRect.Height; y++)
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

            for (int x = MarkRect.X; x < MarkRect.X + MarkRect.Width; x++)
            {
                for (int y = MarkRect.Y; y < MarkRect.Y + MarkRect.Height; y++)
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
    

