using HandyControl.Controls;
using ImageRecognition_Final_Project.Program;
using ImageRecognition_Final_Project.SharedView;
using Microsoft.Win32;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ImageRecognition_Final_Project
{
    public partial class MainWindow : HandyControl.Controls.Window
    {
        private Bitmap? oriImage;
        private Bitmap? watermarkImage;
        private Bitmap? proImage;
        private Bitmap? saveImage;
        private MyImageManager myImageManager;
        private SharedViewModel sharedViewModel;
        double WatermarkSliderValue;
        double SmonnthingSliderValue;
        int current_save_select = 0; //追蹤更新
        //圖片裁切
        private System.Windows.Point startPoint;
        private System.Windows.Point endPoint;
        private bool isSelecting = false;

        public MainWindow()
        {
            oriImage = null;
            watermarkImage = null;
            proImage = null;
            myImageManager = new MyImageManager(); // 初始化 myImageManager
            sharedViewModel = new SharedViewModel(); // 初始化 sharedViewModel
            WatermarkSliderValue = 0.5;
            SmonnthingSliderValue = 3.0;

            InitializeComponent();
        }

        private void SelectImage_Button(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            string tag = clickedButton?.Tag.ToString();

            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                Bitmap selectedImage = new Bitmap(openFileDialog.FileName);
                ImageSource imageSource = BitmapToImageSource(selectedImage); // 假設你有一個方法來轉換圖片為 ImageSource

                // 根據 Tag 來決定要更新哪個圖片控制項與資料
                switch (tag)
                {
                    case "WatermarkMainImage":
                        oriImage = selectedImage;
                        myImageManager.oriImage = selectedImage; // 設定為主影像
                        WatermarkMainImage.Source = imageSource;
                        break;

                    case "SmonnthingMainImage":
                        oriImage = selectedImage;
                        myImageManager.oriImage = selectedImage; // 設定為主影像
                        SmonnthingMainImage.Source = imageSource;
                        break;

                    case "FourierTransformMainImage":
                        oriImage = selectedImage;
                        myImageManager.oriImage = selectedImage; // 設定為主影像
                        FourierTransformMainImage.Source = imageSource;
                        break;

                    case "WatermarkImage":
                        watermarkImage = selectedImage;
                        myImageManager.watermarkImage = selectedImage; // 設定為浮水印影像
                        WatermarkImage.Source = imageSource;
                        break;
                    //removewatermark需重新調整大小
                    case "RemoveMarkMainImage":
                        selectedImage = myImageManager.ResizeImage(selectedImage, 490, selectedImage.Height * 490 / selectedImage.Width);
                        imageSource = BitmapToImageSource(selectedImage);
                        RemoveMarkMainImage.Source = imageSource;
                        break;
                    default:
                        break;
                }
            }
        }

        private void WatermarkGenerateImage_Button(object sender, RoutedEventArgs e)
        {
            if (oriImage == null || watermarkImage == null)
            {
                HandyControl.Controls.MessageBox.Show("請先選擇主圖片和浮水印圖片！");
                return;
            }

            ColorMatrix colorMatrix = new()
            {
                Matrix33 = (float)WatermarkSliderValue
            };

            proImage = myImageManager.AddWatermark(colorMatrix);

            // 顯示合成後的圖片
            WatermarkGenerateImage.Source = BitmapToImageSource(proImage);
            GenerateImage_save1.Source = BitmapToImageSource(proImage);
            GenerateImage_save2.Source = BitmapToImageSource(proImage);
            GenerateImage_save3.Source = BitmapToImageSource(proImage);
            HandyControl.Controls.Growl.Success("浮水印生成成功！");

            //防止未選擇圖片儲存bug
            if (current_save_select == 0)
                saveImage = ConvertImageSourceToBitmap(GenerateImage_save1.Source);
        }

        private void Watermark_Slider_Value(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            WatermarkSliderValue = e.NewValue;

            if (oriImage != null && watermarkImage != null)
            {
                ColorMatrix colorMatrix = new()
                {
                    Matrix33 = (float)e.NewValue
                };

                myImageManager.AddWatermark(colorMatrix);

                // 顯示合成後的圖片
                WatermarkGenerateImage.Source = BitmapToImageSource(myImageManager.proImage);
                GenerateImage_save1.Source = BitmapToImageSource(myImageManager.proImage);
                GenerateImage_save2.Source = BitmapToImageSource(myImageManager.proImage);
                GenerateImage_save3.Source = BitmapToImageSource(myImageManager.proImage);
                //HandyControl.Controls.Growl.Success("浮水印生成成功！");
            }

            //HandyControl.Controls.Growl.Info($"滑塊值: {e.NewValue}");
        }

        private void GaussianSmonnthing_Button(object sender, RoutedEventArgs e)
        {
            if (oriImage == null)
            {
                HandyControl.Controls.MessageBox.Show("請先選擇主圖片！");
                return;
            }

            ColorMatrix colorMatrix = new()
            {
                Matrix33 = (float)SmonnthingSliderValue
            };
            myImageManager.Smonnthing1();

            // 顯示合成後的圖片
            GaussianSmonnthing.Source = BitmapToImageSource(myImageManager.proImage);
            Smonnthing1_save1.Source = BitmapToImageSource(myImageManager.proImage);
            Smonnthing1_save2.Source = BitmapToImageSource(myImageManager.proImage);
            Smonnthing1_save3.Source = BitmapToImageSource(myImageManager.proImage);
            HandyControl.Controls.Growl.Success("高斯模糊成功！");
            if (current_save_select == 1)
                saveImage = ConvertImageSourceToBitmap(GenerateImage_save1.Source);
        }

        private void NormallySmonnthing_Button(object sender, RoutedEventArgs e)
        {
            if (oriImage == null)
            {
                HandyControl.Controls.MessageBox.Show("請先選擇主圖片！");
                return;
            }
            myImageManager.Smonnthing2((int)SmonnthingSliderValue);

            // 顯示合成後的圖片
            NormallySmonnthing.Source = BitmapToImageSource(myImageManager.proImage);
            Smonnthing2_save1.Source = BitmapToImageSource(myImageManager.proImage);
            Smonnthing2_save2.Source = BitmapToImageSource(myImageManager.proImage);
            Smonnthing2_save3.Source = BitmapToImageSource(myImageManager.proImage);
            HandyControl.Controls.Growl.Success("一般平滑化成功！");
            if (current_save_select == 2)
                saveImage = ConvertImageSourceToBitmap(GenerateImage_save1.Source);
        }

        private void Smonnthing_Slider_Value(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SmonnthingSliderValue = e.NewValue;
            //if (oriImage != null)
            //{
            //    myImageManager.Smonnthing2((int)value);

            //    // 顯示合成後的圖片
            //    Smonnthing2.Source = BitmapToImageSource(myImageManager.proImage);
            //    HandyControl.Controls.Growl.Success("模糊化2成功！");
            //}

            //HandyControl.Controls.Growl.Info($"滑塊值: {e.NewValue}");
        }

        private void DiscreteFourierTransform_Button(object sender, RoutedEventArgs e)
        {
            if (oriImage == null)
            {
                HandyControl.Controls.MessageBox.Show("請先選擇主圖片！");
                return;
            }
            myImageManager.DiscreteFourierTransform();

            // 顯示合成後的圖片
            DiscreteFourierTransform.Source = BitmapToImageSource(myImageManager.proImage);
            DiscreteFourierTransform_save1.Source = BitmapToImageSource(myImageManager.proImage);
            DiscreteFourierTransform_save2.Source = BitmapToImageSource(myImageManager.proImage);
            DiscreteFourierTransform_save3.Source = BitmapToImageSource(myImageManager.proImage);
            HandyControl.Controls.Growl.Success("傅立葉變換成功！");

            if (current_save_select == 3)
                saveImage = ConvertImageSourceToBitmap(GenerateImage_save1.Source);
        }

        private void InverseDiscreteFourierTransform_Button(object sender, RoutedEventArgs e)
        {
            if (oriImage == null)
            {
                HandyControl.Controls.MessageBox.Show("請先選擇主圖片！");
                return;
            }
            myImageManager.InverseDiscreteFourierTransform();

            // 顯示合成後的圖片
            InverseDiscreteFourierTransform.Source = BitmapToImageSource(myImageManager.proImage);
            InverseDiscreteFourierTransform_save1.Source = BitmapToImageSource(myImageManager.proImage);
            InverseDiscreteFourierTransform_save2.Source = BitmapToImageSource(myImageManager.proImage);
            InverseDiscreteFourierTransform_save3.Source = BitmapToImageSource(myImageManager.proImage);
            HandyControl.Controls.Growl.Success("逆傅立葉變換成功！");

            if (current_save_select == 4)
                saveImage = ConvertImageSourceToBitmap(GenerateImage_save1.Source);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (saveImage == null)
            {
                HandyControl.Controls.MessageBox.Show("請先生成並保存圖片！");
                return;
            }

            // 設定 SaveFileDialog 參數
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "選取儲存路徑",
                Filter = "JPEG Image (*.jpg)|*.jpg",
                DefaultExt = "jpg",
                FileName = "output.jpg",
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // 儲存圖片為 JPEG 格式
                    saveImage.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                    HandyControl.Controls.MessageBox.Show("圖片儲存成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    HandyControl.Controls.MessageBox.Show($"儲存圖片時發生錯誤：{ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 確保在選擇項目之後才執行
            if (e.Source is System.Windows.Controls.TabControl tabControl && tabControl.SelectedItem is System.Windows.Controls.TabItem selectedTab)
            {
                current_save_select = tabControl.SelectedIndex;
                switch (tabControl.SelectedIndex)
                {
                    //Watermark
                    case 0:
                        saveImage = ConvertImageSourceToBitmap(GenerateImage_save1.Source);
                        break;
                    //case "Gaussian_smoothing":
                    case 1:
                        saveImage = ConvertImageSourceToBitmap(Smonnthing1_save1.Source);
                        break;
                    //case "general_smoothing":
                    case 2:
                        saveImage = ConvertImageSourceToBitmap(Smonnthing2_save1.Source);
                        break;
                    //case "Fourier_transform":
                    case 3:
                        saveImage = ConvertImageSourceToBitmap(DiscreteFourierTransform_save1.Source);
                        break;
                    //case "Inverse_Fourier_Transform":
                    case 4:
                        saveImage = ConvertImageSourceToBitmap(InverseDiscreteFourierTransform_save1.Source);
                        break;
                    default:
                        //MessageBox.Show("未知選項卡");
                        break;
                }

            }
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

        public static Bitmap? ConvertImageSourceToBitmap(ImageSource imageSource)
        {
            // 創建一個 MemoryStream 來存儲圖像數據
            var bitmapImage = imageSource as BitmapImage;
            if (bitmapImage != null)
            {
                // 創建一個 Bitmap 來轉換 ImageSource
                using MemoryStream stream = new MemoryStream();
                // 將 ImageSource（BitmapImage）保存到 MemoryStream
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(stream);

                // 將 MemoryStream 轉換為 System.Drawing.Bitmap
                return new Bitmap(stream);
            }
            return null;
        }
        private void ImageViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //如果沒有圖像
            if (RemoveMarkMainImage.Source == null) return;
                //獲取起始座標
                startPoint = e.GetPosition(RemoveMarkMainImage);
                //繪製矩形
                SelectionRect.Visibility = Visibility.Visible;
                //設置矩形左上角座標
                Canvas.SetLeft(SelectionRect, startPoint.X);
                Canvas.SetTop(SelectionRect, startPoint.Y);
                //初始化矩形
                SelectionRect.Width = 0;
                SelectionRect.Height = 0;
                //標記正在剪裁
                isSelecting = true;

                // 捕捉滑鼠
                RemoveMarkMainImage.CaptureMouse();   

        }

        private void ImageViewer_MouseMove(object sender, MouseEventArgs e)
        {
            //如果沒有圖象or沒有剪裁動作
            if (!isSelecting || RemoveMarkMainImage.Source == null) return;
            //獲取滑鼠座標
            endPoint = e.GetPosition(RemoveMarkMainImage);
            //獲取左上角頂點座標
            double x = Math.Min(startPoint.X, endPoint.X);
            double y = Math.Min(startPoint.Y, endPoint.Y);
            //獲取矩形長寬
            double width = Math.Abs(startPoint.X - endPoint.X);
            double height = Math.Abs(startPoint.Y - endPoint.Y);
            //設置矩形
            Canvas.SetLeft(SelectionRect, x);
            Canvas.SetTop(SelectionRect, y);
            SelectionRect.Width = width;
            SelectionRect.Height = height;
        }
        private void ImageViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isSelecting || RemoveMarkMainImage.Source == null) return;

            isSelecting = false;
            SelectionRect.Visibility = Visibility.Collapsed;

            endPoint = e.GetPosition(RemoveMarkMainImage);

            var bitmapSource = RemoveMarkMainImage.Source as BitmapSource;
            if (bitmapSource != null)
            {
                // 限制滑鼠座標
                startPoint.X = Math.Max(0, Math.Min(startPoint.X, RemoveMarkMainImage.ActualWidth));
                startPoint.Y = Math.Max(0, Math.Min(startPoint.Y, RemoveMarkMainImage.ActualHeight));
                endPoint.X = Math.Max(0, Math.Min(endPoint.X, RemoveMarkMainImage.ActualWidth));
                endPoint.Y = Math.Max(0, Math.Min(endPoint.Y, RemoveMarkMainImage.ActualHeight));

                // 計算比例
                double xRatio = bitmapSource.PixelWidth / RemoveMarkMainImage.ActualWidth;
                double yRatio = bitmapSource.PixelHeight / RemoveMarkMainImage.ActualHeight;

                // 計算裁剪範圍
                Int32Rect cropRect = new Int32Rect(
                    Math.Max(0, (int)(Math.Min(startPoint.X, endPoint.X) * xRatio)),
                    Math.Max(0, (int)(Math.Min(startPoint.Y, endPoint.Y) * yRatio)),
                    Math.Max(1, (int)(Math.Abs(startPoint.X - endPoint.X) * xRatio)),
                    Math.Max(1, (int)(Math.Abs(startPoint.Y - endPoint.Y) * yRatio))
                );

                // 確保裁剪範圍不超出圖像邊界
                if (cropRect.X + cropRect.Width > bitmapSource.PixelWidth)
                    cropRect.Width = bitmapSource.PixelWidth - cropRect.X;
                if (cropRect.Y + cropRect.Height > bitmapSource.PixelHeight)
                    cropRect.Height = bitmapSource.PixelHeight - cropRect.Y;

                // 執行裁剪
                try
                {
                    CroppedBitmap croppedBitmap = new CroppedBitmap(bitmapSource, cropRect);
                    RemoveWatermarkImage.Source = croppedBitmap;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error cropping image: {ex.Message}");
                }
            }
        }
    }
}