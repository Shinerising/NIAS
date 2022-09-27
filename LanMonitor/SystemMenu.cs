﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace LanMonitor
{
    internal static class SystemMenu
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

        [DllImport("user32.dll")]
        static extern int GetMenuItemCount(IntPtr hMenu);
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool InsertMenu(IntPtr hmenu, int position, uint flags, uint item_id, [MarshalAs(UnmanagedType.LPWStr)] string item_text);

        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        public static void ApplyCustomMenuItems(IntPtr handle)
        {
            IntPtr systemMenu = GetSystemMenu(handle, false);
            int count = GetMenuItemCount(systemMenu);
            InsertMenu(systemMenu, count - 1, MF_BYPOSITION | MF_SEPARATOR, 0, string.Empty);
            InsertMenu(systemMenu, count - 1, MF_BYPOSITION | MF_STRING | MF_DISABLED, COMMAND_ABOUT, AppResource.GetString("Menu_Option"));
            InsertMenu(systemMenu, count - 0, MF_BYPOSITION | MF_STRING, COMMAND_HELP, AppResource.GetString("Menu_Help"));
            InsertMenu(systemMenu, count + 1, MF_BYPOSITION | MF_STRING, COMMAND_ABOUT, AppResource.GetString("Menu_About"));
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