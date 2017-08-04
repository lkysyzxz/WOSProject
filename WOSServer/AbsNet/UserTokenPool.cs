using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbsNet
{
    public class UserTokenPool
    {
        private Stack<UserToken> pool;

        public UserTokenPool(int size)
        {
            pool = new Stack<UserToken>();
        }

        public UserToken Pop()
        {
            if (this.Size() > 0)
            {
                UserToken res = pool.Pop();
                return res;
            }
            else
            {
                return null;
            }
        }

        public void Push(UserToken item)
        {
            if (item != null)
                pool.Push(item);
        }

        public int Size()
        {
            return pool.Count;
        }
    }
}
