namespace NIASReport
{
    public class RawData
    {
        public enum ImpactLevel
        {
            Unknown,
            Idle,
            Normal,
            Warning,
            Error,
            Fatal
        }
        public abstract class TimeData<T> where T : class
        {
            public abstract long Time { get; set; }
            public abstract T Combine(IEnumerable<T> list);
        }
        [Serializable]
        public class SwitchInfo
        {
            public int ID { get; set; } = -1;
            public string Name { get; set; } = "";
            public string Address { get; set; } = "";
            public string MACAddress { get; set; } = "";
            public string Vendor { get; set; } = "";
        }
        [Serializable]
        public class Switch : TimeData<Switch>
        {
            public override long Time { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
            public int SwitchID { get; set; } = -1;
            public int State { get; set; } = 0;
            public float CPU { get; set; } = 0;
            public float REM { get; set; } = 0;
            public float TEM { get; set; } = 0;
            public string Port { get; set; } = "";
            public string PortInSpeed { get; set; } = "";
            public string PortOutSpeed { get; set; } = "";
            public override Switch Combine(IEnumerable<Switch> list)
            {
                return list.First();
            }
        }
        [Serializable]
        public class HostInfo
        {
            public int ID { get; set; } = -1;
            public string Name { get; set; } = "";
        }
        [Serializable]
        public class AdapterInfo
        {
            public int ID { get; set; } = -1;
            public int HostID { get; set; } = -1;
            public string Address { get; set; } = "";
            public string MACAddress { get; set; } = "";
            public string Vendor { get; set; } = "";
        }
        [Serializable]
        public class Adapter : TimeData<Adapter>
        {
            public override long Time { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
            public int HostID { get; set; } = -1;
            public int AdapterID { get; set; } = -1;
            public int State { get; set; } = 0;
            public float Latency { get; set; } = 0;
            public float InSpeed { get; set; } = 0;
            public float OutSpeed { get; set; } = 0;
            public override Adapter Combine(IEnumerable<Adapter> list)
            {
                return list.First();
            }
        }
        [Serializable]
        public class Connection : TimeData<Connection>
        {
            public override long Time { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
            public int Type { get; set; } = 0;
            public int Source { get; set; } = -1;
            public int Target { get; set; } = -1;
            public int AdapterID { get; set; } = -1;
            public int PortID { get; set; } = -1;
            public int State { get; set; } = 0;
            public override Connection Combine(IEnumerable<Connection> list)
            {
                return list.First();
            }
        }
        [Serializable]
        public class DeviceInfo
        {
            public string Name { get; set; } = "";
            public string Address { get; set; } = "";
            public string MACAddress { get; set; } = "";
            public string Vendor { get; set; } = "";
            public string OS { get; set; } = "";
            public int PortCount { get; set; } = 0;
            public int WarningCount { get; set; } = 0;
        }

        [Serializable]
        public class Log : TimeData<Log>
        {
            public override long Time { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
            public string Name { get; set; } = "";
            public string Text { get; set; } = "";

            public override Log Combine(IEnumerable<Log> list)
            {
                return list.First();
            }
        }

        [Serializable]
        public class Alarm : TimeData<Alarm>
        {
            public override long Time { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
            public string Name { get; set; } = "";
            public string Text { get; set; } = "";

            public override Alarm Combine(IEnumerable<Alarm> list)
            {
                return list.First();
            }
        }
    }
}