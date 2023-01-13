using System;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace NIASReport
{
    public class ReportGenerator : IDisposable
    {
        private const string scriptTag = "<script id=\"rawData\" type=\"application/json\">{0}</script>";

        private readonly Task task;
        private readonly CancellationTokenSource cancellation;
        public string ReportDirectory { get; private set; }
        public string ReportTemplatePath { get; private set; }
        public string LocationName { get; private set; }
        public TimeSpan RefreshTime { get; set; }
        public bool IsGenerateRequested { get; set; }

        public ReportGenerator()
        {
            cancellation = new CancellationTokenSource();
            task = new Task(Procedure, cancellation.Token);

            RefreshTime = TimeSpan.FromMinutes(240);
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
                    await GenerateFile();
                }

                timestamp = DateTimeOffset.Now;
                Thread.Sleep(60000);
            }
        }

        public async Task GenerateFile()
        {
            ReportData data = new()
            {
                Title = "测试数据",
                Location = LocationName,
                User = "测试人员",
                CreateTime = DateTime.Now
            };
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions() { });
            string filename = Path.Combine(ReportDirectory, string.Format("NetworkReport {0} {1}.html", LocationName, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")));
            await ApplyData(json, ReportTemplatePath, filename);
        }

        public async Task GetData()
        {
            DateTimeOffset startTime = DateTimeOffset.Now - TimeSpan.FromDays(1);
            DateTimeOffset endTime = DateTimeOffset.Now;

            var switchInfo = await DatabaseHelper.Instance?.GetData<RawData.SwitchInfo>()!;
            var hostInfo = await DatabaseHelper.Instance?.GetData<RawData.HostInfo>()!;
            var adapterInfo = await DatabaseHelper.Instance?.GetData<RawData.AdapterInfo>()!;
            var deviceInfo = await DatabaseHelper.Instance?.GetData<RawData.DeviceInfo>()!;

            var switchList = SamplingData(await DatabaseHelper.Instance?.GetDataByTime<RawData.Switch>(startTime, endTime)!, startTime, endTime);
            var adapterList = SamplingData(await DatabaseHelper.Instance?.GetDataByTime<RawData.Adapter>(startTime, endTime)!, startTime, endTime);
            var connectionList = SamplingData(await DatabaseHelper.Instance?.GetDataByTime<RawData.Connection>(startTime, endTime)!, startTime, endTime);
        }

        private async Task ApplyData(string data, string template, string target)
        {
            string tempFile = Path.GetTempFileName();
            File.Copy(template, tempFile, true);
            using StreamWriter sw = File.AppendText(tempFile);
            await sw.WriteLineAsync(string.Format(scriptTag, data));
            sw.Flush();
            sw.Dispose();
            File.Move(tempFile, target);
        }

        private static IEnumerable<T> SamplingData<T>(IEnumerable<T> list, DateTimeOffset startTime, DateTimeOffset endTime) where T : RawData.TimeData<T>
        {
            var newList = list
                .OrderBy(item => item.Time)
                .GroupBy(item => item.Time - (item.Time % 60))
                .Select(group => group.First().Combine(group))
                .SelectMany(group => group);
            return newList;
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