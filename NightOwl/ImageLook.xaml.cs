using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NightOwl
{
    /// <summary>
    /// Interaction logic for ImageLook.xaml
    /// </summary>
    public partial class ImageLook : Window
    {
        public ImageLook()
        {
            InitializeComponent();
            showImg();
        }

        public void showImg()
        {
            string address = Info.PicAddress;
            Uri uri = new Uri(address);
            ImageSource imgSource = new BitmapImage(uri);
            haha.ImageSource = imgSource;
            
        }

     

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

       

        
    }
}
