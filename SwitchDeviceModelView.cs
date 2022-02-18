using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LanMonitor
{
    public class SwitchPort
    {
        public string Name { get; set; }
        public string Brief { get; set; }
        public bool IsUp { get; set; }
        public bool IsFiber { get; set; }
        public string Tip => Brief;
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            SwitchPort other = obj as SwitchPort;
            if (other == null)
            {
                return false;
            }
            return Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name == null ? 0 : Name.GetHashCode();
        }
    }
    public class SwitchHost
    {
        public string HostName { get; set; }
        public string IPAddress { get; set; }
        public string MACAddress { get; set; }
        public HostState State { get; set; }
        public string Tip => MACAddress;
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            SwitchHost other = obj as SwitchHost;
            if (other == null)
            {
                return false;
            }
            return IPAddress == other.IPAddress;
        }

        public override int GetHashCode()
        {
            return IPAddress == null ? 0 : IPAddress.GetHashCode();
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
        public LineVector Vector { get; set; }
        public DeviceState State { get; set; }

        public bool IsHover { get; set; }
        public void SetHover(bool flag)
        {
            IsHover = flag;
            Notify(new { IsHover });
        }
        public LanHostAdapter(string ip)
        {
            IPAddress = ip;
        }
    }
    public class LanHostModelView
    {
        public string Name { get; set; }
        public List<LanHostAdapter> AdapterList { get; set; }
        public LanHostModelView(string name, string iplist)
        {
            Name = name;
            AdapterList = iplist == null ? new List<LanHostAdapter>() : iplist.Split(';').Select(item => new LanHostAdapter(item)).ToList();
            for (int i = 0; i < AdapterList.Count; i += 1)
            {
                AdapterList[i].RefreshVector(i, AdapterList.Count, i % 2, 2);
                AdapterList[i].State = i % 2 == 0 ? DeviceState.Online : DeviceState.Offline;
            }
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
        public List<SwitchPort> PortList { get; set; }
        public List<SwitchHost> HostList { get; set; }
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
            PortList = list;
            Notify(new { PortList });
        }
        public void RefreshHostList(List<SwitchHost> list)
        {
            HostList = list;
            Notify(new { HostList });
        }
        public void SetIdle()
        {
            RefreshPortList(null);
            RefreshHostList(null);
        }
        public void Refresh()
        {
            Notify(new { State, Information });
        }
        public static SwitchDeviceModelView GetPreviewInstance(string ip)
        {
            SwitchDeviceModelView switchDevice = new SwitchDeviceModelView("test", ip);
            switchDevice.Address = ip;
            switchDevice.EndPoint = new IPEndPoint(IPAddress.Parse(ip), 161);
            switchDevice.Information = string.Format("HUAWEI S5720{0}HUAWEI S5720{0}HUAWEI S5720", Environment.NewLine);
            switchDevice.PortList = Enumerable.Range(0, 28).Select(item => new SwitchPort()
            {
                Name = "GE1/0/" + item.ToString(),
                Brief = "GigabitEthernet1/0/" + item.ToString(),
                IsUp = item % 3 == 1,
                IsFiber = item >= 24
            }).ToList();
            switchDevice.HostList = new List<SwitchHost>()
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
            };
            return switchDevice;
        }
    }
}
