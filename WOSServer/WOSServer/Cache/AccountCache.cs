using AbsNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOSServer.Cache
{
    public class AccountCache : IAccountCache
    {
        private static AccountCache _accountCache;
        private static object syncRoot = new Object();
        public static AccountCache Instance
        {
            get
            {
                if (_accountCache == null)
                {
                    lock (syncRoot)
                    {
                        if (_accountCache == null)
                        {
                            _accountCache = new AccountCache();
                        }
                    }
                }
                return _accountCache;
            }
        }


        private Dictionary<UserToken, string> onlineAccountMap;
        private Dictionary<string, Account> accountMap;

        private int count;

        private AccountCache()
        {
            onlineAccountMap = new Dictionary<UserToken, string>();
            accountMap = new Dictionary<string, Account>();
            count = 0;
        }

        public void Add(string username, string password)
        {
            Account ace = new Account();
            ace.ID = count++;
            ace.Username = username;
            ace.Password = password;
            accountMap.Add(username, ace);
        }

        public void Add(DataTable account)
        {
            for (int i = 0; i < account.Rows.Count; i++)
            {
                string username = account.Rows[i]["username"].ToString();
                string password = account.Rows[i]["password"].ToString();
                Add(username, password);
            }
        }

        public bool HasAccount(string username)
        {
            return accountMap.ContainsKey(username);
        }

        public bool Match(string username, string password)
        {
            if (!HasAccount(username)) return false;
            return accountMap[username].Password.Equals(password);
        }

        public bool IsOnline(string username)
        {
            return onlineAccountMap.ContainsValue(username);
        }

        public int GetID(UserToken token)
        {
            if (!onlineAccountMap.ContainsKey(token)) return -1;
            return accountMap[onlineAccountMap[token]].ID;
        }

        public void Online(UserToken token, string username)
        {
            onlineAccountMap.Add(token, username);
        }

        public void Offline(UserToken token)
        {
            onlineAccountMap.Remove(token);
        }

        public string GetOnlineUserName(UserToken token)
        {
            if (onlineAccountMap.ContainsKey(token))
                return onlineAccountMap[token];
            else
                return null;
        }
    }
}
