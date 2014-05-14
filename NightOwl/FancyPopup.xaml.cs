using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.Xml;

namespace NightOwl
{
    /// <summary>
    /// Interaction logic for FancyPopup.xaml
    /// </summary>
    public partial class FancyPopup : UserControl
    {
        

        public FancyPopup()
        {
            InitializeComponent();
            string ip = GetIpFromNet();
            string[] result = GetLocationFromIP(ip);
            if (result != null)
            {
                LocationLabel.Content = result[0] + result[1] + result[4];
            }
            else
            {
                LocationLabel.Content = "No Information";
            }
        }


        private string GetIpFromNet()
        {
            try
            {
                string address = @"http://www.ip138.com/ip2city.asp"; ;

                WebClient wc = new WebClient();

                string html = wc.DownloadString(address);

                string[] result = html.Substring(html.IndexOf('[') + 1).Split(']');

                return result[0];
            }
            catch
            {
                return null;
            }
        }

        private string[] GetLocationFromIP(string ip)
        {
            try
            {
                string url = @"http://api.map.sina.com.cn/i/ip2xy.php?ip=" + ip +"&source=4107916330";
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                string html = wc.DownloadString(url);
                XmlDocument document = new XmlDocument();
                document.LoadXml(html);
                string haha = document.SelectSingleNode("//more").InnerText;
                string[] result = haha.Split('\t');
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string NightOwlLink = "http://www.teddylong.net/nightowl/index.html";
            System.Diagnostics.Process.Start("iexplore.exe", NightOwlLink);
        }
    }
}
