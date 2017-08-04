using AbsNet;
using MessageClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WOSServer.Cache
{
    public class UserCache:IUserCache
    {
        private static UserCache _instance;
        private static object syncObj = new object();
        public static UserCache Instance
        {
            get
            {
                if(_instance== null)
                {
                    lock (syncObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new UserCache();
                        }
                    }
                }
                return _instance;
            }
        }
        /// <summary>
        /// 账号对应的角色数据
        /// </summary>
        private Dictionary<string, UserData> username_userdata = new Dictionary<string, UserData>();

        private int index = 0;

        public void addUser(RegisterUserInfo registerInfo)
        {
            UserData userData = new UserData();
            userData.id = index++;
            userData.exp = 0;
            userData.level = 1;
            userData.nickname = registerInfo.Nickname;
            userData.totalCount = 0;
            userData.winCount = 0;

            addUser(registerInfo.Username, userData);
        }

        public void addUser(DataTable table)
        {
            for(int i=0;i<table.Rows.Count;i++)
            {
                UserData userData = new UserData();
                userData.id = index++;
                userData.nickname = table.Rows[i]["nickname"].ToString();
                userData.level = int.Parse(table.Rows[i]["level"].ToString());
                userData.exp = int.Parse(table.Rows[i]["exp"].ToString());
                userData.winCount = int.Parse(table.Rows[i]["winCount"].ToString());
                userData.totalCount = int.Parse(table.Rows[i]["totalCount"].ToString());
                string username = table.Rows[i]["username"].ToString();
                addUser(username, userData);
            }
        }

        public void addUser(string username, UserData userData)
        {
            username_userdata.Add(username, userData);
        }

        public UserData getUserDataByUsername(string username)
        {
            if (username_userdata.ContainsKey(username))
                return username_userdata[username];
            else
                return null;
        }
    }
}
