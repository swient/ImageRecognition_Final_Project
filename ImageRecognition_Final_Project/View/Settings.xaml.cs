using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ImageRecognition_Final_Project
{
    public partial class SettingsWindow : HandyControl.Controls.Window
    {
        public ObservableCollection<Item> ListTitle { get; set; }
        public ObservableCollection<string> Languages { get; set; }
        public ObservableCollection<string> Themes { get; set; }

        public SettingsWindow()
        {
            InitializeComponent();

            ListTitle = new ObservableCollection<Item>
            {
                new Item { Name = "語言設定" },
                new Item { Name = "主題設定" },
                new Item { Name = "軟體更新" },
                new Item { Name = "關於我們" }
            };

            Languages = new ObservableCollection<string> {
                "繁體中文", "English", "Japanese", "Korean"
            };
            Themes = new ObservableCollection<string> {
                "淺色", "深色", "跟隨系統"
            };
            // 設置 DataContext
            this.DataContext = this;
        }

        private void MasterListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MasterListBox.SelectedItem != null)
            {
                var selectedItem = MasterListBox.SelectedItem as Item;
                if (selectedItem != null)
                {
                    if (selectedItem.Name == "語言設定")
                    {
                        settingStackPanel.UpdateLayout();
                        var offset = languageSettings.TranslatePoint(new Point(0, 0), settingStackPanel).Y;
                        settingScrollViewer.ScrollToVerticalOffset(offset);
                    }
                    else if (selectedItem.Name == "主題設定")
                    {
                        settingStackPanel.UpdateLayout();
                        var offset = themeSettings.TranslatePoint(new Point(0, 0), settingStackPanel).Y;
                        settingScrollViewer.ScrollToVerticalOffset(offset);
                    }
                    else if (selectedItem.Name == "軟體更新")
                    {
                        settingStackPanel.UpdateLayout();
                        var offset = updateSettings.TranslatePoint(new Point(0, 0), settingStackPanel).Y;
                        settingScrollViewer.ScrollToVerticalOffset(offset);
                    }
                    else if (selectedItem.Name == "關於我們")
                    {
                        settingStackPanel.UpdateLayout();
                        var offset = aboutUsSettings.TranslatePoint(new Point(0, 0), settingStackPanel).Y;
                        settingScrollViewer.ScrollToVerticalOffset(offset);
                    }
                }
            }
        }
    }

    public class Item
    {
        public string? Name { get; set; }
    }
}