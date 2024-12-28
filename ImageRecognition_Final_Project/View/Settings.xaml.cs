using System.Windows;

namespace ImageRecognition_Final_Project
{
    public partial class SettingsWindow : HandyControl.Controls.Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            // 保存設定的邏輯
            HandyControl.Controls.MessageBox.Show("設定已保存！");
        }
    }
}