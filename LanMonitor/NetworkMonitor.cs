﻿using System;
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
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Configuration;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Threading;
using System.Media;
using System.Xml.Linq;
using System.Runtime.Versioning;
using Microsoft.Management.Infrastructure;
using SNMP;

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
        public List<SwitchConnectonModelView> ConnectionList => new List<SwitchConnectonModelView>() { new SwitchConnectonModelView("", SwitchDeviceModelView.GetPreviewInstance("172.16.24.1"), SwitchDeviceModelView.GetPreviewInstance("172.16.24.2"), 0, 1, null, null) };
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
        public List<SwitchConnectonModelView> ConnectionList { get; set; }
        public List<OverrideConnection> OverrideConnectionList { get; set; }
        public string SwitchPortCount => SwitchDeviceList == null ? "0" : string.Join(", ", SwitchDeviceList.Select(item => item.PortCount));
        public string SwitchHostCount => SwitchDeviceList == null ? "0" : string.Join(", ", SwitchDeviceList.Select(item => item.HostCount));
        public string LanHostCount => SwitchDeviceList == null ? "0" : string.Join(", ", LanHostList.Select(item => item.ActiveCount));

        private readonly NetworkMonitor networkMoniter;
        private readonly LocalNetworkManager lanMonitor;
        private readonly PortMonitor portMonitor;

        private readonly CancellationTokenSource cancellation;

        public bool IsSwitchEnabled { get; set; } = true;
        public bool IsSwitchPingEnabled { get; set; } = true;
        private long LastSwitchRefreshTimeStamp;
        private readonly Stopwatch RefreshStopwatch = new Stopwatch();

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

        [SupportedOSPlatform("windows")]
        public static string ManagementQuery(string query, string property)
        {
            using CimSession session = CimSession.Create(null);
            return session.QueryInstances(@"root\cimv2", "WQL", query).FirstOrDefault()?.CimInstanceProperties[property].Value.ToString();
        }
        public string SystemName
        {
            get
            {
                return ManagementQuery("SELECT Caption FROM Win32_OperatingSystem", "Caption") ?? "Unknown";
            }
        }
        public string MachineType
        {
            get
            {
                return (ManagementQuery("SELECT Caption FROM Win32_Battery", "Caption") != null || ManagementQuery("SELECT Caption FROM Win32_PortableBattery", "Caption") != null) ? "Laptop" : "Desktop";
            }
        }

        public string WorkGroup
        {
            get
            {
                return ManagementQuery("SELECT Workgroup FROM Win32_ComputerSystem", "Workgroup") ?? "Unknown";
            }
        }
        public string Manufacturer
        {
            get
            {
                return ManagementQuery("SELECT Manufacturer FROM Win32_ComputerSystem", "Manufacturer") ?? "Unknown";
            }
        }

        public string Model
        {
            get
            {
                return ManagementQuery("SELECT Model FROM Win32_ComputerSystem", "Model") ?? "Unknown";
            }
        }

        public string DomainName => Environment.UserDomainName;
        public string UserName => Environment.UserName;

        public NetworkManager()
        {
            cancellation = new CancellationTokenSource();

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
            Task.Factory.StartNew(NetworkMonitoring, cancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            Task.Factory.StartNew(NetworkAdapterMonitoring, cancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            Task.Factory.StartNew(LocalNetworkMonitoring, cancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            Task.Factory.StartNew(LocalComputerMonitoring, cancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            Task.Factory.StartNew(ActivePortMonitoring, cancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            Task.Factory.StartNew(NetworkStatusMonitoring, cancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            Task.Factory.StartNew(SwitchMonitoring, cancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            Task.Factory.StartNew(SwitchRefreshMonitoring, cancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            if (IsSwitchPingEnabled)
            {
                StartSwitchPingMonitoring();
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
                cancellation.Cancel();

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

            string isSwitchPingEnabled = ConfigurationManager.AppSettings.Get("switch_ping");
            IsSwitchPingEnabled = isSwitchPingEnabled.ToUpper() == "TRUE";

            string name = ConfigurationManager.AppSettings.Get("switch_username");

            NameValueCollection switchList = (NameValueCollection)ConfigurationManager.GetSection("switchList");
            NameValueCollection deviceList = (NameValueCollection)ConfigurationManager.GetSection("deviceList");
            NameValueCollection connectionList = (NameValueCollection)ConfigurationManager.GetSection("connectionList");

            SwitchDeviceList = switchList == null ? new List<SwitchDeviceModelView>() : switchList.AllKeys.Select(item => new SwitchDeviceModelView(item, switchList[item])).ToList();
            LanHostList = deviceList == null ? new List<LanHostModelView>() : deviceList.AllKeys.Select(item => new LanHostModelView(item, deviceList[item])).ToList();
            ConnectionList = connectionList == null ? new List<SwitchConnectonModelView>() : connectionList.AllKeys.Select(item =>
            {
                string[] values = connectionList[item] == null ? new string[] { null, null } : connectionList[item].Split(';');
                if (values.Length < 2)
                {
                    values = new string[] { null, null };
                }
                string nameA = null, portA = null, nameB = null, portB = null;
                if (values != null && values[0].Contains("|"))
                {
                    nameA = values[0].Split('|')[0].Trim();
                    portA = values[0].Split('|')[1].Trim();
                }
                else
                {
                    nameA = values[0].Trim();
                }
                if (values != null && values[1].Contains("|"))
                {
                    nameB = values[1].Split('|')[0].Trim();
                    portB = values[1].Split('|')[1].Trim();
                }
                else
                {
                    nameB = values[1];
                }
                SwitchDeviceModelView deviceA = nameA == null ? null : SwitchDeviceList.FirstOrDefault(device => device.Name == nameA);
                SwitchDeviceModelView deviceB = nameB == null ? null : SwitchDeviceList.FirstOrDefault(device => device.Name == nameB);
                return new SwitchConnectonModelView(item, deviceA, deviceB, deviceA == null ? 0 : SwitchDeviceList.IndexOf(deviceA), deviceB == null ? 0 : SwitchDeviceList.IndexOf(deviceB), portA, portB);
            }).ToList();

            LoadOverdriveConnectionList();

            SnmpHelper.Initialize(name);
        }

        private void LoadOverdriveConnectionList()
        {
            OverrideConnectionList = new List<OverrideConnection>();
            string filename = "conn-override.conf";
            try
            {
                if (!File.Exists(filename))
                {
                    return;
                }

                using (StreamReader sr = new StreamReader(filename, Encoding.UTF8))
                {
                    while (!sr.EndOfStream)
                    {
                        string text = sr.ReadLine();
                        string[] options = text.Split(';');
                        if (options.Length >= 5)
                        {
                            OverrideConnectionList.Add(new OverrideConnection()
                            {
                                Switch = options[0],
                                HostIP = options[1],
                                HostMacAddress = options[2],
                                State = options[3],
                                IsForced = options[4].ToUpper() == "TRUE",
                            });
                        }
                    }
                }
            }
            catch
            {
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
                List<ActivePort> portList = portMonitor.ListActivePort().Take(256).ToList();

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

        private void SwitchRefreshMonitoring()
        {
            if (!IsSwitchEnabled)
            {
                return;
            }

            bool isRefreshExpiredAlert = false;

            while (true)
            {
                if (RefreshStopwatch.IsRunning && RefreshStopwatch.ElapsedMilliseconds - LastSwitchRefreshTimeStamp > 20000)
                {
                    if (!isRefreshExpiredAlert)
                    {
                        string message = AppResource.GetString(AppResource.StringKey.Message_SNMPWarning);
                        AddToast(AppResource.GetString(AppResource.StringKey.Message_Title), message);
                        LogHelper.WriteLog("WARNING", message);
                        isRefreshExpiredAlert = true;
                    }
                }
                else
                {
                    isRefreshExpiredAlert = false;
                }

                Thread.Sleep(1000);
            }
        }

        private void StartSwitchPingMonitoring()
        {
            List<string> ipList = new List<string>();

            ipList.AddRange(SwitchDeviceList.Select(item => item.Address));
            LanHostList.ForEach(item => ipList.AddRange(item.AdapterList.Select(adapter => adapter.IPAddress)));
            ipList = ipList.Distinct().ToList();

            SwitchDeviceList.ForEach(switchDevice =>
            {
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        foreach (string targetIP in ipList)
                        {
                            if (targetIP == switchDevice.Address)
                            {
                                continue;
                            }

                            try
                            {
                                SnmpHelper.SendPing(switchDevice.EndPoint, targetIP);
                            }
                            catch
                            {
                                continue;
                            }

                            uint result = 0;
                            int waitCount = 0;
                            while (true)
                            {
                                if (waitCount > 10)
                                {
                                    break;
                                }
                                try
                                {
                                    result = SnmpHelper.FetchUIntData(switchDevice.EndPoint, SnmpHelper.DISMAN_PING.OID_pingResultsMaxRtt).Values.First();
                                    break;
                                }
                                catch (InvalidOperationException)
                                {
                                    waitCount += 1;
                                    Thread.Sleep(1000);
                                }
                                catch (Exception)
                                {
                                    break;
                                }
                            }
                        }
                        Thread.Sleep(1000);
                    }
                }, cancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            });
        }

        private void SwitchMonitoring()
        {
            if (!IsSwitchEnabled)
            {
                return;
            }

            RefreshStopwatch.Start();

            SwitchDeviceList.ForEach(switchDevice =>
            {
                Task.Factory.StartNew(() =>
                {
                    int errorLimit = 3;
                    int errorCount = 0;

                    while (true)
                    {
                        try
                        {
                            {
                                if (switchDevice.Information == null)
                                {
                                    var dict0 = SnmpHelper.FetchStringData(switchDevice.EndPoint, SnmpHelper.OIDString.OID_sysDescr);
                                    switchDevice.Information = dict0?.FirstOrDefault().Value;
                                }
                                var dict1 = SnmpHelper.FetchTimeSpanData(switchDevice.EndPoint, SnmpHelper.OIDString.OID_sysUpTime);
                                var dict2 = SnmpHelper.FetchBytesData(switchDevice.EndPoint, SnmpHelper.OIDString.OID_hwStackSystemMac);
                                TimeSpan upTime = dict1 == null ? new TimeSpan() : dict1.FirstOrDefault().Value;
                                switchDevice.UpTime = upTime.TotalMilliseconds == 0 ? AppResource.GetString(AppResource.StringKey.Unknown) : string.Format(AppResource.GetString(AppResource.StringKey.TimeSpan), upTime.Days, upTime.Hours, upTime.Minutes, upTime.Seconds);
                                switchDevice.MACAddress = BitConverter.ToString(dict2.FirstOrDefault().Value, 2).Replace("-", ":");
                            }

                            {
                                var dict0 = SnmpHelper.FetchIntData(switchDevice.EndPoint, SnmpHelper.OIDString.OID_ifIndex);
                                var dict1 = SnmpHelper.FetchIntData(switchDevice.EndPoint, SnmpHelper.OIDString.OID_ifType);
                                var dict2 = SnmpHelper.FetchStringData(switchDevice.EndPoint, SnmpHelper.OIDString.OID_ifDescr);
                                var dict3 = SnmpHelper.FetchIntData(switchDevice.EndPoint, SnmpHelper.OIDString.OID_ifOperStatus);
                                var dict4 = SnmpHelper.FetchCounterData(switchDevice.EndPoint, SnmpHelper.OIDString.OID_ifInOctets);
                                var dict5 = SnmpHelper.FetchCounterData(switchDevice.EndPoint, SnmpHelper.OIDString.OID_ifOutOctets);
                                List<SwitchPort> list = new List<SwitchPort>();

                                var duration = RefreshStopwatch.ElapsedMilliseconds - LastSwitchRefreshTimeStamp;

                                if (dict0 != null && dict1 != null && dict2 != null && dict3 != null)
                                {
                                    for (int i = 0; i < dict0.Count; i += 1)
                                    {
                                        if (dict1.Count > i && dict2.Count > i && dict3.Count > i)
                                        {
                                            if (dict1.ElementAt(i).Value == 6)
                                            {
                                                string text = dict2.ElementAt(i).Value;
                                                long inCount = 0;
                                                long outCount = 0;

                                                if (dict4 != null && dict5 != null && dict4.Count > i && dict5.Count > i)
                                                {
                                                    inCount = dict4.ElementAt(i).Value;
                                                    outCount = dict5.ElementAt(i).Value;
                                                }

                                                SwitchPort port = new SwitchPort
                                                {
                                                    Index = dict0.ElementAt(i).Value,
                                                    Name = text.Split('/').Last(),
                                                    Brief = text,
                                                    IsUp = dict3.ElementAt(i).Value == 1,
                                                    InCount = inCount,
                                                    OutCount = outCount,
                                                    RefreshDelay = duration
                                                };
                                                list.Add(port);
                                            }
                                        }
                                    }

                                    #region Adjust Switch Port Arrangement
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
                                    #endregion

                                }

                                switchDevice.RefreshPortList(list);
                            }

                            {
                                var dict3 = SnmpHelper.FetchBytesData(switchDevice.EndPoint, SnmpHelper.OIDString.OID_hwArpDynMacAdd);
                                var dict4 = SnmpHelper.FetchStringData(switchDevice.EndPoint, SnmpHelper.OIDString.OID_hwArpDynOutIfIndex);
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

                                var dict6 = SnmpHelper.FetchBytesData(switchDevice.EndPoint, SnmpHelper.OIDString.OID_dot1dTpFdbAddress);
                                var dict7 = SnmpHelper.FetchStringData(switchDevice.EndPoint, SnmpHelper.OIDString.OID_dot1dTpFdbPort);
                                var dict8 = new Dictionary<string, string>();

                                if (dict6 != null && dict7 != null)
                                {
                                    for (int i = 0; i < dict6.Count; i += 1)
                                    {
                                        string mac = BitConverter.ToString(dict6.ElementAt(i).Value, 2);
                                        if (dict7.Count > i && !dict8.ContainsKey(mac))
                                        {
                                            dict8.Add(mac, dict7.ElementAt(i).Value);
                                        }
                                    }
                                }

                                var dict0 = SnmpHelper.FetchBytesData(switchDevice.EndPoint, SnmpHelper.OIDString.OID_ipNetToMediaPhysAddress);
                                var dict1 = SnmpHelper.FetchStringData(switchDevice.EndPoint, SnmpHelper.OIDString.OID_ipNetToMediaNetAddress);
                                var dict2 = SnmpHelper.FetchIntData(switchDevice.EndPoint, SnmpHelper.OIDString.OID_ipNetToMediaType);

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
                                                State = (HostState)dict2.ElementAt(i).Value,
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

                                            if (dict8.ContainsKey(mac))
                                            {
                                                dict8.Remove(mac);
                                            }

                                            list.Add(host);
                                        }
                                    }
                                }

                                for (int i = 0; i < dict8.Count; i += 1)
                                {
                                    string mac = dict8.ElementAt(i).Key;
                                    var port = switchDevice.PortList.FirstOrDefault(item => item.Name == dict8.ElementAt(i).Value);
                                    var index = port == null ? 0 : port.Index;

                                    SwitchHost host = new SwitchHost
                                    {
                                        MACAddress = mac.Replace('-', ':'),
                                        IPAddress = AppResource.GetString(AppResource.StringKey.UnknownIPAddress),
                                        State = HostState.Invalid,
                                        PortIndex = index,
                                        Port = port
                                    };

                                    if (index != 0)
                                    {
                                        var find = list.Find(item => item.PortIndex == index);
                                        if (find != null)
                                        {
                                            find.IsCascade = true;
                                            host.IsCascade = true;
                                        }
                                    }

                                    list.Add(host);
                                }

                                switchDevice.RefreshHostList(list);
                            }

                            errorCount = 0;
                            if (switchDevice.State == DeviceState.Offline)
                            {
                                string message = string.Format(AppResource.GetString(AppResource.StringKey.Message_SwitchReconnect), switchDevice.Address, switchDevice.Name);
                                AddToast(AppResource.GetString(AppResource.StringKey.Message_Title), message, ToastMessage.ToastType.Info);
                                LogHelper.WriteLog("INFO", message);
                            }
                            switchDevice.State = DeviceState.Online;

                        }
                        catch
                        {
                            errorCount += 1;
                            if (errorCount > errorLimit)
                            {
                                if (switchDevice.State == DeviceState.Online)
                                {
                                    string message = string.Format(AppResource.GetString(AppResource.StringKey.Message_SwitchDisconnect), switchDevice.Address, switchDevice.Name);
                                    AddToast(AppResource.GetString(AppResource.StringKey.Message_Title), message);
                                    LogHelper.WriteLog("WARNING", message);
                                }
                                switchDevice.SetIdle();
                            }
                        }

                        switchDevice.Refresh();

                        Thread.Sleep(1000);
                    }
                }, cancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            });

            while (true)
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
                        bool isReserved = false;
                        if (list[i].MACAddress == null)
                        {
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
                        }
                        else
                        {
                            foreach (SwitchDeviceModelView device in SwitchDeviceList)
                            {
                                switchHost = device.HostList?.FirstOrDefault(item => item.IPAddress == list[i].IPAddress && item.MACAddress == list[i].MACAddress && !item.IsCascade);
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
                                switchIndex = 0;
                                foreach (SwitchDeviceModelView device in SwitchDeviceList)
                                {
                                    switchHost = device.HostList?.FirstOrDefault(item => item.State == HostState.Invalid && item.MACAddress == list[i].MACAddress && !item.IsCascade);
                                    if (switchHost != null)
                                    {
                                        isReserved = true;
                                        switchIP = device.Address;
                                        switchParent = device;
                                        break;
                                    }
                                    switchIndex += 1;
                                }
                            }

                            if (switchIP == null)
                            {
                                switchIndex = 0;
                                foreach (SwitchDeviceModelView device in SwitchDeviceList)
                                {
                                    switchHost = device.HostList?.FirstOrDefault(item => item.IPAddress == list[i].IPAddress && item.MACAddress == list[i].MACAddress && item.IsCascade);
                                    if (switchHost != null)
                                    {
                                        var secondList = device.HostList?.Where(item => item.PortIndex == switchHost.PortIndex && item.IsCascade).Select(item => item.MACAddress).ToList();
                                        if (secondList.Count >= 2)
                                        {
                                            list.ForEach(item =>
                                            {
                                                if (item.MACAddress != null && secondList.Contains(item.MACAddress))
                                                {
                                                    secondList.Remove(item.MACAddress);
                                                }
                                            });
                                            if (secondList.Count == 0)
                                            {
                                                switchIP = device.Address;
                                                switchParent = device;
                                                break;
                                            }
                                        }
                                    }
                                    switchIndex += 1;
                                }
                            }
                        }

                        #region Override Connection

                        foreach (OverrideConnection conn in OverrideConnectionList)
                        {
                            bool flag = false;
                            if (string.IsNullOrEmpty(conn.HostMacAddress))
                            {
                                flag = conn.HostIP == list[i].IPAddress;
                            }
                            else if (string.IsNullOrEmpty(conn.HostIP))
                            {
                                flag = conn.HostMacAddress == list[i].MACAddress;
                            }
                            else
                            {
                                flag = conn.HostIP == list[i].IPAddress && conn.HostMacAddress == list[i].MACAddress;
                            }
                            if (flag)
                            {
                                if (switchIP == null || conn.IsForced)
                                {
                                    SwitchDeviceModelView device = SwitchDeviceList.FirstOrDefault(item => item.Name == conn.Switch);
                                    if (device != null)
                                    {
                                        if (conn.State == "0")
                                        {
                                            switchIP = null;
                                            switchParent = null;
                                        }
                                        else
                                        {
                                            switchIP = device.Address;
                                            switchParent = device;
                                            switchIndex = SwitchDeviceList.IndexOf(device);
                                            isReserved = conn.State == "1";
                                        }
                                        break;
                                    }
                                }
                            }
                        }

                        #endregion

                        if (switchIP == null)
                        {
                            if (list[i].State == DeviceState.Online || list[i].State == DeviceState.Reserve)
                            {
                                if (list[i].SwitchDevice.State == DeviceState.Offline)
                                {
                                    list[i].State = DeviceState.Unknown;
                                }
                                else
                                {
                                    list[i].State = DeviceState.Offline;
                                    string message = string.Format(AppResource.GetString(AppResource.StringKey.Message_HostDisconnect), host.Name, list[i].IPAddress);
                                    AddToast(AppResource.GetString(AppResource.StringKey.Message_Title), message);
                                    LogHelper.WriteLog("WARNING", message);
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
                                string message = string.Format(AppResource.GetString(AppResource.StringKey.Message_HostReconnect), host.Name, list[i].IPAddress, switchParent.Name);
                                AddToast(AppResource.GetString(AppResource.StringKey.Message_Title), message, ToastMessage.ToastType.Info);
                                LogHelper.WriteLog("INFO", message);
                            }
                            list[i].SwitchIPAddress = switchIP;
                            list[i].SwitchDevice = switchParent;
                            list[i].Host = switchHost;
                            list[i].RefreshVector(i, list.Count, switchIndex, SwitchDeviceList.Count);
                            list[i].State = isReserved ? DeviceState.Reserve : DeviceState.Online;
                        }

                        list[i].Refresh();
                    }
                }

                foreach (SwitchConnectonModelView connection in ConnectionList)
                {
                    if (connection.IsHidden)
                    {
                        continue;
                    }

                    if (connection.DeviceA.State != DeviceState.Online || connection.DeviceB.State != DeviceState.Online)
                    {
                        connection.State = DeviceState.Unknown;
                        connection.HostA = null;
                        connection.HostB = null;
                        connection.Refresh();
                        continue;
                    }

                    var hostA = connection.DeviceA.HostList?.FirstOrDefault(item => item.MACAddress == connection.DeviceB.MACAddress);
                    var hostB = connection.DeviceB.HostList?.FirstOrDefault(item => item.MACAddress == connection.DeviceA.MACAddress);
                    if (connection.PortA != null)
                    {
                        var port = connection.DeviceA.PortList.FirstOrDefault(item => item.Name == connection.PortA);
                        if (port == null || !port.IsUp)
                        {
                            hostA = null;
                        }
                    }
                    if (connection.PortB != null)
                    {
                        var port = connection.DeviceB.PortList.FirstOrDefault(item => item.Name == connection.PortB);
                        if (port == null || !port.IsUp)
                        {
                            hostB = null;
                        }
                    }
                    if (hostA == null || hostB == null)
                    {
                        if (connection.State == DeviceState.Online)
                        {
                            string message = string.Format(AppResource.GetString(AppResource.StringKey.Message_LineDisconnect), connection.DeviceA.Name, connection.DeviceB.Name);
                            AddToast(AppResource.GetString(AppResource.StringKey.Message_Title), message);
                            LogHelper.WriteLog("WARNING", message);
                        }
                        if (connection.State != DeviceState.Unknown)
                        {
                            connection.State = DeviceState.Offline;
                        }
                    }
                    else
                    {
                        if (connection.State == DeviceState.Offline)
                        {
                            string message = string.Format(AppResource.GetString(AppResource.StringKey.Message_LineReconnect), connection.DeviceA.Name, connection.DeviceB.Name);
                            AddToast(AppResource.GetString(AppResource.StringKey.Message_Title), message, ToastMessage.ToastType.Info);
                            LogHelper.WriteLog("INFO", message);
                        }
                        connection.State = DeviceState.Online;
                    }

                    connection.HostA = hostA;
                    connection.HostB = hostB;

                    connection.Refresh();
                }

                Notify(new { SwitchPortCount, SwitchHostCount, LanHostCount });

                LastSwitchRefreshTimeStamp = RefreshStopwatch.ElapsedMilliseconds;

                RefreshTopology();

                Thread.Sleep(1000);
            }
        }

        public class TopologyInfo
        {
            public object Node;
            public List<object> Neighbours;
            public double Width => 96;
            public double Height => 64;
            public double Left => X - 48;
            public double Top => Y - 32;
            public bool IsSwitch { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public double W { get; set; }
            public double Angle { get; set; }
            public string Name { get; set; }
        }
        public List<TopologyInfo> TopologyDotList { get; set; }
        public List<TopologyInfo> TopologyLineList { get; set; }
        public double TopologyWidth { get; set; }
        public double TopologyHeight { get; set; }
        private void RefreshTopology()
        {
            /*
             * 
                public List<SwitchDeviceModelView> SwitchDeviceList { get; set; }
                public List<LanHostModelView> LanHostList { get; set; }
                public List<SwitchConnectonModelView> ConnectionList { get; set; }
             *
             */
            if (SwitchDeviceList.Count == 0)
            {
                return;
            }

            int rootCount = SwitchDeviceList.Count;
            const double ratio = 2;
            const double size = 32;
            double height = size * 10 + size * rootCount;
            double width = height * ratio;
            double ox = width / 2;
            double oy = height / 2;

            var dots0 = new List<TopologyInfo>();
            var dots1 = new List<TopologyInfo>();

            dots0 = SwitchDeviceList.Select(item => new TopologyInfo() { Node = item, Name = item.Name, X = 0, Y = 0, Angle = 0, IsSwitch = true }).ToList();
            dots1 = LanHostList.Select(item => new TopologyInfo() { Node = item, Name = item.Name, X = 0, Y = 0, Angle = 0, Neighbours = item.AdapterList.Select(_item => (object)_item.SwitchDevice).ToList() }).ToList();

            {
                int count = dots0.Count;
                double d0 = count == 0 ? 0 : Math.PI - (count % 2 == 0 && count > 2 ? Math.PI / count : 0);
                double r0 = size * count * 0.6;
                int i = 0;
                foreach (var dot in dots0)
                {
                    double d = (i * Math.PI * 2) / count + d0;
                    dot.Angle = d - d0;
                    dot.X = ox + r0 * Math.Cos(d) * ratio;
                    dot.Y = oy - r0 * Math.Sin(d);
                    i++;
                }
            }

            {
                foreach (var dot in dots1)
                {
                    double d = 0;
                    if (dot.Neighbours.Count == 0)
                    {
                        d = 0;
                    }
                    else if (dot.Neighbours.Count == 1)
                    {
                        d = dots0.FirstOrDefault(item => item.Node == dot.Neighbours[0]).Angle;
                    }
                    else if (dot.Neighbours.Count == 2)
                    {
                        double d0 = dots0.FirstOrDefault(item => item.Node == dot.Neighbours[0]).Angle;
                        double d1 = dots0.FirstOrDefault(item => item.Node == dot.Neighbours[1]).Angle;
                        if (d1 - d0 > Math.PI)
                        {
                            d = (d0 + d1) / 2 + Math.PI;
                        }
                        else
                        {
                            d = (d0 + d1) / 2;
                        }
                    }
                    else
                    {
                        d = dot.Neighbours.Sum(item => dots0.FirstOrDefault(_item => _item.Node == item).Angle) / dot.Neighbours.Count;
                    }
                    dot.Angle = d;
                }
                dots1 = dots1.OrderBy(item => item.Angle).ToList();
            }

            {
                int i = 0;
                int count = dots1.Count;
                double d0 = count == 0 ? 0 : Math.PI - (dots0.Count % 2 == 0 ? Math.PI / dots0.Count : 0);
                double r0 = size * 3 + size * dots0.Count * 0.75;
                foreach (var dot in dots1)
                {
                    double d = (i * Math.PI * 2 / count) + d0 - Math.PI / count;
                    dot.Angle = d;
                    dot.X = ox + r0 * Math.Cos(d) * ratio;
                    dot.Y = oy - r0 * Math.Sin(d);
                    i++;
                }
            }

            var lineList = new List<TopologyInfo>();

            {
                foreach (var connection in ConnectionList)
                {
                    var dotA = dots0.FirstOrDefault(item => item.Node == connection.DeviceA);
                    var dotB = dots0.FirstOrDefault(item => item.Node == connection.DeviceB);
                    lineList.Add(new TopologyInfo() { Node = connection, IsSwitch = true, X = dotA.X, Y = dotA.Y, Z = dotB.X, W = dotB.Y });
                }
            }

            {
                foreach (var dot in dots1)
                {
                    foreach (var target in dot.Neighbours)
                    {
                        var dotA = dot;
                        var dotB = dots0.FirstOrDefault(item => item.Node == target);
                        lineList.Add(new TopologyInfo() { Node = ((LanHostModelView)dot.Node).AdapterList.FirstOrDefault(item => item.SwitchDevice == target), X = dotA.X, Y = dotA.Y, Z = dotB.X, W = dotB.Y });
                    }
                }
            }

            TopologyWidth = width;
            TopologyHeight = height;
            TopologyDotList = dots0.Concat(dots1).ToList();
            TopologyLineList = lineList;

            Notify(new { TopologyWidth, TopologyHeight, TopologyDotList, TopologyLineList });
        }

        public ObservableCollection<ToastMessage> ToastCollection { get; set; } = new ObservableCollection<ToastMessage>();

        public void AddToast(string title, string message, ToastMessage.ToastType toastType = ToastMessage.ToastType.Alert)
        {
            Dispatcher dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
            if (dispatcher == null)
            {
                dispatcher = Application.Current.Dispatcher;
            }
            dispatcher?.Invoke(() =>
            {
                while (ToastCollection.Count > 4)
                {
                    ToastCollection.RemoveAt(0);
                }
                var toast = new ToastMessage(title, message)
                {
                    Type = toastType
                };
                ToastCollection.Add(toast);
                if (toastType == ToastMessage.ToastType.Alert)
                {
                    SystemSounds.Exclamation.Play();
                }
                else
                {
                    SystemSounds.Beep.Play();
                }
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
        public enum ToastType
        {
            Alert,
            Info
        }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public ToastType Type { get; set; } = ToastType.Alert;
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

            return computerList.ToList();
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

        public List<NetworkAdapter> Refresh()
        {
            var list = adapterList.ToList();
            foreach (NetworkAdapter adapter in list)
            {
                adapter.Refresh();
            }
            return list;
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
