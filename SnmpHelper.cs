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
    internal class SnmpHelper
    {
        [Obsolete]
        public static void GetData()
        {
            Discovery discovery = Messenger.GetNextDiscovery(SnmpType.GetBulkRequestPdu);
            ReportMessage report = discovery.GetResponse(60000, new IPEndPoint(IPAddress.Parse("172.16.24.1"), 161));

            var auth = new SHA1AuthenticationProvider(new OctetString("admin@huawei"));
            var priv = new AESPrivacyProvider(new OctetString("admin@huawei"), auth);
            var result = new List<Variable>();
            var count = Messenger.BulkWalk(VersionCode.V3,
                              new IPEndPoint(IPAddress.Parse("172.16.24.1"), 161),
                              new OctetString("admin"),
                              OctetString.Empty,
                              new ObjectIdentifier("1.3.6.1.2.1.4.22.1.2"),
                              result,
                              60000,
                              10,
                              WalkMode.WithinSubtree,
                              priv,
                              report);
            var a = result;
        }
    }
}
