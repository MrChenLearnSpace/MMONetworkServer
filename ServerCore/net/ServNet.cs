using System;
using System.Collections.Generic;

using System.Net;
using System.Net.Sockets;
using System.Reflection;
#pragma warning disable CS8622
#pragma warning disable CS8618 
#pragma warning disable CS8603

//using ServerLoginHotfix;
namespace ServerCore.net {
    //网络底层 ，使用异步 TCP处理客户端连接，读取客户端消息后分发给HandleConnMsg和HandlePlayerMsg 处理
    public class ServNet {
        public Socket listenfd;
        public List<Conn> clients = new List<Conn>();
        public Dictionary<string, IPlayer> players = new Dictionary<string, IPlayer>();
        //public Conn[] conns;

        //public int maxConn = 50;

        public static ServNet instance;

        public string HandleDllName = "null";

        //主定时器
        System.Timers.Timer timer = new System.Timers.Timer(1000);
        public ProtocolBase proto;



        public bool isShowTime;
        //心跳时间
        public long heartBeatTime = 800;


        public ServNet() {
            if (instance == null) {
                instance = this;
                proto = new ProtocolBytes();
            }

        }
    
        public void Start(string host, int port) {
            //instance = new ServNet();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(HandleMainTimer);
            timer.AutoReset = false;
            timer.Enabled = true;


            //conns = new Conn[maxConn];
            //for (int i = 0; i < maxConn; i++) {
            //    conns[i] = new Conn();
            //}
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAdr = IPAddress.Parse(host);
            IPEndPoint ipEp = new IPEndPoint(ipAdr, port);
            listenfd.Bind(ipEp);
            listenfd.Listen(0);
            listenfd.BeginAccept(AcceptCb, null);
            Console.WriteLine("[服务器]启动成功");
        }
        private void AcceptCb(IAsyncResult ar) {
            Socket socket = listenfd.EndAccept(ar);
            try {
           
                Conn conn = new Conn();
                conn.Init(socket);
                clients.Add(conn);
                string host = conn.GetAdress();
                Console.WriteLine("客户端连接:[" + host + "] conn池ID:" + clients.IndexOf(conn));
                conn.socket.BeginReceive(conn.readBuff.bytes, conn.readBuff.writeIdx, conn.BuffRemain(), SocketFlags.None, ReciveCb, conn);
                listenfd.BeginAccept(AcceptCb, null);//再次调用AcceprCb回调函数
            }
            catch (Exception e) {
                Console.WriteLine("AccpetCb 失败:" + e.Message);
            }
        }
        private void ReciveCb(IAsyncResult ar) {
            Conn conn = (Conn)ar.AsyncState;//这个AsyncState就是上面那个BeginRecive函数里面最后一个参数
            if (conn == null)
                return;
            if (!conn.isUse) {
                clients.Remove(conn);
                return;
            }

            lock (conn) {
                // try {
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
                /*  }
                  catch (Exception e) {
                      Console.WriteLine("Recive失败" + e.Message);
                  }*/
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
            //  Console.WriteLine("Name: "+protocol.GetName()+ "GetDesc: " + protocol.GetDesc());
            HandleMsg(conn, protocol);

            //清除已处理的消息
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
            if (conn.player == null || methodName == "MsgHeatBeat" || methodName == "MsgLogout") {
                MethodInfo mm = CodeLoader.GetInstance().Find(HandleDllName, HandleDllName + ".HandleConnMsg").GetType().GetMethod(methodName);
                if (mm == null) {
                    string str = "[警告]ConnHandleMsg没有处理连接方法 ";
                    Console.WriteLine(str + methodName);
                    return;
                }
                Action<Conn, ProtocolBase> updateDel = (Action<Conn, ProtocolBase>)Delegate.CreateDelegate(typeof(Action<Conn, ProtocolBase>), null, mm);

                updateDel(conn, protoBase);
                if (methodName != "MsgHeatBeat")
                    Console.WriteLine("[处理连接消息]" + conn.GetAdress() + " :" + methodName);
            }
            //角色协议分发
            else {
                MethodInfo mm = CodeLoader.GetInstance().Find(HandleDllName, HandleDllName + ".HandlePlayerMsg").GetType().GetMethod(methodName);
                if (mm == null) {
                    string str = "[警告]PlayerHandleMsg没有处理玩家方法";
                    Console.WriteLine(str + methodName);
                    return;
                }
                Action<IPlayer, ProtocolBase> updateDel = (Action<IPlayer, ProtocolBase>)Delegate.CreateDelegate(typeof(Action<IPlayer, ProtocolBase>), null, mm);
                updateDel(conn.player, protoBase);
                Console.WriteLine("[处理玩家消息]" + conn.player.id + " :" + methodName);
            }
        }

        public void HandleMainTimer(object sender, System.Timers.ElapsedEventArgs e) {
            //处理心跳
            HeartBeat();
            timer.Start();
        }
        //心跳
        public void HeartBeat() {
            if (isShowTime)
                Console.WriteLine("[主定时器执行]");
            long timeNow = Sys.GetTimeStamp();
            for (int i = 0; i < clients.Count; i++) {
                Conn conn = clients[i];
                if (conn == null) continue;
                if (!conn.isUse) continue;
                if (conn.lastTickTime < timeNow - heartBeatTime) {
                    Console.WriteLine("[心跳引起断开连接]" + conn.GetAdress());
                    lock (conn) {
                        conn.Close(); 
                        
                    }
                }
            }
        }

        public void Boradcast(ProtocolBase protocol) {
            for (int i = 0; i < clients.Count; i++) {
                if (!clients[i].isUse) continue;
                if (clients[i].player == null) continue;
                // Send(conns[i], protocol);
                clients[i].SendAsync(protocol);
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
            for (int i = 0; i < clients.Count; i++) {
                if (!clients[i].isUse) continue;
                if (clients[i].player == null) continue;

                string str = "连接 [" + clients[i].GetAdress() + "]";
                if (clients[i].player != null)
                    str += ("玩家id " + clients[i].player.id);
                Console.WriteLine(str);
            }
        }
        public void Close() {
            ProtocolBytes prore = new ProtocolBytes();
            prore.AddString("Kick");


            //全部下线
            for (int i = 0; i < clients.Count; i++) {
                if (!clients[i].isUse) continue;
                if (clients[i].player == null) continue;
                if (clients[i].player != null) {
                    clients[i].player.Send(prore);
                    clients[i].player.Logout();
                    return;
                }
                clients[i].Close();
            }
            Console.WriteLine("全部下线");
        }
    }
}
