using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbsNet;
using WOSServer.Cache;
using MessageClass;
using ProtocolLibrary;

namespace WOSServer.User.Login
{
    public class LoginHandler : AbsOnceHnadler, Interface.HandlerInterface
    {
        private AccountCache cacheInstance;
        public LoginHandler()
        {
            cacheInstance = AccountCache.Instance;
        }

        public void ClientClose(UserToken token)
        {
            //Do Nothing
        }

        public void MessageReceive(UserToken token, MSGModel message)
        {
            LoginUserInfo userinfo = message.GetMessage<LoginUserInfo>();
            ExecutePool.Instance.execute(delegate ()
            {
                int res = Login(token,userinfo.Username,userinfo.Password);
                this.write(token, res);
            });
        }

        private int Login(UserToken token,string username,string password)
        {
            if(username==null||
                password== null||
                username.Equals("")||
                password.Equals("")||
                !cacheInstance.HasAccount(username)||
                cacheInstance.IsOnline(username)||
                !cacheInstance.Match(username,password))
            {
                return Protocol.COMMAND_LOGIN_FAIL;
            }
            cacheInstance.Online(token, username);
            return Protocol.COMMAND_LOGIN_SUCCESS;
        }

        public override int getType()
        {
            return Protocol.TYPE_USER;
        }

        public override int getArea()
        {
            return Protocol.AREA_LOGIN;
        }
    }
}
