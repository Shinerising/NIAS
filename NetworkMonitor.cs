using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LanMonitor
{
    public class CustomINotifyPropertyChanged : INotifyPropertyChanged
    {
        /// <summary>
        /// 属性变化时的事件处理
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 通知UI更新数据的方法
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <param name="propertyExpression">待更新的数据项</param>
        protected void Notify<T>(Expression<Func<T>> propertyExpression)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyExpression.Body as MemberExpression).Member.Name));
        }
    }
    
    public class NetworkManager : CustomINotifyPropertyChanged
    {
        public ObservableCollection<NetworkModelView> NetworkCollection { get; set; }
        public ObservableCollection<LANComputerModelView> ComputerCollection { get; set; }

        private readonly NetworkMonitor networkMoniter;
        private readonly LocalNetworkManager lanMonitor;

        public string GlobalUploadSpeed { get; set; }
        public string GlobalDownloadSpeed { get; set; }

        public PointCollection GraphPointCollection
        {
            get
            {
                PointCollection collection = new PointCollection
                {
                    new Point(0, 20)
                };
                int x = 0;
                long y = 0;
                foreach (long speed in speedQueue)
                {
                    y = (1000000 - speed) * 20 / 1000000;
                    collection.Add(new Point(x, y));
                    x += 1;
                }
                collection.Add(new Point(x, 20));
                return collection;
            }
        }

        private readonly Queue<long> speedQueue;

        public NetworkManager()
        {
            speedQueue = new Queue<long>();

            NetworkCollection = new ObservableCollection<NetworkModelView>();

            ComputerCollection = new ObservableCollection<LANComputerModelView>();

            networkMoniter = new NetworkMonitor();

            lanMonitor = new LocalNetworkManager();
        }

        public void Start()
        {
            Task.Factory.StartNew(NetworkMonitoring, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(NetworkAdapterMonitoring, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(LocalNetworkMonitoring, TaskCreationOptions.LongRunning);
        }

        private void NetworkAdapterMonitoring()
        {
            while (true)
            {
                Thread.Sleep(5000);
                networkMoniter.EnumerateNetworkAdapters();
            }
        }

        private void LocalNetworkMonitoring()
        {
            while (true)
            {
                List<LocalNetworkComputer> computerList = lanMonitor.EnumerateLANComputers();

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ComputerCollection.Clear();
                    for (int i = 0; i < computerList.Count; i += 1)
                    {
                        ComputerCollection.Add(new LANComputerModelView(computerList[i]));
                    }
                }));

                Thread.Sleep(5000);
            }
        }

        private void NetworkMonitoring()
        {
            while (true)
            {
                List<NetworkAdapter> adapters = networkMoniter.Refresh();

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    long uploadSpeed = 0;
                    long downloadSpeed = 0;

                    NetworkCollection.Clear();
                    for (int i = 0; i < adapters.Count; i += 1)
                    {
                        uploadSpeed += adapters[i].UploadSpeed;
                        downloadSpeed += adapters[i].downloadSpeed;

                        NetworkCollection.Add(new NetworkModelView(adapters[i]));
                    }

                    speedQueue.Enqueue(uploadSpeed + downloadSpeed);

                    while (speedQueue.Count > 200)
                    {
                        speedQueue.Dequeue();
                    }

                    GlobalUploadSpeed = NetworkAdapter.GetSpeedString(uploadSpeed);
                    GlobalDownloadSpeed = NetworkAdapter.GetSpeedString(downloadSpeed);

                    Notify(() => GlobalUploadSpeed);
                    Notify(() => GlobalDownloadSpeed);
                    Notify(() => GraphPointCollection);
                }));

                Thread.Sleep(1000);
            }
        }
    }

    public class LocalNetworkManager
    {
        private List<LocalNetworkComputer> computerList;

        public LocalNetworkManager()
        {
            computerList = new List<LocalNetworkComputer>();
        }

        public List<LocalNetworkComputer> EnumerateLANComputers()
        {
            computerList.Clear();

            DirectoryEntry root = new DirectoryEntry("WinNT:");

            Ping pinger = new Ping();
            foreach (DirectoryEntry computers in root.Children)
            {
                foreach (DirectoryEntry computer in computers.Children)
                {
                    if (computer.Name != "Schema")
                    {
                        int result = -1;
                        int latency = 0;
                        string macAddress = string.Empty;
                        string ipAddress = string.Empty;
                        try
                        {
                            IPAddress ipv4 = null;
                            IPAddress ipv6 = null;

                            IPHostEntry ipHost = Dns.GetHostEntry(computer.Name);
                            foreach (IPAddress ip in ipHost.AddressList)
                            {
                                if (ip.AddressFamily == AddressFamily.InterNetwork)
                                {
                                    ipv4 = ip;
                                }
                                else if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                                {
                                    ipv6 = ip;
                                }
                            }

                            PingReply reply;
                            if (ipv4 != null)
                            {
                                reply = pinger.Send(ipv4, 1000);
                                ipAddress = ipv4.ToString();

                            }
                            else if (ipv6 != null)
                            {
                                reply = pinger.Send(ipv6, 1000);
                                ipAddress = ipv6.ToString();
                            }
                            else
                            {
                                reply = pinger.Send(computer.Name, 1000);
                                ipAddress = reply.Address.ToString();
                            }
                            
                            result = (int)reply.Status;
                            latency = result == 0 ? (int)reply.RoundtripTime : 1000;
                        }
                        catch
                        {
                        }
                        computerList.Add(new LocalNetworkComputer()
                        {
                            Name = computer.Name,
                            IPAddress = ipAddress,
                            MacAddress = macAddress,
                            Status = result,
                            Latency = latency
                        });
                    }
                }
            }
            pinger.Dispose();
            return computerList;
        }
    }

    public class LocalNetworkComputer
    {
        public string Name;
        public string IPAddress;
        public string Type;
        public string MacAddress;
        public int Status;
        public int Latency;
    }

    public class LANComputerModelView
    {
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public string Type { get; set; }
        public string MacAddress { get; set; }
        public string Status { get; set; }
        public string Latency { get; set; }
        public string ToolTip { get; set; }

        public LANComputerModelView(LocalNetworkComputer computer)
        {
            Name = computer.Name;
            Status = computer.Status.ToString();
            IPAddress = computer.IPAddress;
            MacAddress = computer.MacAddress;
            Latency = computer.Latency >= 1000 ? ">1000ms" : computer.Latency.ToString() + "ms";

            ToolTip = string.Format("计算机名称：{1}{0}IP地址：{2}{0}网络延迟：{3}",
                Environment.NewLine, Name, IPAddress, Latency);
        }
    }

    public class NetworkModelView
    {
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public string Type { get; set; }
        public string MacAddress { get; set; }
        public string Status { get; set; }
        public string Speed { get; set; }
        public string ToolTip { get; set; }
        public string DownloadSpeed { get; set; }
        public string UploadSpeed { get; set; }

        public NetworkModelView(NetworkAdapter adapter)
        {
            Name = adapter.Name;
            Status = adapter.Status.ToString();
            Type = adapter.Type.ToString();
            DownloadSpeed = adapter.DownloadSpeedString;
            UploadSpeed = adapter.UploadSpeedString;

            ToolTip = string.Format("网卡名称：{1}{0}IP地址：{2}{0}MAC地址：{3}{0}带宽：{4}",
                Environment.NewLine, adapter.Description, adapter.IPAddress, adapter.MACAddress, adapter.MaxSpeed);
        }
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
                    foreach (UnicastIPAddressInformation ip in networkInterface?.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip.Address.ToString();
                        }
                    }
                }
                catch
                {
                }
                return "不可用";
            }
        }
        public string MACAddress => string.Join(":", (from c in networkInterface?.GetPhysicalAddress().GetAddressBytes() select c.ToString("X2")).ToArray());
        public string MaxSpeed => GetSpeedString((long)networkInterface?.Speed);
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
            if (speed > 1000000000)
            {
                return ">1GB/s";
            }
            else if (speed > 1000000)
            {
                return string.Format("{0:G4}{1}", speed / 1000000.0, "MB/s");
            }
            else if (speed > 1000)
            {
                return string.Format("{0:G4}{1}", speed / 1000.0, "KB/s");
            }
            else
            {
                return speed + "B/s";
            }
        }

        public string DownloadSpeedString => GetSpeedString(downloadSpeed);
        public string UploadSpeedString => GetSpeedString(uploadSpeed);
    }

    public class NetworkMonitor
    {               
        private readonly List<NetworkAdapter> adapterList;

        public NetworkMonitor()
        {
            adapterList = new List<NetworkAdapter>();
            EnumerateNetworkAdapters();
        }

        public void EnumerateNetworkAdapters()
        {
            lock (adapterList)
            {
                PerformanceCounterCategory category = new PerformanceCounterCategory("Network Interface");
                string[] interfaceArray = category.GetInstanceNames();

                IEnumerable networkCollection = NetworkInterface
                    .GetAllNetworkInterfaces()
                    .Where(nic => nic.NetworkInterfaceType != NetworkInterfaceType.Loopback && nic.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                    .Select(nic => nic);

                int index = 0;
                foreach (NetworkInterface network in networkCollection)
                {
                    if (adapterList.Count > index && adapterList[index].ID == network.Id)
                    {
                        adapterList[index].SetNetwork(network);
                        index += 1;
                        continue;
                    }
                    string name = network.Name;
                    string description = network.Description;
                    string flag = string.Empty;
                    foreach (string interfaceName in interfaceArray)
                    {
                        if (GetLetter(name) == GetLetter(interfaceName) || GetLetter(description) == GetLetter(interfaceName))
                        {
                            flag = interfaceName;
                            break;
                        }
                    }
                    if (flag != string.Empty)
                    {
                        NetworkAdapter adapter = new NetworkAdapter(network)
                        {
                            downloadCount = new PerformanceCounter("Network Interface", "Bytes Received/sec", flag),
                            uploadCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", flag),
                            bandwidthCounter = new PerformanceCounter("Network Interface", "Current Bandwidth", flag)
                        };

                        if (adapterList.Count > index)
                        {
                            adapterList[index] = adapter;
                        }
                        else
                        {
                            adapterList.Add(adapter);
                        }
                    }
                    index += 1;
                }
                while (adapterList.Count > index)
                {
                    adapterList.RemoveAt(index);
                }
            }
        }

        public List<NetworkAdapter> Refresh()
        {
            foreach (NetworkAdapter adapter in adapterList)
            {
                adapter.Refresh();
            }
            return adapterList;
        }

        public void StartMonitoring()
        {
            if (adapterList.Count > 0)
            {
                foreach (NetworkAdapter adapter in adapterList)
                {
                    adapter.Refresh();
                }
            }
        }
        
        public void StopMonitoring()
        {
            if (adapterList.Count > 0)
            {
                foreach (NetworkAdapter adapter in adapterList)
                {
                    adapter.Dispose();
                }
            }
        }

        private static string GetLetter(string input)
        {
            return new string(input.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToLower();
        }
    }
}
