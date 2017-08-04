using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbsNet;
using ProtocolLibrary;
using WOSServer.Cache;

namespace WOSServer.FightRoom
{
    public class FightHandler : Interface.HandlerInterface
    {
        public void ClientClose(UserToken token)
        {
            int playerID = AccountCache.Instance.GetID(token);
            if (playerID == -1) return;
            int roomID = RoomManager.Instance.HashPlayer[playerID];
            if (roomID != -1)
            {
                RoomManager.Instance.HashRoom[roomID].ClientClose(token);
            }
        }

        public void MessageReceive(UserToken token, MSGModel message)
        {
            DistributesMeesageToRoom(token, message);
        }
        
        public void DistributesMeesageToRoom(UserToken token,MSGModel message)
        {
            int roomid = message.command;
            Room room = null;
            RoomManager.Instance.HashRoom.TryGetValue(roomid, out room);
            if (room == null)
            {
                throw new Exception("房间不存在");
            }
            room.MessageReceive(token, message);
        }
    }
}
