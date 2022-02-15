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
            string name = adapter.Name;
            string status = adapter.Status.ToString();
            string type = adapter.Type.ToString();
            string downloadSpeed = adapter.DownloadSpeedString;
            string uploadSpeed = adapter.UploadSpeedString;
            string toolTip = string.Format(Application.Current.FindResource("NetworkToolTip").ToString(),
                Environment.NewLine, adapter.Description, adapter.IPAddress, adapter.MACAddress, adapter.MaxSpeed);

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
            if (Type != type)
            {
                Type = type;
                Notify(new { Type });
            }
            if (DownloadSpeed != downloadSpeed)
            {
                DownloadSpeed = downloadSpeed;
                Notify(new { DownloadSpeed });
            }
            if (UploadSpeed != uploadSpeed)
            {
                UploadSpeed = uploadSpeed;
                Notify(new { UploadSpeed });
            }
            if (ToolTip != toolTip)
            {
                ToolTip = toolTip;
                Notify(new { ToolTip });
            }
        }
    }
}
