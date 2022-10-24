using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace MMONetworkServer.Core {
    public class CodeLoader  {
        AssemblyLoadContext assemblyLoadContext;
        public Assembly hotfix;
        Dictionary<string, object> hotfixInstance = new Dictionary<string, object>();
        public static CodeLoader instance ;

        public CodeLoader() {
            
                instance = this;
            
        }
        public static CodeLoader GetInstance() {
            return instance;
        }
        public void Reload( string HotfixPath = @"F:\project\VSProject\ServerLoginHotfix\bin\Debug\netcoreapp3.1\ServerLoginHotfix") {
            try {
                hotfixInstance.Clear();
                assemblyLoadContext?.Unload();
                GC.Collect();
                assemblyLoadContext = new AssemblyLoadContext("ServerLoginHotfix", true);
                byte[] dllBytes = File.ReadAllBytes(HotfixPath + ".dll");//加载dll
                byte[] pdbBytes = File.ReadAllBytes(HotfixPath + ".pdb");//加载pdb
                hotfix = assemblyLoadContext.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));
                //object obj = hotfix.CreateInstance("ClassLibrary1.Class1");
                
                //MethodInfo mm = obj.GetType().GetMethod("add");
                //mm.Invoke(obj, null);

                foreach (Type type in hotfix.GetTypes()) {
                    object instance = Activator.CreateInstance(type);
                    hotfixInstance.Add(type.FullName, instance);
                    Console.WriteLine(type.FullName);
                }
            }
            catch(Exception e) {
                Console.WriteLine("[MMONetworkServer.Core.CodeLoader]"+ e.ToString());
            }
        }
        public object Find(string className) {
            return hotfixInstance[className];
        }
        public void FindFunRun(string className, string funName, object[] objs) {

            MethodInfo mm = Find(className).GetType().GetMethod(funName);
            if (mm != null)
                mm.Invoke(Find(className), objs);
            else
                Console.WriteLine("className: " + className + " funName: " + funName  +"没找到");

        }

    }
}
