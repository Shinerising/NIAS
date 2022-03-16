using System.Windows;

namespace LanMonitor
{
    public static class AppResource
    {
        public enum StringKey
        {
            Title,
            UploadSpeed,
            DownloadSpeed,
            Connected,
            Disconnected,
            Dynamic,
            Static,
            Other,
            SwitchCascading,
            Online,
            Offline,
            Reserve,
            UnknownIP,
            Unknown,
            UnknownIPAddress,
            TimeSpan,
            Port_Default,
            Port_Established,
            Port_Listening,
            Port_CloseWait,
            Port_TimeWait,
            Port_SynSent,
            Text_Graph,
            Row_Adapter,
            Row_Port,
            Row_LAN,
            Row_SwitchPort,
            Row_SwitchHost,
            Row_NetworkStructure,
            Tip_ComputerName,
            Tip_SystemName,
            Tip_DomainName,
            Tip_WorkGroup,
            Tip_Model,
            Tip_Graph,
            Tip_User,
            Tip_Port,
            Tip_Latency,
            Tip_Computer,
            Tip_ActivePort,
            Tip_NetworkDevice,
            Tip_SwitchPort,
            Tip_SwitchHost,
            Tip_SwitchDevice,
            Tip_Adapter,
            Tip_SwitchConnection,
            Status_Offline,
            Status_LAN,
            Status_Internet,
            Message_Title,
            Message_SNMPWarning,
            Message_SwitchReconnect,
            Message_SwitchDisconnect,
            Message_HostReconnect,
            Message_HostDisconnect,
            Message_LineReconnect,
            Message_LineDisconnect
        }
        public static string GetString(string key)
        {
            try
            {
                return Application.Current?.FindResource(key).ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string GetString(StringKey key)
        {
            try
            {
                return Application.Current?.FindResource(key.ToString()).ToString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
