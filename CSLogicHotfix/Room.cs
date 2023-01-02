using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLogicHotfix {
    public class Room {
        public int id = 0;
        //最大玩家数
        public int maxPlayer = 6;
        public string ownerId = "";
        //状态
        public Dictionary<string, bool> playerIds = new Dictionary<string, bool>();
        public int status = 0;
        public  long lastjudgeTime = 0;

        public bool AddPlayer(string id) {
            Player player;
            if (PlayerManager.players.ContainsKey(id)) {
                 player = PlayerManager.players[id];
            }   
            else {
                Console.WriteLine(id + " is not Online");
                return false;
            }
            if (playerIds.Count >= maxPlayer) {
                Console.WriteLine("room.AddPlayer fail, reach maxPlayer");
                return false;
            }
            //已经在房间里
            if (playerIds.ContainsKey(id)) {
                Console.WriteLine("room.AddPlayer fail, already in this room");
                return false;
            }
            //if (rooms == null) return false;
            playerIds[id] = true;
            player.tempData.camp = SwitchCamp();
            if (ownerId == "") {
                ownerId = player.id;
            }
            Broadcast(ToMsg());
            //设置玩家数据
            return true;
        }

        public int SwitchCamp() {
            return -1;
        }
        public bool isOwner(Player player) {
            return player.id == ownerId;
        }
        public bool RemovePlayer(string id) {
            return false;
        }
        public string SwitchOwner() {
            return "";
        }
        public void Broadcast(MsgBase msg) {
        }
        //生成MsgGetRoomInfo协议
        public MsgBase ToMsg() {
            return new MsgBase();
        }
        //能否开战
        public bool CanStartBattle() {
            return false;
        }
        //初始化位置
        private void SetBirthPos(Player player, int index) {
        }
        public bool StartBattle() {
            return false;
        }
        public bool IsDie(Player player) {
            return false;
        }
        public void Update() {

        }

        public int Judgment() {
            return -1;
        }
    }
}
