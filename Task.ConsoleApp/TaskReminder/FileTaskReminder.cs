using System;
using System.IO;
using System.Linq;

namespace Task.ConsoleApp
{

    class FileTaskReminder : TaskReminder<IFileOptions>, IDisposable
    {
        public int FileReadErrorTimeWait = 2000;
        FileSystemWatcher _FileSystemWatcher;
        public FileTaskReminder(IFileOptions opts) : base(opts)
        {
        }
        public override void Process()
        {
            base.Process();

            LoadTasksAndWatch();

            if(_Opts.WatchFileChange)
                WatchFileChange();
        }

        private void LoadTasksAndWatch()
        {
            Log.InfoAsync($"Loading tasks from file '{_Opts.FileName}'");
            TaskLib.Task[] tasks = GetFileTasks();
            WatchTasks(tasks);
        }

        private void ReLoadTasksAndWatch()
        {
            base.DisposeTasks();
            LoadTasksAndWatch();
        }

        private TaskLib.Task[] GetFileTasks()
        {
            if(!File.Exists(_Opts.FileName))
                throw new InvalidOperationException("File not found");

            string text = null;
            while((text = ReadFileTasks()) == null) { }

            var tasks = Newtonsoft.Json.JsonConvert.DeserializeObject<TaskLib.Task[]>(text);
            return tasks;
        }
        string ReadFileTasks()
        {
            string text = null;
            try
            {
                text = File.ReadAllText(_Opts.FileName);
            }
            catch(System.IO.IOException ioEx)
            {
                Log.InfoAsync(ioEx.Message);
                Log.InfoAsync($"Wait file {FileReadErrorTimeWait}ms...");
                System.Threading.Tasks.Task.Delay(FileReadErrorTimeWait).Wait();
            }

            return text;
        }

        #region FileSystemWatcher
        void WatchFileChange()
        {
            string path = Path.GetDirectoryName(_Opts.FileName);
            string filter = Path.GetFileName(_Opts.FileName);
            _FileSystemWatcher = new FileSystemWatcher(path, filter);
            _FileSystemWatcher.Changed += _FileSystemWatcher_Changed;
            _FileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _FileSystemWatcher.EnableRaisingEvents = true;
        }

        private void _FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if(e.ChangeType == WatcherChangeTypes.Changed && IsRealFileChanged(e.FullPath))
            {
                Log.InfoAsync($"File Changed");
                ReLoadTasksAndWatch();
            }
        }

        DateTime lastRead = DateTime.MinValue;
        bool IsRealFileChanged(string file)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(file);
            if(lastWriteTime != lastRead)
            {
                lastRead = lastWriteTime;
                return true;
            }
            return false;
        }
        #endregion

        public override void Dispose()
        {
            _FileSystemWatcher.EnableRaisingEvents = false;
            _FileSystemWatcher?.Dispose();
            base.Dispose();
        }
    }
}
