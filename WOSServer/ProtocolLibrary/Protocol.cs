namespace ProtocolLibrary
{
    public static class Protocol
    {

        /// <summary>
        /// 用户模块
        /// </summary>
        public const int TYPE_USER = 0;

        public const int AREA_LOGIN = 0;

        public const int AREA_REGISTER = 1;

        public const int AREA_REQUESTUSERDATA = 2;

        public const int COMMAND_LOGIN_SUCCESS = 0;

        public const int COMMAND_LOGIN_FAIL = 1;

        public const int COMMAND_REGISTER_SUCCESS = 0;

        public const int COMMAND_REGISTER_FAIL = 1;

        /// <summary>
        /// 战斗匹配
        /// </summary>
        public const int TYPE_MATCH = 1;

        public const int AREA_CMATH_REQ = 0;

        public const int AREA_CMATCH_LEAVE = 1;

        public const int AREA_SMATCH_SUCCESS = 2;

        public const int AREA_SMATCH_LEAVE = 3;

        /// <summary>
        /// 战斗消息
        /// </summary>
        public const int TYPE_FIGHT = 2;

        public const int AREA_C_ENTERROOM = 0;

        public const int AREA_S_ENTERSELECT = 1;

        public const int AREA_C_CONFIRMSELECT = 2;

        public const int AREA_S_ENTERLOADING = 3;

        public const int AREA_C_ENTERLOADING = 4;

        public const int AREA_S_SERVER_ENTER_INITBATTLE = 5;

        public const int AREA_C_REQ_PLAYERNICKNAME = 6;

        public const int AREA_S_RES_PLAYERNICKNAME = 7;

        public const int AREA_C_REQ_PLAYERSELECT = 8;

        public const int AREA_S_RES_PLAYERSELECT = 9;

        public const int AREA_C_REQ_HERODATA = 20;

        public const int AREA_S_RES_HERODATA = 21;

        public const int AREA_C_LOADINFOACCEPT = 10;

        public const int AREA_S_ENTERGAME = 11;

        public const int AREA_C_REQ_TEAMMAX = 12;

        public const int AREA_S_RES_TEAMMAX = 13;

        public const int AREA_C_BATTLE_MOVE = 14;

        public const int AREA_S_BATTLE_MOVE = 15;

        public const int AREA_C_BATTLE_POS = 16;

        public const int AREA_S_BATTLE_POS = 17;

        public const int AREA_C_BATTLE_HERO_ATTACK = 18;

        public const int AREA_S_BATTLE_HERO_ATTACK = 19;

        public const int AREA_C_GOLD_TIMEADD = 22;

        public const int AREA_S_GOLD_TIMEADD_SUCCESS = 23;

        public const int AREA_C_REQ_BUY = 24;

        public const int AREA_S_RES_BUY_SUCCESS = 25;

        public const int AREA_S_PLAYER_BUY = 26;

        public const int AREA_C_REQ_SELL = 27;

        public const int AREA_S_PLAYER_SELL = 28;

        public const int AREA_S_RES_SELL_SUCCESS = 29;

        public const int AREA_C_REQ_TOWERDATA = 30;

        public const int AREA_S_RES_TOWERDATA = 31;

        public const int AREA_C_NPCATTACK = 32;

        public const int AREA_S_NPCATTACK = 33;

        public const int AREA_C_REQ_CRYSTALDATA = 34;

        public const int AREA_S_RES_CRYSTALDATA = 35;

        public const int AREA_C_DEMAGE = 36;

        public const int AREA_S_DEMAGE = 37;

        public const int AREA_S_HERO_DEAD = 38;

        public const int AREA_S_HERO_RESET = 39;

        public const int AREA_S_SODIER_BORN = 40;

        public const int AREA_C_REQ_BORN = 41;

        public const int AREA_S_SODIER_DEADTH = 42;

    }
}
