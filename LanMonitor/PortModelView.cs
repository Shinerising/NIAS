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
                return State switch
                {
                    "Established" => AppResource.GetString(AppResource.StringKey.Port_Established),
                    "Listen" => AppResource.GetString(AppResource.StringKey.Port_Listening),
                    "CloseWait" => AppResource.GetString(AppResource.StringKey.Port_CloseWait),
                    "TimeWait" => AppResource.GetString(AppResource.StringKey.Port_TimeWait),
                    "SynSent" => AppResource.GetString(AppResource.StringKey.Port_SynSent),
                    _ => AppResource.GetString(AppResource.StringKey.Port_Default),
                };
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
