// https://github.com/devunt/DFAssist

using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace App
{
    partial class Network
    {
        private void AnalyseFFXIVPacket(byte[] payload)
        {
            try
            {
                while (true)
                {
                    if (payload.Length < 4)
                    {
                        break;
                    }

                    var type = BitConverter.ToUInt16(payload, 0);

                    if (type == 0x0000 || type == 0x5252)
                    {
                        if (payload.Length < 28)
                        {
                            break;
                        }

                        var length = BitConverter.ToInt32(payload, 24);

                        if ((length <= 0) || (payload.Length < length))
                        {
                            break;
                        }

                        using (MemoryStream messages = new MemoryStream())
                        {
                            using (MemoryStream stream = new MemoryStream(payload, 0, length))
                            {
                                stream.Seek(40, SeekOrigin.Begin);

                                if (payload[33] == 0x00)
                                {
                                    stream.CopyTo(messages);
                                }
                                else
                                {
                                    stream.Seek(2, SeekOrigin.Current); // .Net DeflateStream 버그 (앞 2바이트 강제 무시)

                                    using (DeflateStream z = new DeflateStream(stream, CompressionMode.Decompress))
                                    {
                                        z.CopyTo(messages);
                                    }
                                }
                            }
                            messages.Seek(0, SeekOrigin.Begin);

                            var messageCount = BitConverter.ToUInt16(payload, 30);
                            for (int i = 0; i < messageCount; i++)
                            {
                                try
                                {
                                    var buffer = new byte[4];
                                    var read = messages.Read(buffer, 0, 4);
                                    if (read < 4)
                                    {
                                        Console.WriteLine("메시지 처리 요청중 길이 에러 발생함: {0}, {1}/{2}", read, i, messageCount);
                                        break;
                                    }
                                    var messageLength = BitConverter.ToInt32(buffer, 0);

                                    var message = new byte[messageLength];
                                    messages.Seek(-4, SeekOrigin.Current);
                                    messages.Read(message, 0, messageLength);

                                    HandleMessage(message);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("메시지 처리 요청중 에러 발생함");
                                    Console.WriteLine(ex);
                                }
                            }
                        }

                        if (length < payload.Length)
                        {
                            // 더 처리해야 할 패킷이 남아 있음
                            var buff = new byte[payload.Length - length];
                            Buffer.BlockCopy(payload, length, buff, 0, buff.Length);
                            payload = buff;

                            continue;
                        }
                    }
                    else
                    {
                        // 앞쪽이 잘려서 오는 패킷 workaround
                        // 잘린 패킷 1개는 버리고 바로 다음 패킷부터 찾기...
                        // TODO: 버리는 패킷 없게 제대로 수정하기

                        for (var offset = 0; offset < (payload.Length - 2); offset++)
                        {
                            var possibleType = BitConverter.ToUInt16(payload, offset);
                            if (possibleType == 0x5252)
                            {
                                var buff = new byte[payload.Length - offset];
                                Buffer.BlockCopy(payload, offset, buff, 0, buff.Length);
                                payload = buff;

                                AnalyseFFXIVPacket(payload);
                                break;
                            }
                        }
                    }

                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("패킷 처리중 에러 발생함");
                Console.WriteLine(ex);
            }
        }

        private static string[] IndexNames =
        {
            "말하기",      "떠들기",
            "외치기",      "귓속말",
            "자유부대",    "파티",
            "연합파티",
            "링크셸1", "링크셸2",
            "링크셸3", "링크셸4",
            "링크셸5", "링크셸6",
            "링크셸7", "링크셸8"
        };
        private void HandleMessage(byte[] message)
        {
            try
            {
                if (message.Length < 32)
                {
                    // type == 0x0000 이였던 메시지는 여기서 걸러짐
                    return;
                }
                
                var opcode = BitConverter.ToUInt16(message, 18);

                //Console.WriteLine(BitConverter.ToString(message));

                // 64 귓속말
                // 65 자유부대
                // 67 외치기

                if (opcode == 0x64 || opcode == 0x65 || opcode == 0x67)
                {
                    bool mine = message[0] != 0x58;

                    int indexCode = message[32];

                    int i = 0;
                    // 말하기 떠들기 외치기
                    if (opcode == 0x67)
                    {
                        if (mine)
                            indexCode = message[56];

                        switch (indexCode)
                        {
                            case 0x0A: i = 0; break; // 말하기
                            case 0x1E: i = 1; break; // 떠들기
                            case 0x0B: i = 2; break; // 외치기
                        }

                        if (mine)
                            AddChat(i, "-", GetString(message, 58));
                        else
                            AddChat(i, GetString(message, 52), GetString(message, 84));
                    }
                    // 귓속말
                    else if (opcode == 0x64)
                    {
                        if (mine)
                            AddChat(3, string.Format(">>{0}: {1}", GetString(message, 33), GetString(message, 65)));
                        else
                            AddChat(3, string.Format("{0} >> {1}", GetString(message, 41), GetString(message, 73)));
                    }
                    // 부대 / 파티
                    else if (opcode == 0x65)
                    {
                        switch (indexCode)
                        {
                            case 0xDC: i = 4; break; // 부대
                            case 0xF9: i = 1; break; // 파티
                        }
                        if (mine)
                            AddChat(i, "-",                  GetString(message, 40));
                        else
                            AddChat(i, GetString(message, 53), GetString(message, 85));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("패킷 처리중 에러 발생함");
                Console.WriteLine(ex);
            }
        }

        private void AddChat(int index, string name, string text)
        {
            if (text == null)
                return;

            var sb = new StringBuilder(256);
            sb.Append(DateTime.Now.ToString("[HH:mm] "));
            sb.Append('[');
            sb.Append(IndexNames[index]);
            sb.Append("] ");
            sb.Append('<');
            sb.Append(name);
            sb.Append("> ");
            sb.Append(text);

            FFChat.frmMain.Instance.AddChat(index, sb.ToString());
        }

        private void AddChat(int index, string str)
        {
            var sb = new StringBuilder(256);
            sb.Append(DateTime.Now.ToString("[HH:mm] "));
            sb.Append(str);

            FFChat.frmMain.Instance.AddChat(index, sb.ToString());
        }

        private string GetString(byte[] bytes, int index)
        {
            int pos = index;
            while (bytes[pos++] != 0 && pos < bytes.Length);

            if (index == pos - 1)
                return null;

            return Encoding.UTF8.GetString(bytes, index, pos - index - 1);
        }
    }
}
