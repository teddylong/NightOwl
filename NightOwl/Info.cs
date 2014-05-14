using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NightOwl
{
    public  class Info
    {
        private static string username;
        private static string pwd;
        private static string messageID;
        private static string messageText;
        private static string picAddress;
        private static string re_messageText;
        private static bool isRetweet;
        private static bool isComment;
        private static string c_id;
        private static string linkAddress;
        private static string customPopUpWindowMessage;
        private static bool isOpenMainWindow;
        private static string version;
        private static string uid;
        private static string repostCount;
        private static string commentCount;

        public static string CommentCount
        {
            get { return Info.commentCount; }
            set { Info.commentCount = value; }
        }

        public static string RepostCount
        {
            get { return Info.repostCount; }
            set { Info.repostCount = value; }
        }

        public static string UID
        {
            get { return Info.uid; }
            set { Info.uid = value; }
        }

        public static string Version
        {
            get { return Info.version; }
            set { Info.version = value; }
        }

        public static bool IsOpenMainWindow
        {
            get { return Info.isOpenMainWindow; }
            set { Info.isOpenMainWindow = value; }
        }

        public static string CustomPopUpWindowMessage
        {
            get { return Info.customPopUpWindowMessage; }
            set { Info.customPopUpWindowMessage = value; }
        }

        public static string LinkAddress
        {
            get { return Info.linkAddress; }
            set { Info.linkAddress = value; }
        }

        public static string C_id
        {
            get { return Info.c_id; }
            set { Info.c_id = value; }
        }

        public static bool IsComment
        {
            get { return Info.isComment; }
            set { Info.isComment = value; }
        }



        public static bool IsRetweet
        {
            get { return Info.isRetweet; }
            set { Info.isRetweet = value; }
        }

        public static string Re_messageText
        {
            get { return Info.re_messageText; }
            set { Info.re_messageText = value; }
        }

        public static string MessageText
        {
            get { return Info.messageText; }
            set { Info.messageText = value; }
        }
        

        public static string PicAddress
        {
            get { return Info.picAddress; }
            set { Info.picAddress = value; }
        }

        public static string MessageID
        {
            get { return messageID; }
            set { messageID = value; }
        }

        public static string Username
        {
            get { return username; }
            set { username = value; }
        }
        

        public static string Pwd
        {
            get { return pwd; }
            set { pwd = value; }
        }

        
    }
}
