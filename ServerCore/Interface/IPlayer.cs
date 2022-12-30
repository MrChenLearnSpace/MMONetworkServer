using ServerCore.net;

namespace ServerCore {
    public interface IPlayer {
        string Id { get; set; }
        Conn Connect { get; set; }
        //public string Name { get; set; }
        void Send(ProtocolBase protocol);
        //bool KickOff(string id, ProtocolBase proto);

        bool Logout();
        // Conn GetConn();

        // string GetId();


        bool SavePlayer();

        bool GetPlayerData();
        // public byte[] Serialize(IPlayer player);

    }
}
