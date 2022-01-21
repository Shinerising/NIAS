using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lextm.SharpSnmpLib.Messaging;
using System.Net;
using Lextm.SharpSnmpLib.Security;
using Lextm.SharpSnmpLib;

namespace LanMonitor
{
    public class SwitchDevice
    {
        public class SwitchPort
        {
            public string Name { get; set; }
            public string Brief { get; set; }
            public bool IsUp { get; set; }
        }
        public string IPAddress { get; set; }
        public List<SwitchPort> PortList { get; set; }
    }
    public class SwitchHost
    {
        public string HostName { get; set; }
        public string IPAddress { get; set; }
        public string MACAddress { get; set; }
    }
    internal class SnmpHelper
    {
        public static class OIDString
        {
            public const string OID_ARP = "";
        }

        public static string AuthUsername;
        public static string AuthPassword;
        public static string PrivPassword;
        public static List<string> SwitchIPList;
        public const int Timeout = 60000;
        public const int RetryCount = 10;

        public void Initialize()
        {
            SwitchIPList = new List<string>()
            {
                "172.16.24.1"
            };

        }

        [Obsolete]
        public static void GetData()
        {
            Discovery discovery = Messenger.GetNextDiscovery(SnmpType.GetBulkRequestPdu);
            ReportMessage report = discovery.GetResponse(Timeout, new IPEndPoint(IPAddress.Parse("172.16.24.1"), 161));

            SHA1AuthenticationProvider auth = new SHA1AuthenticationProvider(new OctetString("admin@huawei"));
            AESPrivacyProvider priv = new AESPrivacyProvider(new OctetString("admin@huawei"), auth);
            List<Variable> result = new List<Variable>();
            var unused = Messenger.BulkWalk(VersionCode.V3,
                              new IPEndPoint(IPAddress.Parse("172.16.24.1"), 161),
                              new OctetString("admin"),
                              OctetString.Empty,
                              new ObjectIdentifier("1.3.6.1.2.1.4.22.1.2"),
                              result,
                              Timeout,
                              10,
                              WalkMode.WithinSubtree,
                              priv,
                              report);
        }
    }
}
