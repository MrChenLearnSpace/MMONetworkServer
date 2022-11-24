﻿using System;
using System.Collections.Generic;
using System.Text;
using MMONetworkServer.Core;
//using ServerLoginHotfix;
using MMONetworkServer.net;
namespace MMONetworkServer {
    public class MainClass{
        static void Main(string[] args) {
            string dllName = @"ServerLoginHotfix";
            string dllpath = @"F:\project\VSProject\ServerLoginHotfix\bin\Debug\netcoreapp3.1\ServerLoginHotfix";
            CodeLoader codeLoader =new CodeLoader();
            codeLoader.Reload(dllName,dllpath);
            DataMgr dataMgr = DataMgr.GetInstance();
            ServNet servNet = new ServNet();
           
            servNet.Start("127.0.0.1", 6667);
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
                        codeLoader.Reload(dllName, dllpath);
                        break;
                    default:
                        string FunStr = "Msg" + str;
                        codeLoader.FindFunRun(dllName, "ServerLoginHotfix.LogicManager",FunStr,new object[] { });
                        break;
                            

                }

            }
        }
    }
}
