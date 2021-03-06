﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skogsaas.Monolith.Plugins.MqttBridge
{
    public interface IMqttClientConfiguration : Configuration.IIdentifier
    {
        string Broker { get; set; }
        int Port { get; set; }

        string ClientId { get; set; }
    }
}
