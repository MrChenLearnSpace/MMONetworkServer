using System;
using System.Collections.Generic;
using System.Text;

namespace MMONetworkServer {
    [Serializable]
    public class PlayerData {
        public float hp = 1;
        public float atk = 0;//attack
        public float dft = 0;//defend
        public float attRange = 1;
        public PlayerData() {
            atk = 100;
        }
    }
}
