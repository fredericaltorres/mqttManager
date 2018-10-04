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
        public delegate void MessageArrivedType(PublishArrivedArgs message);
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

        public void Start()
        {
            Notify("Client connecting");
            this._client.Connect(true);
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
            this._client.Subscribe(channel, QoS.BestEfforts);
        }

        public void Publish(string channel, string message)
        {
            this._client.Publish(channel, message, QoS.BestEfforts, false);
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
            if(this.MessageArrived != null)
                this.MessageArrived(e);
            return true;
        }

        public void Dispose()
        {
            this.Stop();
        }
    }
}
