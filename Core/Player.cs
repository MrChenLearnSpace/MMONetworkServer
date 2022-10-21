using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
//using MMONetworkServer.Logic;
namespace MMONetworkServer.Core {
   // 游戏中的角色，功能包括:给角色发消息、踢下线、保存角色数据等
    public class Player {
        public string id;
        public Conn conn;
        public PlayerData data;
        public PlayerTempData tempData;
        public Player(string id , Conn conn) {
            this.id = id;
            this.conn = conn;
            tempData = new PlayerTempData();
        }
        public void Send(ProtocolBase protocol) {
            if (conn == null)
                return;
            ServNet.instance.Send(conn, protocol);
        }
        public static bool KickOff(string id , ProtocolBase proto) {
            Conn[] conns = ServNet.instance.conns;
            for(int i = 0; i < conns.Length; i++) {
                if (!conns[i].isUse) continue;
                if (conns[i].player == null) continue;
                if (conns[i].player == null) continue;
                if(conns[i].player.id ==id) {
                    lock(conns[i].player) {
                        if(proto!=null) {
                            conns[i].player.Send(proto);
                        }
                        return conns[i].player.Logout();
                    }
                }
            }
            return true;
        }

        public bool Logout() {
            //ServNet.instance.handlePlayerEvent.OnLogout(this);
           
            CodeLoader.GetInstance().FindFunRun("MMONetworkServer.Logic.HandleConnMsg", "OnLogout", new object[] { this });
            if (!DataMgr.instance.SavePlayer(this))
                return false;
            conn.player = null;
            conn.Close();
            return true;
        }
    }
}
