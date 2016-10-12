using System.Windows;

namespace FFChat.Windows
{
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();

            this.ctlCombo.ItemsSource = Worker.ProcessList;
        }

        private void ctlSelect_Click(object sender, RoutedEventArgs e)
        {
            Worker.SelectFFXIVProcess(int.Parse(((string)this.ctlCombo.SelectedItem).Split(':')[1]));        
        }

        private void ctlReset_Click(object sender, RoutedEventArgs e)
        {
            Worker.ResetFFXIVProcess();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
