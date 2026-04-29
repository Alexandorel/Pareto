using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;

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

    public class DayPlan
    {
        public string DayName { get; set; } = string.Empty;
        public ObservableCollection<PlannedTask> Tasks { get; set; } = new();
    }

    public partial class PlanMyWeekWindow : Window
    {
        public ObservableCollection<DayPlan> WeekDays { get; set; } = new();
        public ObservableCollection<PlannedTask> MasterTasks { get; set; } = new();

        public PlanMyWeekWindow()
        {
            InitializeComponent();
        }

        public PlanMyWeekWindow(
            ObservableCollection<string> critical,
            ObservableCollection<string> major,
            ObservableCollection<string> minor,
            ObservableCollection<string> deferred) : this()
        {
            var allTasks = new List<string>();
            allTasks.AddRange(critical);
            allTasks.AddRange(major);
            allTasks.AddRange(minor);
            allTasks.AddRange(deferred);

            foreach (var t in allTasks)
            {
                MasterTasks.Add(new PlannedTask { TaskName = t });
            }

            var days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            foreach (var day in days)
            {
                var dayPlan = new DayPlan { DayName = day };
                foreach (var t in allTasks)
                {
                    dayPlan.Tasks.Add(new PlannedTask { TaskName = t });
                }
                WeekDays.Add(dayPlan);
            }

            DataContext = this;
        }

        private void OnApplyToAllClicked(object sender, RoutedEventArgs e)
        {
            foreach (var day in WeekDays)
            {
                foreach (var task in day.Tasks)
                {
                    foreach (var masterTask in MasterTasks)
                    {
                        if (masterTask.TaskName == task.TaskName && !string.IsNullOrWhiteSpace(masterTask.TimeInterval))
                        {
                            task.TimeInterval = masterTask.TimeInterval;
                        }
                    }
                }
            }
        }

        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void OnCopyToClipboardClicked(object sender, RoutedEventArgs e)
        {
            foreach (var day in WeekDays)
            {
                foreach (var task in day.Tasks)
                {
                    if (string.IsNullOrWhiteSpace(task.TimeInterval))
                    {
                        var master = MasterTasks.FirstOrDefault(m => m.TaskName == task.TaskName);
                        if (master != null && !string.IsNullOrWhiteSpace(master.TimeInterval))
                        {
                            task.TimeInterval = master.TimeInterval;
                        }
                    }
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine("# Pareto Weekly Plan\n");

            foreach (var day in WeekDays)
            {
                sb.AppendLine($"## {day.DayName}");
                foreach (var task in day.Tasks)
                {
                    string timeInfo = string.IsNullOrWhiteSpace(task.TimeInterval) ? "" : $" (⏱️ {task.TimeInterval})";
                    sb.AppendLine($"- {task.TaskName}{timeInfo}");
                }
                sb.AppendLine();
            }

            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard != null)
            {
                await clipboard.SetTextAsync(sb.ToString().TrimEnd());
            }
        }
    }
}