namespace ProjektniMenadzment.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public string? DisplayName { get; set; }
        public bool IsAdminOrPM { get; set; }

        public int TotalProjects { get; set; }
        public int MyProjects { get; set; }
        public int ActiveTasks { get; set; }
        public int OverdueTasks { get; set; }
        public decimal BudgetUsedPercent { get; set; } // 0..100

        public List<ProjectItem> RecentProjects { get; set; } = new();
        public List<TaskItem> UpcomingTasks { get; set; } = new();
        public List<ActivityItem> TeamActivity { get; set; } = new();

        public class ProjectItem
        {
            public Guid Id { get; set; }
            public string Naziv { get; set; } = "";
            public string Status { get; set; } = "";
            public int ProgressPercent { get; set; } // 0..100
            public DateOnly? Rok { get; set; }
        }
        public class TaskItem
        {
            public Guid Id { get; set; }
            public Guid ProjekatId { get; set; }
            public string Title { get; set; } = "";
            public DateOnly? Due { get; set; }
            public string ProjectName { get; set; } = "";
            public bool AssignedToMe { get; set; }
        }
        public class ActivityItem
        {
            public string Text { get; set; } = "";
            public DateTime WhenUtc { get; set; }
        }
    }
}
