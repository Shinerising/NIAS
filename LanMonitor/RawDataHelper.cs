using NIASReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using static LanMonitor.NetworkManager;
using static NIASReport.RawData;

namespace LanMonitor
{
    public class RawDataHelper
    {
        private static ReportManager reportManager;
        private static readonly TimeSpan ListDataSampleSpan = TimeSpan.FromMilliseconds(200);
        private static readonly TimeSpan LogDataSampleSpan = TimeSpan.FromMilliseconds(60000);
        private static readonly TimeSpan InfoDataSampleSpan = TimeSpan.FromMilliseconds(3600000);
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
        private static DateTime switchTimestamp = DateTime.MinValue;
        public static void SaveSwitchData(SwitchDeviceModelView switchDevice)
        {
            if (reportManager == null || DateTime.Now - switchTimestamp < ListDataSampleSpan)
            {
                return;
            }
            switchTimestamp = DateTime.Now;

            if (switchDevice == null)
            {
                return;
            }
            var portList = (switchDevice.PortList ?? new List<SwitchPort>()).OrderBy(item => item.Name);
            Switch @switch = new()
            {
                SwitchID = switchDevice.ID,
                State = ConvertState(switchDevice.State),
                CPU = switchDevice.CpuUsage,
                REM = switchDevice.MemoryUsage,
                TEM = switchDevice.Temperature,
                Port = switchDevice.PortList == null ? "" : string.Join(',', portList.Select(item => item.Name)),
                PortInSpeed = switchDevice.PortList == null ? "" : string.Join(',', portList.Select(item => item.InRate)),
                PortOutSpeed = switchDevice.PortList == null ? "" : string.Join(',', portList.Select(item => item.OutRate)),
            };
            reportManager.AddData(new[] { @switch });
        }
        private static DateTime adapterTimestamp = DateTime.MinValue;
        public static void SaveAdapterData(IEnumerable<LanHostModelView> hostList)
        {
            if (reportManager == null || DateTime.Now - adapterTimestamp < ListDataSampleSpan)
            {
                return;
            }
            adapterTimestamp = DateTime.Now;

            if (hostList == null)
            {
                return;
            }
            List<Adapter> list = new();
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
                        State = ConvertState(adapter.State),
                        Latency = 0,
                        InSpeed = (int)(adapter.Host?.Port?.OutRate ?? 0),
                        OutSpeed = (int)(adapter.Host?.Port?.InRate ?? 0),
                    });
                }
            }
            reportManager.AddData(list);
        }
        private static DateTime connectionTimestamp = DateTime.MinValue;
        public static void SaveConnectionData(IEnumerable<LanHostModelView> hostList, IEnumerable<SwitchConnectonModelView> connectionList)
        {
            if (reportManager == null || DateTime.Now - connectionTimestamp < ListDataSampleSpan)
            {
                return;
            }
            connectionTimestamp = DateTime.Now;

            List<Connection> list = new();
            if (connectionList != null)
            {
                foreach (var connection in connectionList)
                {
                    if (connection.DeviceA == null || connection.DeviceB == null)
                    {
                        continue;
                    }
                    list.Add(new()
                    {
                        Type = 0,
                        Source = connection.DeviceA.ID,
                        Target = connection.DeviceB.ID,
                        State = ConvertState(connection.State)
                    });
                }
            }
            if (hostList != null)
            {
                foreach (var host in hostList)
                {
                    if (host.AdapterList == null)
                    {
                        continue;
                    }
                    foreach (var adapter in host.AdapterList)
                    {
                        if (adapter.SwitchDevice == null)
                        {
                            continue;
                        }
                        list.Add(new()
                        {
                            Type = 1,
                            Source = adapter.SwitchDevice.ID,
                            Target = host.ID,
                            AdapterID = adapter.ID,
                            PortID = adapter.Host?.Port?.Index ?? -1,
                            State = ConvertState(adapter.State)
                        });
                    }
                }
            }
            reportManager.AddData(list);
        }
        private static DateTime logTimestamp = DateTime.MinValue;
        private static int logCount = 0;
        public static void SaveLogData(string name, string text)
        {
            if (reportManager == null)
            {
                return;
            }
            if (DateTime.Now - logTimestamp < LogDataSampleSpan)
            {
                logCount += 1;
                if (logCount >= 20)
                {
                    return;
                }
            }
            else
            {
                logCount = 0;
                logTimestamp = DateTime.Now;
            }

            Log @log = new()
            {
                Name = name,
                Text = text,
            };
            reportManager.AddData(new[] { @log });
        }
        private static DateTime alarmTimestamp = DateTime.MinValue;
        private static int alarmCount = 0;
        public static void SaveAlarmData(string name, string text)
        {
            if (reportManager == null)
            {
                return;
            }
            if (DateTime.Now - alarmTimestamp < LogDataSampleSpan)
            {
                alarmCount += 1;
                if (alarmCount >= 20)
                {
                    return;
                }
            }
            else
            {
                alarmCount = 0;
                alarmTimestamp = DateTime.Now;
            }

            Alarm @alarm = new()
            {
                Name = name,
                Text = text,
            };
            reportManager.AddData(new[] { @alarm });
        }

        private static DateTime switchInfoTimestamp = DateTime.MinValue;
        public static void SaveSwitchInfo(IEnumerable<SwitchDeviceModelView> switchList)
        {
            if (reportManager == null || DateTime.Now - switchInfoTimestamp < InfoDataSampleSpan)
            {
                return;
            }
            switchInfoTimestamp = DateTime.Now;

            List<SwitchInfo> list = switchList.Select(item => new SwitchInfo()
            {
                ID = item.ID,
                Name = item.Name,
                Address = item.Address,
                MACAddress = item.MACAddress,
                Vendor = item.Information
            }).ToList();
            reportManager.UpdateInfo(list);
        }

        private static DateTime hostInfoTimestamp = DateTime.MinValue;
        public static void SaveHostInfo(IEnumerable<LanHostModelView> hostList)
        {
            if (reportManager == null || DateTime.Now - hostInfoTimestamp < InfoDataSampleSpan)
            {
                return;
            }
            hostInfoTimestamp = DateTime.Now;

            List<HostInfo> list = new();
            List<AdapterInfo> adapterList = new();
            foreach (var host in hostList)
            {
                list.Add(new()
                {
                    ID = host.ID,
                    Name = host.Name,
                });

                if (host.AdapterList == null)
                {
                    continue;
                }
                foreach (var adapter in host.AdapterList)
                {
                    adapterList.Add(new()
                    {
                        HostID = host.ID,
                        ID = adapter.ID,
                        Address = adapter.IPAddress,
                        MACAddress = adapter.MACAddress,
                        Vendor = adapter.MACVendor,
                    });
                }
            }
            reportManager.UpdateInfo(list);
            reportManager.UpdateInfo(adapterList);
        }

        private static DateTime deviceInfoTimestamp = DateTime.MinValue;
        public static void SaveDeviceInfo(IEnumerable<NMAPHost> hostList)
        {
            if (reportManager == null || DateTime.Now - deviceInfoTimestamp < InfoDataSampleSpan)
            {
                return;
            }
            deviceInfoTimestamp = DateTime.Now;

            List<DeviceInfo> list = hostList.Select(item => new DeviceInfo()
            {
                Name = item.Name,
                Address = item.Address,
                MACAddress = item.MACAddress,
                Vendor = item.MACVendor,
                OS = item.OSName,
                PortCount = item.PortList == null ? 0 : item.PortList.Count,
                WarningCount = item.PortList == null ? 0 : item.PortList.Count(item => item.IsWarning),
            }).ToList();
            reportManager.UpdateInfo(list);
        }
    }
}
