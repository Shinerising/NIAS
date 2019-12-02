using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;

namespace LanMonitor
{
    public class ActivePort
    {
        public string Type;
        public string LocalEndPoint;
        public string RemoteEndPoint;
        public string State;
        public int PID;
        public string ProcessName;
    }

    public class LocalNetworkComputer
    {
        public string UID;
        public string Name;
        public string IPAddress;
        public string Type;
        public string MacAddress;
        public int Status = -1;
        public int Latency = -1;
        public bool Updated = false;
    }

    public class NetworkAdapter
    {
        private NetworkInterface networkInterface;

        internal NetworkAdapter(NetworkInterface network)
        {
            networkInterface = network;
            Name = network?.Name;
        }

        public void SetNetwork(NetworkInterface network)
        {
            networkInterface = network;
        }

        internal long downloadSpeed;
        internal long uploadSpeed;

        private long downloadValue;
        private long uploadValue;
        private long downloadValue_Old;
        private long uploadValue_Old;

        internal PerformanceCounter downloadCount;
        internal PerformanceCounter uploadCounter;
        internal PerformanceCounter bandwidthCounter;

        public string Name { get; set; }
        public string Description => networkInterface?.Description;
        public string ID => networkInterface?.Id;
        public string IPAddress
        {
            get
            {
                try
                {
                    IEnumerable<string> ipList = networkInterface?.GetIPProperties().UnicastAddresses
                        .Where((ip) => ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        .Select((ip) => ip.Address.ToString()).Cast<string>();
                    return string.Join(",", ipList);
                }
                catch
                {
                }
                return Application.Current.FindResource("UnknownIP").ToString();
            }
        }
        public string MACAddress => string.Join(":", (from c in networkInterface?.GetPhysicalAddress().GetAddressBytes() select c.ToString("X2")).ToArray());
        public string MaxSpeed => GetLegacySpeedString((long)networkInterface?.Speed);
        public int Status => (int)networkInterface?.OperationalStatus;
        public int Type => (int)networkInterface?.NetworkInterfaceType;

        internal void Init()
        {
            if (downloadCount == null || uploadCounter == null)
            {
                return;
            }
            try
            {
                downloadValue_Old = downloadCount.NextSample().RawValue;
                uploadValue_Old = uploadCounter.NextSample().RawValue;
            }
            catch
            {
                downloadValue_Old = 0;
                uploadValue_Old = 0;
            }
        }

        internal void Refresh()
        {
            if (downloadCount == null || uploadCounter == null)
            {
                return;
            }
            try
            {
                downloadValue = downloadCount.NextSample().RawValue;
                uploadValue = uploadCounter.NextSample().RawValue;

                downloadSpeed = downloadValue_Old == 0 ? 0 : downloadValue - downloadValue_Old;
                if (downloadValue < 0)
                {
                    downloadValue = 0;
                }
                downloadValue_Old = downloadValue;

                uploadSpeed = uploadValue_Old == 0 ? 0 : uploadValue - uploadValue_Old;
                if (uploadSpeed < 0)
                {
                    uploadSpeed = 0;
                }
                uploadValue_Old = uploadValue;
            }
            catch
            {
                downloadValue_Old = 0;
                uploadValue_Old = 0;

                downloadValue = 0;
                uploadValue = 0;
            }
        }

        internal void Dispose()
        {
        }

        public override string ToString() => Name;

        public long DownloadSpeed => downloadSpeed;
        public long UploadSpeed => uploadSpeed;

        public static string GetSpeedString(long speed)
        {
            if (speed > 1073741824)
            {
                return ">1GB/s";
            }
            else if (speed > 1048576)
            {
                return string.Format("{0:G4}{1}", speed / 1048576.0, "MB/s");
            }
            else if (speed > 1024)
            {
                return string.Format("{0:G4}{1}", speed / 1024.0, "KB/s");
            }
            else if (speed < 0)
            {
                return "0B/s";
            }
            else
            {
                return speed + "B/s";
            }
        }

        public static string GetLegacySpeedString(long speed)
        {
            if (speed > 1000000000000)
            {
                return ">1Tbps";
            }
            else if (speed >= 1000000000)
            {
                return string.Format("{0:G4}{1}", speed / 1000000000.0, "Gbps");
            }
            else if (speed >= 1000000)
            {
                return string.Format("{0:G4}{1}", speed / 1000000.0, "Mbps");
            }
            else if (speed >= 1000)
            {
                return string.Format("{0:G4}{1}", speed / 1000.0, "Kbps");
            }
            else if (speed < 0)
            {
                return "0bps";
            }
            else
            {
                return speed + "bps";
            }
        }

        public string DownloadSpeedString => GetSpeedString(downloadSpeed);
        public string UploadSpeedString => GetSpeedString(uploadSpeed);
    }
}
