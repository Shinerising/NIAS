using System;
using System.Windows;

namespace LanMonitor
{
    /// <summary>
    /// ModelView for LAN Computer
    /// </summary>
    public class LANComputerModelView : CustomINotifyPropertyChanged
    {
        /// <summary>
        /// Name of the computer
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// IP Address of the computer
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// Type of the computer
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// MAC Address of the computer
        /// </summary>
        public string MacAddress { get; set; }
        /// <summary>
        /// Status of the computer
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// UID of the computer
        /// </summary>
        public string UID { get; set; }
        /// <summary>
        /// Latency of the computer
        /// </summary>
        public string Latency { get; set; }
        /// <summary>
        /// ToolTip of the computer
        /// </summary>
        public string ToolTip => string.Format(AppResource.GetString(AppResource.StringKey.Tip_Computer), Environment.NewLine, Name, IPAddress, UID);

        /// <summary>
        /// Constructor
        /// </summary>
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
