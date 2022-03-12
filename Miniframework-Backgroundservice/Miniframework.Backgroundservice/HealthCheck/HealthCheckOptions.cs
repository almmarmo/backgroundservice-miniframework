using System;
using System.Collections.Generic;
using System.Text;

namespace Miniframework.Backgroundservice.HealthCheck
{
    public class HealthCheckOptions
    {
        public const string PROTOCOL_HTTP = "http";
        public const string PROTOCOL_TCP = "tcp";

        public bool Enabled { get; set; }
        public int Port { get; set; }
        public string Protocol { get; set; }
        public int? ProcessTimeoutSeconds { get; set; }
    }
}
