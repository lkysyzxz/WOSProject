using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WOSServer
{
    class ConcurrentInteger
    {
        private Mutex mutex = new Mutex();
        private int value;
        public ConcurrentInteger(int iniValue = 0)
        {
            value = iniValue;
        }

        /// <summary>
        /// 自增并返值
        /// </summary>
        public int GetAndAdd()
        {
            lock (this)
            {
                mutex.WaitOne();
                int res = value++;
                mutex.ReleaseMutex();
                return res;
            }
           
        }

        public int GetAndReduce()
        {
            lock (this)
            {
                mutex.WaitOne();
                int res = value--;
                mutex.ReleaseMutex();
                return res;
            }
        }

        public void Reset()
        {
            lock (this)
            {
                mutex.WaitOne();
                value = 0;
                mutex.ReleaseMutex();
            }
        }

        public int GetValue()
        {
            return value;
        }

        public void SyncAdd()
        {
            lock(this)
            {
                mutex.WaitOne();
                value++;
                mutex.ReleaseMutex();
            }
        }
    }
}
