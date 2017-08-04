using AbsNet;
using AbsNet.Tools;
using MessageClass;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WOSServer.FightRoom
{
    
    public class Room:AbsOnceHnadler,Interface.HandlerInterface
    {
        public int ID;

        public int teamMax = 1;

        internal AbsRoomState roomState;

        internal bool isRun = false;
        /// <summary>
        /// 在该房间内的玩家ID
        /// </summary>
        public List<RoomPlayer> teamOne = new List<RoomPlayer>();

        public List<RoomPlayer> teamTwo = new List<RoomPlayer>();

        

        public ConcurrentQueue<MSGModel> roomMessages = new ConcurrentQueue<MSGModel>();

        /// <summary>
        /// 房间中玩家的信息
        /// </summary>
        public List<UserData> teamOneUserInfo = new List<UserData>();
        public List<UserData> teamTwoUserInfo = new List<UserData>();

        /// <summary>
        /// 玩家选择英雄的信息
        /// </summary>
        public int[] teamOneHeroSelect;
        public int[] teamTwoHeroSelect;


        public Queue<SodierActorModel> sodiersPool = new Queue<SodierActorModel>();
        /// <summary>
        /// 保存世界中每一个物体的字典
        /// </summary>
        public ConcurrentDictionary<int, AbsActorModel> worldAcotr = new ConcurrentDictionary<int, AbsActorModel>();


        public Room(int id)
        {
            this.ID = id;
            roomState = new IdleRoomState(this);
            teamOneHeroSelect = new int[teamMax];
            teamTwoHeroSelect = new int[teamMax];
        }

        //游戏开始
        public void Begin()
        {
            isRun = true;
            roomState.update();
            //Thread roomThread = new Thread(new ThreadStart(Run));
            //roomThread.Start();
        }

        //public void Run()
        //{
        //    while(isRun)
        //    {
        //        roomState.update();
        //    }
        //}

        public void MessageReceive(UserToken token, MSGModel message)
        {
            roomState.MessageReceive(token, message);
        }

        public void ClientClose(UserToken token)
        {
            
        }


        public void BrocastToTeamOne(int type,int area,int command,object message)
        {
            foreach(RoomPlayer rp in teamOne)
            {
                write(rp.Token, type, area, command, message);
            }
        }

        public void BrocastToTeamTwo(int type,int area,int command,object message)
        {
            foreach (RoomPlayer rp in teamTwo)
            {
                write(rp.Token, type, area, command, message);
            }
        }

        public void Write(RoomPlayer player,int type,int area,int command,object message)
        {
            write(player.Token, type, area, command, message);
        }

        public void Write(UserToken token,int type,int area,int command,object message)
        {
            write(token, type, area, command, message);
        }

        internal void Brocast(int type, int area, int command, object msg, UserToken exclude)
        {
            MSGModel message = MSGModel.CreateMessage(type, area, command, msg);
            byte[] tmp = MessageEncoding.Encode(message);
            byte[] buffer = LengthEncoding.encode(tmp);
            foreach(RoomPlayer rp in teamOne)
            {
                if(rp.Token!=exclude)
                {
                    tmp = new byte[buffer.Length];
                    Buffer.BlockCopy(buffer, 0, tmp, 0, buffer.Length);
                    rp.Token.Write(tmp);
                }
            }


            foreach(RoomPlayer rp in teamTwo)
            {
                if(rp.Token!=exclude)
                {
                    tmp = new byte[buffer.Length];
                    Buffer.BlockCopy(buffer, 0, tmp, 0, buffer.Length);
                    rp.Token.Write(tmp);
                }
            }


        }
    }
}
