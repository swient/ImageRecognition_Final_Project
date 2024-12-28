using System;
using System.Drawing;
using System.Windows;

namespace ImageRecognition_Final_Project.Program
{
    internal class RemoveMarkFunction
    {
        public Bitmap oriImage; // 原始圖像
        public Bitmap proImage; // 處理後圖像
        public Int32Rect MarkRect; //浮水印區域
        public RemoveMarkFunction()
        {
            oriImage = new Bitmap(1, 1);
            proImage = new Bitmap(1, 1);
        }

        public Bitmap option1()
        {
            
            // 創建一個與原圖大小一致的處理後圖像
            proImage = new Bitmap(oriImage);

          

            // 替換浮水印區域的像素為背景顏色
            for (int i = MarkRect.X; i < MarkRect.X + MarkRect.Width; i++)
            {
                for (int j = MarkRect.Y; j < MarkRect.Y + MarkRect.Height; j++)
                {
                   
                }
            }

            return proImage;
        }

        
    }
}
