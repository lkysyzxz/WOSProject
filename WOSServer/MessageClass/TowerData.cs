using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageClass
{
    [Serializable]
    public class TowerData
    {
        public int ID;
        public int Team;
        public int HP;
        public int Attack;
        public int Defence;

        public TowerData()
        {

        }
    }
}
