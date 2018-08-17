using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.ConsoleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.Title = "tasks";

            ITaskReminder taskReminder = null;

            var res = CommandLine.Parser.Default
                .ParseArguments<FileCmdArgs, NewTaskCmdArgs, CmdArgs>(args)
                .MapResult(
                 (FileCmdArgs fileArgs) => { taskReminder = new FileTaskReminder(fileArgs); return 1; },
                 (NewTaskCmdArgs newTaskArgs) => { taskReminder = new NewTaskReminder(newTaskArgs); return 1; },
                 HandleParseError);

            Console.WriteLine("press 'E'- to exit; 'H'- to hide console.");

            bool processCommnads = true;
            while(processCommnads)
            {
                processCommnads = ProcessAdditionalCommands(Console.ReadKey());
            }

            ((IDisposable)taskReminder)?.Dispose();

            return res;
        }

        static bool ProcessAdditionalCommands(ConsoleKeyInfo readKey)
        {
            switch(readKey.Key)
            {
                case ConsoleKey.E: return false;
                case ConsoleKey.H: HideConsoleWindow.HideCurentConsoleWindow(); break;
            }

            return true;
        }

        static int HandleParseError(IEnumerable<Error> errs)
        {
            Environment.Exit(10);
            return 10;
        }

    }
}
