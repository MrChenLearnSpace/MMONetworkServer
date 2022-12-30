using System;

namespace ServerCore {
    //存放一些辅助方法，比如:获取时间戳
    public class Sys {
        public static long GetTimeStamp() {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

    }
}
