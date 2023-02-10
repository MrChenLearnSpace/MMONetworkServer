using System;
using System.Text.RegularExpressions;
//using ServerLoginHotfix;

namespace ServerCore {
    //数据库封装，操作mysql数据库，比如:读取角色数据，更新角色数据
    public class DataMgr {

        public static DataMgr instance;

        public IDatabase database;
        //  public string Database = "game";
        private static readonly object syncRoot = new object();
        public DataMgr() {
            //database = new Mongo();
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
        public void Connect(string Database, string DataSource, string port, string user, string pw) {
            database.Connect(Database, DataSource, port, user, pw); 
        }
        public bool IsSafeStr(string str) {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        public bool Register(string id, string pw) {
            if ((!IsSafeStr(id)) || !IsSafeStr(pw)) {
                Console.WriteLine("DataMgr Resister失败,使用非法字符");
                return false;
            }
            return  database.Register(id,pw);
        }
        public bool CheckPassWord(string id, string pw) {
            if ((!IsSafeStr(id)) || !IsSafeStr(pw)) {
                Console.WriteLine("DataMgr Resister失败,使用非法字符");
                return false;
            }
            return database.CheckPassWord(id,pw);

        }
        public bool  InsertPlayer(string id , string buff, string ip) {
            return database.InsertPlayerData(id, buff, ip);
        }
        public bool SavePlayerStream(string id, string playerStream, string ip) {
            return  database.SavePlayerData(id, playerStream, ip);
        }
        public string  GetPlayerData(string id) {
            return database.GetPlayerData(id);
        }
    }
}
