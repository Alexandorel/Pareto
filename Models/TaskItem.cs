using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ParetoApp
{
    public class TaskItem : INotifyPropertyChanged
    {
        private string _text = string.Empty;
        private bool _completed = false;

        public string Text
        {
            get => _text;
            set { _text = value; OnPropertyChanged(); }
        }

        public bool Completed
        {
            get => _completed;
            set { _completed = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
