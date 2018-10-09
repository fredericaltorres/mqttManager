using MqttLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTManagerLib
{
    /// <summary>
    /// Based on MQTT Server : https://www.cloudmqtt.com/docs-dotnet.html
    /// Mqtt lib : https://github.com/stevenlovegrove/MqttDotNet
    /// </summary>
    public class MQTTManager : IDisposable
    {
        private string _connectionString;
        private string _clientId;
        private IMqtt _client;

        public delegate void NotificationEventType(string message);
        public delegate void MessageArrivedType(MQTTMessage message);
        public event NotificationEventType NotificationEvent;
        public event MessageArrivedType MessageArrived;

        private void Notify(string m)
        {
            if(NotificationEvent != null)
                NotificationEvent(m);
        }

        public MQTTManager(string connectionString, string clientId, string userName, string password)
        {
            this._connectionString = connectionString;
            this._clientId = clientId;
            this._client = MqttClientFactory.CreateClient(connectionString, _clientId, userName, password);
            this._client.Connected += new ConnectionDelegate(client_Connected);
            this._client.ConnectionLost += new ConnectionDelegate(_client_ConnectionLost);
            this._client.PublishArrived += new PublishArrivedDelegate(client_PublishArrived);
        }

        public static string BuildClientId()
        {
            var g = Guid.NewGuid().ToString().Split('-');
            return $"{Environment.MachineName}-{g[0]}";
        }

        public void Start(string channel)
        {
            Notify("Client connecting");
            this._client.Connect(true);
            this.Publish(channel, $"New console instance connected");
        }

        public void Stop()
        {
            if (this._client.IsConnected)
            {
                this.Notify("Client disconnecting");
                this._client.Disconnect();
                this.Notify("Client disconnected");
            }
        }
        public void Subscribe(string channel)
        {
            var r = this._client.Subscribe(channel, QoS.BestEfforts);
            this.Notify($"Client Subscribed '{channel}'");
        }
        private string BuildMessage(string message, string topic)
        {
            var m = new MQTTMessage() {
                Message = message,
                ClientId = this._clientId,
                Topic = topic
            };
            return m.ToJSON();
        }
        public void Publish(string channel, string message)
        {
            this._client.Publish(channel, BuildMessage(message, channel), QoS.BestEfforts, false);
        }
        void client_Connected(object sender, EventArgs e)
        {
            this.Notify("Client connected");
        }
        void _client_ConnectionLost(object sender, EventArgs e)
        {
            this.Notify("Client connection lost");
        }
        bool client_PublishArrived(object sender, PublishArrivedArgs e)
        {
            var m = MQTTMessage.Parse(e.Payload.ToString(), e.Topic);
            if(m.ClientId == this._clientId)
            {
                // Ignore message sent by this instance
                return false;
            }
            else {
                if(this.MessageArrived != null)
                    this.MessageArrived(m);
                return true;
            }
        }
        public void Dispose()
        {
            this.Stop();
        }
    }
}
