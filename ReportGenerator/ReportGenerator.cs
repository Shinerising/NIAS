namespace NIASReport
{
    public class ReportGenerator
    {
        private readonly string templatePath;
        private const string scriptTag = "<script id=\"rawData\" type=\"application/json\">{0}</script>";
        public ReportGenerator(string templatePath)
        {
            this.templatePath = templatePath;
        }

        public void ApplyData(string data, string target)
        {
            string tempFile = Path.GetTempFileName();
            File.Copy(templatePath, tempFile, true);
            using StreamWriter sw = File.AppendText(tempFile);
            sw.WriteLine(string.Format(scriptTag, data));
            File.Move(tempFile, target);
        }

        public void ExportReport(string directory, string filename)
        {

        }
    }
}