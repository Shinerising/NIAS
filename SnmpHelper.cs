using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lextm.SharpSnmpLib.Messaging;
using System.Net;
using Lextm.SharpSnmpLib.Security;
using Lextm.SharpSnmpLib;
using System.Numerics;

namespace LanMonitor
{
    public class SwitchPort
    {
        public string Name { get; set; }
        public string Brief { get; set; }
        public bool IsUp { get; set; }
        public bool IsFiber { get; set; }
    }
    public class SwitchHost
    {
        public string HostName { get; set; }
        public string IPAddress { get; set; }
        public string MACAddress { get; set; }
    }
    public class LanHost
    {
        public string Name { get; set; }
        public List<string> IPAddress { get; set; }
        public class LineVector
        {
            public double Left { get; set; }
            public double Top { get; set; }
            public double Length { get; set; }
            public DeviceState State { get; set; }
        }
        public List<LineVector> VectorList { get; set; }
        public static List<LineVector> RefreshVector(int count)
        {
            if (count == 0)
            {
                return null;
            }
            double width = 60;
            double height0 = 92;
            double height1 = 38;
            return Enumerable.Range(0, count).Select(item => new LineVector()
            {
                Left = width / -2 + width / count * (item + 0.5),
                Top = 6 + height1 * item,
                Length = height0 * item - height1 * item - 137,
                State = DeviceState.Online
            }).ToList();
        }
    }
    public enum DeviceState
    {
        Unknown,
        Online,
        Offline
    }
    public class SwitchDevice
    {
        public string Address { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public string Information { get; set; }
        public DeviceState State { get; set; } = DeviceState.Online;
        public List<SwitchPort> PortList { get; set; }
        public List<SwitchHost> HostList { get; set; }
        public SwitchDevice(string ip)
        {
            Address = ip;
            EndPoint = new IPEndPoint(IPAddress.Parse(ip), 161);
            Information = string.Format("HUAWEI S5720{0}HUAWEI S5720{0}HUAWEI S5720", Environment.NewLine);
            PortList = Enumerable.Range(0, 28).Select(item => new SwitchPort()
            {
                Name = "GE1/0/" + item.ToString(),
                Brief = "GigabitEthernet1/0/" + item.ToString(),
                IsUp = item % 3 == 1,
                IsFiber = item >= 24
            }).ToList();
            HostList = new List<SwitchHost>()
            {
                new SwitchHost()
                {
                    IPAddress = "172.16.24.90",
                    MACAddress = "AA-BB-CC-DD-EE-FF"
                },
                new SwitchHost()
                {
                    IPAddress = "172.16.24.91",
                    MACAddress = "AA-BB-CC-DD-EE-FF"
                },
                new SwitchHost()
                {
                    IPAddress = "172.16.24.92",
                    MACAddress = "AA-BB-CC-DD-EE-FF"
                },
                new SwitchHost()
                {
                    IPAddress = "172.16.24.93",
                    MACAddress = "AA-BB-CC-DD-EE-FF"
                },
                new SwitchHost()
                {
                    IPAddress = "172.16.24.95",
                    MACAddress = "AA-BB-CC-DD-EE-FF"
                },
                new SwitchHost()
                {
                    IPAddress = "172.16.24.101",
                    MACAddress = "AA-BB-CC-DD-EE-FF"
                },
                new SwitchHost()
                {
                    IPAddress = "172.16.24.102",
                    MACAddress = "AA-BB-CC-DD-EE-FF"
                },
                new SwitchHost()
                {
                    IPAddress = "172.16.24.103",
                    MACAddress = "AA-BB-CC-DD-EE-FF"
                }
            };
        }
    }
    public class SnmpClient
    {
        private SnmpClient(List<string> list)
        {
            SwitchIPList = list;
            SwitchDeviceList = SwitchIPList.Select(item => new SwitchDevice(item)).ToList();
        }

        public List<string> SwitchIPList { get; set; }
        public List<SwitchDevice> SwitchDeviceList { get; set; }
        public static SnmpClient PreviewSnmpClient => new SnmpClient(new List<string>() { "172.16.24.1", "172.16.24.188" });
        public static List<LanHost> LanHostList => new List<LanHost>() {
            new LanHost()
            {
                Name = "Host01",
                IPAddress = new List<string>() { "172.16.24.90", "172.16.34.90" },
                VectorList = LanHost.RefreshVector(2)
            },
            new LanHost()
            {
                Name = "Host02",
                IPAddress = new List<string>() { "172.16.24.91", "172.16.34.91" },
                VectorList = LanHost.RefreshVector(2)
            },
            new LanHost()
            {
                Name = "Host03",
                IPAddress = new List<string>() { "172.16.24.92", "172.16.34.192" },
                VectorList = LanHost.RefreshVector(2)
            },
            new LanHost()
            {
                Name = "Host04",
                IPAddress = new List<string>() { "172.16.24.93", "172.16.34.93" },
                VectorList = LanHost.RefreshVector(2)
            }
        };
    }
    internal class SnmpHelper
    {
        public static class OIDString
        {
            public const string OID_ARP = "1.3.6.1.2.1.4.22.1.2";
            public const string OID_ifDescr = "1.3.6.1.2.1.4.22.1.2";
            public const string OID_ifType = "1.3.6.1.2.1.4.22.1.3";
            public const string OID_ifOperStatus = "1.3.6.1.2.1.4.22.1.8";
        }

        public static string Username;
        public static string AuthPassword;
        public static string PrivPassword;
        public static List<string> SwitchIPList;
        public static List<SwitchDevice> SwitchDeviceList;
        public const int Timeout = 60000;
        public const int RetryCount = 10;

        public static void Initialize()
        {
            SwitchIPList = new List<string>()
            {
                "172.16.24.1"
            };
            SwitchDeviceList = SwitchIPList.Select(item => new SwitchDevice(item)).ToList();

            Username = "admin";
            AuthPassword = "admin@huawei";
            PrivPassword = "admin@huawei";
        }

        [Obsolete]
        public static void FetchData()
        {
            return;
            Discovery discovery = Messenger.GetNextDiscovery(SnmpType.GetBulkRequestPdu);
            ReportMessage report = discovery.GetResponse(Timeout, new IPEndPoint(IPAddress.Parse("172.16.24.1"), 161));

            SHA1AuthenticationProvider auth = new SHA1AuthenticationProvider(new OctetString(AuthPassword));
            AESPrivacyProvider priv = new AESPrivacyProvider(new OctetString(PrivPassword), auth);
            List<Variable> result = new List<Variable>();
            _ = Messenger.BulkWalk(VersionCode.V3,
                              new IPEndPoint(IPAddress.Parse("172.16.24.1"), 161),
                              new OctetString(Username),
                              OctetString.Empty,
                              new ObjectIdentifier("1.3.6.1.2.1.4.22.1.2"),
                              result,
                              Timeout,
                              RetryCount,
                              WalkMode.WithinSubtree,
                              priv,
                              report);
        }
    }
}
