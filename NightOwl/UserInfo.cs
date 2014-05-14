using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NightOwl
{
    public class UserInfo
    {
        private static string userName;

        public static string UserName
        {
            get { return UserInfo.userName; }
            set { UserInfo.userName = value; }
        }
        private static string userImg;

        public static string UserImg
        {
            get { return UserInfo.userImg; }
            set { UserInfo.userImg = value; }
        }
        private static string location;

        public static string Location
        {
            get { return UserInfo.location; }
            set { UserInfo.location = value; }
        }
        private static string entries;

        public static string Entries
        {
            get { return UserInfo.entries; }
            set { UserInfo.entries = value; }
        }
        private static string guanzhu;

        public static string Guanzhu
        {
            get { return UserInfo.guanzhu; }
            set { UserInfo.guanzhu = value; }
        }
        private static string fensi;

        public static string Fensi
        {
            get { return UserInfo.fensi; }
            set { UserInfo.fensi = value; }
        }
        private static string lastEntry;

        public static string LastEntry
        {
            get { return UserInfo.lastEntry; }
            set { UserInfo.lastEntry = value; }
        }

        private static string lastTime;

        public static string LastTime
        {
            get { return UserInfo.lastTime; }
            set { UserInfo.lastTime = value; }
        }

        private static string id;

        public static string Id
        {
            get { return UserInfo.id; }
            set { UserInfo.id = value; }
        }

        private static string description;

        public static string Description
        {
            get { return UserInfo.description; }
            set { UserInfo.description = value; }
        }
        private static string gender;

        public static string Gender
        {
            get { return UserInfo.gender; }
            set { UserInfo.gender = value; }
        }

        
        
    }
}
