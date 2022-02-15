using System;
using System.Windows;

namespace LanMonitor
{
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
                Notify(new { Name });
            }
            if (Status != status)
            {
                Status = status;
                Notify(new { Status });
            }
            if (IPAddress != ipAddress)
            {
                IPAddress = ipAddress;
                Notify(new { IPAddress });
            }
            if (UID != uid)
            {
                UID = uid;
                Notify(new { UID });
            }
            if (Latency != latency)
            {
                Latency = latency;
                Notify(new { Latency });
            }
            if (ToolTip != toolTip)
            {
                ToolTip = toolTip;
                Notify(new { ToolTip });
            }
        }
    }
}
