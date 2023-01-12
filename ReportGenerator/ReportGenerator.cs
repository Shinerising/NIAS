using System.Text.Json;

namespace NIASReport
{
    public class ReportGenerator
    {
        private const string scriptTag = "<script id=\"rawData\" type=\"application/json\">{0}</script>";

        private readonly Task task;
        private readonly CancellationTokenSource cancellation;
        public string ReportDirectory { get; private set; }
        public string ReportTemplatePath { get; private set; }
        public string LocationName { get; private set; }
        public TimeSpan RefreshTime { get; set; }

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
                    GenerateFile();
                }

                timestamp = DateTimeOffset.Now;
                Thread.Sleep(60000);
            }
        }

        public void GenerateFile()
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
            ApplyData(json, ReportTemplatePath, filename);
        }

        public void GetData()
        {

        }

        public void ApplyData(string data, string template, string target)
        {
            string tempFile = Path.GetTempFileName();
            File.Copy(template, tempFile, true);
            using StreamWriter sw = File.AppendText(tempFile);
            sw.WriteLine(string.Format(scriptTag, data));
            sw.Flush();
            sw.Dispose();
            File.Move(tempFile, target);
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