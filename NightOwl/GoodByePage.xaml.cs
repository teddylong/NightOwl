using System.Windows;
using System.Windows.Input;

namespace NightOwl
{
    /// <summary>
    /// Interaction logic for GoodByePage.xaml
    /// </summary>
    public partial class GoodByePage : Window
    {
        public GoodByePage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
