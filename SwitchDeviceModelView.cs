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
    }
    public class SwitchHost
    {
        public string HostName { get; set; }
        public string IPAddress { get; set; }
        public string MACAddress { get; set; }
    }
    public class LanHostModelView
    {
        public string Name { get; set; }
        public List<string> IPAddress { get; set; }
        public class LineVector
        {
            public double Left { get; set; }
            public double Top { get; set; }
            public double Length { get; set; }
            public DeviceState State { get; set; }
        }
        public List<LineVector> VectorList { get; set; }
        public static List<LineVector> RefreshVector(int count)
        {
            if (count == 0)
            {
                return null;
            }
            double width = 60;
            double height0 = 92;
            double height1 = 38;
            return Enumerable.Range(0, count).Select(item => new LineVector()
            {
                Left = width / -2 + width / count * (item + 0.5),
                Top = 6 + height1 * item,
                Length = height0 * item - height1 * item - 137,
                State = DeviceState.Online
            }).ToList();
        }
        public LanHostModelView()
        {

        }
        public LanHostModelView(string name, string iplist, int switchCount)
        {
            Name = name;
            IPAddress = iplist == null ? new List<string>() : iplist.Split(';').ToList();
            VectorList = RefreshVector(switchCount);
        }
    }
    public enum DeviceState
    {
        Unknown,
        Online,
        Offline
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
