using AbsNet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOSServer.FightRoom
{
    public class RoomManager
    {
        private static RoomManager _instance;
        private static object syncObj = new object();
        public static RoomManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new RoomManager(10);
                        }
                    }
                }
                return _instance;
            }
        }
        /// <summary>
        /// 房间ID到房间的映射
        /// </summary>
        public ConcurrentDictionary<int, Room> HashRoom = new ConcurrentDictionary<int, Room>();



        /// <summary>
        /// 玩家ID到房间的映射，表达的是某某玩家是否在房间内
        /// </summary>
        public ConcurrentDictionary<int, int> HashPlayer = new ConcurrentDictionary<int, int>();

        public List<Room> IdleRooms = new List<Room>();

        public List<Room> BusyRooms = new List<Room>();

        private ConcurrentInteger IDValue = new ConcurrentInteger(0);

        private int RoomCount;
        /// <summary>
        /// 初始化房间
        /// </summary>
        /// <param name="cnt">房间数量</param>
        private RoomManager(int cnt)
        {
            this.RoomCount = cnt;
        }

        public void init()
        {
            for (int i = 0; i < RoomCount; i++)
            {
                int id = IDValue.GetAndAdd();
                Room room = new Room(id);
                HashRoom.TryAdd(id, room);
                IdleRooms.Add(room);
            }
        }

        public void CheckIdleRoom(int i)
        {
            Room room = IdleRooms[i];
            if (room.teamOne.Count == room.teamTwo.Count && room.teamOne.Count == room.teamMax)
            {
                //可以开始游戏了
                IdleRooms.RemoveAt(i);
                BusyRooms.Add(room);
                room.Begin();
            }
        }
    }
}
