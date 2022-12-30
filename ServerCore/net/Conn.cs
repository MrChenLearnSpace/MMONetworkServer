using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
#pragma warning disable CS8618 
#pragma warning disable CS8602 
#pragma warning disable CS8603
namespace ServerCore.net {
    public class Conn {
        //public const int BUFFER_SIZE = 16384;
        public Socket socket;
        public bool isUse = false;
        //public byte[] readBuff = new byte[BUFFER_SIZE];
        public ByteArray readBuff = new ByteArray(16384);


        public Int32 msgLength = 0;
        public byte[] lenBytes = new byte[sizeof(Int32)];
        public long lastTickTime = 0;
        public IPlayer player;

        public Queue<ByteArray> writeQueue = new Queue<ByteArray>();


        public Conn() {
            //readBuff = new byte[BUFFER_SIZE];
        }
        public void Init(Socket socket) {
            this.socket = socket;
            isUse = true;
            //buffCount = 0;
            lastTickTime = Sys.GetTimeStamp();

        }
        public int BuffRemain() {
            return readBuff.remain;
        }
        public string GetAdress() {
            if (!isUse) { return "无法获取地址"; }
            return socket.RemoteEndPoint.ToString();
        }
        public void Close() {
            if (!isUse) {
                return;
            }
            if (player != null) {
                player.Logout();
                return;
            }
            Console.WriteLine("[断开连接]" + GetAdress());
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            isUse = false;
        }
        //同步发送
        public void Send(ProtocolBase protol) {
            if (!isUse) {
                return;
            }
            byte[] bytes = protol.Encode();
            byte[] length = BitConverter.GetBytes(bytes.Length);
            byte[] sendbuff = length.Concat(bytes).ToArray();
            socket.Send(sendbuff);
        }
        public void SendAsync(ProtocolBase protol) {
            if (socket == null || !socket.Connected) {
                return;
            }
            if (!isUse) {
                return;
            }
            byte[] bytes = protol.Encode();
            byte[] length = BitConverter.GetBytes(bytes.Length);
            byte[] sendBytes = length.Concat(bytes).ToArray();
            ByteArray ba = new ByteArray(sendBytes);
            int count = 0;  //writeQueue的长度
            lock (writeQueue) {
                writeQueue.Enqueue(ba);
                count = writeQueue.Count;
            }
            if (count == 1) {
                socket.BeginSend(sendBytes, 0, sendBytes.Length,
                  0, SendCallback, socket);
            }
        }
        public void SendCallback(IAsyncResult ar) {

            //获取state、EndSend的处理
            Socket socket = (Socket)ar.AsyncState;
            //状态判断
            if (socket == null || !socket.Connected) {
                return;
            }

            //EndSend
            int count = socket.EndSend(ar);
            //获取写入队列第一条数据            
            ByteArray ba;
            lock (writeQueue) {
                ba = writeQueue.First();
            }
            //完整发送
            ba.readIdx += count;
            if (ba.length == 0) {
                lock (writeQueue) {
                    writeQueue.Dequeue();
                    if (writeQueue.Count > 0)
                        ba = writeQueue.First();
                }
            }
            //继续发送
            if (writeQueue.Count > 0) {
                socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
            }
            //正在关闭
            else if (!isUse) {
                socket.Close();
            }
        }

    }
}
