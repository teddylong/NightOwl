using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NetDimension.Weibo;
using System.ComponentModel;

namespace NightOwl
{
    /// <summary>
    /// Interaction logic for Display.xaml
    /// </summary>
    public partial class Display : Window
    {
        OAuth oauth = new OAuth("4107916330", "d64d9caea923f51684318b81b6c0bfed", @"https://api.weibo.com/oauth2/default.html");
        public Display(string name)
        {
            InitializeComponent();
            if (name == "guanzhu")
            {
                ShowPanel("guanzhu");
                this.disLabel.Content = "Friends List";
            }
            else
            {
                ShowPanel("fensi");
                this.disLabel.Content = "Followers List";
            }       
        }

        private void ShowPanel(string type)
        { 
            bool AuthResult = oauth.ClientLogin(Info.Username, Info.Pwd);
            if (AuthResult)
            {
                var Sina = new NetDimension.Weibo.Client(oauth);
                var uid = Sina.API.Account.GetUID();
                NetDimension.Weibo.Entities.user.Collection people = null;
                if (type == "guanzhu")
                {
                    people = Sina.API.Friendships.Friends(uid, null, 200);
                }
                else
                {
                    people = Sina.API.Friendships.Followers(uid, null, 200);
                }
                List<User> users = new List<User>();
                foreach(NetDimension.Weibo.Entities.user.Entity user in people.Users)
                {
                    users.Add(new User { screen_name = user.ScreenName, description = user.Description, gender = user.Gender, profile_image_url = user.ProfileImageUrl, followers_count=user.FollowersCount,friends_count=user.FriendsCount});
                }
                disPlayListView.ItemsSource = users;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
