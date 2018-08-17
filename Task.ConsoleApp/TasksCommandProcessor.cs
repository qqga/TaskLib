using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.ConsoleApp
{
    class TasksCommandProcessor
    {
        readonly TaskLib.Task[] _Tasks;
        readonly IOptions _Opts;
        public TasksCommandProcessor(IOptions opts, params TaskLib.Task[] tasks)
        {
            _Opts = opts;
            _Tasks = tasks;
        }

        public void WatchTasks()
        {
            foreach(TaskLib.Task task in _Tasks.Where(t => t.IsComplited != true))
            {
                Console.WriteLine($"Start observe {task}");
                task.OnTaskTime += (s, e) => InvokeTaskComand(_Opts, (TaskLib.Task)s);
                task.StartObserve();
            }
        }

        public static void InvokeTaskComand(IOptions opts, TaskLib.Task task)
        {
            //foreach(TaskLib.Task task in tasks)
            {
                Console.WriteLine($"start executing task({task.Name}) command({opts.CmdCommandText})");

                string command = opts.CmdCommandText.Replace(CmdArgs.TaskNameReplacer, $"{task.Name}");
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
        }
    }
}
