using System;
using System.Collections.Generic;
using System.Text;

namespace MMONetworkServer {
    [Serializable]
    public class PlayerData {
        public int score = 0;
        public PlayerData() {
            score = 100;
        }
    }
}
