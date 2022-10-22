using System;
using System.Collections.Generic;
using System.Text;


public class ProtocolBase {
    //解码器
    public virtual ProtocolBase Decode(byte[] readBuff, int start, int length) {
        return new ProtocolBase();
    }
    //编码器
    public virtual byte[] Encode() {
        return new byte[] { };
    }
    public virtual string GetName() {
        return "Base";
    }
    public virtual string GetDesc() {
        return "";
    }
}

