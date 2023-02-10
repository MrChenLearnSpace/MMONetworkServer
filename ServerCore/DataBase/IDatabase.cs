namespace ServerCore {
    public interface IDatabase {
        void Connect(string Database, string DataSource, string port, string user, string pw);
        bool Register(string id, string pw);
        bool CheckPassWord(string id, string pw);
        bool InsertPlayerData(string id, string playerStream, string ip);
        string GetPlayerData(string id);
        bool SavePlayerData(string id, string playerStream, string ip);
    }
}
