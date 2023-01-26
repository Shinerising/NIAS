namespace NIASReport
{
    public class ReportFileInfo
    {
        public string Name { get; set; }
        public string Directory { get; set; }
        public DateTime CreateTime { get; set; }
        public ReportFileInfo(FileInfo info)
        {
            Name = info.Name;
            Directory = info.Directory?.Name ?? "";
            CreateTime = info.CreationTime;
        }
    }
    public class ReportManager
    {
        public event ErrorEventHandler? ErrorHandler;

        private const int ExpiredDays = 32;
        private readonly string FileDirectory;
        private readonly ReportRecorder recorder;
        private readonly ReportGenerator generator;
        private readonly DatabaseHelper dbHelper;
        public ReportManager(string directory, string template, string location, int triggerTime)
        {
            FileDirectory = directory;

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
            generator.Start();
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
        public static List<ReportFileInfo> GetFileList(string directory)
        {
            return Directory.GetFiles(directory, "NetworkReport*.html").Select(item => new ReportFileInfo(new FileInfo(item))).ToList();
        }
        public List<ReportFileInfo> GetFileList()
        {
            return ReportManager.GetFileList(FileDirectory);
        }
        public void DeleteExpiredFiles()
        {
            try
            {
                foreach (var file in Directory.GetFiles(FileDirectory, "NetworkReport*.html"))
                {
                    var info = new FileInfo(file);
                    if (DateTime.Now - info.CreationTime > TimeSpan.FromDays(ExpiredDays))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception e)
            {
                HandleError(this, new ErrorEventArgs(e));
            }
        }
    }
}