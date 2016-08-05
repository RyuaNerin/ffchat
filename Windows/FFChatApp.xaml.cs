using System;
using System.IO;
using System.Text;
using System.Windows;

namespace FFChat.Windows
{
    public partial class FFChatApp : Application
    {
        private readonly StreamWriter m_out;

        public FFChatApp()
        {
            InitializeComponent();
            RyuaNerin.ErrorTrace.Load();

            Console.SetOut(m_out = new StreamWriter("ffchat.log", true, Encoding.UTF8));

            FFData.Load();
        }
        
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            this.m_out.Flush();
            this.m_out.Close();
        }
    }
}
