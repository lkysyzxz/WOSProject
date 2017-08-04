using AbsNet;
using AbsNet.Abs;
using ProtocolLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOSServer.FightRoom;
using WOSServer.Interface;
using WOSServer.Match;
using WOSServer.User;

namespace WOSServer
{
    public class MessageHandler : AbsHandlerCenter
    {

        private HandlerInterface User;
        private HandlerInterface Match;
        private HandlerInterface Fight;
        public MessageHandler()
        {
            User = new UserHandler();
            Match = new MatchHandler();
            Fight = new FightHandler();
        }

        public override void ClientClose(UserToken token, string error)
        {
            //Console.WriteLine(error);
            Match.ClientClose(token);
            User.ClientClose(token);
        }

        public override void ClientConnect(UserToken token)
        {

            Console.WriteLine("有客户端连接:" + token.ConnectSocket.AddressFamily.ToString());
        }

        public override void MessageReceive(UserToken token, object message)
        {
            MSGModel msg = message as MSGModel;
            switch (msg.type)
            {
                case Protocol.TYPE_USER:
                    User.MessageReceive(token, msg);
                    break;
                case Protocol.TYPE_MATCH:
                    Match.MessageReceive(token, msg);
                    break;
                case Protocol.TYPE_FIGHT:
                    Fight.MessageReceive(token, msg);
                    break;
            }
        }
    }

}
