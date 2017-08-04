using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageClass
{
    [Serializable]
    public class RoomInfo
    {
        public int RoomID;
        public int PlayerTeam;
        public int PlayerNumber;
        public RoomInfo() { }
    }
}
