using static NIASReport.RawData;

namespace NIASReport
{
    public class ReportData
    {
        public string? Title { get; set; }
        public string? User { get; set; }
        public string? Location { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ReportSwitch? Switch { get; set; }
        public ReportHost? Host { get; set; }
        public ReportConnection? Connection { get; set; }
    }

    public class ReportScore
    {
        
    }

    public class ReportStats
    {

    }

    public class ReportSwitch
    {
        public long[]? Time { get; set; }
        public int[]? State { get; set; }
        public float[]? CPU { get; set; }
        public float[]? REM { get; set; }
        public float[]? TEM { get; set; }
        public string[]? Port { get; set; }
        public string[]? PortInSpeed { get; set; }
        public string[]? PortOutSpeed { get; set; }
    }

    public class ReportHost
    {
        public long[]? Time { get; set; }
        public int[]? State { get; set; }
        public float[]? Latency { get; set; }
        public float[]? InSpeed { get; set; }
        public float[]? OutSpeed { get; set; }
    }

    public class ReportConnection
    {
        public long[]? Time { get; set; }
        public int[]? State { get; set; }
        public string[]? Line { get; set; }
    }

    public class ReportLog{

    }
}