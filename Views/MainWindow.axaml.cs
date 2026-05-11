using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public ObservableCollection<string> SavedBoards { get; } = new();

        private readonly BoardStorage _storage = new();
        private BoardData? _currentBoard = null;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

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

            foreach (var name in _storage.ListBoards())
            {
                SavedBoards.Add(name);
            }
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

        private void OnSaveAsBoardClicked(object sender, RoutedEventArgs e)
        {
            string name = BoardNameInput.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(name))
            {
                StatusText.Text = "⚠️ Please enter a board name.";
                return;
            }

            try
            {
                var board = new BoardData { Name = name };
                foreach (var t in CriticalTasksList) board.Tasks[Priority.Critical].Add(t);
                foreach (var t in MajorTasksList)    board.Tasks[Priority.Major].Add(t);
                foreach (var t in MinorTasksList)    board.Tasks[Priority.Minor].Add(t);
                foreach (var t in DeferredTasksList) board.Tasks[Priority.Deferred].Add(t);

                _storage.Save(board);
                if (!SavedBoards.Contains(name))
                {
                    SavedBoards.Add(name);
                }
                StatusText.Text = $"✅ Board '{name}' saved.";
                BoardNameInput.Text = string.Empty;
            }
            catch (Exception ex)
            {
                StatusText.Text = $"❌ {ex.Message}";
            }
        }

        private void OnClearClicked(object sender, RoutedEventArgs e)
        {
            UnassignedTasksList.Clear();
            CriticalTasksList.Clear();
            MajorTasksList.Clear();
            MinorTasksList.Clear();
            DeferredTasksList.Clear();
            StatusText.Text = "🧹 General cleared.";
        }

        private void OnBoardClicked(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Content is string boardName)
            {
                try
                {
                    var board = _storage.Load(boardName);
                    if (board == null)
                    {
                        StatusText.Text = $"❌ Board '{boardName}' not found.";
                        return;
                    }

                    UnsubscribeFromCurrentBoard();
                    _currentBoard = board;
                    SubscribeToCurrentBoard();

                    BoardTitleText.Text = $"📋 {boardName}";
                    BoardCriticalList.ItemsSource = board.Tasks[Priority.Critical];
                    BoardMajorList.ItemsSource    = board.Tasks[Priority.Major];
                    BoardMinorList.ItemsSource    = board.Tasks[Priority.Minor];
                    BoardDeferredList.ItemsSource = board.Tasks[Priority.Deferred];

                    GeneralPanel.IsVisible = false;
                    ParetoBoardPanel.IsVisible = true;
                    StatusText.Text = "";
                }
                catch (Exception ex)
                {
                    StatusText.Text = $"❌ {ex.Message}";
                }
            }
        }

        private void OnGeneralClicked(object sender, RoutedEventArgs e)
        {
            UnsubscribeFromCurrentBoard();
            GeneralPanel.IsVisible = true;
            ParetoBoardPanel.IsVisible = false;
            StatusText.Text = "";
        }

        private void SubscribeToCurrentBoard()
        {
            if (_currentBoard == null) return;
            foreach (var list in _currentBoard.Tasks.Values)
                foreach (var task in list)
                    task.PropertyChanged += OnTaskCompletedChanged;
        }

        private void UnsubscribeFromCurrentBoard()
        {
            if (_currentBoard == null) return;
            foreach (var list in _currentBoard.Tasks.Values)
                foreach (var task in list)
                    task.PropertyChanged -= OnTaskCompletedChanged;
            _currentBoard = null;
        }

        private void OnTaskCompletedChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TaskItem.Completed) && _currentBoard != null)
            {
                try
                {
                    _storage.Save(_currentBoard);
                }
                catch (Exception ex)
                {
                    StatusText.Text = $"❌ Auto-save failed: {ex.Message}";
                }
            }
        }
    }
}
