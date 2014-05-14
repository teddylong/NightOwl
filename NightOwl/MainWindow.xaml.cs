using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Sina.Api;
using System.Timers;
using System.IO;
using System.Windows.Threading;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using NetDimension.Weibo;
using System.Collections.Generic;

namespace NightOwl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Timers.Timer time = new System.Timers.Timer(30000);
        OAuth oauth = new OAuth("4107916330", "d64d9caea923f51684318b81b6c0bfed", @"https://api.weibo.com/oauth2/default.html");

        public MainWindow()
        {
            InitializeComponent();

            ShowSelf();
            ChangeToFriendTimeLine();
            this.SecondPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.ThridPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.newMessage.Visibility = System.Windows.Visibility.Hidden;
            this.atMessagePic.Visibility = System.Windows.Visibility.Hidden;
            this.commentcount.Visibility = System.Windows.Visibility.Hidden;
            this.FourthPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.FifthPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.GifLoadingPic.Visibility = System.Windows.Visibility.Hidden;
            this.clearImg.Visibility = System.Windows.Visibility.Hidden;
            this.myEmo.Visibility = System.Windows.Visibility.Hidden;
            this.FavBorder.Visibility = System.Windows.Visibility.Hidden;

            time.AutoReset = true;
            time.Enabled = true;
            time.Elapsed += new ElapsedEventHandler(time_Elapsed);

        }

        public void time_Elapsed(object source, ElapsedEventArgs e)
        {

            bool AuthResult = oauth.ClientLogin(Info.Username, Info.Pwd);
            if (AuthResult)
            {
                var Sina = new NetDimension.Weibo.Client(oauth);
                NetDimension.Weibo.Entities.UnreadCountResult unreadResult = Sina.API.Account.UnreadCount(Info.UID);
                string unreadComment = unreadResult.Comment;
                string unreadMention = unreadResult.MentionStatus;
                string unreadStatus = unreadResult.Status;
                string unreadSiXin = unreadResult.DirectMessage;
                string unreadMentionComment = unreadResult.MentionComment;
                if (!unreadComment.Equals("0"))
                {
                    commentcount.Dispatcher.BeginInvoke
                        (DispatcherPriority.Normal, (Action)(() =>
                        {
                            commentcount.Text = unreadComment;
                            commentcount.Visibility = System.Windows.Visibility.Visible;
                        }));
                }

                if (!unreadMentionComment.Equals("0"))
                {
                    newAtCommentLabel.Dispatcher.BeginInvoke
                        (DispatcherPriority.Normal, (Action)(() =>
                        {
                            newAtCommentLabel.Content = unreadMentionComment;

                        }));
                }
                else
                {
                    newAtCommentLabel.Dispatcher.BeginInvoke
                        (DispatcherPriority.Normal, (Action)(() =>
                        {
                            newAtCommentLabel.Content = "";

                        }));
                }

                if (!unreadMention.Equals("0"))
                {
                    atMessagePic.Dispatcher.BeginInvoke
                        (DispatcherPriority.Normal, (Action)(() =>
                        {
                            atMessagePic.Visibility = System.Windows.Visibility.Visible;
                            atMsgLabel.Content = unreadMention;
                        }));
                }
                else
                {
                    atMsgLabel.Dispatcher.BeginInvoke
                        (DispatcherPriority.Normal, (Action)(() =>
                        {
                            atMsgLabel.Content = "";
                        }));
                }
                if (!unreadStatus.Equals("0"))
                {
                    newMessage.Dispatcher.BeginInvoke
                        (DispatcherPriority.Normal, (Action)(() =>
                        {
                            if (newMessage.Visibility != System.Windows.Visibility.Visible)
                            {
                                newMessage.Visibility = System.Windows.Visibility.Visible;
                                newMsgLabel.Visibility = System.Windows.Visibility.Visible;
                            }
                            newMsgLabel.Content = unreadStatus;
                        }
                        ));
                }
                else
                {
                    newMsgLabel.Dispatcher.BeginInvoke
                        (DispatcherPriority.Normal, (Action)(() =>
                        {
                            newMsgLabel.Content = "";
                        }
                        ));
                }
                if (!unreadMention.Equals("0") || !unreadComment.Equals("0") || !unreadMentionComment.Equals("0"))
                {
                    this.Dispatcher.BeginInvoke
                    (DispatcherPriority.Normal, (Action)(() =>
                        {
                            this.showBalloon("You GOT " + unreadStatus + " new Entry!" + "\n" + "You GOT " + unreadMention + " new Mention!" + "\n" + "You GOT " + (Int16.Parse(unreadComment) + Int16.Parse(unreadMentionComment)) + " new Comment!");
                        }
                        ));
                }
            }
        }

        public void showBalloon(string message)
        {
            FancyBalloon fb = new FancyBalloon();
            fb.BalloonText = message;
            MyNotification.ShowCustomBalloon(fb, PopupAnimation.Slide, 7000);
        }


        private void moveWindows(object sender, MouseEventArgs e)
        {
            this.DragMove();
        }

        private void minWindow(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != System.Windows.WindowState.Minimized)
            {
                this.WindowState = System.Windows.WindowState.Minimized;
            }
        }

        private void closeWindow(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != System.Windows.WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                this.WindowState = System.Windows.WindowState.Minimized;
            }
        }

        private void SthToSayTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SthToSayTextBox.Text.Length <= 140)
            {
                Label140.Content = (140 - SthToSayTextBox.Text.Length);
            }
            else
            {
                SthToSayTextBox.MaxLength = SthToSayTextBox.Text.Length - 1;
            }
        }

        private void ShowSelf()
        {
            XmlDocument document = new XmlDocument();
            document.Load("Self.xml");
            BlogerName.Text = document.SelectSingleNode("//screen_name").InnerText;
            BlogerLocation.Text = document.SelectSingleNode("//location").InnerText;
            AvatarPic.Source = new BitmapImage(new Uri(document.SelectSingleNode("//profile_image_url").InnerText));
            Guanzhushu.Text = document.SelectSingleNode("//friends_count").InnerText;
            Fensishu.Text = document.SelectSingleNode("//followers_count").InnerText;
            Weiboshu.Text = document.SelectSingleNode("//statuses_count").InnerText;
        }

        private void ChangeToUserTimeLine(object sender, MouseButtonEventArgs e)
        {
            this.SecondPolygon.Visibility = System.Windows.Visibility.Visible;
            this.FirstPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.ThridPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.FourthPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.FifthPolygon.Visibility = System.Windows.Visibility.Hidden;

            XmlDocument document = new XmlDocument();
            document.Load("UserTimeLine.xml");
            XmlNodeList nodelist = document.SelectNodes("//statusNightOwl");
            this.MainListBox.ItemsSource = nodelist;
        }

        private void ChangeToFriendTimeLine(object sender, MouseButtonEventArgs e)
        {
            this.FirstPolygon.Visibility = System.Windows.Visibility.Visible;
            this.SecondPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.ThridPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.FourthPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.FifthPolygon.Visibility = System.Windows.Visibility.Hidden;

            XmlDocument document = new XmlDocument();
            document.Load("FriendsTimeLine.xml");
            XmlNodeList nodelist = document.SelectNodes("//statusNightOwl");
            this.MainListBox.ItemsSource = nodelist;
        }

        private void ChangeToFriendTimeLine()
        {
            this.FirstPolygon.Visibility = System.Windows.Visibility.Visible;
            this.SecondPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.ThridPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.FourthPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.FifthPolygon.Visibility = System.Windows.Visibility.Hidden;

            XmlDocument document = new XmlDocument();
            document.Load("FriendsTimeLine.xml");
            XmlNodeList nodelist = document.SelectNodes("//statusNightOwl");
            this.MainListBox.ItemsSource = nodelist;

        }

        private void ChangeToCommentTimeLine(object sender, MouseButtonEventArgs e)
        {
            this.FirstPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.SecondPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.ThridPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.FourthPolygon.Visibility = System.Windows.Visibility.Visible;
            this.FifthPolygon.Visibility = System.Windows.Visibility.Hidden;

            XmlDocument document = new XmlDocument();
            document.Load("CommentTimeLine.xml");
            XmlNodeList nodelist = document.SelectNodes("//comment");
            this.MainListBox.ItemsSource = nodelist;
        }

        private void ChangeToCommentTimeLine()
        {
            this.FirstPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.SecondPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.ThridPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.FourthPolygon.Visibility = System.Windows.Visibility.Visible;
            this.FifthPolygon.Visibility = System.Windows.Visibility.Hidden;

            XmlDocument document = new XmlDocument();
            document.Load("CommentTimeLine.xml");
            XmlNodeList nodelist = document.SelectNodes("//comment");
            this.MainListBox.ItemsSource = nodelist;
        }

        private void ChangeToAtCommentTimeLine()
        {
            this.FirstPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.SecondPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.ThridPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.FourthPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.FifthPolygon.Visibility = System.Windows.Visibility.Visible;

            XmlDocument document = new XmlDocument();
            document.Load("AtCommentTimeLine.xml");
            XmlNodeList nodelist = document.SelectNodes("//comment");
            this.MainListBox.ItemsSource = nodelist;
        }

        private void PublishBtn_Click(object sender, RoutedEventArgs e)
        {
            bool AuthResult = oauth.ClientLogin(Info.Username, Info.Pwd);
            if (AuthResult)
            {
                var Sina = new NetDimension.Weibo.Client(oauth);

                if (UploadPIC.ImageSource != null)
                {

                    string file = UploadPIC.ImageSource.ToString().Remove(UploadPIC.ImageSource.ToString().IndexOf("file"), 8);
                    //string file = @"C:\Users\v-telong\Desktop\1CAMX4XGW.jpg";
                    var bitmap = new System.Drawing.Bitmap(file);
                    MemoryStream ms = new MemoryStream();
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    string newWeiBoID = Sina.API.Statuses.Upload(SthToSayTextBox.Text, ms.ToArray()).ID;
                }
                else
                {
                    string newWeiBoID = Sina.API.Statuses.Update(SthToSayTextBox.Text).ID;
                }
            }
        }

        private void newMessage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Thread newThread = new Thread(new ThreadStart(newMessageProcess));
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.IsBackground = true;
            newThread.Start();

            newMsgLabel.Visibility = System.Windows.Visibility.Hidden;
            GifLoadingPic.Visibility = System.Windows.Visibility.Visible;
        }

        public void newMessageProcess()
        {
            bool AuthResult = oauth.ClientLogin(Info.Username, Info.Pwd);
            if (AuthResult)
            {
                MainListBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)delegate() { MainListBox.Visibility = Visibility.Hidden; });

                var Sina = new NetDimension.Weibo.Client(oauth);
                var json = Sina.API.Statuses.FriendsTimeline(count: 50);
                string friendsTimelineText = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                friendsTimelineText += @"<statuses>";
                foreach (var entity in json.Statuses)
                {
                    XmlDocument doc = Json2XML.J2X(entity.ToString(), "statusNightOwl");
                    friendsTimelineText += doc.InnerXml;
                    friendsTimelineText = friendsTimelineText.Replace("&gt;", ">");
                    friendsTimelineText = friendsTimelineText.Replace("&lt;", "<");
                }
                friendsTimelineText += @"</statuses>";
                Common.WriteFile("FriendsTimeLine.xml", friendsTimelineText);
                Thread.Sleep(2000);

                newMessage.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)delegate() { newMessage.Visibility = Visibility.Hidden; });

                GifLoadingPic.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)delegate() { GifLoadingPic.Visibility = System.Windows.Visibility.Hidden; });

                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)delegate() { this.ChangeToFriendTimeLine(); });

                MainListBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)delegate() { MainListBox.Visibility = System.Windows.Visibility.Visible; });
            }
        }

        private void Comment_Click(object sender, RoutedEventArgs e)
        {
            GifLoadingPic.Visibility = System.Windows.Visibility.Visible;
            Thread newThread = new Thread(new ParameterizedThreadStart(WaitingComment));
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.IsBackground = true;
            newThread.Start(sender);
        }

        public void WaitingComment(object parm)
        {
            System.Windows.Controls.Image btn = (System.Windows.Controls.Image)parm;
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                (Action)delegate()
                {
                    XmlElement element = (XmlElement)btn.Tag;

                    Info.MessageID = element.SelectSingleNode("id").InnerText;
                    Info.MessageText = element.SelectSingleNode("text").InnerText;
                    Info.RepostCount = element.SelectSingleNode("reposts_count").InnerText;
                    Info.CommentCount = element.SelectSingleNode("comments_count").InnerText; ;

                    CommentWindow cw = new CommentWindow();

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (Action)delegate() { ShowWindow(cw); });
                    GifLoadingPic.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (Action)delegate() { GifLoadingPic.Visibility = Visibility.Hidden; });
                });
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            GifLoadingPic.Visibility = System.Windows.Visibility.Visible;
            Thread newThread = new Thread(new ParameterizedThreadStart(WaitingForward));
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.IsBackground = true;
            newThread.Start(sender);
        }

        public void WaitingForward(object parm)
        {
            System.Windows.Controls.Image btn = (System.Windows.Controls.Image)parm;
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                (Action)delegate()
                {
                    XmlElement element = (XmlElement)btn.Tag;

                    Info.MessageID = element.SelectSingleNode("id").InnerText;
                    Info.MessageText = element.SelectSingleNode("text").InnerText;
                    Info.RepostCount = element.SelectSingleNode("reposts_count").InnerText;
                    Info.CommentCount = element.SelectSingleNode("comments_count").InnerText; ;

                    ForwardWindow fw = new ForwardWindow();
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (Action)delegate() { ShowWindow(fw); });

                    GifLoadingPic.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (Action)delegate() { GifLoadingPic.Visibility = Visibility.Hidden; });
                });
        }

        private void Image_entity_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            GifLoadingPic.Visibility = System.Windows.Visibility.Visible;
            Thread newThread = new Thread(new ParameterizedThreadStart(LoadingInShowImg));

            newThread.SetApartmentState(ApartmentState.STA);
            newThread.IsBackground = true;
            newThread.Start(sender);
        }

        public void LoadingInShowImg(object sender)
        {
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)sender;

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                (Action)delegate() { this.ShowShowImg(image); });

        }

        public void ShowShowImg(System.Windows.Controls.Image image)
        {
            if (image.Tag != null)
            {
                XmlElement element = (XmlElement)image.Tag;
                string address = element.InnerText;
                Info.PicAddress = address;

                System.Drawing.Image temp = System.Drawing.Image.FromStream(WebRequest.Create(address).GetResponse().GetResponseStream());
                double height = temp.Height;
                double width = temp.Width;

                double screenH = SystemParameters.PrimaryScreenHeight;
                double screenW = SystemParameters.PrimaryScreenWidth;

                ImageLook viewer = new ImageLook();
                if (height > screenH || address.EndsWith(".gif"))
                {
                    address = address.Replace("bmiddle", "large");
                    System.Diagnostics.Process.Start(address);
                    GifLoadingPic.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (Action)delegate() { GifLoadingPic.Visibility = Visibility.Hidden; });
                }
                else
                {
                    viewer.Width = width;
                    viewer.Height = height;
                    viewer.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                    GifLoadingPic.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (Action)delegate() { GifLoadingPic.Visibility = Visibility.Hidden; });
                    ShowWindow(viewer);
                }
            }
        }

        private void Image_entity_Initialized(object sender, EventArgs e)
        {
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)sender;
            XmlElement element = (XmlElement)image.Tag;

            if (image.Tag == null || element.InnerText.Equals(""))
            {
                image.Source = null;
            }
        }

        public void ShowWindow(Window window)
        {
            window.Owner = this;
            window.Show();
        }

        private void ChangeToMention(object sender, MouseButtonEventArgs e)
        {
            this.FourthPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.FirstPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.SecondPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.ThridPolygon.Visibility = System.Windows.Visibility.Visible;
            this.FifthPolygon.Visibility = System.Windows.Visibility.Hidden;

            XmlDocument document = new XmlDocument();
            document.Load("Mentions.xml");
            XmlNodeList nodelist = document.SelectNodes("//statusNightOwl");
            this.MainListBox.ItemsSource = nodelist;
        }

        private void ChangeToMention()
        {
            this.FourthPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.FirstPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.SecondPolygon.Visibility = System.Windows.Visibility.Hidden;
            this.ThridPolygon.Visibility = System.Windows.Visibility.Visible;
            this.FifthPolygon.Visibility = System.Windows.Visibility.Hidden;

            XmlDocument document = new XmlDocument();
            document.Load("Mentions.xml");
            XmlNodeList nodelist = document.SelectNodes("//statusNightOwl");
            this.MainListBox.ItemsSource = nodelist;
        }


        private void atMessagePic_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Thread newThread = new Thread(new ThreadStart(atMessageProcess));
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.IsBackground = true;
            newThread.Start();
            GifLoadingPic.Visibility = System.Windows.Visibility.Visible;
        }

        public void atMessageProcess()
        {
            bool AuthResult = oauth.ClientLogin(Info.Username, Info.Pwd);
            if (AuthResult)
            {
                MainListBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            (Action)delegate() { MainListBox.Visibility = Visibility.Hidden; });

                var Sina = new NetDimension.Weibo.Client(oauth);
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
                Thread.Sleep(2000);

                Sina.API.Account.SetCount(ResetCountType.mention_status);

                atMsgLabel.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
        (Action)delegate() { atMsgLabel.Content = ""; });

                atMessagePic.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            (Action)delegate() { atMessagePic.Visibility = Visibility.Hidden; });

                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (Action)delegate() { this.ChangeToMention(); });
            }
            GifLoadingPic.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)delegate() { GifLoadingPic.Visibility = Visibility.Hidden; });

            MainListBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)delegate() { MainListBox.Visibility = Visibility.Visible; });
        }

        private void commentcount_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Thread newThread = new Thread(new ThreadStart(commentcountProcess));
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.IsBackground = true;
            newThread.Start();
            GifLoadingPic.Visibility = System.Windows.Visibility.Visible;
        }

        public void commentcountProcess()
        {
            bool AuthResult = oauth.ClientLogin(Info.Username, Info.Pwd);
            if (AuthResult)
            {
                var Sina = new NetDimension.Weibo.Client(oauth);
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

                Thread.Sleep(2000);

                Sina.API.Account.SetCount(ResetCountType.cmt);
                commentcount.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)delegate() { commentcount.Visibility = Visibility.Hidden; });


                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (Action)delegate() { this.ChangeToCommentTimeLine(); });

            }
            GifLoadingPic.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
               (Action)delegate() { GifLoadingPic.Visibility = Visibility.Hidden; });

            MainListBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)delegate() { MainListBox.Visibility = Visibility.Visible; });

        }

        private void Image_link_Initialized(object sender, EventArgs e)
        {
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)sender;
            XmlElement element = (XmlElement)image.Tag;

            if (!element.InnerText.Contains("http://t.cn") && !element.InnerText.Contains("http://sinaurl.cn"))
            {
                image.Source = null;
            }
        }

        private void Image_link_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)sender;
            XmlElement element = (XmlElement)image.Tag;

            Regex regex = new Regex(@"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?");
            Match match = regex.Match(element.InnerText);
            System.Diagnostics.Process.Start("iexplore.exe", match.Value);
        }

        private void DropImage(object sender, DragEventArgs e)
        {
            string[] fileName = e.Data.GetData(DataFormats.FileDrop, true) as string[];

            if (fileName.Length > 0)
            {
                long fileLength = 0;
                string filePath = fileName[0].ToLower();
                FileInfo fi = new FileInfo(filePath);

                string extension = fi.Extension;
                if (extension != "")
                {
                    fileLength = fi.Length;
                }

                if (extension == ".jpg" || extension == ".gif" || extension == ".png" && fileLength < 5000000)
                {
                    UploadBorder.BorderBrush = null;
                    UploadPIC.ImageSource = new BitmapImage(new Uri(filePath));
                    clearImg.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    Common.ShowCustomPopupWindow("Please Drag the JPG, GIF, PNG file and less then 5M!", this);
                }
            }
        }

        private void clearImg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            UploadPIC.ImageSource = null;
            UploadBorder.BorderBrush = System.Windows.Media.Brushes.DarkGray;
            clearImg.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Fav_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)sender;
            XmlElement element = (XmlElement)image.Tag;
            string id = element.SelectSingleNode("id").InnerText;

            bool AuthResult = oauth.ClientLogin(Info.Username, Info.Pwd);
            if (AuthResult)
            {
                var Sina = new NetDimension.Weibo.Client(oauth);
                NetDimension.Weibo.Entities.favorite.Entity newFav = Sina.API.Favorites.Create(id);
                if (!newFav.Status.ID.Equals("0"))
                {
                    Common.ShowCustomPopupWindow("Marked Favorites", this);
                }
                else
                {
                    Common.ShowCustomPopupWindow("Marked Failed", this);
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Application.Current.Shutdown();
            this.Close();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != System.Windows.WindowState.Normal)
            {
                this.WindowState = System.Windows.WindowState.Normal;
                this.ShowInTaskbar = true;
            }
        }

        private void MyNotification_DoubleDouble(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != System.Windows.WindowState.Normal)
            {
                this.WindowState = System.Windows.WindowState.Normal;
                this.ShowInTaskbar = true;
            }
        }

        private void biaoqing_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (myEmo.Visibility != System.Windows.Visibility.Visible)
            {
                myEmo.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                myEmo.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void Home_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Thread newThread = new Thread(new ParameterizedThreadStart(WaitHomePage));
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.IsBackground = true;
            newThread.Start(sender);
            GifLoadingPic.Visibility = System.Windows.Visibility.Visible;
        }

        public void WaitHomePage(object parm)
        {
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)parm;
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                (Action)delegate()
                {
                    XmlElement element = (XmlElement)image.Tag;
                    UserInfo.Id = element.SelectSingleNode("id").InnerText;
                    UserInfo.UserName = element.SelectSingleNode("screen_name").InnerText;
                    UserInfo.UserImg = element.SelectSingleNode("profile_image_url").InnerText;
                    UserInfo.Location = element.SelectSingleNode("location").InnerText;
                    UserInfo.Entries = element.SelectSingleNode("statuses_count").InnerText;
                    UserInfo.Guanzhu = element.SelectSingleNode("friends_count").InnerText;
                    UserInfo.Fensi = element.SelectSingleNode("followers_count").InnerText;
                    //UserInfo.Description = element.SelectSingleNode("description").InnerText;
                    UserInfo.Gender = element.SelectSingleNode("gender").InnerText;

                    UserWindow userWindow = new UserWindow();
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (Action)delegate()
                    {
                        userWindow.Owner = this;
                        userWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                        userWindow.Show();
                    });
                    GifLoadingPic.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (Action)delegate() { GifLoadingPic.Visibility = Visibility.Hidden; });
                });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindow configW = new ConfigWindow();
            configW.Owner = this;
            configW.Show();
        }

        private void Guanzhushu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Display disWindow = new Display("guanzhu");
            disWindow.Owner = this;
            disWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            disWindow.Show();
        }

        private void Fensishu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Display disWindow = new Display("fensi");
            disWindow.Owner = this;
            disWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            disWindow.Show();
        }

        private void atCommentsIMG_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Thread newThread = new Thread(new ThreadStart(atCommentsProcess));
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.IsBackground = true;
            newThread.Start();
            GifLoadingPic.Visibility = System.Windows.Visibility.Visible;
        }

        private void atCommentsProcess()
        {
            bool AuthResult = oauth.ClientLogin(Info.Username, Info.Pwd);
            if (AuthResult)
            {
                var Sina = new NetDimension.Weibo.Client(oauth);
                var json2 = Sina.API.Comments.Mentions(count: 50);
                string atCommentsTimelineText = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                atCommentsTimelineText += @"<comments>";
                foreach (var entity in json2.Comments)
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

                Thread.Sleep(2000);

                Sina.API.Account.SetCount(ResetCountType.mention_cmt);
                commentcount.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)delegate() { newAtCommentLabel.Content = ""; });

                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (Action)delegate() { this.ChangeToAtCommentTimeLine(); });

            }
            GifLoadingPic.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
               (Action)delegate() { GifLoadingPic.Visibility = Visibility.Hidden; });

            MainListBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)delegate() { MainListBox.Visibility = Visibility.Visible; });
        }

        private void ShowFavBtn_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.FavBorder.Visibility == System.Windows.Visibility.Hidden)
            {
                this.FavBorder.Visibility = System.Windows.Visibility.Visible;
                this.ShowFavBtn.Content = "Close Fav";
                if (FavListBox.Items.Count == 0)
                {
                    bool AuthResult = oauth.ClientLogin(Info.Username, Info.Pwd);
                    if (AuthResult)
                    {
                        var Sina = new NetDimension.Weibo.Client(oauth);
                        NetDimension.Weibo.Entities.favorite.Collection collection = Sina.API.Favorites.Favorites(count: 50);
                        List<Status> Statuses = new List<Status>();
                        foreach (NetDimension.Weibo.Entities.favorite.Entity entity in collection.Favorites)
                        {
                            if (!entity.Status.CreatedAt.Equals(""))
                            {
                                if (entity.Status.RetweetedStatus != null)
                                {
                                    Statuses.Add(new Status
                                    {
                                        time = NetDimension.Weibo.Utility.ParseUTCDate(entity.Status.CreatedAt).ToString(),
                                        id = entity.Status.ID,
                                        user_name = entity.Status.User.ScreenName,
                                        user_image = entity.Status.User.ProfileImageUrl,
                                        text = entity.Status.Text,
                                        comments_count = entity.Status.CommentsCount.ToString(),
                                        reposts_count = entity.Status.RepostsCount.ToString(),
                                        image = entity.Status.ThumbnailPictureUrl,
                                        source = entity.Status.Source.Split('>')[1].Split('<')[0],
                                        retweeted_image = entity.Status.RetweetedStatus.MiddleSizePictureUrl,
                                        retweeted_status_text = entity.Status.RetweetedStatus.Text,
                                        retweeted_status_id = entity.Status.RetweetedStatus.ID,
                                        retweeted_status_time = NetDimension.Weibo.Utility.ParseUTCDate(entity.Status.RetweetedStatus.CreatedAt).ToString(),
                                        retweeted_status_user_name = entity.Status.RetweetedStatus.User.ScreenName,
                                        retweeted_status_user_image = entity.Status.RetweetedStatus.User.ProfileImageUrl,
                                        retweeted_source = entity.Status.RetweetedStatus.Source.Split('>')[1].Split('<')[0],
                                    });
                                }
                                else
                                {
                                    Statuses.Add(new Status
                                    {
                                        time = NetDimension.Weibo.Utility.ParseUTCDate(entity.Status.CreatedAt).ToString(),
                                        id = entity.Status.ID,
                                        text = entity.Status.Text,
                                        user_name = entity.Status.User.ScreenName,
                                        user_image = entity.Status.User.ProfileImageUrl,
                                        comments_count = entity.Status.CommentsCount.ToString(),
                                        reposts_count = entity.Status.RepostsCount.ToString(),
                                        image = entity.Status.MiddleSizePictureUrl,
                                        source = entity.Status.Source.Split('>')[1].Split('<')[0],
                                    });
                                }
                            }
                        }
                        FavListBox.ItemsSource = Statuses;
                    }
                }
            }
            else
            {
                this.FavBorder.Visibility = System.Windows.Visibility.Hidden;
                this.ShowFavBtn.Content = "Show Fav";
            }
        }
    }
}
