using System.IO;
using System.Text.Json;

namespace NIASReport
{
    public class ReportManager
    {
        public string ReportDirectory { get; private set; }
        public string ReportTemplatePath { get; private set; }
        public string LocationName { get; private set; }
        public event ErrorEventHandler? ErrorHandler;

        private readonly ReportRecorder recorder;
        private readonly DatabaseHelper dbHelper;
        public ReportManager(string directory, string template, string location, int triggerTime)
        {
            ReportDirectory = directory;
            ReportTemplatePath = template;
            LocationName = location;

            dbHelper = DatabaseHelper.Initialize("raw.sqlite");
            dbHelper.ErrorHandler += HandleError;

            recorder = new ReportRecorder();
            recorder.ErrorHandler += HandleError;
        }

        public async Task Initialize()
        {
            await dbHelper.OpenDatabase();

            recorder.Start();
        }

        private void HandleError(object sender, ErrorEventArgs e)
        {
            ErrorHandler?.Invoke(sender, e);
        }

        public async Task Close()
        {
            await dbHelper.CloseDatabase();
        }

        public void ListReport()
        {

        }

        public void CreateDatabase()
        {

        }

        public void AddData<T>(IEnumerable<T> list)
        {
            recorder.AddData(list);
        }

        public void LoadData()
        {

        }

        public void GenerateNewReport()
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
            ReportGenerator.ApplyData(json, ReportTemplatePath, filename);
        }
    }
}