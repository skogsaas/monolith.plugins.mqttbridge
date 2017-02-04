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
    internal class MqttBinding
    {
        private MqttClient client;

        private IObject obj;
        private string propertyName;
        private string topic;
        private BindingMode mode;

        private AutoResetEvent objLock;

        public MqttBinding(MqttClient client, Channel channel, string objectId, string propertyName, string topic, BindingMode mode)
        {
            this.obj = channel.Find(objectId);
            this.propertyName = propertyName;
            this.topic = topic;
            this.mode = mode;
            
            if(this.obj != null)
            {
                init();
            }
            else
            {
                channel.SubscribePublishId(objectId, onObjectPublished);
            }
        }

        private void init()
        {
            if(this.mode == BindingMode.TwoWay)
            {
                this.objLock = new AutoResetEvent(false);
            }

            if (this.mode != BindingMode.Subscriber)
            {
                this.obj.PropertyChanged += onPropertyChanged;
            }

            if(this.mode == BindingMode.Publisher)
            {
                publish();
            }

            if(this.mode != BindingMode.Publisher)
            {
                this.client.Subscribe(this.topic, onMqttPublish);
            }
        }

        private void publish()
        {
            PropertyInfo property = this.obj.GetType().GetProperty(this.propertyName);
            object value = property.GetValue(this.obj);

            this.objLock?.WaitOne();
            this.client.Publish(this.topic, value.ToString());
            this.objLock?.Set();
        }

        private void onObjectPublished(Channel channel, IObject o)
        {
            this.obj = o;

            init();
        }

        private void onPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == this.propertyName)
            {
                publish();
            }
        }

        private void onMqttPublish(string topic, string message, byte qos, bool retain)
        {
            PropertyInfo property = this.obj.GetType().GetProperty(this.propertyName);

            this.objLock?.WaitOne();
            property.SetValue(this.obj, Convert.ChangeType(message, property.PropertyType));
            this.objLock?.Set();
        }
    }
}
