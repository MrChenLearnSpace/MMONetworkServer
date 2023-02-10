using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ServerCore;
using ServerCore.net;
using MongoDB.Driver;
using Google.Protobuf.WellKnownTypes;

namespace CSLogicHotfix {
    public class Player : IPlayer {
   
        public PlayerData data;
        public PlayerTempData tempData;

        
      

        //public int Id { get; set; }
        public Player() { }
        public Player(string id, Conn conn) {
            this.id = id;
            this.client = conn;
            tempData = new PlayerTempData();
        }
        public override void Send(ProtocolBase protocol) {
            if (client == null)
                return;
            client.Send(protocol);
            //ServNet.instance.Send(conn, protocol);
        }
        public void SendAsync(ProtocolBase protocol) {
            if (client == null) return;
            client.SendAsync(protocol);
        }

        public bool Logout() {
            //ServNet.instance.handlePlayerEvent.OnLogout(this);

            //CodeLoader.GetInstance().FindFunRun("MMONetworkServer.Logic.HandlePlayerEvent", "OnLogout", new object[] { this });
            HandlePlayerEvent.OnLogout(this);
            if (!SavePlayer())
                return false;
            client.player = null;
            client.Close();
            return true;
        }
        public bool SavePlayer() {
            try {
                if (!DataMgr.instance.IsSafeStr(id))
                    return false;
                FilterDefinition<PlayerSaveData> filter = Builders<PlayerSaveData>.Filter.Eq("id", id);
                UpdateDefinition<PlayerSaveData> update = Builders<PlayerSaveData>.Update.Set("playerData", data).Set("ip", client.GetAdress()).Set("tempData",tempData);
                
                UpdateResult result = ((Mongo)DataMgr.instance.database).database.GetCollection<PlayerSaveData>("player").UpdateOne(filter, update);
                return result.IsAcknowledged;
            }
            catch (Exception ex) {
                Console.WriteLine("[Player] SavePlayer " + ex.Message);
                return false;
            }
            //LogicManager logic = new LogicManager();
            //string buff = LogicManager.Serialize(data);
            //return DataMgr.instance.SavePlayerStream(id, buff, conn.GetAdress());
        }

        public bool GetPlayerData() {
            try {
                if (!DataMgr.instance.IsSafeStr(id))
                    return false;
                FilterDefinition<PlayerSaveData> filter = Builders<PlayerSaveData>.Filter.Eq("id", id);
                List<PlayerSaveData> playerList = ((Mongo)DataMgr.instance.database).database.GetCollection<PlayerSaveData>("player").Find(filter).ToList();
                if (playerList.Count != 1) {
                    Console.WriteLine("[Player] GetPlayerData Count = " + playerList.Count);
                    return false;
                }
                data = playerList[0].playerData;
                return true;
            }
            catch (Exception ex) {
                Console.WriteLine("[Player] GetPlayerData " + ex.Message);
                return false;
            }
        }

    }
}
