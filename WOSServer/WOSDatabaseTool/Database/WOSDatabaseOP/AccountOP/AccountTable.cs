using MessageClass;
using System.Data;
using System.Data.SqlClient;
using WOSServer.Database.WOSDatabaseOP;

namespace WOSDatabaseTool.Database.WOSDatabaseOP.AccountOP
{
    public static class AccountTable
    {
        public static bool AddNewAccount(RegisterUserInfo userInfo)
        {
            SqlConnection con = WOSDB.GetConection();
            SqlCommand cmd = new SqlCommand("ADD_ACCOUNT", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@username", userInfo.Username);
            cmd.Parameters.AddWithValue("@password", userInfo.Password);
            cmd.Parameters.AddWithValue("@nickname", userInfo.Nickname);
            int res = -1;
            SqlParameter parOutput = cmd.Parameters.AddWithValue("@errorcode", res);
            parOutput.Direction = System.Data.ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            res = int.Parse(parOutput.Value.ToString());
            return res == 0;
        }

        public static DataTable GetAccounts()
        {
            SqlConnection con = WOSDB.GetConection();
            string cmdStr = "select username,password from account";
            SqlCommand cmd = new SqlCommand(cmdStr,con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            con.Close();
            return dataSet.Tables[0];
        }

        public static DataTable GetUserData()
        {
            SqlConnection con = WOSDB.GetConection();
            string cmdStr = "select username,nickname,wincount,totalcount,level,exp from account";
            SqlCommand cmd = new SqlCommand(cmdStr, con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            con.Close();
            return dataSet.Tables[0];
        }
    }
}
