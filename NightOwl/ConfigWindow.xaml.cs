using System;
using System.Text;
using System.Windows;
using System.Threading;
using System.Net;
using System.Xml;
using System.Windows.Threading;

namespace NightOwl
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
            LatestVersion.Content = "";
            VersionSummary.Text = "";
            DownLoad.Visibility = System.Windows.Visibility.Hidden;
            CheckGIF.Visibility = System.Windows.Visibility.Visible;

            CurrentVersion.Content = Info.Version;
            CheckVersion();
        }

        private void DownLoad_Click(object sender, RoutedEventArgs e)
        {
            string link = "http://www.teddylong.net/nightowl/index.html";
            System.Diagnostics.Process.Start("iexplore.exe", link);
        }

        public void CheckVersion()
        {
            Thread newThread = new Thread(new ThreadStart(CheckingVersion));
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.IsBackground = true;
            newThread.Start();
            CheckGIF.Visibility = System.Windows.Visibility.Visible;
        }

        public void CheckingVersion()
        {
            
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            string result = wc.DownloadString("http://www.teddylong.net/nightowl/Version.xml");
            //result = result.Remove(0,20);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result.Trim());
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate()
            {
                string versionID = doc.SelectSingleNode("version/id").InnerText;
                string versionSummary = doc.SelectSingleNode("version/text").InnerText;
                if (CurrentVersion.Content != null)
                {
                    if (versionID != CurrentVersion.Content.ToString())
                    {
                        DownLoad.Visibility = System.Windows.Visibility.Visible;
                        LatestVersion.Content = versionID;
                        VersionSummary.Text = versionSummary;
                    }
                    else
                    {
                        LatestVersion.Content = versionID;
                        VersionSummary.Text = "Already the latest version.";
                    }
                }
                CheckGIF.Visibility = System.Windows.Visibility.Hidden;  
            });
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
