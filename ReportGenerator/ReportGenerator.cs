namespace NIASReport
{
    public class ReportGenerator
    {
        private const string scriptTag = "<script id=\"rawData\" type=\"application/json\">{0}</script>";

        public static void ApplyData(string data, string template, string target)
        {
            string tempFile = Path.GetTempFileName();
            File.Copy(template, tempFile, true);
            using StreamWriter sw = File.AppendText(tempFile);
            sw.WriteLine(string.Format(scriptTag, data));
            sw.Flush();
            sw.Dispose();
            File.Move(tempFile, target);
        }
    }
}