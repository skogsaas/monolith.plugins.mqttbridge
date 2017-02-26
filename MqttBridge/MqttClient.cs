using Skogsaas.Monolith.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Skogsaas.Monolith.Plugins.MqttBridge
{
    internal class MqttClient
    {
        public delegate void MqttMessageHandler(string topic, string message, byte qos, bool retain);

        private uPLibrary.Networking.M2Mqtt.MqttClient client;

        private Dictionary<string, MqttMessageHandler> handlers;

        internal MqttClient(string host, int port, string clientId)
        {
            this.handlers = new Dictionary<string, MqttMessageHandler>();

            this.client = new uPLibrary.Networking.M2Mqtt.MqttClient(host);
            this.client.ConnectionClosed += onConnectionClosed;

            Logger.Info($"Trying to connect <{this.client.Connect(clientId)}>.");

            this.client.MqttMsgPublishReceived += onMqttMsgPublishReceived;
        }

        private void onConnectionClosed(object sender, EventArgs e)
        {
            Logger.Info($"Connection closed. Trying to reconnect <{this.client.Connect(this.client.ClientId)}>.");
        }

        private void onMqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            if(this.handlers.ContainsKey(e.Topic))
            {
                Logger.Trace($"Received from <{e.Topic}> ");
                this.handlers[e.Topic](e.Topic, Encoding.UTF8.GetString(e.Message), e.QosLevel, e.Retain);
            }
            else
            {
                Logger.Info($"Received MQTT from unknown topic <{e.Topic}> message <{Encoding.UTF8.GetString(e.Message)}>");
            }
        }

        internal void Publish(string topic, string message, byte qos = 1, bool retain = false)
        {
            Logger.Trace($"Publishing topic <{topic}> message <{message}>");
            this.client.Publish(topic, Encoding.UTF8.GetBytes(message), qos, retain);
        }

        internal void Subscribe(string topic, MqttMessageHandler handler)
        {
            Logger.Trace($"Subscribing topic <{topic}>");
            this.client.Subscribe(new string[] { topic }, new byte[] { 1 });

            this.handlers[topic] = handler;
        }

        internal void Unsubcribe(string topic)
        {
            this.client.Unsubscribe(new string[] { topic });

            this.handlers.Remove(topic);
        }
    }
}
