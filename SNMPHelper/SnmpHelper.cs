using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SNMP
{
    public class SnmpHelper
    {
        public static class OIDString
        {
            public const string OID_ipNetToMediaPhysAddress = "1.3.6.1.2.1.4.22.1.2";
            public const string OID_ipNetToMediaNetAddress = "1.3.6.1.2.1.4.22.1.3";
            public const string OID_ipNetToMediaType = "1.3.6.1.2.1.4.22.1.4";
            public const string OID_ifIndex = "1.3.6.1.2.1.2.2.1.1";
            public const string OID_ifDescr = "1.3.6.1.2.1.2.2.1.2";
            public const string OID_ifType = "1.3.6.1.2.1.2.2.1.3";
            public const string OID_ifOperStatus = "1.3.6.1.2.1.2.2.1.8";
            public const string OID_ifInOctets = "1.3.6.1.2.1.2.2.1.10";
            public const string OID_ifOutOctets = "1.3.6.1.2.1.2.2.1.16";
            public const string OID_sysDescr = "1.3.6.1.2.1.1.1";
            public const string OID_sysUpTime = "1.3.6.1.2.1.1.3";
            public const string OID_hwStackSystemMac = "1.3.6.1.4.1.2011.5.25.183.1.4";
            public const string OID_hwArpDynMacAdd = "1.3.6.1.4.1.2011.5.25.123.1.17.1.11";
            public const string OID_hwArpDynOutIfIndex = "1.3.6.1.4.1.2011.5.25.123.1.17.1.14";
            public const string OID_hwArpDynTable = "1.3.6.1.4.1.2011.5.25.123.1.17.1";
            public const string OID_hwArpCfgTable = "1.3.6.1.4.1.2011.5.25.123.1.18.1";
            public const string OID_hwEntityCpuUsage = "1.3.6.1.4.1.2011.5.25.31.1.1.1.1.5";
            public const string OID_hwEntityMemUsage = "1.3.6.1.4.1.2011.5.25.31.1.1.1.1.7";
            public const string OID_hwEntityTemperature = "1.3.6.1.4.1.2011.5.25.31.1.1.1.1.11";
            public const string OID_dot1dTpFdbAddress = "1.3.6.1.2.1.17.4.3.1.1";
            public const string OID_dot1dTpFdbPort = "1.3.6.1.2.1.17.4.3.1.2";
        }

        public static class DISMAN_PING
        {
            public const string OID_pingCtlRowStatus = "1.3.6.1.2.1.80.1.2.1.23.1.49.1.51";
            public const string OID_pingResultsMaxRtt = "1.3.6.1.2.1.80.1.3.1.5";
            public const string OID_pingCtlEntry = @"
1.3.6.1.2.1.80.1.2.1.3.1.49.1.51 i 1
1.3.6.1.2.1.80.1.2.1.4.1.49.1.51 s 0.0.0.0
1.3.6.1.2.1.80.1.2.1.5.1.49.1.51 u 16
1.3.6.1.2.1.80.1.2.1.7.1.49.1.51 u 1
1.3.6.1.2.1.80.1.2.1.8.1.49.1.51 i 1
1.3.6.1.2.1.80.1.2.1.10.1.49.1.51 u 0
1.3.6.1.2.1.80.1.2.1.16.1.49.1.51 o 1.3.6.1.2.1.80.3.1
1.3.6.1.2.1.80.1.2.1.23.1.49.1.51 i 4";
            private static List<Variable>? entryDict;
            public static List<Variable> GetEntryList(string targetIP)
            {
                entryDict ??= OID_pingCtlEntry.Trim().Split('\n').Select(item =>
                    {
                        var fragments = item.Trim().Split(' ');
                        ISnmpData? data = null;
                        switch (fragments[1])
                        {
                            case "i":
                                data = new Integer32(int.Parse(fragments[2]));
                                break;
                            case "u":
                                data = new Gauge32(uint.Parse(fragments[2]));
                                break;
                            case "s":
                                data = new OctetString(fragments[2]);
                                break;
                            case "o":
                                data = new ObjectIdentifier(fragments[2]);
                                break;
                        }
                        return new Variable(new ObjectIdentifier(fragments[0]), data);
                    }).ToList();
                var dict = entryDict.ToList();
                dict[1] = new Variable(dict[1].Id, new OctetString(targetIP));
                return dict;
            }
        }

        public static string Username = string.Empty;
        public const int Timeout = 1000;
        public const int RetryCount = 3;

        public static void Initialize(string user)
        {
            Username = user;
        }
        private static List<Variable> FetchData(IPEndPoint endpoint, string OID)
        {
            List<Variable> result = new List<Variable>();
            _ = Messenger.BulkWalk(VersionCode.V2,
                              endpoint,
                              new OctetString(Username),
                              OctetString.Empty,
                              new ObjectIdentifier(OID),
                              result,
                              Timeout,
                              RetryCount,
                              WalkMode.WithinSubtree, null, null);
            return result;
        }

        private static void SetData(IPEndPoint endpoint, List<Variable> list)
        {
            _ = Messenger.Set(VersionCode.V2,
                              endpoint,
                              new OctetString(Username),
                              list,
                              Timeout);
        }

        public static void SendPing(IPEndPoint endpoint, string targetIP)
        {
            SetData(endpoint, new List<Variable>() { new Variable(new ObjectIdentifier(DISMAN_PING.OID_pingCtlRowStatus), new Integer32(6)) });
            SetData(endpoint, DISMAN_PING.GetEntryList(targetIP));
        }

        public static Dictionary<string, string>? FetchStringData(IPEndPoint endpoint, string OID)
        {
            List<Variable> result = FetchData(endpoint, OID);
            return result?.ToDictionary(item => item.Id.ToString(), item => item.Data.ToString());
        }

        public static Dictionary<string, byte[]>? FetchBytesData(IPEndPoint endpoint, string OID)
        {
            List<Variable> result = FetchData(endpoint, OID);
            return result?.ToDictionary(item => item.Id.ToString(), item => item.Data.ToBytes());
        }
        public static Dictionary<string, int>? FetchIntData(IPEndPoint endpoint, string OID)
        {
            List<Variable> result = FetchData(endpoint, OID);
            return result?.ToDictionary(item => item.Id.ToString(), item => ((Integer32)item.Data).ToInt32());
        }
        public static Dictionary<string, uint>? FetchUIntData(IPEndPoint endpoint, string OID)
        {
            List<Variable> result = FetchData(endpoint, OID);
            return result?.ToDictionary(item => item.Id.ToString(), item => ((Gauge32)item.Data).ToUInt32());
        }
        public static Dictionary<string, uint>? FetchCounterData(IPEndPoint endpoint, string OID)
        {
            List<Variable> result = FetchData(endpoint, OID);
            return result?.ToDictionary(item => item.Id.ToString(), item => ((Counter32)item.Data).ToUInt32());
        }
        public static Dictionary<string, TimeSpan>? FetchTimeSpanData(IPEndPoint endpoint, string OID)
        {
            List<Variable> result = FetchData(endpoint, OID);
            return result?.ToDictionary(item => item.Id.ToString(), item => ((TimeTicks)item.Data).ToTimeSpan());
        }
    }
}
