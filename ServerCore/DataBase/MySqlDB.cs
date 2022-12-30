
using MySql.Data.MySqlClient;
using System;
#pragma warning disable CS8618 
namespace ServerCore {

    public class MySqlDB : IDatabase {
        MySqlConnection sqlConn;
        public void Connect(string Database, string DataSource, string port, string user, string pw) {
            string connStr = "Database={0};Data Source={1};";
            connStr += "User Id={2};Password={3};port={4}";
            connStr = string.Format(connStr, Database, DataSource, user, pw, port);
            sqlConn = new MySqlConnection(connStr);
            try {
                sqlConn.Open();
                Console.WriteLine(Database + "数据库打开成功");
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public bool RunCmd(string cmd) {

            MySqlCommand mysqlcmd = new MySqlCommand(cmd, sqlConn);
            try {
                mysqlcmd.ExecuteNonQuery();
                Console.WriteLine("[MySqlDB]RunCmd  成功");
                return true;
            }
            catch (Exception e) {
                Console.WriteLine("[MySqlDB]RunCmd " + e.Message);
                return false;
            }
        }
        bool CanRegister(string id) {
            string cmdStr = string.Format("select * from user where id ='{0}';", id);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            try {
                MySqlDataReader dataReader = cmd.ExecuteReader();
                bool hasRows = dataReader.HasRows;
                dataReader.Close();
                return !hasRows;
            }
            catch (Exception e) {
                Console.WriteLine("DataMgr CanRegister失败" + e.Message);
                return false;
            }
        }
        public bool Register(string id, string pw) {

            if (!CanRegister(id)) {
                Console.WriteLine("DataMgr Resister失败,!CanRegister");
                return false;
            }
            string cmdStr = string.Format("insert into user set id ='{0}',pw = '{1}';", id, pw);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            try {
                cmd.ExecuteNonQuery();

                Console.WriteLine("DataMgr Resister 成功");
                return true;
            }
            catch (Exception e) {
                Console.WriteLine("DataMgr Resister失败" + e.Message);
                return false;
            }
        }
        public bool CheckPassWord(string id, string pw) {

            //带优化和加强；

            //防sql注入
           
            //查询
            string cmdStr = string.Format("select * from user where id='{0}' and pw='{1}';", id, pw);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            try {
                MySqlDataReader dataReader = cmd.ExecuteReader();
                bool hasRows = dataReader.HasRows;
                dataReader.Close();
                Console.WriteLine("[DataMgr]CheckPassWord has " + hasRows.ToString());
                return hasRows;
            }
            catch (Exception e) {
                Console.WriteLine("[DataMgr]CheckPassWord " + e.Message);
                return false;
            }
        }

        public bool InsertPlayerData(string id, string dataStream, string ip) {
            if (!DataMgr.instance.IsSafeStr(id))
                return false;
            string cmdStr = string.Format("insert into player set id ='{0}' ,data ='{2}', ip ='{1}' ;", id, ip, dataStream);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            try {
                cmd.ExecuteNonQuery();
                Console.WriteLine("[DataMgr]CreatePlayer 写入 成功");
                return true;
            }
            catch (Exception e) {
                Console.WriteLine("[DataMgr]CreatePlayer 写入 " + e.Message);
                return false;
            }
        }
        public string GetPlayerData(string id) {

            if (!DataMgr.instance.IsSafeStr(id))
                return "";
            //查询
            string cmdStr = string.Format("select * from player where id ='{0}';", id);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);

            try {
                MySqlDataReader dataReader = cmd.ExecuteReader();
                if (!dataReader.HasRows) {
                    dataReader.Close();
                    return "";
                }
                dataReader.Read();
                string buffer = dataReader.GetString("data");


                dataReader.Close();
                return buffer;
            }
            catch (Exception e) {
                Console.WriteLine("[DataMgr]GetPlayerData 查询 " + e.Message);
                return "";
            }
        }
        public bool SavePlayerData(string id, string playerStream, string ip) {
            if (!DataMgr.instance.IsSafeStr(id))
                return false;
            //byte[] byteArr = stream.ToArray();
            //写入数据库
            string formatStr = "update player set data ={2},ip ='{0}' where id = '{1}';";
            string cmdStr = string.Format(formatStr, ip, id, playerStream);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            try {
                cmd.ExecuteNonQuery();
                Console.WriteLine("[DataMgr]SavePlayer 写入 成功");
                return true;
            }
            catch (Exception e) {
                Console.WriteLine("[DataMgr]SavePlayer 写入" + e.Message);
                return false;
            }
        }


    }
}
