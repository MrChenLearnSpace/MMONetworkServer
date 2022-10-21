using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace MMONetworkServer.Core {
    public class CodeLoader  {
        AssemblyLoadContext assemblyLoadContext;
        Assembly hotfix;
        Dictionary<string, object> hotfixInstance = new Dictionary<string, object>();
        public static CodeLoader instance ;

        public CodeLoader() {
            
                instance = this;
            
        }
        public static CodeLoader GetInstance() {
            return instance;
        }
        public void Reload() {
            hotfixInstance.Clear();
            assemblyLoadContext?.Unload();
            GC.Collect();
            assemblyLoadContext = new AssemblyLoadContext("ClassLibrary1", true);
            byte[] dllBytes = File.ReadAllBytes(@"F:\project\VSProject\ClassLibrary1\bin\Debug\netcoreapp3.1\ClassLibrary1.dll");//加载dll
            byte[] pdbBytes = File.ReadAllBytes(@"F:\project\VSProject\ClassLibrary1\bin\Debug\netcoreapp3.1\ClassLibrary1.pdb");//加载pdb
            hotfix = assemblyLoadContext.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));
            //object obj = hotfix.CreateInstance("ClassLibrary1.Class1");
            //MethodInfo mm = obj.GetType().GetMethod("add");
            //mm.Invoke(obj, null);

            foreach(Type type in hotfix.GetTypes()) {
                object instance = Activator.CreateInstance(type);
                hotfixInstance.Add(type.FullName, instance);
                Console.WriteLine(type.FullName);
            }
        }
        public object Find(string className) {
            return hotfixInstance[className];
        }
        public void FindFunRun(string className,string funName,object[] objs) {
            MethodInfo mm = Find(className).GetType().GetMethod(funName);
            mm.Invoke(Find(className), objs);
        }

    }
}
