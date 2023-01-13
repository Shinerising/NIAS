using System.IO;
using System.Text;
using System.Text.Json;

namespace NIASReport
{
    public class ReportManager
    {
        public event ErrorEventHandler? ErrorHandler;

        private readonly ReportRecorder recorder;
        private readonly ReportGenerator generator;
        private readonly DatabaseHelper dbHelper;
        public ReportManager(string directory, string template, string location, int triggerTime)
        {
            dbHelper = DatabaseHelper.Initialize("raw.sqlite");
            dbHelper.ErrorHandler += HandleError;

            recorder = new ReportRecorder();
            recorder.ErrorHandler += HandleError;

            generator = new ReportGenerator(directory, template, location, triggerTime);
            generator.ErrorHandler += HandleError;
        }

        public async Task Initialize()
        {
            await dbHelper.OpenDatabase();

            recorder.Start();
            //generator.Start();
        }

        private void HandleError(object sender, ErrorEventArgs e)
        {
            ErrorHandler?.Invoke(sender, e);
        }

        public async Task Close()
        {
            await dbHelper.CloseDatabase();
        }

        public void Dispose()
        {
            dbHelper.ErrorHandler -= HandleError;
            recorder.ErrorHandler -= HandleError;
            generator.ErrorHandler -= HandleError;

            recorder.Dispose();
            generator.Dispose();

            dbHelper.Dispose();
        }

        public void AddData<T>(IEnumerable<T> list)
        {
            recorder.AddData(list);
        }
        public void UpdateInfo<T>(IEnumerable<T> list)
        {
            recorder.UpdateData(list);
        }
        public void GenerateReport()
        {
            generator.IsGenerateRequested = true;
        }
    }
}