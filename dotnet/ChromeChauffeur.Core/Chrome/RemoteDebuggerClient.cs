using System;
using System.Linq;
using System.Threading;
using ChromeChauffeur.Core.Chrome.Api;
using ChromeChauffeur.Core.Exceptions;
using ChromeChauffeur.Core.Infrastructure.Http;
using Newtonsoft.Json;
using WebSocket4Net;
using JsonSerializer = ChromeChauffeur.Core.Infrastructure.Json.JsonSerializer;

namespace ChromeChauffeur.Core.Chrome
{
    public class RemoteDebuggerClient : IDisposable
    {
        private readonly JsonSerializer _jsonSerializer;

        private string _webSocketUrl;
        private WebSocket _socket;

        public RemoteDebuggerClient()
        {
            _jsonSerializer = new JsonSerializer();
        }

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(15);

        public string SendExpressionCommand(string expression)
        {
            var command = new RemoteDebuggerCommand { Params = { Expression = expression } };

            var json = _jsonSerializer.Serialize(command);

            var jsonResponse = SendRawCommand(json);

            var response = _jsonSerializer.Deserialize<RemoteDebuggerCommandResponse>(jsonResponse);

            return response.Result.Result.Value;
        }

        public string SendRawCommand(string command)
        {
            var waitEvent = new ManualResetEvent(false);

            string message = "";
            byte[] receivedData;

            Exception exception = null;

            _socket.MessageReceived += (o, e) =>
            {
                message = e.Message;
                waitEvent.Set();
            };

            _socket.Error += (o, e) =>
            {
                exception = e.Exception;
                waitEvent.Set();
            };

            _socket.DataReceived += (o, e) =>
            {
                receivedData = e.Data;
                waitEvent.Set();
            };

            _socket.Send(command);

            waitEvent.WaitOne(Timeout);

            if (exception != null)
                throw new ChromeChauffeurException("Socket communication failed with Chrome remote debugger. See inner exception for details.", exception);

            return message;
        }

        public void Connect(string hostName, int portNumber)
        {
            string url = $"http://{hostName}:{portNumber}/json";

            var response = HttpClient.Get(url);

            ValidateSessionsResponse(response);

            InitializeWebSocketUrl(response);

            EstablishWebSocketConnection();
        }

        private void EstablishWebSocketConnection()
        {
            var waitEvent = new ManualResetEvent(false);

            _socket = new WebSocket(_webSocketUrl);
            _socket.Opened += (o, e) => waitEvent.Set();

            _socket.Open();

            waitEvent.WaitOne(Timeout);
        }

        private static void ValidateSessionsResponse(HttpResponse response)
        {
            if (response.StatusCode != 200)
            {
                throw new ChromeChauffeurException(
                    $"Failed to retrieve remote debugging session information from Chrome from url '{response.RequestedUrl}'." +
                    $"Status code: {response.StatusCode}, description: {response.StatusDescription}");
            }
        }

        private void InitializeWebSocketUrl(HttpResponse response)
        {
            var sessions = JsonConvert.DeserializeObject<RemoteSessionsResponse[]>(response.Body);

            var automationSession = sessions.FirstOrDefault(x => x.Url == "chrome://newtab/");

            if (automationSession == null)
            {
                throw new ChromeChauffeurException(
                    "Failed to find suitable remote debuggins session to automate. Available sessions: " + Environment.NewLine + 
                    string.Join(Environment.NewLine, sessions.Select(s => s.Url)));
            }

            _webSocketUrl = automationSession.WebSocketDebuggerUrl;
        }

        public void Dispose()
        {
            _socket?.Dispose();
        }
    }
}