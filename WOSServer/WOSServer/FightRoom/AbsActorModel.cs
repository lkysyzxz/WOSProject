using MessageClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOSServer.Cache;

namespace WOSServer.FightRoom
{
    //场景中每一个物体
    public abstract class AbsActorModel
    {
        //物体的ID
        public int id;
        public AbsActorModel(int id)
        {
            this.id = id;
        }

        public abstract int GetHP();

        public abstract void SetHP(int value);

        public abstract int GetAttack();

        public abstract int GetDefence();

        public abstract int GetKillGold();
    }

    public class HeroActorModel : AbsActorModel
    {
        public int HP;
        public int Attack;
        public int Defence;
        public int Speed;
        public int Gold;
        public int Exp;
        public int Level;

        public int MaxHP;
        public int MaxExp;

        public int KillGold = 300;
        public long ResetTime = 10;

        public List<int> package = new List<int>();

        public HeroActorModel(int id, HeroInitInfo initInfo, int initGold) : base(id)
        {
            HP = initInfo.InitHP;
            Attack = initInfo.InitAttack;
            Defence = initInfo.InitDefence;
            Speed = initInfo.InitSpeed;
            this.Gold = initGold;
            this.Exp = 0;
            this.Level = 1;
            MaxHP = initInfo.MaxHP;
            MaxExp = initInfo.MaxExp;
        }

        public override int GetHP()
        {
            return HP;
        }

        public override int GetAttack()
        {
            return Attack;
        }

        public override int GetDefence()
        {
            return Defence;
        }

        public override void SetHP(int value)
        {
            this.HP = value;
        }

        public override int GetKillGold()
        {
            return KillGold;
        }

        internal void Reset()
        {
            this.HP = this.MaxHP;
        }
    }

    public static class HeroDataFactory
    {
        public static HeroData CreateHeroData(HeroActorModel ha)
        {
            HeroData hd = new HeroData();
            hd.WorldID = ha.id;
            hd.HP = ha.HP;
            hd.Attack = ha.Attack;
            hd.Defence = ha.Defence;
            hd.Speed = ha.Speed;
            hd.Level = ha.Level;
            hd.Exp = ha.Exp;
            hd.Gold = ha.Gold;
            hd.MaxExp = ha.MaxExp;
            hd.MaxHP = ha.MaxHP;

            return hd;
        }
    }


    public class TowerActorModel : AbsActorModel
    {
        public int HP = 1000;
        public int Attack = 50;
        public int Defence = 0;
        public bool IsAttacking = false;
        public int AttackInterval = 1;
        public int KillGold = 125;
        public TowerActorModel(int id) : base(id)
        {

        }

        public void SetAttackState(bool value)
        {
            this.IsAttacking = value;
        }

        public override int GetHP()
        {
            return HP;
        }

        public override int GetAttack()
        {
            return Attack;
        }

        public override int GetDefence()
        {
            return Defence;
        }

        public override void SetHP(int value)
        {
            this.HP = value;
        }

        public override int GetKillGold()
        {
            return KillGold;
        }
    }


    public class CrystalActorModel : AbsActorModel
    {
        public int HP = 1500;
        public int Attack = 50;
        public int Defence = 0;

        public CrystalActorModel(int id) : base(id)
        {

        }

        public override int GetHP()
        {
            return HP;
        }

        public override int GetAttack()
        {
            return Attack;
        }

        public override int GetDefence()
        {
            return Defence;
        }

        public override void SetHP(int value)
        {
            this.HP = value;
        }

        public override int GetKillGold()
        {
            return 0;
        }
    }

    public class SodierActorModel : AbsActorModel
    {
        public int HP = 50;
        public int Attack = 4;
        public int Defence = 0;
        public long AttackInterval = 1;
        public bool IsAttacking = false;
        public int KillGold = 25;

        public SodierActorModel(int id) : base(id)
        {

        }

        public override int GetAttack()
        {
            return Attack;
        }

        public override int GetDefence()
        {
            return Defence;
        }

        public override int GetHP()
        {
            return HP;
        }

        public override int GetKillGold()
        {
            return KillGold;
        }

        public override void SetHP(int value)
        {
            this.HP = value;
        }

        public SodierData CreateSodierData(int team)
        {
            SodierData sd = new SodierData();
            sd.ID = this.id;
            sd.Team = team;
            sd.Attack = this.Attack;
            sd.Defence = this.Defence;
            sd.HP = this.HP;
            return sd;
        }

        public void Reset()
        {
            HP = 50;
            Attack = 4;
            Defence = 0;
            AttackInterval = 1;
            IsAttacking = false;
            KillGold = 25;
        }
    }
}
