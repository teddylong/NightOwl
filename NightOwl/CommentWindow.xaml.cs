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
    /// Interaction logic for CommentWindow.xaml
    /// </summary>
    public partial class CommentWindow : Window
    {
        OAuth oauth = new OAuth("4107916330", "d64d9caea923f51684318b81b6c0bfed", @"https://api.weibo.com/oauth2/default.html");
        
        public CommentWindow()
        {
            InitializeComponent();
            TextFromComment.Text = Info.MessageText;
            ShowCountAndDetail(Info.MessageID);
            FocusManager.SetFocusedElement(this, comment);
            Keyboard.Focus(comment);        
        }

        private void go_Click(object sender, RoutedEventArgs e)
        {
            string id = Info.MessageID;
            string commentString = comment.Text;
            string cid = Info.C_id;
            CommentToEntity(id, commentString, cid);
        }

        public void ShowCountAndDetail(string id)
        {
            bool AuthResult = oauth.ClientLogin(Info.Username, Info.Pwd);
            List<Comments> comments = new List<Comments>();
            if (AuthResult)
            {
                Comment_label.Content = Info.CommentCount;
                Forward_label.Content = Info.RepostCount;

                var Sina = new NetDimension.Weibo.Client(oauth);
                NetDimension.Weibo.Entities.comment.Collection commentsCollection = Sina.API.Comments.Show(id);
                foreach (NetDimension.Weibo.Entities.comment.Entity  entity in commentsCollection.Comments)
                {
                    comments.Add(new Comments { id = entity.ID, text = entity.Text,  time = NetDimension.Weibo.Utility.ParseUTCDate(entity.CreatedAt).ToString(), userImg = entity.User.ProfileImageUrl, userName = entity.User.ScreenName });
                }
                CommentList.ItemsSource = comments;
            }
        }

        public void CommentToEntity(string id, string comment, string cid)
        {
            bool AuthResult = oauth.ClientLogin(Info.Username, Info.Pwd);

            if (AuthResult)
            {
                var Sina = new NetDimension.Weibo.Client(oauth);
                if (CheckBoxForward.IsChecked.Value)
                { 
                    NetDimension.Weibo.Entities.status.Entity forwardEntity = Sina.API.Statuses.Repost(id, comment, 1);
                    if (forwardEntity.ID.Equals("0"))
                    {
                        Common.ShowCustomPopupWindow("Comment & Repost Failed!", this);
                    }
                    else
                    {
                        Common.ShowCustomPopupWindow("Comment & Repost Successfully!", this);
                    }
                }
                else
                {
                    NetDimension.Weibo.Entities.comment.Entity commentEntity = Sina.API.Comments.Create(id, comment);
                    if (commentEntity.ID.Equals("0"))
                    {
                        Common.ShowCustomPopupWindow("Comment Failed!", this);
                    }
                    else
                    {
                        Common.ShowCustomPopupWindow("Comment Successfully!", this);
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

        private void comment_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (comment.Text.Length <= 140)
            {
                LabelCount.Content = (140 - comment.Text.Length);
                go.IsEnabled = true;
            }
            else
            {
                comment.MaxLength = comment.Text.Length - 1;
                LabelCount.Content = -(comment.Text.Length - 140);
                go.IsEnabled = false;
            }
        }
    }
}
