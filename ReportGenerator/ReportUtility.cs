﻿using System.IO;
using System.IO.Compression;
using System.Text;
using static NIASReport.RawData;

namespace NIASReport
{
    public static class ReportUtility
    {
        private const string scriptTag = "<script id=\"rawData\" type=\"application/json\" {0}>{1}</script>";
        private const int CPUThreshold = 80;
        private const int MemoryThreshold = 80;
        private const int TemperatureThreshold = 50;
        private const int LatencyThreshold = 50;
        private const int RateThreshold = 10485760;
        private const int NormalState = 2;

        private static readonly bool IsCompressing = true;

        public static async Task ApplyDataAndExport(string data, string template, string target) {
            string tempFile = Path.GetTempFileName();
            File.Copy(template, tempFile, true);
            using StreamWriter sw = File.AppendText(tempFile);
            if (IsCompressing) {
                await sw.WriteLineAsync(string.Format(scriptTag, "compressed", await CompressText(data)));
            }
            else {
                await sw.WriteLineAsync(string.Format(scriptTag, "", data));
            }
            sw.Flush();
            sw.Dispose();
            File.Move(tempFile, target);
        }

        public static async Task<string> CompressText(string text)
        {
            using MemoryStream outStream = new();
            using (GZipStream gzipStream = new(outStream, CompressionMode.Compress)) {
                await gzipStream.WriteAsync(Encoding.UTF8.GetBytes(text));
                await gzipStream.FlushAsync();
            }
            return Convert.ToBase64String(outStream.ToArray());
        }

        public static int[] GetHealthStats(List<ReportSwitch> list0, List<ReportHost> list1)
        {
            int switchCount = list0.Sum(item => item.State.Count);
            int switchErrorCount = list0.Sum(item => item.State.Count(_item => _item != NormalState));
            int hostCount = list1.Sum(item => item.State.Count);
            int hostErrorCount = list1.Sum(item => item.State.Count(_item => _item != NormalState));

            return new int[]{ switchErrorCount, switchCount, hostErrorCount, hostCount };
        }

        public static int[] GetNetworkStats(List<ReportHost> list)
        {
            int totalCount = list.Sum(item => item.InSpeed.Count);
            int inSpeedCount = list.Sum(item => item.InSpeed.Count(_item => _item > RateThreshold));
            int outSpeedCount = list.Sum(item => item.OutSpeed.Count(_item => _item > RateThreshold));
            int latencyCount = list.Sum(item => item.Latency.Count(_item => _item > LatencyThreshold));

            return new int[]{ inSpeedCount, totalCount, outSpeedCount, totalCount, latencyCount, totalCount };
        }
        public static int[] GetSensorStats(List<ReportSwitch> list)
        {
            int totalCount = list.Sum(item => item.CPU.Count);
            int cpuCount = list.Sum(item => item.CPU.Count(_item => _item > CPUThreshold));
            int memoryCount = list.Sum(item => item.REM.Count(_item => _item > MemoryThreshold));
            int temperatureCount = list.Sum(item => item.TEM.Count(_item => _item > TemperatureThreshold));

            return new int[]{ cpuCount, totalCount, memoryCount, totalCount, temperatureCount, totalCount };
        }

        public static int[] GetPortStats(List<ReportDeviceInfo> list)
        {
            int totalCount = list.Count;
            int systemCount = list.Count(item => item.OS?.Contains("XP") ?? false);
            int warningCount = list.Sum(item => item.WarningCount ?? 0);
            int portCount = list.Sum(item => item.PortCount ?? 0);

            return new int[]{ systemCount, totalCount, warningCount, portCount };
        }

        public static List<ReportSwitchInfo> ResolveSwitchInfo(IEnumerable<SwitchInfo> infoList)
        {
            return infoList.Select(item => new ReportSwitchInfo()
            {
                ID = item.ID,
                Name = item.Name,
                Address = item.Address,
                MACAddress = item.MACAddress,
                Vendor = item.Vendor,
            }).ToList();
        }
        public static List<ReportHostInfo> ResolveHostInfo(IEnumerable<HostInfo> hostList, IEnumerable<AdapterInfo> adapterList)
        {
            return hostList.Select(item =>
            {
                var list = adapterList.Where(_item => _item.HostID == item.ID);
                return new ReportHostInfo()
                {
                    ID = item.ID,
                    Name = item.Name,
                    Address = string.Join(',', list.Select(_item => _item.Address)),
                    MACAddress = string.Join(',', list.Select(_item => _item.MACAddress)),
                    Vendor = string.Join(',', list.Select(_item => _item.Vendor).Distinct()),
                };
            }).ToList();
        }
        public static List<ReportDeviceInfo> ResolveDeviceInfo(IEnumerable<DeviceInfo> infoList)
        {
            return infoList.Select(item => new ReportDeviceInfo()
            {
                Name = item.Name,
                Address = item.Address,
                MACAddress = item.MACAddress,
                Vendor = item.Vendor,
                OS = item.OS,
                PortCount = item.PortCount,
                WarningCount = item.WarningCount,
            }).ToList();
        }
        public static List<ReportSwitch> ResolveSwitchData(IEnumerable<SwitchInfo> infoList, IEnumerable<Switch> dataList, long startTime, long endTime)
        {
            var result = new List<ReportSwitch>();

            if (infoList == null || !infoList.Any())
            {
                return result;
            }

            var dict = dataList == null || !dataList.Any() ? new Dictionary<int, List<Switch>>()
                : dataList
                .GroupBy(item => item.SwitchID)
                .ToDictionary(group => group.Key, group => SamplingData(group).ToList());

            foreach (var item in infoList)
            {
                var target = new ReportSwitch(item.ID);

                var list = new List<Switch>();

                if (dict.ContainsKey(item.ID))
                {
                    list = dict[item.ID];
                }

                Switch add;
                Switch empty = new();
                int index = 0;
                for (long timestamp = startTime - (startTime % 60); timestamp < endTime; timestamp += 60)
                {
                    if (list.Count > index && list[index].Time == timestamp)
                    {
                        add = list[index];
                        index += 1;
                    }
                    else
                    {
                        add = empty;
                    }

                    target.Time.Add(timestamp);
                    target.State.Add(add.State);
                    target.CPU.Add(add.CPU);
                    target.REM.Add(add.REM);
                    target.TEM.Add(add.TEM);
                    target.Port.Add(add.Port);
                    target.PortInSpeed.Add(add.PortInSpeed);
                    target.PortOutSpeed.Add(add.PortOutSpeed);
                }

                result.Add(target);
            }

            return result;
        }
        public static List<ReportHost> ResolveHostData(IEnumerable<HostInfo> hostInfo, IEnumerable<AdapterInfo> adapterInfo, IEnumerable<Adapter> adapterList, long startTime, long endTime)
        {
            var result = new List<ReportHost>();

            if (hostInfo == null || !hostInfo.Any() || adapterInfo == null || !adapterInfo.Any())
            {
                return result;
            }

            var dict = adapterList == null || !adapterList.Any() ? new Dictionary<int, List<Adapter>>()
                : adapterList
                .GroupBy(item => item.HostID * 100 + item.AdapterID)
                .ToDictionary(group => group.Key, group => SamplingData(group).ToList());

            foreach (var item in hostInfo)
            {
                var target = new ReportHost(item.ID);

                var collection = new List<List<Adapter>>();

                foreach (var _item in adapterInfo.Where(_item => _item.HostID == item.ID))
                {
                    int id = _item.HostID * 100 + _item.ID;
                    if (dict.TryGetValue(id, out var value))
                    {
                        collection.Add(value);
                    }
                }

                List<Adapter> add;
                Adapter empty = new();
                List<int> offset = new(new int[collection.Count]);
                for (long timestamp = startTime - (startTime % 60); timestamp < endTime; timestamp += 60)
                {
                    add = new List<Adapter>();
                    int index = 0;
                    foreach (var list in collection)
                    {
                        if (list.Count > offset[index] && list[offset[index]].Time == timestamp)
                        {
                            add.Add(list[offset[index]]);
                            offset[index] += 1;
                        }
                        index += 1;
                    }
                    if (add.Count == 0)
                    {
                        add.Add(empty);
                    }

                    target.Time.Add(timestamp);
                    target.State.Add(add.Any(item => item.State == NormalState) ? NormalState : add.Max(item => item.State));
                    target.Latency.Add(add.Min(item => item.Latency));
                    target.InSpeed.Add(add.Sum(item => item.InSpeed));
                    target.OutSpeed.Add(add.Sum(item => item.OutSpeed));
                }

                result.Add(target);
            }

            return result;
        }
        public static ReportConnection ResolveConnectionData(IEnumerable<Connection> connectionList, long startTime, long endTime)
        {
            var result = new ReportConnection();

            var dict = connectionList == null || !connectionList.Any() ? new Dictionary<long, List<Connection>>()
                : connectionList
                .GroupBy(item => item.Time)
                .GroupBy(group => group.Key - (group.Key % 60))
                .Select(group => group.First())
                .ToDictionary(group => group.Key - (group.Key % 60), group => group.ToList());

            List<Connection> add;
            List<Connection> empty = new();
            int index = 0;
            for (long timestamp = startTime - (startTime % 60); timestamp < endTime; timestamp += 60)
            {
                if (dict.Count > index && dict.Keys.ElementAt(index) == timestamp)
                {
                    add = dict.Values.ElementAt(index);
                    index += 1;
                }
                else
                {
                    add = empty;
                }

                int count = add.Count;
                int error = add.Count(item => item.State != NormalState);
                int state = count == 0 ? 0 : error > 2 ? 3 : error > 0 ? 2 : 1;

                result.Time.Add(timestamp);
                result.State.Add(state);
                result.Line.Add(string.Join(',', add.Select(item => string.Format("[{0},{1},{2},{3}]", item.Type, item.Source, item.Target, item.State))));
            }

            return result;
        }

        public static List<ReportLog> ResolveLogData(IEnumerable<Log> logList)
        {
            return logList.Take(100).Select(item => new ReportLog(item.Time, item.Name, item.Text)).ToList();
        }
        public static List<ReportAlarm> ResolveAlarmData(IEnumerable<Alarm> alarmList)
        {
            return alarmList.Take(100).Select(item => new ReportAlarm(item.Time, item.Name, item.Text)).ToList();
        }

        public static IEnumerable<T> SamplingData<T>(IEnumerable<T> list) where T : TimeData<T>
        {
            return list
                .OrderBy(item => item.Time)
                .GroupBy(item => item.Time - (item.Time % 60))
                .Select(group => group.First().Combine(group))
                .Select(item =>
                {
                    item.Time -= item.Time % 60;
                    return item;
                });
        }
    }
}