using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ParetoApp
{
    public partial class PlanMyWeekWindow : Window
    {
        public ObservableCollection<DayPlan> WeekDays { get; set; } = new();
        public ObservableCollection<PlannedTask> MasterTasks { get; set; } = new();

        public PlanMyWeekWindow()
        {
            InitializeComponent();
        }

        public PlanMyWeekWindow(
            ObservableCollection<TaskItem> critical,
            ObservableCollection<TaskItem> major,
            ObservableCollection<TaskItem> minor,
            ObservableCollection<TaskItem> deferred) : this()
        {
            var allTasks = new List<string>();
            allTasks.AddRange(critical.Select(t => t.Text));
            allTasks.AddRange(major.Select(t => t.Text));
            allTasks.AddRange(minor.Select(t => t.Text));
            allTasks.AddRange(deferred.Select(t => t.Text));

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