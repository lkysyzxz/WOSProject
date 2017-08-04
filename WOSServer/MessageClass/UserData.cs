using System;

namespace MessageClass
{
    [Serializable]
    public class UserData
    {
        public int id;  //玩家ID
        public string nickname; //玩家昵称
        public int level;   //玩家等级
        public int exp;     //玩家经验值
        public int winCount;    //玩家胜利场次
        public int totalCount;  //玩家游戏场次
        public UserData() { }
    }
}
