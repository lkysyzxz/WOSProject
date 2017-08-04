using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageClass
{
    [Serializable]
    public class HeroData
    {
        public int WorldID;
        public int HP;
        public int Attack;
        public int Defence;
        public int Speed;
        public int Gold;
        public int Exp;
        public int Level;

        public int MaxHP;
        public int MaxExp;
        public HeroData()
        {

        }
    }
}
