using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOSServer
{
    public class Equipment
    {
        public int ID;
        public int Price;
        public int HP;
        public int Attack;
        public int Defence;
        public int Speed;
        public Equipment(int id,int price,int hp,int attack,int defence,int speed)
        {
            this.ID = id;
            this.Price = price;
            this.HP = hp;
            this.Attack = attack;
            this.Defence = defence;
            this.Speed = speed;
        }
    }
}
