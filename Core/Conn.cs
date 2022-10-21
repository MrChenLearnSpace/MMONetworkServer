using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Linq;
namespace MMONetworkServer.Core {
    public class Conn {
        public const int BUFFER_SIZE = 16384;
        public Socket socket;
        public bool isUse = false;
        public byte[] readBuff = new byte[BUFFER_SIZE];
        public int buffCount = 0;

        public Int32 msgLength = 0;
        public byte[] lenBytes = new byte[sizeof(UInt32)];
        public long lastTickTime = long.MinValue;
        public Player player;

        public Conn() {
            readBuff = new byte[BUFFER_SIZE];
        }
        public void Init(Socket socket) {
            this.socket = socket;
            isUse = true;
            buffCount = 0;
            lastTickTime = Sys.GetTimeStamp();
        }
        public int BuffRemain() {
            return BUFFER_SIZE - buffCount;
        }
        public string GetAdress() {
            if (!isUse) { return "无法获取地址"; }
            return socket.RemoteEndPoint.ToString();
        }
        public void Close() {
            if (!isUse) {
                return;
            }
            if(player !=null) {
                // player.Logout();
                return;
            }
            Console.WriteLine("[断开连接]" + GetAdress());
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            isUse = false;
        }
        public void Send(ProtocolBase protol) {
            ServNet.instance.Send(this, protol);

        }

    }
}
