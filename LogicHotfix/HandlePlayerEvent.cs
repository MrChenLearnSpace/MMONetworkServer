using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using ServerCore;

namespace LogicHotfix {
    //处理玩家事件，某个事件发生时需要处理的事情,比如:玩家登录、登出
    public class HandlePlayerEvent {
        public static void OnLogin(IPlayer iplayer) {
            if (iplayer == null) return;
            Player player= (Player)iplayer;
            PlayerManager.players.Add(player.Id,player);

        }
        public static void OnLogout(IPlayer iplayer) {
            PlayerManager.players.Remove(iplayer.Id);
        }

    }
}
