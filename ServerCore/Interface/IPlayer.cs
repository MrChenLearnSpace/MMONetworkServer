using ServerCore.net;
using System;

namespace ServerCore {
    public abstract class IPlayer {
        public string id ;
        public Conn client;
        
        //public string Name { get; set; }
        public virtual  void Send(ProtocolBase protocol) {}
        //bool KickOff(string id, ProtocolBase proto);

       public virtual bool  Logout() { Console.WriteLine("IPlayer Logout is flase"); return false; }
        // Conn GetConn();

        // string GetId();


        public virtual bool SavePlayer() { Console.WriteLine("IPlayer SavePlayer is flase"); return false; }

        public virtual bool GetPlayerData() { Console.WriteLine("IPlayer GetPlayerData is flase"); return false; }
        // public byte[] Serialize(IPlayer player);

    }
}
