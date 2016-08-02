using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

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
        private readonly IntPtr m_hookHwnd;

        private readonly IntPtr m_handle;

        private ScrollViewer m_scrollViewer;

        public MainWindow()
        {
            Instance = this;

            InitializeComponent();

            var helper = new WindowInteropHelper(this);
            helper.EnsureHandle();
            this.m_handle = helper.Handle;
            
            this.m_hookProc = new NativeMethods.WinEventDelegate(this.WinEventProc);
            this.m_hookHwnd = NativeMethods.SetWinEventHook(NativeMethods.EVENT_SYSTEM_FOREGROUND, NativeMethods.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, this.m_hookProc, 0, 0, NativeMethods.WINEVENT_OUTOFCONTEXT | NativeMethods.WINEVENT_SKIPOWNPROCESS);

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
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NativeMethods.UnhookWinEvent(this.m_hookHwnd);
            Worker.Stop();
        }

        private void ctlList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
                Worker.SettingWindow.Show();
        }

        private bool m_isBottom = false;
        private void ctlScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.m_isBottom = this.ctlScroll.VerticalOffset == this.ctlScroll.ScrollableHeight;
        }

        public void ScrollToBottom()
        {
            if (m_isBottom)
                this.ctlScroll.ScrollToBottom();
        }
    }
}
