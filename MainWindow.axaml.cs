using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace ParetoApp
{
    public partial class MainWindow : Window
    {
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
                UnassignedTasksList.Add(TaskInput.Text);
                TaskInput.Text = string.Empty;
            }
        }

        private async void OnTaskPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (sender is Control control && control.DataContext is string taskText)
            {
                var pointerPoint = e.GetCurrentPoint(this);

                if (pointerPoint.Properties.IsLeftButtonPressed)
                {
                    var dragData = new DataObject();
                    dragData.Set("application/pareto-task", taskText);
                    
                    // Așteptăm să vedem ce s-a întâmplat cu operațiunea de Drag & Drop
                    var result = await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);

                    // Dacă Drop-ul a fost într-o zonă nepermisă sau a fost anulat (rezultând None), ștergem task-ul
                    if (result == DragDropEffects.None)
                    {
                        bool removed = UnassignedTasksList.Remove(taskText) ||
                                       CriticalTasksList.Remove(taskText) ||
                                       MajorTasksList.Remove(taskText) ||
                                       MinorTasksList.Remove(taskText) ||
                                       DeferredTasksList.Remove(taskText);
                    }
                }
            }
        }

        private void OnTaskDropped(object sender, DragEventArgs e)
        {
            if (e.Data.Contains("application/pareto-task") && e.Data.Get("application/pareto-task") is string taskText)
            {
                if (sender is Control targetControl && targetControl.Name != null)
                {
                    // Dacă task-ul este dat drumul pe Coșul de Gunoi
                    if (targetControl.Name == "TrashCan")
                    {
                        bool removed = UnassignedTasksList.Remove(taskText) ||
                                       CriticalTasksList.Remove(taskText) ||
                                       MajorTasksList.Remove(taskText) ||
                                       MinorTasksList.Remove(taskText) ||
                                       DeferredTasksList.Remove(taskText);

                        e.DragEffects = DragDropEffects.Move; // Confirmăm succesul ca să nu existe delay de OS!
                        e.Handled = true;
                        return;
                    }

                    ObservableCollection<string>? targetList = targetControl.Name switch
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
                        bool removed = UnassignedTasksList.Remove(taskText) ||
                                       CriticalTasksList.Remove(taskText) ||
                                       MajorTasksList.Remove(taskText) ||
                                       MinorTasksList.Remove(taskText) ||
                                       DeferredTasksList.Remove(taskText);

                        targetList.Add(taskText);
                        
                        e.DragEffects = DragDropEffects.Move; // Confirmăm sistemului că mutarea a avut succes
                        e.Handled = true;
                    }
                }
            }
        }
    }
}