using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbsNet;
using ProtocolLibrary;
using WOSServer.Cache;
using WOSServer.FightRoom;

namespace WOSServer.Match
{
    public class MatchHandler : AbsOnceHnadler, Interface.HandlerInterface
    {
        public void ClientClose(UserToken token)
        {

        }

        public void MessageReceive(UserToken token, MSGModel message)
        {
            switch (message.area)
            {
                case Protocol.AREA_CMATH_REQ:
                    enter(token);
                    break;
            }
        }


        private void enter(UserToken token)
        {
            int playerID = AccountCache.Instance.GetID(token);
            if (playerID != -1)//玩家在线
            {
                //TO DO 寻找空闲房间
                //将玩家加入房间,同时添加映射ID->房间号
                //
                Room select = null;
                RoomPlayer rp = new RoomPlayer();
                rp.ID = playerID;
                rp.Token = token;
                int i = 0;
                lock (RoomManager.Instance)
                {
                    if (RoomManager.Instance.IdleRooms.Count > 0)
                    {
                        foreach (Room room in RoomManager.Instance.IdleRooms)
                        {
                            if (room.teamOne.Count < room.teamMax)
                            {
                                room.teamOne.Add(rp);
                                select = room;
                            }
                            else if (room.teamTwo.Count < room.teamMax)
                            {
                                room.teamTwo.Add(rp);
                                select = room;
                            }
                            if (select != null)
                            {
                                RoomManager.Instance.HashPlayer.TryAdd(playerID, select.ID);
                                RoomManager.Instance.CheckIdleRoom(i);
                                break;
                            }
                            i++;
                        }
                    }
                    else
                    {
                        //To Do::
                        //返回一个匹配拒绝
                    }
                }
            }
        }

        public override int getType()
        {
            return ProtocolLibrary.Protocol.TYPE_MATCH;
        }
    }
}
