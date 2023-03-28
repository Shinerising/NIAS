using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Media;
using static LanMonitor.NetworkManager;

namespace LanMonitor
{
    public interface IHoverable
    {
        bool IsHover { get; }
        void SetHover(bool flag);
    }
    /// <summary>
    /// ModelView for Switch Port
    /// </summary>
    public class SwitchPort : CustomINotifyPropertyChanged, IHoverable
    {
        /// <summary>
        /// Index of the port
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Name of the port
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Brief of the port
        /// </summary>
        public string Brief { get; set; }
        /// <summary>
        /// Is the port up
        /// </summary>
        public bool IsUp { get; set; }
        /// <summary>
        /// Is the port a fiber port
        /// </summary>
        public bool IsFiber { get; set; }
        /// <summary>
        /// Bytes count in
        /// </summary>
        public long InCount { get; set; }
        /// <summary>
        /// Bytes count out
        /// </summary>
        public long OutCount { get; set; }
        /// <summary>
        /// Refresh delay
        /// </summary>
        public long RefreshDelay { get; set; } = 1;
        /// <summary>
        /// In rate
        /// </summary>
        public long InRate { get; set; }
        /// <summary>
        /// Out rate
        /// </summary>
        public long OutRate { get; set; }
        /// <summary>
        /// In speed
        /// </summary>
        public string InSpeed { get; set; } = "0B/s";
        /// <summary>
        /// Out speed
        /// </summary>
        public string OutSpeed { get; set; } = "0B/s";
        /// <summary>
        /// Is the port hovered
        /// </summary>
        public bool IsHover { get; set; }
        /// <summary>
        /// Tooltip of the port
        /// </summary>
        public string Tip => string.Format(AppResource.GetString(AppResource.StringKey.Tip_SwitchPort), Environment.NewLine, Name, Brief, IsUp ? AppResource.GetString(AppResource.StringKey.Connected) : AppResource.GetString(AppResource.StringKey.Disconnected), InSpeed, OutSpeed);

        public List<long> InRateList { get; set; } = Enumerable.Repeat<long>(0, 30).ToList();
        public List<long> OutRateList { get; set; } = Enumerable.Repeat<long>(0, 30).ToList();
        public long MaxRate => Math.Max(InRateList.Max(), OutRateList.Max());
        public StreamGeometry InRateGeometry => GetChart(InRateList, Math.Max(1000, MaxRate));
        public StreamGeometry OutRateGeometry => GetChart(OutRateList, Math.Max(1000, MaxRate));
        private static StreamGeometry GetChart(IEnumerable<long> valueList, double maxValue)
        {
            double max = maxValue;
            double min = 0;
            double range = maxValue;

            var geometry = new StreamGeometry();
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
                context.LineTo(new Point(index - 1, range), true, true);
                context.LineTo(new Point(index - 1, max), false, false);
                context.LineTo(new Point(index - 1, min), false, false);
                context.LineTo(new Point(index - 1, range), false, false);
                context.LineTo(new Point(0, range), true, true);
                context.LineTo(new Point(0, max), false, false);
                context.LineTo(new Point(0, min), false, false);
                context.LineTo(new Point(0, range), false, false);
            }
            geometry.Freeze();
            return geometry;
        }

        public void Refresh(SwitchPort port)
        {
            Index = port.Index;
            Name = port.Name;
            Brief = port.Brief;
            IsUp = port.IsUp;
            IsFiber = port.IsFiber;

            if (port.InCount >= InCount)
            {
                InRate = (port.InCount - InCount) * 1000 / port.RefreshDelay;
                InSpeed = NetworkAdapter.GetSpeedString(InRate);
            }
            else
            {
                InRate = 0;
                InSpeed = "0B/s";
            }
            InCount = port.InCount;

            if (port.OutCount >= OutCount)
            {
                OutRate = (port.OutCount - OutCount) * 1000 / port.RefreshDelay;
                OutSpeed = NetworkAdapter.GetSpeedString(OutRate);
            }
            else
            {
                OutRate = 0;
                OutSpeed = "0B/s";
            }
            OutCount = port.OutCount;

            InRateList.RemoveAt(0);
            InRateList.Add(InRate);
            OutRateList.RemoveAt(0);
            OutRateList.Add(OutRate);

            Notify(new { Index, Name, Brief, IsUp, IsFiber, InSpeed, OutSpeed, Tip, InRateGeometry, OutRateGeometry });
        }
        public void SetHover(bool flag)
        {
            IsHover = flag;
            Notify(new { IsHover });
        }
    }
    /// <summary>
    /// Host in switch record
    /// </summary>
    public class SwitchHost : CustomINotifyPropertyChanged, IHoverable
    {
        /// <summary>
        /// Hostname of the network device
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// IP address
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// MAC address
        /// </summary>
        public string MACAddress { get; set; }
        /// <summary>
        /// MAC vendor
        /// </summary>
        public string MACVendor => ManuHelper.Instance.FindInfo(MACAddress)?.Organization ?? AppResource.GetString(AppResource.StringKey.Unknown);
        /// <summary>
        /// Port index of the host
        /// </summary>
        public int PortIndex { get; set; }
        /// <summary>
        /// Port of the host
        /// </summary>
        public SwitchPort Port { get; set; }
        /// <summary>
        /// Is the host a cascade device
        /// </summary>
        public bool IsCascade { get; set; }
        /// <summary>
        /// State of the host
        /// </summary>
        public HostState State { get; set; }
        /// <summary>
        /// Tooltip of the host
        /// </summary>
        public string Tip => string.Format(AppResource.GetString(AppResource.StringKey.Tip_SwitchHost), Environment.NewLine, IPAddress, MACAddress, MACVendor, Port == null ? AppResource.GetString(AppResource.StringKey.Unknown) : Port.Name, State == HostState.Dynamic ? AppResource.GetString(AppResource.StringKey.Dynamic) : (State == HostState.Static ? AppResource.GetString(AppResource.StringKey.Static) : (State == HostState.Invalid ? AppResource.GetString(AppResource.StringKey.Invalid) : AppResource.GetString(AppResource.StringKey.Other))));
        /// <summary>
        /// Is the host hovered
        /// </summary>
        public bool IsHover { get; set; }
        public void SetHover(bool flag)
        {
            IsHover = flag;
            Port?.SetHover(flag);
            Notify(new { IsHover });
        }
        public void Refresh(SwitchHost host)
        {
            HostName = host.HostName;
            IPAddress = host.IPAddress;
            MACAddress = host.MACAddress;
            PortIndex = host.PortIndex;
            Port = host.Port;
            IsCascade = host.IsCascade;
            State = host.State;

            Notify(new { HostName, IPAddress, MACAddress, Port, PortIndex, IsCascade, State, Tip });
        }
    }
    /// <summary>
    /// Adapter of the host in LAN
    /// </summary>
    public class LanHostAdapter : CustomINotifyPropertyChanged, IHoverable
    {
        public struct LineVector
        {
            public double Left { get; set; }
            public double Top { get; set; }
            public double Length { get; set; }
            public double Offset { get; set; }
            public override bool Equals([NotNullWhen(true)] object obj)
            {
                if (obj is not LineVector)
                {
                    return false;
                }
                var other = (LineVector)obj;
                return Left == other.Left && Top == other.Top && Length == other.Length && Offset == other.Offset;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public static bool operator ==(LineVector left, LineVector right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(LineVector left, LineVector right)
            {
                return !(left == right);
            }
        }
        public void RefreshVector(int adapterIndex, int adapterCount, int switchIndex, int switchCount)
        {
            LineVector vector;
            if (adapterIndex < 0 || adapterCount < 0 || switchIndex < 0 || switchCount < 0)
            {
                vector = new LineVector();
            }
            else
            {
                const double width = 72;
                const double height0 = 92;
                const double height1 = 38;

                vector = new LineVector()
                {
                    Left = width / -2 + width / adapterCount * (adapterIndex + 0.5),
                    Top = 6 + height1 * adapterIndex,
                    Length = height0 * (switchIndex - switchCount) - height1 * adapterIndex + 47,
                    Offset = -40 - height1 * adapterIndex
                };
            }

            if (!Vector.Equals(vector))
            {
                Vector = vector;
                Notify(new { Vector });
            }
        }
        /// <summary>
        /// ID of the adapter
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Host of the adapter
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// Host of the adapter
        /// </summary>
        public string MACAddress { get; set; }
        /// <summary>
        /// Mac vendor of the adapter
        /// </summary>
        public string MACVendor => ManuHelper.Instance.FindInfo(MACAddress)?.Organization ?? AppResource.GetString(AppResource.StringKey.Unknown);
        /// <summary>
        /// Switch IP Address
        /// </summary>
        public string SwitchIPAddress { get; set; }
        public LineVector Vector { get; set; }
        /// <summary>
        /// State of the adapter
        /// </summary>
        public DeviceState State { get; set; }
        /// <summary>
        /// Tooltip of the adapter
        /// </summary>
        public string Tip => string.Format(AppResource.GetString(AppResource.StringKey.Tip_Adapter), Environment.NewLine, IPAddress, MACAddress ?? (Host == null ? AppResource.GetString(AppResource.StringKey.Unknown) : Host.MACAddress), MACVendor, SwitchDevice == null ? AppResource.GetString(AppResource.StringKey.Unknown) : SwitchDevice.Name, Host == null || Host.Port == null ? AppResource.GetString(AppResource.StringKey.Unknown) : Host.Port.Name, State == DeviceState.Online ? AppResource.GetString(AppResource.StringKey.Connected) : (State == DeviceState.Offline ? AppResource.GetString(AppResource.StringKey.Disconnected) : (State == DeviceState.Reserve ? AppResource.GetString(AppResource.StringKey.Reserve) : AppResource.GetString(AppResource.StringKey.Unknown))));
        /// <summary>
        /// Host of the adapter
        /// </summary>
        public SwitchHost Host { get; set; }
        /// <summary>
        /// Switch device of the adapter
        /// </summary>
        public SwitchDeviceModelView SwitchDevice { get; set; }
        /// <summary>
        /// Latency of the adapter
        /// </summary>
        public int Latency { get; set; }
        /// <summary>
        /// Average in rate of the adapter
        /// </summary>
        public long? AverageInRate => Host?.Port?.InRateList.Skip(25).Sum() / 5;
        /// <summary>
        /// Average out rate of the adapter
        /// </summary>
        public long? AverageOutRate => Host?.Port?.OutRateList.Skip(25).Sum() / 5;
        /// <summary>
        /// Is the adapter alert
        /// </summary>
        public bool IsAlert => AverageInRate > 50000000 || AverageOutRate > 50000000 || Latency > 50;
        /// <summary>
        /// Alert text of the adapter
        /// </summary>
        public string AlertText => string.Join('\n', new string[] { AverageInRate > 50000000 ? "传入流量异常" : "", AverageOutRate > 50000000 ? "传出流量异常" : "", Latency > 50 ? "网络延迟异常" : "" }.Where(item => !string.IsNullOrEmpty(item)));

        /// <summary>
        /// Is the adapter hover
        /// </summary>
        public bool IsHover { get; set; }
        public void SetHover(bool flag)
        {
            IsHover = flag;
            if (Host != null && Host.Port != null)
            {
                Host.Port.SetHover(flag);
            }
            Notify(new { IsHover });
        }
        public void Refresh()
        {
            Notify(new { State, SwitchIPAddress, SwitchDevice, Host, Tip, IsAlert, AlertText });
        }
        public LanHostAdapter(int id, string ip)
        {
            ID = id;
            IPAddress = ip;
        }
        public LanHostAdapter(int id, string ip, string mac)
        {
            ID = id;
            IPAddress = ip;
            MACAddress = mac.ToUpper();
        }
    }
    /// <summary>
    /// Modelview of the host in LAN
    /// </summary>
    public class LanHostModelView
    {
        /// <summary>
        /// ID of the host
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Name of the host
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Tooltip of the host
        /// </summary>
        public string Tip { get; set; }
        /// <summary>
        /// State of the host
        /// </summary>
        public DeviceState State { get; set; }
        /// <summary>
        /// Adapter list of the host
        /// </summary>
        public List<LanHostAdapter> AdapterList { get; set; }
        /// <summary>
        /// Active count of the host
        /// </summary>
        public string ActiveCount => string.Format("{0}/{1}", AdapterList.Where(item => item.State == DeviceState.Online).Count(), AdapterList.Count);
        public LanHostModelView(int id, string name, string iplist)
        {
            Name = name;
            ID = id;
            AdapterList = iplist == null ? new List<LanHostAdapter>() : iplist.Split(';').Select((item, index) =>
            {
                if (item.Contains('|'))
                {
                    var arr = item.Trim().Split('|');
                    var ip = arr[0];
                    var mac = arr[1];
                    return new LanHostAdapter(index, ip, mac);
                }
                else
                {
                    var ip = item.Trim();
                    return new LanHostAdapter(index, ip);
                }
            }).ToList();
        }
    }
    public enum DeviceState
    {
        Unknown,
        Online,
        Offline,
        Reserve
    }
    public enum HostState
    {
        Other = 1,
        Invalid = 2,
        Dynamic = 3,
        Static = 4,
    }
    /// <summary>
    /// Modelview of the switch connection
    /// </summary>
    public class SwitchConnectonModelView : CustomINotifyPropertyChanged, IHoverable
    {
        public double Top { get; set; }
        public double Bottom { get; set; }
        public string Brief { get; set; }
        public bool IsHidden { get; set; }
        /// <summary>
        /// State of the connection
        /// </summary>
        public DeviceState State { get; set; }
        /// <summary>
        /// Device A of the connection
        /// </summary>
        public SwitchDeviceModelView DeviceA { get; set; }
        /// <summary>
        /// Host A of the connection
        /// </summary>
        public SwitchHost HostA { get; set; }
        /// <summary>
        /// Port A of the connection
        /// </summary>
        public string PortA { get; set; }
        /// <summary>
        /// Device B of the connection
        /// </summary>
        public SwitchDeviceModelView DeviceB { get; set; }
        /// <summary>
        /// Host B of the connection
        /// </summary>
        public SwitchHost HostB { get; set; }
        /// <summary>
        /// Port B of the connection
        /// </summary>
        public string PortB { get; set; }
        /// <summary>
        /// Tooltip of the connection
        /// </summary>
        public string Tip => string.Format(AppResource.GetString(AppResource.StringKey.Tip_SwitchConnection), Environment.NewLine, AppResource.GetString(AppResource.StringKey.SwitchCascading), string.Format("{0} - {1}", DeviceA?.Name, DeviceB?.Name), string.Format("{0} - {1}", PortA ?? (HostA?.Port == null ? AppResource.GetString(AppResource.StringKey.Unknown) : HostA.Port.Name), PortB ?? (HostB?.Port == null ? AppResource.GetString(AppResource.StringKey.Unknown) : HostB.Port.Name)), State == DeviceState.Unknown ? AppResource.GetString(AppResource.StringKey.Unknown) : (State == DeviceState.Online ? AppResource.GetString(AppResource.StringKey.Connected) : AppResource.GetString(AppResource.StringKey.Disconnected)));
        public bool IsHover { get; set; }
        public void SetHover(bool flag)
        {
            IsHover = flag;
            if (HostA != null && HostA.Port != null)
            {
                HostA.Port.SetHover(flag);
            }
            if (HostB != null && HostB.Port != null)
            {
                HostB.Port.SetHover(flag);
            }
            Notify(new { IsHover });
        }
        public SwitchConnectonModelView(string brief, SwitchDeviceModelView deviceA, SwitchDeviceModelView deviceB, int indexA, int indexB, string portA, string portB)
        {
            Brief = brief;
            if (deviceA == null || deviceB == null || indexA == indexB)
            {
                IsHidden = true;
                return;
            }

            double top = 66;
            double height = 92;

            if (indexA < indexB)
            {
                DeviceA = deviceA;
                DeviceB = deviceB;
                PortA = portA;
                PortB = portB;
                Top = indexA * height + top;
                Bottom = indexB * height + top;
            }
            else
            {
                DeviceA = deviceB;
                DeviceB = deviceA;
                PortA = portB;
                PortB = portA;
                Top = indexB * height + top;
                Bottom = indexA * height + top;
            }
        }
        public void Refresh()
        {
            Notify(new { State, Tip });
        }

    }

    /// <summary>
    /// Modelview of the switch device
    /// </summary>
    public class SwitchDeviceModelView : CustomINotifyPropertyChanged
    {
        /// <summary>
        /// ID of the device
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Name of the device
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// IP address of the device
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// End point of the device
        /// </summary>
        public IPEndPoint EndPoint { get; set; }
        /// <summary>
        /// MAC address of the device
        /// </summary>
        public string MACAddress { get; set; }
        /// <summary>
        /// Vendor of the device
        /// </summary>
        public string MACVendor => ManuHelper.Instance.FindInfo(MACAddress)?.Organization ?? AppResource.GetString(AppResource.StringKey.Unknown);
        /// <summary>
        /// Information of the device
        /// </summary>
        public string Information { get; set; }
        /// <summary>
        /// State of the device
        /// </summary>
        public DeviceState State { get; set; } = DeviceState.Unknown;
        public float CpuUsage { get; set; }
        public float MemoryUsage { get; set; }
        public float Temperature { get; set; }
        public string UpTime { get; set; }
        public string Tip => string.Format(AppResource.GetString(AppResource.StringKey.Tip_SwitchDevice), Environment.NewLine, Name, Address, MACAddress, MACVendor, State == DeviceState.Online ? AppResource.GetString(AppResource.StringKey.Online) : (State == DeviceState.Offline ? AppResource.GetString(AppResource.StringKey.Offline) : AppResource.GetString(AppResource.StringKey.Unknown)), UpTime ?? AppResource.GetString(AppResource.StringKey.Unknown), CpuUsage, MemoryUsage, Temperature);
        public List<SwitchPort> PortList { get; set; }
        /// <summary>
        /// List of the hosts of the device
        /// </summary>
        public List<SwitchHost> HostList { get; set; }
        /// <summary>
        /// Count of the ports of the device
        /// </summary>
        public int PortCount => PortList == null ? 0 : PortList.Where(item => item.IsUp).Count();
        /// <summary>
        /// Count of the hosts of the device
        /// </summary>
        public int HostCount => HostList == null ? 0 : HostList.Count;
        public SwitchDeviceModelView(int id, string name, string ip)
        {
            ID = id;
            Name = name;
            Address = ip;
            EndPoint = new IPEndPoint(IPAddress.Parse(ip), 161);
            PortList = new List<SwitchPort>();
            HostList = new List<SwitchHost>();
        }
        public void RefreshPortList(List<SwitchPort> list)
        {
            if (list == null)
            {
                PortList = null;
                Notify(new { PortList, PortCount });
                return;
            }
            if (PortList == null || PortList.Count != list.Count)
            {
                PortList = list;
                Notify(new { PortList, PortCount });
                return;
            }
            for (int i = 0; i < list.Count; i += 1)
            {
                PortList[i].Refresh(list[i]);
            }

            Notify(new { PortCount });
        }
        public void RefreshHostList(List<SwitchHost> list)
        {
            if (list == null)
            {
                HostList = null;
                Notify(new { HostList, HostCount });
                return;
            }
            if (HostList == null || HostList.Count != list.Count)
            {
                HostList = list;
                Notify(new { HostList, HostCount });
                return;
            }
            for (int i = 0; i < list.Count; i += 1)
            {
                HostList[i].Refresh(list[i]);
            }

            Notify(new { HostCount });
        }
        public void SetIdle()
        {
            State = DeviceState.Offline;
            UpTime = AppResource.GetString(AppResource.StringKey.Unknown);
            Information = null;

            RefreshPortList(null);
            RefreshHostList(null);
        }
        public void Refresh()
        {
            Notify(new { State, UpTime, Information, Tip });
        }
        public static SwitchDeviceModelView GetPreviewInstance(string ip)
        {
            SwitchDeviceModelView switchDevice = new(0, "test", ip)
            {
                Address = ip,
                EndPoint = new IPEndPoint(IPAddress.Parse(ip), 161),
                Information = string.Format("HUAWEI S5720{0}HUAWEI S5720{0}HUAWEI S5720", Environment.NewLine),
                PortList = Enumerable.Range(0, 28).Select(item => new SwitchPort()
                {
                    Name = item.ToString(),
                    Brief = "GigabitEthernet1/0/" + item.ToString(),
                    IsUp = item % 3 == 1,
                    IsFiber = item >= 24,
                    InRateList = Enumerable.Repeat<long>(0, 30).Select(item => new Random().NextInt64(100000)).ToList(),
                    OutRateList = Enumerable.Repeat<long>(0, 30).Select(item => new Random().NextInt64(100000)).ToList(),
                }).ToList(),
                HostList = new List<SwitchHost>()
                {
                    new SwitchHost()
                    {
                        IPAddress = "172.16.24.90",
                        MACAddress = "AA-BB-CC-DD-EE-FF"
                    },
                    new SwitchHost()
                    {
                        IPAddress = "172.16.24.91",
                        MACAddress = "AA-BB-CC-DD-EE-FF"
                    },
                    new SwitchHost()
                    {
                        IPAddress = "172.16.24.92",
                        MACAddress = "AA-BB-CC-DD-EE-FF"
                    },
                    new SwitchHost()
                    {
                        IPAddress = "172.16.24.93",
                        MACAddress = "AA-BB-CC-DD-EE-FF"
                    },
                    new SwitchHost()
                    {
                        IPAddress = "172.16.24.95",
                        MACAddress = "AA-BB-CC-DD-EE-FF"
                    },
                    new SwitchHost()
                    {
                        IPAddress = "172.16.24.101",
                        MACAddress = "AA-BB-CC-DD-EE-FF"
                    },
                    new SwitchHost()
                    {
                        IPAddress = "172.16.24.102",
                        MACAddress = "AA-BB-CC-DD-EE-FF"
                    },
                    new SwitchHost()
                    {
                        IPAddress = "172.16.24.103",
                        MACAddress = "AA-BB-CC-DD-EE-FF"
                    }
                }
            };
            return switchDevice;
        }
    }

    public class OverrideConnection
    {
        public string Switch { get; set; }
        public string HostIP { get; set; }
        public string HostMacAddress { get; set; }
        public string State { get; set; }
        public bool IsForced { get; set; }
    }
}
