using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skogsaas.Monolith.Plugins.MqttBridge
{
    public enum BindingMode
    {
        Subscriber,
        Publisher,
        TwoWay
    }

    public interface IMqttBindingConfiguration : Configuration.Identifier
    {
        string Channel { get; set; }
        string ObjectId { get; set; }
        string PropertyName { get; set; }
        string Topic { get; set; }
        BindingMode Mode { get; set; }
    }
}
