using System;
using System.Data.SqlClient;

namespace WOSServer.Database.WOSDatabaseOP
{
    public static class WOSDB
    {
        public static SqlConnection GetConection()
        {
            try{
                string constr = "Data Source=*****;Initial Catalog=WOSDB;Persist Security Info=True;User ID=sa;Password=*****";
                SqlConnection con= new SqlConnection(constr);
                con.Open();
                return con;
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
