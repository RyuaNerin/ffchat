using System.Collections.Generic;
using System.Windows.Media;

namespace FFChat
{
    internal class ChatType
    {
        // 0 Hour
        // 1 Minute
        // 2 name
        // 3 text
        // 4 linkshell index
        private const string Format_Say     = "[{0:00}:{1:00}] {2}: {3}";
        private const string Format_Tell_S  = "[{0:00}:{1:00}] >>{2}: {3}";
        private const string Format_Tell_R  = "[{0:00}:{1:00}] {2} >> {3}";
        private const string Format_Party   = "[{0:00}:{1:00}] ({2}) {3}";
        private const string Format_Party3  = "[{0:00}:{1:00}] (({2})) {3}";
        private const string Format_FC      = "[{0:00}:{1:00}] [자유부대] <{2}> {3}";
        private const string Format_LS      = "[{0:00}:{1:00}] [{4}]<{2}> {3}";
        private const string Format_Emote   = "[{0:00}:{1:00}] {3}";
        private const string Format_TxtOnly = "[{0:00}:{1:00}] {3}";

        public readonly static IDictionary<int, ChatType> Types = new Dictionary<int, ChatType>
        {
            { 0x0003, new ChatType(0x0003, Format_TxtOnly,  "#ff0000",  "이벤트") },
		    { 0x000a, new ChatType(0x000a, Format_Say,      "#f7f7f7", "말하기") },
		    { 0x000b, new ChatType(0x000b, Format_Say,      "#ffa666", "외치기") },
		    { 0x000c, new ChatType(0x000c, Format_Tell_S,   "#ffb8de", "귓 보냄") },
		    { 0x000d, new ChatType(0x000d, Format_Tell_R,   "#ffb8de", "귓 받음") },
		    { 0x000e, new ChatType(0x000e, Format_Party,    "#66e5ff", "파티") },
		    { 0x000f, new ChatType(0x000e, Format_Party3,   "#ff7f00", "연합파티") }, // ????
		    { 0x0010, new ChatType(0x0010, Format_LS,       "#d4ff7d", "링 1") },
		    { 0x0011, new ChatType(0x0011, Format_LS,       "#d4ff7d", "링 2") },
		    { 0x0012, new ChatType(0x0012, Format_LS,       "#d4ff7d", "링 3") },
		    { 0x0013, new ChatType(0x0013, Format_LS,       "#d4ff7d", "링 4") },
		    { 0x0014, new ChatType(0x0014, Format_LS,       "#d4ff7d", "링 5") },
		    { 0x0015, new ChatType(0x0015, Format_LS,       "#d4ff7d", "링 6") },
		    { 0x0016, new ChatType(0x0016, Format_LS,       "#d4ff7d", "링 7") },
		    { 0x0017, new ChatType(0x0017, Format_LS,       "#d4ff7d", "링 8") },
		    { 0x0018, new ChatType(0x0018, Format_FC,       "#abdbe5", "자유부대") },
		    { 0x001d, new ChatType(0x001d, Format_Emote,    "#bafff0", "감정") },
		    { 0x001e, new ChatType(0x001e, Format_Say,      "#ffff00", "떠들기") },
		    { 0x0038, new ChatType(0x0038, Format_TxtOnly,  "#cccccc", "혼잣말") },
		    { 0x2245, new ChatType(0x2245, Format_TxtOnly,  "#e5fffc", "부대알림") },
		    { 0x2246, new ChatType(0x2246, Format_TxtOnly,  "#e5fffc", "부대원 접속") },
        };
        
        public ChatType(int type, string format, string color, string desc)
        {
            this.m_type     = type;
            this.m_format   = format;
            this.m_brush    = CreateBrushFromHtml(color);
            this.m_desc     = desc;

            this.Visible    = true;
        }

        private static Brush CreateBrushFromHtml(string code)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(code));
        }

        private readonly int m_type;
        public int Type { get { return this.m_type; } }

        private readonly string m_desc;
        public string Desc { get { return this.m_desc; } }

        private readonly string m_format;
        public string Format { get { return this.m_format; } }

        private readonly Brush m_brush;
        public Brush Brush { get { return this.m_brush; } }

        public bool Visible { get; set; }
    }
}
