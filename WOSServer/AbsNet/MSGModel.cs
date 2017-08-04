namespace AbsNet
{
    public class MSGModel
    {
        public int type { get; set; }
        public int area { get; set; }
        public int command { get; set; }
        public object message { get; set; }

        public MSGModel()
        {

        }

        public MSGModel(int t,int a,int c,object m)
        {
            this.type = t;
            this.area = a;
            this.command = c;
            this.message = m;
        }

        public T GetMessage<T>()
        {
            return (T)message;
        }

        public override string ToString()
        {
            return type + "-----" + area + "------" + command + "----" + message;
        }

        public static MSGModel CreateMessage(int type,int area,int command,object obj)
        {
            return new MSGModel(type, area, command, obj);
        }
    }
}
