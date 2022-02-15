﻿using System;
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
    internal class SnmpHelper
    {
        public static class OIDString
        {
            public const string OID_ipNetToMediaPhysAddress = "1.3.6.1.2.1.4.22.1.2";
            public const string OID_ipNetToMediaNetAddress = "1.3.6.1.2.1.4.22.1.3";
            public const string OID_ifDescr = "1.3.6.1.2.1.2.2.1.2";
            public const string OID_ifType = "1.3.6.1.2.1.2.2.1.3";
            public const string OID_ifOperStatus = "1.3.6.1.2.1.2.2.1.8";
            public const string OID_sysDescr = "1.3.6.1.2.1.1.1";
        }

        public static string Username;
        public static string AuthPassword;
        public static string PrivPassword;
        public const int Timeout = 1000;
        public const int RetryCount = 10;

        public static void Initialize(string user, string auth, string priv)
        {
            Username = user;
            AuthPassword = auth;
            PrivPassword = priv;
        }

        public static ReportMessage GetReportMessage(IPEndPoint endpoint)
        {
            try
            {
                Discovery discovery = Messenger.GetNextDiscovery(SnmpType.GetBulkRequestPdu);
                ReportMessage report = discovery.GetResponse(Timeout, endpoint);
                return report;
            }
            catch
            {
                return null;
            }
        }

        public static Dictionary<string, string> FetchData(ReportMessage report, IPEndPoint endpoint, string OID)
        {
            try
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
                return result.ToDictionary(item => item.Id.ToString(), item => item.Data.ToString());
            }
            catch
            {
                return null;
            }
        }
    }
}
