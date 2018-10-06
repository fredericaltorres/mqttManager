using System;
using MqttLib;
using MQTTManagerLib;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "tcp://m15.cloudmqtt.com:10989";
            string username = "user1";
            string password = "";
            const string channel = "/home/temperature" ;

            var clientId = MQTTManager.BuildClientId();
            Console.WriteLine($"Starting mqttManager Console, clientId:{clientId}");

            using(var mqttManager = new MQTTManager(connectionString, clientId, username, password))
            {
                mqttManager.NotificationEvent += MqttManager_NotificationEvent;
                mqttManager.MessageArrived += MqttManager_MessageArrived;
                mqttManager.Start();
                if(args.Length == 1)
                {
                    for(var i=0; i<2; i++)
                    {
                        mqttManager.Publish(channel, $"[${i.ToString("000")}]Yes it is working... source-computer:${Environment.MachineName}");
                    }
                }
                else
                {
                    mqttManager.Subscribe(channel);
                }
                while(true)
                {
                    Console.WriteLine("Q)uit, S)end");
                    var k = Console.ReadKey();
                    if(k.Key == ConsoleKey.Q) break;
                    if(k.Key == ConsoleKey.S) {
                        mqttManager.Publish(channel, $"[{Environment.MachineName}, {Environment.TickCount}]Working...");
                    }
                }
            }
        }

        private static void MqttManager_MessageArrived(MQTTMessage message)
        {
            Console.WriteLine($"Message Arrived {message.Message}, {message.ClientID}, {message.Topic}");
        }

        private static void MqttManager_NotificationEvent(string message)
        {
            Console.WriteLine($">{message}");
        }
    }
}