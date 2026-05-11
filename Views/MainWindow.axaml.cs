using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace ParetoApp
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<TaskItem> UnassignedTasksList { get; } = new();
        public ObservableCollection<TaskItem> CriticalTasksList { get; } = new();
        public ObservableCollection<TaskItem> MajorTasksList { get; } = new();
        public ObservableCollection<TaskItem> MinorTasksList { get; } = new();
        public ObservableCollection<TaskItem> DeferredTasksList { get; } = new();

        public MainWindow()
        {
            InitializeComponent();

            UnassignedTasks.ItemsSource = UnassignedTasksList;
            CriticalTasks.ItemsSource = CriticalTasksList;
            MajorTasks.ItemsSource = MajorTasksList;
            MinorTasks.ItemsSource = MinorTasksList;
            DeferredTasks.ItemsSource = DeferredTasksList;

            CriticalTasks.AddHandler(DragDrop.DropEvent, OnTaskDropped);
            MajorTasks.AddHandler(DragDrop.DropEvent, OnTaskDropped);
            MinorTasks.AddHandler(DragDrop.DropEvent, OnTaskDropped);
            DeferredTasks.AddHandler(DragDrop.DropEvent, OnTaskDropped);

            UnassignedTasks.AddHandler(DragDrop.DropEvent, OnTaskDropped);
            CardCritical.AddHandler(DragDrop.DropEvent, OnTaskDropped);
            CardMajor.AddHandler(DragDrop.DropEvent, OnTaskDropped);
            CardMinor.AddHandler(DragDrop.DropEvent, OnTaskDropped);
            CardDeferred.AddHandler(DragDrop.DropEvent, OnTaskDropped);

            TrashCan.AddHandler(DragDrop.DropEvent, OnTaskDropped);
        }

        private void OnAddTaskClicked(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TaskInput.Text))
            {
                UnassignedTasksList.Add(new TaskItem { Text = TaskInput.Text });
                TaskInput.Text = string.Empty;
            }
        }

        private async void OnPlanMyWeekClicked(object sender, RoutedEventArgs e)
        {
            var planWindow = new PlanMyWeekWindow(CriticalTasksList, MajorTasksList, MinorTasksList, DeferredTasksList);
            await planWindow.ShowDialog(this);
        }

        private async void OnTaskPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (sender is Control control && control.DataContext is TaskItem task)
            {
                var pointerPoint = e.GetCurrentPoint(this);

                if (pointerPoint.Properties.IsLeftButtonPressed)
                {
                    var dragData = new DataObject();
                    dragData.Set("application/pareto-task", task);

                    await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
                }
            }
        }

        private void OnTaskDropped(object sender, DragEventArgs e)
        {
            if (e.Data.Contains("application/pareto-task") && e.Data.Get("application/pareto-task") is TaskItem task)
            {
                if (sender is Control targetControl && targetControl.Name != null)
                {
                    if (targetControl.Name == "TrashCan")
                    {
                        bool removed = UnassignedTasksList.Remove(task) ||
                                       CriticalTasksList.Remove(task) ||
                                       MajorTasksList.Remove(task) ||
                                       MinorTasksList.Remove(task) ||
                                       DeferredTasksList.Remove(task);

                        e.DragEffects = DragDropEffects.Move;
                        e.Handled = true;
                        return;
                    }

                    ObservableCollection<TaskItem>? targetList = targetControl.Name switch
                    {
                        "CardCritical" or "CriticalTasks" => CriticalTasksList,
                        "CardMajor" or "MajorTasks" => MajorTasksList,
                        "CardMinor" or "MinorTasks" => MinorTasksList,
                        "CardDeferred" or "DeferredTasks" => DeferredTasksList,
                        "UnassignedTasks" or "CardUnassigned" => UnassignedTasksList,
                        _ => null
                    };

                    if (targetList != null)
                    {
                        bool removed = UnassignedTasksList.Remove(task) ||
                                       CriticalTasksList.Remove(task) ||
                                       MajorTasksList.Remove(task) ||
                                       MinorTasksList.Remove(task) ||
                                       DeferredTasksList.Remove(task);

                        targetList.Add(task);

                        e.DragEffects = DragDropEffects.Move;
                        e.Handled = true;
                    }
                }
            }
        }
    }
}
