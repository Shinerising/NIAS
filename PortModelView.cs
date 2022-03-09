using System;
using System.Windows;

namespace LanMonitor
{
    public class PortModelView : CustomINotifyPropertyChanged
    {
        public string Type { get; set; }
        public string LocalEndPoint { get; set; }
        public string RemoteEndPoint { get; set; }

        public int PID { get; set; }
        public string ProcessName { get; set; }

        public string State { get; set; }
        public string ToolTip => string.Format(AppResource.GetString(AppResource.StringKey.Tip_ActivePort), Environment.NewLine, Type, LocalEndPoint, RemoteEndPoint);

        public string StateText
        {
            get
            {
                switch (State)
                {
                    case "Established":
                        return AppResource.GetString(AppResource.StringKey.Port_Established);
                    case "Listen":
                        return AppResource.GetString(AppResource.StringKey.Port_Listening);
                    case "CloseWait":
                        return AppResource.GetString(AppResource.StringKey.Port_CloseWait);
                    case "TimeWait":
                        return AppResource.GetString(AppResource.StringKey.Port_TimeWait);
                    case "SynSent":
                        return AppResource.GetString(AppResource.StringKey.Port_SynSent);
                    case "":
                    default:
                        return AppResource.GetString(AppResource.StringKey.Port_Default);
                }
            }
        }

        public PortModelView()
        {

        }
        public PortModelView(ActivePort port)
        {
            Type = port.Type;
            State = port.State;
            LocalEndPoint = port.LocalEndPoint;
            RemoteEndPoint = port.RemoteEndPoint;
        }

        public void Resolve(ActivePort port)
        {
            Type = port.Type;
            State = port.State;
            LocalEndPoint = port.LocalEndPoint;
            RemoteEndPoint = port.RemoteEndPoint;
            PID = port.PID;
            ProcessName = port.ProcessName;

            Notify(new { Type, State, LocalEndPoint, RemoteEndPoint, PID, ProcessName, ToolTip });
        }
    }
}
