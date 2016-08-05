using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using App;
using FFChat.Windows;

namespace FFChat
{
    internal static class Worker
    {
        private const int MaxLog = 1024;
        private const string MyName = "내 캐릭터";

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
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8"
        };

        private const string Format_Say    = "[{0:00}:{1:00}] {2}: {3}";
        private const string Format_Tell_S = "[{0:00}:{1:00}] >>{2}: {3}";
        private const string Format_Tell_R = "[{0:00}:{1:00}] {2} >> {3}";
        private const string Format_Party  = "[{0:00}:{1:00}] ({2}) {3}";
        private const string Format_Guild  = "[{0:00}:{1:00}] [{4}] <{2}> {3}";

        public static void HandleMessage(byte[] message, bool sendMessage)
        {
            try
            {
                if (message.Length < 1000)
                    return;

#if DEBUG
                Console.WriteLine(BitConverter.ToString(message).Replace("-", " "));
#endif

                // 47 인던 파티
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

                    DateTime date = DateTime.Now;

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
                        
                        date = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(BitConverter.ToUInt32(message, 24)).ToLocalTime();

                        fmt  = Format_Say;
                        if (sendMessage)
                        {
                            name = MyName;
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
                            fmt  = Format_Tell_S;
                            name = GetString(message, 33);
                            body = GetString(message, 65);
                        }
                        else
                        {
                            fmt  = Format_Tell_R;
                            name = GetString(message, 41);
                            body = GetString(message, 73);
                        }
                    }
                    #endregion

                    #region 부대 / 파티
                    else if (opcode == 0x65)
                    {
                        fmt = Format_Guild;

                        switch (indexCode)
                        {
                            /*
                            case 0x7A:
                            case 0xA4:
                            case 0xCD:
                            case 0xE5:
                            case 0xF9: fmt = Format_Party;
                                       type = 4;  break; // 파티
                            */
                            case 0xDC: type = 6;  break; // 부대
                            case 0x78: type = 7;  break; // 링1
                            case 0x7E: type = 8;  break; // 링2
                            case 0x81: type = 9;  break; // 링3
                            case 0x82: type = 10; break; // 링4
                            case 0x83: type = 11; break; // 링5
                            case 0x84: type = 12; break; // 링6
                            case 0x85: type = 13; break; // 링7
                            case 0x86: type = 14; break; // 링8
                            default:                     // 파티
                                fmt = Format_Party;
                                type = 4;
                                break;
                        }
                        if (!ShowingTypes[type]) return;

                        if (sendMessage)
                        {
                            name = MyName;
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

                    var str = string.Format(fmt, date.Hour, date.Minute, name, body, TypeNames[type]);
                    lock (ChatLog)
                        FFChatApp.Current.Dispatcher.Invoke(new Action<Chat>(AddChatPriv), new Chat(type, str));
                }
            }
            catch (IndexOutOfRangeException)
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine("패킷 처리중 에러 발생함");
                Console.WriteLine(ex);
            }
        }
        
        private static string GetString_(byte[] bytes, int index)
        {
            int pos = index;
            while (bytes[pos++] != 0 && pos < bytes.Length) ;

            if (index == pos - 1)
                return null;

            return Encoding.UTF8.GetString(bytes, index, pos - index - 1);
        }

        private readonly static string HQChar = Encoding.UTF8.GetString(new byte[] { 0xEE, 0x80, 0xBC });
        private static string GetString(byte[] raw, int index)
        {
            #region ========== 삽질의 흔적 ==========
            // 아이템 링크
            // NQ ==========
            // 0  1  2  3  4  5  6  7  8  9  10 11 12 13
            // 02 2E    C9 05 F2          01 01 FF __ ** 03
            // 02 2E 17 C9 05 F2 1E 57 02 01 01 FF 0D EC 83 9D EC 84 A0 EA B0 80 EB A3 A8 03
            // 02 2E 1B C9 05 F2 30 FA 02 01 01 FF 11 EC 95 84 EB 8B A4 EB A7 8C 20 EA B4 91 EC 84 9D 03
            // 02 2E 24 C9 05 F2 06 60 03 01 01 FF 1A EC A2 85 EA B5 90 EC 9E AC ED 8C 90 EA B4 80 EC 9D 98 20 ED 84 B0 ED 81 AC 03
            // 02 2E 1B C9 05 F2 12 80 02 01 01 FF 11 EC A7 84 EC A3 BC 20 EC B4 88 EC BD 9C EB A6 BF 03
            // 02 2E 1B C9 05 F2 24 76 02 01 01 FF 11 EB 8C 80 EA B5 AC 20 EC 83 90 EB 9F AC EB 93 9C 03
            // 02 2E 1E C9 05 F2 31 7D 02 01 01 FF 14 EA B3 A0 EC 84 B1 EB 8A A5 20 EA B0 95 EC 9E A5 EC A0 9C 03
            // 02 2E 1E C9 05 F2 2F E2 02 01 01 FF 14 EB 9D BC EB B0 94 EB 82 98 EC 9D 98 20 EB 82 A0 EA B0 9C 03
            // 02 2E 1E C9 05 F2 32 49 02 01 01 FF 14 ED 81 B0 EB BF 94 EC 82 B0 EC 96 91 20 EA B3 A0 EA B8 B0 03
            // 02 2E 18 C9 05 F2 14 01 02 01 01 FF 0E EC 95 94 EC B2 A0 20 EA B4 91 EC 84 9D 03
            // 02 2E 1B C9 05 F2 30 FA 02 01 01 FF 11 EC 95 84 EB 8B A4 EB A7 8C 20 EA B4 91 EC 84 9D 03
            // 02 2E 1E C9 05 F2 12 77 02 01 01 FF 14 EB A1 A4 EB 9E 80 EB B2 A0 EB A6 AC 20 EB 9D BC EC 94 A8 03
            // 02 2E 1C C9 05 F2 12 6F 02 01 01 FF 12 ED 8C 8C 20 ED 81 AC EB A6 BC 20 EC 88 98 ED 94 84 03
            // HQ ==========
            // 0  1  2  3  4  5  6  7  8  9  10 11 12 13 14
            // 02 2E    C9 05 F6 0F       02 01 01    __ ** 03
            // 02 2E 1F C9 05 F6 0F 73 3A 02 01 01 FF 14 EC 95 84 EB 8B A4 EB A7 8C 20 EA B4 91 EC 84 9D EE 80 BC 03
            // 02 2E 1C C9 05 F6 0F 56 41 02 01 01 FF 11 EC 95 94 EC B2 A0 20 EA B4 91 EC 84 9D EE 80 BC 03
            // 02 2E 2C C9 05 F6 0F 54 B5 02 01 01 FF 21 EB A1 A4 EB 9E 80 EB B2 A0 EB A6 AC 20 EC B9 98 EC A6 88 20 EC BC 80 EC 9D B4 ED 81 AC EE 80 BC 03
            // 02 2E 25 C9 05 F6 0F 54 70 02 01 01 FF 1A EC 95 84 ED 94 84 EC B9 BC EB A3 A8 20 EC 98 A4 EB AF 88 EB A0 9B EE 80 BC 03
            // 02 2E 25 C9 05 F6 0F 74 72 02 01 01 FF 1A EC 9D B4 EC 8A 88 EA B0 80 EB A5 B4 EB 93 9C 20 EB A8 B8 ED 95 80 EE 80 BC 03
            //
            // 사람 링크
            // 0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16
            // 02 2E 0E C9 02 FF __ ** 03
            // 02 2E 0E C9 02 FF 0A EB A5 9C EC 95 84 EB A6 B0 03
            // 02 2E 0E C9 02 FF 0A EB A5 9C EC 95 84 EB A6 B0 03
            // 02 2E 07 C9 07 F6 98 B0 3D 03
            // 02 2E 07 C9 07 F6 98 B2 18 03
            // 02 2E 0E C9 02 FF 0A EB A5 9C EC 95 84 EB A6 B0 03
            //
            // 지도 링크
            // 0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15 16
            // 02 2E -- C9 04 
            // 02 2E 0D C9 04 F2 01 1C 72 F2 07 D0 F2 05 5F 03              // 안갯빛 마을: 대형 주택 (6,6)
            // 02 2E 0D C9 04 F2 01 1C 72 F2 2C EC F2 14 82 03              // 안갯빛 마을: 대형 주택 (6,6)
            // 02 2E 11 C9 04 F2 01 1C 72 FE FF FF C7 D9 FE FF FF E0 C0 03  // 안갯빛 마을: 대형 주택 (5,5)
            // 02 2E 0F C9 04 F2 01 1C 72 FE FF FF C8 56 F2 25 1C 03        // 안갯빛 마을: 대형 주택 (5,6)

            // 02 2E 0F C9 04 82 0D FE FF FE AB 6D FE FF FF D3 14 03        // 림사 로민사 하층 갑판 (9, 11)
            // 02 2E 0F C9 04 82 0D FE FF FD 25 C7 FE FF FF AA 10 03        // 림사 로민사 하층 갑판 (7,10)
            // 02 2E 0D C9 04 82 0D F6 01 66 66 F6 01 3D 62 03              // 림사 로민사 하층 갑판 (13,12)
            // 02 2E 0E C9 04 82 0D FE FF FD B3 9F F6 02 87 38 03           // 림사 로민사 하층 갑판 (8,14)

            // 02 2E 0D C9 04 81 0C FE FF FF FF 83 F2 0B 7A 03              // 림사 로민사 상층 갑판 (11,11)
            // 02 2E 0F C9 04 81 0C FE FF FF 35 DA FE FF FD D4 18 03        // 림사 로민사 상층 갑판 (10,8)
            // 02 2E 0E C9 04 81 0C FE FF FF 52 E8 F6 02 D9 40 03           // 림사 로민사 상층 갑판 (10,14)
            // 02 2E 0C C9 04 81 0C F2 8F 11 F6 02 27 84 03                 // 림사 로민사 상층 갑판 (11,14)
            //
            // 상용구
            // 0  1  2  3  4  5 
            // 02 2E 03 ?? __ 03            (__ = +1)
            // 02 2E 03 3C 05 03            weather.exh_ko        4 04 안개
            // 02 2E 03 3C 06 03            weather.exh_ko        5 05 바람
            // 02 2E 03 3C 07 03            weather.exh_ko        6 06 강풍
            // 02 2E 03 02 CC 03            completion.exh_ko   203 CB 안녕하세요.
            // 02 2E 03 01 66 03            completion.exh_ko   101 65 상용구 사전을 사용해주세요.
            // 02 2E 03 38 B0 03            action.exh_ko       175 AF 눈에는 눈
            // 02 2E 03 38 0F 03            action.exh_ko        14 0E 플래시
            // 02 2E 03 34 05 03            race.exh_ko           4 04 미코테
            // 0  1  2  3  4  5  6
            // 02 2E 04 ?? F0 __ 03
            // 02 2E 04 38 F0 EF 03         239 EF 왕의 수익
            // 02 2E 04 38 F0 F1 03         241 F1 왕의 수익
            // 0  1  2  3  4  5  6  7
            // 02 2E 05 ?? F2 __ __ 03
            // 02 2E 05 17 F2 07 70 03      completion.exh_ko   1904 07 70 가루다
            // 02 2E 05 18 F2 08 0B 03      completion.exh_ko   2059 08 0B 가루다 토벌전
            // 02 2E 05 18 F2 08 55 03      completion.exh_ko   2133 08 55 진 비스마르크 토벌전
            // 02 2E 05 38 F2 0E 01 03      action.exh_ko       3585 0E 01 전개전술
            // 0  1  2  3  4  5  6  7  8
            // 02 2E 06 ?? F6 __ __ __ 03
            // 02 2E 06 39 F6 01 86 A9 03   100009 01 86 A9 비레고의 축복
            //
            // 상용구 테이블
            // 01 01 : completion.exh_ko
            // 25 19 : completion.exh_ko
            // 49 31 : mount.exh_ko
            // 50 32 : classjob.exh_ko
            // 51 33 : placename.exh_ko
            // 52 34 : race.exh_ko
            // 53 35 : tribe.exh_ko
            // 54 36 : guardiandeity.exh_ko
            // 55 37 : generalaction.exh_ko
            // 56 38 : action.exh_ko
            // 57 39 : craftaction.exh_ko
            // 58 3A : buddyaction.exh_ko
            // 59 3B : petaction.exh_ko
            // 60 3C : weather.exh_ko
            // 61 3D : maincommand.exh_ko
            // 62 3E : textcommand.exh_ko
            // 63 3F : action.exh_ko
            // 64 40 : pet.exh_ko
            // 65 41 : companion.exh_ko
            //
            // 정리
            // 0  1  2  3  4  5  6  7  8  9  10 11 12 13 14
            // 02 2E -- C9 05 F2 -- -- -- 01 01 FF == ** 03     아이템
            // 02 2E -- C9 05 F6 0F -- -- 02 01 01 -- == ** 03  아이템HQ
            // 02 2E 0E C9 02 FF __ ** 03                       사람
            // 02 2E 07 C9 07 FF __ ** 03                       ??? (NPC)
            // 02 2E 03 ?? __ 03                                상용구
            // 02 2E 04 ?? F0 __ 03
            // 02 2E 05 ?? F2 __ __ 03
            // 02 2E 06 ?? F6 __ __ __ 03
            #endregion
            
            byte[] arr = new byte[raw.Length];
            int arrPos = 0;

            byte v;
            int len = 0;
            bool skipTo3 = false;

            int rawPos = index;
            while (rawPos < raw.Length)
            {
                v = raw[rawPos];

                if (v == 0)
                    break;

                if (skipTo3)
                {
                    if (v == 3)
                        skipTo3 = false;

                    ++rawPos;
                    continue;
                }

                if (v == 2)
                {
                    v = raw[rawPos + 2];
                    if (v == 3 || v == 4 || v == 5 || v == 6)
                    {
                        if (FFData.Table.ContainsKey(raw[rawPos + 3]))
                        {
                                 if (v == 3) len = (raw[rawPos + 4] - 1);
                            else if (v == 4) len = (raw[rawPos + 5]);
                            else if (v == 5) len = (raw[rawPos + 5] <<  8) | (raw[rawPos + 6]);
                            else if (v == 6) len = (raw[rawPos + 5] << 16) | (raw[rawPos + 6] << 8) | (raw[rawPos + 7]);

                            var dic = FFData.Table[raw[rawPos + 3]];
                            if (dic.ContainsKey(len))
                            {
                                var bytes = dic[len];
                                
                                arr[arrPos++] = 0x3E; // <
                                Buffer.BlockCopy(bytes, 0, arr, arrPos, bytes.Length);
                                arrPos += bytes.Length;
                                arr[arrPos++] = 0x3C; // >
                            }
                            else
                            {
                                arr[arrPos++] = 0x3E; // <
                                arr[arrPos++] = 0x3F; //?
                                arr[arrPos++] = 0x3F; //?
                                arr[arrPos++] = 0x3F; //?
                                arr[arrPos++] = 0x3C; // >
                            }
                        }
                        else
                        {
                            arr[arrPos++] = 0x3E; // <
                            arr[arrPos++] = 0x3F; //?
                            arr[arrPos++] = 0x3F; //?
                            arr[arrPos++] = 0x3F; //?
                            arr[arrPos++] = 0x3C; // >
                        }

                        rawPos += 2 + v;
                    }
                    else
                    {
                        switch (raw[rawPos + 4])
                        {
                            case 0x05:
                                if (raw[rawPos + 5] == 0xF2)
                                    rawPos += 12; // NQ
                                else
                                    rawPos += 13; // HQ

                                len = raw[rawPos] - 1;

                                arr[arrPos++] = 0x3E; // >
                                Buffer.BlockCopy(raw, rawPos + 1, arr, arrPos, len);
                                arrPos += len;
                                break;

                            case 0x02:
                                rawPos += 6;
                                len = raw[rawPos] - 1;

                                Buffer.BlockCopy(raw, rawPos + 1, arr, arrPos, len);
                                arrPos += len;
                                break;

                            case 0x07:
                                arr[arrPos++] = 0x3F; //?
                                arr[arrPos++] = 0x3F; //?
                                arr[arrPos++] = 0x3F; //?

                                rawPos += 5;
                                break;
                        }
                    }                    

                    skipTo3 = true;
                }
                else
                {
                    arr[arrPos++] = v;
                    ++rawPos;
                }
            }

            if (arrPos == 0)
                return null;
            
            var str = Encoding.UTF8.GetString(arr, 0, arrPos);

            // HQ Icon
            str = str.Replace(HQChar, " (HQ)");

            return str;
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
