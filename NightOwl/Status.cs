using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NightOwl
{
    public class Status
    {
        public string time { set; get; }
        public string id { set; get; }
        public string text { set; get; }
        public string user_name { set; get; }
        public string user_image { set; get; }
        public string image { set; get; }
        public string reposts_count { set; get; }
        public string comments_count { set; get; }
        public string source { set; get; }

        public string retweeted_status_time { set; get; }
        public string retweeted_status_id { set; get; }
        public string retweeted_status_text { set; get; }
        public string retweeted_status_user_name { set; get; }
        public string retweeted_status_user_image { set; get; }
        public string retweeted_image { set; get; }
        public string retweeted_source { set; get; }
        
    }
}
