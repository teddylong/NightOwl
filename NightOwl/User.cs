using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NightOwl
{
    public class User 
    {
        public int id { get; set; }
        public string screen_name { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string profile_image_url { get; set; }
        public string domain { get; set; }
        public string gender { get; set; }
        public int followers_count { get; set; }
        public int friends_count { get; set; }
        public int statuses_count { get; set; }
        public int favourites_count { get; set; }
        public bool following { get; set; }
    }
}
