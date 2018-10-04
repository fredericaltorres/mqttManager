using System;
using MqttLib;
using MQTTManagerLib;

namespace Sample
{
    class Program
    {

        static bool MustPublish = false;

        static void Main(string[] args)
        {
            string connectionString = "tcp://m15.cloudmqtt.com:10989";
            string username = "lvyugmmd";
            string password = "9rqYbUiLLY2s";
            const string channel = "/home/temperature" ;

            Console.WriteLine("Starting MqttDotNet sample program.");
            using(var mqttManager = new MQTTManager(connectionString, Guid.NewGuid().ToString(), username, password))
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
                Console.WriteLine("Hit enter to end");
                Console.ReadLine();
            }
        }

        private static void MqttManager_MessageArrived(PublishArrivedArgs message)
        {
            Console.WriteLine($"Message Arrived {message.Topic}, {message.Payload}, {message.QualityOfService}");
        }

        private static void MqttManager_NotificationEvent(string message)
        {
            Console.WriteLine($">{message}");
        }
    }
}