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
        public string ToolTip => string.Format(Application.Current.FindResource("PortToolTip").ToString(), Environment.NewLine, Type, LocalEndPoint, RemoteEndPoint);

        public string StateText
        {
            get
            {
                switch (State)
                {
                    case "Established":
                        return Application.Current.FindResource("Port_Established").ToString();
                    case "Listen":
                        return Application.Current.FindResource("Port_Listening").ToString();
                    case "CloseWait":
                        return Application.Current.FindResource("Port_CloseWait").ToString();
                    case "TimeWait":
                        return Application.Current.FindResource("Port_TimeWait").ToString();
                    case "SynSent":
                        return Application.Current.FindResource("Port_SynSent").ToString();
                    case "":
                    default:
                        return Application.Current.FindResource("Port_Default").ToString();
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
