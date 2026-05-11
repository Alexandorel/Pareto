using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ParetoApp
{
    public class PlannedTask : INotifyPropertyChanged
    {
        private string _taskName = string.Empty;
        private string _timeInterval = string.Empty;

        public string TaskName
        {
            get => _taskName;
            set { _taskName = value; OnPropertyChanged(); }
        }

        public string TimeInterval
        {
            get => _timeInterval;
            set { _timeInterval = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
