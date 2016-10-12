namespace FFChat
{
    internal class MemoryPattern
    {
        public MemoryPattern(bool x64, string pattern, long[] start, long[] end, long[] lenStart, long[] lenEnd)
        {
            this.IsX64           = x64;
            this.Pattern         = pattern;
            this.ChatLogStart    = start;
            this.ChatLogEnd      = end;
            this.ChatLogLenStart = lenStart;
            this.ChatLogLenEnd   = lenEnd;
        }

        public bool   IsX64             { get; private set; }
        public string Pattern           { get; private set; }
        public long[] ChatLogStart      { get; private set; }
        public long[] ChatLogEnd        { get; private set; }
        public long[] ChatLogLenStart   { get; private set; }
        public long[] ChatLogLenEnd     { get; private set; }
    }
}
