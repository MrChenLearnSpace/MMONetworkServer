using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//using MMONetworkServer.Logic;

namespace MMONetworkServer.Core {
    //数据库封装，操作mysql数据库，比如:读取角色数据，更新角色数据
    public class DataMgr {
        MySqlConnection sqlConn;
        public static DataMgr instance;
        private static readonly object syncRoot = new object();
        public DataMgr() {
            Connect();
        }
        public static DataMgr GetInstance() {
            if (instance == null) {//先判断实例是否存在，不存在再加锁处理
                lock (syncRoot) {
                    if (instance == null) {
                        instance = new DataMgr();
                    }
                }
            }
            return instance;
        }
        public void Connect() {
            string connStr = "Database=game;Data Source=127.0.0.1;";
            connStr += "User Id=root;Password=tankwar;port=3306";
            sqlConn = new MySqlConnection(connStr);
            try {
                sqlConn.Open();
                Console.WriteLine("game数据库打开成功");
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
        public bool IsSafeStr(string str) {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }
        private bool CanRegister(string id) {
            if (!IsSafeStr(id)) {
                //可以设置错误码
                return false;
            }
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
            if ((!IsSafeStr(id)) || !IsSafeStr(pw)) {
                Console.WriteLine("DataMgr Resister失败,使用非法字符");
                return false;
            }
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

        public bool CreatePlayer(string id) {
                //防sql注入
                if (!IsSafeStr(id))
                    return false;
                //序列化
                IFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                PlayerData playerData = new PlayerData();
                try {
                    formatter.Serialize(stream, playerData);
                }
                catch (Exception e) {
                    Console.WriteLine("[DataMgr]CreatePlayer 序列化" + e.Message);
                    return false;
                }
                byte[] byteArr = stream.ToArray();
                //写入数据库
                string cmdStr = string.Format("insert into player set id ='{0}' ,data =@data;", id);
                MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
                cmd.Parameters.Add("@data", MySqlDbType.Blob);
                cmd.Parameters[0].Value = byteArr;
                try {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("[DataMgr]CreatePlayer 写入 成功" );
                    return true;
                }
                catch (Exception e) {
                    Console.WriteLine("[DataMgr]CreatePlayer 写入 " + e.Message);
                    return false;
                }
        }

        public bool CheckPassWord(string id, string pw) {

            //带优化和加强；

            //防sql注入
            if (!IsSafeStr(id) || !IsSafeStr(pw))
                return false;
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

        public PlayerData GetPlayerData(string id) {
            PlayerData playerData = null;
            //防sql注入
            if (!IsSafeStr(id))
                return playerData;
            //查询
            string cmdStr = string.Format("select * from player where id ='{0}';", id);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            byte[] buffer = new byte[1];
            try {
                MySqlDataReader dataReader = cmd.ExecuteReader();
                if (!dataReader.HasRows) {
                    dataReader.Close();
                    return playerData;
                }
                dataReader.Read();
                long len = dataReader.GetBytes(1, 0, null, 0, 0);//1是data
                buffer = new byte[len];
                dataReader.GetBytes(1, 0, buffer, 0, (int)len);
                dataReader.Close();
            }
            catch (Exception e) {
                Console.WriteLine("[DataMgr]GetPlayerData 查询 " + e.Message);
                return playerData;
            }
            //反序列化
            MemoryStream stream = new MemoryStream(buffer);
            try {
                BinaryFormatter formatter = new BinaryFormatter();
                playerData = (PlayerData)formatter.Deserialize(stream);
                return playerData;
            }
            catch (SerializationException e) {
                Console.WriteLine("[DataMgr]GetPlayerData 反序列化" + e.Message);
                return playerData;
            }
        }

        //保存角色
        public bool SavePlayer(Player player) {
              string id = player.id;
              PlayerData playerData = player.data;
              //序列化
              IFormatter formatter = new BinaryFormatter();
              MemoryStream stream = new MemoryStream();
              try {
                  formatter.Serialize(stream, playerData);
              }
              catch (Exception e) {
                  Console.WriteLine("[DataMgr]SavePlayer 序列化" + e.Message);
              return false;
              }
              byte[] byteArr = stream.ToArray();
              //写入数据库
              string formatStr = "update player set data =@data where id = '{0}';";
              string cmdStr = string.Format(formatStr, player.id);
              MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
              cmd.Parameters.Add("@data", MySqlDbType.Blob);
              cmd.Parameters[0].Value = byteArr;
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
