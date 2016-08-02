using System.Windows;
using System.Windows.Controls;

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
            Worker.SelectProcess(int.Parse(((string)this.ctlCombo.SelectedItem).Split(':')[1]));        
        }

        private void ctlReset_Click(object sender, RoutedEventArgs e)
        {
            Worker.ResetProcess();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var ctl = (CheckBox)sender;
            Worker.SetVisible(int.Parse(ctl.Tag.ToString()), ctl.IsChecked.HasValue ? ctl.IsChecked.Value : false);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
