using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using RyuaNerin;

namespace FFChat.Windows
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        static class NativeMethods
        {
            public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

            [DllImport("user32.dll")]
            public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

            [DllImport("user32.dll")]
            public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

            [DllImport("user32.dll")]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

            public const int EVENT_SYSTEM_FOREGROUND = 3;
            public const int WINEVENT_OUTOFCONTEXT   = 0;
            public const int WINEVENT_SKIPOWNPROCESS = 2;

            public const int HWND_TOPMOST = -1;
            public const int SWP_NOMOVE = 2;
            public const int SWP_NOSIZE = 1;
        }

        public static MainWindow Instance { get; private set; }

        private readonly NativeMethods.WinEventDelegate m_hookProc;
        private readonly IntPtr m_handle;
        private IntPtr m_hookHwnd;

        public MainWindow()
        {
            Instance = this;

            InitializeComponent();

            var helper = new WindowInteropHelper(this);
            helper.EnsureHandle();
            this.m_handle = helper.Handle;
            
            this.m_hookProc = new NativeMethods.WinEventDelegate(this.WinEventProc);

            this.ctlList.ItemsSource = Worker.ChatLog;
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (idObject != 0 || idChild != 0)
                return;

            NativeMethods.SetWindowPos(this.m_handle, new IntPtr(NativeMethods.HWND_TOPMOST), 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Worker.Start();

            Task.Factory.StartNew(() => {
                var newVer = LastestRelease.CheckNewVersion("RyuaNerin", "ffchat");
                if (newVer != null)
                    this.Dispatcher.Invoke(new Action(() => {
                        MessageBox.Show("새 버전이 출시되었어요!", "파판챗");
                        Process.Start(new ProcessStartInfo { UseShellExecute = true, FileName = string.Format("\"{0}\"", newVer) }).Dispose();
                        Application.Current.Shutdown();
                    }));
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NativeMethods.UnhookWinEvent(this.m_hookHwnd);
            Worker.Stop();
        }

        private bool m_isBottom = false;
        private void ctlScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.m_isBottom = this.ctlScroll.VerticalOffset == this.ctlScroll.ScrollableHeight;
        }

        public void ScrollToBottom()
        {
            if (this.m_isBottom)
                this.ctlScroll.ScrollToBottom();
        }

        private void ctlTopMost_Checked(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            this.m_hookHwnd = NativeMethods.SetWinEventHook(NativeMethods.EVENT_SYSTEM_FOREGROUND, NativeMethods.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, this.m_hookProc, 0, 0, NativeMethods.WINEVENT_OUTOFCONTEXT | NativeMethods.WINEVENT_SKIPOWNPROCESS);
        }

        private void ctlTopMost_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
            NativeMethods.UnhookWinEvent(this.m_hookHwnd);
        }

        private void ctlSetting_Click(object sender, RoutedEventArgs e)
        {
            Worker.SettingWindow.Show();
        }

        private void ctlSaveText_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "파판챗";
            dlg.DefaultExt = ".txt";
            dlg.Filter = "텍스트 파일|*.txt"; // Filter files by extension

            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                string filename = dlg.FileName;

                lock (Worker.ChatLog)
                {
                    using (var file = new FileStream(dlg.FileName, FileMode.OpenOrCreate))
                    {
                        var writer = new StreamWriter(file, Encoding.UTF8);
                        for (int i = 0; i < Worker.ChatLog.Count; ++i)
                            writer.WriteLine(Worker.ChatLog[i].ChatBody);

                        writer.Flush();
                        file.Flush();
                    }
                }
            }
        }

        private void ctlChatClear_Click(object sender, RoutedEventArgs e)
        {
            lock (Worker.ChatLog)
                Worker.ChatLog.Clear();
        }
    }
}
