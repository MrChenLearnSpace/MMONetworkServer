using System;
using System.Collections.Generic;
using System.Text;

using MySql.Data;
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Data;
using System.Reflection;
using MMONetworkServer.Core;
//using ServerLoginHotfix;
namespace MMONetworkServer.net {
    //网络底层 ，使用异步 TCP处理客户端连接，读取客户端消息后分发给HandleConnMsg和HandlePlayerMsg 处理
    public class ServNet {
        public Socket listenfd;

        public Conn[] conns;

        public int maxConn = 50;

        public static ServNet instance;



        //主定时器
        System.Timers.Timer timer = new System.Timers.Timer(1000);
        public ProtocolBase proto;



        public bool isShowTime;
        //心跳时间
        public long heartBeatTime = 800;
        /*
                //消息分发
                public HandleConnMsg handleConnMsg = new HandleConnMsg();
                public HandlePlayerMsg handlePlayerMsg = new HandlePlayerMsg();
                public HandlePlayerEvent handlePlayerEvent = new HandlePlayerEvent();
        */

        public ServNet() {
            instance = this;
            proto = new ProtocolBytes();
        }
        public int NewIndex() {
            if (conns == null) {
                return -1;
            }
            for (int i = 0; i < conns.Length; i++) {
                if (conns[i] == null) {
                    conns[i] = new Conn();
                    return i;
                }
                else if (conns[i].isUse == false) {
                    return i;
                }
            }
            return -1;
        }
        public void Start(string host, int port) {

            timer.Elapsed += new System.Timers.ElapsedEventHandler(HandleMainTimer);
            timer.AutoReset = false;
            timer.Enabled = true;


            conns = new Conn[maxConn];
            for (int i = 0; i < maxConn; i++) {
                conns[i] = new Conn();
            }
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAdr = IPAddress.Parse(host);
            IPEndPoint ipEp = new IPEndPoint(ipAdr, port);
            listenfd.Bind(ipEp);
            listenfd.Listen(maxConn);
            listenfd.BeginAccept(AcceptCb, null);
            Console.WriteLine ("[服务器]启动成功");
        }
         private void AcceptCb(IAsyncResult ar) {
            Socket socket = listenfd.EndAccept(ar);
            try {
                //接收客户端
                int index = NewIndex();
                if (index < 0) {
                    socket.Close();
                    Console.WriteLine("[警告]连接已满");
                }
                else {
                    Conn conn = conns[index];
                    conn.Init(socket);
                    string host = conn.GetAdress();
                    Console.WriteLine("客户端连接:[" + host + "] conn池ID:" + index);
                    conn.socket.BeginReceive(conn.readBuff.bytes, conn.readBuff.writeIdx, conn.BuffRemain(), SocketFlags.None, ReciveCb, conn);//接收的同时调用ReciveCb回调函数
                }
                listenfd.BeginAccept(AcceptCb, null);//再次调用AcceprCb回调函数
            }
            catch (Exception e) {
                Console.WriteLine("AccpetCb 失败:" + e.Message);
            }
        }
        private  void ReciveCb(IAsyncResult ar) {
            Conn conn = (Conn)ar.AsyncState;//这个AsyncState就是上面那个BeginRecive函数里面最后一个参数
            if (!conn.isUse)
                return;
            lock (conn) {
                try {
                    int count = conn.socket.EndReceive(ar);//返回接收的字节数
                                                           //没有信息就关闭
                    
                    if (count <= 0) {
                        Console.WriteLine("收到[" + conn.GetAdress() + "] 断开连接");
                        conn.Close();
                        return;
                    }
                    conn.readBuff.writeIdx += count;
                    ProcessData(conn);
                    if (conn.BuffRemain() < 8) {
                        conn.readBuff.MoveBytes();
                        conn.readBuff.ReSize(conn.readBuff.length * 2);
                    }
                    //继续接收
                    conn.socket.BeginReceive(conn.readBuff.bytes, conn.readBuff.writeIdx, conn.BuffRemain(), SocketFlags.None, ReciveCb, conn);
                }
                catch (Exception e) {
                    Console.WriteLine("Recive失败" + e.Message);
                }
            }
        }

        private void ProcessData(Conn conn) {
            if (conn.readBuff.length < sizeof(Int32)) {
                return;
            }
           // Console.WriteLine("接收到了 " + conn.readBuff.length + " 个字节") ;
            Array.Copy(conn.readBuff.bytes, conn.lenBytes, sizeof(Int32));
            conn.msgLength = BitConverter.ToInt32(conn.lenBytes, 0);

            //小于最小要求长度则返回表示未接收完全
            if (conn.readBuff.length < conn.msgLength + sizeof(Int32)) {
                return;
            }
            ProtocolBase protocol = proto.Decode(conn.readBuff.bytes, sizeof(Int32), conn.msgLength);
            Console.WriteLine("Name: "+protocol.GetName()+ "GetDesc: " + protocol.GetDesc());
            HandleMsg(conn, protocol);



            //这里接收信息有个细节，因为之前发送回来的信息又被加了一次长度，相当于要把他所有的信息接收完了
            //才算接收成功，然后再把前面的sizeof(Int32)去掉，剩下的就是带长度的信息了
            /*ProtocolByte proto = new ProtocolByte();
            ProtocolByte protoStr = new ProtocolByte();
            ProtocolByte protocol = proto.Decode(conn.readBuff, sizeof(Int32), conn.msgLength) as ProtocolByte;
            protoStr.AddString(conn.GetAdress());
            protocol.bytes = protoStr.bytes.Concat(protocol.bytes).ToArray();
            lock (msgHandle.msgList) {
                msgHandle.msgList.Add(protocol);
            }
            Console.WriteLine(protocol.GetDesc());
*/

            //清除已处理的消息
            //int count = conn.buffCount - conn.msgLength - sizeof(Int32);
            //Array.Copy(conn.readBuff, sizeof(Int32) + conn.msgLength, conn.readBuff, 0, count);
            //conn.buffCount = count;
            int count = sizeof(Int32) + conn.msgLength;
            conn.readBuff.readIdx += count;
            conn.readBuff.CheckAndMoveBytes();
            //如果还有多余信息就继续处理
            if (conn.readBuff.length > 4) {
                ProcessData(conn);
            }
        }

        private void HandleMsg(Conn conn, ProtocolBase protoBase) {
            
            string methodName = protoBase.GetName();
           // string methodName = "Msg" + name;
            //连接协议分发
           // if (conn.player == null || name == "HeatBeat" || name == "Logout") {
            if (conn.player == null || methodName == "HeatBeat" || methodName == "MsgLogout") {
                MethodInfo mm = CodeLoader.GetInstance().Find("ServerLoginHotfix", "ServerLoginHotfix.HandleConnMsg").GetType().GetMethod(methodName);
                if (mm == null) {
                    string str = "[警告]HandleMsg没有处理连接方法 ";
                    Console.WriteLine(str + methodName);
                    return;
                }
                //Object[] obj = new object[] { conn, protoBase };
                //Console.WriteLine("[处理连接消息]" + conn.GetAdress() + " :" + name);
                //mm.Invoke(CodeLoader.GetInstance().Find("ServerLoginHotfix.HandleConnMsg"), obj);
                Action<Conn, ProtocolBase> updateDel = (Action<Conn, ProtocolBase>)Delegate.CreateDelegate(typeof(Action<Conn, ProtocolBase>), null, mm);

                updateDel(conn, protoBase);
                Console.WriteLine("[处理连接消息]" + conn.GetAdress() + " :" + methodName);
            }
            //角色协议分发
            else {
                MethodInfo mm = CodeLoader.GetInstance().Find("ServerLoginHotfix", "ServerLoginHotfix.HandlePlayerMsg").GetType().GetMethod(methodName);
                if (mm == null) {
                    string str = "[警告]HandleMsg没有处理玩家方法";
                Console.WriteLine(str + methodName);
                    return;
                }
                //Object[] obj = new object[] { conn.player, protoBase };
                //Console.WriteLine("[处理玩家消息]" + conn.player.GetId() + " :" + name);
                //mm.Invoke(CodeLoader.GetInstance().Find("ServerLoginHotfix.HandlePlayerMsg"), obj);
                Action<IPlayer, ProtocolBase> updateDel = (Action<IPlayer, ProtocolBase>)Delegate.CreateDelegate(typeof(Action<IPlayer, ProtocolBase>), null, mm);
                updateDel(conn.player, protoBase);
                Console.WriteLine("[处理玩家消息]" + conn.player.GetId() + " :" + methodName);
            }
        }

        public void HandleMainTimer(object sender, System.Timers.ElapsedEventArgs e) {
            //处理心跳
            HeartBeat();
            timer.Start();
        }
        //心跳
        public void HeartBeat() {
            if(isShowTime)
            Console.WriteLine("[主定时器执行]");
        long timeNow = Sys.GetTimeStamp();
            for (int i = 0; i < conns.Length; i++) {
                Conn conn = conns[i];
                if (conn == null) continue;
                if (!conn.isUse) continue;
                if (conn.lastTickTime < timeNow - heartBeatTime) {
                    Console.WriteLine("[心跳引起断开连接]" + conn.GetAdress());
                lock (conn)
                        conn.Close();
                }
            }
        }

        public void Boradcast(ProtocolBase protocol) {
            for (int i = 0; i < conns.Length; i++) {
                if (!conns[i].isUse) continue;
                if (conns[i].player == null) continue;
                // Send(conns[i], protocol);
                conns[i].Send(protocol);
            }
        }
        public string GetLocalIp() {
            ///获取本地的IP地址
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList) {

                if (_IPAddress.AddressFamily.ToString() == "InterNetwork") {

                    AddressIP = _IPAddress.ToString();
                }
            }
            return AddressIP;
        }
        public void Print() {
            Console.WriteLine("===== 服务器登录信息 =====");
            for (int i = 0; i < conns.Length; i++) {
                if (!conns[i].isUse) continue;
                if (conns[i].player == null) continue;

                string str = "连接 [" + conns[i].GetAdress() + "]";
                if (conns[i].player != null)
                    str += "玩家id " + conns[i].player.GetId();
                Console.WriteLine(str);
            }
        }
        public void Close() {
            ProtocolBytes prore = new ProtocolBytes();
            prore.AddString("Kick");
            

            //全部下线
            for (int i = 0; i < conns.Length; i++) {
                if (!conns[i].isUse) continue;
                if (conns[i].player == null) continue;
                if (conns[i].player != null) {
                    conns[i].player.Send(prore);
                    conns[i].player.Logout();
                    return;
                }
                conns[i].Close();
            }
            Console.WriteLine("全部下线");
        }
    }
}
