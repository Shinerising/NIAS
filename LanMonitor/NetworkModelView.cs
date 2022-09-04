using System;
using System.Windows;

namespace LanMonitor
{
    public class NetworkModelView : CustomINotifyPropertyChanged
    {
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public string Type { get; set; }
        public string MacAddress { get; set; }
        public string Status { get; set; }
        public string MaxSpeed { get; set; }
        public string Description { get; set; }
        public string ToolTip => string.Format(AppResource.GetString(AppResource.StringKey.Tip_NetworkDevice), Environment.NewLine, Description, IPAddress, MacAddress, MaxSpeed);
        public string DownloadSpeed { get; set; }
        public string UploadSpeed { get; set; }

        public NetworkModelView()
        {

        }

        public NetworkModelView(NetworkAdapter adapter)
        {
            Name = adapter.Name;
            Status = adapter.Status.ToString();
            Type = adapter.Type.ToString();
            DownloadSpeed = adapter.DownloadSpeedString;
            UploadSpeed = adapter.UploadSpeedString;
            Description = adapter.Description;
            MaxSpeed = adapter.MaxSpeed;
            IPAddress = adapter.IPAddress;
            MacAddress = adapter.MACAddress;
        }

        public void Resolve(NetworkAdapter adapter)
        {
            Name = adapter.Name;
            Status = adapter.Status.ToString();
            Type = adapter.Type.ToString();
            DownloadSpeed = adapter.DownloadSpeedString;
            UploadSpeed = adapter.UploadSpeedString;
            Description = adapter.Description;
            MaxSpeed = adapter.MaxSpeed;
            IPAddress = adapter.IPAddress;
            MacAddress = adapter.MACAddress;

            Notify(new { Name, Status, Type, DownloadSpeed, UploadSpeed, Description, MaxSpeed, IPAddress, MacAddress, ToolTip });
        }
    }
}
