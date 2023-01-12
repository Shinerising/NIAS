﻿namespace NIASReport
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
        public class Switch
        {
            public int SwitchID { get; set; } = -1;
            public long Time { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
            public int State { get; set; } = 0;
            public float CPU { get; set; } = 0;
            public float REM { get; set; } = 0;
            public float TEM { get; set; } = 0;
            public string Port { get; set; } = "";
            public string PortInSpeed { get; set; } = "";
            public string PortOutSpeed { get; set; } = "";
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
        public class Adapter
        {
            public int HostID { get; set; } = -1;
            public int AdapterID { get; set; } = -1;
            public long Time { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
            public float Latency { get; set; } = 0;
            public float InSpeed { get; set; } = 0;
            public float OutSpeed { get; set; } = 0;
        }
        [Serializable]
        public class Connection
        {
            public long Time { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
            public int Type { get; set; } = 0;
            public int Source { get; set; } = -1;
            public int Target { get; set; } = -1;
            public int AdapterID { get; set; } = -1;
            public int State { get; set; } = 0;
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
        public class Log
        {
            public long Time { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
            public string Name { get; set; } = "";
            public string Text { get; set; } = "";
        }

        [Serializable]
        public class Alarm
        {
            public long Time { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
            public string Name { get; set; } = "";
            public string Text { get; set; } = "";
        }
    }
}