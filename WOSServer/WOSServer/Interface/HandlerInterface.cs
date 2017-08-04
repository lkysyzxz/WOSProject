using AbsNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOSServer.Interface
{
    interface HandlerInterface
    {
        void MessageReceive(UserToken token, MSGModel message);
        void ClientClose(UserToken token);
    }
}
