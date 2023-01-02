﻿using Google.Protobuf.WellKnownTypes;
using MongoDB.Bson;
using MongoDB.Driver;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLogicHotfix {
    public class RoomManager {
        private static int maxId = 0;
        public static Dictionary<int, Room> rooms = new Dictionary<int, Room>();
        public static Room AddRoom() {
            maxId++;
            Room room = new Room();
            room.id= maxId;
            rooms.Add(maxId, room);
           //((Mongo)DataMgr.instance.database).database.GetCollection<Room>("room").InsertOne(room);
           return room;
        }

        //删除房间
        public static bool RemoveRoom(int id) {
             rooms.Remove(id);
            //FilterDefinition<Room> findDefinition = Builders<Room>.Filter.Eq("id", id);
            //var result = ((Mongo)DataMgr.instance.database).database.GetCollection<Room>("room").DeleteOne(findDefinition);
            return true;
        }

        //获取房间
        public static  Room GetRoom(int id) {
            if (rooms.ContainsKey(id)) {
                return rooms[id];
            }
            return null;
            //FilterDefinition<Room> findDefinition = Builders<Room>.Filter.Eq("id", id);
            //Room room = ((Mongo)DataMgr.instance.database).database.GetCollection<Room>("room").Find(findDefinition).ToList()[0];
        }
        //将数据库更新到rooms中
        public static void ReflashRooms() {
            var collection = ((Mongo)DataMgr.instance.database).database.GetCollection<Room>("rooms");
            FilterDefinition<Room> findDefinition = Builders<Room>.Filter.Empty;
            List<Room> rooms = collection.Find(findDefinition).ToList();

            //if (rooms.ContainsKey(id)) {
            //    return rooms[id];
            //}
            // FilterDefinition<Room> findDefinition = Builders<Room>.Filter.Eq("id",room.id);
            // UpdateDefinition<Room> update = Builders<Room>.Update
            //     .Set("ownerId", room.ownerId).Set("playerIds", room.playerIds).Set("status", room.status);

            //((Mongo)DataMgr.instance.database).database.GetCollection<Room>("room").UpdateOne(findDefinition).ToList()[0];
            return ;
        }
        public static void SaveRooms() {
            var collection = ((Mongo)DataMgr.instance.database).database.GetCollection<Room>("rooms");
            FilterDefinition<Room> filter =Builders<Room>.Filter.Empty;
            collection.DeleteMany(filter);
            foreach (Room room in rooms.Values) {            
                    collection.InsertOne(room);
            }
        }
    }
}
