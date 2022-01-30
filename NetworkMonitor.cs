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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Management;
using System.IO;
using System.Text.RegularExpressions;

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

    public class NetworkManager_Test
    {
        public static NetworkManager_Test Instance = new NetworkManager_Test();
        public static ObservableCollection<NetworkModelView> NetworkCollection { get; set; } = new ObservableCollection<NetworkModelView> {
            new NetworkModelView()
            {
                Name = "测试名称",
                Status = "",
                Type = "",
                DownloadSpeed = "2Mbps",
                UploadSpeed = "1Mbps",
                ToolTip = "Test"
            }
        };

        public ObservableCollection<LANComputerModelView> ComputerCollection { get; set; } = new ObservableCollection<LANComputerModelView>
        {
            new LANComputerModelView()
            {
                Name = "LAN Computer",
                Status = "",
                IPAddress = "192.168.1.45",
                Latency = "365ms",
                ToolTip = string.Format(Application.Current.FindResource("ComputerToolTip").ToString(), Environment.NewLine, "LAN Computer", "192.168.1.45", "365ms")
            }
        };
        public ObservableCollection<PortModelView> PortCollection { get; set; } = new ObservableCollection<PortModelView>
        {
            new PortModelView()
            {
                Type = "TCP",
                LocalEndPoint = "10.211.55.3:52940",
                RemoteEndPoint = "40.90.189.152:443",
                State = "Established",
                ToolTip = string.Format(Application.Current.FindResource("PortToolTip").ToString(), Environment.NewLine, "TCP", "10.211.55.3:52940", "40.90.189.152:443")
            }
        };
    }

    public class NetworkManager : CustomINotifyPropertyChanged, IDisposable
    {
        public ObservableCollection<NetworkModelView> NetworkCollection { get; set; }
        public ObservableCollection<PortModelView> PortCollection { get; set; }
        public ObservableCollection<LANComputerModelView> ComputerCollection { get; set; }

        private readonly NetworkMonitor networkMoniter;
        private readonly LocalNetworkManager lanMonitor;
        private readonly PortMonitor portMonitor;

        public string GlobalUploadSpeed { get; set; }
        public string GlobalDownloadSpeed { get; set; }
        public void RefreshChart()
        {
            double value = Math.Max(downloadSpeedQueue.Max(), uploadSpeedQueue.Max());
            double maxValue = Math.Pow(2, Math.Floor(Math.Log(Math.Abs(value), 2)) + 1);
            if (maxValue < 1024)
            {
                maxValue = 1024;
            }

            UploadGeometry = GetChart(uploadSpeedQueue, maxValue);
            DownloadGeometry = GetChart(downloadSpeedQueue, maxValue);
            speedGraphLimit = (long)maxValue;

            Notify(() => UploadGeometry);
            Notify(() => DownloadGeometry);
            Notify(() => SpeedLimit);
        }
        public StreamGeometry GetChart(IEnumerable<long> valueList, double maxValue)
        {
            double max = maxValue;
            double min = 0;
            double range = maxValue;

            StreamGeometry geometry = new StreamGeometry();
            using (StreamGeometryContext context = geometry.Open())
            {
                context.BeginFigure(new Point(0, range), true, true);
                int index = 0;
                foreach (double value in valueList)
                {
                    if (value > max)
                    {
                        context.LineTo(new Point(index, min), true, true);
                    }
                    else if (value < min)
                    {
                        context.LineTo(new Point(index, max), true, true);
                    }
                    else
                    {
                        context.LineTo(new Point(index, range - value), true, true);
                    }
                    index += 1;
                }
                context.LineTo(new Point(119, range), true, true);
                context.LineTo(new Point(119, max), false, false);
                context.LineTo(new Point(119, min), false, false);
                context.LineTo(new Point(119, range), false, false);
                context.LineTo(new Point(0, range), true, true);
                context.LineTo(new Point(0, max), false, false);
                context.LineTo(new Point(0, min), false, false);
                context.LineTo(new Point(0, range), false, false);
            }
            geometry.Freeze();
            return geometry;
        }
        public StreamGeometry DownloadGeometry { get; private set; }
        public StreamGeometry UploadGeometry { get; private set; }

        public string SpeedLimit => NetworkAdapter.GetSpeedString(speedGraphLimit);

        public int NetworkStatus { get; set; }

        private readonly Queue<long> uploadSpeedQueue;
        private readonly Queue<long> downloadSpeedQueue;

        private long speedGraphLimit = 1024;

        public string ComputerName => Dns.GetHostEntry("").HostName;
        public string SystemName
        {
            get
            {
                object name;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem"))
                {
                    name = searcher.Get().Cast<ManagementObject>().Select(item => item.GetPropertyValue("Caption")).FirstOrDefault();
                }
                return name != null ? name.ToString() : "Unknown";
            }
        }
        public string MachineType
        {
            get
            {
                object name0, name1;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_Battery"))
                {
                    name0 = searcher.Get().Cast<ManagementObject>().Select(item => item.GetPropertyValue("Caption")).FirstOrDefault();
                }
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_PortableBattery"))
                {
                    name1 = searcher.Get().Cast<ManagementObject>().Select(item => item.GetPropertyValue("Caption")).FirstOrDefault();
                }
                return (name0 != null || name1 != null) ? "Laptop" : "Desktop";
            }
        }

        public string WorkGroup
        {
            get
            {
                object name;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Workgroup FROM Win32_ComputerSystem"))
                {
                    name = searcher.Get().Cast<ManagementObject>().Select(item => item.GetPropertyValue("Workgroup")).FirstOrDefault();
                }
                return name != null ? name.ToString() : "Unknown";
            }
        }

        public string Model
        {
            get
            {
                object name;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Model FROM Win32_ComputerSystem"))
                {
                    name = searcher.Get().Cast<ManagementObject>().Select(item => item.GetPropertyValue("Model")).FirstOrDefault();
                }
                return name != null ? name.ToString() : "Unknown";
            }
        }

        public string DomainName => Environment.UserDomainName;
        public string UserName => Environment.UserName;

        public NetworkManager()
        {
            uploadSpeedQueue = new Queue<long>(Enumerable.Repeat<long>(0, 120));
            downloadSpeedQueue = new Queue<long>(Enumerable.Repeat<long>(0, 120));

            NetworkCollection = new ObservableCollection<NetworkModelView>();

            ComputerCollection = new ObservableCollection<LANComputerModelView>();

            PortCollection = new ObservableCollection<PortModelView>();

            networkMoniter = new NetworkMonitor();

            lanMonitor = new LocalNetworkManager();

            portMonitor = new PortMonitor();
        }

        public void Start()
        {
            Task.Factory.StartNew(NetworkMonitoring, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(NetworkAdapterMonitoring, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(LocalNetworkMonitoring, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(LocalComputerMonitoring, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(ActivePortMonitoring, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(NetworkStatusMonitoring, TaskCreationOptions.LongRunning);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (lanMonitor != null)
                {
                    lanMonitor.Dispose();
                }
            }
        }

        private void NetworkStatusMonitoring()
        {
            while (true)
            {
                int status = -1;
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    status = 0;
                    using (Ping ping = new Ping())
                    {
                        IPStatus iPStatus = ping.Send("8.8.8.8").Status;
                        if (iPStatus == IPStatus.Success)
                        {
                            status = 1;
                        }
                    }
                }
                Application.Current?.Dispatcher?.BeginInvoke(new Action(() =>
                {
                    NetworkStatus = status;
                    Notify(() => NetworkStatus);
                }));

                Thread.Sleep(5000);
            }
        }


        private void NetworkAdapterMonitoring()
        {
            while (true)
            {
                networkMoniter.EnumerateNetworkAdapters();
                Thread.Sleep(5000);
            }
        }

        private void LocalComputerMonitoring()
        {
            while (true)
            {
                lanMonitor.ListLANComputers();
                Thread.Sleep(5000);
            }
        }
        private void ActivePortMonitoring()
        {
            while (true)
            {
                List<ActivePort> portList = portMonitor.ListActivePort();

                Application.Current?.Dispatcher?.BeginInvoke(new Action(() =>
                {
                    int i = 0;
                    for (; i < portList.Count; i += 1)
                    {
                        if (PortCollection.Count <= i)
                        {
                            PortCollection.Add(new PortModelView(portList[i]));
                        }
                        else
                        {
                            PortCollection[i].Resolve(portList[i]);
                        }
                    }
                    while (PortCollection.Count > i)
                    {
                        PortCollection.RemoveAt(i);
                    }
                }));

                Thread.Sleep(1000);
            }
        }

        private void LocalNetworkMonitoring()
        {
            while (true)
            {
                List<LocalNetworkComputer> computerList = lanMonitor.TestLANComputers();

                Application.Current?.Dispatcher?.BeginInvoke(new Action(() =>
                {
                    int i = 0;
                    for (; i < computerList.Count; i += 1)
                    {
                        if (ComputerCollection.Count <= i)
                        {
                            ComputerCollection.Add(new LANComputerModelView(computerList[i]));
                        }
                        else
                        {
                            ComputerCollection[i].Resolve(computerList[i]);
                        }
                    }
                    while (ComputerCollection.Count > i)
                    {
                        ComputerCollection.RemoveAt(i);
                    }
                }));

                Thread.Sleep(1000);
            }
        }
        
        private void NetworkMonitoring()
        {
            while (true)
            {
                List<NetworkAdapter> adapters = networkMoniter.Refresh();

                Application.Current?.Dispatcher?.BeginInvoke(new Action(() =>
                {
                    long uploadSpeed = 0;
                    long downloadSpeed = 0;

                    int i = 0;
                    for (; i < adapters.Count; i += 1)
                    {
                        uploadSpeed += adapters[i].UploadSpeed;
                        downloadSpeed += adapters[i].downloadSpeed;

                        if (NetworkCollection.Count <= i)
                        {
                            NetworkCollection.Add(new NetworkModelView(adapters[i]));
                        }
                        else
                        {
                            NetworkCollection[i].Resolve(adapters[i]);
                        }
                    }

                    while (NetworkCollection.Count > i)
                    {
                        NetworkCollection.RemoveAt(i);
                    }

                    uploadSpeedQueue.Enqueue(uploadSpeed);
                    downloadSpeedQueue.Enqueue(downloadSpeed);

                    while (uploadSpeedQueue.Count > 120)
                    {
                        uploadSpeedQueue.Dequeue();
                    }
                    while (downloadSpeedQueue.Count > 120)
                    {
                        downloadSpeedQueue.Dequeue();
                    }

                    GlobalUploadSpeed = NetworkAdapter.GetSpeedString(uploadSpeed);
                    GlobalDownloadSpeed = NetworkAdapter.GetSpeedString(downloadSpeed);

                    Notify(() => GlobalUploadSpeed);
                    Notify(() => GlobalDownloadSpeed);

                    RefreshChart();
                }));

                Thread.Sleep(1000);
            }
        }
    }

    public class LocalNetworkManager : IDisposable
    {
        private readonly List<LocalNetworkComputer> computerList;
        private readonly Ping pinger;

        public LocalNetworkManager()
        {
            computerList = new List<LocalNetworkComputer>();

            pinger = new Ping();

            pinger.PingCompleted += Pinger_PingCompleted;
        }

        private void Pinger_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            LocalNetworkComputer computer = e.UserState as LocalNetworkComputer;
            int result;
            int latency;
            if (e.Cancelled || e.Error != null)
            {
                result = 10;
                latency = 1000;
            }
            else
            {
                result = (int)e.Reply.Status;
                latency = result == 0 ? (int)e.Reply.RoundtripTime : 1000;
            }
            computer.Status = result;
            computer.Latency = latency;
            if (computer.IPAddress == string.Empty)
            {
                computer.IPAddress = e.Reply.Address.ToString();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }
            computerList.Clear();
        }

        public List<LocalNetworkComputer> TestLANComputers()
        {
            computerList.Sort((LocalNetworkComputer x, LocalNetworkComputer y) =>
            {
                if (x == null || y == null)
                {
                    return 0;
                }
                else
                {
                    int i = x.Status > 1 ? 1 : x.Status;
                    int j = y.Status > 1 ? 1 : y.Status;
                    if (i == j)
                    {
                        return string.Compare(x.Name, y.Name);
                    }
                    else if (i == 0)
                    {
                        return -1;
                    }
                    else if (j == 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return i > j ? 1 : -1;
                    }
                }
            });

            return computerList;
        }

        public void ListLANComputers()
        {
            DirectoryEntry root = new DirectoryEntry("WinNT:");

            foreach (DirectoryEntry computers in root.Children)
            {
                foreach (DirectoryEntry computer in computers.Children)
                {
                    if (computer.Name != "Schema")
                    {
                        LocalNetworkComputer activeComputer = null;
                        foreach (LocalNetworkComputer item in computerList)
                        {
                            if (item.UID == computer.Path)
                            {
                                activeComputer = item;
                                break;
                            }
                        }

                        if (activeComputer == null)
                        {
                            activeComputer = new LocalNetworkComputer(); ;
                            computerList.Add(activeComputer);
                        }

                        activeComputer.Name = computer.Name;
                        activeComputer.UID = computer.Path;
                        activeComputer.Updated = true;
                        
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
                            
                            if (ipv4 != null)
                            {
                                ipAddress = ipv4.ToString();
                                pinger.SendAsync(ipv4, 1000, activeComputer);
                            }
                            else if (ipv6 != null)
                            {
                                ipAddress = ipv6.ToString();
                                pinger.SendAsync(ipv6, 1000, activeComputer);
                            }
                            else
                            {
                                pinger.SendAsync(computer.Name, 1000, activeComputer);
                            }

                        }
                        catch
                        {
                        }
                        activeComputer.IPAddress = ipAddress;
                    }
                }
            }

            computerList.RemoveAll(computer =>
            {
                if (computer.Updated)
                {
                    computer.Updated = false;
                    return false;
                }
                else
                {
                    return true;
                }
            });

            root.Dispose();
        }
    }
    
    public class PortMonitor
    {
        private string GetProcessName(int pid)
        {
            string name;
            try
            {
                name = Process.GetProcessById(pid).ProcessName;
            }
            catch (Exception)
            {
                name = "-";
            }
            return name;
        }

        private int GetPIDByEndPoint(string endPoint)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            int pid = -1;
            try
            {
                using (Process process = new Process())
                {
                    ProcessStartInfo processInfo = new ProcessStartInfo
                    {
                        Arguments = "-aon",
                        FileName = "netstat.exe",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    process.StartInfo = processInfo;
                    process.Start();

                    StreamReader stdOutput = process.StandardOutput;
                    StreamReader stdError = process.StandardError;

                    string content = stdOutput.ReadToEnd() + stdError.ReadToEnd();
                    string exitStatus = process.ExitCode.ToString();

                    if (exitStatus == "0")
                    {
                        string[] rows = Regex.Split(content, "\r\n");
                        foreach (string row in rows)
                        {
                            string[] tokens = Regex.Split(row, "\\s+");
                            if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")))
                            {
                                dictionary.Add(tokens[2], tokens[1] == "UDP" ? Convert.ToInt32(tokens[4]) : Convert.ToInt32(tokens[5]));
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            if (dictionary.ContainsKey(endPoint))
            {
                pid = dictionary[endPoint];
            }
            return pid;
        }

        public List<ActivePort> ListActivePort()
        {
            List<ActivePort> portList = new List<ActivePort>();
            IPGlobalProperties iPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            foreach (TcpConnectionInformation connection in iPGlobalProperties.GetActiveTcpConnections())
            {
                portList.Add(new ActivePort
                {
                    Type = "TCP",
                    LocalEndPoint = connection.LocalEndPoint.ToString(),
                    RemoteEndPoint = connection.RemoteEndPoint.ToString(),
                    State = connection.State.ToString()
                });
            }
            foreach (IPEndPoint endpoint in iPGlobalProperties.GetActiveTcpListeners())
            {
                portList.Add(new ActivePort
                {
                    Type = "TCP",
                    LocalEndPoint = endpoint.ToString(),
                    RemoteEndPoint = "",
                    State = TcpState.Listen.ToString()
                });
            }
            foreach (IPEndPoint endpoint in iPGlobalProperties.GetActiveUdpListeners())
            {
                portList.Add(new ActivePort
                {
                    Type = "UDP",
                    LocalEndPoint = endpoint.ToString(),
                    RemoteEndPoint = "",
                    State = ""
                });
            }
            return portList;
        }
    }

    public class NetworkMonitor
    {               
        private readonly List<NetworkAdapter> adapterList;

        public NetworkMonitor()
        {
            adapterList = new List<NetworkAdapter>();
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
                    .OrderBy(nic => nic.OperationalStatus != OperationalStatus.Up)
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

    public class LANComputerModelView : CustomINotifyPropertyChanged
    {
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public string Type { get; set; }
        public string MacAddress { get; set; }
        public string Status { get; set; }
        public string UID { get; set; }
        public string Latency { get; set; }
        public string ToolTip { get; set; }

        public LANComputerModelView()
        {

        }

        public LANComputerModelView(LocalNetworkComputer computer)
        {
            Name = computer.Name;
            Status = computer.Status.ToString();
            IPAddress = computer.IPAddress;
            UID = computer.UID;
            Latency = computer.Latency == -1 ? "???" : (computer.Latency >= 1000 ? ">1000ms" : computer.Latency.ToString() + "ms");

            ToolTip = string.Format(Application.Current.FindResource("ComputerToolTip").ToString(),
                Environment.NewLine, Name, IPAddress, UID);
        }

        public void Resolve(LocalNetworkComputer computer)
        {
            string name = computer.Name;
            string status = computer.Status.ToString();
            string ipAddress = computer.IPAddress;
            string uid = computer.UID;
            string latency = computer.Latency == -1 ? "???" : (computer.Latency >= 1000 ? ">1000ms" : computer.Latency.ToString() + "ms");

            string toolTip = string.Format(Application.Current.FindResource("ComputerToolTip").ToString(),
                Environment.NewLine, name, ipAddress, uid);

            if (Name != name)
            {
                Name = name;
                Notify(() => Name);
            }
            if (Status != status)
            {
                Status = status;
                Notify(() => Status);
            }
            if (IPAddress != ipAddress)
            {
                IPAddress = ipAddress;
                Notify(() => IPAddress);
            }
            if (UID != uid)
            {
                UID = uid;
                Notify(() => UID);
            }
            if (Latency != latency)
            {
                Latency = latency;
                Notify(() => Latency);
            }
            if (ToolTip != toolTip)
            {
                ToolTip = toolTip;
                Notify(() => ToolTip);
            }
        }
    }

    public class NetworkModelView : CustomINotifyPropertyChanged
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

        public NetworkModelView()
        {

        }

        public NetworkModelView(NetworkAdapter adapter)
        {
            Name = adapter.Name;
            Status = adapter.Status.ToString();
            Type = adapter.Type.ToString();
            DownloadSpeed = adapter.DownloadSpeedString;
            UploadSpeed = adapter.UploadSpeedString;

            ToolTip = string.Format(Application.Current.FindResource("NetworkToolTip").ToString(),
                Environment.NewLine, adapter.Description, adapter.IPAddress, adapter.MACAddress, adapter.MaxSpeed);
        }

        public void Resolve(NetworkAdapter adapter)
        {
            string name = adapter.Name;
            string status = adapter.Status.ToString();
            string type = adapter.Type.ToString();
            string downloadSpeed = adapter.DownloadSpeedString;
            string uploadSpeed = adapter.UploadSpeedString;
            string toolTip = string.Format(Application.Current.FindResource("NetworkToolTip").ToString(),
                Environment.NewLine, adapter.Description, adapter.IPAddress, adapter.MACAddress, adapter.MaxSpeed);

            if (Name != name)
            {
                Name = name;
                Notify(() => Name);
            }
            if (Status != status)
            {
                Status = status;
                Notify(() => Status);
            }
            if (Type != type)
            {
                Type = type;
                Notify(() => type);
            }
            if (DownloadSpeed != downloadSpeed)
            {
                DownloadSpeed = downloadSpeed;
                Notify(() => DownloadSpeed);
            }
            if (UploadSpeed != uploadSpeed)
            {
                UploadSpeed = uploadSpeed;
                Notify(() => UploadSpeed);
            }
            if (ToolTip != toolTip)
            {
                ToolTip = toolTip;
                Notify(() => ToolTip);
            }
        }
    }

    public class PortModelView : CustomINotifyPropertyChanged
    {
        public string Type { get; set; }
        public string LocalEndPoint { get; set; }
        public string RemoteEndPoint { get; set; }

        public int PID { get; set; }
        public string ProcessName { get; set; }

        public string State { get; set; }
        public string ToolTip { get; set; }

        public string StateText
        {
            get
            {
                switch (State)
                {
                    case "Established":
                        return Application.Current.FindResource("Port_Established").ToString();
                    case "Listen":
                        return Application.Current.FindResource("Port_Listening").ToString();
                    case "CloseWait":
                        return Application.Current.FindResource("Port_CloseWait").ToString();
                    case "TimeWait":
                        return Application.Current.FindResource("Port_TimeWait").ToString();
                    case "SynSent":
                        return Application.Current.FindResource("Port_SynSent").ToString();
                    case "":
                    default:
                        return Application.Current.FindResource("Port_Default").ToString();
                }
            }
        }

        public PortModelView()
        {

        }
        public PortModelView(ActivePort port)
        {
            Type = port.Type;
            State = port.State;
            LocalEndPoint = port.LocalEndPoint;
            RemoteEndPoint = port.RemoteEndPoint;

            ToolTip = string.Format(Application.Current.FindResource("PortToolTip").ToString(),
                Environment.NewLine, Type, LocalEndPoint, RemoteEndPoint);
        }

        public void Resolve(ActivePort port)
        {
            string type = port.Type;
            string state = port.State;
            if (state != "" && state != "Listen" && state != "CloseWait" && state != "TimeWait" && state != "Established")
            {
                var a = state;
            }
            string localEndPoint = port.LocalEndPoint;
            string remoteEndPoint = port.RemoteEndPoint;
            int pid = port.PID;
            string processName = port.ProcessName;

            string toolTip = string.Format(Application.Current.FindResource("PortToolTip").ToString(),
                Environment.NewLine, type, localEndPoint, remoteEndPoint);

            if (Type != type)
            {
                Type = type;
                Notify(() => Type);
            }
            if (State != state)
            {
                State = state;
                Notify(() => State);
                Notify(() => StateText);
            }
            if (LocalEndPoint != localEndPoint)
            {
                LocalEndPoint = localEndPoint;
                Notify(() => LocalEndPoint);
            }
            if (RemoteEndPoint != remoteEndPoint)
            {
                RemoteEndPoint = remoteEndPoint;
                Notify(() => RemoteEndPoint);
            }
            if (PID != pid)
            {
                PID = pid;
                Notify(() => PID);
            }
            if (ProcessName != processName)
            {
                ProcessName = processName;
                Notify(() => ProcessName);
            }
            if (Type != type)
            {
                Type = type;
                Notify(() => Type);
            }
            if (ToolTip != toolTip)
            {
                ToolTip = toolTip;
                Notify(() => ToolTip);
            }
        }
    }
}
