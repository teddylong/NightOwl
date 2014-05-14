using System.Windows;

namespace NightOwl
{
    /// <summary>
    /// Interaction logic for WindowPop.xaml
    /// </summary>
    public partial class WindowPop : Window
    {
        public WindowPop()
        {
            
            InitializeComponent();
            CustomMessage.Text = Info.CustomPopUpWindowMessage;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
