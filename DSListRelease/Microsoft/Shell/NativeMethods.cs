namespace Microsoft.Shell
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Collections.Generic;

    [SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {
        [DllImport("shell32.dll", EntryPoint = "CommandLineToArgvW", CharSet = CharSet.Unicode)]
        private static extern IntPtr _CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string cmdLine, out int numArgs);
        [DllImport("kernel32.dll", EntryPoint = "LocalFree", SetLastError = true)]
        private static extern IntPtr _LocalFree(IntPtr hMem);
        public static string[] CommandLineToArgvW(string cmdLine)
        {
            string[] strArray2;
            IntPtr zero = IntPtr.Zero;
            try
            {
                int numArgs = 0;
                zero = _CommandLineToArgvW(cmdLine, out numArgs);
                if (zero == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }
                string[] strArray = new string[numArgs];
                for (int i = 0; i < numArgs; i++)
                {
                    IntPtr ptr = Marshal.ReadIntPtr(zero, i * Marshal.SizeOf(typeof(IntPtr)));
                    strArray[i] = Marshal.PtrToStringUni(ptr);
                }
                strArray2 = strArray;
            }
            finally
            {
                IntPtr ptr3 = _LocalFree(zero);
            }
            return strArray2;
        }

        public delegate IntPtr MessageHandler(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled);
    }
}

