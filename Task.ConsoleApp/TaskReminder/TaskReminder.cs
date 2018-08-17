using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.ConsoleApp
{
    public interface ITaskReminder
    {
        void Process();
    }

    internal class TaskReminder<T> : ITaskReminder, IDisposable
    where T : IOptions //, IDisposable
    {
        protected T _Opts;
        protected IEnumerable<TaskLib.Task> _Tasks;
        public TaskReminder(T opts, bool proccessImmediately = true)
        {
            _Opts = opts;
            if(proccessImmediately)
                Process();
        }
        public virtual void Process()
        {
            if(_Opts.Hidden)
            {
                HideConsoleWindow.HideCurentConsoleWindow();
            }
        }

        public void WatchTasks(params TaskLib.Task[] tasks)
        {
            _Tasks = tasks;
            foreach(TaskLib.Task task in _Tasks.Where(t => t.IsComplited != true))
            {
                Console.WriteLine($"{DateTime.Now} Start observe {task}");
                task.OnTaskTime += (s, e) => InvokeTaskComand((TaskLib.Task)s);
                task.StartObserve();
            }
        }

        public void InvokeTaskComand(TaskLib.Task task)
        {
            Console.WriteLine($"{DateTime.Now} Start executing task:'{task.Name}' command:'{_Opts.CmdCommandText}'");

            string command = _Opts.GetTaskCmdCommand(task);//_Opts.CmdCommandText.Replace(CmdArgs.TaskNameReplacer, $"{task.Name}");
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo("cmd", @"/c " + command);
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();

            void streamShow(System.IO.StreamReader streamReader)
            {
                if(streamReader.ReadToEnd() is string streamData && !string.IsNullOrEmpty(streamData))
                    Console.WriteLine(streamData);
            }
            streamShow(process.StandardOutput);
            streamShow(process.StandardError);
        }

        public virtual void Dispose()
        {

        }
        protected void DisposeTasks()
        {
            _Tasks?.ToList().ForEach(t => t.Dispose());
        }
    }
}
