using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageRecognition_Final_Project
{
    public partial class MainWindow : HandyControl.Controls.Window
    {
        private Bitmap? oriImage;
        private Bitmap? watermarkImage;
        private Bitmap? proImage;
        private MyImageManager myImageManager;

        public MainWindow()
        {
            oriImage = null;
            watermarkImage = null;
            proImage = null;
            myImageManager = new MyImageManager(); // 初始化 myImageManager
            InitializeComponent();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            // 創建並顯示設定窗口
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            // 創建並顯示關於窗口
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        private static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using MemoryStream memory = new MemoryStream();
            bitmap.Save(memory, ImageFormat.Bmp);
            memory.Position = 0;
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        private void SelectMainImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (openFileDialog.ShowDialog() == true)
            {
                oriImage = new Bitmap(openFileDialog.FileName);
                myImageManager.oriImage = oriImage;
                MainImage.Source = BitmapToImageSource(oriImage);
            }
        }

        private void SelectMainImage_Click2(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (openFileDialog.ShowDialog() == true)
            {
                oriImage = new Bitmap(openFileDialog.FileName);
                myImageManager.oriImage = oriImage;
                MainImage2.Source = BitmapToImageSource(oriImage);
            }
        }

        private void SelectMainImage_Click3(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (openFileDialog.ShowDialog() == true)
            {
                oriImage = new Bitmap(openFileDialog.FileName);
                myImageManager.oriImage = oriImage;
                MainImage3.Source = BitmapToImageSource(oriImage);
            }
        }

        private void SelectWatermark_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (openFileDialog.ShowDialog() == true)
            {
                watermarkImage = new Bitmap(openFileDialog.FileName);
                myImageManager.watermarkImage = watermarkImage;
                WatermarkImage.Source = BitmapToImageSource(watermarkImage);
            }
        }

        private void GenerateImage_Click(object sender, RoutedEventArgs e)
        {
            if (oriImage == null || watermarkImage == null)
            {
                HandyControl.Controls.MessageBox.Show("請先選擇主圖片和浮水印圖片！");
                return;
            }
            proImage = myImageManager.AddWatermark();

            // 顯示合成後的圖片
            GenerateImage.Source = BitmapToImageSource(proImage);
            HandyControl.Controls.Growl.Success("浮水印生成成功！");
        }

        private void Smonnthing1_Click(object sender, RoutedEventArgs e)
        {
            if (oriImage == null)
            {
                HandyControl.Controls.MessageBox.Show("請先選擇主圖片！");
                return;
            }
            myImageManager.Smonnthing1();

            // 顯示合成後的圖片
            Smonnthing1.Source = BitmapToImageSource(myImageManager.proImage);
            HandyControl.Controls.Growl.Success("影像平滑化1成功！");
        }

        private void Smonnthing2_Click(object sender, RoutedEventArgs e)
        {
            if (oriImage == null)
            {
                HandyControl.Controls.MessageBox.Show("請先選擇主圖片！");
                return;
            }
            myImageManager.Smonnthing2();

            // 顯示合成後的圖片
            Smonnthing2.Source = BitmapToImageSource(myImageManager.proImage);
            HandyControl.Controls.Growl.Success("影像平滑化2成功！");
        }

        private void DiscreteFourierTransform_Click(object sender, RoutedEventArgs e)
        {
            if (oriImage == null)
            {
                HandyControl.Controls.MessageBox.Show("請先選擇主圖片！");
                return;
            }
            myImageManager.DiscreteFourierTransform();

            // 顯示合成後的圖片
            DiscreteFourierTransform.Source = BitmapToImageSource(myImageManager.proImage);
            HandyControl.Controls.Growl.Success("傅立葉變換成功！");
        }

        private void InverseDiscreteFourierTransform_Click(object sender, RoutedEventArgs e)
        {
            if (oriImage == null)
            {
                HandyControl.Controls.MessageBox.Show("請先選擇主圖片！");
                return;
            }
            myImageManager.InverseDiscreteFourierTransform();

            // 顯示合成後的圖片
            InverseDiscreteFourierTransform.Source = BitmapToImageSource(myImageManager.proImage);
            HandyControl.Controls.Growl.Success("逆傅立葉變換成功！");
        }
    }
}