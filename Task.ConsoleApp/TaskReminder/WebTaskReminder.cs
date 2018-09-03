using System;
using System.Collections.Generic;
using System.Linq;
using Task.ConsoleApp.Utils;
using Task.ConsoleApp.Web;

namespace Task.ConsoleApp
{
    class WebTaskReminder : TaskReminder<IWebTasksOptions>, IDisposable
    {
        Func<string, int, IWebClient> _webclientCtor;
        IWebClient _WebclientCtor;

        public WebTaskReminder(IWebTasksOptions arguments, Func<string, int, IWebClient> webclientCtor) : base(arguments, false)
        {
            _webclientCtor = webclientCtor;
            this.Process();
        }
        public override void Process()
        {
            base.Process();

            _WebclientCtor = _webclientCtor(_Opts.URI, _Opts.ReconnectPeriod);
            _WebclientCtor.OnConnected += _WebclientCtor_OnConnected;
            _WebclientCtor.OnConnectError += _WebclientCtor_OnConnectError;
            _WebclientCtor.OnReceiveError += _WebclientCtor_OnReceiveError;
            _WebclientCtor.OnReceiveMessage += _WebclientCtor_OnReceiveMessage;
            _WebclientCtor.ConnectToServerAsync();

            //WatchTasks(task);
        }

        private void _WebclientCtor_OnReceiveMessage(object sender, string e)
        {
            Log.InfoAsync(e);

            try
            {
                Message message = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(e);

                switch(message.Operation)
                {
                    case OperationType.init:
                        OnInitMessage(message.Tasks);
                        break;
                    case OperationType.add:
                        OnAddMessage(message.Tasks);
                        break;
                    case OperationType.update:
                        OnUpdateMessage(message.Tasks.FirstOrDefault());
                        break;
                    case OperationType.delete:
                        OnDeleteMessage(message.Tasks.FirstOrDefault().TaskId);
                        break;
                }
            }
            catch(Exception ex)
            {
                Log.ErrorAsync(ex.GetMessages());
            }
        }
        void OnInitMessage(IList<TaskLib.Task> tasks)
        {
            WatchTasks(tasks.ToArray());
        }
        void OnAddMessage(IList<TaskLib.Task> tasks)
        {
            AddTasksToWatch(tasks.ToArray());
        }
        void OnUpdateMessage(TaskLib.Task task)
        {
            RemoveTask(task.TaskId);
            AddTasksToWatch(task);
        }
        void OnDeleteMessage(int taskId)
        {
            RemoveTask(taskId);
        }

        private void _WebclientCtor_OnReceiveError(object sender, Exception e)
        {
            Log.ErrorAsync(e.GetMessages());
        }

        private void _WebclientCtor_OnConnectError(object sender, Exception e)
        {
            Log.ErrorAsync(e.GetMessages());
        }

        private void _WebclientCtor_OnConnected(object sender, EventArgs e)
        {
            Log.InfoAsync("Connected.");
        }

        public override void Dispose()
        {
            _WebclientCtor.Dispose();
            base.Dispose();
        }
    }

}

namespace Task.ConsoleApp.Web
{
    public class Message
    {
        public OperationType Operation { get; set; }
        public IList<TaskLib.Task> Tasks { get; set; }
    }
    public enum OperationType
    {
        init,
        add,
        update,
        delete,
    }
}