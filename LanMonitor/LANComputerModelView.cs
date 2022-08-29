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
        public string ToolTip => string.Format(AppResource.GetString(AppResource.StringKey.Tip_Computer), Environment.NewLine, Name, IPAddress, UID);

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
        }

        public void Resolve(LocalNetworkComputer computer)
        {
            Name = computer.Name;
            Status = computer.Status.ToString();
            IPAddress = computer.IPAddress;
            UID = computer.UID;
            Latency = computer.Latency == -1 ? "???" : (computer.Latency >= 1000 ? ">1000ms" : computer.Latency.ToString() + "ms");

            Notify(new { Name, Status, IPAddress, UID, Latency, ToolTip });
        }
    }
}
