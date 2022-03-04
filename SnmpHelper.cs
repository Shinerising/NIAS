using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LanMonitor
{
    internal class SnmpHelper
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
            public const string OID_dot1dTpFdbAddress = "1.3.6.1.2.1.17.4.3.1.1";
            public const string OID_dot1dTpFdbPort = "1.3.6.1.2.1.17.4.3.1.2";
        }

        public static string Username;
        public static string AuthPassword;
        public static string PrivPassword;
        public const int Timeout = 1000;
        public const int RetryCount = 3;

        public static void Initialize(string user, string auth, string priv)
        {
            Username = user;
            AuthPassword = auth;
            PrivPassword = priv;
        }

        public static ReportMessage GetReportMessage(IPEndPoint endpoint)
        {
            Discovery discovery = Messenger.GetNextDiscovery(SnmpType.GetBulkRequestPdu);
            ReportMessage report = discovery.GetResponse(Timeout, endpoint);
            return report;
        }

        private static List<Variable> FetchData(ReportMessage report, IPEndPoint endpoint, string OID)
        {
#pragma warning disable CS0618 // 类型或成员已过时
            SHA1AuthenticationProvider auth = new SHA1AuthenticationProvider(new OctetString(AuthPassword));
#pragma warning restore CS0618 // 类型或成员已过时
            AESPrivacyProvider priv = new AESPrivacyProvider(new OctetString(PrivPassword), auth);
            List<Variable> result = new List<Variable>();
            _ = Messenger.BulkWalk(VersionCode.V3,
                              endpoint,
                              new OctetString(Username),
                              OctetString.Empty,
                              new ObjectIdentifier(OID),
                              result,
                              Timeout,
                              RetryCount,
                              WalkMode.WithinSubtree,
                              priv,
                              report);
            return result;
        }

        public static Dictionary<string, string> FetchStringData(ReportMessage report, IPEndPoint endpoint, string OID)
        {
            List<Variable> result = FetchData(report, endpoint, OID);
            return result?.ToDictionary(item => item.Id.ToString(), item => item.Data.ToString());
        }

        public static Dictionary<string, byte[]> FetchBytesData(ReportMessage report, IPEndPoint endpoint, string OID)
        {
            List<Variable> result = FetchData(report, endpoint, OID);
            return result?.ToDictionary(item => item.Id.ToString(), item => item.Data.ToBytes());
        }
        public static Dictionary<string, int> FetchIntData(ReportMessage report, IPEndPoint endpoint, string OID)
        {
            List<Variable> result = FetchData(report, endpoint, OID);
            return result?.ToDictionary(item => item.Id.ToString(), item => ((Integer32)item.Data).ToInt32());
        }
        public static Dictionary<string, uint> FetchCounterData(ReportMessage report, IPEndPoint endpoint, string OID)
        {
            List<Variable> result = FetchData(report, endpoint, OID);
            return result?.ToDictionary(item => item.Id.ToString(), item => ((Counter32)item.Data).ToUInt32());
        }
        public static Dictionary<string, TimeSpan> FetchTimeSpanData(ReportMessage report, IPEndPoint endpoint, string OID)
        {
            List<Variable> result = FetchData(report, endpoint, OID);
            return result?.ToDictionary(item => item.Id.ToString(), item => ((TimeTicks)item.Data).ToTimeSpan());
        }
    }
}
