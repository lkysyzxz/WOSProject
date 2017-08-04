using AbsNet;
using MessageClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOSServer.Cache
{
    public interface IUserCache
    {
        UserData getUserDataByUsername(string username);

        void addUser(string username, UserData userData);

        void addUser(DataTable table);
    }
}
