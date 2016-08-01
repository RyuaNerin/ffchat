using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using App;

namespace FFChat
{
    public partial class frmMain : Form
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

        private const int MaxLog = 256;

        private static frmMain m_instance;
        public static frmMain Instance { get { return frmMain.m_instance; } }

        private readonly frmSetting m_setting;

        private readonly bool[] m_chatTypes = { false, false, false, true, true, true, true, true, true, true, true, true, true, true, true };

        private readonly Network m_network;
        private Process m_ffxiv;
        
        private readonly NativeMethods.WinEventDelegate m_hookProc;
        private readonly IntPtr m_hookHwnd;

        public frmMain()
        {
            frmMain.m_instance = this;

            InitializeComponent();

            this.m_setting = new frmSetting(this);
            this.m_network = new Network();
            
            this.m_hookProc = new NativeMethods.WinEventDelegate(this.WinEventProc);
            this.m_hookHwnd = NativeMethods.SetWinEventHook(NativeMethods.EVENT_SYSTEM_FOREGROUND, NativeMethods.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, this.m_hookProc, 0, 0, NativeMethods.WINEVENT_OUTOFCONTEXT | NativeMethods.WINEVENT_SKIPOWNPROCESS);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            NativeMethods.UnhookWinEvent(this.m_hookHwnd);

            this.m_network.StopCapture();
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (idObject != 0 || idChild != 0)
                return;

            NativeMethods.SetWindowPos(this.Handle, new IntPtr(NativeMethods.HWND_TOPMOST), 0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
        }


        private void lstChat_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                this.m_setting.Show(this);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.FindFFXIVProcess();

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(30 * 1000);

                    if ((this.m_ffxiv == null) || this.m_ffxiv.HasExited)
                    {
                        this.m_ffxiv = null;

                        this.Invoke(new Action(this.FindFFXIVProcess));
                    }
                    else
                    {
                        // FFXIVProcess is alive

                        if (this.m_network.IsRunning)
                        {
                            this.m_network.UpdateGameConnections(this.m_ffxiv);
                        }
                        else
                        {
                            this.m_network.StartCapture(this.m_ffxiv);
                        }
                    }
                }
            });
        }

        public void FindFFXIVProcess()
        {
            this.lsb.Items.Clear();

            this.m_setting.comboBox_Process.Items.Clear();
            Console.WriteLine("파이널판타지14 프로세스를 찾는 중...");

            var processes = new List<Process>();
            processes.AddRange(Process.GetProcessesByName("ffxiv"));
            processes.AddRange(Process.GetProcessesByName("ffxiv_dx11"));

            if (processes.Count == 0)
            {
                Console.WriteLine("파이널판타지14 프로세스를 찾을 수 없습니다");
                this.m_setting.button_SelectProcess.Enabled = false;
                this.m_setting.comboBox_Process.Enabled = false;
            }
            else if (processes.Count >= 2)
            {
                Console.WriteLine("파이널판타지14가 2개 이상 실행중입니다");
                this.m_setting.button_SelectProcess.Enabled = true;
                this.m_setting.comboBox_Process.Enabled = true;

                foreach (var process in processes)
                    using (process)
                        this.m_setting.comboBox_Process.Items.Add(string.Format("{0}:{1}", process.ProcessName, process.Id));

                this.m_setting.comboBox_Process.SelectedIndex = 0;
            }
            else
            {
                SetFFXIVProcess(processes[0]);
            }
        }

        public void SetFFXIVProcess(Process process)
        {
            if (this.m_ffxiv != null)
                this.m_ffxiv.Dispose();

            try
            {
                this.m_ffxiv = process;

                var name = string.Format("{0}:{1}", this.m_ffxiv.ProcessName, this.m_ffxiv.Id);
                Console.WriteLine("파이널판타지14 프로세스가 선택되었습니다: {0}", name);

                this.m_setting.comboBox_Process.Enabled = false;
                this.m_setting.button_SelectProcess.Enabled = false;

                this.m_setting.comboBox_Process.Items.Clear();
                this.m_setting.comboBox_Process.Items.Add(name);
                this.m_setting.comboBox_Process.SelectedIndex = 0;

                this.m_network.StartCapture(this.m_ffxiv);
            }
            catch
            {
            }
        }

        public void ResetProcess()
        {
            try
            {
                this.lsb.Items.Clear();

                this.m_network.StopCapture();
                this.m_ffxiv = null;
                FindFFXIVProcess();
            }
            catch
            {
            }
        }

        public void SelectProcess(int pid)
        {
            try
            {
                this.lsb.Items.Clear();

                SetFFXIVProcess(Process.GetProcessById(int.Parse(((string)this.m_setting.comboBox_Process.SelectedItem).Split(':')[1])));
            }
            catch
            {
                Console.WriteLine("파이널판타지14 프로세스 설정에 실패했습니다");
            }
        }

        public void SetLog(int index, bool value)
        {
            this.m_chatTypes[index] = value;
        }

        int lastIndex;
        public void AddChat(int index, string chatString)
        {
//             if (!this.m_chatTypes[index])
//                 return;

            if (this.InvokeRequired)
                this.Invoke(new Action<int, string>(this.AddChat), index, chatString);
            else
            {
                while (this.lsb.Items.Count >= MaxLog)
                    this.lsb.Items.RemoveAt(0);

                var bottom = this.lsb.TopIndex > this.lsb.Items.Count - (int)Math.Floor((double)this.lsb.Height / this.lsb.ItemHeight);

                this.lsb.Items.Add(chatString);

                this.lsb.TopIndex = this.lsb.Items.Count;
            }
        }
    }
}
