using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Threading;
using System.Xml;
using System.Windows.Threading;
using NetDimension.Weibo;


namespace NightOwl
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    /// 
    public partial class Login : Window
    {

        OAuth oauth = new OAuth("4107916330", "d64d9caea923f51684318b81b6c0bfed", @"https://api.weibo.com/oauth2/default.html");
        
        public static Loadingxaml loadingPage
        {
            get;
            set;
        }
           
        public Login()
        {
            InitializeComponent();
            UserName.Text = GetUserName();
            LoadVersion();
        }

        public void LoadVersion()
        {
            Info.Version = Version.Content.ToString().Remove(0, 8);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != System.Windows.WindowState.Minimized)
            {
                this.WindowState = System.Windows.WindowState.Minimized;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
            //Application.Current.Shutdown();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            Thread newWindowThread = new Thread(new ThreadStart(LoadingPage));
            newWindowThread.SetApartmentState(ApartmentState.STA);
            newWindowThread.IsBackground = true;
            newWindowThread.Start();

            LoadResources();
        }

        // <add>

        public void LoadingPage()
        {
            loadingPage = new Loadingxaml();
            loadingPage.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            loadingPage.ShowDialog();
            System.Windows.Threading.Dispatcher.Run();
        }

        public void LoadResources()
        {
            bool AuthResult = oauth.ClientLogin(UserName.Text, Password.Password);
            Info.Username = UserName.Text;
            Info.Pwd = Password.Password;
            if (AuthResult)
            {
                var Sina = new NetDimension.Weibo.Client(oauth);
                var uid = Sina.API.Account.GetUID();
                Info.UID = uid;
                NetDimension.Weibo.Entities.user.Entity userInfo = Sina.API.Users.Show(uid);
                
                
                CreateSelf(userInfo, "Self.xml");
                GetFourFiles();
                LoginPage.Visibility = System.Windows.Visibility.Hidden;
                SaveUserName();

                loadingPage.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
        (Action)delegate() { loadingPage.Close(); });

                ShowDialog(new MainWindow());
                ShowDialog(new GoodByePage());
                LoginPage.Close();
                Application.Current.Shutdown();

            }
            else
            {
                Common.ShowCustomPopupWindow("Login Failed!",this);
                Info.Pwd = "";
                Info.Username = "";
                loadingPage.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
        (Action)delegate() { loadingPage.Close(); });
            }
        }


        // </add>
        public void SaveUserName()
        {
            if (RememberUserName.IsChecked.Value)
            {
                WriteAccountFile("User.txt", Info.Username);
            }
        }

        public string GetUserName()
        {
            if (File.Exists("User.txt"))
            {
                string contents = File.ReadAllText("User.txt", Encoding.UTF8).Trim();
                string[] content = contents.Split('*');
                for (int i = 0; i < content.Length - 1; i++)
                {
                    UserName.Items.Add(content[i]);
                }
                int lastOne = content.Length - 2;
                return UserName.Items[lastOne].ToString();
            }
            else
            {
                return "";
            }
        }

        private void ShowDialog(Window window)
        {
            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();    
        }

        private void GetFourFiles()
        {
            var Sina = new NetDimension.Weibo.Client(oauth);

            //******* FriendsTimeline ********//
            var json = Sina.API.Statuses.FriendsTimeline(count: 20);
            string friendsTimelineText = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            friendsTimelineText += @"<statuses>";
            foreach (var entity in json.Statuses)
            {
                XmlDocument doc = Json2XML.J2X(entity.ToString(), "statusNightOwl");
                foreach (XmlNode removeNode in doc.SelectNodes("//description"))
                {
                    removeNode.ParentNode.RemoveChild(removeNode);
                }
                friendsTimelineText += doc.InnerXml;
                friendsTimelineText = friendsTimelineText.Replace("&gt;", ">");
                friendsTimelineText = friendsTimelineText.Replace("&lt;", "<");
            }
            friendsTimelineText += @"</statuses>";
            Common.WriteFile("FriendsTimeLine.xml", friendsTimelineText);

            //******* UserTimeline ********//
            var json1 = Sina.API.Statuses.UserTimeline(count: 50);
            string userTimelineText = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            userTimelineText += @"<statuses>";
            foreach (var entity in json1.Statuses)
            {
                XmlDocument doc = Json2XML.J2X(entity.ToString(), "statusNightOwl");
                //foreach (XmlNode removeNode in doc.SelectNodes("//description"))
                //{
                //    removeNode.ParentNode.RemoveChild(removeNode);
                //}
                userTimelineText += doc.InnerXml;
                userTimelineText = userTimelineText.Replace("&gt;", ">");
                userTimelineText = userTimelineText.Replace("&lt;", "<");
            }
            userTimelineText += @"</statuses>";
            Common.WriteFile("UserTimeLine.xml", userTimelineText);

            //******* CommentTimeLine ********//
            var json2 = Sina.API.Comments.Timeline(count: 50);
            string commentsTimelineText = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            commentsTimelineText += @"<comments>";
            foreach (var entity in json2.Comments)
            {
                XmlDocument doc = Json2XML.J2X(entity.ToString(), "comment");
                commentsTimelineText += doc.OuterXml;
                commentsTimelineText = commentsTimelineText.Replace("&gt;", ">");
                commentsTimelineText = commentsTimelineText.Replace("&lt;", "<");
                commentsTimelineText = commentsTimelineText.Replace("<status>", "<retweeted_status>");
                commentsTimelineText = commentsTimelineText.Replace("</status>", "</retweeted_status>");
            }
            commentsTimelineText += @"</comments>";
            Common.WriteFile("CommentTimeLine.xml", commentsTimelineText);

            //******* MentionsTimeLine ********//
            var json3 = Sina.API.Statuses.Mentions(count: 50);
            string mentionsTimelineText = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            mentionsTimelineText += @"<statuses>";
            foreach (var entity in json3.Statuses)
            {
                XmlDocument doc = Json2XML.J2X(entity.ToString(), "statusNightOwl");
                mentionsTimelineText += doc.InnerXml;
                mentionsTimelineText = mentionsTimelineText.Replace("&gt;", ">");
                mentionsTimelineText = mentionsTimelineText.Replace("&lt;", "<");
            }
            mentionsTimelineText += @"</statuses>";
            Common.WriteFile("Mentions.xml", mentionsTimelineText);

            //******* AtCommentTimeLine ********//
            var json4 = Sina.API.Comments.Mentions(count: 50);
            string atCommentsTimelineText = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            atCommentsTimelineText += @"<comments>";
            foreach (var entity in json4.Comments)
            {
                XmlDocument doc = Json2XML.J2X(entity.ToString(), "comment");
                atCommentsTimelineText += doc.OuterXml;
                atCommentsTimelineText = atCommentsTimelineText.Replace("&gt;", ">");
                atCommentsTimelineText = atCommentsTimelineText.Replace("&lt;", "<");
                atCommentsTimelineText = atCommentsTimelineText.Replace("<status>", "<retweeted_status>");
                atCommentsTimelineText = atCommentsTimelineText.Replace("</status>", "</retweeted_status>");
            }
            atCommentsTimelineText += @"</comments>";
            Common.WriteFile("AtCommentTimeLine.xml", atCommentsTimelineText);
        }

        private void WriteAccountFile(string fileName, string fileContent)
        {
            fileContent = fileContent + "*";
            FileStream fs = File.Open(fileName, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            try
            {
                sw.Write(fileContent);
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
            
        }
        private void CreateSelf(NetDimension.Weibo.Entities.user.Entity user, string fileName)
        {
            string name = user.ScreenName;
            string location = user.Location;
            string pic = user.ProfileImageUrl;
            string guanzhu = user.FriendsCount.ToString();
            string fensi = user.FollowersCount.ToString();
            string entityNumber = user.StatusesCount.ToString();
            string favourite = user.FavouritesCount.ToString();
            string desc = user.Description;
            string id = user.ID.ToString();
            string xmlText = @"<statuses>
                              <status>
                                <user>
                                  <id>" + id + @"</id>
                                  <screen_name>" + name + @"</screen_name>
                                  <name>" + name + @"</name>
                                  <location>" + location + @"</location>
                                  <description>" + desc + @"</description>
                                  <profile_image_url>" + pic + @"</profile_image_url>
                                  <followers_count>" + fensi + @"</followers_count>
                                  <friends_count>" + guanzhu + @"</friends_count>
                                  <statuses_count>" + entityNumber + @"</statuses_count>
                                  <favourites_count>" + favourite + @"</favourites_count>
                                </user>
                              </status>
                            </statuses>";
            XmlDocument document = new XmlDocument();
            if (File.Exists(fileName))
            {
                File.Delete(fileName); 
                document.LoadXml(xmlText);
                document.Save(fileName);
            }
            else
            {
                document.LoadXml(xmlText);
                document.Save(fileName);
            }
        }    
    }
}
