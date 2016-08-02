﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using App;
using FFChat.Windows;
using System.Text.RegularExpressions;

namespace FFChat
{
    internal static class Worker
    {
        private const int MaxLog = 512;

        public static readonly ObservableCollection<string> ProcessList = new ObservableCollection<string>();
        public static readonly ObservableCollection<Chat> ChatLog = new ObservableCollection<Chat>();

        private static readonly bool[] ShowingTypes = { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };
        
        private static readonly Network m_network;
        private static Process m_ffxiv;

        public static readonly SettingWindow SettingWindow;

        static Worker()
        {
            m_network = new Network();
            SettingWindow = new SettingWindow() { Owner = MainWindow.Instance };
        }

        public static void Start()
        {
            FindFFXIVProcess();

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(30 * 1000);

                    if ((m_ffxiv == null) || m_ffxiv.HasExited)
                    {
                        m_ffxiv = null;

                        FFChatApp.Current.Dispatcher.Invoke(new Action(FindFFXIVProcess));
                    }
                    else
                    {
                        // FFXIVProcess is alive

                        if (m_network.IsRunning)
                            m_network.UpdateGameConnections(m_ffxiv);
                        else
                            m_network.StartCapture(m_ffxiv);
                    }
                }
            });
        }
        public static void Stop()
        {
            m_network.StopCapture();
        }

        public static void FindFFXIVProcess()
        {
            ProcessList.Clear();
            Console.WriteLine("파이널판타지14 프로세스를 찾는 중...");

            var processes = new List<Process>();
            processes.AddRange(Process.GetProcessesByName("ffxiv"));
            processes.AddRange(Process.GetProcessesByName("ffxiv_dx11"));

            if (processes.Count == 0)
            {
                Console.WriteLine("파이널판타지14 프로세스를 찾을 수 없습니다");
                SettingWindow.ctlSelect.IsEnabled = false;
                SettingWindow.ctlCombo.IsEnabled = false;
            }
            else if (processes.Count >= 2)
            {
                Console.WriteLine("파이널판타지14가 2개 이상 실행중입니다");
                SettingWindow.ctlSelect.IsEnabled = true;
                SettingWindow.ctlCombo.IsEnabled = true;

                foreach (var process in processes)
                    using (process)
                        ProcessList.Add(string.Format("{0}:{1}", process.ProcessName, process.Id));
                SettingWindow.ctlCombo.SelectedIndex = 0;
            }
            else
            {
                SetFFXIVProcess(processes[0]);
            }
        }

        private static void SetFFXIVProcess(Process process)
        {
            if (m_ffxiv != null)
                m_ffxiv.Dispose();

            try
            {
                m_ffxiv = process;

                var name = string.Format("{0}:{1}", m_ffxiv.ProcessName, m_ffxiv.Id);
                Console.WriteLine("파이널판타지14 프로세스가 선택되었습니다: {0}", name);

                SettingWindow.ctlSelect.IsEnabled = false;
                SettingWindow.ctlCombo.IsEnabled = false;

                ProcessList.Clear();
                ProcessList.Add(name);
                SettingWindow.ctlCombo.SelectedIndex = 0;

                m_network.StartCapture(m_ffxiv);
            }
            catch
            {
            }
        }

        public static void ResetProcess()
        {
            try
            {
                m_network.StopCapture();
                m_ffxiv = null;
                FindFFXIVProcess();
            }
            catch
            {
            }
        }

        public static void SelectProcess(int pid)
        {
            try
            {
                SetFFXIVProcess(Process.GetProcessById(pid));
            }
            catch
            {
                Console.WriteLine("파이널판타지14 프로세스 설정에 실패했습니다");
            }
        }
        
        private static string[] TypeNames =
        {
            "말하기",
            "떠들기",
            "외치기",
            "귓속말",
            "파티",
            "연합파티",
            "자유부대",
            "링크셸1",
            "링크셸2",
            "링크셸3",
            "링크셸4",
            "링크셸5",
            "링크셸6",
            "링크셸7",
            "링크셸8"
        };

        private const string Format_67   = "[{0:00}:{1:00}] {2}: {3}";
        private const string Format_64_S = "[{0:00}:{1:00}] >>{2}: {3}";
        private const string Format_64_R = "[{0:00}:{1:00}] {2} >> {3}";
        private const string Format_65   = "[{0:00}:{1:00}] [{4}] <{2}> {3}";

        public static void HandleMessage(byte[] message, bool sendMessage)
        {
            try
            {
                if (message.Length < 1000)
                    return;


                Console.WriteLine(BitConverter.ToString(message).Replace("-", " "));

                // 64 귓속말
                // 65 자유부대
                // 67 외치기

                var opcode = BitConverter.ToUInt16(message, 18);
                if (opcode == 0x64 || opcode == 0x65 || opcode == 0x67)
                {
                    int indexCode = message[32];

                    string fmt  = null;
                    string name = null;
                    string body = null;

                    int type = 0;

                    #region 말하기 떠들기 외치기
                    if (opcode == 0x67)
                    {
                        if (sendMessage)
                            indexCode = message[56];

                        switch (indexCode)
                        {
                            case 0x0A: type = 0; break; // 말하기
                            case 0x1E: type = 1; break; // 떠들기
                            case 0x0B: type = 2; break; // 외치기
                        }
                        if (!ShowingTypes[type]) return;

                        fmt  = Format_67;
                        if (sendMessage)
                        {
                            name = "-";
                            body = GetString(message, 58);
                        }
                        else
                        {
                            name = GetString(message, 52);
                            body = GetString(message, 84);
                        }
                    }
                    #endregion

                    #region 귓속말
                    else if (opcode == 0x64)
                    {
                        type = 3;
                        if (!ShowingTypes[type]) return;

                        if (sendMessage)
                        {
                            fmt  = Format_64_S;
                            name = GetString(message, 33);
                            body = GetString(message, 65);
                        }
                        else
                        {
                            fmt  = Format_64_R;
                            name = GetString(message, 41);
                            body = GetString(message, 73);
                        }
                    }
                    #endregion

                    #region 부대 / 파티
                    else if (opcode == 0x65)
                    {
                        switch (indexCode)
                        {
                            case 0xDC: type = 6; break; // 부대
                            case 0xF9: type = 4; break; // 파티
                            case 0x78: type = 7; break; // 링1
                        }
                        if (!ShowingTypes[type]) return;

                        fmt = Format_65;
                        if (sendMessage)
                        {
                            name = "-";
                            body = GetString(message, 40);
                        }
                        else
                        {
                            name = GetString(message, 53);
                            body = GetString(message, 85);
                        }
                    }
                    #endregion

                    if (string.IsNullOrWhiteSpace(body))
                        return;

                    var date = DateTime.Now;
                    FFChatApp.Current.Dispatcher.Invoke(new Action<Chat>(AddChatPriv), new Chat(type, string.Format(fmt, date.Hour, date.Minute, name, body, TypeNames[type])));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("패킷 처리중 에러 발생함");
                Console.WriteLine(ex);
            }
        }

        private static string GetString(byte[] bytes, int index)
        {
            /*
            귀찮아
            byte[] sBuff = new byte[bytes.Length];
            int sPos = 0;

            byte v;

            // 0 일반
            // 1 아이템 링크
            byte state = 0;
            int statePos;

            int pos = index;
            while ((v = bytes[pos++]) != 0 && pos < bytes.Length)
            {
                if (state == 0)
                {
                    if (v == 0x02)
                        state = 1;
                    else
                        sBuff[sPos++] = v;
                }
                else
                {
                    if (statePos == )
                }
            }
            */

            int pos = index;
            while (bytes[pos++] != 0 && pos < bytes.Length);

            if (index == pos - 1)
                return null;

            return Encoding.UTF8.GetString(bytes, index, pos - index - 1);
        }

        private static void AddChatPriv(Chat chat)
        {
            while (ChatLog.Count >= MaxLog)
                ChatLog.RemoveAt(0);
            ChatLog.Add(chat);

            MainWindow.Instance.ScrollToBottom();
        }

        public static void SetVisible(int index, bool value)
        {
            ShowingTypes[index] = value;
        }
    }
}