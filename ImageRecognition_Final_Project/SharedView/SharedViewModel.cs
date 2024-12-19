using System.Drawing;
using System.Drawing.Imaging;

namespace ImageRecognition_Final_Project.SharedView
{

    internal class SharedViewModel
    {
        public Bitmap oriImage;
        public Bitmap watermarkImage;
        public Bitmap proImage;
        private double[,] real;
        private double[,] imag;

        public SharedViewModel()
        {
            oriImage = new Bitmap(1, 1);
            watermarkImage = new Bitmap(1, 1);
            proImage = new Bitmap(1, 1);
            real = new double[1, 1]; // 初始化 real 欄位
            imag = new double[1, 1]; // 初始化 imag 欄位
        }

    }
}