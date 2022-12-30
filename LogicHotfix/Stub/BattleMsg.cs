using System;
using System.Collections.Generic;
using System.Text;


[Serializable]
public class PlayerInfo {
	public string id = "";  //玩家id
	public int camp = 0;    //阵营
	public int hp = 0;      //生命值
	public float[] cvector3 = new float[3];
	public float[] evector3 = new float[3];

	//public float x = 0;     //位置
	//public float y = 0;
	//public float z = 0;
	//public float ex = 0;    //旋转
	//public float ey = 0;
	//public float ez = 0;
}
public class MsgBattleStart : MsgBase {
	public MsgBattleStart() { protoName = "MsgBattleStart"; }
	public List<PlayerInfo> CtName = new List<PlayerInfo>();
	public List<PlayerInfo> TName = new List<PlayerInfo>();

}
