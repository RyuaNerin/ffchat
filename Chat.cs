using System.Windows.Media;

namespace FFChat
{
    internal class Chat
    {
        public Chat(ChatType chatType, string text)
        {
            this.m_type = chatType;
            this.m_text = text;
        }

        private readonly ChatType m_type;
        public ChatType ChatType { get { return this.m_type; } }

        public Brush Brush { get { return this.m_type.Brush; } }

        private readonly string m_text;
        public string Text { get { return this.m_text; } }
    }
}
