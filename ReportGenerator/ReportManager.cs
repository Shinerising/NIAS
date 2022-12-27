using System.IO;
using System.Text.Json;

namespace NIASReport
{
    public class ReportManager
    {
        public string ReportDirectory { get; private set; }
        public string ReportTemplatePath { get; private set; }
        public string LocationName { get; private set; }
        public ReportManager(string directory, string template, string location, int triggerTime)
        {
            ReportDirectory = directory;
            ReportTemplatePath = template;
            LocationName = location;
        }

        public void ListReport()
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