using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FFChat
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var writer = new StreamWriter("ffchat.log", true, Encoding.UTF8);

            RyuaNerin.ErrorTrace.Load();

            Console.SetOut(writer);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());

            writer.Flush();

            writer.Close();
        }
    }
}
