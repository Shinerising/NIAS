﻿using System;
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
    public class SwitchPort
    {
        public string Name { get; set; }
        public string Brief { get; set; }
        public bool IsUp { get; set; }
    }
    public class SwitchHost
    {
        public string HostName { get; set; }
        public string IPAddress { get; set; }
        public string MACAddress { get; set; }
    }
    public class SwitchDevice
    {
        public string Address { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public List<SwitchPort> PortList { get; set; }
        public List<SwitchHost> HostList { get; set; }
        public SwitchDevice(string ip)
        {
            Address = ip;
            EndPoint = new IPEndPoint(IPAddress.Parse(ip), 161);
            PortList = new List<SwitchPort>()
            {
                new SwitchPort()
                {
                    Name = "GE1/0/1",
                    IsUp = true
                },
                new SwitchPort()
                {
                    Name = "GE1/0/2",
                    IsUp = false
                },
                new SwitchPort()
                {
                    Name = "GE1/0/3",
                    IsUp = true
                },
                new SwitchPort()
                {
                    Name = "GE1/0/4",
                    IsUp = false
                }
            };
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
                }
            };
        }
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
