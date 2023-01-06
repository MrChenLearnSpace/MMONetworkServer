using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLogicHotfix {
    //处理角色消息 ，具体是登录成功后的逻辑,比如:强化装备、打副本
    public partial class HandlePlayerMsg {

        public void MsgGetRoomList(IPlayer iplayer, ProtocolBase protocolBase) {

            Player player = iplayer as Player;
            if (player == null) {
                Console.WriteLine("Player is null");
                return;
            }
            RoomInfo roomInfo = new RoomInfo();
            MsgGetRoomList msgGetRoomList = new MsgGetRoomList();

            foreach (Room room in RoomManager.rooms.Values) {
                roomInfo.id = room.id;
                roomInfo.count = room.playerIds.Count;
                roomInfo.status = room.status;
                msgGetRoomList.rooms.Add(roomInfo);
            }
            protocolBase = msgGetRoomList.Encode();
            player.SendAsync(protocolBase);
        }
        public void MsgCreateRoom(IPlayer iplayer, ProtocolBase protocolBase) {

            Player player = iplayer as Player;
            if (player == null) {
                Console.WriteLine("Player is null");
                return;
            }
            
            
        }
    }
}