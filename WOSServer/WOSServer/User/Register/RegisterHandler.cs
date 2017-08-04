using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbsNet;
using WOSServer.Cache;
using MessageClass;
using WOSDatabaseTool.Database.WOSDatabaseOP.AccountOP;
using ProtocolLibrary;

namespace WOSServer.User.Register
{
    public class RegisterHandler : AbsOnceHnadler, Interface.HandlerInterface
    {
        private AccountCache cacheInstance;
        private UserCache userCacheInstance;
        public RegisterHandler()
        {
            cacheInstance = AccountCache.Instance;
            userCacheInstance = UserCache.Instance;
        }

        public void ClientClose(UserToken token)
        {
            //Do Nothing
        }

        public void MessageReceive(UserToken token, MSGModel message)
        {
            RegisterUserInfo userInfo = message.GetMessage<RegisterUserInfo>();
            ExecutePool.Instance.execute(delegate ()
            {
                int res = Register(userInfo);
                write(token, res);
            });
        }

        private int Register(RegisterUserInfo userInfo)
        {
            if (userInfo.Username == null ||
                userInfo.Password == null ||
                userInfo.Nickname == null ||
                userInfo.Username.Equals("")||
                userInfo.Password.Equals("")||
                userInfo.Nickname.Equals("")||
                cacheInstance.HasAccount(userInfo.Username))
            {
                return Protocol.COMMAND_REGISTER_FAIL;
            }

            //写入数据库
            AccountTable.AddNewAccount(userInfo);
            cacheInstance.Add(userInfo.Username, userInfo.Password);
            userCacheInstance.addUser(userInfo);
            return Protocol.COMMAND_REGISTER_SUCCESS;
        }

        public override int getType()
        {
            return Protocol.TYPE_USER;
        }

        public override int getArea()
        {
            return Protocol.AREA_REGISTER;
        }
    }
}
