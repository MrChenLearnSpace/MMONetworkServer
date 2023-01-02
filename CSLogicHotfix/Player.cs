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
        public string id;
        public Conn conn;
        public PlayerData data;
        public PlayerTempData tempData;

        public string Id { get => id; set => id =value; }
        public Conn Connect { get => conn; set => conn =value; }

        //public int Id { get; set; }
        
        public Player(string id, Conn conn) {
            this.id = id;
            this.conn = conn;
            tempData = new PlayerTempData();
        }
        public void Send(ProtocolBase protocol) {
            if (conn == null)
                return;
            conn.Send(protocol);
            //ServNet.instance.Send(conn, protocol);
        }


        public bool Logout() {
            //ServNet.instance.handlePlayerEvent.OnLogout(this);

            //CodeLoader.GetInstance().FindFunRun("MMONetworkServer.Logic.HandlePlayerEvent", "OnLogout", new object[] { this });
            HandlePlayerEvent.OnLogout(this);
            if (!SavePlayer())
                return false;
            conn.player = null;
            conn.Close();
            return true;
        }
        public bool SavePlayer() {
            try {
                if (!DataMgr.instance.IsSafeStr(id))
                    return false;
                FilterDefinition<PlayerSaveData> filter = Builders<PlayerSaveData>.Filter.Eq("id", id);
                UpdateDefinition<PlayerSaveData> update = Builders<PlayerSaveData>.Update.Set("playerData", data).Set("ip", conn.GetAdress()).Set("tempData",tempData);
                
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
