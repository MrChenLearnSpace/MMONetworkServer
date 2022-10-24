using System;
using System.Collections.Generic;
using System.Text;
using MMONetworkServer.Core;
//using MMONetworkServer.Logic;
using MMONetworkServer.net;
namespace MMONetworkServer {
    public class MainClass{
        static void Main(string[] args) {
            CodeLoader codeLoader =new CodeLoader();
            codeLoader.Reload();
            DataMgr dataMgr = DataMgr.GetInstance();
            ServNet servNet = new ServNet();
           
            servNet.Start("127.0.0.1", 7777);
            Console.WriteLine(servNet.GetLocalIp());
            while(true) {
                string str = Console.ReadLine();
                switch (str) {
                    case "quit":
                        servNet.Close();
                        break;
                    case "print":
                        servNet.Print();
                        break;
                    case "c" :
                        codeLoader.Reload();
                        break;
             

                }

            }
        }
    }
}
