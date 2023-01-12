namespace NIASReport
{
    public class RawData
    {
        [Serializable]
        public class SwitchInfo
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string MACAddress { get; set; }
            public string Vendor { get; set; }
        }
        [Serializable]
        public class Switch
        {
            public int SwitchID { get; set; }
            public DateTime Time { get; set; } = DateTime.Now;
            public int State { get; set; }
            public float CPU { get; set; }
            public float REM { get; set; }
            public float TEM { get; set; }
            public string Port { get; set; }
            public string PortInSpeed { get; set; }
            public string PortOutSpeed { get; set; }
        }
        [Serializable]
        public class HostInfo
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }
        [Serializable]
        public class AdapterInfo
        {
            public int ID { get; set; }
            public int HostID { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string MACAddress { get; set; }
            public string Vendor { get; set; }
        }
        [Serializable]
        public class Adapter
        {
            public int HostID { get; set; }
            public int AdapterID { get; set; }
            public DateTime Time { get; set; } = DateTime.Now;
            public string Host { get; set; }
            public float Latency { get; set; }
            public float InSpeed { get; set; }
            public float OutSpeed { get; set; }
        }
        [Serializable]
        public class Connection
        {
            public DateTime Time { get; set; } = DateTime.Now;
            public int Type { get; set; }
            public int Source { get; set; }
            public int Target { get; set; }
            public int State { get; set; }
        }
        [Serializable]
        public class DeviceInfo
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string MACAddress { get; set; }
            public string Vendor { get; set; }
            public int Port { get; set; }
            public int Warning { get; set; }
        }
    }
}