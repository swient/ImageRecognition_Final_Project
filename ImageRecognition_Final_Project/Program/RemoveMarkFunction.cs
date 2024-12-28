using System;
using System.Drawing;
using System.Windows;
using System;
using System.Drawing.Imaging;
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
        public Bitmap option1()
        {
            proImage = new Bitmap(oriImage);

            for (int x = MarkRect.X; x < MarkRect.X + MarkRect.Width; x++)
            {
                for (int y = MarkRect.Y; y < MarkRect.Y + MarkRect.Height; y++)
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
            return proImage;
        }
    }
}
