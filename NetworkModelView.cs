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
        public string Speed { get; set; }
        public string ToolTip { get; set; }
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

            ToolTip = string.Format(Application.Current.FindResource("NetworkToolTip").ToString(),
                Environment.NewLine, adapter.Description, adapter.IPAddress, adapter.MACAddress, adapter.MaxSpeed);
        }

        public void Resolve(NetworkAdapter adapter)
        {
            Name = adapter.Name;
            Status = adapter.Status.ToString();
            Type = adapter.Type.ToString();
            DownloadSpeed = adapter.DownloadSpeedString;
            UploadSpeed = adapter.UploadSpeedString;
            ToolTip = string.Format(Application.Current.FindResource("NetworkToolTip").ToString(),
                Environment.NewLine, adapter.Description, adapter.IPAddress, adapter.MACAddress, adapter.MaxSpeed);

            Notify(new { Name, Status, Type, DownloadSpeed, UploadSpeed, ToolTip });
        }
    }
}
