using Microsoft.VisualBasic;
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

        private const uint MF_SEPARATOR = 0x800;
        private const uint MF_BYPOSITION = 0x400;
        private const uint MF_STRING = 0x0;

        private const uint COMMAND_HELP = 101;
        private const uint COMMAND_ABOUT = 102;
        private const uint COMMAND_OPTION = 103;

        [LibraryImport("user32.dll")]
        private static partial int GetMenuItemCount(IntPtr hMenu);
        [LibraryImport("user32.dll")]
        private static partial IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

        [LibraryImport("user32.dll", EntryPoint = "InsertMenuW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool InsertMenu(IntPtr hmenu, int position, uint flags, uint item_id, [MarshalAs(UnmanagedType.LPWStr)] string item_text);

        public static void ApplyCustomMenuItems(IntPtr handle)
        {
            IntPtr systemMenu = GetSystemMenu(handle, false);
            int count = GetMenuItemCount(systemMenu);
            InsertMenu(systemMenu, count - 1, MF_BYPOSITION | MF_SEPARATOR, 0, string.Empty);
            InsertMenu(systemMenu, count - 1, MF_BYPOSITION | MF_STRING, COMMAND_OPTION, string.Format("{0}{1}{2}", AppResource.GetString("Menu_Option"), ControlChars.Tab, "Alt + O"));
            InsertMenu(systemMenu, count - 0, MF_BYPOSITION | MF_STRING, COMMAND_HELP, string.Format("{0}{1}{2}", AppResource.GetString("Menu_Help"), ControlChars.Tab, "Alt + H"));
            InsertMenu(systemMenu, count + 1, MF_BYPOSITION | MF_STRING, COMMAND_ABOUT, string.Format("{0}{1}{2}", AppResource.GetString("Menu_About"), ControlChars.Tab, "Alt + A"));
        }

        public static string HandleMenuCommand(int menuID)
        {
            return (uint)menuID switch
            {
                COMMAND_HELP => "Menu_Help",
                COMMAND_ABOUT => "Menu_About",
                _ => string.Empty,
            };
        }
    }
}
