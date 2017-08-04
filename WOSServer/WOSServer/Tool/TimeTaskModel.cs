namespace WOSServer.Tool
{
    internal class TimeTaskModel
    {
        private TimeEvent execute;
        public long time;
        public int id;
        public TimeTaskModel(int id,TimeEvent execute,long time)
        {
            this.id = id;
            this.execute = execute;
            this.time = time;
        }

        public void Run()
        {
            execute();
        }

    }
}