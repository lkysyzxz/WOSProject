using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WOSServer
{
    public delegate void ExecuteDelegate();
    public class ExecutePool
    {
        private Mutex mutex = new Mutex();
        private static ExecutePool pool;
        private static object SyncObj = new object();
        public static ExecutePool Instance
        {
            get
            {
                if (pool == null)
                {
                    lock (SyncObj)
                    {
                        if (pool == null)
                        {
                            pool = new ExecutePool();
                        }
                    }
                }
                return pool;
            }
        }

        public void execute(ExecuteDelegate execute)
        {
            lock (this)
            {
                mutex.WaitOne();
                execute();
                mutex.ReleaseMutex();
            }

        }
    }
}
