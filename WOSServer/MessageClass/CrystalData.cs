using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageClass
{
    [Serializable]
    public class CrystalData
    {
        public int ID;
        public int Team;
        public int HP = 1500;
        public int Attack = 50;
        public int Defence = 0;
        public CrystalData()
        {

        }
    }
}
