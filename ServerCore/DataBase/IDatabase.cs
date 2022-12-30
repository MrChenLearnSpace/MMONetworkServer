namespace ServerCore {
    public interface IDatabase {
        void Connect(string Database, string DataSource, string port, string user, string pw);
        bool Register(string id, string pw);
        bool CheckPassWord(string id, string pw);

    }
}
