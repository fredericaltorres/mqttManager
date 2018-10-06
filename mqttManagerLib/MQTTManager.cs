using MqttLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTTManagerLib
{
    public class MQTTMessage {

        public string ClientID;
        public string Message;
        public string Topic;
        public static MQTTMessage Parse(string rawMessage, string topic)
        {
            var m = new MQTTMessage();
            m.Topic = topic;
            var values = rawMessage.Split('|');
            if(values.Length == 2)
            {
                m.ClientID = values[0];
                m.Message = values[1];
            }
            else
            {
                m.Message = rawMessage;
            }
            return m;
        }
    }

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

        private string BuildMessage(string message)
        {
            return $"{this._clientId}|{message}";
        }

        public void Publish(string channel, string message)
        {
            this._client.Publish(channel, BuildMessage(message), QoS.BestEfforts, false);
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
            
            if(m.ClientID == this._clientId)
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
