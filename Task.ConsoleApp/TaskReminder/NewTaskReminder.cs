using System;
using System.Linq;

namespace Task.ConsoleApp
{
    class NewTaskReminder : TaskReminder<NewTaskCmdArgs>, IDisposable
    {
        public NewTaskReminder(NewTaskCmdArgs arguments) : base(arguments)
        {
        }
        public override void Process()
        {
            base.Process();

            TaskLib.Task task = new TaskLib.Task()
            {
                Name = _Opts.TaskName,
                CreatedDT = DateTime.Now,
                RemindPeriod = _Opts.PeriodObj,
                TargetDT = _Opts.DateTimeObj
            };

            WatchTasks(task);
        }

        public override void Dispose()
        {
            base.DisposeTasks();
        }
    }
}
