using static NIASReport.RawData;

namespace NIASReport
{
    public static class ReportUtility
    {
        private const string scriptTag = "<script id=\"rawData\" type=\"application/json\">{0}</script>";

        public static async Task ApplyDataAndExport(string data, string template, string target)
        {
            string tempFile = Path.GetTempFileName();
            File.Copy(template, tempFile, true);
            using StreamWriter sw = File.AppendText(tempFile);
            await sw.WriteLineAsync(string.Format(scriptTag, data));
            sw.Flush();
            sw.Dispose();
            File.Move(tempFile, target);
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
                Switch empty = new Switch();
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
                .ToDictionary(group => group.Key, group => group.ToList());

            List<Connection> add;
            List<Connection> empty = new List<Connection>();
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
                int error = add.Count(item => item.State != 2);
                int state = count == 0 ? 1 : error > 2 ? 4 : error > 0 ? 3 : 2;

                result.Time.Add(timestamp);
                result.State.Add(state);
                result.Line.Add(string.Join(',', add.Select(item => string.Format("[{0},{1},{2},{3}]", item.Type, item.Source, item.Target, item.State))));
            }

            return result;
        }

        public static IEnumerable<T> SamplingData<T>(IEnumerable<T> list) where T : TimeData<T>
        {
            return list
                .OrderBy(item => item.Time)
                .GroupBy(item => item.Time - (item.Time % 60))
                .Select(group => group.First().Combine(group))
                .SelectMany(group => group);
        }
    }
}