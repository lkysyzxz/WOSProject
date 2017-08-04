using AbsNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOSServer.Cache
{
    interface IAccountCache
    {
        bool HasAccount(string username);

        bool Match(string username, string password);

        bool IsOnline(string username);

        int GetID(UserToken token);

        void Online(UserToken token, string username);

        void Offline(UserToken token);

        string GetOnlineUserName(UserToken token);
    }
}
