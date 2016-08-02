using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace RyuaNerin
{
    static class ErrorTrace
    {
        private static readonly string ExeDir;
        private static readonly string Version;

        static ErrorTrace()
        {
            System.AppDomain.CurrentDomain.UnhandledException               += (s, e) => ShowCrashReport((Exception)e.ExceptionObject);
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException    += (s, e) => ShowCrashReport(e.Exception);
            System.Windows.Application.Current.DispatcherUnhandledException += (s, e) => ShowCrashReport(e.Exception);

            var asm = Assembly.GetExecutingAssembly();
            Version = asm.GetName().Version.ToString();
            ExeDir  = Path.GetDirectoryName(Path.GetFullPath(asm.Location));
        }

        public static void Load()
        {
        }

        private static void ShowCrashReport(Exception exception)
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var file = string.Format("Crash-{0}.txt", date);

            using (var writer = new StreamWriter(Path.Combine(ExeDir, file)))
            {
                writer.WriteLine("Decchi Crash Report");
                writer.WriteLine("Date    : " + date);
                writer.WriteLine("Version : " + Version);
                writer.WriteLine();
                writer.WriteLine("OS Ver  : " + GetOSInfomation());
                writer.WriteLine("SPack   : " + NativeMethods.GetOSServicePack());
                writer.WriteLine();
                writer.WriteLine("Exception");
                writer.WriteLine(exception.ToString());
            }
        }

        private static string GetOSInfomation()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion"))
                    return key.GetValue("ProductName").ToString();
            }
            catch
            {
                return "Operating System Information unavailable";
            }
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.Dll")]
            private static extern short GetVersionEx(ref OsVersioninfo o);

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct OsVersioninfo
            {
                public int      dwOSVersionInfoSize;
                public int      dwMajorVersion;
                public int      dwMinorVersion;
                public int      dwBuildNumber;
                public int      dwPlatformId;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
                public string   szCSDVersion;
                public ushort   wServicePackMajor;
                public ushort   wServicePackMinor;
                public ushort   wSuiteMask;
                public byte     wProductType;
                public byte     wReserved;
            }

            public static string GetOSServicePack()
            {
                try
                {
                    var os = new OsVersioninfo();
                    os.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OsVersioninfo));
                    GetVersionEx(ref os);

                    if (!string.IsNullOrWhiteSpace(os.szCSDVersion))
                        return os.szCSDVersion.Trim();
                }
                catch
                { }

                return "?";
            }
        }
    }
}
