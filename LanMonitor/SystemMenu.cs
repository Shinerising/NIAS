using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace LanMonitor
{
    internal static partial class SystemMenu
    {
        private const uint WM_SYSCOMMAND = 0x112;

        private const uint WM_INITMENUPOPUP = 0x0117;

        private const uint MF_SEPARATOR = 0x800;

        private const uint MF_BYCOMMAND = 0x0;

        private const uint MF_BYPOSITION = 0x400;

        private const uint MF_STRING = 0x0;

        private const uint MF_ENABLED = 0x0;

        private const uint MF_DISABLED = 0x2;
        private const uint COMMAND_HELP = 101;
        private const uint COMMAND_ABOUT = 102;

        [LibraryImport("user32.dll")]
        private static partial int GetMenuItemCount(IntPtr hMenu);
        [LibraryImport("user32.dll")]
        private static partial IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

        [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool InsertMenu(IntPtr hmenu, int position, uint flags, uint item_id, [MarshalAs(UnmanagedType.LPTStr)] string item_text);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        public static void ApplyCustomMenuItems(IntPtr handle)
        {
            IntPtr systemMenu = GetSystemMenu(handle, false);
            int count = GetMenuItemCount(systemMenu);
            InsertMenu(systemMenu, count - 1, MF_BYPOSITION | MF_SEPARATOR, 0, string.Empty);
            InsertMenu(systemMenu, count - 1, MF_BYPOSITION | MF_STRING, COMMAND_HELP, AppResource.GetString("Menu_Help"));
            InsertMenu(systemMenu, count, MF_BYPOSITION | MF_STRING, COMMAND_ABOUT, AppResource.GetString("Menu_About"));
        }

        public static string HandleMenuCommand(int menuID)
        {
            switch ((uint)menuID)
            {
                case COMMAND_HELP: return "Menu_Help";
                case COMMAND_ABOUT: return "Menu_About";
                default:
                    break;
            }
            return string.Empty;
        }
    }
}
