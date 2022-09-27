using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace LanMonitor
{
    public partial class NetworkManager
    {
        public class NMAPPort
        {
            private static readonly Dictionary<ushort, string> WarningPort = new Dictionary<ushort, string>
            {
                { 20, "FTP服务的数据传输端口" },
                { 21, "FTP服务的连接端口，可能存在弱口令暴力破解" },
                { 22, "SSH服务端口，可能存在弱口令暴力破解" },
                { 23, "Telnet端口，可能存在弱口令暴力破解" },
                { 25, "SMTP简单邮件传输协议端口，和POP3的110端口对应" },
                { 43, "Whois服务端口" },
                { 53, "DNS服务端口(TCP/UDP 53)" },
                { 67, "DHCP服务端口" },
                { 68, "DHCP服务端口" },
                { 69, "TFTP端口，可能存在弱口令" },
                { 80, "HTTP端口，常见web漏洞" },
                { 110, "POP3邮件服务端口，和SMTP的25端口对应" },
                { 135, "RPC服务" },
                { 137, "NMB服务" },
                { 138, "NMB服务" },
                { 139, "SMB/CIFS服务" },
                { 143, "IMAP协议端口" },
                { 389, "LDAP目录访问协议，有可能存在注入、弱口令" },
                { 443, "HTTPS端口，心脏滴血等与SSL有关的漏洞" },
                { 445, "SMB服务端口，可能存在永恒之蓝漏洞MS17-010" },
                { 593, "RPC服务" },
                { 1025, "IIS服务" },
                { 1080, "socket端口，可能存在爆破" },
                { 1099, "RMI，可能存在 RMI反序列化漏洞" },
                { 3389, "Windows远程桌面服务，可能存在弱口令漏洞或者CVE-2019-0708远程桌面漏洞" }
            };
            public ushort ID { get; set; }
            public string Type { get; set; }
            public string State { get; set; }
            public string Name { get; set; }
            public bool IsWarning { get; set; }
            public string WarningInfo { get; set; }
            public NMAPPort(port port)
            {
                ID = ushort.Parse(port.portid);
                Type = port.protocol.ToString();
                State = port.state.state1;
                Name = port.service?.name ?? "unknown";
                if (WarningPort.ContainsKey(ID))
                {
                    IsWarning = true;
                    WarningInfo = WarningPort[ID];
                }
            }
        }
        public class NMAPHost
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string IPV4Address { get; set; }
            public string IPV6Address { get; set; }
            public string MACAddress { get; set; }
            public string MACVendor { get; set; }
            public string State { get; set; }
            public DateTime RefreshTime { get; set; }
            public TimeSpan UpTime { get; set; }
            public double Latency { get; set; }
            public int Distance { get; set; }
            public string OSName { get; set; }
            public string OSType { get; set; }
            public string OSVendor { get; set; }
            public string OSFamily { get; set; }
            public string OSGen { get; set; }
            public string OSBrief {
                get
                {
                    return OSFamily == "Windows" ? OSFamily + " " + OSGen : OSFamily;
                }
            }
            public List<NMAPPort> PortList { get; set; }
            public NMAPHost(host host)
            {
                Name = string.Join(',', host.Items.OfType<hostnames>().FirstOrDefault()?.hostname.Select(item => item.name));
                Address = host.address?.addr;
                IPV4Address = host.Items.OfType<address>().Where(item => item.addrtype == addressAddrtype.ipv4).FirstOrDefault()?.addr;
                IPV6Address = host.Items.OfType<address>().Where(item => item.addrtype == addressAddrtype.ipv6).FirstOrDefault()?.addr;
                MACAddress = host.Items.OfType<address>().Where(item => item.addrtype == addressAddrtype.mac).FirstOrDefault()?.addr;
                MACVendor = host.Items.OfType<address>().Where(item => item.addrtype == addressAddrtype.mac).FirstOrDefault()?.vendor;
                State = host.status.state.ToString();
                RefreshTime = DateTimeOffset.FromUnixTimeSeconds(int.Parse(host.endtime ?? "0")).LocalDateTime;
                UpTime = TimeSpan.FromSeconds(int.Parse(host.Items.OfType<uptime>().FirstOrDefault()?.seconds ?? "0"));
                Latency = int.Parse(host.times.srtt) / 100000;
                Distance = int.Parse(host.Items.OfType<distance>().FirstOrDefault()?.value);

                var os = host.Items.OfType<os>().FirstOrDefault();
                var osmatch = os?.osmatch?.FirstOrDefault();
                var osclass = osmatch?.osclass?.FirstOrDefault(); ;
                OSName = osmatch?.name;
                OSType = osclass?.type;
                OSVendor = osclass?.vendor;
                OSFamily = osclass?.osfamily;
                OSGen = osclass?.osgen;

                PortList = host.Items.OfType<ports>().FirstOrDefault()?.port.Select(item => new NMAPPort(item)).ToList();
            }
        }
        public class NMAPReport
        {
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public TimeSpan ElapsedTime { get; set; }
            public string Version { get; set; }
            public List<NMAPHost> HostList { get; set; }
            public NMAPReport(nmaprun data) {
                StartTime = DateTimeOffset.FromUnixTimeSeconds(int.Parse(data.start)).LocalDateTime;
                EndTime = DateTimeOffset.FromUnixTimeSeconds(int.Parse(data.runstats.finished.time)).LocalDateTime;
                ElapsedTime = TimeSpan.FromSeconds(double.Parse(data.runstats.finished.elapsed));
                HostList = data.Items.OfType<host>().Select(item => new NMAPHost(item)).ToList();
            }
        }
        public class NMAPHelper
        {
            public enum WorkingState
            {
                Idle,
                Executing,
                Parsing,
                Succeed,
                Fail
            }
            public static WorkingState State { get; private set; }
            public static string ErrorMessage { get; private set; }
            public static string Target = "127.0.0.1 10.211.55.1";
            private const string PingParams = "-sn --unprivileged -oX {0} {1}";
            private const string ScanParams = "-sS -p- -T5 -O -oX {0} {1}";
            private static string TempFile = Path.GetTempFileName();
            public static NMAPReport GetExampleData()
            {
                using (var reader = new StreamReader("test.xml"))
                {
                    nmaprun data = (nmaprun)new XmlSerializer(typeof(nmaprun)).Deserialize(reader);
                    return new NMAPReport(data);
                }
            }

            private static nmaprun GetNMAPData(string p, string target)
            {
                string command = "nmap";
                string parameters = string.Format(p, TempFile, target);
                int result = ExecuteCommand(command, parameters, out string output, out string error);
                if (result != 0)
                {
                    throw new Exception(error);
                }

                using (var reader = new StreamReader(TempFile))
                {
                    return (nmaprun)new XmlSerializer(typeof(nmaprun)).Deserialize(reader);
                }
            }
            public static NMAPReport GetData()
            {
                try
                {
                    ErrorMessage = string.Empty;
                    State = WorkingState.Executing;
                    //var pingResult = GetNMAPData(PingParams, Target);

                    //string target = pingResult.Items == null ? "" : string.Join(' ', pingResult.Items.OfType<host>().Select(item => item.address.addr).Where(item => item != null));

                    State = WorkingState.Parsing;
                    var scanResult = GetNMAPData(ScanParams, Target);
                    var report = new NMAPReport(scanResult);
                    State = WorkingState.Succeed;
                    return report;

                }
                catch (Exception e)
                {
                    State = WorkingState.Fail;
                    ErrorMessage = e.Message;
                }
                return null;
            }
            private static int ExecuteCommand(string command, string parameters, out string output, out string error)
            {
                int exitCode;

                ProcessStartInfo processInfo = new ProcessStartInfo(command, parameters)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

                using (Process process = Process.Start(processInfo))
                {
                    process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                    {
                        Console.WriteLine(e.Data);
                    };
                    process.WaitForExit();

                    output = process.StandardOutput.ReadToEnd();
                    error = process.StandardError.ReadToEnd();
                    exitCode = process.ExitCode;
                }

                return exitCode;
            }
        }
    }
}
