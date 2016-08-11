using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StressTestTool.Core
{
    public class RequestConfiguration
    {
        public string Host { get; set; }

        public string HostUri { get; set; }

        public string PageUri { get; set; }

        public string HttpMethod { get; set; }

        public Dictionary<string, string> FormData { get; set; }

        public bool IsJson { get; set; }

        public object JsonData { get; set; }
    }
}
