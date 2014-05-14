using Sina.Api;
using System.Threading;
using System.IO;
using System.Xml;

namespace NightOwl
{
    public class Common
    {
        public static void ShowCustomPopupWindow(string message, System.Windows.Window windowOwner)
        {
            Info.CustomPopUpWindowMessage = message;
            WindowPop customPopupWindow = new WindowPop();
            customPopupWindow.Owner = windowOwner;
            customPopupWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            customPopupWindow.Show();
        }

        public static void UpdateUserTimeLine(SinaApiService api)
        {
            string result = api.user_timeline();
            WriteFile("UserTimeLine.xml", result);
            Thread.Sleep(1000);
        }

        public static void WriteFile(string fileName, string fileContent)
        {
            if (!File.Exists(fileName))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(fileContent);
                doc.Save(fileName);
            }
            else
            {
                File.Delete(fileName);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(fileContent);
                doc.Save(fileName);
            }
        }
    }
}
