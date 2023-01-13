using System.Runtime.Intrinsics.Arm;
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
        public List<ReportSwitchInfo>? SwitchInfo { get; set; }
        public List<ReportHostInfo>? HostInfo { get; set; }
        public List<ReportDeviceInfo>? DeviceInfo { get; set; }
        public List<ReportSwitch>? Switch { get; set; }
        public List<ReportHost>? Host { get; set; }
        public List<ReportLog>? Log { get; set; }
        public List<ReportAlarm>? Alarm { get; set; }
        public ReportConnection? Connection { get; set; }
    }

    public class ReportScore
    {
        
    }

    public class ReportStats
    {

    }
    public class ReportSwitchInfo
    {
        public int? ID { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? MACAddress { get; set; }
        public string? Vendor { get; set; }
    }

    public class ReportSwitch
    {
        public int ID { get; set; }
        public List<long> Time { get; set; }
        public List<int> State { get; set; }
        public List<float> CPU { get; set; }
        public List<float> REM { get; set; }
        public List<float> TEM { get; set; }
        public List<string> Port { get; set; }
        public List<string> PortInSpeed { get; set; }
        public List<string> PortOutSpeed { get; set; }
        public ReportSwitch(int id)
        {
            ID = id;
            Time = new List<long>();
            State = new List<int>();
            CPU = new List<float>();
            REM = new List<float>();
            TEM = new List<float>();
            Port = new List<string>();
            PortInSpeed = new List<string>();
            PortOutSpeed = new List<string>();
        }
    }
    public class ReportHostInfo
    {
        public int? ID { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? MACAddress { get; set; }
        public string? Vendor { get; set; }
    }

    public class ReportHost
    {
        public int ID { get; set; }
        public List<long> Time { get; set; }
        public List<int> State { get; set; }
        public List<float> Latency { get; set; }
        public List<float> InSpeed { get; set; }
        public List<float> OutSpeed { get; set; }
        public ReportHost(int id)
        {
            ID = id;
            Time = new List<long>();
            State = new List<int>();
            Latency = new List<float>();
            InSpeed = new List<float>();
            OutSpeed = new List<float>();
        }
    }

    public class ReportConnection
    {
        public List<long> Time { get; set; }
        public List<int> State { get; set; }
        public List<string> Line { get; set; }
        public ReportConnection()
        {
            Time = new List<long>();
            State = new List<int>();
            Line = new List<string>();
        }
    }
    public class ReportDeviceInfo
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? MACAddress { get; set; }
        public string? Vendor { get; set; }
        public string? OS { get; set; }
        public int? PortCount { get; set; }
        public int? WarningCount { get; set; }
    }

    public class ReportLog
    {
        public long? Time { get; set; } 
        public string? Name { get; set; }
        public string? Text { get; set; }
    }
    public class ReportAlarm
    {
        public long? Time { get; set; }
        public string? Name { get; set; }
        public string? Text { get; set; }
    }
}