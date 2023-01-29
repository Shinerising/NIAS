using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace LanMonitor
{
    public class Options
    {
        public class Pair
        {
            public string Name { get; set; }
            public string Data { get; set; }
            public Pair(string name, string data)
            {
                Name = name;
                Data = data;
            }
        }
        public bool IsSwitchMonitorEnabled { get; set; } = true;
        public bool IsSwitchPingEnabled { get; set; } = true;
        public string SwitchUserName { get; set; } = "adminnms02";

        public bool IsNmapPingScan { get; set; } = false;
        public bool IsNmapFullScan { get; set; } = false;
        public bool IsNmapFastScan { get; set; } = false;
        public string NmapTarget { get; set; } = "172.16.24.*";

        public string ReportFolder { get; set; } = @"D:\";
        public string ReportLocation { get; set; } = "工作地点";
        public int ReportTime { get; set; } = 480;

        public ObservableCollection<Pair> SwitchList { get; set; } = new ObservableCollection<Pair>(new Dictionary<string, string>()
        {
            { "Switch01", "172.16.24.1" },
            { "Switch02", "172.16.24.2" },
        }.Select(item => new Pair(item.Key, item.Value)));
        public ObservableCollection<Pair> HostList { get; set; } = new ObservableCollection<Pair>(new Dictionary<string, string>()
        {
            { "CRSCD-Test", "172.16.24.208;172.16.24.209" },
            { "CRSCD-HostA", "172.16.24.74|00:0E:0C:5A:FE:D6;172.16.24.74|00:0E:0C:5A:FE:D7" },
            { "CRSCD-HostB", "172.16.24.75" },
            { "TWSVR-A", "172.16.24.90" },
            { "TWSVR-B", "172.16.24.82" },
        }.Select(item => new Pair(item.Key, item.Value)));
        public ObservableCollection<Pair> ConnectionList { get; set; } = new ObservableCollection<Pair>(new Dictionary<string, string>()
        {
            { "conn-A", "Switch01;Switch02" }
        }.Select(item => new Pair(item.Key, item.Value)));
    }
}
