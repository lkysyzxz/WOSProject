using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOSServer.Cache
{
    public class HeroInitTable
    {
        private static HeroInitTable _instance;
        public static HeroInitTable Instance
        {
            get
            {
                if(_instance==null)
                {
                    _instance = new HeroInitTable();
                }
                return _instance;
            }
        }

        private HeroInitTable()
        {
            heroInitInfo.Add(0, new HeroInitInfo(100, 10, 5, 4,100,100,0.35f, 0.2f, 0.25f));
            heroInitInfo.Add(1, new HeroInitInfo(100, 10, 5, 4,100,100,0.35f, 0.2f, 0.25f));
        }

        public Dictionary<int, HeroInitInfo> heroInitInfo = new Dictionary<int, HeroInitInfo>();
    }

    //英雄初始化数据
    public class HeroInitInfo
    {
        public int InitHP;
        public int InitAttack;
        public int InitDefence;
        public int InitSpeed;
        public int MaxHP;
        public int MaxExp;

        public float HPAdd;
        public float AttackAdd;
        public float DefenceAdd;
        public HeroInitInfo(int hp,
            int attack,
            int defence,
            int speed,
            int MaxHP,
            int MaxExp,
            float hpadd,
            float attackadd,
            float defenceadd)
        {
            this.InitHP = hp;
            this.InitAttack = attack;
            this.InitDefence = defence;
            this.InitSpeed = speed;
            this.MaxHP = MaxHP;
            this.MaxExp = MaxExp;
            this.HPAdd = hpadd;
            this.AttackAdd = attackadd;
            this.DefenceAdd = defenceadd;
        }
    }
}
