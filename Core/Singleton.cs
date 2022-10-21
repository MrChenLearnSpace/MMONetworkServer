using System;
using System.Collections.Generic;
using System.Text;

namespace MMONetworkServer.Core {
    public class Singleton<T> where T : Singleton<T> {
        public  static T instance;
        private static readonly object syncRoot = new object();
        public static T GetInstance() {
            if (instance == null) {//先判断实例是否存在，不存在再加锁处理
                lock (syncRoot) {
                    if (instance == null) {
                        instance =  (T)(new Singleton<T>());
                    }
                }
            }
            return instance;
        }

    }
}
