using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.ConsoleApp
{
    public class CmdArgs : IOptions
    {
        public const string TaskNameReplacer = "%TaskName";
        [Option('c', "CmdCommandText", Default = "alert \"%TaskName\"", HelpText = "Cmd Command Text . %TaskName as task name replacer.")]
        public string CmdCommandText { get; set; }

        [Option('h', "Hidden", Default = false, HelpText = "Hide console window")]
        public bool Hidden { get; set; }

        public string GetTaskCmdCommand(TaskLib.Task task) =>
            CmdCommandText.Replace(CmdArgs.TaskNameReplacer, $"{task.Name}");

    }


    [Verb("file", HelpText = "Get tasks from file in json format.")]
    class FileCmdArgs : CmdArgs, IFileOptions
    {
        [Value(0, Default = "tasks.json", HelpText = "File name (with json tasks).")]
        //[Option('f', "FileName", Default = "tasks.json", HelpText = "File name (with json tasks).")]
        public string FileName { get; set; }
        [Option('w', "WatchFileChange", Default = true, HelpText = "Watch file changes and actualize tasks.")]
        public bool WatchFileChange { get; set; }
    }


    [Verb("new", HelpText = "Create tasks (not persistent)")]
    class NewTaskCmdArgs : CmdArgs, INewTaskOptions
    {
        [Value(0, Default = "TaskName", HelpText = "Task name")]
        //[Option('f', "FileName", Default = "tasks.json", HelpText = "File name (with json tasks).")]
        public string TaskName { get; set; }

        [Option('d', "DateTime", HelpText = "Target DateTime.")]
        public string DateTimeStr { get; set; }

        [Option('p', "Period", HelpText = "Task period.")]
        public string PeriodStr { get; set; }

        public DateTime? DateTimeObj => string.IsNullOrEmpty(DateTimeStr) ? null : (DateTime?)DateTime.Parse(DateTimeStr);
        public TimeSpan? PeriodObj => string.IsNullOrEmpty(PeriodStr) ? null : (TimeSpan?)TimeSpan.Parse(PeriodStr);
    }

}
