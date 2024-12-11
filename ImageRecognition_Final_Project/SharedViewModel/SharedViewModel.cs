using System.ComponentModel;

namespace ImageRecognition_Final_Project
{
    public class SharedViewModel : INotifyPropertyChanged
    {
        private bool _isChecked1;
        public bool IsChecked1
        {
            get => _isChecked1;
            set
            {
                _isChecked1 = value;
                OnPropertyChanged(nameof(IsChecked1));
            }
        }

        private bool _isChecked2;
        public bool IsChecked2
        {
            get => _isChecked2;
            set
            {
                _isChecked2 = value;
                OnPropertyChanged(nameof(IsChecked2));
            }
        }

        private bool _isChecked3;
        public bool IsChecked3
        {
            get => _isChecked3;
            set
            {
                _isChecked3 = value;
                OnPropertyChanged(nameof(IsChecked3));
            }
        }

        private bool _isChecked4;
        public bool IsChecked4
        {
            get => _isChecked4;
            set
            {
                _isChecked4 = value;
                OnPropertyChanged(nameof(IsChecked4));
            }
        }

        private bool _isChecked5;
        public bool IsChecked5
        {
            get => _isChecked5;
            set
            {
                _isChecked5 = value;
                OnPropertyChanged(nameof(IsChecked5));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}