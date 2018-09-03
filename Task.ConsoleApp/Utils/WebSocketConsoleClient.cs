using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task.ConsoleApp.Utils
{
    public interface IWebClient : IDisposable
    {
        System.Threading.Tasks.Task ConnectToServerAsync();
        void SendMessageAsync(string message);
        string Uri { get; }
        event EventHandler OnConnected;
        event EventHandler<Exception> OnConnectError;
        event EventHandler<Exception> OnReceiveError;
        event EventHandler<string> OnReceiveMessage;
    }

    class WebSocketConsoleClient : IWebClient, IDisposable
    {
        public ClientWebSocket Client { get; private set; }
        public CancellationTokenSource Cts { get; }

        public event EventHandler<string> OnReceiveMessage;
        public event EventHandler<Exception> OnReceiveError;
        public event EventHandler<Exception> OnConnectError;
        public event EventHandler OnConnected;

        public string Uri { get; }
        public int ReconnectPerioid { get; set; }
        public WebSocketConsoleClient(string uri, int reconnectPerioid = -1)
        {
            Client = new ClientWebSocket();
            Cts = new CancellationTokenSource();
            Uri = uri;
            ReconnectPerioid = reconnectPerioid;
        }

        public async System.Threading.Tasks.Task ConnectToServerAsync()
        {
            try
            {
                await Client.ConnectAsync(new Uri(Uri), Cts.Token);
                OnConnected?.Invoke(this, EventArgs.Empty);
                await System.Threading.Tasks.Task.Factory.StartNew(async () =>
                {
                    while(true)
                    {
                        try
                        {
                            await ReadMessageAsync();
                        }
                        catch(Exception ex)
                        {
                            OnReceiveError?.Invoke(this, ex);
                            RecconectWithPeroidASync();
                            return;
                        }
                    }
                }, Cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            catch(Exception ex)
            {
                OnReceiveError?.Invoke(this, ex);
                RecconectWithPeroidASync();
                return;
            }
        }

        async void RecconectWithPeroidASync()
        {
            if(ReconnectPerioid > 0)
            {
                Log.InfoAsync($"Wait {ReconnectPerioid} before reconnect.");
                await System.Threading.Tasks.Task.Delay(ReconnectPerioid);
                await ReconnectToServerAsync();
            }
        }
        async System.Threading.Tasks.Task ReconnectToServerAsync()
        {
            try
            {
                await Client.CloseAsync(WebSocketCloseStatus.Empty, "reconnect", CancellationToken.None);
                Client.Dispose();
            }
            catch(Exception ex)
            {

            }
            Log.InfoAsync("Try Reconnect");
            Client = new ClientWebSocket();
            await ConnectToServerAsync();
        }
        async System.Threading.Tasks.Task ReadMessageAsync()
        {
            WebSocketReceiveResult result;
            string receivedMessage=string.Empty;
            var message = new ArraySegment<byte>(new byte[4096]);
            do
            {
                result = await Client.ReceiveAsync(message, Cts.Token);
                
                if(result.MessageType != WebSocketMessageType.Text)
                    break;

                var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                receivedMessage += Encoding.UTF8.GetString(messageBytes);
                
                //Console.WriteLine("Received: {0}", receivedMessage);
            }
            while(!result.EndOfMessage);

            System.Threading.Tasks.Task.Factory.StartNew(()=> { OnReceiveMessage?.Invoke(this, receivedMessage); } ); 
        }

        public async void SendMessageAsync(string message)
        {
            var byteMessage = Encoding.UTF8.GetBytes(message);
            var segmnet = new ArraySegment<byte>(byteMessage);
            await Client.SendAsync(segmnet, WebSocketMessageType.Text, true, Cts.Token);
        }

        public async void Dispose()
        {
            if(Client != null)
            {
                await Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Object disposing closure.", Cts.Token);
                Client.Dispose();
            }
        }
    }
}
