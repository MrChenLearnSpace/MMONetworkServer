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

}

