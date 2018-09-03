using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task.ConsoleApp
{
    class Log
    {
        const ConsoleColor ERROR_COCLOR = ConsoleColor.Red;
        const ConsoleColor INFO_COCLOR = ConsoleColor.Green;

        public static async System.Threading.Tasks.Task LogAsync(string message)
        {
            await System.Threading.Tasks.Task.Factory.StartNew(
                () => Console.WriteLine($"{DateTime.Now} {message}"));
        }


        public static async System.Threading.Tasks.Task WriteLineAsync(string message)
        {
            await System.Threading.Tasks.Task.Factory.StartNew(
                async () => await LogAsync($"INFO: {message}"));
        }

        public static async System.Threading.Tasks.Task InfoAsync(string message)
        {
            await System.Threading.Tasks.Task.Factory.StartNew(
                async () =>
                {
                    using(LogColor.Set(INFO_COCLOR))
                    {
                        await LogAsync($"INFO: {message}");
                    }
                });
        }

        public static async System.Threading.Tasks.Task ErrorAsync(string message)
        {
            await System.Threading.Tasks.Task.Factory.StartNew(
                async () =>
                {

                    using(LogColor.Set(ERROR_COCLOR))
                    {
                        await LogAsync($"Error: {message}");
                    }
                });
        }


        internal class LogColor : IDisposable
        {
            public static LogColor Set(ConsoleColor consoleColor)
            {
                return new LogColor(consoleColor);
            }

            ConsoleColor _OriginConsoleColor;
            ConsoleColor _NewConsoleColor;

            public LogColor(ConsoleColor consoleColor)
            {
                _NewConsoleColor = consoleColor;
                _OriginConsoleColor = Console.ForegroundColor;
                Console.ForegroundColor = _NewConsoleColor;
            }

            public void Dispose()
            {
                Console.ForegroundColor = _OriginConsoleColor;
            }
        }
    }
}
