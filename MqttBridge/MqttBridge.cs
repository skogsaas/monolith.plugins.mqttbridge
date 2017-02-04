using Skogsaas.Legion;
using System.Collections.Generic;

namespace Skogsaas.Monolith.Plugins.MqttBridge
{
    public class MqttBridge : IPlugin
    {
        private Channel configChannel;
        private MqttClient client;

        private Dictionary<string, MqttBinding> bindings;

        public MqttBridge()
        {
            this.bindings = new Dictionary<string, MqttBinding>();
        }

        public void initialize()
        {
            this.configChannel = Manager.Create(Configuration.Constants.Channel);
            this.configChannel.RegisterType(typeof(IMqttClientConfiguration));
            this.configChannel.RegisterType(typeof(IMqttBindingConfiguration));

            this.configChannel.SubscribePublish(typeof(IMqttClientConfiguration), onClientConfigPublish);
        }

        private void onClientConfigPublish(Channel channel, IObject obj)
        {
            IMqttClientConfiguration config = obj as IMqttClientConfiguration;

            this.client = new MqttClient(config.Broker, config.Port, config.ClientId);

            this.configChannel.SubscribePublish(typeof(IMqttBindingConfiguration), onBindingConfigPublish);
        }

        private void onBindingConfigPublish(Channel channel, IObject obj)
        {
            IMqttBindingConfiguration config = obj as IMqttBindingConfiguration;

            MqttBinding binding = new MqttBinding(this.client, Manager.Create(config.Channel), config.ObjectId, config.PropertyName, config.Topic, config.Mode);
            this.bindings.Add(config.Topic, binding);
        }
    }
}
