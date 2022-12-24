namespace NIASReport
{
    public class ReportManager
    {
        public string ReportDirectory { get; private set; }
        public string ReportTemplatePath { get; private set; }
        public string StationName { get; private set; }
        public ReportManager(string directory, string template, string stationName, int triggerTime)
        {
            ReportDirectory = directory;
            ReportTemplatePath = template;
            StationName = stationName;
        }

        public void ListReport()
        {

        }
    }
}