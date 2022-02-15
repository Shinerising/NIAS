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
        public string ToolTip { get; set; }

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

            ToolTip = string.Format(Application.Current.FindResource("PortToolTip").ToString(),
                Environment.NewLine, Type, LocalEndPoint, RemoteEndPoint);
        }

        public void Resolve(ActivePort port)
        {
            string type = port.Type;
            string state = port.State;
            if (state != "" && state != "Listen" && state != "CloseWait" && state != "TimeWait" && state != "Established")
            {
            }
            string localEndPoint = port.LocalEndPoint;
            string remoteEndPoint = port.RemoteEndPoint;
            int pid = port.PID;
            string processName = port.ProcessName;

            string toolTip = string.Format(Application.Current.FindResource("PortToolTip").ToString(),
                Environment.NewLine, type, localEndPoint, remoteEndPoint);

            if (Type != type)
            {
                Type = type;
                Notify(new { Type });
            }
            if (State != state)
            {
                State = state;
                Notify(new { State, StateText });
            }
            if (LocalEndPoint != localEndPoint)
            {
                LocalEndPoint = localEndPoint;
                Notify(new { LocalEndPoint });
            }
            if (RemoteEndPoint != remoteEndPoint)
            {
                RemoteEndPoint = remoteEndPoint;
                Notify(new { RemoteEndPoint });
            }
            if (PID != pid)
            {
                PID = pid;
                Notify(new { PID });
            }
            if (ProcessName != processName)
            {
                ProcessName = processName;
                Notify(new { ProcessName });
            }
            if (Type != type)
            {
                Type = type;
                Notify(new { Type });
            }
            if (ToolTip != toolTip)
            {
                ToolTip = toolTip;
                Notify(new { ToolTip });
            }
        }
    }
}
