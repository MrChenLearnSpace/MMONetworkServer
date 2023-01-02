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
            int count1 = 0;
            int count2 = 0;
            foreach (string id in playerIds.Keys) {
                Player player = PlayerManager.players[id];
                if (player.tempData.camp == 1) { count1++; }
                if (player.tempData.camp == 2) { count2++; }
            }
            //选择
            if (count1 <= count2) {
                return 1;
            }
            else {
                return 2;
            }
        }
        public bool isOwner(Player player) {
            return player.id == ownerId;
        }
        public bool RemovePlayer(string id) {
            Player player = PlayerManager.players[id];
            if (player == null) {
                Console.WriteLine("room.RemovePlayer fail, player is null");
                return false;
            }
            //没有在房间里
            if (!playerIds.ContainsKey(id)) {
                Console.WriteLine("room.RemovePlayer fail, not in this room");
                return false;
            }
            //删除列表
            playerIds.Remove(id);
            //设置玩家数据
            player.tempData.camp = 0;
            player.tempData.roomId = -1;
            //设置房主
            if (ownerId == player.id) {
                ownerId = SwitchOwner();
            }
            //战斗状态退出
            if (status == 1) {
                player.data.lostNum++;
                MsgLeaveBattle msg = new MsgLeaveBattle();
                msg.id = player.id;
                Broadcast(msg);
            }
            //房间为空
            if (playerIds.Count == 0) {
                RoomManager.RemoveRoom(this.id);
            }
            //广播
            Broadcast(ToMsg());
            return true;
        }
        public string SwitchOwner() {
            foreach (string id in playerIds.Keys) {
                return id;
            }
            //房间没人
            return "";
        }
        public void Broadcast(MsgBase msg) {
            ProtocolBytes protocolBytes= msg.Encode();

            foreach (string id in playerIds.Keys) {
                Player player = PlayerManager.players[id];
                player.SendAsync(protocolBytes);
            }
        }
        //生成MsgGetRoomInfo协议
        public MsgBase ToMsg() {
            MsgGetRoomInfo msg = new MsgGetRoomInfo();
            //int count = playerIds.Count;
            //msg.players = new RoomPlayerInfo[count];
            //players
   
            foreach (string id in playerIds.Keys) {
                Player player = PlayerManager.players[id];
                RoomPlayerInfo playerInfo = new RoomPlayerInfo();
                //赋值
                playerInfo.id = player.id;
                playerInfo.camp = player.tempData.camp;
                playerInfo.win = player.data.winNum;
                playerInfo.lost = player.data.lostNum;
                playerInfo.isOwner = 0;
                if (isOwner(player)) {
                    playerInfo.isOwner = 1;
                }
                msg.players .Add(playerInfo);
            }
            return msg;
            
        }
        //能否开战
        public bool CanStartBattle() {
            if (status != 0) {
                return false;
            }
            //统计每个队伍的玩家数
            int count1 = 0;
            int count2 = 0;
            foreach (string id in playerIds.Keys) {
                Player player = PlayerManager.players[id];
                if (player.tempData.camp == 1) { count1++; }
                else { count2++; }
            }
            //每个队伍至少要有1名玩家
            if (count1 < 1 || count2 < 1) {
                return false;
            }
            return true;
        }
        //初始化位置
        private void SetBirthPos(Player player) {
            Scene2 scene2 = new Scene2();
            Random random= new Random();
            if (player.tempData.camp == 1) {
                player.tempData.x = RandomFloat(scene2.CTvexter[0], scene2.CTvexter[2], random);
                player.tempData.z = RandomFloat(scene2.CTvexter[1], scene2.CTvexter[3], random);
                player.tempData.y = scene2.CTvexter[4];
            }
            else {
                player.tempData.x = RandomFloat(scene2.Tvexter[0], scene2.Tvexter[2], random);
                player.tempData.z = RandomFloat(scene2.Tvexter[1], scene2.Tvexter[3], random);
                player.tempData.y = scene2.Tvexter[4];
            }
        }
        public bool StartBattle() {
            if (!CanStartBattle()) {
                return false;
            }
            //状态
            status =1;
            //玩家战斗属性
          //  ResetPlayers();
            //返回数据
            MsgEnterBattle msg = new MsgEnterBattle();
            msg.mapId = 1;

            foreach (string id in playerIds.Keys) {
                Player player = PlayerManager.players[id];
                msg.gamePlayer.Add(RoomToGamePlayer(player));
            }
            Broadcast(msg);
            return true;
        }
        public void ResetPlayers() {
            foreach (string id in playerIds.Keys) {
                Player player = PlayerManager.players[id];
                SetBirthPos(player);

                player.tempData.hp = 100;
            }
        }
        public PlayerInfo RoomToGamePlayer(Player player) {
            PlayerInfo playerInfo =new PlayerInfo();
            playerInfo.camp = player.tempData.camp;
            playerInfo.hp = player.tempData.hp;
            playerInfo.x = player.tempData.x;
            playerInfo.y = player.tempData.y;
            playerInfo.z = player.tempData.z;
            return playerInfo;
        }
        public bool IsDie(Player player) {
            return player.tempData.hp <= 0;
        }
        public void Update() {
            //状态判断
            if (status != 1) {
                return;
            }
            //时间判断
            if (Sys.GetTimeStamp() - lastjudgeTime < 10f) {
                return;
            }
            lastjudgeTime = Sys.GetTimeStamp();
            //胜负判断
            int winCamp = Judgment();
            //尚未分出胜负
            if (winCamp == 0) {
                return;
            }
            //某一方胜利，结束战斗
            status = 0;
            //统计信息
            foreach (string id in playerIds.Keys) {
                Player player = PlayerManager.players[id];
                if (player.tempData.camp == winCamp) { player.data.winNum++; }
                else { player.data.lostNum++; }
            }
            //发送Result
            MsgBattleResult msg = new MsgBattleResult();
            msg.winCamp = winCamp;
            Broadcast(msg);
        }

        public int Judgment() {
            //存活人数
            int count1 = 0;
            int count2 = 0;
            foreach (string id in playerIds.Keys) {
                Player player = PlayerManager.players[id];
                if (!IsDie(player)) {
                    if (player.tempData.camp == 1) { count1++; };
                    if (player.tempData.camp == 2) { count2++; };
                }
            }
            //判断
            if (count1 <= 0) {
                return 2;
            }
            else if (count2 <= 0) {
                return 1;
            }
            return 0;
        }
        float RandomFloat(float Min, float Max, Random random) {
            float index = (float)random.NextDouble();
            float length = Max - Min;
            index *= length;
            index += Min;
            return index;

        }
    }
}
