using Skogsaas.Legion;
using System.Collections.Generic;

namespace Skogsaas.Monolith.Plugins.MqttBridge
{
    public class MqttBridge : IPlugin
    {
        private Channel configChannel;
        private MqttClient client;

        private Dictionary<string, MqttTopicHandler> topics;

        public MqttBridge()
        {
            this.topics = new Dictionary<string, MqttTopicHandler>();
        }

        public void initialize()
        {
            this.configChannel = Manager.Create(Configuration.Constants.Channel);
            this.configChannel.RegisterType(typeof(IMqttClientConfiguration));
            this.configChannel.RegisterType(typeof(IMqttTopicConfiguration));

            this.configChannel.SubscribePublish(typeof(IMqttClientConfiguration), onClientConfigPublish);
        }

        private void onClientConfigPublish(Channel channel, IObject obj)
        {
            IMqttClientConfiguration config = obj as IMqttClientConfiguration;

            this.client = new MqttClient(config.Broker, config.Port, config.ClientId);

            this.configChannel.SubscribePublish(typeof(IMqttTopicConfiguration), onBindingConfigPublish);
        }

        private void onBindingConfigPublish(Channel channel, IObject obj)
        {
            IMqttTopicConfiguration config = obj as IMqttTopicConfiguration;

            IMqttTopic topic = this.configChannel.CreateType<IMqttTopic>($"{typeof(IMqttTopic).FullName}.{config.Topic.Replace('/', '.')}");
            this.configChannel.Publish(topic);

            MqttTopicHandler binding = new MqttTopicHandler(this.client, topic, config);
            this.topics.Add(config.Topic, binding);
        }
    }
}
