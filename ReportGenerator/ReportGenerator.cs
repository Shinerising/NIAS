using System.Reflection;
using System.Text.Json;
using static NIASReport.RawData;
using static NIASReport.ReportUtility;

namespace NIASReport
{
    public class ReportGenerator : IDisposable
    {
        public event ErrorEventHandler? ErrorHandler;

        private static readonly TimeSpan collectTime = TimeSpan.FromDays(1);

        private readonly Task task;
        private readonly CancellationTokenSource cancellation;
        public string ReportDirectory { get; private set; }
        public string ReportTemplatePath { get; private set; }
        public string LocationName { get; private set; }
        public string UserName { get; private set; }
        public TimeSpan RefreshTime { get; set; }
        public bool IsGenerateRequested { get; set; }

        public ReportGenerator(string directory, string template, string location, string username, int triggerTime)
        {
            ReportDirectory = directory;
            ReportTemplatePath = template;
            LocationName = location;
            UserName = username;

            cancellation = new CancellationTokenSource();
            task = new Task(Procedure, cancellation.Token);

            RefreshTime = TimeSpan.FromMinutes(triggerTime);
        }
        public void Start()
        {
            task.Start();
        }
        private async void Procedure()
        {
            DateTimeOffset timestamp = DateTimeOffset.Now;
            while (!cancellation.Token.IsCancellationRequested)
            {
                if (DateTimeOffset.Now >= (DateTimeOffset.Now.Date + RefreshTime) && timestamp < (DateTimeOffset.Now.Date + RefreshTime))
                {
                    await GenerateFile();
                }
                else if (IsGenerateRequested)
                {
                    IsGenerateRequested = false;
                    await GenerateFile();
                }

                timestamp = DateTimeOffset.Now;
                Thread.Sleep(60000);
            }
        }

        public async Task GenerateFile()
        {
            try
            {
                ReportData data = await GetData();
                string json = JsonSerializer.Serialize(data, ReportDataContext.Default.ReportData);
                string filename = Path.Combine(ReportDirectory, string.Format("NetworkReport {0} {1}.html", LocationName, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")));
                await ApplyDataAndExport(json, ReportTemplatePath, filename);
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(this, new ErrorEventArgs(e));
            }
        }

        public async Task<ReportData> GetData()
        {
            if (DatabaseHelper.Instance == null)
            {
                throw new NullReferenceException();
            }

            long startTime = (DateTimeOffset.Now - collectTime).ToUnixTimeSeconds();
            long endTime = DateTimeOffset.Now.ToUnixTimeSeconds();

            var switchInfo = await DatabaseHelper.Instance.GetData<SwitchInfo>()!;
            var hostInfo = await DatabaseHelper.Instance.GetData<HostInfo>()!;
            var adapterInfo = await DatabaseHelper.Instance.GetData<AdapterInfo>()!;
            var deviceInfo = await DatabaseHelper.Instance.GetData<DeviceInfo>()!;

            var switchList = await DatabaseHelper.Instance.GetDataByTime<Switch>(startTime, endTime)!;
            var adapterList = await DatabaseHelper.Instance.GetDataByTime<Adapter>(startTime, endTime)!;
            var connectionList = await DatabaseHelper.Instance.GetDataByTime<Connection>(startTime, endTime)!;

            var logList = await DatabaseHelper.Instance.GetDataByTime<Log>(startTime, endTime)!;
            var alarmList = await DatabaseHelper.Instance.GetDataByTime<Alarm>(startTime, endTime)!;

            var reportSwitchInfo = ResolveSwitchInfo(switchInfo);
            var reportHostInfo = ResolveHostInfo(hostInfo, adapterInfo);
            var reportDeviceInfo = ResolveDeviceInfo(deviceInfo);
            var reportSwitchData = ResolveSwitchData(switchInfo, switchList, startTime, endTime);
            var reportHostData = ResolveHostData(hostInfo, adapterInfo, adapterList, startTime, endTime);
            var reportConnectionData = ResolveConnectionData(connectionList, startTime, endTime);
            var reportLogList = ResolveLogData(logList);
            var reportAlarmList = ResolveAlarmData(alarmList);

            var reportStatsList = new List<int[]>
            {
                GetHealthStats(reportSwitchData, reportHostData),
                GetNetworkStats(reportHostData),
                GetSensorStats(reportSwitchData),
                GetPortStats(reportDeviceInfo)
            };

            ReportData data = new()
            {
                Title = "系统版本号：" + Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
                Location = LocationName,
                User = UserName,
                CreateTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                StartTime = startTime,
                EndTime = endTime,
                SwitchInfo = reportSwitchInfo,
                HostInfo = reportHostInfo,
                DeviceInfo = reportDeviceInfo,
                Switch = reportSwitchData,
                Host = reportHostData,
                Connection = reportConnectionData,
                Log = reportLogList,
                Alarm = reportAlarmList,
                Stats = reportStatsList
            };

            return data;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                cancellation.Cancel();
            }
        }
    }
}