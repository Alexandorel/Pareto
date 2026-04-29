using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace ParetoApp
{
    public partial class MainWindow : Window
    {
        // Collections for each task category
        public ObservableCollection<string> UnassignedTasksList { get; } = new();
        public ObservableCollection<string> CriticalTasksList { get; } = new();
        public ObservableCollection<string> MajorTasksList { get; } = new();
        public ObservableCollection<string> MinorTasksList { get; } = new();
        public ObservableCollection<string> DeferredTasksList { get; } = new();

        public MainWindow()
        {
            InitializeComponent();
            
            UnassignedTasks.ItemsSource = UnassignedTasksList;
            CriticalTasks.ItemsSource = CriticalTasksList;
            MajorTasks.ItemsSource = MajorTasksList;
            MinorTasks.ItemsSource = MinorTasksList;
            DeferredTasks.ItemsSource = DeferredTasksList;

            CardCritical.AddHandler(DragDrop.DropEvent, OnTaskDropped);
            CardMajor.AddHandler(DragDrop.DropEvent, OnTaskDropped);
            CardMinor.AddHandler(DragDrop.DropEvent, OnTaskDropped);
            CardDeferred.AddHandler(DragDrop.DropEvent, OnTaskDropped);
        }

        private void OnAddTaskClicked(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TaskInput.Text))
            {
                UnassignedTasksList.Add(TaskInput.Text);
                TaskInput.Text = string.Empty;
            }
        }

        private async void OnTaskPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (sender is Control control && control.DataContext is string taskText)
            {
                var dragData = new DataObject();
                dragData.Set("TaskText", taskText);

                await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
            }
        }

        private void OnTaskDropped(object sender, DragEventArgs e)
        {
            if (e.Data.Contains("TaskText") && e.Data.Get("TaskText") is string taskText)
            {
                UnassignedTasksList.Remove(taskText);
                CriticalTasksList.Remove(taskText);
                MajorTasksList.Remove(taskText);
                MinorTasksList.Remove(taskText);
                DeferredTasksList.Remove(taskText);

                if (sender is Border border && border.Name != null)
                {
                    switch (border.Name)
                    {
                        case "CardCritical":
                            CriticalTasksList.Add(taskText);
                            break;
                        case "CardMajor":
                            MajorTasksList.Add(taskText);
                            break;
                        case "CardMinor":
                            MinorTasksList.Add(taskText);
                            break;
                        case "CardDeferred":
                            DeferredTasksList.Add(taskText);
                            break;
                    }
                }
            }
        }
    }
}
