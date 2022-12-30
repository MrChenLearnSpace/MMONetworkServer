
using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;
using ServerCore.net;
using ServerCore;

namespace LogicHotfix {

    //处理连接消息 ，具体是登录前的逻辑，比如:用户名密码校验、注册账号
    public partial class HandleConnMsg {

      
        public HandleConnMsg() {
          
        }
        public void MsgHeatBeat(Conn conn, ProtocolBase protoBase) {
            conn.lastTickTime = Sys.GetTimeStamp();
            if(ServNet.instance.isShowTime)
            Console.WriteLine("[更新心跳时间]" + conn.GetAdress());
            MsgPong msgPong = new MsgPong();
            ProtocolBytes protocolBytes = msgPong.Encode();
            conn.SendAsync(protocolBytes);
        }
        public void MsgRegister(Conn conn, ProtocolBase protoBase) {
            //获取数值
            //int start = 0;
            ProtocolBytes protocol = (ProtocolBytes)protoBase;
            MsgRegister msgReg = new MsgRegister();
            msgReg =(MsgRegister) msgReg.Decode(protocol);
            
           // string protoName = protocol.GetString(start, ref start);


            string id = msgReg.id;
            string pw = msgReg.pw;
            string strFormat = "[收到注册协议]" + conn.GetAdress();
            Console.WriteLine(strFormat + " 用户名：" + id + " 密码：" + pw);
            //构建返回协议
            protocol = new ProtocolBytes();
            //protocol.AddString("MsgRegister");
            //注册
            if (DataMgr.instance.Register(id, pw)) {
                msgReg.result = 0;
            }
            else {
                msgReg.result = -1;
            }
            //创建角色
            protocol = msgReg.Encode();
            // DataMgr.instance.CreatePlayer(id);
            LogicManager.CreatePlayer(id, conn);
            //返回协议给客户端
            conn.Send(protocol);
        }
     
        public void MsgLogin(Conn conn, ProtocolBase protoBase) {

            ProtocolBytes protocol = (ProtocolBytes)protoBase;
            MsgLogin msgLogin = new MsgLogin();
            msgLogin = (MsgLogin)msgLogin.Decode(protocol);
            string id = msgLogin.id;
            string pw = msgLogin.pw;

            string strFormat = "[收到登录协议]" + conn.GetAdress();
            Console.WriteLine(strFormat + " 用户名：" + id + " 密码：" + pw);
            //构建返回协议
            //ProtocolBytes protocolRet = new ProtocolBytes();
            //protocolRet.AddString("Login");
            //验证
            if (!DataMgr.instance.CheckPassWord(id, pw)) {
                //protocolRet.AddInt(-1);
                msgLogin.result = -1;
                protocol = msgLogin.Encode();
                conn.Send(protocol);
                Console.WriteLine(strFormat + " 用户名：" + id + " 验证");
                return;
            }
            //是否已经登录
            ProtocolBytes protocolLogout = new ProtocolBytes();
            protocolLogout.AddString("MsgLogout");
            if(PlayerManager.players.ContainsKey(id)) {
                conn.SendAsync(protocolLogout);
                if (!PlayerManager.players[id].Logout()) {
                    msgLogin.result = -1;
                    protocol = msgLogin.Encode();
                    conn.Send(protocol);
                    Console.WriteLine(strFormat + " 用户名：" + id + " 是否已经登录");
                    return;
                }

            }
           /*if (!LogicManager.KickOff(id, protocolLogout)) {
                msgLogin.result = -1;
                protocol = msgLogin.Encode();
                conn.Send(protocol);
                Console.WriteLine(strFormat + " 用户名：" + id + " 是否已经登录");

                return;
            }*/
            //获取玩家数据
            conn.player = (IPlayer)(new Player(id, conn));

            if (!conn.player.GetPlayerData()) {
                msgLogin.result = -1;
                protocol = msgLogin.Encode();
                conn.Send(protocol);
                Console.WriteLine(strFormat + " 用户名：" + id + " 获取玩家数据");

                return;
            }
           // PlayerManager.players.Add(id, (Player)conn.player );
            HandlePlayerEvent.OnLogin(conn.player);
            //返回
            msgLogin.result = 0;
            protocol = msgLogin.Encode();
            conn.Send(protocol);
            return;
        }



        public void MsgLogout(Conn conn, ProtocolBase protoBase) {
            ProtocolBytes protocol = new ProtocolBytes();
            MsgLogout msgLogout = new MsgLogout();
            protocol = msgLogout.Encode();
            if (conn.player == null) {
                conn.Send(protocol);
                conn.Close();
            }
            else {
                conn.Send(protocol);
                conn.player.Logout();
            }
        }

        public void MsgWWWW(Conn conn, ProtocolBase protoBase) {
            ProtocolBytes protocol = (ProtocolBytes)protoBase;
            ProtocolBytes str = new ProtocolBytes();
            str.AddString("ssss");
            str.AddString("dsadsadas");
            conn.SendAsync(str);
            Console.WriteLine(protocol.GetDesc());
        }
    }

}
