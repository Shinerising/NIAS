export interface ReportData {
  Title: string | null;
  User: string | null;
  Location: string | null;
  CreateTime: Date | null;
  StartTime: Date | null;
  EndTime: Date | null;
  SwitchInfo: ReportSwitchInfo[] | null;
  HostInfo: ReportHostInfo[] | null;
  DeviceInfo: ReportDeviceInfo[] | null;
  Switch: ReportSwitch[] | null;
  Host: ReportHost[] | null;
  Log: ReportLog[] | null;
  Alarm: ReportAlarm[] | null;
  Connection: ReportConnection | null;
}

export interface ReportSwitchInfo {
  ID: number | null;
  Name: string | null;
  Address: string | null;
  MACAddress: string | null;
  Vendor: string | null;
}

export interface ReportSwitch {
  ID: number;
  Time: number[];
  State: number[];
  CPU: number[];
  REM: number[];
  TEM: number[];
  Port: string[];
  PortInSpeed: string[];
  PortOutSpeed: string[];
  PortTotalSpeed: string[];
}

export interface ReportHostInfo {
  ID: number | null;
  Name: string | null;
  Address: string | null;
  MACAddress: string | null;
  Vendor: string | null;
}

export interface ReportHost {
  ID: number;
  Time: number[];
  State: number[];
  Latency: number[];
  InSpeed: number[];
  OutSpeed: number[];
}

export interface ReportConnection {
  Time: number[];
  State: number[];
  Line: string[];
}

export interface ReportDeviceInfo {
  Name: string | null;
  Address: string | null;
  MACAddress: string | null;
  Vendor: string | null;
  OS: string | null;
  PortCount: number | null;
  WarningCount: number | null;
}

export interface ReportLog {
  Time: number | null;
  Name: string | null;
  Text: string | null;
}

export interface ReportAlarm {
  Time: number | null;
  Name: string | null;
  Text: string | null;
}
