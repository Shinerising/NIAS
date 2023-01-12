using NIASReport;
using System.Collections.Generic;
using System.Linq;
using static NIASReport.RawData;

namespace LanMonitor
{
    public class RawDataHelper
    {
        private static ReportManager reportManager;
        public static void SetManager(ReportManager manager)
        {
            reportManager = manager;
        }
        public static int ConvertState(DeviceState state)
        {
            return state switch
            {
                DeviceState.Unknown => (int)ImpactLevel.Idle,
                DeviceState.Online => (int)ImpactLevel.Normal,
                DeviceState.Offline => (int)ImpactLevel.Error,
                DeviceState.Reserve => (int)ImpactLevel.Warning,
                _ => (int)ImpactLevel.Unknown,
            };
        }
        public static void SaveSwitchData(SwitchDeviceModelView switchDevice)
        {
            if (switchDevice == null)
            {
                return;
            }
            Switch @switch = new()
            {
                SwitchID = switchDevice.ID,
                State = ConvertState(switchDevice.State),
                CPU = 0,
                REM = 0,
                TEM = 0,
                Port = switchDevice.PortList == null ? "" : string.Join(',', switchDevice.PortList.Select(item => item.Index)),
                PortInSpeed = switchDevice.PortList == null ? "" : string.Join(',', switchDevice.PortList.Select(item => item.InRate)),
                PortOutSpeed = switchDevice.PortList == null ? "" : string.Join(',', switchDevice.PortList.Select(item => item.OutRate)),
            };
            reportManager?.AddData(new[] { @switch });
        }
        public static void SaveAdapterData(IEnumerable<LanHostModelView> hostList)
        {
            if (hostList == null)
            {
                return;
            }
            List<Adapter> list = new List<Adapter>();
            foreach (var host in hostList)
            {
                if (host.AdapterList == null)
                {
                    continue;
                }
                foreach (var adapter in host.AdapterList)
                {
                    list.Add(new()
                    {
                        HostID = host.ID,
                        AdapterID = adapter.ID,
                        Latency = 0,
                        InSpeed = adapter.Host?.Port?.InRate ?? 0,
                        OutSpeed = adapter.Host?.Port?.OutRate ?? 0,
                    });
                }
            }
            reportManager?.AddData(list);
        }
        public static void SaveConnectionData(IEnumerable<LanHostModelView> hostList, IEnumerable<SwitchConnectonModelView> connectionList)
        {

        }
        public static void SaveLogData(string name, string text)
        {
            Log @log = new()
            {
                Name = name,
                Text = text,
            };
            reportManager?.AddData(new[] { @log });
        }
        public static void SaveAlarmData(string name, string text)
        {
            Alarm @alarm = new()
            {
                Name = name,
                Text = text,
            };
            reportManager?.AddData(new[] { @alarm });
        }
    }
}
