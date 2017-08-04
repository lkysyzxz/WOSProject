using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageClass
{
    [Serializable]
    public class AttackInfo
    {
        public int a;

        public int b;

        /// <summary>
        /// 攻击类型
        /// 0   英雄
        /// 1   防御塔
        /// 2   小兵
        /// </summary>
        public byte type;
        public AttackInfo()
        {

        }
    }
}
