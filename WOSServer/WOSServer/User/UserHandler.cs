using AbsNet.Abs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbsNet;
using WOSServer.Cache;
using ProtocolLibrary;

namespace WOSServer.User
{
    class UserHandler :Interface.HandlerInterface
    {
        private Interface.HandlerInterface login;
        private Interface.HandlerInterface register;
        private Interface.HandlerInterface userdataRequest;
        private AccountCache accountCache;

        public UserHandler()
        {
            login = new Login.LoginHandler();
            register = new Register.RegisterHandler();
            userdataRequest = new UserData.UserDataRequestHandler();
            accountCache = AccountCache.Instance;
        }
        public void ClientClose(UserToken token)
        {
            accountCache.Offline(token);
        }

        public void MessageReceive(UserToken token, MSGModel message)
        {
            switch(message.area)
            {
                case Protocol.AREA_LOGIN:
                    Login(token, message);
                    break;
                case Protocol.AREA_REGISTER:
                    Register(token, message);
                    break;
                case Protocol.AREA_REQUESTUSERDATA:
                    RequestUserData(token, message);
                    break;
            }
        }

        private void Login(UserToken token,MSGModel message)
        {
            login.MessageReceive(token, message);
        }

        private void Register(UserToken token,MSGModel message)
        {
            register.MessageReceive(token, message);
        }

        private void RequestUserData(UserToken token,MSGModel message)
        {
            userdataRequest.MessageReceive(token, message);
        }
    }


}
