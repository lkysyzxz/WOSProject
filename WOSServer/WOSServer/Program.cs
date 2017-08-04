using AbsNet;
using AbsNet.Abs;
using AbsNet.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WOSDatabaseTool.Database.WOSDatabaseOP.AccountOP;
using WOSServer.Cache;
using WOSServer.FightRoom;

namespace WOSServer
{
    
    class Program
    {
        static void Main(string[] args)
        {
            MessageHandler MH = new MessageHandler();
            AccountCache.Instance.Add(AccountTable.GetAccounts());
            UserCache.Instance.addUser(AccountTable.GetUserData());
            RoomManager.Instance.init();
            ServerCenter serverCenter = new ServerCenter(4051, 1000, 1024, MH);
            Console.Read();
        }
    }
}
