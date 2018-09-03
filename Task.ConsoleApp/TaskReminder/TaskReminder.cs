using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task.ConsoleApp.Utils;

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
        protected List<TaskLib.Task> _Tasks = new List<TaskLib.Task>();
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
            if(_Tasks.Count > 0)
            {
                DisposeTasks();
                _Tasks.Clear();
            }

            AddTasksToWatch(tasks);
        }
        public void AddTasksToWatch(params TaskLib.Task[] tasks)
        {
            foreach(TaskLib.Task task in tasks.Where(t => t.IsComplited != true))
            {
                AddTaskToWatch(task);
            }
        }
        public void RemoveTask(int taskId)
        {
            TaskLib.Task task = _Tasks.FirstOrDefault(t => t.TaskId == taskId);
            Log.InfoAsync($"remove task({task}).");
            _Tasks.Remove(task);
            task.Dispose();
        }
        public bool AddTaskToWatch(TaskLib.Task task)
        {
            _Tasks.Add(task);

            if(task.IsComplited == true)
                return false;

            Log.InfoAsync($"Start observe {task}");
            task.OnTaskTime += (s, e) => InvokeTaskComand((TaskLib.Task)s);
            task.StartObserve();

            return true;
        }

        public void InvokeTaskComand(TaskLib.Task task)
        {
            try
            {
                string command = _Opts.GetTaskCmdCommand(task);

                //System.Threading.Tasks.Task.Factory.StartNew(() => Console.WriteLine($"{DateTime.Now} Start executing task:'{task.Name}' command:'{command}'"));
                Log.InfoAsync($"Start executing task:'{task.Name}' command:'{command}'");

                //_Opts.CmdCommandText.Replace(CmdArgs.TaskNameReplacer, $"{task.Name}");
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
                        Log.InfoAsync(streamData);
                }
                streamShow(process.StandardOutput);
                streamShow(process.StandardError);
            }
            catch(Exception ex)
            {
                Log.ErrorAsync(ex.GetMessages());
            }
        }

        public virtual void Dispose()
        {
            DisposeTasks();
        }
        protected void DisposeTasks()
        {
            _Tasks?.ForEach(t => t.Dispose());
        }
    }
}
