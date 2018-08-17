using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace TaskLib
{
    public interface TaskRepo
    {
        IQueryable<Task> GetTasks();
        IQueryable<Task> GetTasks(DateTime dateTime);
        void AddTask(Task task);
        void UpdateTask(Task task);
        void DeleteTask(int taskId);
    }
}
