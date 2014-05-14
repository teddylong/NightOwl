using NetDimension.Weibo;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using GifImageLib;

namespace NightOwl
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {

        string result = "";
        OAuth oauth = new OAuth("4107916330", "d64d9caea923f51684318b81b6c0bfed", @"https://api.weibo.com/oauth2/default.html");
        public UserWindow()
        {
            InitializeComponent();
            SendMessageBtn.Visibility = System.Windows.Visibility.Hidden;
            SendMessageCallOut.Visibility = System.Windows.Visibility.Hidden;
            SendMessageText.Visibility = System.Windows.Visibility.Hidden;
            SendMessageOK.Visibility = System.Windows.Visibility.Hidden;
            showPanel();
        }

        public void showPanel()
        {
            string picAddress = UserInfo.UserImg;
            picAddress = picAddress.Replace("/50/", "/180/");
            UserWindowPic.ImageSource = new BitmapImage(new Uri(picAddress));
            screen_name.Text = UserInfo.UserName;
            location.Text = UserInfo.Location;
            description.Text = UserInfo.Description;
            friends.Text = UserInfo.Guanzhu;
            followers.Text = UserInfo.Fensi;
            statuses.Text = UserInfo.Entries;
            if (UserInfo.Gender == "f")
            {
                genderPic.Source = new BitmapImage(new Uri(@"pics\Woman.png", UriKind.Relative));
            }
            else
            {
                genderPic.Source = new BitmapImage(new Uri(@"pics\baby.png", UriKind.Relative));
            }

            bool AuthResult = oauth.ClientLogin(Info.Username, Info.Pwd);
            
            if (AuthResult)
            {
                var Sina = new NetDimension.Weibo.Client(oauth);
                NetDimension.Weibo.Entities.status.Collection collection = Sina.API.Statuses.UserTimeline(screenName: UserInfo.UserName, count: 50);
                List<Status> Statuses = new List<Status>();
                foreach (NetDimension.Weibo.Entities.status.Entity entity in collection.Statuses)
                {
                    if (entity.RetweetedStatus != null)
                    {
                        Statuses.Add(new Status
                        {
                            time = NetDimension.Weibo.Utility.ParseUTCDate(entity.CreatedAt).ToString(),
                            id = entity.ID,
                            user_name = entity.User.ScreenName,
                            user_image = entity.User.ProfileImageUrl,
                            text = entity.Text,
                            comments_count = entity.CommentsCount.ToString(),
                            reposts_count = entity.RepostsCount.ToString(),
                            image = entity.ThumbnailPictureUrl,
                            source = entity.Source.Split('>')[1].Split('<')[0],
                            retweeted_image = entity.RetweetedStatus.MiddleSizePictureUrl,
                            retweeted_status_text = entity.RetweetedStatus.Text,
                            retweeted_status_id = entity.RetweetedStatus.ID,
                            retweeted_status_time = NetDimension.Weibo.Utility.ParseUTCDate(entity.RetweetedStatus.CreatedAt).ToString(),
                            retweeted_status_user_name = entity.RetweetedStatus.User.ScreenName,
                            retweeted_status_user_image = entity.RetweetedStatus.User.ProfileImageUrl,
                            retweeted_source = entity.RetweetedStatus.Source.Split('>')[1].Split('<')[0],
                        });
                    }
                    else
                    {
                        Statuses.Add(new Status
                        {
                            time = NetDimension.Weibo.Utility.ParseUTCDate(entity.CreatedAt).ToString(),
                            id = entity.ID,
                            text = entity.Text,
                            user_name = entity.User.ScreenName,
                            user_image = entity.User.ProfileImageUrl,
                            comments_count = entity.CommentsCount.ToString(),
                            reposts_count = entity.RepostsCount.ToString(),
                            image = entity.MiddleSizePictureUrl,
                            source = entity.Source.Split('>')[1].Split('<')[0],
                        });
                    }
                }
                MainListBox.ItemsSource = Statuses;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SendMessagePic_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SendMessageBtn.Visibility = System.Windows.Visibility.Visible;
            SendMessageCallOut.Visibility = System.Windows.Visibility.Visible;
            SendMessageText.Visibility = System.Windows.Visibility.Visible;
            SendMessageOK.Visibility = System.Windows.Visibility.Visible;
        }

        private void SendMessageBtn_Click(object sender, RoutedEventArgs e)
        {
            Sina.Api.SinaApiService api = new Sina.Api.SinaApiService();
            bool isAuth = api.oAuthDesktop(Info.Username, Info.Pwd);
            if (isAuth)
            {
                
                result = api.newDirectMessage(UserInfo.Id, SendMessageText.Text);
                if (result != null && result != "")
                {
                    Common.ShowCustomPopupWindow("Send successfully!", this);
                    SendMessageBtn.Visibility = System.Windows.Visibility.Hidden;
                    SendMessageCallOut.Visibility = System.Windows.Visibility.Hidden;
                    SendMessageText.Visibility = System.Windows.Visibility.Hidden;
                    SendMessageOK.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    Common.ShowCustomPopupWindow("Send Failed!", this);
                }
            }
        }

        private void SendMessageOK_Click(object sender, RoutedEventArgs e)
        {
            if (result != "" && result !=null)
            {
                SendMessageText.Text = "";
            }
            SendMessageBtn.Visibility = System.Windows.Visibility.Hidden;
            SendMessageCallOut.Visibility = System.Windows.Visibility.Hidden;
            SendMessageText.Visibility = System.Windows.Visibility.Hidden;
            SendMessageOK.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Image_comment_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            //GifLoadingPic.Visibility = System.Windows.Visibility.Visible;
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
                    string entityID  = btn.Tag.ToString();
                    bool AuthResult = oauth.ClientLogin(Info.Username, Info.Pwd);
                    if (AuthResult)
                    {
                        var Sina = new NetDimension.Weibo.Client(oauth);
                        NetDimension.Weibo.Entities.status.Entity entity = Sina.API.Statuses.Show(entityID);
                        Info.MessageID = entity.ID;
                        Info.MessageText = entity.Text;
                        Info.RepostCount = entity.RepostsCount.ToString();
                        Info.CommentCount = entity.CommentsCount.ToString();
                    }

                    CommentWindow cw = new CommentWindow();

                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (Action)delegate() { ShowWindow(cw); });
                    //GifLoadingPic.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    //    (Action)delegate() { GifLoadingPic.Visibility = Visibility.Hidden; });
                });
        }
        public void ShowWindow(Window window)
        {
            window.Owner = this;
            window.Show();
        }

        private void Image_forward_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {

            //GifLoadingPic.Visibility = System.Windows.Visibility.Visible;
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
                    string entityID = btn.Tag.ToString();
                    bool AuthResult = oauth.ClientLogin(Info.Username, Info.Pwd);
                    if (AuthResult)
                    {
                        var Sina = new NetDimension.Weibo.Client(oauth);
                        NetDimension.Weibo.Entities.status.Entity entity = Sina.API.Statuses.Show(entityID);
                        Info.MessageID = entity.ID;
                        Info.MessageText = entity.Text;
                        Info.RepostCount = entity.RepostsCount.ToString();
                        Info.CommentCount = entity.CommentsCount.ToString();
                    }

                    ForwardWindow fw = new ForwardWindow();
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (Action)delegate() { ShowWindow(fw); });

                    //GifLoadingPic.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    //    (Action)delegate() { GifLoadingPic.Visibility = Visibility.Hidden; });
                });
        }

        private void Favorites_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)sender;
            string entityID = image.Tag.ToString();
            bool AuthResult = oauth.ClientLogin(Info.Username, Info.Pwd);
            if (AuthResult)
            {
                var Sina = new NetDimension.Weibo.Client(oauth);
                NetDimension.Weibo.Entities.favorite.Entity newFav = Sina.API.Favorites.Create(entityID);
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

        private void Border_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
