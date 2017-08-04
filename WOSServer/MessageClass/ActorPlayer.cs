using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageClass
{
    [Serializable]
    public class ActorPosition
    {
        public int team;
        public float x;
        public float y;
        public float z;
        public ActorPosition()
        {

        }
    }
}
