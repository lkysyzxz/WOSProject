using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbsNet;
using WOSServer.Cache;
using ProtocolLibrary;

namespace WOSServer.User.UserData
{
    class UserDataRequestHandler : AbsOnceHnadler,Interface.HandlerInterface
    {
        private IUserCache userCache = UserCache.Instance;

        public void ClientClose(UserToken token)
        {
            
        }

        public void MessageReceive(UserToken token, MSGModel message)
        {
            string username = message.GetMessage<string>();
            ExecutePool.Instance.execute(delegate ()
            {
                MessageClass.UserData userData = userCache.getUserDataByUsername(username);
                write(token, 0, userData);
            });
        }

        public override int getType()
        {
            return Protocol.TYPE_USER;
        }

        public override int getArea()
        {
            return Protocol.AREA_REQUESTUSERDATA;
        }
    }
}
