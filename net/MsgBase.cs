using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using MMONetworkServer.Core;
using MMONetworkServer.net;
public class MsgBase {
    ProtocolBytes protocolBytes;
    public string protoName = "null";
    public MsgBase() { }
    public virtual ProtocolBytes Encode() {
        protocolBytes = new ProtocolBytes();
        protocolBytes.AddString(protoName);
        protocolBytes.AddString(JsonConvert.SerializeObject(this));
        return protocolBytes;
    }
    public virtual MsgBase Decode(ProtocolBytes protocol) {
        int start = 0;
        protoName = protocol.GetString(start, ref start);
        string json = protocol.GetString(start, ref start);
        return (MsgBase)JsonConvert.DeserializeObject(json, CodeLoader.instance.hotfixDictionary[ServNet.instance.HandleDllName].GetType(protoName));

    }
    public virtual string GetName() {
        return protoName;
    }

}

