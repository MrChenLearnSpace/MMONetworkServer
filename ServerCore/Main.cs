using ServerCore.net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore {
    public class MainClass {
        static void Main(string[] args) {
            string dllName = "CSLogicHotfix";
            string dllPath = "F:\\project\\VSProject\\MMONetworkServer\\CSLogicHotfix\\bin\\Debug\\net6.0\\CSLogicHotfix";
            CodeLoader code = new CodeLoader();
            code.Reload(dllName,dllPath);
            code.FindFunRun(dllName, dllName + ".LogicManager", "Init", new object[] { });
            while (true) {
                string str = Console.ReadLine();
                switch (str) {
                    case "c" :
                        code.Reload(dllName, dllPath);
                        break;
                    default:
                        string FunStr = "Msg" + str;
                        code.FindFunRun(dllName, dllName + ".LogicManager", FunStr, new object[] { });
                        break;


                }
            }
        }
    }
}

