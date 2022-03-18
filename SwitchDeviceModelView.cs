using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LanMonitor
{
    public interface IHoverable
    {
        bool IsHover { get; }
        void SetHover(bool flag);
    }
    public class SwitchPort : CustomINotifyPropertyChanged, IHoverable
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Brief { get; set; }
        public bool IsUp { get; set; }
        public bool IsFiber { get; set; }
        public long InCount { get; set; }
        public long OutCount { get; set; }
        public long RefreshDelay { get; set; } = 1;
        public string InSpeed { get; set; } = "0B/s";
        public string OutSpeed { get; set; } = "0B/s";
        public bool IsHover { get; set; }
        public string Tip => string.Format(AppResource.GetString(AppResource.StringKey.Tip_SwitchPort), Environment.NewLine, Name, Brief, IsUp ? AppResource.GetString(AppResource.StringKey.Connected) : AppResource.GetString(AppResource.StringKey.Disconnected), InSpeed, OutSpeed);
        public void Refresh(SwitchPort port)
        {
            Index = port.Index;
            Name = port.Name;
            Brief = port.Brief;
            IsUp = port.IsUp;
            IsFiber = port.IsFiber;

            if (port.InCount >= InCount)
            {
                InSpeed = NetworkAdapter.GetSpeedString((port.InCount - InCount) * 1000 / port.RefreshDelay);
            }
            else
            {
                InSpeed = "0B/s";
            }
            InCount = port.InCount;

            if (port.OutCount >= OutCount)
            {
                OutSpeed = NetworkAdapter.GetSpeedString((port.OutCount - OutCount) * 1000 / port.RefreshDelay);
            }
            else
            {
                OutSpeed = "0B/s";
            }
            OutCount = port.OutCount;

            Notify(new { Index, Name, Brief, IsUp, IsFiber, InSpeed, OutSpeed, Tip });
        }
        public void SetHover(bool flag)
        {
            IsHover = flag;
            Notify(new { IsHover });
        }
    }
    public class SwitchHost : CustomINotifyPropertyChanged, IHoverable
    {
        public string HostName { get; set; }
        public string IPAddress { get; set; }
        public string MACAddress { get; set; }
        public int PortIndex { get; set; }
        public SwitchPort Port { get; set; }
        public bool IsCascade { get; set; }
        public HostState State { get; set; }
        public string Tip => string.Format(AppResource.GetString(AppResource.StringKey.Tip_SwitchHost), IPAddress, MACAddress, Port == null ? AppResource.GetString(AppResource.StringKey.Unknown) : Port.Name, State == HostState.Dynamic ? AppResource.GetString(AppResource.StringKey.Dynamic) : (State == HostState.Static ? AppResource.GetString(AppResource.StringKey.Static) : (State == HostState.Invalid ? AppResource.GetString(AppResource.StringKey.Invalid) : AppResource.GetString(AppResource.StringKey.Other))), Environment.NewLine);
        public bool IsHover { get; set; }
        public void SetHover(bool flag)
        {
            IsHover = flag;
            if (Port != null)
            {
                Port.SetHover(flag);
            }
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
    public class LanHostAdapter : CustomINotifyPropertyChanged, IHoverable
    {
        public struct LineVector
        {
            public double Left { get; set; }
            public double Top { get; set; }
            public double Length { get; set; }
        }
        public void RefreshVector(int adapterIndex, int adapterCount, int switchIndex, int switchCount)
        {
            if (adapterIndex < 0 || adapterCount < 0 || switchIndex < 0 || switchCount < 0)
            {
                Vector = new LineVector();
            }
            else
            {
                double width = 72;
                double height0 = 92;
                double height1 = 38;

                Vector = new LineVector()
                {
                    Left = width / -2 + width / adapterCount * (adapterIndex + 0.5),
                    Top = 6 + height1 * adapterIndex,
                    Length = height0 * (switchIndex - switchCount) - height1 * adapterIndex + 47
                };
            }

            Notify(new { Vector });
        }
        public string IPAddress { get; set; }
        public string MACAddress { get; set; }
        public string SwitchIPAddress { get; set; }
        public LineVector Vector { get; set; }
        public DeviceState State { get; set; }
        public string Tip => string.Format(AppResource.GetString(AppResource.StringKey.Tip_Adapter), IPAddress, MACAddress ?? (Host == null ? AppResource.GetString(AppResource.StringKey.Unknown) : Host.MACAddress), SwitchDevice == null ? AppResource.GetString(AppResource.StringKey.Unknown) : SwitchDevice.Name, Host == null || Host.Port == null ? AppResource.GetString(AppResource.StringKey.Unknown) : Host.Port.Name, State == DeviceState.Online ? AppResource.GetString(AppResource.StringKey.Connected) : (State == DeviceState.Offline ? AppResource.GetString(AppResource.StringKey.Disconnected) : (State == DeviceState.Reserve ? AppResource.GetString(AppResource.StringKey.Reserve) : AppResource.GetString(AppResource.StringKey.Unknown))), Environment.NewLine);
        public SwitchHost Host { get; set; }
        public SwitchDeviceModelView SwitchDevice { get; set; }

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
            Notify(new { State, SwitchIPAddress, SwitchDevice, Host, Tip });
        }
        public LanHostAdapter(string ip)
        {
            IPAddress = ip;
        }
        public LanHostAdapter(string ip, string mac)
        {
            IPAddress = ip;
            MACAddress = mac.ToUpper();
        }
    }
    public class LanHostModelView
    {
        public string Name { get; set; }
        public string Tip { get; set; }
        public List<LanHostAdapter> AdapterList { get; set; }
        public string ActiveCount => string.Format("{0}/{1}", AdapterList.Where(item => item.State == DeviceState.Online).Count(), AdapterList.Count);
        public LanHostModelView(string name, string iplist)
        {
            Name = name;
            AdapterList = iplist == null ? new List<LanHostAdapter>() : iplist.Split(';').Select(item =>
            {
                if (item.Contains("|"))
                {
                    var arr = item.Trim().Split('|');
                    var ip = arr[0];
                    var mac = arr[1];
                    return new LanHostAdapter(ip, mac);
                }
                else
                {
                    var ip = item.Trim();
                    return new LanHostAdapter(ip);
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
    public class SwitchConnectonModelView : CustomINotifyPropertyChanged, IHoverable
    {
        public double Top { get; set; }
        public double Bottom { get; set; }
        public string Brief { get; set; }
        public bool IsHidden { get; set; }
        public DeviceState State { get; set; }
        public SwitchDeviceModelView DeviceA { get; set; }
        public SwitchHost HostA { get; set; }
        public SwitchDeviceModelView DeviceB { get; set; }
        public SwitchHost HostB { get; set; }
        public string Tip => string.Format(AppResource.GetString(AppResource.StringKey.Tip_SwitchConnection), Environment.NewLine, AppResource.GetString(AppResource.StringKey.SwitchCascading), string.Format("{0} - {1}", DeviceA?.Name, DeviceB?.Name), string.Format("{0} - {1}", HostA?.Port == null ? AppResource.GetString(AppResource.StringKey.Unknown) : HostA.Port.Name, HostB?.Port == null ? AppResource.GetString(AppResource.StringKey.Unknown) : HostB.Port.Name), State == DeviceState.Unknown ? AppResource.GetString(AppResource.StringKey.Unknown) : (State == DeviceState.Online ? AppResource.GetString(AppResource.StringKey.Connected) : AppResource.GetString(AppResource.StringKey.Disconnected)));
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
        public SwitchConnectonModelView(string brief, SwitchDeviceModelView deviceA, SwitchDeviceModelView deviceB, int indexA, int indexB)
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
                Top = indexA * height + top;
                Bottom = indexB * height + top;
            }
            else
            {
                DeviceA = deviceB;
                DeviceB = deviceA;
                Top = indexB * height + top;
                Bottom = indexA * height + top;
            }
        }
        public void Refresh()
        {
            Notify(new { State, Tip });
        }

    }
    public class SwitchDeviceModelView : CustomINotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public string MACAddress { get; set; }
        public string Information { get; set; }
        public DeviceState State { get; set; } = DeviceState.Unknown;
        public string UpTime { get; set; }
        public string Tip => string.Format(AppResource.GetString(AppResource.StringKey.Tip_SwitchDevice), Environment.NewLine, Name, Address, MACAddress, State == DeviceState.Online ? AppResource.GetString(AppResource.StringKey.Online) : (State == DeviceState.Offline ? AppResource.GetString(AppResource.StringKey.Offline) : AppResource.GetString(AppResource.StringKey.Unknown)), UpTime ?? AppResource.GetString(AppResource.StringKey.Unknown));
        public List<SwitchPort> PortList { get; set; }
        public List<SwitchHost> HostList { get; set; }
        public int PortCount => PortList == null ? 0 : PortList.Where(item => item.IsUp).Count();
        public int HostCount => HostList == null ? 0 : HostList.Count();
        public SwitchDeviceModelView(string name, string ip)
        {
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
            SwitchDeviceModelView switchDevice = new SwitchDeviceModelView("test", ip)
            {
                Address = ip,
                EndPoint = new IPEndPoint(IPAddress.Parse(ip), 161),
                Information = string.Format("HUAWEI S5720{0}HUAWEI S5720{0}HUAWEI S5720", Environment.NewLine),
                PortList = Enumerable.Range(0, 28).Select(item => new SwitchPort()
                {
                    Name = item.ToString(),
                    Brief = "GigabitEthernet1/0/" + item.ToString(),
                    IsUp = item % 3 == 1,
                    IsFiber = item >= 24
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
}
