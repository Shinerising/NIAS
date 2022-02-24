using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LanMonitor
{
    public class SwitchPort : CustomINotifyPropertyChanged
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Brief { get; set; }
        public bool IsUp { get; set; }
        public bool IsFiber { get; set; }
        public string Tip => string.Format("网口号：{0}{3}接口名称：{1}{3}当前状态：{2}", Name, Brief, IsUp ? "已连接" : "未连接", Environment.NewLine);
        public void Refresh(SwitchPort port)
        {
            Index = port.Index;
            Name = port.Name;
            Brief = port.Brief;
            IsUp = port.IsUp;
            IsFiber = port.IsFiber;
            Notify(new { Index, Name, Brief, IsUp, IsFiber, Tip });
        }
    }
    public class SwitchHost : CustomINotifyPropertyChanged
    {
        public string HostName { get; set; }
        public string IPAddress { get; set; }
        public string MACAddress { get; set; }
        public int PortIndex { get; set; }
        public SwitchPort Port { get; set; }
        public bool IsCascade { get; set; }
        public HostState State { get; set; }
        public string Tip => string.Format("IP地址：{0}{4}MAC地址：{1}{4}网口号：{2}{4}地址类型：{3}", IPAddress, MACAddress, Port == null ? "未知" : Port.Name, State == HostState.Dynamic ? "动态" : (State == HostState.Static ? "静态" : "其他"), Environment.NewLine);
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
    public class LanHostAdapter : CustomINotifyPropertyChanged
    {
        public struct LineVector
        {
            public double Left { get; set; }
            public double Top { get; set; }
            public double Length { get; set; }
            public bool IsEnabled { get; set; }
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
                    Length = height0 * (switchIndex - switchCount) - height1 * adapterIndex + 47,
                    IsEnabled = true
                };
            }

            Notify(new { Vector });
        }
        public string IPAddress { get; set; }
        public string SwitchIPAddress { get; set; }
        public LineVector Vector { get; set; }
        public DeviceState State { get; set; }
        public string Tip => string.Format("IP地址：{0}{5}MAC地址：{1}{5}接入交换机：{2}{5}网口号：{3}{5}连接状态：{4}", IPAddress, Host == null ? "未知" : Host.MACAddress, SwitchDevice == null ? "未知" : SwitchDevice.Name, Host == null || Host.Port == null ? "未知" : Host.Port.Name, State == DeviceState.Online ? "已连接" : (State == DeviceState.Offline ? "连接断开" : "未知"), Environment.NewLine);
        public SwitchHost Host { get; set; }
        public SwitchDeviceModelView SwitchDevice { get; set; }

        public bool IsHover { get; set; }
        public void SetHover(bool flag)
        {
            IsHover = flag;
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
            AdapterList = iplist == null ? new List<LanHostAdapter>() : iplist.Split(';').Select(item => new LanHostAdapter(item.Trim())).ToList();
        }
    }
    public enum DeviceState
    {
        Unknown,
        Online,
        Offline
    }
    public enum HostState
    {
        Other = 1,
        Invalid = 2,
        Dynamic = 3,
        Static = 4,
    }
    public class SwitchDeviceModelView : CustomINotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public string Information { get; set; }
        public DeviceState State { get; set; } = DeviceState.Unknown;
        public string Tip => string.Format("设备名称：{0}{3}通信IP地址：{1}{3}当前状态：{2}", Name, Address, State == DeviceState.Online ? "在线" : (State == DeviceState.Offline ? "离线" : "未知"), Environment.NewLine);
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
            RefreshPortList(null);
            RefreshHostList(null);
        }
        public void Refresh()
        {
            Notify(new { State, Information, Tip });
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
