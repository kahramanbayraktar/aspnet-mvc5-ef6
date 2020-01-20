using System.Collections.Generic;

namespace Ced.Web.Models.Task
{
    public class TaskListModel
    {
        public IList<TaskListItemModel> Tasks { get; set; }
    }
}