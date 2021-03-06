﻿using System;
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
            string password = "user1";
            const string channel = "/frederictorres/iotdashboard";

            var clientId = MQTTManager.BuildClientId();
            Console.WriteLine($"Starting mqttManager Console, clientId:{clientId}");

            using(var mqttManager = new MQTTManager(connectionString, clientId, username, password))
            {
                mqttManager.NotificationEvent += MqttManager_NotificationEvent;
                mqttManager.MessageArrived += MqttManager_MessageArrived;
                mqttManager.Start(channel);
                mqttManager.Subscribe(channel);
                while(true)
                {
                    Console.WriteLine("Q)uit, S)end, C)lear");
                    var k = Console.ReadKey(true);
                    if(k.Key == ConsoleKey.Q) break;
                    if(k.Key == ConsoleKey.C) Console.Clear();
                    if(k.Key == ConsoleKey.S) {
                        mqttManager.Publish(channel, $"[{Environment.MachineName}, {Environment.TickCount}]Working...");
                    }
                }
            }
        }

        private static void MqttManager_MessageArrived(MQTTMessage message)
        {
            Console.WriteLine($"Message Arrived {message.Message}, {message.ClientId}, {message.Topic}");
        }

        private static void MqttManager_NotificationEvent(string message)
        {
            Console.WriteLine($">{message}");
        }
    }
}