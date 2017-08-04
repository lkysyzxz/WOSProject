using AbsNet;
using AbsNet.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOSServer
{
    public abstract class AbsOnceHnadler
    {
        private int type;
        private int area;
        
        public void setType(int type)
        {
            this.type = type;
        }

        public void setArea(int area)
        {
            this.area = area;
        }

        public virtual int getType()
        {
            return type;
        }

        public virtual int getArea()
        {
            return area;
        }

        protected void write(UserToken token, int command)
        {
            write(token, getType(), getArea(), command, null);
        }

        protected void write(UserToken token,int command,object message)
        {
            write(token, getType(),getArea(), command, message);
        }

        protected void write(UserToken token,int area,int command,object message)
        {
            write(token, getType(), area, command, message);
        }

        protected void write(UserToken token,int type,int area,int command,object message)
        {
            byte[] buffer = MessageEncoding.Encode(MSGModel.CreateMessage(type, area, command, message));
            buffer = LengthEncoding.encode(buffer);
            token.Write(buffer);
        }
    }
}
