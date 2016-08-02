namespace FFChat
{
    internal class Chat
    {
        public Chat(int index, string body)
        {
            this.m_chatIndex = index;
            this.m_chatBody = body;
        }

        private readonly int m_chatIndex;
        public int ChatIndex { get { return this.m_chatIndex; } }

        private readonly string m_chatBody;
        public string ChatBody { get { return this.m_chatBody; } }
    }
}
