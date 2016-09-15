using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FFChat
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
    //
    // STRUCTURE
    //=========================
    // VA
    // xx < 0xA0
    // yy xx (xx = xx + 1)
    // F2 xx xx
    // F6 xx xx xx
    // FE xx xx xx xx
    //
    // |-----|-----|-----|--------------------|
    // | OFF | LEN | HEX | DESCRIPTION        |
    // |-----|-----|-----|--------------------|
    // |  0  |  1  | 02  |                    |
    // |  1  |  1  | 2E  |                    |
    // |  2  |  1  |     | Struct Length - 2  |
    // |  3  |  1  |     | Category           |
    // | ... |     |     |                    |
    // |  -  |  1  | 03  |                    |
    // |-----|-----|-----|--------------------|
    //
    // ================================================================================
    // Category == 01-19 31 32 33 34 35 36 37 38 39 3A 3B 3C 3D 3E 3F 40 41 (Completion)
    // |-----|-----|-----|--------------------|
    // | OFF | LEN | HEX | DESCRIPTION        |
    // |-----|-----|-----|--------------------|
    // |  4  |  -  |     | V = VALUE          |
    // |  -  |  1  | 03  |                    |
    // |-----|-----|-----|--------------------|
    //
    // ================================================================================
    // Category == C9 <flag> <item> <me> <t> ...
    // |-----|-----|-----|--------------------|
    // | OFF | LEN | HEX | DESCRIPTION        |
    // |-----|-----|-----|--------------------|
    // |  4  |  1  | 04  | Sub Category       |
    // | ... |     |     |                    |
    // |  -  |  1  | 03  |                    |
    // |-----|-----|-----|--------------------|
    // SubCategory
    // 04 Flag
    // 02 Player
    // 07 NPC
    // 05 Item
    #endregion

    static class ChatParser
    {
        private readonly static string HQChar = Encoding.UTF8.GetString(new byte[] { 0xEE, 0x80, 0xBC });
        public static string GetString(byte[] raw, int index)
        {
            byte v;

            int rawPos = index;
            int nextPos;
            int val;

            byte[] buff;
            string str;
            using (var mem = new MemoryStream(raw.Length))
            {
                while (rawPos < raw.Length && (v = raw[rawPos]) != 0) 
                {
                    if (v != 2)
                    {
                        mem.WriteByte(v);
                        rawPos += 1;
                    }
                    else
                    {
                        nextPos = rawPos + raw[rawPos + 2] + 3;

                        v = raw[rawPos + 3];

                        rawPos += 4;
                        if (v != 0xC9)
                        {
                            #region completion
                            // |-----|-----|-----|--------------------|
                            // | OFF | LEN | HEX | DESCRIPTION        |
                            // |-----|-----|-----|--------------------|
                            // |  0  |  1  | 02  |                    |
                            // |  1  |  1  | 2E  |                    |
                            // |  2  |  1  |     | Struct Length - 2  |
                            // |  3  |  1  | C9  | Category           |
                            // | >4  |  -  |     | V = VALUE          |
                            // |  -  |  1  | 03  |                    |
                            // |-----|-----|-----|--------------------|

                            val = GetValue(raw, ref rawPos);

                            buff = FFData.Table[v][val];

                            mem.WriteByte(0xE2);
                            mem.WriteByte(0x96);
                            mem.WriteByte(0xB6); // ▶

                            mem.Write(buff, 0, buff.Length);

                            mem.WriteByte(0xE2);
                            mem.WriteByte(0x97);
                                    mem.WriteByte(0x80); // ◀
                            #endregion
                        }
                        else
                        {
                            v = raw[rawPos];
                            rawPos += 1;

                            // <flag>
                            switch (v)
                            {
                                #region <flag>
                                // |-----|-----|-----|--------------------|
                                // | OFF | LEN | HEX | DESCRIPTION        |
                                // |-----|-----|-----|--------------------|
                                // |  0  |  1  | 02  |                    |
                                // |  1  |  1  | 2E  |                    |
                                // |  2  |  1  |     | Struct Length - 2  |
                                // |  3  |  1  | C9  | Category           |
                                // |  4  |  1  | 04  | Sub Category       |
                                // | >5  |  -  |     | V = ? Region ID    |
                                // |  -  |  -  |     | V = <Map ID>       |
                                // |  -  |  -  |     | V = Unknown        |
                                // |  -  |  -  |     | V = Unknown        |
                                // |  -  |  1  | 03  |                    |
                                // |-----|-----|-----|--------------------|
                                case 0x04:
                                    GetValue(raw, ref rawPos);
                                    val = GetValue(raw, ref rawPos);
                                    GetValue(raw, ref rawPos);
                                    GetValue(raw, ref rawPos);
                                    
                                    buff = FFData.PlaceName[FFData.Map[val]];
                                    mem.WriteByte(0xE2);
                                    mem.WriteByte(0x96);
                                    mem.WriteByte(0xB6); // ▶

                                    mem.Write(buff, 0, buff.Length);

                                    mem.WriteByte(0xE2);
                                    mem.WriteByte(0x97);
                                    mem.WriteByte(0x80); // ◀

                                    break;
                                #endregion

                                #region <player>
                                // |-----|-----|-----|--------------------|
                                // | OFF | LEN | HEX | DESCRIPTION        |
                                // |-----|-----|-----|--------------------|
                                // |  0  |  1  | 02  |                    |
                                // |  1  |  1  | 2E  |                    |
                                // |  2  |  1  |     | Struct Length - 2  |
                                // |  3  |  1  | C9  | Category           |
                                // |  4  |  1  | 02  | Sub Category       |
                                // | >5  |  1  | FF  |                    |
                                // |  6  |  -  |     | Length + 1         |
                                // |  7  |  L  |     | UTF8 String        |
                                // |  -  |  1  | 03  |                    |
                                // |-----|-----|-----|--------------------|
                                case 0x02:
                                    val = raw[rawPos + 1] - 1;
                                    mem.Write(raw, rawPos + 2, val);
                                    break;
                                #endregion

                                #region <npc>
                                // |-----|-----|-----|--------------------|
                                // | OFF | LEN | HEX | DESCRIPTION        |
                                // |-----|-----|-----|--------------------|
                                // |  0  |  1  | 02  |                    |
                                // |  1  |  1  | 2E  |                    |
                                // |  2  |  1  |     | Struct Length - 2  |
                                // |  3  |  1  | C9  | Category           |
                                // |  4  |  1  | 07  | Sub Category       |
                                // | >5  |  -  |     | V = NPC ID         |
                                // |  -  |  1  | 03  |                    |
                                // |-----|-----|-----|--------------------|
                                case 0x07:
                                    mem.WriteByte(0x3F); // ?
                                    mem.WriteByte(0x3F); // ?
                                    mem.WriteByte(0x3F); // ?
                                    break;
                                #endregion

                                #region <item>
                                // |-----|-----|-----|--------------------|
                                // | OFF | LEN | HEX | DESCRIPTION        |
                                // |-----|-----|-----|--------------------|
                                // |  0  |  1  | 02  |                    |
                                // |  1  |  1  | 2E  |                    |
                                // |  2  |  1  |     | Struct Length - 2  |
                                // |  3  |  1  | C9  | Category           |
                                // |  4  |  1  | 05  | Sub Category       |
                                // | >5  |  -  |     | V = Item Id        | (VA & 0xF0000 == 0xF0000 => HQ Item)
                                // |  -  |  1  | 02  |                    |
                                // |  -  |  1  | 01  |                    |
                                // |  -  |  1  | 01  |                    |
                                // |  -  |  -  |     | V = L              |
                                // |  -  |  L  |     | Name               |
                                // |  -  |  1  | 03  |                    |
                                // |-----|-----|-----|--------------------|
                                case 0x05:
                                    MovePos(raw, ref rawPos);
                                    rawPos += 3;
                                    val = GetValue(raw, ref rawPos);

                                    mem.WriteByte(0xE2);
                                    mem.WriteByte(0x96);
                                    mem.WriteByte(0xB6); // ▶

                                    mem.Write(raw, rawPos, val);

                                    mem.WriteByte(0xE2);
                                    mem.WriteByte(0x97);
                                    mem.WriteByte(0x80); // ◀
                                    break;
                                #endregion
                            }
                        }

                        rawPos = nextPos;
                    }
                }

                if (mem.Length == 0)
                    return null;

                buff = mem.ToArray();
                str = Encoding.UTF8.GetString(buff, 0, buff.Length);
            }

            // HQ Icon
            str = str.Replace(HQChar, "∂");

            return str;
        }

        private static int GetValue(byte[] raw, ref int index)
        {
            var v = raw[index];
            int i = v;

            if (v < 0xF0)
            {
                i = v - 1;
                index += 1;
            }
            else if (v == 0xF2)
            {
                i = (raw[index + 1] <<  8) | (raw[index + 2]);
                index += 3;
            }
            else if (v == 0xF6)
            {
                i = (raw[index + 1] << 16) | (raw[index + 2] <<  8) | (raw[index + 3]);
                index += 4;
            }
            else if (v == 0xF6)
            {
                i = (raw[index + 1] << 32) | (raw[index + 2] << 16) | (raw[index + 3] << 8) | (raw[index + 4]);
                index += 5;
            }
            else
            {
                i = raw[index + 1];
                index += 2;
            }

            return i;
        }
        private static void MovePos(byte[] raw, ref int index)
        {
            var v = raw[index];

            if (v < 0xF0)
                index += 1;
            else if (v == 0xF2)
                index += 3;
            else if (v == 0xF6)
                index += 4;
            else if (v == 0xF6)
                index += 5;
            else
                index += 2;
        }
    }
}
