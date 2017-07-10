using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace DSList
{
    internal static class NativeMethods
    {
        private const uint INFINITE = uint.MaxValue;
        private const int LOGON_NETCREDENTIALS_ONLY = 2;
        private const int LOGON_WITH_PROFILE = 1;
        public const int MOD_ALT = 1;
        public const int MOD_CONTROL = 2;
        public const int MOD_SHIFT = 4;
        public const int MOD_WIN = 8;
        private const int STARTF_USESHOWWINDOW = 1;
        private const int SW_HIDE = 0;
        public const uint VK_KEY_A = 0x41;
        public const uint VK_KEY_W = 0x57;
        public const uint VK_KEY_X = 0x58;
        public const uint VK_KEY_Z = 90;
        public const uint VK_OEM_3 = 0xc0;
        public const uint VK_PAUSE = 0x13;
        private const uint WAIT_FAILED = uint.MaxValue;
        public const int WM_HOTKEY = 0x312;

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);
        public static int CreateHiddenProcessAs(string strCommand, bool Hidden, string strName, string strPassword)
        {
            PROCESS_INFORMATION processInfo = new PROCESS_INFORMATION();
            STARTUPINFO structure = new STARTUPINFO();
            uint maxValue = uint.MaxValue;
            uint exitCode = 0;
            try
            {
                structure.cb = Marshal.SizeOf(structure);
                if (Hidden)
                {
                    structure.dwFlags = 1;
                    structure.wShowWindow = 0;
                }
                if (!CreateProcessWithLogonW(strName, "", strPassword, 1, null, strCommand, 0, IntPtr.Zero, null, ref structure, out processInfo))
                {
                    return Marshal.GetLastWin32Error();
                }
                maxValue = WaitForSingleObject(processInfo.hProcess, uint.MaxValue);
                GetExitCodeProcess(processInfo.hProcess, ref exitCode);
                if (maxValue == uint.MaxValue)
                {
                    return Marshal.GetLastWin32Error();
                }
            }
            finally
            {
                CloseHandle(processInfo.hProcess);
                CloseHandle(processInfo.hThread);
            }
            return (int)exitCode;
        }

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CreateProcessWithLogonW(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonFlags, string applicationName, string commandLine, int creationFlags, IntPtr environment, string currentDirectory, ref STARTUPINFO sui, out PROCESS_INFORMATION processInfo);
        public static int CreateProcessWithNetOnlyCredentials(string path, string arguments, string login, string domain, string password, bool Hidden = false, bool wait = false)
        {
            uint maxValue = uint.MaxValue;
            uint exitCode = 0;
            STARTUPINFO structure = new STARTUPINFO();
            structure.cb = Marshal.SizeOf(structure);
            if (Hidden)
            {
                structure.dwFlags = 1;
                structure.wShowWindow = 0;
            }
            if (!string.IsNullOrWhiteSpace(arguments))
            {
                path = path + " " + arguments;
            }
            PROCESS_INFORMATION processInfo = new PROCESS_INFORMATION();
            if (!CreateProcessWithLogonW(login, domain, password, 2, null, path, 0, IntPtr.Zero, Environment.CurrentDirectory, ref structure, out processInfo))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            if (wait)
            {
                maxValue = WaitForSingleObject(processInfo.hProcess, uint.MaxValue);
                GetExitCodeProcess(processInfo.hProcess, ref exitCode);
                if (maxValue == uint.MaxValue)
                {
                    return Marshal.GetLastWin32Error();
                }
                return (int)exitCode;
            }
            return 0;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeProcess(IntPtr process, ref uint exitCode);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetPrivateProfileString(string sSection, string sKey, string sDefault, StringBuilder sString, int iSize, string sFile);
        [DllImport("kernel32", SetLastError = true)]
        public static extern short GlobalAddAtom(string lpString);
        [DllImport("kernel32", SetLastError = true)]
        public static extern short GlobalDeleteAtom(short nAtom);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("kernel32.dll")]
        private static extern uint ResumeThread(IntPtr hThread);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }
    }
}
