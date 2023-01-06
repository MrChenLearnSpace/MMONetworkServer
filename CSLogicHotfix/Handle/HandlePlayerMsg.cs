using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using ServerCore;

namespace CSLogicHotfix {
    //处理角色消息 ，具体是登录成功后的逻辑,比如:强化装备、打副本
    public partial class HandlePlayerMsg {
       
        public void MsgGetPlayerData(IPlayer iplayer, ProtocolBase protoBase) {
           // if (iplayer == null) return;
            Player player = iplayer as Player;
            if (player == null) return;
            string data = JsonConvert.SerializeObject(player.data);
            ProtocolBytes protocolGetPlayerData = new ProtocolBytes();
            protocolGetPlayerData.AddString("MsgGetPlayerData");
            protocolGetPlayerData.AddString(data);
            //player.Send(protocolGetPlayerData);
            player.conn.SendAsync(protocolGetPlayerData);
        }
        public void MsgGetAchieve(IPlayer iplayer, ProtocolBase protocolBase) {

            Player player = iplayer as Player;
            if (player == null) {
                Console.WriteLine("Player is null");
                return;
            }
            //  ProtocolBytes protocolBytes = protocolBase as ProtocolBytes;
            MsgGetAchieve msgGetAchieve = new MsgGetAchieve();

            msgGetAchieve.win = player.data.winNum;
            msgGetAchieve.lost = player.data.lostNum;
            protocolBase = msgGetAchieve.Encode();
            player.SendAsync(protocolBase);

        }
    }
}
