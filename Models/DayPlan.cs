using System.Collections.ObjectModel;

namespace ParetoApp
{
    public class DayPlan
    {
        public string DayName { get; set; } = string.Empty;
        public ObservableCollection<PlannedTask> Tasks { get; set; } = new();
    }
}
