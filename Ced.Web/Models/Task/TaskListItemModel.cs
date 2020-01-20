using System;

namespace Ced.Web.Models.Task
{
    public class TaskListItemModel
    {
        public int TaskId { get; set; }

        public string TaskName { get; set; }

        public string Description { get; set; }

        public string TaskUrl { get; set; }

        public DateTime? LastRunTime { get; set; }

        public bool IsActive { get; set; }
    }
}