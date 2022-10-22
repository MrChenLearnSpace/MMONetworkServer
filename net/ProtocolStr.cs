using System;
using System.Collections.Generic;
using System.Text;

public class ProtocolStr : ProtocolBase {
    public string str;
    public override ProtocolBase Decode(byte[] readBuff, int start, int length) {
        ProtocolStr proStr = new ProtocolStr();

        proStr.str = System.Text.Encoding.UTF8.GetString(readBuff, start, length);
        return (ProtocolBase)proStr;
    }

    public override byte[] Encode() {

        return System.Text.Encoding.UTF8.GetBytes(str);
    }
    public override string GetName() {
        if (str.Length == 0) return "";

        return str.Split(',')[0];

    }
    public override string GetDesc() {
        return str;
    }
}

