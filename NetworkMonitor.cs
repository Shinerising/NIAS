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
using System.Reflection;
using System.Configuration;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Threading;

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
        /// <param name="obj">以待更新项目为属性的匿名类实例</param>
        protected void Notify<T>(T obj)
        {
            if (obj == null)
            {
                return;
            }
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property.Name));
            }
        }
    }

    public class NetworkManager_Test
    {
        public static NetworkManager_Test Instance = new NetworkManager_Test();
        public ObservableCollection<NetworkModelView> NetworkCollection => new ObservableCollection<NetworkModelView> {
            new NetworkModelView()
            {
                Name = "测试名称",
                Status = "",
                Type = "",
                DownloadSpeed = "2Mbps",
                UploadSpeed = "1Mbps"
            }
        };

        public ObservableCollection<LANComputerModelView> ComputerCollection => new ObservableCollection<LANComputerModelView>
        {
            new LANComputerModelView()
            {
                Name = "LAN Computer",
                Status = "",
                IPAddress = "192.168.1.45",
                Latency = "365ms"
            }
        };
        public ObservableCollection<PortModelView> PortCollection => new ObservableCollection<PortModelView>
        {
            new PortModelView()
            {
                Type = "TCP",
                LocalEndPoint = "10.211.55.3:52940",
                RemoteEndPoint = "40.90.189.152:443",
                State = "Established"
            }
        };

        public ObservableCollection<ToastMessage> ToastCollection => new ObservableCollection<ToastMessage>
        {
            new ToastMessage()
            {
                Title = "检测到网络故障",
                Content = "某个设备的网络通信已断开，请检查设备连接状态！",
                Time = DateTime.Now
            }
        };

        public List<SwitchDeviceModelView> SwitchDeviceList => new List<string>() { "172.16.24.1", "172.16.24.188" }.Select(item => SwitchDeviceModelView.GetPreviewInstance(item)).ToList();
        public List<LanHostModelView> LanHostList => new List<LanHostModelView>() {
            new LanHostModelView("Host01", "172.16.24.90,172.16.34.90"),
            new LanHostModelView("Host02", "172.16.24.91,172.16.34.91"),
            new LanHostModelView("Host03", "172.16.24.92,172.16.34.92"),
            new LanHostModelView("Host04", "172.16.24.93,172.16.34.93")
        };
    }

    public class NetworkManager : CustomINotifyPropertyChanged, IDisposable
    {
        public List<NetworkModelView> NetworkCollection { get; set; }
        public List<PortModelView> PortCollection { get; set; }
        public List<LANComputerModelView> ComputerCollection { get; set; }
        public List<SwitchDeviceModelView> SwitchDeviceList { get; set; }
        public List<LanHostModelView> LanHostList { get; set; }
        public string SwitchPortCount => SwitchDeviceList == null ? "0" : string.Join(",", SwitchDeviceList.Select(item => item.PortCount));
        public string SwitchHostCount => SwitchDeviceList == null ? "0" : string.Join(",", SwitchDeviceList.Select(item => item.HostCount));
        public string LanHostCount => SwitchDeviceList == null ? "0" : string.Join(",", LanHostList.Select(item => item.ActiveCount));

        private readonly NetworkMonitor networkMoniter;
        private readonly LocalNetworkManager lanMonitor;
        private readonly PortMonitor portMonitor;

        public bool IsSwitchEnabled { get; set; } = true;

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

            Notify(new { UploadGeometry, DownloadGeometry, SpeedLimit });
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
        public string Manufacturer
        {
            get
            {
                object name;
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Manufacturer FROM Win32_ComputerSystem"))
                {
                    name = searcher.Get().Cast<ManagementObject>().Select(item => item.GetPropertyValue("Manufacturer")).FirstOrDefault();
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

            NetworkCollection = new List<NetworkModelView>();
            ComputerCollection = new List<LANComputerModelView>();
            PortCollection = new List<PortModelView>();

            networkMoniter = new NetworkMonitor();
            lanMonitor = new LocalNetworkManager();
            portMonitor = new PortMonitor();

            InitializeSwitchData();
        }

        public void Start()
        {
            Task.Factory.StartNew(NetworkMonitoring, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(NetworkAdapterMonitoring, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(LocalNetworkMonitoring, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(LocalComputerMonitoring, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(ActivePortMonitoring, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(NetworkStatusMonitoring, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(SwitchMonitoring, TaskCreationOptions.LongRunning);
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

        private void InitializeSwitchData()
        {
            string isSwitchEnabled = ConfigurationManager.AppSettings.Get("switch_enable");
            if (isSwitchEnabled.ToUpper() != "TRUE")
            {
                IsSwitchEnabled = false;
                return;
            }

            IsSwitchEnabled = true;

            string name = ConfigurationManager.AppSettings.Get("switch_username");
            string auth = ConfigurationManager.AppSettings.Get("switch_auth");
            string priv = ConfigurationManager.AppSettings.Get("switch_priv");

            NameValueCollection switchList = (NameValueCollection)ConfigurationManager.GetSection("switchList");
            NameValueCollection deviceList = (NameValueCollection)ConfigurationManager.GetSection("deviceList");

            SwitchDeviceList = switchList == null ? new List<SwitchDeviceModelView>() : switchList.AllKeys.Select(item => new SwitchDeviceModelView(item, switchList[item])).ToList();
            LanHostList = deviceList == null ? new List<LanHostModelView>() : deviceList.AllKeys.Select(item => new LanHostModelView(item, deviceList[item])).ToList();

            SnmpHelper.Initialize(name, auth, priv);
        }

        private void NetworkStatusMonitoring()
        {
            while (true)
            {
                int status = -1;
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    status = 0;
                    try
                    {
                        using (Ping ping = new Ping())
                        {
                            IPStatus iPStatus = ping.Send("8.8.8.8").Status;
                            if (iPStatus == IPStatus.Success)
                            {
                                status = 1;
                            }
                        }
                    }
                    catch
                    {

                    }
                }

                NetworkStatus = status;
                Notify(new { NetworkStatus });

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

                if (PortCollection != null && portList.Count == PortCollection.Count)
                {
                    for (int i = 0; i < portList.Count; i += 1)
                    {
                        PortCollection[i].Resolve(portList[i]);
                    }
                }
                else
                {
                    PortCollection = portList.Select(item => new PortModelView(item)).ToList();
                    Notify(new { PortCollection });
                }

                Thread.Sleep(5000);
            }
        }

        private void LocalNetworkMonitoring()
        {
            while (true)
            {
                List<LocalNetworkComputer> computerList = lanMonitor.TestLANComputers();

                if (ComputerCollection != null && ComputerCollection.Count == computerList.Count)
                {
                    for (int i = 0; i < computerList.Count; i += 1)
                    {
                        ComputerCollection[i].Resolve(computerList[i]);
                    }
                }
                else
                {
                    ComputerCollection = computerList.Select(item => new LANComputerModelView(item)).ToList();
                    Notify(new { ComputerCollection });
                }

                Thread.Sleep(5000);
            }
        }
        
        private void NetworkMonitoring()
        {
            while (true)
            {
                List<NetworkAdapter> adapters = networkMoniter.Refresh();

                long uploadSpeed = 0;
                long downloadSpeed = 0;

                for (int i = 0; i < adapters.Count; i += 1)
                {
                    uploadSpeed += adapters[i].UploadSpeed;
                    downloadSpeed += adapters[i].downloadSpeed;
                }

                if (NetworkCollection != null && adapters.Count == NetworkCollection.Count)
                {
                    for (int i = 0; i < adapters.Count; i += 1)
                    {
                        NetworkCollection[i].Resolve(adapters[i]);
                    }
                }
                else
                {
                    NetworkCollection = adapters.Select(item => new NetworkModelView(item)).ToList();
                    Notify(new { NetworkCollection });
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

                Notify(new { GlobalUploadSpeed, GlobalDownloadSpeed });

                RefreshChart();

                Thread.Sleep(1000);
            }
        }

        private void SwitchMonitoring()
        {
            if (!IsSwitchEnabled)
            {
                return;
            }

            while (true)
            {
                foreach (SwitchDeviceModelView switchDevice in SwitchDeviceList)
                {
                    var report = SnmpHelper.GetReportMessage(switchDevice.EndPoint);
                    if (report == null)
                    {
                        if (switchDevice.State == DeviceState.Online)
                        {
                            AddToast("消息提示", string.Format("无法使用IP地址[{0}]采集交换机[{1}]的信息，请检查设备连接状态！", switchDevice.Address, switchDevice.Name));
                        }
                        switchDevice.State = DeviceState.Offline;
                        switchDevice.SetIdle();
                        switchDevice.Refresh();
                        continue;
                    }
                    switchDevice.State = DeviceState.Online;

                    {
                        var dict = SnmpHelper.FetchStringData(report, switchDevice.EndPoint, SnmpHelper.OIDString.OID_sysDescr);
                        switchDevice.Information = dict?.FirstOrDefault().Value;
                    }

                    {
                        var dict0 = SnmpHelper.FetchStringData(report, switchDevice.EndPoint, SnmpHelper.OIDString.OID_ifIndex);
                        var dict1 = SnmpHelper.FetchStringData(report, switchDevice.EndPoint, SnmpHelper.OIDString.OID_ifType);
                        var dict2 = SnmpHelper.FetchStringData(report, switchDevice.EndPoint, SnmpHelper.OIDString.OID_ifDescr);
                        var dict3 = SnmpHelper.FetchStringData(report, switchDevice.EndPoint, SnmpHelper.OIDString.OID_ifOperStatus);
                        List<SwitchPort> list = new List<SwitchPort>();

                        if (dict0 != null && dict1 != null && dict2 != null && dict3 != null)
                        {
                            for (int i = 0; i < dict0.Count; i += 1)
                            {
                                if (dict1.Count > i && dict2.Count > i && dict3.Count > i)
                                {
                                    if (dict1.ElementAt(i).Value == "6")
                                    {
                                        string text = dict2.ElementAt(i).Value;
                                        SwitchPort port = new SwitchPort
                                        {
                                            Index = int.Parse(dict0.ElementAt(i).Value),
                                            Name = text.Split('/').Last(),
                                            Brief = text,
                                            IsUp = dict3.ElementAt(i).Value == "1"
                                        };
                                        list.Add(port);
                                    }
                                }
                            }

                            for (int i = 0; i < list.Count; i += 1)
                            {
                                if (i >= 24)
                                {
                                    list[i].IsFiber = true;
                                }
                                else if (i % 2 == 0)
                                {
                                    var tmp = list[i];
                                    list[i] = list[i + 1];
                                    list[i + 1] = tmp;
                                }
                            }

                        }

                        switchDevice.RefreshPortList(list);
                    }

                    {
                        var dict3 = SnmpHelper.FetchBytesData(report, switchDevice.EndPoint, SnmpHelper.OIDString.OID_hwArpDynMacAdd);
                        var dict4 = SnmpHelper.FetchStringData(report, switchDevice.EndPoint, SnmpHelper.OIDString.OID_hwArpDynOutIfIndex);
                        var dict5 = new Dictionary<string, string>();

                        if (dict3 != null && dict4 != null)
                        {
                            for (int i = 0; i < dict3.Count; i += 1)
                            {
                                string mac = BitConverter.ToString(dict3.ElementAt(i).Value, 2);
                                if (dict4.Count > i && !dict5.ContainsKey(mac))
                                {
                                    dict5.Add(mac, dict4.ElementAt(i).Value);
                                }
                            }
                        }

                        var dict0 = SnmpHelper.FetchBytesData(report, switchDevice.EndPoint, SnmpHelper.OIDString.OID_ipNetToMediaPhysAddress);
                        var dict1 = SnmpHelper.FetchStringData(report, switchDevice.EndPoint, SnmpHelper.OIDString.OID_ipNetToMediaNetAddress);
                        var dict2 = SnmpHelper.FetchStringData(report, switchDevice.EndPoint, SnmpHelper.OIDString.OID_ipNetToMediaType);

                        List<SwitchHost> list = new List<SwitchHost>();

                        if (dict0 != null && dict1 != null && dict2 != null)
                        {
                            for (int i = 0; i < dict0.Count; i += 1)
                            {
                                if (dict1.Count > i && dict2.Count > i)
                                {
                                    string mac = BitConverter.ToString(dict0.ElementAt(i).Value, 2);
                                    int portIndex = dict5.ContainsKey(mac) ? int.Parse(dict5[mac]) : 0;
                                    SwitchHost host = new SwitchHost
                                    {
                                        MACAddress = mac.Replace('-', ':'),
                                        IPAddress = dict1.ElementAt(i).Value,
                                        State = (HostState)(int.Parse(dict2.ElementAt(i).Value)),
                                        PortIndex = portIndex,
                                        Port = switchDevice.PortList.FirstOrDefault(item => item.Index == portIndex)
                                    };

                                    if (portIndex != 0)
                                    {
                                        var find = list.Find(item => item.PortIndex == portIndex);
                                        if (find != null)
                                        {
                                            find.IsCascade = true;
                                            host.IsCascade = true;
                                        }
                                    }

                                    list.Add(host);
                                }
                            }
                        }

                        switchDevice.RefreshHostList(list);
                    }

                    {
                        foreach (LanHostModelView host in LanHostList)
                        {
                            List<LanHostAdapter> list = host.AdapterList;
                            for (int i = 0; i < list.Count; i += 1)
                            {
                                string switchIP = null;
                                SwitchHost switchHost = null;
                                SwitchDeviceModelView switchParent = null;
                                int switchIndex = 0;
                                foreach (SwitchDeviceModelView device in SwitchDeviceList)
                                {
                                    switchHost = device.HostList?.FirstOrDefault(item => item.IPAddress == list[i].IPAddress && !item.IsCascade);
                                    if (switchHost != null)
                                    {
                                        switchIP = device.Address;
                                        switchParent = device;
                                        break;
                                    }
                                    switchIndex += 1;
                                }

                                if (switchIP == null)
                                {
                                    if (list[i].State == DeviceState.Online)
                                    {
                                        if (list[i].SwitchDevice.State == DeviceState.Offline)
                                        {
                                            list[i].State = DeviceState.Unknown;
                                        }
                                        else
                                        {
                                            list[i].State = DeviceState.Offline;
                                            AddToast("消息提示", string.Format("主机[{0}]的网络适配器[{1}]已断开连接！", host.Name, list[i].IPAddress));
                                        }
                                    }
                                    list[i].SwitchIPAddress = null;
                                    list[i].SwitchDevice = null;
                                    list[i].Host = null;
                                }
                                else
                                {
                                    if (list[i].State == DeviceState.Offline)
                                    {
                                        AddToast("消息提示", string.Format("主机[{0}]的网络适配器[{1}]已连接至交换机[{2}]！", host.Name, list[i].IPAddress, switchParent.Name));
                                    }
                                    list[i].SwitchIPAddress = switchIP;
                                    list[i].SwitchDevice = switchParent;
                                    list[i].Host = switchHost;
                                    list[i].RefreshVector(i, list.Count, switchIndex, SwitchDeviceList.Count);
                                    list[i].State = DeviceState.Online;
                                }

                                list[i].Refresh();
                            }
                        }
                    }

                    switchDevice.Refresh();
                }

                Notify(new { SwitchPortCount, SwitchHostCount, LanHostCount });

                Thread.Sleep(1000);
            }
        }

        public ObservableCollection<ToastMessage> ToastCollection { get; set; } = new ObservableCollection<ToastMessage>();

        public void AddToast(string title, string message)
        {
            Dispatcher dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
            if (dispatcher == null)
            {
                dispatcher = Application.Current.Dispatcher;
            }
            dispatcher?.Invoke(() =>
            {
                while (ToastCollection.Count > 3)
                {
                    ToastCollection.RemoveAt(0);
                }
                ToastCollection.Add(new ToastMessage(title, message));
            });
        }

        public void RemoveToast(ToastMessage toast)
        {
            if (toast == null)
            {
                return;
            }
            ToastCollection.Remove(toast);
        }
    }

    public class ToastMessage
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public ToastMessage()
        {

        }
        public ToastMessage(string title, string message)
        {
            Time = DateTime.Now;
            Title = title;
            Content = message;
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
}
