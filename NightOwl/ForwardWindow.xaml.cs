using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sina.Api;
using System.Xml;
using NetDimension.Weibo;
using System.Collections.Generic;

namespace NightOwl
{
    /// <summary>
    /// Interaction logic for Forward.xaml
    /// </summary>
    public partial class ForwardWindow : Window
    {
        OAuth oauth = new OAuth("4107916330", "d64d9caea923f51684318b81b6c0bfed", @"https://api.weibo.com/oauth2/default.html");
        public ForwardWindow()
        {
            InitializeComponent();
            TextFromComment_forward.Text = Info.MessageText;
            ShowCountAndDetail(Info.MessageID);
            FocusManager.SetFocusedElement(this, forwardRichTextbox);
            Keyboard.Focus(forwardRichTextbox);
        }

        private void go_Click(object sender, RoutedEventArgs e)
        {
            string id = Info.MessageID;
            string forwardString = forwardRichTextbox.Text;
            ForwardToEntity(id, forwardString);
        }

        public void ShowCountAndDetail(string id)
        {
            bool AuthResult = oauth.ClientLogin(Info.Username, Info.Pwd);
            List<Forwards> forwards = new List<Forwards>();
            if (AuthResult)
            {
                Comment_label_forward.Content = Info.CommentCount;
                Forward_label_forward.Content = Info.RepostCount;

                var Sina = new NetDimension.Weibo.Client(oauth);
                NetDimension.Weibo.Entities.repost.Collection repostCollection = Sina.API.Statuses.RepostTimeline(id);
                foreach (NetDimension.Weibo.Entities.status.Entity entity in repostCollection.Statuses)
                {
                    forwards.Add(new Forwards { id = entity.ID, text = entity.Text, time = NetDimension.Weibo.Utility.ParseUTCDate(entity.CreatedAt).ToString(), userImg = entity.User.ProfileImageUrl, userName = entity.User.ScreenName });
                }
                ForwardList.ItemsSource = forwards;
            }   
        }

        public void ForwardToEntity(string id, string forward)
        {
            bool AuthResult = oauth.ClientLogin(Info.Username, Info.Pwd);
            if (AuthResult)
            {
                var Sina = new NetDimension.Weibo.Client(oauth);
                if (CheckBox_forward.IsChecked.Value)
                {
                    NetDimension.Weibo.Entities.status.Entity forwardEntity = Sina.API.Statuses.Repost(id, forward, 1);
                    if (forwardEntity.ID.Equals("0"))
                    {
                        Common.ShowCustomPopupWindow("Repost & Comment Failed!", this);
                    }
                    else
                    {
                        Common.ShowCustomPopupWindow("Repost & Comment Successfully!", this);
                    }
                }
                else
                {
                    NetDimension.Weibo.Entities.status.Entity forwardEntity = Sina.API.Statuses.Repost(id, forward, 0);
                    if (forwardEntity.ID.Equals("0"))
                    {
                        Common.ShowCustomPopupWindow("Repost Failed!", this);
                    }
                    else
                    {
                        Common.ShowCustomPopupWindow("Repost Successfully!", this);
                    }
                }
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void forwardRichTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (forwardRichTextbox.Text.Length <= 140)
            {
                LabelCount.Content = (140 - forwardRichTextbox.Text.Length);
                go_forward.IsEnabled = true;
            }
            else
            {
                forwardRichTextbox.MaxLength = forwardRichTextbox.Text.Length - 1;
                LabelCount.Content = -(forwardRichTextbox.Text.Length - 140);
                go_forward.IsEnabled = false;
            }
        }
    }
}
