using Skogsaas.Legion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Skogsaas.Monolith.Plugins.MqttBridge
{
    internal class MqttTopicHandler
    {
        private MqttClient client;
        private IMqttTopic topic;
        private IMqttTopicConfiguration config;

        public MqttTopicHandler(MqttClient client, IMqttTopic topic, IMqttTopicConfiguration config)
        {
            this.client = client;
            this.topic = topic;
            this.config = config;

            if(this.config.Subscribe)
            {
                this.client.Subscribe(this.config.Topic, onMqttPublish);
            }
            else
            {
                this.topic.PropertyChanged += onPropertyChanged;
            }
        }

        private void onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.client.Publish(this.topic.Topic, this.topic.Value, this.config.Qos, this.config.Retain);
        }

        private void onMqttPublish(string topic, string message, byte qos, bool retain)
        {
            this.topic.Value = message;
            this.topic.Qos = qos;
            this.topic.Retain = retain;
        }
    }
}
