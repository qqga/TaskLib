using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.ConsoleApp
{
    public interface IOptions
    {
        string CmdCommandText { get; set; }
        bool Hidden { get; set; }
        string GetTaskCmdCommand(TaskLib.Task task);// todo 
    }

    public interface IFileOptions : IOptions
    {
        string FileName { get; set; }
        bool WatchFileChange { get; set; }
    }

    public interface INewTaskOptions : IOptions
    {
        DateTime? DateTimeObj { get; }
        TimeSpan? PeriodObj { get; }
        string TaskName { get; set; }
    }

    public interface IWebTasksOptions : IOptions
    {        
        string URI { get; set; }
        //string UserKey { get; set; }
        int ReconnectPeriod { get; set; }
    }
}
