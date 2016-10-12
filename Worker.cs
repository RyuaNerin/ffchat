using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFChat.Windows;

namespace FFChat
{
    internal static class Worker
    {
        // 3.15
        private static readonly MemoryPattern SignatureX86 = new MemoryPattern(
            false,
            "**088b**********505152e8********a1",
            new long[] { 0, 0x18, 0x2F0 },
            new long[] { 0, 0x18, 0x2F4 },
            new long[] { 0, 0x18, 0x2E0 },
            new long[] { 0, 0x18, 0x2E4 }
            );
        private static readonly MemoryPattern SignatureX64 = new MemoryPattern(
            true,
            "e8********85c0740e488b0d********33D2E8********488b0d",
            new long[] { 0, 0x30, 0x438 },
            new long[] { 0, 0x30, 0x440 },
            new long[] { 0, 0x30, 0x418 },
            new long[] { 0, 0x30, 0x420 }
            );

        public static readonly ObservableCollection<string> ProcessList = new ObservableCollection<string>();
        public static readonly ObservableCollection<Chat> ChatLog = new ObservableCollection<Chat>();

        private static readonly bool[] ShowingTypes = { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };
        
        private static readonly ManualResetEvent m_work = new ManualResetEvent(false);
        private static Process m_ffxiv;
        private static MemoryPattern m_pattern;
        private static IntPtr m_ffxivHandle;
        private static bool   m_isX64;
        private static IntPtr m_basePtr;
        private static IntPtr m_chatLog;

        public static readonly SettingWindow SettingWindow;

        static Worker()
        {
            SettingWindow = new SettingWindow() { Owner = MainWindow.Instance };
        }

        public static void Initialize()
        {
            FindFFXIVProcess();

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(30 * 1000);

                    if ((m_ffxiv == null) || m_ffxiv.HasExited)
                    {
                        ResetFFXIVProcess();

                        FFChatApp.Current.Dispatcher.Invoke(new Action(FindFFXIVProcess));
                    }
                }
            });

            Task.Factory.StartNew(WorkerThread);
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
                m_isX64 = !NativeMethods.IsX86Process(m_ffxiv.Handle);
                //m_ffxivHandle = process.Handle;
                m_ffxivHandle = NativeMethods.OpenProcess(0x00000010, false, m_ffxiv.Id);

                m_pattern = m_isX64 ? SignatureX64 : SignatureX86;
                m_basePtr = m_ffxiv.MainModule.BaseAddress;
                m_chatLog = Scan(m_pattern.Pattern, 0, m_pattern.IsX64);

                if (m_chatLog != IntPtr.Zero)
                {
                    m_work.Set();

                    var name = string.Format("{0}:{1}", m_ffxiv.ProcessName, m_ffxiv.Id);
                    Console.WriteLine("파이널판타지14 프로세스가 선택되었습니다: {0}", name);

                    SettingWindow.ctlSelect.IsEnabled = false;
                    SettingWindow.ctlCombo.IsEnabled = false;

                    ProcessList.Clear();
                    ProcessList.Add(name);
                    SettingWindow.ctlCombo.SelectedIndex = 0;
                }
            }
            catch
            {
            }
        }

        public static void ResetFFXIVProcess()
        {
            try
            {
                m_ffxiv = null;
                m_work.Reset();

                FindFFXIVProcess();
            }
            catch
            {
            }
        }

        public static void SelectFFXIVProcess(int pid)
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

        private static void WorkerThread()
        {
            IntPtr start;
            IntPtr end;
            IntPtr lenStart;
            IntPtr lenEnd;
            
            int[] buff = new int[0xfa0];
            int num = 0;
            //bool flag = true;
            bool flag = false;
            IntPtr zero = IntPtr.Zero;
            IntPtr ptr2 = IntPtr.Zero;

            int j;
            int i;
            int len;

            var data = new List<byte[]>();

            while (m_work.WaitOne())
            {
                start      = GetPointer(m_chatLog, m_pattern.ChatLogStart);
                end        = GetPointer(m_chatLog, m_pattern.ChatLogEnd);
                lenStart   = GetPointer(m_chatLog, m_pattern.ChatLogLenStart);
                lenEnd     = GetPointer(m_chatLog, m_pattern.ChatLogLenEnd);
                
                if ((start == IntPtr.Zero || end == IntPtr.Zero) || (lenStart == IntPtr.Zero || lenEnd == IntPtr.Zero))
                    Thread.Sleep(100);

                else if ((lenStart.ToInt64() + num * 4) == lenEnd.ToInt64())
                {
                    flag = false;
                    Thread.Sleep(100);
                }
                else
                {
                    if (lenEnd.ToInt64() < lenStart.ToInt64())
                        throw new ApplicationException("error with chat log - end len pointer is before beginning len pointer.");

                    if (lenEnd.ToInt64() < (lenStart.ToInt64() + (num * 4)))
                    {
                        if ((zero != IntPtr.Zero) && (zero != IntPtr.Zero))
                        {
                            for (j = num; j < 0x3e8; j++)
                            {
                                buff[j] = ReadInt32(ptr2 + (j * 4));
                                if (buff[j] > 0x100000)
                                {
                                    zero = IntPtr.Zero;
                                    ptr2 = IntPtr.Zero;
                                    throw new ApplicationException("Error with chat log - message length too long.");
                                }
                                int length = buff[j] - ((j == 0) ? 0 : buff[j - 1]);
                                if (length != 0)
                                {
                                    byte[] message = ReadBytes(IntPtr.Add(start, j == 0 ? 0 : buff[j - 1]), length);
                                    if (CheckMessage(message))
                                        data.Add(message);
                                }
                            }
                        }
                        buff = new int[0xfa0];
                        num = 0;
                    }
                    zero = start;
                    ptr2 = lenStart;
                    if ((lenEnd.ToInt64() - lenStart.ToInt64()) > 0x100000L)
                        throw new ApplicationException("Error with chat log - too much unread Len data (>100kb).");

                    if (((lenEnd.ToInt64() - lenStart.ToInt64()) % 4L) != 0L)
                        throw new ApplicationException("Error with chat log - Log length array is invalid.");

                    if ((lenEnd.ToInt64() - lenStart.ToInt64()) > 0xfa0L)
                        throw new ApplicationException("Error with chat log - Log length array is too small.");

                    len = (int)(lenEnd.ToInt64() - lenStart.ToInt64()) / 4;
                    for (i = num; i < len; i++)
                    {
                        buff[i] = ReadInt32(lenStart + (i * 4));
                        byte[] message = ReadBytes(start + (i == 0 ? 0 : buff[i - 1]), buff[i] - (i == 0 ? 0 : buff[i - 1]));
                        num++;
                        if (!flag && CheckMessage(message))
                            data.Add(message);
                    }
                    flag = false;

                    if (data.Count > 0)
                    {
                        Task.Factory.StartNew(LogChatMessage, data.ToArray());
                        data.Clear();
                    }

                    Thread.Sleep(200);
                }
            }
        }
        
        private static bool CheckMessage(byte[] rawData)
        {
            return ChatType.Types.ContainsKey(BitConverter.ToInt16(rawData, 4));
        }

        private static void LogChatMessage(object arg)
        {
            var rawData = (byte[][])arg;

            if (rawData.Length == 0)
                return;

            var arr = new Chat[rawData.Length];
            for (int i = 0; i < rawData.Length; ++i)
                arr[i] = ParseChat(rawData[i]);

            FFChatApp.Current.Dispatcher.Invoke(new Action<Chat[]>(AddChat), (object)arr);
        }

        private static readonly DateTime BaseTimeStamp = new DateTime(1970, 1, 1, 0, 0, 0);
        private static Chat ParseChat(byte[] rawData)
        {
#if DEBUG
            Console.WriteLine(BitConverter.ToString(rawData).Replace("-", " "));
#endif
            // Chat Type
            var type = BitConverter.ToInt16(rawData, 4);
            if (!ChatType.Types.ContainsKey(type))
                return null;

            // TimeStamp
            var dateTime = BaseTimeStamp.AddSeconds(BitConverter.ToInt32(rawData, 0)).ToLocalTime();

            // Nickname
            int pos;
            var nick = GetNick(rawData, 9, 0x3A, out pos); // ':' = 3A
            var text = GetStr(rawData, pos, rawData.Length);

            var typeObj = ChatType.Types[type];

            return new Chat(typeObj, string.Format(typeObj.Format, dateTime.Hour, dateTime.Minute, nick, text, type - 0xF));
        }
        
        private static void AddChat(Chat[] lst)
        {
            for (int i = 0; i < lst.Length; ++i)
                if (lst[i] != null)
                    ChatLog.Add(lst[i]);

            MainWindow.Instance.ScrollToBottom();
        }

        private static string GetNick(byte[] rawData, int index, int endByte, out int pos)
        {
            int len = 0;
            while (rawData[index + len] != endByte)
                len++;

            pos = index + len + 1;
            return len > 0 ? GetStr(rawData, index, index + len) : null;
        }

        private static string GetStr(byte[] rawData, int index, int endIndex)
        {
            var buff = new byte[rawData.Length];
            var bindex = 0;

            byte v;
            while (index < endIndex)
            {
                v = rawData[index++];

                if (v == 2)
                    index += rawData[index + 1] + 2;
                else
                    buff[bindex++] = v;
            }

            return bindex > 0 ? Encoding.UTF8.GetString(buff, 0, bindex) : null;
        }

        private static IntPtr Scan(string pattern, int offset, bool isX64)
        {
            var patArray = GetPatternArray(pattern);

            int len = 0x1000;
            byte[] buff = new byte[len];

            IntPtr curPtr = m_ffxiv.MainModule.BaseAddress;
            IntPtr maxPtr = IntPtr.Add(curPtr, m_ffxiv.MainModule.ModuleMemorySize);

            int index;
            IntPtr read = IntPtr.Zero;
            IntPtr nSize = new IntPtr(len);

            while (curPtr.ToInt64() < maxPtr.ToInt64())
            {
                try
                {
                    if ((curPtr + len).ToInt64() > maxPtr.ToInt64())
                        nSize = new IntPtr(maxPtr.ToInt64() - curPtr.ToInt64());

                    if (NativeMethods.ReadProcessMemory(m_ffxivHandle, curPtr, buff, nSize, ref read))
                    {
                        index = FindArray(buff, patArray, 0, read.ToInt32() - 3);

                        if (index != -1)
                        {
                            IntPtr ptr;
                            if (isX64)
                            {
                                ptr = new IntPtr(BitConverter.ToInt32(buff, index + patArray.Length));
                                ptr = new IntPtr(curPtr.ToInt64() + index + patArray.Length + 4 + ptr.ToInt64());
                            }
                            else
                            {
                                ptr = new IntPtr(BitConverter.ToInt32(buff, index + patArray.Length));
                                //ptr6 = new IntPtr(ptr6.ToInt64());
                            }

                            return ptr;
                        }
                    }
                    curPtr += len;
                }
                catch (Exception exception)
                {
                    Console.WriteLine("ERROR: Cannot scan pointers.");
                    Console.WriteLine(exception.Message);
                    Console.WriteLine(exception.StackTrace.ToString());
                }
            }

            return IntPtr.Zero;
        }

        private static byte?[] GetPatternArray(string pattern)
        {
            byte?[] arr = new byte?[pattern.Length / 2];

            for (int i = 0; i < (pattern.Length / 2); i++)
            {
                string str = pattern.Substring(i * 2, 2);
                if (str == "**")
                    arr[i] = null;
                else
                    arr[i] = new byte?(Convert.ToByte(str, 0x10));
            }

            return arr;
        }

        private static int FindArray(byte[] buff, byte?[] pattern, int startIndex, int len)
        {
            len = Math.Min(buff.Length, len);

            int i, j;
            for (i = startIndex; i < (len - pattern.Length); i++)
            {
                for (j = 0; j < pattern.Length; j++)
                    if (pattern[j].HasValue && buff[i + j] != pattern[j].Value)
                        break;

                if (j == pattern.Length)
                {
                    return i;
                }
            }

            return -1;
        }
        
        private static IntPtr GetPointer(IntPtr sigPointer, long[] pointerTree)
        {
            if (pointerTree == null)
                return IntPtr.Zero;

            if (pointerTree.Length == 0)
                return new IntPtr(sigPointer.ToInt64());

            IntPtr ptr = new IntPtr(sigPointer.ToInt64());
            for (int i = 0; i < pointerTree.Length; i++)
            {
                ptr = ReadPointer(new IntPtr(ptr.ToInt64() + pointerTree[i]));

                if (ptr == IntPtr.Zero)
                    return IntPtr.Zero;
            }
            return ptr;
        }

        private static IntPtr ReadPointer(IntPtr offset)
        {
            int num = m_isX64 ? 8 : 4;

            byte[] lpBuffer = new byte[num];
            IntPtr zero = IntPtr.Zero;
            if (!NativeMethods.ReadProcessMemory(m_ffxivHandle, offset, lpBuffer, new IntPtr(num), ref zero))
                return IntPtr.Zero;

            if (m_isX64)
                return new IntPtr(BitConverter.ToInt64(lpBuffer, 0));
            else
                return new IntPtr(BitConverter.ToInt32(lpBuffer, 0));
        }

        private static int ReadInt32(IntPtr offset)
        {
            byte[] lpBuffer = new byte[4];
            IntPtr zero = IntPtr.Zero;
            if (!NativeMethods.ReadProcessMemory(m_ffxivHandle, offset, lpBuffer, new IntPtr(4), ref zero))
                return 0;

            return BitConverter.ToInt32(lpBuffer, 0);
        }

        public static byte[] ReadBytes(IntPtr offset, int length)
        {
            IntPtr zero = IntPtr.Zero;
            if ((length <= 0) || (length > 0x186a0))
                return null;

            if (offset == IntPtr.Zero)
                return null;

            byte[] lpBuffer = new byte[length];
            NativeMethods.ReadProcessMemory(m_ffxivHandle, offset, lpBuffer, new IntPtr(length), ref zero);

            return lpBuffer;
        }
        
        private static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            public static extern bool ReadProcessMemory(
                IntPtr hProcess,
                IntPtr lpBaseAddress,
                byte[] lpBuffer,
                IntPtr nSize,
                ref IntPtr lpNumberOfBytesRead);

            [DllImport("kernel32.dll")]
            public static extern IntPtr OpenProcess(
                uint dwDesiredAccess,
                [MarshalAs(UnmanagedType.Bool)]
                bool bInheritHandle,
                int dwProcessId);

            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool IsWow64Process(
                [In]
                IntPtr process,
                [Out]
                out bool wow64Process);

            public static bool IsX86Process(IntPtr handle)
            {
                var OSVersion = Environment.OSVersion.Version;
                if ((OSVersion.Major == 5 && OSVersion.Minor >= 1) || OSVersion.Major > 5)
                {
                    bool ret;
                    return NativeMethods.IsWow64Process(handle, out ret) && ret;
                }
                return true;
            }
        }
    }
}
