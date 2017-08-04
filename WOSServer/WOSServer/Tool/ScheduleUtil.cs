using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace WOSServer.Tool
{
    public delegate void TimeEvent();
    public class ScheduleUtil
    {
        private static ScheduleUtil _instance;
        public static ScheduleUtil Instance
        {
            get
            {
                if(_instance== null)
                {
                    _instance = new ScheduleUtil();
                }
                return _instance;
            }
        }

        private ConcurrentInteger index = new ConcurrentInteger(0);
        private Dictionary<int, TimeTaskModel> mission = new Dictionary<int, TimeTaskModel>();

        private List<int> removeList = new List<int>();

        private Timer timer;
        private ScheduleUtil()
        {
            timer = new Timer(14);
            timer.Elapsed += CallBack;
            timer.Start();
        }

        private void CallBack(object sender,ElapsedEventArgs e)
        {
            lock(mission)
            {
                lock(removeList)
                {
                    foreach(int item in removeList)
                    {
                        mission.Remove(item);
                    }
                    removeList.Clear();
                    foreach(TimeTaskModel item in mission.Values)
                    {
                        if (item.time <= DateTime.Now.Ticks)
                        {
                            item.Run();
                            removeList.Add(item.id);
                        }
                    }
                }
            }
        }

        public int Schedule(TimeEvent task,long delay)
        {
            return Schedulemms(task, delay * 1000 * 10000);
        }

        private int Schedulemms(TimeEvent task, long delay)
        {
            lock(mission)
            {
                int id = index.GetAndAdd();
                mission.Add(id, new TimeTaskModel(id,task, DateTime.Now.Ticks + delay));
                return id;
            }
        }
    }
}
