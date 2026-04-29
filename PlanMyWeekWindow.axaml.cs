using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ParetoApp
{
    public partial class PlanMyWeekWindow : Window
    {
        public PlanMyWeekWindow()
        {
            InitializeComponent();
        }

        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}