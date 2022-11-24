using MMONetworkServer.net;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMONetworkServer {
    public interface IPlayer {
        
        void Send(ProtocolBase protocol);
        //bool KickOff(string id, ProtocolBase proto);

        bool Logout();
        Conn GetConn();

        string GetId();


        bool SavePlayer();

        bool GetPlayerData();
       // public byte[] Serialize(IPlayer player);

    }
}
