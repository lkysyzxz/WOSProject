using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbsNet;
using ProtocolLibrary;
using MessageClass;
using WOSServer.Cache;
using WOSServer.Tool;
using System.Timers;

namespace WOSServer.FightRoom
{
    public abstract class AbsRoomState : Interface.HandlerInterface
    {
        protected Room room;
        public AbsRoomState(Room room)
        {
            this.room = room;
        }

        public abstract void update();

        public abstract void nextState();
        public abstract void MessageReceive(UserToken token, MSGModel message);
        public abstract void ClientClose(UserToken token);
    }

    public class IdleRoomState : AbsRoomState
    {
        public IdleRoomState(Room room) : base(room)
        {

        }

        public override void ClientClose(UserToken token)
        {
            //TO DO NOTHING
            //throw new NotImplementedException();
        }

        public override void MessageReceive(UserToken token, MSGModel message)
        {
            //TO DO NOTHING
            //throw new NotImplementedException();
        }

        public override void nextState()
        {
            room.roomState = new WaitEnterRoomState(base.room);
        }

        public override void update()
        {
            if (room.teamOne.Count == room.teamTwo.Count && room.teamOne.Count == room.teamMax)
            {
                nextState();
            }
        }
    }


    public class WaitEnterRoomState : AbsRoomState
    {
        /// <summary>
        /// 确认进入房间的人数
        /// </summary>
        private ConcurrentInteger value = new ConcurrentInteger(0);
        public WaitEnterRoomState(Room room) : base(room)
        {

            //开启定时任务,30s后所有人未进入房间则解散房间

            //TO DO::在这里给所有房间内的玩家返回匹配成功消息   出现匹配成功界面    同时等待所有玩家加入房间
            //TO DO::给所有在房间内的玩家发送进入房间的消息
            for (int i = 0; i < room.teamOne.Count; i++)
            {
                RoomPlayer rp = room.teamOne[i];
                RoomInfo ri = new RoomInfo();
                ri.RoomID = room.ID;
                ri.PlayerTeam = 1;
                ri.PlayerNumber = i;
                room.Write(rp, Protocol.TYPE_MATCH, Protocol.AREA_SMATCH_SUCCESS, 0, ri);
            }

            for (int i = 0; i < room.teamTwo.Count; i++)
            {
                RoomPlayer rp = room.teamTwo[i];
                RoomInfo ri = new RoomInfo();
                ri.RoomID = room.ID;
                ri.PlayerTeam = 2;
                ri.PlayerNumber = i;
                room.Write(rp, Protocol.TYPE_MATCH, Protocol.AREA_SMATCH_SUCCESS, 0, ri);
            }
        }

        public override void ClientClose(UserToken token)
        {
            //throw new NotImplementedException();
        }

        public override void MessageReceive(UserToken token, MSGModel message)
        {
            //throw new NotImplementedException();
            if (message.area == Protocol.AREA_C_ENTERROOM)
            {
                value.SyncAdd();
                update();
            }
        }

        public override void nextState()
        {
            room.roomState = new SelectHeroRoomState(room);
        }

        public override void update()
        {
            //Console.WriteLine("Wait...");
            if (value.GetValue() == room.teamMax * 2)//玩家全部进入房间
            {
                nextState();
            }
        }
    }


    public class SelectHeroRoomState : AbsRoomState
    {
        private ConcurrentInteger confirmCount = new ConcurrentInteger(0);
        public SelectHeroRoomState(Room room) : base(room)
        {
            room.BrocastToTeamOne(Protocol.TYPE_FIGHT, Protocol.AREA_S_ENTERSELECT, 0, null);
            room.BrocastToTeamTwo(Protocol.TYPE_FIGHT, Protocol.AREA_S_ENTERSELECT, 0, null);
            //开启一个定时任务,30s后未选择英雄的玩家随机选择一位英雄
            //V1.0版本选择1号英雄(Ahri)
        }

        public override void ClientClose(UserToken token)
        {

        }

        public override void MessageReceive(UserToken token, MSGModel message)
        {
            if (message.area == Protocol.AREA_C_CONFIRMSELECT)
            {
                int msg = message.GetMessage<int>();
                int team = msg & 3;
                msg >>= 2;
                int playerID = msg & 7;
                msg >>= 3;
                int heroID = msg;
                RecordHeroSelect(team, playerID, heroID);
                confirmCount.SyncAdd();
                update();
            }
        }


        private void RecordHeroSelect(int team, int playerID, int heroID)
        {
            if (team == 1)
            {
                room.teamOneHeroSelect[playerID] = heroID;
            }
            else if (team == 2)
            {
                room.teamTwoHeroSelect[playerID] = heroID;
            }
        }

        public override void nextState()
        {
            room.roomState = new PreStartRoomState(room);
        }

        public override void update()
        {
            if (confirmCount.GetValue() == room.teamMax * 2)
            {
                nextState();
            }
        }
    }


    public class PreStartRoomState : AbsRoomState
    {
        private ConcurrentInteger confirmEnter = new ConcurrentInteger(0);
        public PreStartRoomState(Room room) : base(room)
        {
            room.BrocastToTeamOne(Protocol.TYPE_FIGHT, Protocol.AREA_S_ENTERLOADING, 0, null);
            room.BrocastToTeamTwo(Protocol.TYPE_FIGHT, Protocol.AREA_S_ENTERLOADING, 0, null);
        }

        public override void ClientClose(UserToken token)
        {
            //throw new NotImplementedException();
        }

        public override void MessageReceive(UserToken token, MSGModel message)
        {
            //throw new NotImplementedException();
            if (message.area == Protocol.AREA_C_ENTERLOADING)
            {
                confirmEnter.SyncAdd();
                update();
            }
        }

        public override void nextState()
        {
            room.roomState = new InitBattleRoomState(room);
        }

        public override void update()
        {
            if (confirmEnter.GetValue() == room.teamMax * 2)
            {
                nextState();
            }
        }
    }


    public class InitBattleRoomState : AbsRoomState
    {
        private ConcurrentInteger acceptCount = new ConcurrentInteger(0);

        public InitBattleRoomState(Room room) : base(room)
        {
            //初始化士兵对象池子
            for (int i = 30; i <= 1029; i++)
            {
                SodierActorModel sam = new SodierActorModel(i);
                room.sodiersPool.Enqueue(sam);
            }

            ///加载队伍玩家信息
            for (int i = 0; i < room.teamOne.Count; i++)
            {
                RoomPlayer rp = room.teamOne[i];
                UserData userData = UserCache.Instance.getUserDataByUsername(AccountCache.Instance.GetOnlineUserName(rp.Token));
                room.teamOneUserInfo.Add(userData);
            }

            for (int i = 0; i < room.teamTwo.Count; i++)
            {
                RoomPlayer rp = room.teamTwo[i];
                UserData userData = UserCache.Instance.getUserDataByUsername(AccountCache.Instance.GetOnlineUserName(rp.Token));
                room.teamTwoUserInfo.Add(userData);
            }

            //队伍1的玩家的角色序号为0到9
            for (int i = 0; i < room.teamOne.Count; i++)
            {
                room.worldAcotr.TryAdd(i, new HeroActorModel(i, HeroInitTable.Instance.heroInitInfo[room.teamOneHeroSelect[i]], 1000));
            }

            //队伍2的玩家的角色序号为10~19
            for (int i = 10; i < room.teamTwo.Count + 10; i++)
            {
                room.worldAcotr.TryAdd(i, new HeroActorModel(i, HeroInitTable.Instance.heroInitInfo[room.teamTwoHeroSelect[i - 10]], 1000));
            }


            room.worldAcotr.TryAdd(20, new TowerActorModel(20));//生成队伍1防御塔
            room.worldAcotr.TryAdd(-20, new TowerActorModel(-20));//生成队伍2防御塔

            room.worldAcotr.TryAdd(21, new CrystalActorModel(21));//生成队伍1水晶
            room.worldAcotr.TryAdd(-21, new CrystalActorModel(-21));//生成队伍2水晶


            room.BrocastToTeamOne(Protocol.TYPE_FIGHT, Protocol.AREA_S_SERVER_ENTER_INITBATTLE, 0, null);
            room.BrocastToTeamTwo(Protocol.TYPE_FIGHT, Protocol.AREA_S_SERVER_ENTER_INITBATTLE, 0, null);

        }

        public override void ClientClose(UserToken token)
        {

        }

        public override void MessageReceive(UserToken token, MSGModel message)
        {
            switch (message.area)
            {
                case Protocol.AREA_C_LOADINFOACCEPT:
                    ClientLoadDone();
                    break;
                case Protocol.AREA_C_REQ_TEAMMAX:
                    SendTeamMax(token);
                    break;
                case Protocol.AREA_C_REQ_PLAYERNICKNAME:
                    SendNickName(token, message);
                    break;
                case Protocol.AREA_C_REQ_PLAYERSELECT:
                    SendSelect(token, message);
                    break;
                case Protocol.AREA_C_REQ_HERODATA:
                    SendHeroData(token, message);
                    break;
                case Protocol.AREA_C_REQ_TOWERDATA:
                    SendTowerData(token, message);
                    break;
                case Protocol.AREA_C_REQ_CRYSTALDATA:
                    SendCrystalData(token, message);
                    break;
            }
        }

        private void SendCrystalData(UserToken token, MSGModel message)
        {
            int team = message.GetMessage<int>();
            int crystalID = 21;
            if (team == 1)
            {
                crystalID = 21;
            }
            else if (team == 2)
            {
                crystalID = -21;
            }
            CrystalActorModel crystal = room.worldAcotr[crystalID] as CrystalActorModel;
            CrystalData cd = new CrystalData();
            cd.Team = team;
            cd.ID = crystalID;
            cd.HP = crystal.HP;
            cd.Defence = crystal.Defence;
            cd.Attack = crystal.Attack;
            room.Write(token, Protocol.TYPE_FIGHT, Protocol.AREA_S_RES_CRYSTALDATA, 0, cd);
        }

        private void SendTowerData(UserToken token, MSGModel message)
        {
            int team = message.GetMessage<int>();//获取队伍编号
            int towerID = 20;
            switch (team)
            {
                case 1:
                    towerID = 20;
                    break;
                case 2:
                    towerID = -20;
                    break;
            }
            TowerActorModel tam = room.worldAcotr[towerID] as TowerActorModel;

            TowerData td = new TowerData();
            td.ID = towerID;
            td.Team = team;
            td.HP = tam.HP;
            td.Attack = tam.Attack;
            td.Defence = tam.Defence;
            room.Write(token, Protocol.TYPE_FIGHT, Protocol.AREA_S_RES_TOWERDATA, 0, td);
        }

        private void SendHeroData(UserToken token, MSGModel message)
        {
            int wolrdID = message.GetMessage<int>();
            HeroActorModel ham = room.worldAcotr[wolrdID] as HeroActorModel;
            HeroData data = HeroDataFactory.CreateHeroData(ham);
            room.Write(token, Protocol.TYPE_FIGHT, Protocol.AREA_S_RES_HERODATA, 0, data);
        }

        private void SendSelect(UserToken token, MSGModel message)
        {
            int msg = message.GetMessage<int>();
            int team = msg & 3;
            int playerNum = msg >> 2;
            int heroSelect = 0;
            if (team == 1)
            {
                heroSelect = room.teamOneHeroSelect[playerNum];
            }
            else if (team == 2)
            {
                heroSelect = room.teamTwoHeroSelect[playerNum];
            }
            room.Write(token, Protocol.TYPE_FIGHT, Protocol.AREA_S_RES_PLAYERSELECT, msg, heroSelect);
        }

        private void SendTeamMax(UserToken token)
        {
            room.Write(token, Protocol.TYPE_FIGHT, Protocol.AREA_S_RES_TEAMMAX, 0, room.teamMax);
        }

        private void SendNickName(UserToken token, MSGModel message)
        {
            int msg = message.GetMessage<int>();
            int team = msg & 3;
            int playerNum = msg >> 2;
            string nickname = null;
            if (team == 1)
            {
                nickname = room.teamOneUserInfo[playerNum].nickname;
            }
            else if (team == 2)
            {
                nickname = room.teamTwoUserInfo[playerNum].nickname;
            }
            room.Write(token, Protocol.TYPE_FIGHT, Protocol.AREA_S_RES_PLAYERNICKNAME, msg, nickname);

        }

        private void ClientLoadDone()
        {
            acceptCount.SyncAdd();
            update();
        }

        public override void nextState()
        {
            room.roomState = new BattleRoomState(room);
        }

        public override void update()
        {
            if (acceptCount.GetValue() == room.teamMax * 2)
            {
                nextState();
            }
        }
    }


    public class BattleRoomState : AbsRoomState
    {
        private bool Runing;
        private Timer sodierTimer;
        public BattleRoomState(Room room) : base(room)
        {
            Runing = true;
            sodierTimer = new Timer(20000);
            sodierTimer.Elapsed += SodierBornEvent;
            sodierTimer.Start();
            room.BrocastToTeamOne(Protocol.TYPE_FIGHT, Protocol.AREA_S_ENTERGAME, 0, null);
            room.BrocastToTeamTwo(Protocol.TYPE_FIGHT, Protocol.AREA_S_ENTERGAME, 0, null);
        }


        private void SodierBornEvent(object sender, EventArgs e)
        {
            if (Runing && room.sodiersPool.Count > 0)
            {
                SodierBorn();
            }
        }

        private void SodierBorn()
        {
            SodierActorModel sam1 = room.sodiersPool.Dequeue();
            SodierActorModel sam2 = room.sodiersPool.Dequeue();
            SodierData sodierTeamOne = sam1.CreateSodierData(1);
            SodierData sodierTeamTwo = sam2.CreateSodierData(2);
            bool res = room.worldAcotr.TryAdd(sam1.id, sam1);
            if (res)
            {
                Console.WriteLine("生成小兵:" + sam1.id);
            }
            res = room.worldAcotr.TryAdd(sam2.id, sam2);
            if (res)
            {
                Console.WriteLine("生成小兵:" + sam2.id);
            }
            room.Brocast(Protocol.TYPE_FIGHT, Protocol.AREA_S_SODIER_BORN, 0, sodierTeamOne, null);
            room.Brocast(Protocol.TYPE_FIGHT, Protocol.AREA_S_SODIER_BORN, 0, sodierTeamTwo, null);

        }
        public override void ClientClose(UserToken token)
        {
            Runing = false;
        }

        public override void MessageReceive(UserToken token, MSGModel message)
        {
            switch (message.area)
            {
                case Protocol.AREA_C_BATTLE_MOVE:
                    PlayerMove(token, message);
                    break;
                case Protocol.AREA_C_BATTLE_POS:
                    PlayerPos(token, message);
                    break;
                case Protocol.AREA_C_BATTLE_HERO_ATTACK:
                    PlayerAttack(token, message);
                    break;
                case Protocol.AREA_C_GOLD_TIMEADD:
                    ExecutePool.Instance.execute(delegate ()
                    {
                        AddGold(token, message);
                    });
                    break;
                case Protocol.AREA_C_REQ_BUY:
                    ExecutePool.Instance.execute(delegate ()
                    {
                        PlayerBuy(token, message);
                    });
                    break;
                case Protocol.AREA_C_REQ_SELL:
                    ExecutePool.Instance.execute(delegate ()
                    {
                        PlayerSell(token, message);
                    });
                    break;
                case Protocol.AREA_C_NPCATTACK:
                    NPCAttack(token, message);
                    break;
                case Protocol.AREA_C_DEMAGE:
                    OnDemageInfo(token, message);
                    break;
            }
        }

        private void OnDemageInfo(UserToken token, MSGModel message)
        {
            DemageInfo di = message.GetMessage<DemageInfo>();

            lock (room.worldAcotr)
            {
                AbsActorModel aam = null;
                if (room.worldAcotr.ContainsKey(di.DemagedID))
                    aam = room.worldAcotr[di.DemagedID];
                else//如果世界中没有这个物体则返回
                    return;
                int HP = -1;
                HP = aam.GetHP();

                if (HP == di.PreHP && HP >= 0)
                {
                    //可以造成伤害
                    int defence = aam.GetDefence();
                    int damage = Math.Max(di.Attack - defence,2);
                    HP -= damage;
                    aam.SetHP(HP);

                    DemageResult dr = new DemageResult();
                    dr.RemainHP = HP;
                    dr.TargetID = di.DemagedID;
                    room.Brocast(Protocol.TYPE_FIGHT, Protocol.AREA_S_DEMAGE, 0, dr, null);

                    if (HP <= 0)
                    {//单位被击杀
                        int killGold = aam.GetKillGold();//奖励金币

                        //击杀者为英雄
                        if (di.Attacker == 0 || di.Attacker == 10)
                        {
                            HeroActorModel ham = room.worldAcotr[di.Attacker] as HeroActorModel;
                            ham.Gold += killGold;
                            room.Write(token, Protocol.TYPE_FIGHT, Protocol.AREA_S_GOLD_TIMEADD_SUCCESS, ham.Gold, null);
                        }

                        if (di.DemagedID == 0 || di.DemagedID == 10)
                        {//英雄死亡
                         //通知客户端英雄死亡进入死亡状态
                            HeroActorModel ham_demage = room.worldAcotr[di.DemagedID] as HeroActorModel;
                            room.Brocast(Protocol.TYPE_FIGHT, Protocol.AREA_S_HERO_DEAD, di.DemagedID, ham_demage.ResetTime, null);
                            ScheduleUtil.Instance.Schedule(delegate ()
                            {
                                HeroReset(di.DemagedID, ham_demage);
                            }, ham_demage.ResetTime);
                        }
                        else if (di.DemagedID >= 30 && di.DemagedID <= 1029 && room.worldAcotr.ContainsKey(di.DemagedID))
                        {//被击杀的是小兵
                         //通知客户端有小兵死亡.
                         //SodierActorModel sam_demage = room.worldAcotr[di.DemagedID] as SodierActorModel;
                            try
                            {
                                AbsActorModel tmp = null;
                                room.worldAcotr.TryRemove(di.DemagedID, out tmp);
                                if (tmp != null)
                                    ((SodierActorModel)tmp).Reset();
                                room.sodiersPool.Enqueue(tmp as SodierActorModel);
                                Console.WriteLine("小兵 " + tmp.id + " 死亡");
                                room.Brocast(Protocol.TYPE_FIGHT, Protocol.AREA_S_SODIER_DEADTH, di.DemagedID, di.Attacker, null);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                }
            }
        }

        private void HeroReset(int id, HeroActorModel ham)
        {
            ham.Reset();
            room.Brocast(Protocol.TYPE_FIGHT, Protocol.AREA_S_HERO_RESET, id, null, null);
        }

        private void NPCAttack(UserToken token, MSGModel message)
        {
            AttackInfo attackInfo = message.GetMessage<AttackInfo>();
            switch (attackInfo.type)
            {
                case 1:
                    TowerAttack(token, attackInfo);
                    break;
                case 2:
                    SodierAttack(token, attackInfo);
                    break;
            }

        }

        private void SodierAttack(UserToken token, AttackInfo attackInfo)
        {
            int a = attackInfo.a;
            int b = attackInfo.b;
            if (!room.worldAcotr.ContainsKey(a))
                throw new Exception("物体不存在");
            lock (room.worldAcotr)
            {
                SodierActorModel temp = room.worldAcotr[a] as SodierActorModel;
                if (!temp.IsAttacking)
                {
                    temp.IsAttacking = true;
                    room.Brocast(Protocol.TYPE_FIGHT, Protocol.AREA_S_NPCATTACK, 0, attackInfo, null);
                    Console.WriteLine("小兵 " + a + " 攻击 " + " 小兵 " + b);
                    ScheduleUtil.Instance.Schedule(delegate ()
                    {
                        temp.IsAttacking = false;
                    }, 1);
                }
            }
        }

        private void TowerAttack(UserToken token, AttackInfo attackInfo)
        {
            int a = attackInfo.a;
            int b = attackInfo.b;
            TowerActorModel temp = room.worldAcotr[a] as TowerActorModel;
            lock (room.worldAcotr)
            {
                if (!room.worldAcotr.ContainsKey(a))
                    throw new Exception("物体不存在");
                if (!temp.IsAttacking)
                {
                    temp.IsAttacking = true;
                    room.Brocast(Protocol.TYPE_FIGHT, Protocol.AREA_S_NPCATTACK, 0, attackInfo, null);
                    ScheduleUtil.Instance.Schedule(delegate ()
                    {
                        temp.IsAttacking = false;
                    }, 1);
                }
            }
        }

        private void PlayerSell(UserToken token, MSGModel message)
        {
            int mask = 0xff;
            int msg = message.GetMessage<int>();
            int worldID = msg & mask;
            int equipment = msg >> 8;
            HeroActorModel ham = room.worldAcotr[worldID] as HeroActorModel;
            if (ham.package.Contains(equipment))
            {
                Equipment equimentAttribute = EquipmentCache.Instance.GetEquipmentByID(equipment);
                ham.package.Remove(equipment);
                ham.Gold += equimentAttribute.Price;

                ham.HP -= equimentAttribute.HP;
                ham.Attack -= equimentAttribute.Attack;
                ham.Defence -= equimentAttribute.Defence;
                ham.Speed -= equimentAttribute.Speed;

                uint sellNotifyMsg = (uint)((equimentAttribute.HP << 16) | (equimentAttribute.Attack << 12) | (equimentAttribute.Defence << 8) | (equimentAttribute.Speed << 4) | (worldID));
                room.Brocast(Protocol.TYPE_FIGHT, Protocol.AREA_S_PLAYER_SELL, (int)sellNotifyMsg, null, null);
                room.Write(token, Protocol.TYPE_FIGHT, Protocol.AREA_S_RES_SELL_SUCCESS, ham.Gold, null);
            }
        }

        private void PlayerBuy(UserToken token, MSGModel message)
        {
            ulong msg = message.GetMessage<ulong>();
            ulong mask = 0xff;
            ulong worldID = msg & mask;
            ulong equipmentnum = (msg >> 8) & mask;
            ulong gold = (msg >> 16);

            HeroActorModel ham = room.worldAcotr[(int)worldID] as HeroActorModel;
            Equipment eq = EquipmentCache.Instance.GetEquipmentByID((int)equipmentnum);
            if (eq != null && gold <= (ulong)ham.Gold && ham.Gold >= eq.Price && ham.package.Count < 6)
            {
                ham.Gold = ham.Gold - eq.Price;
                ham.package.Add((int)equipmentnum);

                uint id = (uint)worldID;
                uint speed = (uint)eq.Speed;
                uint defence = (uint)eq.Defence;
                uint attack = (uint)eq.Attack;
                uint hp = (uint)eq.HP;

                ham.Speed += eq.Speed;
                ham.Defence += eq.Defence;
                ham.Attack += eq.Attack;
                ham.HP += eq.HP;
                uint cmd = (hp << 16) | (attack << 12) | (defence << 8) | (speed << 4) | id;

                room.Write(token, Protocol.TYPE_FIGHT, Protocol.AREA_S_RES_BUY_SUCCESS, ham.Gold, null);
                room.Brocast(Protocol.TYPE_FIGHT, Protocol.AREA_S_PLAYER_BUY, (int)cmd, null, null);
            }
        }

        private void AddGold(UserToken token, MSGModel message)
        {
            int msg = message.GetMessage<int>();
            int targetGold = msg >> 10;
            int worldID = msg & 1023;
            HeroActorModel heroModel = room.worldAcotr[worldID] as HeroActorModel;
            if (heroModel.Gold == targetGold - 1)
            {//验证通过
                (room.worldAcotr[worldID] as HeroActorModel).Gold = targetGold;
                room.Write(token, Protocol.TYPE_FIGHT, Protocol.AREA_S_GOLD_TIMEADD_SUCCESS, targetGold, null);
            }
            else
            {
                //客户端可能开挂了
            }
        }

        private void PlayerAttack(UserToken token, MSGModel message)
        {
            room.Brocast(Protocol.TYPE_FIGHT, Protocol.AREA_S_BATTLE_HERO_ATTACK, 0, message.GetMessage<AttackInfo>(), token);
        }

        public void PlayerMove(UserToken token, MSGModel message)
        {
            ThumAxis axis = message.GetMessage<ThumAxis>();
            int team = axis.team;
            if (team == 1)
            {
                room.BrocastToTeamTwo(Protocol.TYPE_FIGHT, Protocol.AREA_S_BATTLE_MOVE, 0, axis);
            }
            else if (team == 2)
            {
                room.BrocastToTeamOne(Protocol.TYPE_FIGHT, Protocol.AREA_S_BATTLE_MOVE, 0, axis);
            }
        }

        public void PlayerPos(UserToken token, MSGModel message)
        {
            ActorPosition ap = message.GetMessage<ActorPosition>();
            if (ap.team == 1)
            {
                room.BrocastToTeamTwo(Protocol.TYPE_FIGHT, Protocol.AREA_S_BATTLE_POS, 0, ap);
            }
            else if (ap.team == 2)
            {
                room.BrocastToTeamOne(Protocol.TYPE_FIGHT, Protocol.AREA_S_BATTLE_POS, 0, ap);
            }
        }

        public override void nextState()
        {
            //throw new NotImplementedException();
        }

        public override void update()
        {
            //throw new NotImplementedException();
        }
    }

}
