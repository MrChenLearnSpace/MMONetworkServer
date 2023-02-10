using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace ServerCore {
    public class Mongo : IDatabase {
        MongoClient client;
        public IMongoDatabase database;


        public void Connect(string Database, string DataSource, string port, string user, string pw) {
            string cmdStr = string.Format("mongodb://{0}:{1}", DataSource, port);

            MongoClient client = new MongoClient(cmdStr);
            try {
                database = client.GetDatabase(Database);
                Console.WriteLine("Connected to MongoDB!"+ Database);
            }
            catch (Exception ex) {
                Console.WriteLine("Error connecting to MongoDB: " + ex.Message);
            }
           
            
        }

        public bool Register(string id, string pw) {
            FilterDefinition<BsonDocument> filterDefinition = Builders<BsonDocument>.Filter.Eq("id", id);
            var collection = database.GetCollection<BsonDocument>("account");

            if (collection.Find(filterDefinition).ToList().Count> 0) {
                Console.WriteLine("DataMgr Resister失败,!CanRegister");
                return false;
            }
            BsonDocument file = new BsonDocument {
                {"_id",id },
                { "pw", pw }
            };
            collection.InsertOne(file);
            return true;
        }

        public bool CheckPassWord(string id, string pw) {
            FilterDefinition<BsonDocument> filterDefinition = Builders<BsonDocument>.Filter.Where(
                x =>  x["_id"] == id && x["pw"] == pw);
            var collection = database.GetCollection<BsonDocument>("account");
            if (collection.Find(filterDefinition).ToList().Count > 0) {
                return true;
            }
            return false;
        }

        public bool InsertPlayerData(string id, string playerStream, string ip) {
            throw new NotImplementedException();
        }

        public string GetPlayerData(string id) {
            throw new NotImplementedException();
        }

        public bool SavePlayerData(string id, string playerStream, string ip) {
            throw new NotImplementedException();
        }
    }
}
