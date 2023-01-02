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
        public void MsgRoom(IPlayer iplayer, ProtocolBase protocolBase) {

            Player player = iplayer as Player;
            ProtocolBytes protocolBytes = protocolBase as ProtocolBytes;
            //MsgRoom msgRoom = new MsgRoom();
            //if (protocolBytes == null) return;
            //msgRoom = (MsgRoom)msgRoom.Decode(protocolBytes);


            //switch (msgRoom.Camp) {
            //    case 1:
            //        if(LogicManager.CTCamp.Contains(player.id)) {
            //            LogicManager.CTCamp.Remove(player.id);
            //        }
            //        if(!LogicManager.TCamp.Contains(player.id)) {
            //            LogicManager.TCamp.Add(player.id);
            //        }

            //        break;
            //    case 2:
            //        if (LogicManager.TCamp.Contains(player.id)) {
            //            LogicManager.TCamp.Remove(player.id);
            //        }
            //        if (!LogicManager.CTCamp.Contains(player.id)) {
            //            LogicManager.CTCamp.Add(player.id);
            //        }
            //        break;

            //    default: Console.WriteLine("MsgRoom 消息错误"); break;
            //}
            //Console.WriteLine("LogicManager.ctNum: " + LogicManager.CTCamp.Count + ", LogicManager.tNum: " + LogicManager.TCamp.Count);
            //msgRoom.CtNum = LogicManager.CTCamp.Count;
            //msgRoom.TNum = LogicManager.TCamp.Count;
            //protocolBytes = msgRoom.Encode();
            //ServNet.instance.Boradcast(protocolBytes);
            //Console.WriteLine(LogicManager.isActive);
            //if (!LogicManager.isActive) {
            //    //是否开战
            //    if (msgRoom.CtNum == msgRoom.TNum && msgRoom.CtNum > 0) {
            //        Scene2 scene2 = new Scene2();
            //        Random random = new Random();
            //        MsgBattleStart msgBattleStart = new MsgBattleStart();
            //        foreach (string id in LogicManager.CTCamp) {
            //            PlayerInfo playerInfo = new PlayerInfo();
            //            playerInfo.id = id;
            //            playerInfo.camp = 1;
            //            float x = RandomFloat(scene2.CTvexter[0], scene2.CTvexter[2], random);
            //            float z = RandomFloat(scene2.CTvexter[1], scene2.CTvexter[3], random);
            //            float y = scene2.CTvexter[4];
            //            playerInfo.cvector3[0] = x;
            //            playerInfo.cvector3[1] = y;
            //            playerInfo.cvector3[2] = z;
            //            //Console.WriteLine("[id]: "+ id +" [pos]: "+ x.ToString()+" "+y.ToString()+" "+ z.ToString() );
            //            LogicManager.CTCampPlayerInfos.Add(playerInfo);
            //        }
            //        foreach (string id in LogicManager.TCamp) {
            //            PlayerInfo playerInfo = new PlayerInfo();
            //            playerInfo.id = id;
            //            playerInfo.camp = 2;
            //            float x = RandomFloat(scene2.Tvexter[0], scene2.Tvexter[2], random);
            //            float z = RandomFloat(scene2.Tvexter[1], scene2.Tvexter[3], random);
            //            float y = scene2.Tvexter[4];
            //            playerInfo.cvector3[0] = x;
            //            playerInfo.cvector3[1] = y;
            //            playerInfo.cvector3[2] = z;
            //           // Console.WriteLine("[id]: " + id + " [pos]: " + x.ToString() + " " + y.ToString() + " " + z.ToString());
            //            LogicManager.TCampPlayerInfos.Add(playerInfo); 
            //        }
            //        msgBattleStart.CtName = LogicManager.CTCampPlayerInfos;
            //        msgBattleStart.TName = LogicManager.TCampPlayerInfos;
            //        protocolBytes = msgBattleStart.Encode();
            //        ServNet.instance.Boradcast(protocolBytes);

            //        LogicManager.isActive = true;
            //    }

            //}
            //else {
            //    MsgBattleStart msgBattleStart = new MsgBattleStart();
            //    msgBattleStart.CtName = LogicManager.CTCampPlayerInfos;
            //    msgBattleStart.TName = LogicManager.TCampPlayerInfos;
            //    protocolBytes = msgBattleStart.Encode();
            //    player.conn.AsySend(protocolBytes);
            //}

        }
        float RandomFloat(float Min,float Max, Random random) {
            float index =  (float)random.NextDouble();
            float length = Max - Min;
            index *= length;
            index += Min;
            return index;

        }
    }
}
