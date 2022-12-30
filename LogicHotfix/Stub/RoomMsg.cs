
using System.Collections.Generic;
[System.Serializable]
public class MsgRoom :MsgBase
{
   public MsgRoom() { protoName = "MsgRoom"; }
    public int CtNum = 0;
    public int TNum = 0;

    public int Camp = 0;// 1代表T 2代表CT 3代表c转t 4代表t转c

}

