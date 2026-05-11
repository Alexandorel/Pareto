using System;
using System.Collections.Generic;

namespace ParetoApp
{
    public class BoardData
    {
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Dictionary<Priority, List<TaskItem>> Tasks { get; set; } = new()
        {
            { Priority.Critical, new List<TaskItem>() },
            { Priority.Major,    new List<TaskItem>() },
            { Priority.Minor,    new List<TaskItem>() },
            { Priority.Deferred, new List<TaskItem>() }
        };
    }
}
