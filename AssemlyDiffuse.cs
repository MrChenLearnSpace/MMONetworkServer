using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using MMONetworkServer.Core;
namespace MMONetworkServer {
    public class AssemlyDiffuse : SerializationBinder {
        public override Type BindToType(string assemblyName, string typeName) {
            Console.WriteLine("assemblyName: " + assemblyName + "  typeName : " + typeName);
            if(assemblyName == "ServerLoginHotfix, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") {
                return CodeLoader.instance.hotfix.GetType();
            }
            return Type.GetType(typeName, false);
          
        }
    }
}
