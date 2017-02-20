using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skogsaas.Monolith.Plugins.MqttBridge
{
    public interface IMqttTopicConfiguration : Configuration.IIdentifier
    {
        string Topic { get; set; }
        byte Qos { get; set; }
        bool Retain { get; set; } 
    }
}
